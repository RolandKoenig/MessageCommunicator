using System;
using System.Collections.Generic;
using System.Text;

namespace FirLib.Formats.Gpx;

public class GpxFileException : Exception
{
    public GpxFileException(string message)
        : base(message)
    {
    }

    public GpxFileException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}