using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace Hiku.Services.Models
{

    public class DeviceAnswer
    {
        public DeviceAnswer()
        {
            response = new Response();
        }
        public Response response { get; set; }
    }

    public class Response
    {
        public Response()
        {
            status = "ok";
            data = new Data();
        }

        public string status { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public Data()
        {
            status = "ok";
        }

        public string status { get; set; }
        public string audioToken { get; set; }

        public string errMsg { get; set; }
        

    }


}
