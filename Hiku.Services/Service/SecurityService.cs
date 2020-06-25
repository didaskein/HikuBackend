using Hiku.Framework;
using Hiku.Framework.Enums;
using Hiku.Services.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Hiku.Services
{
    public class SecurityService : ISecurityService
    {
        private readonly SecurityConfiguration _securityConfiguration;

        public SecurityService(SecurityConfiguration securityConfiguration)
        {
            _securityConfiguration = securityConfiguration;
        }

        public string GetToken(string time=null)
        {
            //# Result	Protocol	Host	URL	Body	Caching	Content-Type	Process	Comments	Custom	Privacy Info	
            //411	200	HTTPS	app.hiku.us	/api/v1/user?app_id=mZ30fj48tjmcswEw353h1Q&sig=c697d663776bc25b2cbe8bd1d27b9f82296524e86e8541bc2dbd603dd73dc514&time=2018-04-14%2012:41:14.433000&token=6ee949dc35f3c7a5275d0dad9348b83a	86		application/json					
            //639	200	HTTPS	app.hiku.us	/api/v1/user?app_id=mZ30fj48tjmcswEw353h1Q&sig=6c0a6e3b950bcb2b60f2d4cd8d953def770cbbe45cb010f3b4ddab6ae11a700b&time=2018-04-14%2012:54:45.624000&token=6ee949dc35f3c7a5275d0dad9348b83a	86		application/json					
            //https://github.com/hikuinc/hiku_shared/blob/4377c6faee68321e26b573f15575916e5af2c397/API/hikuApiExample.py
            //https://github.com/Slyce-Inc/hiku_shared/blob/4377c6faee68321e26b573f15575916e5af2c397/API/hikuApiExample.py

            //Need UTC time !!!! (not local time)
            //string appId = "e3xxxxxxxxxxxxxxxxxxxx44";
            //string secret = "c9xxxxxxxxx6";
            //string time = "2020-04-15 20:29:05.433000";  // yyyy-MM-dd HH:mm:ss.SSSSSS
            //string hash = sha256(appId + secret + time);  //  "3bdcde99acac91fc1069515e2c1513d6d2191a8bd639ffcb16b49ac9178689e4"

            //Need UTC time !!!! (not local time)
            string appId = _securityConfiguration.AppId;
            string secret = _securityConfiguration.Secret;
            if(string.IsNullOrWhiteSpace(time)) time = DateTimeOffset.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.ffffff");
            string hash = Sha256(appId + secret + time);  

            return hash;
        }

        public bool ValidateToken(Dictionary<EnumDataField, string> parameters)
        {
            bool tokenValid = false;

            // input = "time=2020-04-17%2013%3A13%3A12.000000&sig=8be5714fa4d50c6f4e3305dfbea49331711c44e3f55540cecda873b37ae09cc2&agentUrl=https%3A%2F%2Fagent.electricimp.com%2FRf6ii2XMsbig&token=20000c2a6909f5a2&app_id=e3xxxxxxxxxxxxxxxxxxxx44&agentAudioRate=8000";
            if (parameters != null && parameters.ContainsKey(EnumDataField.time) && parameters.ContainsKey(EnumDataField.sig)  && parameters.ContainsKey(EnumDataField.app_id))
            {
                if (_securityConfiguration.AppId != parameters[EnumDataField.app_id]) return false;

                string sig = GetToken(parameters[EnumDataField.time]);

                if (sig == parameters[EnumDataField.sig]) return true;
            }

            return tokenValid;

        }

        private string Sha256(string randomString)
        {
            var crypt = new System.Security.Cryptography.SHA256Managed();
            var hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(randomString));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }

    }
}
