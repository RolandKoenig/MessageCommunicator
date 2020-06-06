using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TcpCommunicator.TestGui
{
    public interface IConnectionConfigViewService
    {
        Task<ConnectionParameters?> ConfigureConnectionAsync(ConnectionParameters? template);
    }
}
