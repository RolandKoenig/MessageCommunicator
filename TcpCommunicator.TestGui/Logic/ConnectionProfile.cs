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
        private MessageRecognizerBase _messageRecognizer;

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

            this.SetupTcpCommunicator(connParams);
        }

        public void ChangeParameters(ConnectionParameters newConnParameters)
        {
            var prefWasRunning = false;
            if (_tcpCommunicator.IsRunning)
            {
                _tcpCommunicator.Stop();
                prefWasRunning = true;
            }

            this.SetupTcpCommunicator(newConnParameters);

            if (prefWasRunning)
            {
                _tcpCommunicator.Start();
            }
        }

        public Task SendMessageAsync(string message)
        {
            return _messageRecognizer.SendAsync(message);
        }

        public void Start()
        {
            _tcpCommunicator.Start();
        }

        public void Stop()
        {
            _tcpCommunicator.Stop();
        }

        private void SetupTcpCommunicator(ConnectionParameters connParams)
        {
            this.Parameters = connParams;

            // Build the TcpCommunicator
            TcpCommunicatorBase tcpCommunicator;
            switch (connParams.Mode)
            {
                case ConnectionMode.Active:
                    tcpCommunicator = new TcpCommunicatorActive(connParams.Target, connParams.Port);
                    break;

                case ConnectionMode.Passive:
                    tcpCommunicator = new TcpCommunicatorPassive(connParams.Port);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
            tcpCommunicator.Logger = this.OnLoggingMessage;

            // Build the MessageRecognizer
            var messageRecognizer = new EndSymbolMessageRecognizer(
                tcpCommunicator, Encoding.Unicode, new []{ '#', '#'});
            messageRecognizer.ReceiveHandler = OnMessageReceived;

            _tcpCommunicator = tcpCommunicator;
            _messageRecognizer = messageRecognizer;
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

        private void OnMessageReceived(Message message)
        {
            try
            {
                this.OnLoggingMessage(
                    new LoggingMessage(
                        _tcpCommunicator, DateTime.UtcNow, LoggingMessageType.Info, message.RawMessage.ToString(), null));
            }
            finally
            {
                message.ClearAndReturnToPool();
            }
        }
    }
}
