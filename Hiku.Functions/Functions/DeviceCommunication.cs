using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Hiku.Services.Models;
using Microsoft.Extensions.Configuration;
using Hiku.Framework;
using Hiku.Services;
using Hiku.Framework.Configuration;
using System.Net.Http;
using Hiku.Framework.Enums;

namespace Hiku.Functions
{
    public class DeviceCommunication
    {
        private readonly IConfigurationHelper _configurationHelper;
        private readonly ISecurityService _securityService;
        private readonly StringHelper _stringHelper;
        private readonly IHttpClientFactory _clientFactory;
        private readonly AudioService _audioService;
        private readonly HikuService _hikuService;
        private readonly bool _onAzureFlag;

        public DeviceCommunication(IConfigurationHelper configurationHelper, ISecurityService securityService, StringHelper stringHelper, IHttpClientFactory clientFactory, AudioService audioService, HikuService hikuService)
        {
            _configurationHelper = configurationHelper;
            _securityService = securityService;
            _stringHelper = stringHelper;
            _clientFactory = clientFactory;
            _audioService = audioService;
            _hikuService = hikuService;

            _onAzureFlag = _configurationHelper.GetBoolSetting("ASPNETCORE_ONAZURE", false);
        }


        [FunctionName("BarcodeOrAudio")]
        public async Task<IActionResult> AddBarcodeOrAudio([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger logger)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync().ConfigureAwait(false);
            logger.LogInformation($"BarcodeOrAudio : {requestBody}");

            var parameters = _stringHelper.GetParameters(requestBody);
            bool secuOk = _securityService.ValidateToken(parameters);
            if (!secuOk) return (ActionResult)new ForbidResult();

            bool eanOrAudio = false;

            if (parameters.ContainsKey(EnumDataField.ean) && !string.IsNullOrWhiteSpace(parameters[EnumDataField.ean]))
            {
                await _hikuService.SaveBarcodeRequestAsync(parameters).ConfigureAwait(false);
                eanOrAudio = true;
            }
            else if (parameters.ContainsKey(EnumDataField.audioToken) && !string.IsNullOrWhiteSpace(parameters[EnumDataField.audioToken]))
            {
                //goodAudio=True : If > 400ms & >80% of audio is not just silence
                var goodAudio = await _audioService.GetFileandSTTAsync(parameters[EnumDataField.audioToken], parameters[EnumDataField.agentUrl]).ConfigureAwait(false);
                if (goodAudio)
                {
                    eanOrAudio = true;
                    await _hikuService.SaveAudioRequestToProcessAsync(parameters[EnumDataField.audioToken]).ConfigureAwait(false);
                }
            }

            if (eanOrAudio)
            {
                DeviceAnswer da = new DeviceAnswer();
                return (ActionResult)new OkObjectResult(da);
            }
            else
            {
                DeviceAnswer da = new DeviceAnswer();
                da.response.data.status = "ko";
                return (ActionResult)new BadRequestObjectResult(da);
            }
        }



        [FunctionName("BatteryDevice")]
        public async Task<IActionResult> BatteryDevice([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "BatteryDevice/{name}")] HttpRequest req, string name, ILogger logger)
        {
            string q = req.Query["name"];
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync().ConfigureAwait(false);
            logger.LogInformation($"BatteryDevice : {requestBody}");
            var parameters = _stringHelper.GetParameters(requestBody);
            bool secuOk = _securityService.ValidateToken(parameters);
            if (!secuOk) return (ActionResult)new ForbidResult();

            await _hikuService.SaveBatteryDeviceAsync(parameters).ConfigureAwait(false);

            //TODO : Add Notification to the user if Battery <10% (Store the fact that the notification have been already send)

            DeviceAnswer da = new DeviceAnswer();
            return (ActionResult)new OkObjectResult(da);
        }


        [FunctionName("LogDevice")]
        public async Task<IActionResult> LogDevice([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger logger)
        {
            string name = req.Query["name"];
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync().ConfigureAwait(false);
            logger.LogInformation($"LogDevice : {requestBody}");
            var parameters = _stringHelper.GetParameters(requestBody);
            bool secuOk = _securityService.ValidateToken(parameters);
            if (!secuOk) return (ActionResult)new ForbidResult();

            await _hikuService.SaveDeviceLogAsync(parameters).ConfigureAwait(false);

            DeviceAnswer da = new DeviceAnswer();
            return (ActionResult)new OkObjectResult(da);
        }


        [FunctionName("AudioStart")]
        public async Task<IActionResult> AudioStart([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger logger)
        {
            string name = req.Query["name"];
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync().ConfigureAwait(false);
            logger.LogInformation($"AudioStart : {requestBody}");
            var parameters = _stringHelper.GetParameters(requestBody);
            bool secuOk = _securityService.ValidateToken(parameters);
            if (!secuOk) return (ActionResult)new ForbidResult();

            Guid audioToken = Guid.NewGuid();

            await _hikuService.SaveAudioStartAsync(parameters, audioToken).ConfigureAwait(false);

            DeviceAnswer da = new DeviceAnswer();
            da.response.data.audioToken = audioToken.ToString();
            return (ActionResult)new OkObjectResult(da);
        }
    }
}