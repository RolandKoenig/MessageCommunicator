using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace FirLib.Core.Patterns.PropertiesContainer;

public class ConcurrentPropertiesContainer : IPropertiesContainer
{
    public IDictionary<string, PropertyValue> Properties { get; }
        = new ConcurrentDictionary<string, PropertyValue>();

    /// <inheritdoc />
    public override string ToString()
    {
        var propertyCount = this.Properties.Count;
        if (propertyCount == 1)
        {
            return $"{nameof(ConcurrentPropertiesContainer)} (1 property)";
        }
        else
        {
            return $"{nameof(ConcurrentPropertiesContainer)} ({propertyCount} properties)";
        }
    }
}