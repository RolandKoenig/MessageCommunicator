using System;
using System.Collections.Generic;
using System.Text;

namespace FirLib.Core.Patterns.PropertiesContainer;

public readonly struct PropertyValue
{
    public static readonly PropertyValue PROPERTY_VALUE_TRUE = "X";
    public static readonly PropertyValue Empty = new PropertyValue();

    private readonly string _value;
    private readonly int _startIndex;
    private readonly int _length;

    public string Value => this.ToString();

    public bool IsEmpty => _length == 0;

    public PropertyValue(string value, int startIndex, int length)
    {
        if (value == null) { throw new ArgumentNullException(nameof(value)); }
        if (startIndex < 0) { throw new IndexOutOfRangeException(nameof(startIndex)); }
        if (startIndex >= value.Length) { throw new IndexOutOfRangeException(nameof(startIndex)); }
        if (startIndex + length > value.Length)
        {
            throw new IndexOutOfRangeException(
                $"Given start index {startIndex} with length {length} is not possible for given value with length {value.Length}");
        }

        _value = value;
        _startIndex = startIndex;
        _length = length;
    }

    public PropertyValue(string value)
    {
        _value = value;
        _startIndex = 0;
        _length = value?.Length ?? 0;
    }

    public ReadOnlySpan<char> AsSpan()
    {
        if(_length == 0){ return ReadOnlySpan<char>.Empty; }

        return _value.AsSpan(_startIndex, _length);
    }

    public bool AsBoolean()
    {
        return _length > 0;
    }

    public string AsString()
    {
        return this.ToString();
    }

    public int AsInt32(int defaultValue = 0)
    {
        if (_length == 0) { return defaultValue; }

#if NETSTANDARD2_0
        if (int.TryParse(this.ToString(), out var parsedValue))
        {
            return parsedValue;
        }
#else
            if (int.TryParse(this.AsSpan(), out var parsedValue))
            {
                return parsedValue;
            }
#endif

        return defaultValue;
    }

    public uint AsUInt32(uint defaultValue = 0)
    {
        if (_length == 0) { return defaultValue; }

#if NETSTANDARD2_0
        if (uint.TryParse(this.ToString(), out var parsedValue))
        {
            return parsedValue;
        }
#else
            if (uint.TryParse(this.AsSpan(), out var parsedValue))
            {
                return parsedValue;
            }
#endif

        return defaultValue;
    }

    public override string ToString()
    {
        if (_value == null){ return string.Empty; }
        if (_length == 0) { return string.Empty; }

        if ((_startIndex == 0) && (_length == _value.Length))
        {
            return _value;
        }
        else
        {
            return _value.Substring(_startIndex, _length);
        }
    }

    public static implicit operator string(PropertyValue value) => value.ToString();

    public static implicit operator PropertyValue(string value) => new PropertyValue(value);
}