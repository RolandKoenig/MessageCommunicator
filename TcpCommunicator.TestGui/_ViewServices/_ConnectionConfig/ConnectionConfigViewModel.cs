using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;
using Force.DeepCloner;
using ReactiveUI;

namespace TcpCommunicator.TestGui
{
    public class ConnectionConfigViewModel : OwnViewModelBase
    {
        public ConnectionParameters Model { get; }

        public ReactiveCommand<object?, Unit> Command_OK { get; }

        public ReactiveCommand<object?, Unit> Command_Cancel { get; }

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
