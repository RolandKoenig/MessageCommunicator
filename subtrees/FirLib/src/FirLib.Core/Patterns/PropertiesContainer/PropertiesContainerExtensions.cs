using System;
using System.Collections.Generic;
using System.Text;

namespace FirLib.Core.Patterns.PropertiesContainer;

public static class PropertiesContainerExtensions
{
    public static PropertyValue GetProperty(this IPropertiesContainer propContainer, string key)
    {
        if (propContainer.Properties.TryGetValue(key, out var value))
        {
            return value;
        }
        return PropertyValue.Empty;
    }

    public static void SetProperty(this IPropertiesContainer propContainer, string key, PropertyValue value)
    {
        if (value.IsEmpty)
        {
            propContainer.Properties.Remove(key);
        }
        else
        {
            propContainer.Properties[key] = value;
        }
    }

    public static void SetProperty(this IPropertiesContainer propContainer, string key, bool value)
    {
        propContainer.SetProperty(key, value ? PropertyValue.PROPERTY_VALUE_TRUE : PropertyValue.Empty);
    }

    public static void SetProperty(this IPropertiesContainer propContainer, string key, int value)
    {
        propContainer.SetProperty(key, value != 0 ? value.ToString() : PropertyValue.Empty);
    }

    public static void SetProperty(this IPropertiesContainer propContainer, string key, uint value)
    {
        propContainer.SetProperty(key, value != 0 ? value.ToString() : PropertyValue.Empty);
    }

    public static void RemoveProperty(this IPropertiesContainer propContainer, string key)
    {
        propContainer.Properties.Remove(key);
    }

    public static bool ContainsProperty(this IPropertiesContainer propContainer, string key)
    {
        return propContainer.Properties.ContainsKey(key);
    }

    public static void ClearProperties(this IPropertiesContainer propContainer)
    {
        propContainer.Properties.Clear();
    }

    public static void OvertakeAllProperties(
        this IPropertiesContainer targetContainer,
        IPropertiesContainer sourceContainer)
    {
        foreach (var actKey in sourceContainer.Properties.Keys)
        {
            targetContainer.SetProperty(actKey, sourceContainer.GetProperty(actKey));
        }
    }

    public static int CountProperties(this IPropertiesContainer propContainer)
    {
        return propContainer.Properties.Count;
    }
}