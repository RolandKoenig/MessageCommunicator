using System.ComponentModel;
using System.Reactive.Disposables;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MessageCommunicator.TestGui.Views
{
    public class ReleaseCheckView : OwnUserControl<ReleaseCheckViewModel>
    {
        public ReleaseCheckView()
        {
            AvaloniaXamlLoader.Load(this);
            
            if (!Design.IsDesignMode)
            {
                var viewModel = new ReleaseCheckViewModel();
                
                SynchronizationContext.Current!.Post((arg) =>
                {
                    viewModel.TriggerRequest();
                }, null);
                

                this.DataContext = viewModel;
            }
        }
    }
}
