using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Light.GuardClauses;
using MessageCommunicator.Util;

namespace MessageCommunicator
{
    /// <summary>
    /// This class provides all functionality to build up a communication channel with a remote partner.
    /// </summary>
    public class MessageChannel
    {
        private ByteStreamHandler _byteStreamHandler;
        private MessageRecognizer _messageRecognizer;
        private IMessageChannelConnectionObserver? _connectionObserver;

        /// <summary>
        /// Gets the current state of the underlying connection (<see cref="IByteStreamHandler"/>).
        /// </summary>
        public ConnectionState State => _byteStreamHandler.State;

        /// <summary>
        /// Returns true if this <see cref="MessageChannel"/> is running currently.
        /// If the <see cref="MessageChannel"/> is running than this does not mean automatically that it is connected to a remote partner.
        /// </summary>
        public bool IsRunning => _byteStreamHandler.IsRunning;

        /// <summary>
        /// Gets a short description of the local endpoint when started / connected.
        /// </summary>
        public string LocalEndpointDescription => _byteStreamHandler.LocalEndpointDescription;

        /// <summary>
        /// Gets a short description of the remote endpoint when started / connected.
        /// </summary>
        public string RemoteEndpointDescription => _byteStreamHandler.RemoteEndpointDescription;

        /// <summary>
        /// Gets or sets the <see cref="IMessageReceiveHandler"/> which gets notified on a received <see cref="Message"/>.
        /// </summary>
        public IMessageReceiveHandler? ReceiveHandler
        {
            get => _messageRecognizer.ReceiveHandler;
            set => _messageRecognizer.ReceiveHandler = value;
        }

        /// <summary>
        /// Gets the timestamp in UTC when we've received the last <see cref="Message"/>.
        /// </summary>
        public DateTime LastReceivedTimestampUtc => _messageRecognizer.LastReceivedTimestampUtc;

        /// <summary>
        /// Gets the timestamp from the last successful connection (utc).
        /// </summary>
        public DateTime LastSuccessfulConnectTimestampUtc => _byteStreamHandler.LastSuccessfulConnectTimestampUtc;

        /// <summary>
        /// Access to internal objects.
        /// Be careful when using them, wrong method calls can cause unexpected state!
        /// </summary>
        public MessageChannelInternals Internals { get; }

