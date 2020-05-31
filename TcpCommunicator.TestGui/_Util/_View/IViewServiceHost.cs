using System;
using System.Collections.Generic;
using System.Text;

namespace TcpCommunicator.TestGui
{
    public interface IViewServiceHost
    {
        public List<object> ViewServices { get; }
    }
}
