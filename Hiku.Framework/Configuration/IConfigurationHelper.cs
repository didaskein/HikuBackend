using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Hiku.Framework.Configuration
{
    public interface IConfigurationHelper
    {
        IConfiguration Configuration { get; }

        bool GetBoolSetting(string nameValue);
        bool GetBoolSetting(string nameValue, bool defaultValue);
        string GetConnectionString(string key);
        double GetDoubleSetting(string nameValue, double defaultValue);
        List<T> GetEnumSetting<T>(string nameValue, string defaultValue, char separator = ';') where T : struct, IComparable, IConvertible, IFormattable;
        int GetIntSetting(string nameValue, int defaultValue);
        List<string> GetListStringSetting(string nameValue, string defaultValue, char separator = ';');
        string GetSetting(string key);
        string GetStringSetting(string nameValue, string defaultValue);
        Uri GetUriSetting(string nameValue);
    }
}