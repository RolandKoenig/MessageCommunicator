using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
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

        public IMessageReceiveHandler? ReceiveHandler
        {
            get => _messageRecognizer.ReceiveHandler;
            set => _messageRecognizer.ReceiveHandler = value;
        }

        /// <summary>
        /// Access to internal objects.
        /// Be careful when using them, wrong method calls can cause unexpected state!
        /// </summary>
        public MessageChannelInternals Internals { get; }

        public MessageChannel(
            ByteStreamHandlerSettings byteStreamHandlerSettings, 
            MessageRecognizerSettings messageRecognizerSettings, 
            IMessageReceiveHandler? receiveHandler = null,
            IMessageCommunicatorLogger? logger = null)
        {
            _byteStreamHandler = byteStreamHandlerSettings.CreateByteStreamHandler();
            _byteStreamHandler.Logger = logger;

            _messageRecognizer = messageRecognizerSettings.CreateMessageRecognizer();
            _messageRecognizer.ReceiveHandler = receiveHandler;
            _messageRecognizer.Logger = logger;

            _byteStreamHandler.MessageRecognizer = _messageRecognizer;
            _messageRecognizer.ByteStreamHandler = _byteStreamHandler;

            this.ReceiveHandler = receiveHandler;
            
            this.Internals = new MessageChannelInternals(this);
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

        /// <summary>
        /// Waits until we've got a valid connection.
        /// </summary>
        public Task WaitForConnectionAsync(CancellationToken cancelToken)
        {
            return _byteStreamHandler.WaitForConnectionAsync(cancelToken);
        }

        public Task<bool> SendAsync(ReadOnlySpan<char> rawMessage)
        {
            return _messageRecognizer.SendAsync(rawMessage);
        }

        public Task<bool> SendAsync(string rawMessage)
        {
            return _messageRecognizer.SendAsync(rawMessage.AsSpan());
        }

        public Task<bool> SendAsync(Message message)
        {
            return _messageRecognizer.SendAsync(message.GetSpanReadOnly());
        }

        public Task StartAsync()
        {
            return _byteStreamHandler.StartAsync();
        }

        public Task StopAsync()
        {
            return _byteStreamHandler.StopAsync();
        }

        //*********************************************************************
        //*********************************************************************
        //*********************************************************************
        public class MessageChannelInternals
        {
            private MessageChannel _owner;

            public MessageChannelInternals(MessageChannel owner)
            {
                _owner = owner;
            }

            public IByteStreamHandler ByteStreamHandler => _owner._byteStreamHandler;

            public IMessageRecognizer MessageRecognizer => _owner._messageRecognizer;
        }
    }
}