        /// <summary>
        /// Creates a new <see cref="MessageChannel"/> object.
        /// </summary>
        /// <param name="byteStreamHandlerSettings">Settings for building the <see cref="IByteStreamHandler"/>.</param>
        /// <param name="messageRecognizerSettings">Settings for building the <see cref="IMessageRecognizer"/>.</param>
        /// <param name="receiveHandler">The <see cref="IMessageReceiveHandler"/> which gets notified on a received <see cref="Message"/>.</param>
        /// <param name="logger">The <see cref="IMessageCommunicatorLogger"/> to which all logging messages are passed.</param>
        /// <param name="connectionObserver">An observer which can check for invalid connections (like no messages since a period of time).</param>
        public MessageChannel(
            ByteStreamHandlerSettings byteStreamHandlerSettings, 
            MessageRecognizerSettings messageRecognizerSettings, 
            IMessageReceiveHandler? receiveHandler = null,
            IMessageCommunicatorLogger? logger = null,
            IMessageChannelConnectionObserver? connectionObserver = null)
        {
            byteStreamHandlerSettings.MustNotBeNull(nameof(byteStreamHandlerSettings));
            messageRecognizerSettings.MustNotBeNull(nameof(messageRecognizerSettings));

            _connectionObserver = connectionObserver;

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

        /// <summary>
        /// Creates a new <see cref="MessageChannel"/> object.
        /// </summary>
        /// <param name="byteStreamHandlerSettings">Settings for building the <see cref="IByteStreamHandler"/>.</param>
        /// <param name="messageRecognizerSettings">Settings for building the <see cref="IMessageRecognizer"/>.</param>
        /// <param name="receiveHandler">A delegate which gets notified on a received <see cref="Message"/>.</param>
        /// <param name="logger">The <see cref="IMessageCommunicatorLogger"/> to which all logging messages are passed.</param>
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
        /// Triggers reconnect in case of an established connection.
        /// If connection is not established currently, then this channel stays already in connect mode.
        /// </summary>
        public void TriggerReconnect()
        {
            _byteStreamHandler.TriggerReconnect();
        }

        /// <summary>
        /// Waits until we've got a valid connection.
        /// </summary>
        public Task WaitForConnectionAsync()
        {
            return _byteStreamHandler.WaitForConnectionAsync(CancellationToken.None);
        }

        /// <summary>
        /// Waits until we've got a valid connection.
        /// </summary>
        /// <param name="cancelToken">The <see cref="CancellationToken"/> which can be used to cancel the wait task.</param>
        public Task WaitForConnectionAsync(CancellationToken cancelToken)
        {
            return _byteStreamHandler.WaitForConnectionAsync(cancelToken);
        }

        /// <summary>
        /// Sends the given message to the remote partner.
        /// </summary>
        /// <param name="rawMessage">The message to be sent as <see cref="ReadOnlySpan{T}"/>.</param>
        /// <returns>Returns true if message was sent successfully, otherwise false.</returns>
        public Task<bool> SendAsync(ReadOnlySpan<char> rawMessage)
        {
            return _messageRecognizer.SendAsync(rawMessage);
        }

        /// <summary>
        /// Sends the given message to the remote partner.
        /// </summary>
        /// <param name="rawMessage">The message to be sent as <see cref="string"/>.</param>
        /// <returns>Returns true if message was sent successfully, otherwise false.</returns>
        public Task<bool> SendAsync(string rawMessage)
        {
            rawMessage.MustNotBeNull(nameof(rawMessage));

            return _messageRecognizer.SendAsync(rawMessage.AsSpan());
        }

        /// <summary>
        /// Sends the given message to the remote partner.
        /// </summary>
        /// <param name="message">The message to be sent as <see cref="Message"/>.</param>
        /// <returns>Returns true if message was sent successfully, otherwise false.</returns>
        public Task<bool> SendAsync(Message message)
        {
            message.MustNotBeNull(nameof(message));

            return _messageRecognizer.SendAsync(message.GetSpanReadOnly());
        }

        /// <summary>
        /// Starts this channel.
        /// </summary>
        public Task StartAsync()
        {
            return _byteStreamHandler.StartAsync()
                .ContinueWith(task =>
                {
                    if (task.IsCompleted)
                    {
                        _connectionObserver?.RegisterMessageChannel(this);
                    }
                });
        }

        /// <summary>
        /// Stops this channel.
        /// </summary>
        public Task StopAsync()
        {
            return _byteStreamHandler.StopAsync()
                .ContinueWith(task =>
                {
                    if (task.IsCompleted)
                    {
                        _connectionObserver?.DeregisterMessageChannel(this);
                    }
                });
        }

        //*********************************************************************
        //*********************************************************************
        //*********************************************************************
        /// <summary>
        /// Gives access to underlying objects of a <see cref="MessageChannel"/>.
        /// </summary>
        public class MessageChannelInternals
        {
            private MessageChannel _owner;

            internal MessageChannelInternals(MessageChannel owner)
            {
                _owner = owner;
            }

            /// <summary>
            /// Gets the underlying <see cref="IByteStreamHandler"/> object.
            /// </summary>
            public IByteStreamHandler ByteStreamHandler => _owner._byteStreamHandler;

            /// <summary>
            /// Gets the underlying <see cref="IMessageRecognizer"/> object.
            /// </summary>
            public IMessageRecognizer MessageRecognizer => _owner._messageRecognizer;

            /// <summary>
            /// Gets the underlying <see cref="IMessageChannelConnectionObserver"/> object.
            /// </summary>
            public IMessageChannelConnectionObserver? ConnectionObserver => _owner._connectionObserver;
        }
    }
}
