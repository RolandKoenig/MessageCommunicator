using System.Threading.Tasks;
using FirLib.Core.Patterns.Mvvm;

namespace MessageCommunicator.TestGui.ViewServices
{
    public interface IMessageBoxService : IViewService
    {
        Task<MessageBoxResult> ShowAsync(string title, string message, MessageBoxButtons buttons);
    }
}
