using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TcpCommunicator.TestGui.ViewServices
{
    public interface IMessageBoxService
    {
        Task<MessageBoxResult> ShowAsync(string title, string message, MessageBoxButtons buttons);
    }
}
