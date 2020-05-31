using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using ReactiveUI;

namespace TcpCommunicator.TestGui
{
    public class MainWindowViewModel : ReactiveObject
    {
        private ConnectionProfile? _selectedProfile;

        public ObservableCollection<ConnectionProfile> Profiles { get; } = new ObservableCollection<ConnectionProfile>();

        public ConnectionProfile? SelectedProfile
        {
            get => _selectedProfile;
            set => this.RaiseAndSetIfChanged(ref _selectedProfile, value, nameof(this.SelectedProfile));
        }

        public MainWindowViewModel()
        {

        }
    }
}
