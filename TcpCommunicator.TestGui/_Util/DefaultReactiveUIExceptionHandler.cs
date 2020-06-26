using System;
using System.Collections.Generic;
using System.Text;
using Avalonia.Controls;

namespace TcpCommunicator.TestGui
{
    /// <summary>
    /// A dummy object which handles exception cached by ReactiveUI.
    /// </summary>
    public class DefaultReactiveUIExceptionHandler : IObserver<Exception>
    {
        public static DefaultReactiveUIExceptionHandler Current { get; } = new DefaultReactiveUIExceptionHandler();

        public Window? MainWindow { get; set; }

        private DefaultReactiveUIExceptionHandler()
        {

        }

        public void OnNext(Exception value)
        {
            var errorDlg = new ErrorDialog();
            errorDlg.DataContext = new ErrorDialogViewModel(value);
            errorDlg.ShowDialog(this.MainWindow);
        }

        public void OnError(Exception error)
        {
            var errorDlg = new ErrorDialog();
            errorDlg.DataContext = new ErrorDialogViewModel(error);
            errorDlg.ShowDialog(this.MainWindow);
        }

        public void OnCompleted()
        {

        }
    }
}
