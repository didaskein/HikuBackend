using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Hiku.Services.Configuration
{
    [DisplayName("Security")]
    public class SecurityConfiguration
    {
        public string AppId { get; set; }
        public string Secret { get; set; }
    }
}
