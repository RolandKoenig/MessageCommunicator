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
        private Encoding _encoding;

        public MessageCommunicator(
            ByteStreamHandlerSettings byteStreamHandlerSettings, 
            MessageRecognizer messageRecognizer, 
            IMessageReceiveHandler receiveHandler,
            Encoding? encoding = null)
        {
            _byteStreamHandler = byteStreamHandlerSettings.CreateByteStreamHandler();
            _messageRecognizer = messageRecognizer;
            _receiveHandler = receiveHandler;

            _encoding = encoding ?? Encoding.ASCII;
        }

        public Task<bool> SendAsync(Message message)
        {
            return Task.FromResult(false);
        }

        public Task StartAsync()
        {
            return Task.FromResult<object?>(null);
        }

        public Task StopAsync()
        {
            return Task.FromResult<object?>(null);
        }

        public ConnectionState State => _byteStreamHandler.State;
    }
}
