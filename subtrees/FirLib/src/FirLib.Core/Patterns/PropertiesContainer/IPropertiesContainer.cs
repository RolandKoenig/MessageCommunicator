using System;
using System.Collections.Generic;

namespace FirLib.Core.Patterns.PropertiesContainer;

public interface IPropertiesContainer
{
    IDictionary<string, PropertyValue> Properties { get; }
}