using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Hiku.Framework;
using Hiku.Framework.Configuration;
using Hiku.Framework.Events;
using Hiku.Framework.Tools;
using Hiku.Services.Configuration;
using Hiku.Services.Service.BlobStorage;
using Hiku.Services.Service.ServiceBus;
using Microsoft.ApplicationInsights;
using Microsoft.Azure.ServiceBus;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using NAudio.MediaFoundation;
using NAudio.Utils;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Hiku.Services
{
    public class AudioService
    {
        private readonly IConfigurationHelper _configurationHelper;
        private readonly IHttpClientFactory _clientFactory;
        private readonly SpeechToTextService _speechToTextService;
        private readonly IAudioBlobStorage _audioBlobStorage;
        private readonly ServiceBusConfiguration _serviceBusConfiguration;
        private readonly HikuService _hikuService;
        private readonly TelemetryClient _telemetryClient;

        private readonly QueueClient _queueClient;
        private readonly ServiceBusMessageFormatter _formatter;

        public AudioService(IConfigurationHelper configurationHelper, 
                            IHttpClientFactory clientFactory, 
                            SpeechToTextService speechToTextService,
                            IAudioBlobStorage audioBlobStorage,
                            ServiceBusConfiguration serviceBusConfiguration,
                            HikuService hikuService,
                            TelemetryClient telemetryClient)
        {
            _configurationHelper = configurationHelper;
            _clientFactory = clientFactory;
            _speechToTextService = speechToTextService;
            _audioBlobStorage = audioBlobStorage;
            _serviceBusConfiguration = serviceBusConfiguration;
            _hikuService = hikuService;
            _telemetryClient = telemetryClient;

            _queueClient = new QueueClient(serviceBusConfiguration.ConnectionString, serviceBusConfiguration.AudioQueueName);
            _formatter = new ServiceBusMessageFormatter();
        }

        public async Task<bool> GetFileandSTTAsync(string audioToken, string agentBaseUri)
        {
            Guid audioTokenId = new Guid(audioToken);
            string agenturi = $"{agentBaseUri}/audio/{audioToken}";
            var request = new HttpRequestMessage(HttpMethod.Get, agenturi);

            var client = _clientFactory.CreateClient();
            var response = await client.SendAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var reply = await GetAudioStreaminWavAndSTTAsync(audioTokenId, response).ConfigureAwait(false);
                return reply; 
            }

            return false;

        }

        public async Task<bool> GetAudioStreaminWavAndSTTAsync(Guid audioToken, HttpResponseMessage response)
        {
            bool goodAudio = false;

            string speechtoText = string.Empty;
            string wavFile = $"{audioToken}.wav";

            using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
            {
                var waveFormat = WaveFormat.CreateALawFormat(8000, 1);
                using (var reader = new RawSourceWaveStream(responseStream, waveFormat))
                using (WaveStream convertedStream = WaveFormatConversionStream.CreatePcmStream(reader))
                {
                    double duration = reader.TotalTime.TotalMilliseconds;
                    if (duration >= 400) //Need at least 0,4 second of audio
                    {
                        using (WaveStreamReader waveStreamReader = new WaveStreamReader(convertedStream))
                        {
                            TimeSpan tSilence = waveStreamReader.GetSilenceDuration();
                            double silence = tSilence.TotalMilliseconds;
                            double pct = silence * 100 / duration;

                            if (pct <= 80)
                            {
                                goodAudio = true;

                                //Save Wav to Blob
                                using (var outputStream = new MemoryStream())
                                {
                                    outputStream.Position = 0;
                                    WaveFileWriter.WriteWavFileToStream(outputStream, convertedStream); 
                                    outputStream.Position = 0;
                                    await _audioBlobStorage.CreateBlockBlobAsync(wavFile, outputStream, "audio/wav");
                                    //speechtoText = await _speechToTextService.RecognizeSpeechAsync(outputStream).ConfigureAwait(false);
                                    //await _hikuService.SaveAudioRequestAsync(audioToken, speechtoText, wavFile).ConfigureAwait(false);
                                }


                                //Send Message to Queue
                                var message = _formatter.GetBrokeredMessage(new SendAudioEvent(audioToken, wavFile, DateTimeOffset.UtcNow));
                                await _queueClient.SendAsync(message);                        
                            }
                        }
                    }
                }
            }

            return goodAudio;
        }

        public async Task<bool> SaveAudioandSTTAsync(Guid audioToken, string wavFile)
        {
            bool goodAudio = false;

            Stream audioStream = await _audioBlobStorage.GetBlockBlobDataAsStreamAsync(wavFile);
            string speechtoText = await _speechToTextService.RecognizeSpeechAsync(audioStream).ConfigureAwait(false);
            
            //Remove some characters
            if (!string.IsNullOrWhiteSpace(speechtoText)) speechtoText = speechtoText.Replace("?", "");

            bool haveBeenSaved = await _hikuService.SaveAudioRequestAsync(audioToken, speechtoText, wavFile).ConfigureAwait(false);

            if (haveBeenSaved && !string.IsNullOrWhiteSpace(speechtoText)) goodAudio = true;

            return goodAudio;
        }

        public async Task<string> GetAudioFileAndSaveonDiskAsync(string audioToken, HttpResponseMessage response)
        {
            string alawFile = $"{audioToken}.alaw";
            string wavFile = $"{audioToken}.wav";
            using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
            using (var stream = new FileStream(alawFile, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await responseStream.CopyToAsync(stream).ConfigureAwait(false);
            }

            FileStream fileStream = new FileStream(alawFile, FileMode.Open);

            var waveFormat = WaveFormat.CreateALawFormat(8000, 1);
            var reader = new RawSourceWaveStream(fileStream, waveFormat);
            using (WaveStream convertedStream = WaveFormatConversionStream.CreatePcmStream(reader))
            {
                WaveFileWriter.CreateWaveFile(wavFile, convertedStream);
            }
            fileStream.Close();

            return wavFile;
        }

        private async Task<string> TestConversionAlawToWav()
        {
            string basePath = @"C:\_DDK\Hiku\Data\AudioSamples";
            string fileName = $@"{basePath}\croissant.alaw";
            string fileNamOutput = fileName.Replace(".alaw", ".wav");
            string fileTest = $@"{basePath}\whatstheweatherlike.wav";

            FileStream fileStream = new FileStream(fileName, FileMode.Open);

            var waveFormat = WaveFormat.CreateALawFormat(8000, 1);
            var reader = new RawSourceWaveStream(fileStream, waveFormat);
            using (WaveStream convertedStream = WaveFormatConversionStream.CreatePcmStream(reader))
            {
                WaveFileWriter.CreateWaveFile(fileNamOutput, convertedStream);
            }
            fileStream.Close();

            return await _speechToTextService.RecognizeSpeechAsync(fileNamOutput).ConfigureAwait(false);

        }

       
    }
}
