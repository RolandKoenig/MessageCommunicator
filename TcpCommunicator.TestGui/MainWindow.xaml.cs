using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using TcpCommunicator.TestGui.ViewServices;

namespace TcpCommunicator.TestGui
{
    public class MainWindow : OwnWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            AvaloniaXamlLoader.Load(this);

            var ctrlDialogHost = this.FindControl<DialogHostControl>("CrtlDialogHost");
            var ctrlDataGrid = this.FindControl<DataGrid>("LstProfiles");
            ctrlDataGrid.CellPointerPressed += (sender, eArgs) =>
            {
                ctrlDataGrid.SelectedItem = eArgs.Row.DataContext;
            };

            //this.ViewServices.Add(new ConnectionConfigViewService(this));
            this.ViewServices.Add(new ConnectionConfigControlService(ctrlDialogHost));
            this.ViewServices.Add(new MessageBoxControlService(ctrlDialogHost));

            this.ViewModel = new MainWindowViewModel();
            this.DataContext = this.ViewModel;

#if DEBUG
            this.AttachDevTools();
#endif

            CommonErrorHandling.Current.MainWindow = this;
        }
    }
}
