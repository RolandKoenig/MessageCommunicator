using System.Threading.Tasks;

namespace MessageCommunicator.TestGui.ViewServices
{
    public interface IMessageBoxService : IViewService
    {
        Task<MessageBoxResult> ShowAsync(string title, string message, MessageBoxButtons buttons);
    }
}
