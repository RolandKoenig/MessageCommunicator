using System;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Media;
using MessageCommunicator.TestGui.ViewServices;

namespace MessageCommunicator.TestGui
{
    public class MainWindow : OwnWindow<MainWindowViewModel>
    {
        private Grid _ctrlMainGrid;

        public MainWindow()
        {
            AvaloniaXamlLoader.Load(this);

            _ctrlMainGrid = this.FindControl<Grid>("CtrlMainGrid");

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

        private void OnMnuExit_PointerPressed(object sender, PointerPressedEventArgs eArgs)
        {
            this.Close();
        }

        private void OnMnuThemeLight_PointerPressed(object sender, PointerPressedEventArgs eArgs)
        {
            this.Styles[0] = (StyleInclude)this.Resources["ThemeLight"];

            _ctrlMainGrid.IsVisible = false;
            _ctrlMainGrid.IsVisible = true;
        }

        private void OnMnuThemeDark_PointerPressed(object sender, PointerPressedEventArgs eArgs)
        {
            this.Styles[0] = (StyleInclude)this.Resources["ThemeDark"];

            _ctrlMainGrid.IsVisible = false;
            _ctrlMainGrid.IsVisible = true;

            
        }
    }
}
