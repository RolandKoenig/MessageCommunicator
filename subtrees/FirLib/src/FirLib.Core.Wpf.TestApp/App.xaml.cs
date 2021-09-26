using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using FirLib.Core.Infrastructure;
using FirLib.Core;

namespace FirLib.Tests.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <inheritdoc />
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Initialize base application logic
            FirLibApplication.GetLoader()
                .AttachToWpfEnvironment()
                .Load();
        }
    }
}
