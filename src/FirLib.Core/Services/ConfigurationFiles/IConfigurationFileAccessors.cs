using System;
using System.Collections.Generic;
using System.Text;

namespace FirLib.Core.Services.ConfigurationFiles
{
    public interface IConfigurationFileAccessors
    {
        IConfigurationFileAccessor Application { get; }
    }
}
