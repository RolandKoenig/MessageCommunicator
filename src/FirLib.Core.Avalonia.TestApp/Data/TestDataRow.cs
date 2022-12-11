using System;
using FirLib.Core.Patterns.Mvvm;

namespace FirLib.Core.Avalonia.TestApp.Data;

public class TestDataRow : ViewModelBase
{
    private string _value1;
    private string _value2;
    private string _value3;
    private int _value4;

    public string Value1
    {
        get => _value1;
        set => this.SetProperty(ref _value1, value);
    }

    public string Value2
    {
        get => _value2;
        set => this.SetProperty(ref _value2, value);
    }

    public string Value3
    {
        get => _value3;
        set => this.SetProperty(ref _value3, value);
    }

    public int Value4
    {
        get => _value4;
        set => this.SetProperty(ref _value4, value);
    }

    public TestDataRow(Random random)
    {
        _value1 = random.Next(0, int.MaxValue).ToString();
        _value2 = random.Next(0, int.MaxValue).ToString();
        _value3 = random.Next(0, int.MaxValue).ToString();
        _value4 = random.Next(0, int.MaxValue);
    }
}