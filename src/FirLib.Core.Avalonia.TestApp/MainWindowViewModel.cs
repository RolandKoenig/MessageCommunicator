using System;
using System.Collections.ObjectModel;
using FirLib.Core.Avalonia.TestApp.Data;
using FirLib.Core.Patterns.Mvvm;

namespace FirLib.Core.Avalonia.TestApp;

public class MainWindowViewModel : ViewModelBase
{
    public ObservableCollection<TestDataRow> TestData { get; }

    public MainWindowViewModel()
    {
        var random = new Random(100);

        this.TestData = new ObservableCollection<TestDataRow>();
        for (var loop = 0; loop < 100; loop++)
        {
            this.TestData.Add(new TestDataRow(random));
        }
    }
}