using System;
using System.Collections.Generic;

namespace Hiku.Services.Models
{
    public partial class Product
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTimeOffset TimestampAdded { get; set; }
        public string Barcode { get; set; }
    }
}
