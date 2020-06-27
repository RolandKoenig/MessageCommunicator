using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TcpCommunicator.TestGui.Data;

namespace TcpCommunicator.TestGui.Logic
{
    public class ConnectionProfile
    {
        private SynchronizationContext _syncContext;

        private TcpCommunicatorBase _tcpCommunicator;

        public string Name => this.Parameters.Name;

        public ConnectionParameters Parameters { get; private set; }

        public ObservableCollection<LoggingMessageWrapper> Logging { get; } = new ObservableCollection<LoggingMessageWrapper>();

        public bool IsRunning => _tcpCommunicator.IsRunning;

        public ConnectionState State => _tcpCommunicator.State;

        public string RemoteEndpointDescription => _tcpCommunicator.RemoteEndpointDescription;

        public ConnectionProfile(SynchronizationContext syncContext, ConnectionParameters connParams)
        {
            _syncContext = syncContext;
            this.Parameters = connParams;

            _tcpCommunicator = this.SetupTcpCommunicator(connParams);
        }

        public void ChangeParameters(ConnectionParameters newConnParameters)
        {
            var prefWasRunning = false;
            if (_tcpCommunicator.IsRunning)
            {
                _tcpCommunicator.Stop();
                prefWasRunning = true;
            }

            _tcpCommunicator = this.SetupTcpCommunicator(newConnParameters);

            if (prefWasRunning)
            {
                _tcpCommunicator.Start();
            }
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

        private TcpCommunicatorBase SetupTcpCommunicator(ConnectionParameters connParams)
        {
            this.Parameters = connParams;

            TcpCommunicatorBase result;
            switch (connParams.Mode)
            {
                case ConnectionMode.Active:
                    result = new TcpCommunicatorActive(connParams.Target, connParams.Port);
                    break;

                case ConnectionMode.Passive:
                    result = new TcpCommunicatorPassive(connParams.Port);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            result.Logger = this.OnLoggingMessage;

            return result;
        }

        private void OnLoggingMessage(LoggingMessage logMessage)
        {
            _syncContext.Post(arg =>
            {
                this.Logging.Insert(0, new LoggingMessageWrapper(logMessage));

                while (this.Logging.Count > 1000)
                {
                    this.Logging.RemoveAt(1000);
                }
            }, null);
        }
    }
}
