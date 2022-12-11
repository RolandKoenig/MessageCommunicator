using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FirLib.Core.Services.ConfigurationFiles;

public interface IConfigurationFileAccessor
{
    Stream WriteFile(string fileKey, string fileExtension);

    Stream? TryReadFile(string fileKey, string fileExtension);
}