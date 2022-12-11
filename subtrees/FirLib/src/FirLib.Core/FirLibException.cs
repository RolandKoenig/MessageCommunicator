using System;
using System.Collections.Generic;
using System.Text;

namespace FirLib;

public class FirLibException : Exception
{
    public FirLibException(string message)
        : base(message)
    {
    }

    public FirLibException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}