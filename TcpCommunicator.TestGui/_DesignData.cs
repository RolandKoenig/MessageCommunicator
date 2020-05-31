using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace TcpCommunicator.TestGui
{
    public static class DesignData
    {
        public static MainWindowViewModel MainWindowVM
        {
            get
            {
                var result = new MainWindowViewModel();

                var dummyContext = new DummySynchronizationContext();
                result.Profiles.Add(new ConnectionProfile(dummyContext, "Profile 1"));
                result.Profiles.Add(new ConnectionProfile(dummyContext, "Profile 2"));
                result.Profiles.Add(new ConnectionProfile(dummyContext, "Profile 3"));
                result.Profiles.Add(new ConnectionProfile(dummyContext, "Profile 4"));

                return result;
            }


        }

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
