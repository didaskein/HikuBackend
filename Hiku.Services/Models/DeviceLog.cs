using System;
using System.Collections.Generic;

namespace Hiku.Services.Models
{
    public partial class DeviceLog
    {
        public Guid Id { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string AppId { get; set; }
        public string DeviceId { get; set; }
        public string Data { get; set; }
    }
}
