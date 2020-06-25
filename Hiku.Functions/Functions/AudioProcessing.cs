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
using Hiku.Framework.Events;


namespace Hiku.Functions
{
    public class AudioProcessing
    {
        private readonly IConfigurationHelper _configurationHelper;
        private readonly AudioService _audioService;

        private readonly bool _onAzureFlag;

        public AudioProcessing(IConfigurationHelper configurationHelper, AudioService audioService)
        {
            _configurationHelper = configurationHelper;
            _audioService = audioService;

            _onAzureFlag = _configurationHelper.GetBoolSetting("ASPNETCORE_ONAZURE", false);
        }

        [FunctionName("SendAudio")]
        public async Task SendReminderAsync(
        ILogger logger,
        [ServiceBusTrigger("%Queue-Audio%", Connection = "ServiceBusListenerConnectionString")] SendAudioEvent audioEvent)
        {
            DateTimeOffset received = DateTimeOffset.UtcNow;
            logger.LogInformation($"C# ServiceBus Queue trigger function executed at: {received}.");
            try
            {
                if (audioEvent != null)
                {
                    var goodAudio = await _audioService.SaveAudioandSTTAsync(audioEvent.AudioToken, audioEvent.WaveFileName).ConfigureAwait(false);
                    logger.LogInformation($"Processing audio file {audioEvent.AudioToken}, state {goodAudio} sent at {audioEvent.EventDate} received at {received} processed at {DateTimeOffset.UtcNow}");
                }
                else 
                {
                    logger.LogWarning("Empty audioEvent parameters from the queue");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occured while processing audio file");
            }
        }     
    }
}
