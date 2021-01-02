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
                viewModel.TriggerRequest();

                this.DataContext = viewModel;
            }
        }
    }
}
