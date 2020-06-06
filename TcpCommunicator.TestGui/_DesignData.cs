using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using TcpCommunicator.TestGui.Data;
using TcpCommunicator.TestGui.Logic;
using TcpCommunicator.TestGui.ViewServices;

namespace TcpCommunicator.TestGui
{
    public static class DesignData
    {
        public static MainWindowViewModel MainWindowVM
        {
            get
            {
                var result = new MainWindowViewModel();
                var dummyConnParams = new ConnectionParameters(){ Name = "Dummy Profile" };

                var dummyContext = new DummySynchronizationContext();
                result.Profiles.Add(new ConnectionProfile(dummyContext, dummyConnParams));
                result.Profiles.Add(new ConnectionProfile(dummyContext, dummyConnParams));
                result.Profiles.Add(new ConnectionProfile(dummyContext, dummyConnParams));
                result.Profiles.Add(new ConnectionProfile(dummyContext, dummyConnParams));

                return result;
            }
        }

        public static ConnectionConfigViewModel ConnectionConfigVM => new ConnectionConfigViewModel();

        //*****************************************************************
        //*****************************************************************
        //*****************************************************************
        internal class DummySynchronizationContext : SynchronizationContext
        {
            /// <inheritdoc />
            public override void Send(SendOrPostCallback d, object? state)
            {
                
            }

            /// <inheritdoc />
            public override void Post(SendOrPostCallback d, object? state)
            {
                
            }
        }
    }
}
