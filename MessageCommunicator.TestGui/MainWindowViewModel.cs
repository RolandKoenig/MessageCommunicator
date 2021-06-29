using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using MessageCommunicator.TestGui.Data;
using MessageCommunicator.TestGui.Logic;
using MessageCommunicator.TestGui.Views;
using MessageCommunicator.TestGui.ViewServices;
using ReactiveUI;

namespace MessageCommunicator.TestGui
{
    public class MainWindowViewModel : OwnViewModelBase, IDoubleTabEnabledViewModel
    {
        private ConnectionProfileViewModel? _selectedProfile;
        private MainWindowFrameStatus _statusBarState;

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
                    this.SendMessageVM.CurrentConnectionProfile = _selectedProfile?.Model;
                    this.RaisePropertyChanged(nameof(this.SelectedProfile));
                    this.RaisePropertyChanged(nameof(this.IsProfileScreenEnabled));
                }
            }
        }

        public MainWindowFrameStatus StatusBarState
        {
            get => _statusBarState;
            set
            {
                if (_statusBarState != value)
                {
                    _statusBarState = value;
                    this.RaisePropertyChanged(nameof(this.StatusBarState));
                }
            }
        }

        public SendMessageViewModel SendMessageVM { get; }

        public bool IsProfileScreenEnabled => _selectedProfile != null;

        public ReactiveCommand<object?, Unit> Command_ImportProfiles { get; }

        public ReactiveCommand<object?, Unit> Command_ExportProfiles { get; }

        public ReactiveCommand<object?, Unit> Command_CreateProfile { get; }

        public ReactiveCommand<object?, Unit> Command_EditProfile { get; }

        public ReactiveCommand<object?, Unit> Command_DeleteProfile { get; }

        public ReactiveCommand<object?, Unit> Command_ShowAboutDialog { get; }
        
        public ReactiveCommand<Unit, Unit> Command_ShowHelp { get; }

        public MainWindowViewModel()
        {
            this.Command_ImportProfiles = ReactiveCommand.CreateFromTask<object?>(this.OnCommand_ImportProfiles_ExecuteAsync);
            this.Command_ExportProfiles = ReactiveCommand.CreateFromTask<object?>(this.OnCommand_ExportProfiles_ExecuteAsync);
            this.Command_CreateProfile = ReactiveCommand.CreateFromTask<object?>(this.OnCommand_CreateProfile_ExecuteAsync);
            this.Command_EditProfile = ReactiveCommand.CreateFromTask<object?>(this.OnCommand_EditProfile_ExecuteAsync);
            this.Command_DeleteProfile = ReactiveCommand.CreateFromTask<object?>(this.OnCommand_DeleteProfile_ExecuteAsync);
            this.Command_ShowAboutDialog = ReactiveCommand.CreateFromTask<object?>(this.OnCommand_ShowAboutDialog_ExecuteAsync);
            this.Command_ShowHelp =
                ReactiveCommand.Create(() => this.GetViewService<IHelpViewerService>().ShowHelpPage("Home"));

            this.SendMessageVM = new SendMessageViewModel();

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
                (_, _) =>
                {
                    try
                    {
                        var anyConnected = false;
                        var anyConnecting = false;
                        foreach (var actProfile in this.Profiles)
                        {
                            actProfile.RefreshData();

                            if (actProfile.State == ConnectionState.Connected) { anyConnected = true; }
                            else if (actProfile.State == ConnectionState.Connecting) { anyConnecting = true; }
                        }

                        if (anyConnecting) { this.StatusBarState = MainWindowFrameStatus.Yellow; }
                        else if (anyConnected) { this.StatusBarState = MainWindowFrameStatus.Green; }
                        else { this.StatusBarState = MainWindowFrameStatus.NeutralGray; }
                    }
                    catch (Exception e)
                    {
                        CommonErrorHandling.Current.ShowErrorDialog(e);
                    }
                });
            timer.Start();
            disposables.Add(new DummyDisposable(() => timer.Stop()));
        }

        private async Task OnCommand_ImportProfiles_ExecuteAsync(object? arg, CancellationToken cancelToken)
        {
            var connectionProfiles = this.Profiles
                .Select(actProfile => actProfile.Model.Parameters)
                .ToList();

            var srvImportDlg = this.GetViewService<IImportViewService>();
            var importResult = await srvImportDlg.ImportAsync(
                connectionProfiles,
                nameof(ConnectionParameters.Name), Constants.DATA_TYPE_CONNECTION_PROFILES);
            if (importResult != null)
            {
                // Replace updated objects
                foreach (var actUpdatedObject in importResult.UpdatedObjects)
                {
                    var updatedProfile = this.Profiles.FirstOrDefault(actProfile =>
                        actProfile.Model.Name == actUpdatedObject.OriginalObject.Name);
                    if (updatedProfile == null)
                    {
                        importResult.NewObjects.Add(actUpdatedObject.NewObject);
                        continue;
                    }

                    await updatedProfile.Model.ChangeParametersAsync(actUpdatedObject.NewObject);
                }
                
                // Add new objects
                foreach (var actNewObject in importResult.NewObjects)
                {
                    this.Profiles.Add(new ConnectionProfileViewModel(
                        new ConnectionProfile(SynchronizationContext.Current!, actNewObject)));
                }

                ConnectionProfileStore.Current.StoreConnectionProfiles(
                    this.Profiles.Select(actProfileVM => actProfileVM.Model));
            }  
        }

        private async Task OnCommand_ExportProfiles_ExecuteAsync(object? arg, CancellationToken cancelToken)
        {
            if (this.Profiles.Count == 0)
            {
                await this.GetViewService<IMessageBoxService>().ShowAsync(
                    "Export",
                    "Nothing to export",
                    MessageBoxButtons.Ok);
                return;
            }

            var srvExportDlg = this.GetViewService<IExportViewService>();
            await srvExportDlg.ExportAsync(
                this.Profiles.Select(actVM => actVM.Model.Parameters),
                this.SelectedProfile != null ? new[] { this.SelectedProfile.Model.Parameters } : new ConnectionParameters[0],
                nameof(ConnectionParameters.Name),
                Constants.DATA_TYPE_CONNECTION_PROFILES);
        }

        private async Task OnCommand_CreateProfile_ExecuteAsync(object? arg, CancellationToken cancelToken)
        {
            var srvConnectionConfig = this.GetViewService<IConnectionConfigViewService>();
            var connParams = await srvConnectionConfig.ConfigureConnectionAsync(
                null, 
                this.Profiles.Select(actProfile => actProfile.Model.Parameters));
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

        private async Task OnCommand_EditProfile_ExecuteAsync(object? arg, CancellationToken cancelToken)
        {
            var selectedProfileVM = this.SelectedProfile;
            if (selectedProfileVM == null) { return; }

            var srvConnectionConfig = this.GetViewService<IConnectionConfigViewService>();
            var connParams = await srvConnectionConfig.ConfigureConnectionAsync(
                selectedProfileVM.Model.Parameters,
                this.Profiles.Select(actProfile => actProfile.Model.Parameters));
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

        private async Task OnCommand_DeleteProfile_ExecuteAsync(object? arg, CancellationToken cancelToken)
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

        private Task OnCommand_ShowAboutDialog_ExecuteAsync(object? arg, CancellationToken cancelToken)
        {
            var srvAboutDlg = this.GetViewService<IAboutDialogService>();
            return srvAboutDlg.ShowAboutDialogAsync();
        }

        /// <inheritdoc />
        public void NotifyDoubleTap(object rowViewModel)
        {
            if (rowViewModel is ConnectionProfileViewModel profileVM)
            {
                this.SelectedProfile = profileVM;
                this.Command_EditProfile.Execute().Subscribe();
            }
        }
    }
}
