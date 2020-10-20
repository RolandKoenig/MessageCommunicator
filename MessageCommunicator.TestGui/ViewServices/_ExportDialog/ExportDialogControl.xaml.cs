using System;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

namespace MessageCommunicator.TestGui.ViewServices
{
    public class ExportDialogControl : OwnUserControlDialog<ExportDialogControlViewModel>
    {
        public ExportDialogControl()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
