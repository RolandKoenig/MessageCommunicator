using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MessageCommunicator.TestGui.Views
{
    public class SendMessageView : OwnUserControl<SendMessageViewModel>
    {
        public SendMessageView()
        {
            AvaloniaXamlLoader.Load(this);

            var txtSendMessage = this.FindControl<TextBox>("TxtSendMessage");
            txtSendMessage.FontFamily = MonospaceFontFamilyExtension.GetDefaultMonospaceFont();
        }
    }
}
