using System;
using System.Reactive;
using System.Runtime.CompilerServices;
using ReactiveUI;
using TcpCommunicator.TestGui.Logic;

namespace TcpCommunicator.TestGui.Views
{
    public class ConnectionProfileViewModel : OwnViewModelBase
    {
        private bool _isRunning;
        private ConnectionState _connState;
        private string _remoteEndpointDescription;

        public ConnectionProfile Model { get; }

        public ReactiveCommand<object?, Unit> Command_Start { get; }

        public ReactiveCommand<object?, Unit> Command_Stop { get; }

        public ReactiveCommand<string?, Unit> Command_SendMessage { get; }

        public bool IsRunning
        {
            get => _isRunning;
            set
            {
                if (_isRunning != value)
                {
                    _isRunning = value;
                    this.RaisePropertyChanged(nameof(this.IsRunning));
                    this.RaisePropertyChanged(nameof(this.CanStart));
                    this.RaisePropertyChanged(nameof(this.CanStop));
                }
            }
        }

        public bool CanStart => !this.IsRunning;

        public bool CanStop => this.IsRunning;

        public ConnectionState State
        {
            get => _connState;
            set
            {
                if (_connState != value)
                {
                    _connState = value;
                    this.RaisePropertyChanged(nameof(this.State));
                }
            }
        }

        public string RemoteEndpointDescription
        {
            get => _remoteEndpointDescription;
            set
            {
                if (_remoteEndpointDescription != value)
                {
                    _remoteEndpointDescription = value;
                    this.RaisePropertyChanged(nameof(this.RemoteEndpointDescription));
                }
            }
        }

        public ConnectionProfileViewModel(ConnectionProfile connProfile)
        {
            this.Model = connProfile;
            _remoteEndpointDescription = string.Empty;

            this.Command_Start = ReactiveCommand.Create<object?>(arg =>
            {
                if (!this.Model.IsRunning)
                {
                    this.Model.Start();
                }
            });
            this.Command_Stop = ReactiveCommand.Create<object?>(arg =>
            {
                if (this.Model.IsRunning)
                {
                    this.Model.Stop();
                }
            });
            this.Command_SendMessage = ReactiveCommand.CreateFromTask<string?>(async message =>
            {
                try
                {
                    if (message == null) { return; }
                    await this.Model.SendMessageAsync(message);
                }
                catch (Exception e)
                {
                    CommonErrorHandling.Current.ShowErrorDialog(e);
                }
            });
        }

        public void RefreshData()
        {
            this.IsRunning = this.Model.IsRunning;
            this.State = this.Model.State;
            this.RemoteEndpointDescription = this.Model.RemoteEndpointDescription;
        }
    }
}
