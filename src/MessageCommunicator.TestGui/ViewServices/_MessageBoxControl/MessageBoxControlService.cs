using System.Threading.Tasks;
using Avalonia.Controls;
using FirLib.Core.Avalonia.Controls;

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
