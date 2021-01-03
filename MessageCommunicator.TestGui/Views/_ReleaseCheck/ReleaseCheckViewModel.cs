using System.Reactive;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media;
using Avalonia.X11;
using MessageCommunicator.TestGui.ViewServices;
using ReactiveUI;

namespace MessageCommunicator.TestGui.Views
{
    public class ReleaseCheckViewModel : OwnViewModelBase
    {
        private string _statusText;
        private VectorIconDrawingPresenter? _icon;

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

        public VectorIconDrawingPresenter? Icon
        {
            get => _icon;
            set
            {
                if (!ReferenceEquals(_icon, value))
                {
                    _icon = value;
                    this.RaisePropertyChanged(nameof(this.Icon));
                }
            }
        }

        public ReactiveCommand<Unit, Unit> Command_GoToReleases
        {
            get;
        }
        
        public ReleaseCheckViewModel()
        {
            _statusText = "Checking for newer releases...";

            this.Command_GoToReleases = ReactiveCommand.Create(() =>
            {
                CommonUtil.OpenUrlInBrowser("https://github.com/RolandKoenig/MessageCommunicator/releases");
            });
        }

        /// <summary>
        /// Triggers the request for latest release.
        /// </summary>
        public void TriggerRequest()
        {
            ReleaseInformation? latestReleaseInfo = null;

            var taskQueryReleaseInfo = ReleaseOverview.TryGetLatestReleaseAsync()
                .ContinueWith(task =>
                {
                    if (!task.IsFaulted)
                    {
                        latestReleaseInfo = task.Result;
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());

            // Try request latest release info for a maximum of 5 seconds
            Task.WhenAny(taskQueryReleaseInfo, Task.Delay(5000))
                .ContinueWith(task =>
                {
                    this.ProcessLatestReleaseInfo(latestReleaseInfo);
                }, TaskScheduler.FromCurrentSynchronizationContext());
            
            this.TrySetIcon("IconRefresh");
        }

        /// <summary>
        /// Sets the given <see cref="ReleaseInformation"/> as the latest release.
        /// </summary>
        public void ProcessLatestReleaseInfo(ReleaseInformation? releaseInfo)
        {
            if (releaseInfo == null)
            {
                this.StatusText = "Unable to check for newer releases";
                this.TrySetIcon("IconWarning");
                return;
            }

            var currentVersion = Assembly.GetExecutingAssembly().GetName().Version!;
            if (releaseInfo.Version > currentVersion)
            {
                this.StatusText = $"You are running an older release (newest version is {releaseInfo.Version.ToString(3)}";
                this.TrySetIcon("IconWarning");
            }
            else if (releaseInfo.Version == currentVersion)
            {
                this.StatusText = $"You are running the newest release";
                this.TrySetIcon("IconCheck");
            }
            else
            {
                // Should only be possible in development environment
                this.StatusText = "This is a development build";
                this.TrySetIcon("IconLab");
            }
        }

        private void TrySetIcon(string iconResourceName)
        {
            var iconResource = this.GetViewService<IViewResourceService>().TryGetViewResource(iconResourceName) as Drawing;
            if (iconResource == null)
            {
                this.Icon = null;
                return;
            }

            var newIcon = new VectorIconDrawingPresenter()
            {
                Drawing = iconResource,
                Width = 16.0,
                Height = 16.0,
                Margin = new Thickness(2.0)
            };
            newIcon.UpdateBrushes();
            this.Icon = newIcon;
        }
    }
}