using Hiku.Framework;
using Hiku.Framework.Configuration;
using Hiku.Services;
using Hiku.Services.Configuration;
using Hiku.Services.Infrastructure.Repositories;
using Hiku.Services.Infrastructure.Repositories.Sql;
using Hiku.Services.Service.BlobStorage;
using Hiku.Services.Service.ServiceBus;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

[assembly: FunctionsStartup(typeof(Hiku.Functions.Startup))]
namespace Hiku.Functions
{
    internal class Startup : FunctionsStartup
    {
        private bool _onAzureFlag;
        private IConfiguration _configuration;

        public Startup()
        {
            _onAzureFlag = string.Equals(
                Environment.GetEnvironmentVariable("ASPNETCORE_ONAZURE"),
                "true",
                StringComparison.InvariantCultureIgnoreCase);
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            var currentRootLocation = default(string);

            if (_onAzureFlag)
            {
                currentRootLocation = $"{Environment.GetEnvironmentVariable("HOME")}/site/wwwroot";
            }
            else
            {
                currentRootLocation = Directory.GetCurrentDirectory();
            }

            _configuration = new ConfigurationBuilder()
                .SetBasePath(currentRootLocation)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            builder.Services.AddSingleton(_configuration);
            builder.Services.AddSingleton<IConfigurationHelper>(
                          s => new ConfigurationHelper(_configuration));

            string instrumentationKey = _configuration["APPINSIGHTS_INSTRUMENTATIONKEY"];

            builder.Services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddApplicationInsights(instrumentationKey);
                loggingBuilder.AddAzureWebAppDiagnostics();
            });

            ConfigureServices(builder.Services);
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddMemoryCache();

            ConfigureSQLServices(services);

            //Security : Configuration & Service
            services.AddSingleton<SecurityConfiguration>(ctx =>
            {
                return new ConfigurationService<SecurityConfiguration>(_configuration, new SecurityConfiguration()).ConfigurationOptions;
            });            
            services.AddSingleton<ISecurityService, SecurityService>();

            //SpeechToText : Configuration & Service
            services.AddSingleton<SpeechToTextConfiguration>(ctx =>
            {
                return new ConfigurationService<SpeechToTextConfiguration>(_configuration, new SpeechToTextConfiguration()).ConfigurationOptions;
            });
            services.AddSingleton<SpeechToTextService>();

            services.AddSingleton<StringHelper>();
            services.AddScoped<AudioService>();
            services.AddScoped<HikuService>();

            //BlobStorage : Configuration & Service
            services.AddSingleton<BlobStorageConfiguration>(ctx =>
            {
                return new ConfigurationService<BlobStorageConfiguration>(_configuration, new BlobStorageConfiguration()).ConfigurationOptions;
            });
            services.AddSingleton<IAudioBlobStorage>(ctx =>
            {
                BlobStorageConfiguration config = ctx.GetService<BlobStorageConfiguration>();
                return new BlobStorage(config.ConnectionString, config.AudioBlobContainerName);
            });

            //ServiceBus: Configuration & Service
            services.AddSingleton<ServiceBusConfiguration>(ctx =>
            {
                return new ConfigurationService<ServiceBusConfiguration>(_configuration, new ServiceBusConfiguration()).ConfigurationOptions;
            });
              


            //Pushbullet
            //services.AddSingleton<PushbulletSettings>(ctx =>
            //{
            //    bool tryFallBackOnPushBullet = false;
            //    bool.TryParse(_configuration["Pushbullet:TryFallBackOnPushBullet"], out tryFallBackOnPushBullet);
            //    return new PushbulletSettings(tryFallBackOnPushBullet, _configuration["Pushbullet:PushBulletAccessToken"]);
            //});
            //services.AddScoped<IPushBulletNotificationService, PushBulletNotificationService>();
        }

        private void ConfigureSQLServices(IServiceCollection services)
        {
            //SQL
            services.AddDbContext<HikuDbContext>(optionsBuilder =>
            {
                optionsBuilder.UseSqlServer(
                    _configuration["HikuDbConnectionString"],
                    options =>
                    {
                        options.EnableRetryOnFailure();
                    });
            });


            //Repositories
            services.AddScoped<UserRepository>();
            services.AddScoped<BarcodeRequestRepository>();
            services.AddScoped<AudioRequestRepository>();
            services.AddScoped<BatteryRepository>();
            services.AddScoped<DeviceLogRepository>();


            //Factories
            services.AddScoped<Func<UserRepository>>(s => s.GetService<UserRepository>);
            services.AddScoped<Func<BarcodeRequestRepository>>(s => s.GetService<BarcodeRequestRepository>);
            services.AddScoped<Func<AudioRequestRepository>>(s => s.GetService<AudioRequestRepository>);
            services.AddScoped<Func<BatteryRepository>>(s => s.GetService<BatteryRepository>);
            services.AddScoped<Func<DeviceLogRepository>>(s => s.GetService<DeviceLogRepository>);
        }

    }
}
