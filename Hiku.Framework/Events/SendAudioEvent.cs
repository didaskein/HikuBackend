using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hiku.Framework.Events
{
    public class SendAudioEvent : IntegrationEvent
    {
        private const string EventName = "SendAudioEvent";

        public SendAudioEvent(Guid audioToken, string waveFileName, DateTimeOffset eventDate) : base(EventName, DomainEventAuthor.System, audioToken, null)
        {
            AudioToken = audioToken;
            WaveFileName = waveFileName;
            EventDate = eventDate;
        }

        [JsonProperty("audioToken")]
        public Guid AudioToken { get; set; }

        [JsonProperty("waveFileName")]
        public String WaveFileName { get; set; }

        [JsonProperty("eventDate")]
        public DateTimeOffset EventDate { get; set; }
    }
}
