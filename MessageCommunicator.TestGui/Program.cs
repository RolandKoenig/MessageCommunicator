using System;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Logging.Serilog;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace MessageCommunicator.TestGui
{
    internal class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args)
        {
            try
            {
                // Special startup logic for windows
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    WindowsDpiAwarenessHandler.StartupWindowsSettings();

                    switch (WindowsThemeDetector.GetWindowsTheme())
                    {
                        case WindowsTheme.Light:
                            MessageCommunicatorGlobalProperties.Current.CurrentTheme = MessageCommunicatorTheme.Light;
                            break;

                        case WindowsTheme.Dark:
                            MessageCommunicatorGlobalProperties.Current.CurrentTheme = MessageCommunicatorTheme.Dark;
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                // Start avalonia logic
                BuildAvaloniaApp()
                    .StartWithClassicDesktopLifetime(args);
            }
            catch (Exception e)
            {
                CommonErrorHandling.Current.HandleFatalException(e);
            }
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
        {
            RxApp.DefaultExceptionHandler = new DefaultReactiveUIExceptionHandler();

            return AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .UseReactiveUI()
                .LogToDebug();
        }
    }
}
