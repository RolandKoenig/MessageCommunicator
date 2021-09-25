using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageCommunicator.TestGui
{
    internal static class WindowsDpiAwarenessHandler
    {
        public static void StartupWindowsSettings()
        {
            // Code from https://stackoverflow.com/questions/43537990/wpf-clickonce-dpi-awareness-per-monitor-v2
            if (Environment.OSVersion.Version >= new Version(6, 3, 0)) // win 8.1 added support for per monitor dpi
            {
                if (Environment.OSVersion.Version >= new Version(10, 0, 15063)) // win 10 creators update added support for per monitor v2
                {
                    NativeMethodsWin32.SetProcessDpiAwarenessContext((int)NativeMethodsWin32.DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);
                }
                else
                {
                    NativeMethodsWin32.SetProcessDpiAwareness(NativeMethodsWin32.PROCESS_DPI_AWARENESS.Process_Per_Monitor_DPI_Aware);
                }
            }
            else
            {
                NativeMethodsWin32.SetProcessDPIAware();
            }
        }
    }
}
