using System.Threading.Tasks;

namespace MessageCommunicator.TestGui.ViewServices
{
    public interface IMessageBoxService
    {
        Task<MessageBoxResult> ShowAsync(string title, string message, MessageBoxButtons buttons);
    }
}
