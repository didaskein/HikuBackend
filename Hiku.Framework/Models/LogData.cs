using System;
using System.Collections.Generic;
using System.Text;

namespace Hiku.Framework.Models
{
    public class LogData
    {
        public int rssi { get; set; }
        public string agent_url { get; set; }
        public string os_version { get; set; }
        public string wakeup_reason { get; set; }
        public int connectTime { get; set; }
        public string fw_version { get; set; }
        public string ssid { get; set; }
        public int boot_time { get; set; }
        public string dc_reason { get; set; }
        public int sleep_duration { get; set; }
    }

}
