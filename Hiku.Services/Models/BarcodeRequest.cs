using System;
using System.Collections.Generic;

namespace Hiku.Services.Models
{
    public partial class BarcodeRequest
    {
        public Guid Id { get; set; }
        public string State { get; set; }
        public DateTimeOffset TimestampFlashFinished { get; set; }
        public string Barcode { get; set; }
        public string AppId { get; set; }
        public string DeviceId { get; set; }
    }
}
