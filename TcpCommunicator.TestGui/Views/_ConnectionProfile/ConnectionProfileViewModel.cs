using System;
using System.Collections.Generic;
using System.Text;
using TcpCommunicator.TestGui.Logic;

namespace TcpCommunicator.TestGui.Views
{
    public class ConnectionProfileViewModel : OwnViewModelBase
    {
        public ConnectionProfile Model { get; }

        public ConnectionProfileViewModel(ConnectionProfile connProfile)
        {
            this.Model = connProfile;
        }
    }
}
