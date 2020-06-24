using Avalonia;
using Avalonia.Markup.Xaml;
using TcpCommunicator.TestGui.ViewServices;

namespace TcpCommunicator.TestGui
{
    public class MainWindow : OwnWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            AvaloniaXamlLoader.Load(this);

            this.ViewServices.Add(new ConnectionConfigViewService(this));
            this.ViewServices.Add(new MessageBoxService(this));

            this.ViewModel = new MainWindowViewModel();
            this.DataContext = this.ViewModel;

#if DEBUG
            this.AttachDevTools();
#endif
        }
    }
}
