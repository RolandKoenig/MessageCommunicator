using System;
using System.Windows;

namespace FirLib.Core.Wpf.TestApp;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private IDisposable? _unhandledExceptionDisposable;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _unhandledExceptionDisposable = this.AttachUnhandledExceptionListener();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);
         
        _unhandledExceptionDisposable?.Dispose();
    }
}