using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MessageCommunicator.TestGui.Views
{
    public class SendMessageView : OwnUserControl<SendMessageViewModel>
    {
        public SendMessageView()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
