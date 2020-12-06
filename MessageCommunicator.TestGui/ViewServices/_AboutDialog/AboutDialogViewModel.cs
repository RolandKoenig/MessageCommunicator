using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;
using Avalonia.Controls;
using ReactiveUI;

namespace MessageCommunicator.TestGui.ViewServices
{
    public class AboutDialogViewModel : OwnViewModelBase
    {
        public string Name
        {
            get
            {
                var asmAttribute = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyProductAttribute>();
                return asmAttribute?.Product ?? string.Empty;
            }
        }

        public string Version
        {
            get
            {
                var asmAttribute = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>();
                return asmAttribute?.InformationalVersion ?? string.Empty;
            }
        }

        public string Description
        {
            get
            {
                var asmAttribute = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyDescriptionAttribute>();
                return asmAttribute?.Description ?? string.Empty;
            }
        }

        public string Homepage
        {
            get => "https://github.com/RolandKoenig/MessageCommunicator";
        }

        public string Author
        {
            get
            {
                var asmAttribute = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyCompanyAttribute>();
                return asmAttribute?.Company ?? string.Empty;
            }
        }

        public string Copyright
        {
            get
            {
                var asmAttribute = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyCopyrightAttribute>();
                return asmAttribute?.Copyright ?? string.Empty;
            }
        }

        public string TargetFramework
        {
            get
            {
                var asmAttribute = Assembly.GetExecutingAssembly().GetCustomAttribute<TargetFrameworkAttribute>();
                return asmAttribute?.FrameworkName ?? string.Empty;
            }
        }

        public string AvaloniaVersion
        {
            get
            {
                var asmAttribute = typeof(Window).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
                return asmAttribute?.InformationalVersion?? string.Empty;
            }
        }

        public string NetFrameworkVersion
        {
            get
            {
                return Environment.Version.ToString();
            }
        }

        [Browsable(false)]
        public ReactiveCommand<Unit, Unit> Command_Close { get; }

        [Browsable(false)]
        public AboutDialogViewModel Self => this;

        public AboutDialogViewModel()
        {
            this.Command_Close = ReactiveCommand.Create(() => this.CloseWindow(null));
        }
    }
}
