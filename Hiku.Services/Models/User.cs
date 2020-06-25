using System;
using System.Collections.Generic;

namespace Hiku.Services.Models
{
    public partial class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string AppId { get; set; }
        public string DeviceId { get; set; }
        public DateTimeOffset TimestampAdded { get; set; }
        public DateTimeOffset TimestampUpdated { get; set; }
    }
}
