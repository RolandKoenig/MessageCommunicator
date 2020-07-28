using Avalonia.Markup.Xaml;

namespace MessageCommunicator.TestGui.ViewServices
{
    public class ConnectionConfigControl : OwnUserControlDialog<ConnectionConfigControlViewModel>
    {
        public ConnectionConfigControl()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
