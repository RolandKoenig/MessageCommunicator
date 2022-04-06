using System;
using System.Collections.Generic;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Themes.Fluent;
using ReactiveUI;

namespace MessageCommunicator.TestGui
{
    public class App : Application, IWeakMessageTarget<MessageThemeChangeRequest>
    {
        public static App CurrentApp => (App)Application.Current!;

        private List<Window> _openedWindows;

        public App()
        {
            _openedWindows = new List<Window>();
        }
        
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void RegisterWindow(Window window)
        {
            window.Closed += (_, _) => _openedWindows.Remove(window);
            _openedWindows.Add(window);
        }

        private void SetTheme(MessageCommunicatorTheme theme)
        {
            MessageCommunicatorGlobalProperties.Current.CurrentTheme = MessageCommunicatorTheme.Light;
            MessageBus.Current.SendMessage(new MessageThemeChanged(MessageCommunicatorTheme.Light));
            switch (theme)
            {
                case MessageCommunicatorTheme.Light:
                    this.Styles[0] = (StyleInclude)this.Resources["ThemeLight"]!;
                    this.Styles[1] = (StyleInclude)this.Resources["ThemeLightCustom"]!;
                    MessageCommunicatorGlobalProperties.Current.CurrentTheme = MessageCommunicatorTheme.Light;
                    MessageBus.Current.SendMessage(new MessageThemeChanged(MessageCommunicatorTheme.Light));
                    break;

                case MessageCommunicatorTheme.Dark:
                    this.Styles[0] = (StyleInclude)this.Resources["ThemeDark"]!;
                    this.Styles[1] = (StyleInclude)this.Resources["ThemeDarkCustom"]!;
                    MessageCommunicatorGlobalProperties.Current.CurrentTheme = MessageCommunicatorTheme.Dark;
                    MessageBus.Current.SendMessage(new MessageThemeChanged(MessageCommunicatorTheme.Dark));
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(theme), theme, null);
            }
        }

        public override void OnFrameworkInitializationCompleted()
        {
            // Apply initial theme
            MessageBus.Current.ListenWeak(this);
            this.SetTheme(MessageCommunicatorGlobalProperties.Current.CurrentTheme);
            
            // Open main window
            if (this.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }

        /// <inheritdoc />
        public void OnMessage(MessageThemeChangeRequest message)
        {
            this.SetTheme(message.NewTheme);
        }
    }
}
