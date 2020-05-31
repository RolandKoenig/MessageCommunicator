using System;
using System.Collections.Generic;
using System.Text;

namespace TcpCommunicator.TestGui
{
    public class ViewServiceRequestEventArgs
    {
        public ViewServiceRequestEventArgs(Type viewServiceType)
        {
            this.ViewServiceType = viewServiceType;
        }

        public Type ViewServiceType { get; }

        public object? ViewService { get; set; }
    }
}
