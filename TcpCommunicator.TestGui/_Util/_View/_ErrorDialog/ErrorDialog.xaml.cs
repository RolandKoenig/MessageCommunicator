using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TcpCommunicator.TestGui
{
    public class ErrorDialog : OwnWindow<ErrorDialogViewModel>
    {
        public ErrorDialog()
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
