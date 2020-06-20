using System;
using System.Reactive;
using System.Reactive.Disposables;
using Avalonia.Threading;
using ReactiveUI;
using TcpCommunicator.TestGui.Logic;

namespace TcpCommunicator.TestGui.Views
{
    public class ConnectionProfileViewModel : OwnViewModelBase
    {
        private bool _isRunning;
        private ConnectionState _connState;

        public ConnectionProfile Model { get; }

        public ReactiveCommand<object, Unit> Command_Start { get; }

        public ReactiveCommand<object, Unit> Command_Stop { get; }

        public ReactiveCommand<string, Unit> Command_SendMessage { get; }

        public bool IsRunning
        {
            get => _isRunning;
            set
            {
                if (_isRunning != value)
                {
                    _isRunning = value;
                    this.RaisePropertyChanged(nameof(this.IsRunning));
                }
            }
        }

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

        public ConnectionProfileViewModel(ConnectionProfile connProfile)
        {
            this.Model = connProfile;

            this.Command_Start = ReactiveCommand.Create<object>(arg => this.Model.Start());
            this.Command_Stop = ReactiveCommand.Create<object>(arg => this.Model.Stop());
            this.Command_SendMessage = ReactiveCommand.Create<string>(message =>
            {
                this.Model.SendMessageAsync(message);
            });
        }

        /// <inheritdoc />
        protected override void OnActivated(CompositeDisposable disposables)
        {
            base.OnActivated(disposables);

            var timer = new DispatcherTimer(
                TimeSpan.FromMilliseconds(100), DispatcherPriority.Normal,
                (sender, args) =>
                {
                    this.OnRefresh();
                });
            timer.Start();
            disposables.Add(new DummyDisposable(() => timer.Stop()));
        }

        private void OnRefresh()
        {
            this.IsRunning = this.Model.IsRunning;
            this.State = this.Model.State;
        }
    }
}
