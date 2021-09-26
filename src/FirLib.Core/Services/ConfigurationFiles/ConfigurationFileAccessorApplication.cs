using System;
using System.Collections.Generic;
using System.Text;

namespace FirLib.Core.Services.ConfigurationFiles
{
    public class ConfigurationFileAccessorApplication : ConfigurationFileAccessorBase
    {
        public ConfigurationFileAccessorApplication(string appName)
            : base(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                appName)
        {

        }
    }
}
