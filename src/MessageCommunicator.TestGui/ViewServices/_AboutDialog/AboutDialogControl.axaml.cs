using System;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

namespace MessageCommunicator.TestGui.ViewServices
{
    public class AboutDialogControl : OwnUserControlDialog<AboutDialogViewModel>
    {
        public AboutDialogControl()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
