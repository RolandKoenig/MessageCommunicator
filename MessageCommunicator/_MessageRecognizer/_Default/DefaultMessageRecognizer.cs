using System;
using System.Text;
using System.Threading.Tasks;
using MessageCommunicator.Util;

namespace MessageCommunicator
{
    public class DefaultMessageRecognizer : MessageRecognizer
    {
        private const char SYMBOL_START = '<';
        private const char SYMBOL_END = '>';
        private const char SYMBOL_DELIMITER = '|';
        private const string FORMAT = "n";

        private Encoding _encoding;
        private StringBuffer _receiveStringBuffer;

        public DefaultMessageRecognizer(Encoding encoding)
        {
            _encoding = encoding;
            _receiveStringBuffer = new StringBuffer(1024);
        }

        /// <inheritdoc />
        protected override async Task<bool> SendInternalAsync(ByteStreamHandler byteStreamHandler, string rawMessage)
        {
            var rawMessageLength = rawMessage.Length;
            var lengthDigitCount = TcpCommunicatorUtil.GetCountOfDigits(rawMessageLength);
            var sendBuffer = StringBuffer.Acquire(rawMessageLength + 3 + lengthDigitCount);

            byte[]? bytes = null;
            try
            {
                sendBuffer.Append(SYMBOL_START);
                sendBuffer.Append(rawMessageLength, StringView.Empty);
                sendBuffer.Append(SYMBOL_DELIMITER);
                sendBuffer.Append(rawMessage);
                sendBuffer.Append(SYMBOL_END);
                sendBuffer.GetInternalData(out var buffer, out var currentCount);

                var sendMessageByteLength = _encoding.GetByteCount(buffer, 0, currentCount);
                bytes = ByteArrayPool.Take(sendMessageByteLength);

                _encoding.GetBytes(buffer, 0, currentCount, bytes, 0);
                StringBuffer.Release(sendBuffer);
                sendBuffer = null;

                return await byteStreamHandler.SendAsync(
                    new ReadOnlyMemory<byte>(bytes, 0, sendMessageByteLength));
            }
            finally
            {
                if (bytes != null)
                {
                    ByteArrayPool.Return(bytes);
                }
                if (sendBuffer != null)
                {
                    StringBuffer.Release(sendBuffer);
                }
            }
        }

        /// <inheritdoc />
        public override void OnReceivedBytes(bool isNewConnection, ReadOnlySpan<byte> receivedBytes)
        {
            // Clear receive buffer on new connections
            if (isNewConnection) { _receiveStringBuffer.Clear(); }

            if (receivedBytes.Length == 0) { return; }
            _receiveStringBuffer.Append(receivedBytes, _encoding);

            while(_receiveStringBuffer.Count > 0)
            {
                // Check for start symbol
                if (_receiveStringBuffer[0] != SYMBOL_START)
                {
                    throw new MessageRecognitionException($"Error during message recognition. Expected '{SYMBOL_START}' at the start of a message, got '{_receiveStringBuffer[0]}'!");
                }

                // Search delimiter
                var delimiterIndex = -1;
                for (var loop = 1; loop < _receiveStringBuffer.Count; loop++)
                {
                    if (_receiveStringBuffer[loop] == SYMBOL_DELIMITER)
                    {
                        delimiterIndex = loop;
                        break;
                    }
                }
                if (delimiterIndex == -1) { break; }

                // Parse message count
                int rawMessageLength;
                try
                {
                    rawMessageLength = TcpCommunicatorUtil.ParseInt32FromStringPart(
                        _receiveStringBuffer,
                        1, delimiterIndex - 1);
                }
                catch(Exception ex)
                {
                    throw new MessageRecognitionException($"Unable to parse message length: {ex.Message}");
                }

                // Look whether we've received the full message
                var fullMessageLength = delimiterIndex + rawMessageLength + 2;
                if (_receiveStringBuffer.Count < fullMessageLength) { break; }

                // Check endsymbol
                if(_receiveStringBuffer[fullMessageLength-1] != SYMBOL_END)
                {
                    throw new MessageRecognitionException($"Error during message recognition. Expected '{SYMBOL_END}' at the end of a message, got '{_receiveStringBuffer[fullMessageLength - 1]}'!");
                }

                // Raise found message
                var receiveHandler = this.ReceiveHandler;
                if (receiveHandler != null)
                {
                    var recognizedMessage = MessagePool.Rent(rawMessageLength);
                    recognizedMessage.RawMessage.Append(_receiveStringBuffer.GetPart(delimiterIndex + 1, rawMessageLength));
                    receiveHandler.OnMessageReceived(recognizedMessage);
                }

                // Remove the message with endsymbols from receive buffer
                _receiveStringBuffer.RemoveFromStart(fullMessageLength);
            }
        }
    }
}
