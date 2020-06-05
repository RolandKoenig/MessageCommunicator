using System;
using System.Collections.Generic;
using System.Text;

namespace TcpCommunicator.TestGui
{
    public interface IConnectionConfigViewService
    {
        bool ConfigureConnection(ConnectionParameters connectionParameters);
    }
}
