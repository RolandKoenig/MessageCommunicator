using System.Threading.Tasks;

namespace TcpCommunicator.TestGui.ViewServices
{
    public interface IMessageBoxService
    {
        Task<MessageBoxResult> ShowAsync(string title, string message, MessageBoxButtons buttons);
    }
}
