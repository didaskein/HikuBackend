using Hiku.Framework.Enums;
using System.Collections.Generic;

namespace Hiku.Services
{
    public interface ISecurityService
    {
        string GetToken(string time = null);
        bool ValidateToken(Dictionary<EnumDataField, string> parameters);
    }
}