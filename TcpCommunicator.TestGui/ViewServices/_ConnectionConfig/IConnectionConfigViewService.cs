using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TcpCommunicator.TestGui.Data;

namespace TcpCommunicator.TestGui.ViewServices
{
    public interface IConnectionConfigViewService
    {
        Task<ConnectionParameters?> ConfigureConnectionAsync(ConnectionParameters? template);
    }
}
