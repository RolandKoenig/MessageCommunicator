using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ReactiveUI;
using TcpCommunicator.TestGui.Logic;
using TcpCommunicator.TestGui.ViewServices;

namespace TcpCommunicator.TestGui
{
    public class MainWindowViewModel : OwnViewModelBase
    {
        private ConnectionProfile? _selectedProfile;

        public ObservableCollection<ConnectionProfile> Profiles { get; } = new ObservableCollection<ConnectionProfile>();

        public ConnectionProfile? SelectedProfile
        {
            get => _selectedProfile;
            set => this.RaiseAndSetIfChanged(ref _selectedProfile, value, nameof(this.SelectedProfile));
        }

        public ReactiveCommand<object?, Unit> Command_CreateProfile { get; }

        public MainWindowViewModel()
        {
            this.Command_CreateProfile = ReactiveCommand.CreateFromTask<object?>(this.CreateProfileAsync);
        }

        private async Task CreateProfileAsync(object? arg, CancellationToken cancelToken)
        {
            var srvConnectionConfig = this.GetViewService<IConnectionConfigViewService>();
            var connParams = await srvConnectionConfig.ConfigureConnectionAsync(null);
            if (connParams != null)
            {
                var newProfile = new ConnectionProfile(SynchronizationContext.Current!, connParams);
                this.Profiles.Add(newProfile);
                this.SelectedProfile = newProfile;
            }
        }

        /// <inheritdoc />
        protected override void OnActivated(CompositeDisposable disposables)
        {
            // TODO: Implement activation
        }
    }
}
