using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MessageCommunicator.TestGui.Views
{
    public class ConnectionProfileView : OwnUserControl<ConnectionProfileViewModel>
    {
        public ConnectionProfileView()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
