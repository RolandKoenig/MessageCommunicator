using System.Threading.Tasks;
using Avalonia.X11;
using ReactiveUI;

namespace MessageCommunicator.TestGui.Views
{
    public class ReleaseCheckViewModel : OwnViewModelBase
    {
        private string _statusText;

        public string StatusText
        {
            get => _statusText;
            set
            {
                if (_statusText != value)
                {
                    _statusText = value;
                    this.RaisePropertyChanged(nameof(this.StatusText));
                }
            }
        }
        
        public ReleaseCheckViewModel()
        {
            _statusText = "Checking for newer releases...";
        }

        public void TriggerRequest()
        {
            ReleaseOverview.GetLatestReleaseAsync()
                .ContinueWith(task =>
                {
                    if(task.IsFaulted){ this.ProcessLatestReleaseInfo(null); }
                    else
                    {
                        this.ProcessLatestReleaseInfo(task.Result);
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void ProcessLatestReleaseInfo(ReleaseInformation? releaseInfo)
        {
            if (releaseInfo == null)
            {
                this.StatusText = "Unable to check for newer releases";
                return;
            }

            this.StatusText = $"Newest: {releaseInfo.Version}";
        }
    }
}