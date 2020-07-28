using System.Threading.Tasks;
using MessageCommunicator.TestGui.Data;

namespace MessageCommunicator.TestGui.ViewServices
{
    public interface IConnectionConfigViewService
    {
        Task<ConnectionParameters?> ConfigureConnectionAsync(ConnectionParameters? template);
    }
}
