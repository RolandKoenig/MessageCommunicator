using System;
using System.Collections.Generic;
using System.Text;

namespace FirLib.Core.Services.ConfigurationFiles
{
    public class ConfigurationFileAccessors : IConfigurationFileAccessors
    {
        public IConfigurationFileAccessor Application { get; }

        public ConfigurationFileAccessors(string appName)
        {
            this.Application = new ConfigurationFileAccessorApplication(appName);
        }
    }
}
