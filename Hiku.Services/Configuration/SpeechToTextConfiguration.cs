using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Hiku.Services.Configuration
{
    [DisplayName("SpeechToText")]
    public class SpeechToTextConfiguration
    {
        public string EndpointID { get; set; }
        public string ServiceRegion { get; set; }
        public string SubscriptionKey { get; set; }
        public string DefaultLanguage { get; set; }
    }
}
