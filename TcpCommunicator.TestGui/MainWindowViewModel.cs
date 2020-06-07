using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ReactiveUI;
using TcpCommunicator.TestGui.Logic;
using TcpCommunicator.TestGui.Views;
using TcpCommunicator.TestGui.ViewServices;

namespace TcpCommunicator.TestGui
{
    public class MainWindowViewModel : OwnViewModelBase
    {
        private ConnectionProfileViewModel? _selectedProfile;

        public ObservableCollection<ConnectionProfileViewModel> Profiles { get; } = new ObservableCollection<ConnectionProfileViewModel>();

        public ConnectionProfileViewModel? SelectedProfile
        {
            get => _selectedProfile;
            set
            {
                if (_selectedProfile != value)
                {
                    _selectedProfile = value;
                    this.RaisePropertyChanged(nameof(this.SelectedProfile));
                    this.RaisePropertyChanged(nameof(this.IsProfileScreenEnabled));
                }
            }
        }

        public bool IsProfileScreenEnabled => _selectedProfile != null;

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
                var newProfileVM = new ConnectionProfileViewModel(newProfile);

                this.Profiles.Add(newProfileVM);
                this.SelectedProfile = newProfileVM;
            }
        }

        /// <inheritdoc />
        protected override void OnActivated(CompositeDisposable disposables)
        {
            // TODO: Implement activation
        }
    }
}
