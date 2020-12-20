using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace MessageCommunicator.TestGui.Startup
{
    public static class StartupOSThemeChange
    {
        public static AppBuilder HandleOSThemeChange(this AppBuilder appBuilder)
        {
            return appBuilder.AfterSetup(_ =>
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    MessageCommunicatorGlobalProperties.Current.CurrentTheme = WindowsThemeToMessageCommunicator(
                        WindowsThemeDetector.GetWindowsTheme());

                    var syncContext = SynchronizationContext.Current;
                    if (syncContext is AvaloniaSynchronizationContext)
                    {
                        WindowsThemeDetector.ListenForThemeChangeEvent(newTheme =>
                        {
                            syncContext.Post(args =>
                            {
                                MessageBus.Current.SendMessage(
                                    new MessageOSThemeChangeRequest(WindowsThemeToMessageCommunicator(newTheme)));
                            }, null);
                        });
                    }
                }
            });
        }

        /// <summary>
        /// Helper method for converting theme information.
        /// </summary>
        private static MessageCommunicatorTheme WindowsThemeToMessageCommunicator(WindowsTheme winTheme)
        {
            switch (winTheme)
            {
                case WindowsTheme.Dark:
                    return MessageCommunicatorTheme.Dark;

                case WindowsTheme.Light:
                    return MessageCommunicatorTheme.Light;
                default:
                    throw new ArgumentOutOfRangeException(nameof(winTheme), winTheme, null);
            }
        }
    }
}
