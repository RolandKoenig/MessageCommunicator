using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;

namespace TcpCommunicator.TestGui
{
    public class ConnectionProfile
    {
        private SynchronizationContext _syncContext;

        public string Name { get; }

        public ObservableCollection<LoggingMessage> Logging { get; } = new ObservableCollection<LoggingMessage>();

        public ConnectionProfile(SynchronizationContext syncContext, string name)
        {
            _syncContext = syncContext;

            this.Name = name;
        }

        public void Start()
        {

        }

        public void Stop()
        {

        }
    }
}
