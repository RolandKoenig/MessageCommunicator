using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TcpCommunicator
{
    public class MessageCommunicator
    {
        private ByteStreamHandler _byteStreamHandler;
        private MessageRecognizer _messageRecognizer;
        private IMessageReceiveHandler _receiveHandler;
        private IMessageCommunicatorLogger? _logger;

        public ConnectionState State => _byteStreamHandler.State;

        public bool IsRunning => _byteStreamHandler.IsRunning;

        public string LocalEndpointDescription => _byteStreamHandler.LocalEndpointDescription;

        public string RemoteEndpointDescription => _byteStreamHandler.RemoteEndpointDescription;

        public Encoding Encoding { get; }

        public MessageCommunicator(
            ByteStreamHandlerSettings byteStreamHandlerSettings, 
            MessageRecognizerSettings messageRecognizerSettings, 
            IMessageReceiveHandler receiveHandler,
            Encoding? encoding = null,
            IMessageCommunicatorLogger? logger = null)
        {
            this.Encoding = encoding ?? Encoding.ASCII;

            _logger = logger;

            _byteStreamHandler = byteStreamHandlerSettings.CreateByteStreamHandler();
            _byteStreamHandler.Logger = _logger;

            _messageRecognizer = messageRecognizerSettings.CreateMessageRecognizer(_byteStreamHandler, this.Encoding);
            _messageRecognizer.ReceiveHandler = receiveHandler;

            _receiveHandler = receiveHandler;
            
        }

        public Task<bool> SendAsync(Message message)
        {
            return _messageRecognizer.SendAsync(message.ToString());
        }

        public void Start()
        {
            _byteStreamHandler.Start();
        }

        public void Stop()
        {
            _byteStreamHandler.Stop();
        }
    }
}
