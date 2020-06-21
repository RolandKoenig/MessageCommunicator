using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TcpCommunicator.TestGui.Data;
using Tmds.DBus;

namespace TcpCommunicator.TestGui.Logic
{
    public class ConnectionProfile
    {
        private SynchronizationContext _syncContext;

        private TcpCommunicatorBase _tcpCommunicator;

        public string Name => this.Parameters.Name;

        public ConnectionParameters Parameters { get; }

        public ObservableCollection<LoggingMessageWrapper> Logging { get; } = new ObservableCollection<LoggingMessageWrapper>();

        public bool IsRunning => _tcpCommunicator.IsRunning;

        public ConnectionState State => _tcpCommunicator.State;

        public string RemoteEndpointDescription => _tcpCommunicator.RemoteEndpointDescription;

        public ConnectionProfile(SynchronizationContext syncContext, ConnectionParameters connParams)
        {
            _syncContext = syncContext;
            this.Parameters = connParams;

            switch (connParams.Mode)
            {
                case ConnectionMode.Active:
                    _tcpCommunicator = new TcpCommunicatorActive(connParams.Target, connParams.Port);
                    break;
                
                case ConnectionMode.Passive:
                    _tcpCommunicator = new TcpCommunicatorPassive(connParams.Port);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            _tcpCommunicator.Logger = this.OnLoggingMessage;
        }

        public Task SendMessageAsync(string message)
        {
            return _tcpCommunicator.SendAsync(
                Encoding.ASCII.GetBytes(message),
                false);
        }

        public void Start()
        {
            _tcpCommunicator.Start();
        }

        public void Stop()
        {
            _tcpCommunicator.Stop();
        }

        private void OnLoggingMessage(TcpCommunicator.LoggingMessage logMessage)
        {
            _syncContext.Post(new SendOrPostCallback(arg =>
            {
                this.Logging.Insert(0, new LoggingMessageWrapper(logMessage));

                while (this.Logging.Count > 1000)
                {
                    this.Logging.RemoveAt(1000);
                }
            }), null);
        }
    }
}
