using System;
using System.ComponentModel;
using System.Reactive;
using Force.DeepCloner;
using ReactiveUI;
using TcpCommunicator.TestGui.Data;

namespace TcpCommunicator.TestGui.ViewServices
{
    public class ConnectionConfigControlViewModel : OwnViewModelBase
    {
        public ConnectionParameters Model { get; }

        public ConnectionParametersViewModel ModelInteractive { get; }

        public ReactiveCommand<object?, Unit> Command_OK { get; }

        public ReactiveCommand<object?, Unit> Command_Cancel { get; }

        public ConnectionMode[] ConnectionModes => (ConnectionMode[])Enum.GetValues(typeof(ConnectionMode));

        public ConnectionConfigControlViewModel(ConnectionParameters? parameters = null)
        {
            this.Model = parameters != null ? parameters.DeepClone() : new ConnectionParameters();
            this.ModelInteractive = new ConnectionParametersViewModel(this.Model);

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
