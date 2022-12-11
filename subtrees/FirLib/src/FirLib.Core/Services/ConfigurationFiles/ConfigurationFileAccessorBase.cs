using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FirLib.Core.Services.ConfigurationFiles;

public abstract class ConfigurationFileAccessorBase : IConfigurationFileAccessor
{
    private string _baseDirectory;
    private string _appName;

    protected ConfigurationFileAccessorBase(string baseDirectory, string appName)
    {
        CheckFileNamePart(appName, nameof(appName));

        _baseDirectory = baseDirectory;
        _appName = appName;
    }

    public Stream WriteFile(string fileKey, string fileExtension)
    {
        var filePath = this.GetFileSavePath(fileKey, fileExtension);
        var dirPath = Path.GetDirectoryName(filePath)!;

        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }

        return new FileStream(filePath, FileMode.Create, FileAccess.Write);
    }

    public Stream? TryReadFile(string fileKey, string fileExtension)
    {
        var filePath = this.GetFileSavePath(fileKey, fileExtension);
        var dirPath = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(dirPath)) { return null; }
        if (!File.Exists(filePath)) { return null; }

        return File.OpenRead(filePath);
    }

    private static void CheckFileNamePart(string fileKey, string paramName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        for(var loop=0; loop<fileKey.Length; loop++)
        {
            var actChar = fileKey[loop];
            if (Array.IndexOf(invalidChars, actChar) > -1)
            {
                throw new ArgumentException(
                    nameof(fileKey),
                    $"Invalid char {actChar} at index {loop} of parameter {paramName} ({fileKey})!");
            }
        }
    }

    private string GetFileSavePath(string fileKey, string fileExtension)
    {
        CheckFileNamePart(fileKey, nameof(fileKey));
        CheckFileNamePart(fileKey, nameof(fileExtension));

        return Path.Combine(
            _baseDirectory, 
            Path.Combine(_appName, $"{fileKey}.{fileExtension}"));
    }
}