using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Hiku.Services.Configuration
{
    [DisplayName("BlobStorage")]
    public class BlobStorageConfiguration
    {
        public string ConnectionString { get; set; }
        public string AudioBlobContainerName { get; set; }
    }
}


