using System;
using System.Collections.ObjectModel;
using System.Net;
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

        public ObservableCollection<LoggingMessageWrapper> DetailLogging { get; } = new ObservableCollection<LoggingMessageWrapper>();

        public ObservableCollection<LoggingMessageWrapper> Messages { get; } = new ObservableCollection<LoggingMessageWrapper>();

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

        public async Task SendMessageAsync(string message)
        {
            if (await _messageRecognizer.SendAsync(message))
            {
                var newLoggingMessage = new LoggingMessage(
                    _tcpCommunicator, DateTime.UtcNow, LoggingMessageType.Info, $"Message sent: {message}", null);

                LogTo(_syncContext, newLoggingMessage, this.DetailLogging);
                LogTo(_syncContext, newLoggingMessage, this.Messages);
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
                    tcpCommunicator = new TcpCommunicatorPassive(IPAddress.Any, connParams.Port);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
            tcpCommunicator.Logger = this.OnLoggingMessage;

            // Build the MessageRecognizer
            MessageRecognizerBase messageRecognizer;
            switch (connParams.RecognitionMode)
            {
                case MessageRecognitionMode.Default:
                    var settingsRecognizerDefault = (MessageRecognizerDefaultSettings)connParams.RecognizerSettings;
                    messageRecognizer = new DefaultMessageRecognizer(
                        tcpCommunicator, 
                        Encoding.GetEncoding(settingsRecognizerDefault.Encoding));
                    break;

                case MessageRecognitionMode.EndSymbol:
                    var settingsRecognizerEndSymbol = (MessageRecognizerEndSymbolSettings)connParams.RecognizerSettings;
                    messageRecognizer = new EndSymbolMessageRecognizer(
                        tcpCommunicator, 
                        Encoding.GetEncoding(settingsRecognizerEndSymbol.Encoding), 
                        new char[]{ '#', '#' });
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
            messageRecognizer.ReceiveHandler = OnMessageReceived;

            _tcpCommunicator = tcpCommunicator;
            _messageRecognizer = messageRecognizer;
        }

        private static void LogTo(SynchronizationContext syncContext, LoggingMessage logMessage, ObservableCollection<LoggingMessageWrapper> collection)
        {
            syncContext.Post(arg =>
            {
                collection.Insert(0, new LoggingMessageWrapper(logMessage));
                while (collection.Count > 1000)
                {
                    collection.RemoveAt(1000);
                }
            }, null);
        }

        private void OnLoggingMessage(LoggingMessage logMessage)
        {
            LogTo(_syncContext, logMessage, this.DetailLogging);
        }

        private void OnMessageReceived(Message message)
        {
            try
            {
                var newLoggingMessage = new LoggingMessage(
                    _tcpCommunicator, DateTime.UtcNow, LoggingMessageType.Info, $"Message received: {message.RawMessage}", null);

                LogTo(_syncContext, newLoggingMessage, this.DetailLogging);
                LogTo(_syncContext, newLoggingMessage, this.Messages);
            }
            finally
            {
                message.ClearAndReturnToPool();
            }
        }
    }
}
