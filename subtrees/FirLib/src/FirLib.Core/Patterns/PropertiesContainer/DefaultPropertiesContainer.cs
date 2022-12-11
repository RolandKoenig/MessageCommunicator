using System;
using System.Collections.Generic;
using System.Text;

namespace FirLib.Core.Patterns.PropertiesContainer;

public class DefaultPropertiesContainer : IPropertiesContainer
{
    public IDictionary<string, PropertyValue> Properties { get; }

    public DefaultPropertiesContainer(int expectedPropertyCount = 0)
    {
        this.Properties = new Dictionary<string, PropertyValue>(expectedPropertyCount);
    }

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