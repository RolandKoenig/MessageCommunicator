using System;
using System.Reactive;
using Force.DeepCloner;
using ReactiveUI;
using TcpCommunicator.TestGui.Data;

namespace TcpCommunicator.TestGui.ViewServices
{
    public class ConnectionConfigViewModel : OwnViewModelBase
    {
        public ConnectionParameters Model { get; }

        public ReactiveCommand<object?, Unit> Command_OK { get; }

        public ReactiveCommand<object?, Unit> Command_Cancel { get; }

        public ConnectionMode ConnectionMode
        {
            get => this.Model.Mode;
            set
            {
                if (this.Model.Mode != value)
                {
                    this.Model.Mode = value;
                    this.RaisePropertyChanged(nameof(this.ConnectionMode));
                    this.RaisePropertyChanged(nameof(this.IsConfigIPEnabled));
                }
            }
        }

        public bool IsConfigIPEnabled => this.Model.Mode == ConnectionMode.Active;

        public ConnectionMode[] ConnectionModes => (ConnectionMode[])Enum.GetValues(typeof(ConnectionMode));

        public ConnectionConfigViewModel(ConnectionParameters? parameters = null)
        {
            this.Model = parameters != null ? parameters.DeepClone() : new ConnectionParameters();

            this.Command_OK = ReactiveCommand.Create<object?>(this.OnCommandOK);
            this.Command_Cancel = ReactiveCommand.Create<object?>(
                arg => this.CloseWindow(null));
        }

        private void OnCommandOK(object? arg)
        {
            var model = this.Model;

            // TODO: validate model

            this.CloseWindow(model);
        }
    }
}
