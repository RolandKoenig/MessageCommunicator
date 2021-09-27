using System;
using System.ComponentModel;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using FirLib.Core.Avalonia.Controls;
using FirLib.Core.ViewServices.FileDialogs;
using FirLib.Core.ViewServices.MessageBox;
using MessageCommunicator.TestGui.Views;
using MessageCommunicator.TestGui.ViewServices;
using ReactiveUI;

namespace MessageCommunicator.TestGui
{
    public class MainWindow : OwnWindow<MainWindowViewModel>
    {
        private ConnectionProfileView _ctrlConnectionProfile;

        public MainWindow()
        {
            AvaloniaXamlLoader.Load(this);
#if DEBUG
            this.AttachDevTools();
#endif

            // Register this window on App object
            App.CurrentApp.RegisterWindow(this);

            // Find control objects
            _ctrlConnectionProfile = this.Find<ConnectionProfileView>("CtrlConnectionProfile");

            // Update title
            var versionInfoAttrib = Assembly.GetExecutingAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            var versionString = versionInfoAttrib?.InformationalVersion ?? "";
            this.Title = $"{this.Title} {versionString}";
            this.Find<TextBlock>("TxtTitle").Text = this.Title;

            // Register view services
            var ctrlDialogHost = this.Find<MainWindowFrame>("CtrlWindowFrame").DialogHostControl;
            var helpRepo = new IntegratedDocRepository(Assembly.GetExecutingAssembly());

            this.ViewServices.Add(new ConnectionConfigControlService(ctrlDialogHost));
            this.ViewServices.Add(new MessageBoxControlService(ctrlDialogHost));
            this.ViewServices.Add(new ExportViewService(ctrlDialogHost));
            this.ViewServices.Add(new ImportViewService(ctrlDialogHost));
            this.ViewServices.Add(new SaveFileDialogService(this));
            this.ViewServices.Add(new OpenFileDialogService(this));
            this.ViewServices.Add(new AboutDialogService(ctrlDialogHost));
            this.ViewServices.Add(new HelpBrowserService(this, helpRepo));
            this.ViewServices.Add(new ViewResourceService(App.CurrentApp));

            // Load initial main view model
            this.ViewModel = new MainWindowViewModel();
            this.ViewModel.PropertyChanged += this.OnViewModel_PropertyChanged;
            this.DataContext = this.ViewModel;

            // Configure error handling
            CommonErrorHandling.Current.MainWindow = this;
        }

        private void OnViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(MainWindowViewModel.SelectedProfile):
                    _ctrlConnectionProfile.DataContext = this.ViewModel?.SelectedProfile;
                    break;
            }
        }

        private void OnMnuExit_PointerPressed(object sender, PointerPressedEventArgs eArgs)
        {
            this.Close();
        }

        private void OnMnuThemeLight_PointerPressed(object sender, PointerPressedEventArgs eArgs)
        {
            MessageBus.Current.SendMessage(new MessageThemeChangeRequest(MessageCommunicatorTheme.Light));
        }

        private void OnMnuThemeDark_PointerPressed(object sender, PointerPressedEventArgs eArgs)
        {
            MessageBus.Current.SendMessage(new MessageThemeChangeRequest(MessageCommunicatorTheme.Dark));
        }
    }
}
