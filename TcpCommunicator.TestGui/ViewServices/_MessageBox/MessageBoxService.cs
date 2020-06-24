using System.Threading.Tasks;
using Avalonia.Controls;

namespace TcpCommunicator.TestGui.ViewServices
{
    public class MessageBoxService : IMessageBoxService
    {
        private Window _parentWindow;

        public MessageBoxService(Window parentWindow)
        {
            _parentWindow = parentWindow;
        }

        /// <inheritdoc />
        public Task<MessageBoxResult> ShowAsync(string title, string message, MessageBoxButtons buttons)
        {
            return SimpleMessageBox.ShowAsync(
                _parentWindow,
                title, message, buttons);
        }
    }
}
