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

        private TcpByteStreamHandler _tcpCommunicator;
        private MessageRecognizer _messageRecognizer;

        public string Name => this.Parameters.Name;

        public ConnectionParameters Parameters { get; }

        public ObservableCollection<LoggingMessageWrapper> DetailLogging { get; } = new ObservableCollection<LoggingMessageWrapper>();

        public ObservableCollection<LoggingMessageWrapper> Messages { get; } = new ObservableCollection<LoggingMessageWrapper>();

        public bool IsRunning => _tcpCommunicator.IsRunning;

        public ConnectionState State => _tcpCommunicator.State;

        public string RemoteEndpointDescription => _tcpCommunicator.RemoteEndpointDescription;

        public ConnectionProfile(SynchronizationContext syncContext, ConnectionParameters connParams)
        {
            _syncContext = syncContext;
            this.Parameters = connParams;

            (_tcpCommunicator, _messageRecognizer) = SetupTcpCommunicator(connParams);
            _tcpCommunicator.Logger = this.OnLoggingMessage;
            _messageRecognizer.ReceiveHandler = OnMessageReceived;
        }

        public void ChangeParameters(ConnectionParameters newConnParameters)
        {
            var prefWasRunning = false;
            if (_tcpCommunicator.IsRunning)
            {
                _tcpCommunicator.Stop();
                prefWasRunning = true;
            }

            (_tcpCommunicator, _messageRecognizer) = SetupTcpCommunicator(newConnParameters);
            _tcpCommunicator.Logger = this.OnLoggingMessage;
            _messageRecognizer.ReceiveHandler = OnMessageReceived;

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
                    _tcpCommunicator, DateTime.UtcNow, LoggingMessageType.Info, "OUT", message, null);

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

        private static (TcpByteStreamHandler, MessageRecognizer) SetupTcpCommunicator(ConnectionParameters connParams)
        {
            //this.Parameters = connParams;

            // Build the TcpCommunicator
            TcpByteStreamHandler tcpCommunicator;
            switch (connParams.Mode)
            {
                case ConnectionMode.Active:
                    tcpCommunicator = new TcpActiveByteStreamHandler(connParams.Target, connParams.Port);
                    break;

                case ConnectionMode.Passive:
                    tcpCommunicator = new TcpPassiveByteStreamHandler(IPAddress.Any, connParams.Port);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            // Build the MessageRecognizer
            MessageRecognizer messageRecognizer;
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
                        new[]{ '#', '#' });
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return (tcpCommunicator, messageRecognizer);
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
                    _tcpCommunicator, DateTime.UtcNow, LoggingMessageType.Info, "IN", message.ToString(), null);

                LogTo(_syncContext, newLoggingMessage, this.DetailLogging);
                LogTo(_syncContext, newLoggingMessage, this.Messages);

                message.ClearAndReturnToPool();
            }
            finally
            {
                message.ClearAndReturnToPool();
            }
        }
    }
}
