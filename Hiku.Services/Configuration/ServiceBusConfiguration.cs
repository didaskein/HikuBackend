using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Hiku.Services.Configuration
{
    [DisplayName("ServiceBus")]
    public class ServiceBusConfiguration
    {
        public string ConnectionString { get; set; }
        public string AudioQueueName { get; set; }
    }
}


