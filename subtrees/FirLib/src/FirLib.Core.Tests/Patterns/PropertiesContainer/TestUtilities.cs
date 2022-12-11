using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirLib.Core.Patterns.PropertiesContainer;

namespace FirLib.Tests.Core.Patterns.PropertiesContainer;

internal static class TestUtilities
{
    public static IPropertiesContainer CreatePropertiesContainer(string type)
    {
        switch (type)
        {
            case "Concurrent":
                return new ConcurrentPropertiesContainer();

            case "Default":
                return new DefaultPropertiesContainer();

            default:
                throw new ArgumentException($"Unknown type {type}!");
        }
    }
}