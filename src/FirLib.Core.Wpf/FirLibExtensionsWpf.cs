using System;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using FirLib.Core.Dialogs;
using FirLib.Core.Patterns;
using FirLib.Core.Patterns.ErrorAnalysis;
using Application = System.Windows.Application;
using IWin32Window = System.Windows.Forms.IWin32Window;

namespace FirLib.Core;

public static partial class FirLibExtensionsWpf
{
    public static IDisposable AttachUnhandledExceptionListener(this Application application)
    {
        application.DispatcherUnhandledException += CurrentOnDispatcherUnhandledException;

        return new DummyDisposable(
            () => application.DispatcherUnhandledException -= CurrentOnDispatcherUnhandledException);
    }

    private static bool CheckForEqualSynchronizationContexts(SynchronizationContext givenLeft, SynchronizationContext givenRight)
    {
        if (!(givenLeft is DispatcherSynchronizationContext left)) { return false; }
        if (!(givenRight is DispatcherSynchronizationContext right)) { return false; }

        var leftDispatcher = FirLibTools.ReadPrivateMember<Dispatcher, DispatcherSynchronizationContext>(left, "_dispatcher");
        var rightDispatcher = FirLibTools.ReadPrivateMember<Dispatcher, DispatcherSynchronizationContext>(right, "_dispatcher");

        return leftDispatcher == rightDispatcher;
    }

    private static void CurrentOnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        var exceptionInfo = new ExceptionInfo(e.Exception);

        var dlgError = new ErrorDialog();
        dlgError.DataContext = new ErrorDialogViewModel(exceptionInfo);
        dlgError.Owner = Application.Current.MainWindow;

        dlgError.ShowDialog();

        e.Handled = true;
    }

    public static IWin32Window GetIWin32Window(this Visual visual)
    {
        var source = (HwndSource)PresentationSource.FromVisual(visual)!;
        IWin32Window win = new Win32WindowHandleWrapper(source.Handle);
        return win;
    }
}