using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirLib.Core.Patterns.ErrorAnalysis;

namespace FirLib.Tests.Core.Patterns.ErrorAnalysis;

static class TestUtilities
{
    public static Dictionary<string, string> ToDictionary(IEnumerable<ExceptionProperty> properties)
    {
        var result = new Dictionary<string, string>(16);
        foreach(var actProperty in properties)
        {
            result[actProperty.Name] = actProperty.Value;
        }
        return result;
    }
}