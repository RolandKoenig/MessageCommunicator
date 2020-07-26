﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using ReactiveUI;
using TcpCommunicator.TestGui.Logic;
using TcpCommunicator.TestGui.Views;
using TcpCommunicator.TestGui.ViewServices;

namespace TcpCommunicator.TestGui
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

        public ReactiveCommand<object?, Unit> Command_CreateProfile { get; }

        public ReactiveCommand<object?, Unit> Command_EditProfile { get; }

        public ReactiveCommand<object?, Unit> Command_DeleteProfile { get; }

        public MainWindowViewModel()
        {
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
                await selectedProfileVM.Model.ChangeParametersAsync(connParams);

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
                "TcpCommunicator", $"This action will delete the profile {selectedProfile.Model.Name}",
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
