using System;
using System.Collections.Generic;
using System.Text;
using Avalonia.Controls;
using Avalonia.Threading;

namespace MessageCommunicator.TestGui
{
    public class CommonErrorHandling
    {
        public static CommonErrorHandling Current { get; } = new CommonErrorHandling();

        public Window? MainWindow { get; set; }

        private CommonErrorHandling()
        {

        }

        /// <summary>
        /// Shows a common error dialog for the given exception.
        /// </summary>
        public void ShowErrorDialog(Exception ex)
        {
            if (Dispatcher.UIThread.CheckAccess())
            {
                var errorDlg = new ErrorDialog();
                errorDlg.DataContext = new ErrorDialogViewModel(ex);
                errorDlg.ShowDialog(this.MainWindow);
            }
            else
            {
                Dispatcher.UIThread.Post(
                    () => this.ShowErrorDialog(ex));
            }
        }

        /// <summary>
        /// Handles the given exception which causes the application to break.
        /// </summary>
        public void HandleFatalException(Exception ex)
        {
            // TODO: Fatal exception mechanism
            //       GUI is not ensured to be available here
        }
    }
}
