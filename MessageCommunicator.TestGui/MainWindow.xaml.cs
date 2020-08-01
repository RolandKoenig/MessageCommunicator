using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MessageCommunicator.TestGui.ViewServices;

namespace MessageCommunicator.TestGui
{
    public class MainWindow : OwnWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            AvaloniaXamlLoader.Load(this);

            var versionString = Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "";
            this.Title = $"{this.Title} {versionString}";

            // Register view services
            var ctrlDialogHost = this.FindControl<DialogHostControl>("CrtlDialogHost");
            this.ViewServices.Add(new ConnectionConfigControlService(ctrlDialogHost));
            this.ViewServices.Add(new MessageBoxControlService(ctrlDialogHost));

            // Load initial main view model
            this.ViewModel = new MainWindowViewModel();
            this.DataContext = this.ViewModel;

#if DEBUG
            this.AttachDevTools();
#endif

            CommonErrorHandling.Current.MainWindow = this;
        }
    }
}
