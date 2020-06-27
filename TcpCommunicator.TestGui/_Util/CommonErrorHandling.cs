using System;
using System.Collections.Generic;
using System.Text;
using Avalonia.Controls;

namespace TcpCommunicator.TestGui
{
    public class CommonErrorHandling
    {
        public static CommonErrorHandling Current { get; } = new CommonErrorHandling();

        public Window? MainWindow { get; set; }

        private CommonErrorHandling()
        {

        }

        public void ShowErrorDialog(Exception ex)
        {
            var errorDlg = new ErrorDialog();
            errorDlg.DataContext = new ErrorDialogViewModel(ex);
            errorDlg.ShowDialog(this.MainWindow);
        }

        public void ExecuteSafe(Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                this.ShowErrorDialog(e);
            }
        }

        public Action WrapAction(Action action)
        {
            return () =>
            {
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    this.ShowErrorDialog(e);
                }
            };
        }
    }
}
