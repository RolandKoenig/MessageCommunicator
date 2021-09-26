using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using FirLib.Core.Dialogs;
using FirLib.Core.Infrastructure;
using FirLib.Core.Patterns.ErrorAnalysis;
using FirLib.Core.Patterns.Messaging;

namespace FirLib.Core
{
    public static partial class FirLibExtensionsWpf
    {
        public static FirLibApplicationLoader AttachToWpfEnvironment(this FirLibApplicationLoader loader)
        {
            var uiMessenger = new FirLibMessenger();

            loader.AddLoadAction(() =>
            {
                Application.Current.DispatcherUnhandledException += CurrentOnDispatcherUnhandledException;

                uiMessenger.CustomSynchronizationContextEqualityChecker = CheckForEqualSynchronizationContexts;
                uiMessenger.ConnectToGlobalMessaging(
                    FirLibMessengerThreadingBehavior.EnsureMainSyncContextOnSyncCalls,
                    FirLibConstants.MESSENGER_NAME_GUI,
                    SynchronizationContext.Current);
            });
            loader.AddUnloadAction(() =>
            {
                Application.Current.DispatcherUnhandledException -= CurrentOnDispatcherUnhandledException;
                uiMessenger.DisconnectFromGlobalMessaging();
            });
            
            return loader;
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

        public static System.Windows.Forms.IWin32Window GetIWin32Window(this System.Windows.Media.Visual visual)
        {
            var source = (System.Windows.Interop.HwndSource)PresentationSource.FromVisual(visual)!;
            System.Windows.Forms.IWin32Window win = new Win32WindowHandleWrapper(source.Handle);
            return win;
        }
    }
}
