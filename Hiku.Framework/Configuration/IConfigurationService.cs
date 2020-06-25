using System;
using System.Collections.Generic;
using System.Text;

namespace Hiku.Framework.Configuration
{
    public interface IConfigurationService<T> where T : class
    {
        T ConfigurationOptions { get; }
    }
}
