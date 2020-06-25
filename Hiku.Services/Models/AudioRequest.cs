using System;
using System.Collections.Generic;

namespace Hiku.Services.Models
{
    public partial class AudioRequest
    {
        public Guid Id { get; set; }
        public string State { get; set; }
        public DateTimeOffset TimestampRecordStarted { get; set; }
        public DateTimeOffset? TimestampRecordFinished { get; set; }
        public DateTimeOffset? TimestampSpeechToTextFinished { get; set; }
        public string AudioFilePath { get; set; }
        public string Sentence { get; set; }
        public string AppId { get; set; }
        public string DeviceId { get; set; }
    }
}
