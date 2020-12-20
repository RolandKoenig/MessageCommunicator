using System;
using System.Collections.Generic;
using System.Globalization;
using System.Management;
using System.Runtime.Versioning;
using System.Security.Principal;
using System.Text;
using Microsoft.Win32;

namespace MessageCommunicator.TestGui
{
    /// <summary>
    /// Checks for currently configured theme in windows.
    /// See: https://engy.us/blog/2018/10/20/dark-theme-in-wpf/
    /// </summary>
    internal class WindowsThemeDetector
    {
        private const string REGISTRY_KEY_PATH = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
        private const string REGISTRY_VALUE_NAME = "AppsUseLightTheme";

        [SupportedOSPlatform("windows")]
        public static void ListenForThemeChangeEvent(Action<WindowsTheme> setWindowsThemeAction)
        {
            var currentUser = WindowsIdentity.GetCurrent();
            if (currentUser.User == null) { return; }

            string query = string.Format(
                CultureInfo.InvariantCulture,
                @"SELECT * FROM RegistryValueChangeEvent WHERE Hive = 'HKEY_USERS' AND KeyPath = '{0}\\{1}' AND ValueName = '{2}'",
                currentUser.User.Value,
                REGISTRY_KEY_PATH.Replace(@"\", @"\\"),
                REGISTRY_VALUE_NAME);
            try
            {
                var watcher = new ManagementEventWatcher(query);
                watcher.EventArrived += (sender, args) => setWindowsThemeAction(GetWindowsTheme());

                // Start listening for events
                watcher.Start();
            }
            catch (Exception)
            {
                // This can fail on Windows 7
            }
        }

        [SupportedOSPlatform("windows")]
        public static WindowsTheme GetWindowsTheme()
        {
            var subKey = Registry.CurrentUser.OpenSubKey(REGISTRY_KEY_PATH);
            if (subKey == null) { return WindowsTheme.Light; }
            try
            {
                var registryValueObject = subKey.GetValue(REGISTRY_VALUE_NAME);
                if (registryValueObject == null)
                {
                    return WindowsTheme.Light;
                }

                var registryValue = (int) registryValueObject;
                return registryValue > 0 ? WindowsTheme.Light : WindowsTheme.Dark;
            }
            finally
            {
                subKey.Dispose();
            }
        }
    }
}
