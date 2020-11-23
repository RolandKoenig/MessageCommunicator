using System;
using System.Collections.Generic;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace MessageCommunicator.TestGui
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);

            //var configDataAnnotator = (DefaultPropertyGridContractResolver)this.FindResource("ConfigDataAnnotator");
            //configDataAnnotator.AddDataAnnotation(
            //    typeof(ConnectionParametersViewModel),
            //    nameof(ConnectionParametersViewModel.Name), 
            //    new DisplayNameAttribute("sadfadfsdf"));
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (this.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
