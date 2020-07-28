using System;

namespace TcpCommunicator.TestGui
{
    public class ViewServiceRequestEventArgs
    {
        public Type ViewServiceType { get; }

        public object? ViewService { get; set; }

        public ViewServiceRequestEventArgs(Type viewServiceType)
        {
            this.ViewServiceType = viewServiceType;
        }
    }

    public class CloseWindowRequestEventArgs
    {
        public object? DialogResult { get; }

        public CloseWindowRequestEventArgs(object? dialogResult)
        {
            this.DialogResult = dialogResult;
        }
    }
}
