using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Avalonia;

namespace MessageCommunicator.TestGui.Startup
{
    public static class StartupSystemSettings
    {
        public static AppBuilder SetStartupSystemSettings(this AppBuilder appBuilder)
        {
            // Special startup logic for windows
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                WindowsDpiAwarenessHandler.StartupWindowsSettings();
            }

            return appBuilder;
        }
    }
}
