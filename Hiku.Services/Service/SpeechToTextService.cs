using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Hiku.Framework;
using Hiku.Framework.Configuration;
using Hiku.Services.Configuration;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using NAudio.Wave;

namespace Hiku.Services
{
    public class SpeechToTextService
    {
        private readonly SpeechToTextConfiguration _speechToTextConfiguration;
        private readonly IHttpClientFactory _clientFactory;

        private readonly string _serviceRegion;     
        private readonly string _subscriptionKey;

        private readonly string _defaultLanguage;
        private readonly string _endpointId;
        private readonly TelemetryClient _telemetryClient;

        public SpeechToTextService(SpeechToTextConfiguration speechToTextConfiguration, 
                                   IHttpClientFactory clientFactory,
                                   TelemetryClient telemetryClient)
        {
            _speechToTextConfiguration = speechToTextConfiguration;
            _clientFactory = clientFactory;

            _serviceRegion = _speechToTextConfiguration.ServiceRegion;
            _subscriptionKey = _speechToTextConfiguration.SubscriptionKey;
            _defaultLanguage = _speechToTextConfiguration.DefaultLanguage;
            _endpointId = _speechToTextConfiguration.EndpointID;
            _telemetryClient = telemetryClient;
        }

        public async Task<string> RecognizeSpeechAsync(Stream audioStream)
        {
            string speechtoText = string.Empty;

            //Region : https://aka.ms/speech/sdkregion
            var config = SpeechConfig.FromSubscription(_subscriptionKey, _serviceRegion);
            config.EndpointId = _endpointId;

            using (var audioInput = AudioHelper.OpenWavFile(audioStream))
            using (var recognizer = new SpeechRecognizer(config, _defaultLanguage, audioInput))
            {
                speechtoText = await ProcessResult(recognizer);
            }

            return speechtoText;
        }

        public async Task<string> RecognizeSpeechAsync(string fileName)
        {
            string speechtoText = string.Empty;
            var config = SpeechConfig.FromSubscription(_subscriptionKey, _serviceRegion);
            config.EndpointId = _endpointId;

            using (var audioInput = AudioConfig.FromWavFileInput(fileName))
            using (var recognizer = new SpeechRecognizer(config, _defaultLanguage, audioInput))
            {
                speechtoText = await ProcessResult(recognizer);
            }

            return speechtoText;
        }

        private async Task<string> ProcessResult(SpeechRecognizer recognizer)
        {
            //https://docs.microsoft.com/bs-latn-ba/azure/azure-monitor/app/custom-operations-tracking#outgoing-dependencies-tracking
            using (var operation = _telemetryClient.StartOperation<DependencyTelemetry>("SpeechToTextService"))
            {
                string speechtoText = string.Empty;

                Console.WriteLine("Recognizing first result...");
                var result = await recognizer.RecognizeOnceAsync().ConfigureAwait(false);

                switch (result.Reason)
                {
                    case ResultReason.RecognizedSpeech:
                        Console.WriteLine($"We recognized: {result.Text}");
                        speechtoText = result.Text.Trim();
                        break;
                    case ResultReason.NoMatch:
                        Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                        break;
                    case ResultReason.Canceled:
                        var cancellation = CancellationDetails.FromResult(result);
                        Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                        if (cancellation.Reason == CancellationReason.Error)
                        {
                            Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                            Console.WriteLine($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                            Console.WriteLine($"CANCELED: Did you update the subscription info?");
                        }
                        break;
                }

                return speechtoText;
            } 
        }
    }
}
