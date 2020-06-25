using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Hiku.Services.Configuration;
using Hiku.Framework.Configuration;
using Hiku.Services;
using Hiku.Framework;
using Hiku.Framework.Enums;
using NAudio.Wave;

namespace Hiku.Cmd
{
    class Program
    {
        private static IServiceProvider _serviceProvider;
        private static IConfigurationRoot _configuration;

        static async Task Main(EnumEnvironment environment, bool console)
        {
            Console.WriteLine("Hiku Cmd Start");

            var builder = new ConfigurationBuilder()
                          .SetBasePath(Directory.GetCurrentDirectory())
                          .AddJsonFile("appsettings.json");
            IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, true).Build();
            _configuration = builder.Build();

            Options opts = new Options()
            {
                Environment = environment,
                Console = console
            };

            RegisterServices(opts);
            await RunApplicationAsync(opts);

            DisposeServices();
        }
        
        #pragma warning disable CS1998
        private static async Task RunApplicationAsync(Options opts)
        {
            try
            {
                var securityService = _serviceProvider.GetService<ISecurityService>();
                string token = securityService.GetToken();
                //await securityService.RunDevOpsBuildAsync();

                //Token
                //StringHelper sh = new StringHelper();
                //string body = "time=2020-04-17%2013%3A13%3A12.000000&sig=8be5714fa4d50c6f4e3305dfbea49331711c44e3f55540cecda873b37ae09cc2&agentUrl=https%3A%2F%2Fagent.electricimp.com%2FRf6ii2XMsbig&token=20000c2a6909f5a2&app_id=e3xxxxxxxxxxxxxxxxxxxx44&agentAudioRate=8000";
                //body = "serialNumber=20000c2a6909f5a2&app_id=e3xxxxxxxxxxxxxxxxxxxx44&logData=%7B%20%22rssi%22%3A%20-47%2C%20%22agent_url%22%3A%20%22https%3A%2F%2Fagent.electricimp.com%2FRf6ii2XMsbig%22%2C%20%22os_version%22%3A%20%22882c579%20-%20release-40.11%20-%20Wed%20Oct%20%202%2010%3A41%3A02%202019%20-%20production%22%2C%20%22wakeup_reason%22%3A%20%22accelerometerbutton%22%2C%20%22connectTime%22%3A%2047047%2C%20%22fw_version%22%3A%20%221.3.17%22%2C%20%22ssid%22%3A%20%22WifiHome%22%2C%20%22boot_time%22%3A%200%2C%20%22dc_reason%22%3A%20%22No%20Disconnects%22%2C%20%22sleep_duration%22%3A%2032%20%7D&time=2020-04-17%2012%3A22%3A14.000000&sig=a8b788b38c2886707fd5743e1c934901afea4162059c333963a5494e8e435311";
                //var dic = sh.GetParameters(body);
                //bool res = securityService.ValidateToken(dic);

                //using (AudioFileReader reader = new AudioFileReader(@"C:\Temps\04da86d0-e9db-49fe-adc7-746f83583dc9.wav"))
                //{
                //    TimeSpan duration = reader.GetSilenceDuration(AudioFileReaderExt.SilenceLocation.Start);
                //    Console.WriteLine(duration.TotalMilliseconds);
                //}


            }
            catch (Exception ex )
            {
                string err = ex.Message;
                throw;
            }
        


        }


        private static void RegisterServices(Options opts)
        {
            var services = new ServiceCollection();

            //Parameters from Cmd
            services.AddSingleton<Options>(ctx =>
            {
                return opts;
            });

            //Security : Configuration & Service
            services.AddSingleton<SecurityConfiguration>(ctx =>
            {
                return new ConfigurationService<SecurityConfiguration>(_configuration, new SecurityConfiguration()).ConfigurationOptions;
            });
            services.AddSingleton<ISecurityService, SecurityService>();

            _serviceProvider = services.BuildServiceProvider();
        }

        private static void DisposeServices()
        {
            if (_serviceProvider == null)
            {
                return;
            }
            if (_serviceProvider is IDisposable)
            {
                ((IDisposable)_serviceProvider).Dispose();
            }
        }

    

    }
}
