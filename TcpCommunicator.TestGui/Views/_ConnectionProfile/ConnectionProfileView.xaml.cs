using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TcpCommunicator.TestGui.Views
{
    public class ConnectionProfileView : OwnUserControl<ConnectionProfileViewModel>
    {
        public ConnectionProfileView()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
