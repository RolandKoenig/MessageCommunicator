using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using MessageCommunicator.TestGui.Logic;
using MessageCommunicator.TestGui.Views;
using MessageCommunicator.TestGui.ViewServices;
using ReactiveUI;

namespace MessageCommunicator.TestGui
{
    public class MainWindowViewModel : OwnViewModelBase
    {
        private ConnectionProfileViewModel? _selectedProfile;

        public ObservableCollection<ConnectionProfileViewModel> Profiles { get; } =
            new ObservableCollection<ConnectionProfileViewModel>();

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

        public ReactiveCommand<object?, Unit> Command_ImportProfiles { get; }

        public ReactiveCommand<object?, Unit> Command_ExportProfiles { get; }

        public ReactiveCommand<object?, Unit> Command_CreateProfile { get; }

        public ReactiveCommand<object?, Unit> Command_EditProfile { get; }

        public ReactiveCommand<object?, Unit> Command_DeleteProfile { get; }

        public MainWindowViewModel()
        {
            this.Command_ImportProfiles = ReactiveCommand.CreateFromTask<object?>(this.ImportProfilesAsync);
            this.Command_ExportProfiles = ReactiveCommand.CreateFromTask<object?>(this.ExportProfilesAsync);
            this.Command_CreateProfile = ReactiveCommand.CreateFromTask<object?>(this.CreateProfileAsync);
            this.Command_EditProfile = ReactiveCommand.CreateFromTask<object?>(this.EditProfileAsync);
            this.Command_DeleteProfile = ReactiveCommand.CreateFromTask<object?>(this.DeleteSelectedProfile);

            var syncContext = SynchronizationContext.Current;
            if (syncContext == null) { return; }

            var restoredProfiles = ConnectionProfileStore.Current.LoadConnectionProfiles(syncContext);
            if (restoredProfiles != null)
            {
                foreach (var actRestoredProfile in restoredProfiles)
                {
                    this.Profiles.Add(new ConnectionProfileViewModel(actRestoredProfile));
                }
            }
        }

        /// <inheritdoc />
        protected override void OnActivated(CompositeDisposable disposables)
        {
            var timer = new DispatcherTimer(
                TimeSpan.FromMilliseconds(100), DispatcherPriority.Normal,
                (sender, args) =>
                {
                    try
                    {
                        foreach (var actProfile in this.Profiles)
                        {
                            actProfile.RefreshData();
                        }
                    }
                    catch (Exception e)
                    {
                        CommonErrorHandling.Current.ShowErrorDialog(e);
                    }
                });
            timer.Start();
            disposables.Add(new DummyDisposable(() => timer.Stop()));
        }

        private async Task ImportProfilesAsync(object? arg, CancellationToken cancelToken)
        {
            var connectionProfiles = this.Profiles
                .Select(actProfile => actProfile.Model)
                .ToList();

            var srvImportDlg = this.GetViewService<IImportViewService>();
            await srvImportDlg.ImportAsync(
                connectionProfiles,
                nameof(ConnectionProfile.Name), Constants.DATA_TYPE_CONNECTION_PROFILES);
        }

        private async Task ExportProfilesAsync(object? arg, CancellationToken cancelToken)
        {
            var srvExportDlg = this.GetViewService<IExportViewService>();
            await srvExportDlg.ExportAsync(
                this.Profiles.Select(actVM => actVM.Model),
                this.SelectedProfile != null ? new[] { this.SelectedProfile.Model } : new ConnectionProfile[0],
                nameof(ConnectionProfile.Name),
                Constants.DATA_TYPE_CONNECTION_PROFILES);
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

                ConnectionProfileStore.Current.StoreConnectionProfiles(
                    this.Profiles.Select(actProfileVM => actProfileVM.Model));
            }
        }

        private async Task EditProfileAsync(object? arg, CancellationToken cancelToken)
        {
            var selectedProfileVM = this.SelectedProfile;
            if (selectedProfileVM == null) { return; }

            var srvConnectionConfig = this.GetViewService<IConnectionConfigViewService>();
            var connParams = await srvConnectionConfig.ConfigureConnectionAsync(selectedProfileVM.Model.Parameters);
            if (connParams != null)
            {
                // Update connection logic
                await selectedProfileVM.Model.ChangeParametersAsync(connParams);

                // Simple trick here to trigger refresh on the view
                var indexOfProfile = this.Profiles.IndexOf(selectedProfileVM);
                if ((indexOfProfile >= 0) && (indexOfProfile < this.Profiles.Count))
                {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                    this.Profiles[indexOfProfile] = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                    this.Profiles[indexOfProfile] = selectedProfileVM;
                    this.SelectedProfile = selectedProfileVM;
                }

                // Trigger persistence
                ConnectionProfileStore.Current.StoreConnectionProfiles(
                    this.Profiles.Select(actProfileVM => actProfileVM.Model));
            }
        }

        private async Task DeleteSelectedProfile(object? arg, CancellationToken cancelToken)
        {
            var selectedProfile = this.SelectedProfile;
            if (selectedProfile == null) { return; }

            var srvMessageBox = this.GetViewService<IMessageBoxService>();
            var msgResult = await srvMessageBox.ShowAsync(
                "Remove Profile", $"This action will delete the profile '{selectedProfile.Model.Name}'",
                MessageBoxButtons.OkCancel);
            if (msgResult != MessageBoxResult.Ok) { return; }

            // Stop the connection if it is still running
            if (selectedProfile.IsRunning)
            {
                await selectedProfile.Model.StopAsync();
            }

            this.Profiles.Remove(selectedProfile);

            ConnectionProfileStore.Current.StoreConnectionProfiles(
                this.Profiles.Select(actProfileVM => actProfileVM.Model));
        }
    }
}
