using Avalonia.Markup.Xaml;

namespace TcpCommunicator.TestGui.ViewServices
{
    public class ConnectionConfigControl : OwnUserControlDialog<ConnectionConfigControlViewModel>
    {
        public ConnectionConfigControl()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
