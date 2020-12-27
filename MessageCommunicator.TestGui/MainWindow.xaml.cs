using System;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;
using MessageCommunicator.TestGui.ViewServices;
using ReactiveUI;

namespace MessageCommunicator.TestGui
{
    public class MainWindow : OwnWindow<MainWindowViewModel>, IWeakMessageTarget<MessageOSThemeChangeRequest>
    {
        public MainWindow()
        {
            AvaloniaXamlLoader.Load(this);

            // Apply initial theme
            this.SetTheme(MessageCommunicatorGlobalProperties.Current.CurrentTheme);
            MessageBus.Current.ListenWeak(this);

            var versionInfoAttrib = Assembly.GetExecutingAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            var versionString = versionInfoAttrib?.InformationalVersion ?? "";
            this.Title = $"{this.Title} {versionString}";

            // Register view services
            var ctrlDialogHost = this.FindControl<DialogHostControl>("CrtlDialogHost");
            var helpRepo = new IntegratedDocRepository(Assembly.GetExecutingAssembly());

            this.ViewServices.Add(new ConnectionConfigControlService(ctrlDialogHost));
            this.ViewServices.Add(new MessageBoxControlService(ctrlDialogHost));
            this.ViewServices.Add(new ExportViewService(ctrlDialogHost));
            this.ViewServices.Add(new ImportViewService(ctrlDialogHost));
            this.ViewServices.Add(new SaveFileDialogService(this));
            this.ViewServices.Add(new OpenFileDialogService(this));
            this.ViewServices.Add(new AboutDialogService(ctrlDialogHost));
            this.ViewServices.Add(new HelpBrowserService(this, helpRepo));
            //this.ViewServices.Add(new HelpViewerService(ctrlDialogHost, helpRepo));

            // Load initial main view model
            this.ViewModel = new MainWindowViewModel();
            this.DataContext = this.ViewModel;

#if DEBUG
            this.AttachDevTools();
#endif

            // Configure error handling
            CommonErrorHandling.Current.MainWindow = this;
        }

        private void SetTheme(MessageCommunicatorTheme theme)
        {
            switch (theme)
            {
                case MessageCommunicatorTheme.Light:
                    this.Styles[0] = (StyleInclude) this.Resources["ThemeLight"];
                    this.Styles[1] = (StyleInclude) this.Resources["ThemeLightCustom"];
                    MessageCommunicatorGlobalProperties.Current.CurrentTheme = MessageCommunicatorTheme.Light;
                    MessageBus.Current.SendMessage(new MessageThemeChanged(MessageCommunicatorTheme.Light));
                    break;
                
                case MessageCommunicatorTheme.Dark:
                    this.Styles[0] = (StyleInclude) this.Resources["ThemeDark"];
                    this.Styles[1] = (StyleInclude) this.Resources["ThemeDarkCustom"];
                    MessageCommunicatorGlobalProperties.Current.CurrentTheme = MessageCommunicatorTheme.Dark;
                    MessageBus.Current.SendMessage(new MessageThemeChanged(MessageCommunicatorTheme.Dark));
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(theme), theme, null);
            }
        }

        private void OnMnuExit_PointerPressed(object sender, PointerPressedEventArgs eArgs)
        {
            this.Close();
        }

        private void OnMnuThemeLight_PointerPressed(object sender, PointerPressedEventArgs eArgs)
        {
            this.SetTheme(MessageCommunicatorTheme.Light);
        }

        private void OnMnuThemeDark_PointerPressed(object sender, PointerPressedEventArgs eArgs)
        {
            this.SetTheme(MessageCommunicatorTheme.Dark);
        }

        /// <inheritdoc />
        public void OnMessage(MessageOSThemeChangeRequest message)
        {
            this.SetTheme(message.NewTheme);
        }
    }
}
