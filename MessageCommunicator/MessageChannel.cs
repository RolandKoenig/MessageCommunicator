using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MessageCommunicator.Util;

namespace MessageCommunicator
{
    public class MessageChannel
    {
        private ByteStreamHandler _byteStreamHandler;
        private MessageRecognizer _messageRecognizer;

        public ConnectionState State => _byteStreamHandler.State;

        public bool IsRunning => _byteStreamHandler.IsRunning;

        public string LocalEndpointDescription => _byteStreamHandler.LocalEndpointDescription;

        public string RemoteEndpointDescription => _byteStreamHandler.RemoteEndpointDescription;

        public IMessageReceiveHandler ReceiveHandler { get; }

        public MessageChannel(
            ByteStreamHandlerSettings byteStreamHandlerSettings, 
            MessageRecognizerSettings messageRecognizerSettings, 
            IMessageReceiveHandler receiveHandler,
            IMessageCommunicatorLogger? logger = null)
        {
            _byteStreamHandler = byteStreamHandlerSettings.CreateByteStreamHandler();
            _byteStreamHandler.Logger = logger;

            _messageRecognizer = messageRecognizerSettings.CreateMessageRecognizer();
            _messageRecognizer.ReceiveHandler = receiveHandler;
            _messageRecognizer.Logger = logger;

            _byteStreamHandler.RegisterMessageRecognizer(_messageRecognizer);

            this.ReceiveHandler = receiveHandler;
        }

        public MessageChannel(
            ByteStreamHandlerSettings byteStreamHandlerSettings,
            MessageRecognizerSettings messageRecognizerSettings,
            Action<Message> receiveHandler,
            IMessageCommunicatorLogger? logger = null)
            : this(
                byteStreamHandlerSettings, messageRecognizerSettings,
                new DelegateMessageReceiveHandler(receiveHandler),
                logger)
        {

        }

        public Task<bool> SendAsync(Message message)
        {
            return _messageRecognizer.SendAsync(message.ToString());
        }

        public Task StartAsync()
        {
            return _byteStreamHandler.StartAsync();
        }

        public Task StopAsync()
        {
            return _byteStreamHandler.StopAsync();
        }
    }
}
