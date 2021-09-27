using System.Threading.Tasks;
using FirLib.Core.Avalonia.Controls;
using FirLib.Core.Patterns.Mvvm;

namespace MessageCommunicator.TestGui.ViewServices
{
    public class MessageBoxControlService : ViewServiceBase, IMessageBoxService
    {
        private DialogHostControl _host;

        public MessageBoxControlService(DialogHostControl host)
        {
            _host = host;
        }

        /// <inheritdoc />
        public Task<MessageBoxResult> ShowAsync(string title, string message, MessageBoxButtons buttons)
        {
            return MessageBoxControl.ShowAsync(_host, title, message, buttons);
        }
    }
}
