using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using TcpCommunicator.TestGui.ViewServices;

namespace TcpCommunicator.TestGui
{
    public class MainWindow : OwnWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            AvaloniaXamlLoader.Load(this);

            var ctrlDialogHost = this.FindControl<DialogHostControl>("CrtlDialogHost");

            //this.ViewServices.Add(new ConnectionConfigViewService(this));
            this.ViewServices.Add(new ConnectionConfigControlService(ctrlDialogHost));
            this.ViewServices.Add(new MessageBoxControlService(ctrlDialogHost));

            this.ViewModel = new MainWindowViewModel();
            this.DataContext = this.ViewModel;

#if DEBUG
            this.AttachDevTools();
#endif

            DefaultReactiveUIExceptionHandler.Current.MainWindow = this;
        }
    }
}
