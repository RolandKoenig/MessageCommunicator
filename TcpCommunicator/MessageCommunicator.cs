﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TcpCommunicator
{
    public class MessageCommunicator
    {
        private ByteStreamHandler _byteStreamHandler;
        private MessageRecognizer _messageRecognizer;

        public ConnectionState State => _byteStreamHandler.State;

        public bool IsRunning => _byteStreamHandler.IsRunning;

        public string LocalEndpointDescription => _byteStreamHandler.LocalEndpointDescription;

        public string RemoteEndpointDescription => _byteStreamHandler.RemoteEndpointDescription;

        public Encoding Encoding { get; }

        public IMessageReceiveHandler ReceiveHandler { get; }

        public MessageCommunicator(
            ByteStreamHandlerSettings byteStreamHandlerSettings, 
            MessageRecognizerSettings messageRecognizerSettings, 
            IMessageReceiveHandler receiveHandler,
            Encoding? encoding = null,
            IMessageCommunicatorLogger? logger = null)
        {
            this.Encoding = encoding ?? Encoding.ASCII;

            _byteStreamHandler = byteStreamHandlerSettings.CreateByteStreamHandler();
            _byteStreamHandler.Logger = logger;

            _messageRecognizer = messageRecognizerSettings.CreateMessageRecognizer(this.Encoding);
            _messageRecognizer.ReceiveHandler = receiveHandler;
            _messageRecognizer.Logger = logger;

            _byteStreamHandler.RegisterMessageRecognizer(_messageRecognizer);

            this.ReceiveHandler = receiveHandler;
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
