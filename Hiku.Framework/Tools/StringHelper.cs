using Hiku.Framework.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Hiku.Framework
{
    public class StringHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters">sample "time=2020-04-15%2020%3A53%3A41.000000&app_id=e3xxxxxxxxxxxxxxxxxxxx44&sig=4d6fab46efb4debaef7a322a7c7ec6c487d8c83d1e53eb2dfcbc4a8be25afe13&token=20000c2a6909f5a2&audioType=alaw&ean=7610700601532"
        /// </param>
        /// <returns></returns>
        public Dictionary<EnumDataField, string> GetParameters(string parameters, bool withUrlDecode = true)
        {
            Dictionary<EnumDataField, string> dic = new Dictionary<EnumDataField, string>();

            if (!string.IsNullOrWhiteSpace(parameters))
            {
                var splitparams = parameters.Split('&');
                if(splitparams != null)
                {
                    foreach (var splitparam in splitparams)
                    {
                        var splitdetail = splitparam.Split('=');
                        if(splitdetail != null)
                        {
                            EnumDataField field = EnumDataField.none;
                            if (splitdetail.Length >= 1)
                            {
                                if(Enum.TryParse<EnumDataField>(splitdetail[0], true, out field))  // case insensitive
                                {
                                    if (splitdetail.Length == 1)
                                    {
                                        dic.Add(field, string.Empty);
                                    }
                                    else if (splitdetail.Length == 2)
                                    {
                                        string val = splitdetail[1];

                                        if (withUrlDecode) val = WebUtility.UrlDecode(splitdetail[1]);

                                        dic.Add(field, val);
                                    }
                                }
                                else
                                {
                                    throw new Exception($"StringHelper.GetParameters : {splitdetail[0]} not found in enum EnumDataField.");
                                }

                            }
                            
                        }
                        
                    }
                }
            }

            return dic;
        }


    }
}
