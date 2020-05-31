using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Text;
using ReactiveUI;

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

        /// <inheritdoc />
        protected override void OnActivated(CompositeDisposable disposables)
        {
            // TODO: Implement activation
        }
    }
}
