using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using TcpCommunicator.TestGui.Data;
using Tmds.DBus;

namespace TcpCommunicator.TestGui.Logic
{
    public class ConnectionProfile
    {
        private SynchronizationContext _syncContext;
        private ConnectionParameters _connParams;

        private TcpCommunicatorBase _tcpCommunicator;

        public string Name => _connParams.Name;

        public ObservableCollection<LoggingMessage> Logging { get; } = new ObservableCollection<LoggingMessage>();

        public bool IsRunning => _tcpCommunicator.IsRunning;

        public ConnectionProfile(SynchronizationContext syncContext, ConnectionParameters connParams)
        {
            _syncContext = syncContext;
            _connParams = connParams;

            switch (connParams.Mode)
            {
                case ConnectionMode.Active:
                    // TODO
                    break;
                
                case ConnectionMode.Passive:
                    _tcpCommunicator = new TcpCommunicatorPassive(connParams.Port);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Start()
        {
            _tcpCommunicator.Start();
        }

        public void Stop()
        {
            _tcpCommunicator.Stop();
        }
    }
}
