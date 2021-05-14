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
                this.DataContext = new ReleaseCheckViewModel();
            }
        }

        protected override void OnActivated(CompositeDisposable disposables)
        {
            base.OnActivated(disposables);

            if(this.DataContext is ReleaseCheckViewModel viewModel)
            {
                viewModel.TriggerRequest();
            }
        }
    }
}
