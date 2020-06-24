using System.Threading.Tasks;
using TcpCommunicator.TestGui.Data;

namespace TcpCommunicator.TestGui.ViewServices
{
    public interface IConnectionConfigViewService
    {
        Task<ConnectionParameters?> ConfigureConnectionAsync(ConnectionParameters? template);
    }
}
