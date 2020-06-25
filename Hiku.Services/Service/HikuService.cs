using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Hiku.Framework;
using Hiku.Framework.Configuration;
using Hiku.Framework.Enums;
using Hiku.Services.Infrastructure.Repositories;
using Hiku.Services.Models;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using NAudio.MediaFoundation;
using NAudio.Wave;

namespace Hiku.Services
{
    public class HikuService
    {
        private readonly IConfigurationHelper _configurationHelper;
        private readonly Func<BarcodeRequestRepository> _barcodeRequestRepository;
        private readonly Func<AudioRequestRepository> _audioRequestRepositoryFactory;
        private readonly Func<BatteryRepository> _batteryRepository;
        private readonly Func<DeviceLogRepository> _deviceLogRepository;
        

        public HikuService(IConfigurationHelper configurationHelper,
                           Func<BarcodeRequestRepository> barcodeRequestRepository,
                           Func<AudioRequestRepository> audioRequestRepositoryFactory,
                           Func<BatteryRepository> batteryRepository,
                           Func<DeviceLogRepository> deviceLogRepository)
        {
            _configurationHelper = configurationHelper;
            _barcodeRequestRepository = barcodeRequestRepository;
            _audioRequestRepositoryFactory = audioRequestRepositoryFactory;
            _batteryRepository = batteryRepository;
            _deviceLogRepository = deviceLogRepository;
        }

        public async Task<bool> SaveBarcodeRequestAsync(Dictionary<EnumDataField, string> parameters)
        {
            bool res = false;

            using (var repository = _barcodeRequestRepository())
            {
                BarcodeRequest br = new BarcodeRequest()
                {
                    Id = Guid.NewGuid(),
                    State = EnumBarcodeRequestState.FlashFinished.ToString(),
                    TimestampFlashFinished = DateTimeOffset.UtcNow,
                    Barcode = parameters[EnumDataField.ean],
                    AppId = parameters[EnumDataField.app_id],
                    DeviceId = parameters[EnumDataField.token]
                };
                res = await repository.CreateAsync(br).ConfigureAwait(false);
            }

            return res;
        }

        public async Task<bool> SaveAudioRequestToProcessAsync(string audioToken)
        {
            bool res = false;
            Guid id = new Guid(audioToken);

            using (var repository = _audioRequestRepositoryFactory())
            {
                AudioRequest ar = await repository.GetAsync(id);
                ar.State = EnumAudioRequestState.RecordFinished.ToString();
                ar.TimestampRecordFinished = DateTimeOffset.UtcNow;
               
                res = await repository.UpdateAsync(ar).ConfigureAwait(false);
            }

            return res;
        }

        public async Task<bool> SaveAudioRequestAsync(Guid audioToken, string sentence, string fileName)
        {
            bool res = false;

            using (var repository = _audioRequestRepositoryFactory())
            {
                AudioRequest ar = await repository.GetAsync(audioToken);
                ar.State = EnumAudioRequestState.SpeechToTextFinished.ToString();
                ar.TimestampSpeechToTextFinished = DateTimeOffset.UtcNow;
                ar.AudioFilePath = fileName;
                ar.Sentence = sentence;
                //ar.AppId = parameters[EnumDataField.app_id];
                //ar.DeviceId = parameters[EnumDataField.token];
                
                res = await repository.UpdateAsync(ar).ConfigureAwait(false);
            }

            return res;
        }

        public async Task<bool> SaveBatteryDeviceAsync(Dictionary<EnumDataField, string> parameters)
        {
            bool res = false;

            int batteryLevel = 0;
            if(int.TryParse(parameters[EnumDataField.batteryLevel], out batteryLevel))
            {
                using (var repository = _batteryRepository())
                {
                    Battery b = new Battery()
                    {
                        Id = Guid.NewGuid(),
                        BatteryLevelPercentage = batteryLevel,
                        Timestamp = DateTimeOffset.UtcNow,
                        AppId = parameters[EnumDataField.app_id],
                        DeviceId = parameters[EnumDataField.token]
                    };
                    res = await repository.CreateAsync(b).ConfigureAwait(false);
                }

            }

            return res;
        }

        public async Task<bool> SaveDeviceLogAsync(Dictionary<EnumDataField, string> parameters)
        {
            bool res = false;

            using (var repository = _deviceLogRepository())
            {
                DeviceLog log = new DeviceLog()
                {
                    Id = Guid.NewGuid(),
                    Data = parameters[EnumDataField.logData],
                    Timestamp = DateTimeOffset.UtcNow,
                    AppId = parameters[EnumDataField.app_id],  
                    DeviceId = parameters[EnumDataField.serialNumber]
                };
                res = await repository.CreateAsync(log).ConfigureAwait(false);
            }

            return res;
        }

        public async Task<bool> SaveAudioStartAsync(Dictionary<EnumDataField, string> parameters, Guid audioToken)
        {
            bool res = false;

            using (var repository = _audioRequestRepositoryFactory())
            {
                AudioRequest ar = new AudioRequest()
                {
                    Id = audioToken,
                    State = EnumAudioRequestState.RecordStarted.ToString(),
                    TimestampRecordStarted = DateTimeOffset.UtcNow,
                    AppId = parameters[EnumDataField.app_id],
                    DeviceId = parameters[EnumDataField.token]
                };
                res = await repository.CreateAsync(ar).ConfigureAwait(false);
            }

            return res;
        }
    }
}
