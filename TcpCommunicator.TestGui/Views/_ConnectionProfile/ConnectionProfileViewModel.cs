using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;
using ReactiveUI;
using TcpCommunicator.TestGui.Logic;

namespace TcpCommunicator.TestGui.Views
{
    public class ConnectionProfileViewModel : OwnViewModelBase
    {
        public ConnectionProfile Model { get; }

        public ReactiveCommand<object, Unit> Command_Start { get; }

        public ReactiveCommand<object, Unit> Command_Stop { get; }

        public ReactiveCommand<string, Unit> Command_SendMessage { get; }

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
    }
}
