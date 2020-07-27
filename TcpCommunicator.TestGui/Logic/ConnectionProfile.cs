using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TcpCommunicator.TestGui.Data;

namespace TcpCommunicator.TestGui.Logic
{
    public class ConnectionProfile : IMessageReceiveHandler, IMessageCommunicatorLogger
    {
        private SynchronizationContext _syncContext;

        private MessageCommunicator _messageCommunicator;

        public string Name => this.Parameters.Name;

        public ConnectionParameters Parameters { get; }

        public ObservableCollection<LoggingMessageWrapper> DetailLogging { get; } = new ObservableCollection<LoggingMessageWrapper>();

        public ObservableCollection<LoggingMessageWrapper> Messages { get; } = new ObservableCollection<LoggingMessageWrapper>();

        public bool IsRunning => _messageCommunicator.IsRunning;

        public ConnectionState State => _messageCommunicator.State;

        public string RemoteEndpointDescription => _messageCommunicator.RemoteEndpointDescription;

        public ConnectionProfile(SynchronizationContext syncContext, ConnectionParameters connParams)
        {
            _syncContext = syncContext;
            this.Parameters = connParams;

            _messageCommunicator = SetupMessageCommunicator(connParams, this, this);
        }

        public async Task ChangeParametersAsync(ConnectionParameters newConnParameters)
        {
            var prefWasRunning = false;
            if (_messageCommunicator.IsRunning)
            {
                await _messageCommunicator.StopAsync();
                prefWasRunning = true;
            }

            _messageCommunicator = SetupMessageCommunicator(newConnParameters, this, this);

            if (prefWasRunning)
            {
                await _messageCommunicator.StartAsync();
            }
        }

        public async Task SendMessageAsync(string message)
        {
            if (await _messageCommunicator.SendAsync(new Message(message)))
            {
                var newLoggingMessage = new LoggingMessage(
                    DateTime.UtcNow, LoggingMessageType.Info, "OUT", message, null);

                LogTo(_syncContext, newLoggingMessage, this.DetailLogging);
                LogTo(_syncContext, newLoggingMessage, this.Messages);
            }
        }

        public Task StartAsync()
        {
            return _messageCommunicator.StartAsync();
        }

        public Task StopAsync()
        {
            return _messageCommunicator.StopAsync();
        }

        private static MessageCommunicator SetupMessageCommunicator(
            ConnectionParameters connParams,
            IMessageReceiveHandler messageReceiveHandler,
            IMessageCommunicatorLogger messageCommunicatorLogger)
        {
            ByteStreamHandlerSettings streamHandlerSettings;
            switch (connParams.Mode)
            {
                case ConnectionMode.Active:
                    streamHandlerSettings = new TcpActiveByteSteamHandlerSettings(connParams.Target, connParams.Port);
                    break;

                case ConnectionMode.Passive:
                    streamHandlerSettings = new TcpPassiveByteSteamHandlerSettings(IPAddress.Any, connParams.Port);
                    break;

                default:
                    throw new ArgumentOutOfRangeException($"Unknown connection mode: {connParams.Mode}");
            }

            MessageRecognizerSettings messageRecognizerSettings;
            switch (connParams.RecognitionMode)
            {
                case MessageRecognitionMode.Default:
                    var settingsRecognizerDefault = (MessageRecognizerDefaultSettings)connParams.RecognizerSettings;
                    messageRecognizerSettings = new DefaultMessageRecognizerSettings(
                        Encoding.GetEncoding(settingsRecognizerDefault.Encoding));
                    break;

                case MessageRecognitionMode.EndSymbol:
                    var settingsRecognizerEndSymbol = (MessageRecognizerEndSymbolSettings)connParams.RecognizerSettings;
                    messageRecognizerSettings = new EndSymbolsMessageRecognizerSettings(
                        Encoding.GetEncoding(settingsRecognizerEndSymbol.Encoding),
                        settingsRecognizerEndSymbol.EndSymbols);
                    break;

                case MessageRecognitionMode.FixedLengthAndEndSymbol:
                    var settingsRecognizerFixedLengthAndEndSymbol =
                        (MessageRecognizerFixedLengthAndEndSymbolsSettings) connParams.RecognizerSettings;
                    messageRecognizerSettings = new FixedLengthAndEndSymbolsMessageRecognizerSettings(
                        Encoding.GetEncoding(settingsRecognizerFixedLengthAndEndSymbol.Encoding),
                        settingsRecognizerFixedLengthAndEndSymbol.EndSymbols,
                        settingsRecognizerFixedLengthAndEndSymbol.LengthIncludingEndSymbols,
                        settingsRecognizerFixedLengthAndEndSymbol.FillSymbol);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new MessageCommunicator(
                streamHandlerSettings, messageRecognizerSettings,
                messageReceiveHandler,
                messageCommunicatorLogger);
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

        public void OnMessageReceived(Message message)
        {
            try
            {
                var newLoggingMessage = new LoggingMessage(
                    DateTime.UtcNow, LoggingMessageType.Info, "IN", message.ToString(), null);

                LogTo(_syncContext, newLoggingMessage, this.DetailLogging);
                LogTo(_syncContext, newLoggingMessage, this.Messages);

                message.ClearAndReturnToPool();
            }
            finally
            {
                message.ClearAndReturnToPool();
            }
        }

        public void Log(LoggingMessage loggingMessage)
        {
            LogTo(_syncContext, loggingMessage, this.DetailLogging);
        }
    }
}
