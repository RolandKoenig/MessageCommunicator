using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirLib.Core.Patterns.ErrorAnalysis;

public class ExceptionProperty
{
    public string Name { get; set; }
    public string Value { get; set; }

    public ExceptionProperty(string name, string value)
    {
        this.Name = name;
        this.Value = value;
    }

    public override string ToString()
    {
        return $"{this.Name}: {this.Value}";
    }
}