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
    public class ConnectionConfigControl : OwnUserControlDialog<ConnectionConfigControlViewModel>
    {
        public ConnectionConfigControl()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /// <inheritdoc />
        protected override async void OnActivated(CompositeDisposable disposables)
        {
            base.OnActivated(disposables);

            await Task.Delay(100);

            var mainPropertyGrid = this.FindControl<PropertyGrid>("CtrlMainPropertyGrid");
            mainPropertyGrid?.FocusFirstValueRowEditor();
        }
    }
}
