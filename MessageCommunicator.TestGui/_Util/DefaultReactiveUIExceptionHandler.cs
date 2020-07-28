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
        public void OnNext(Exception value)
        {
            CommonErrorHandling.Current.ShowErrorDialog(value);
        }

        public void OnError(Exception error)
        {
            CommonErrorHandling.Current.ShowErrorDialog(error);
        }

        public void OnCompleted()
        {

        }
    }
}
