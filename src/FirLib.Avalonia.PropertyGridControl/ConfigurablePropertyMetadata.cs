using System.ComponentModel;

namespace FirLib.Avalonia.PropertyGridControl;

public class ConfigurablePropertyMetadata
{
    private IPropertyContractResolver? _propertyContractResolver;
    private PropertyDescriptor _descriptor;

    public string CategoryName { get; set; }

    public string PropertyName { get; set; }

    public string PropertyDisplayName { get; set; }

    public PropertyValueType ValueType { get; set; }

    public bool IsReadOnly { get; set; }

    public string Description { get; set; }

    public Type HostObjectType { get; }

    public ConfigurablePropertyMetadata(
        PropertyDescriptor propertyInfo,
        Type hostObjectType,
        IPropertyContractResolver? propertyContractResolver)
    {
        this.HostObjectType = hostObjectType;

        _descriptor = propertyInfo;
        _propertyContractResolver = propertyContractResolver;

        var categoryAttrib = this.GetCustomAttribute<CategoryAttribute>();
        this.CategoryName = categoryAttrib?.Category ?? string.Empty;

        this.PropertyName = propertyInfo.Name;
        this.IsReadOnly = propertyInfo.IsReadOnly;

        this.PropertyDisplayName = propertyInfo.Name;
        var displayNameAttrib = this.GetCustomAttribute<DisplayNameAttribute>();
        if ((displayNameAttrib != null) &&
            (!string.IsNullOrEmpty(displayNameAttrib.DisplayName)))
        {
            this.PropertyDisplayName = displayNameAttrib.DisplayName;
        }

        this.Description = propertyInfo.Description;

        var propertyType = _descriptor.PropertyType;
        if (propertyType == typeof(string) || propertyType == typeof(char) ||
            propertyType == typeof(double) || propertyType == typeof(float) || propertyType == typeof(decimal) ||
            propertyType == typeof(int) || propertyType == typeof(uint) ||
            propertyType == typeof(byte) ||
            propertyType == typeof(short) || propertyType == typeof(ushort) ||
            propertyType == typeof(long) || propertyType == typeof(ulong))
        {
            this.ValueType = PropertyValueType.String;
        }
        else if (propertyType == typeof(bool))
        {
            this.ValueType = PropertyValueType.Bool;
        }
        else if (propertyType.IsSubclassOf(typeof(Enum)))
        {
            this.ValueType = PropertyValueType.Enum;
        }
        else
        {
            this.ValueType = PropertyValueType.Unsupported;
        }
    }

    public override string ToString()
    {
        return $"{this.CategoryName} - {this.PropertyDisplayName} (type {this.ValueType})";
    }

    public Array GetEnumMembers()
    {
        if (this.ValueType != PropertyValueType.Enum)
        {
            throw new InvalidOperationException(
                $"Method {nameof(this.GetEnumMembers)} not supported on value type {this.ValueType}!");
        }
        return Enum.GetValues(_descriptor.PropertyType);
    }

    public T? GetCustomAttribute<T>()
        where T : Attribute
    {
        var resultFromDataAnnotator =
            _propertyContractResolver?.GetDataAnnotation<T>(_descriptor.ComponentType, _descriptor.Name);
        if (resultFromDataAnnotator != null)
        {
            return resultFromDataAnnotator;
        }

        foreach (var actAttribute in _descriptor.Attributes)
        {
            if (actAttribute is T foundAttribute)
            {
                return foundAttribute;
            }
        }

        return null;
    }
}