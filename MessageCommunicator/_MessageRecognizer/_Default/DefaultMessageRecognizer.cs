using System;
using System.Text;
using System.Threading.Tasks;
using MessageCommunicator.Util;

// Type aliases for supporting lower .net standard
#if NETSTANDARD1_3
using MemoryOfByte = MessageCommunicator.ReadOnlySegment<byte>;
using ReadOnlySpanOfByte = MessageCommunicator.ReadOnlySegment<byte>;
using ReadOnlySpanOfChar = MessageCommunicator.ReadOnlySegment<char>;
#else
using MemoryOfByte = System.Memory<byte>;
using ReadOnlySpanOfByte = System.ReadOnlySpan<byte>;
using ReadOnlySpanOfChar = System.ReadOnlySpan<char>;
#endif

namespace MessageCommunicator
{
    public class DefaultMessageRecognizer : MessageRecognizer
    {
        private const char SYMBOL_START = '<';
        private const char SYMBOL_END = '>';
        private const char SYMBOL_DELIMITER = '|';

        private Encoding _encoding;
        private Decoder _decoder;
        private StringBuffer _receiveStringBuffer;

        public DefaultMessageRecognizer(Encoding encoding)
        {
            _encoding = encoding;
            _decoder = _encoding.GetDecoder();
            _receiveStringBuffer = new StringBuffer(1024);
        }

        /// <inheritdoc />
        protected override Task<bool> SendInternalAsync(IByteStreamHandler byteStreamHandler, ReadOnlySpanOfChar rawMessage)
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
                if(rawMessage.Length > 0){ sendBuffer.Append(rawMessage); }
                sendBuffer.Append(SYMBOL_END);
                sendBuffer.GetInternalData(out var buffer, out var currentCount);

                var sendMessageByteLength = _encoding.GetByteCount(buffer, 0, currentCount);
                bytes = ByteArrayPool.Take(sendMessageByteLength);

                _encoding.GetBytes(buffer, 0, currentCount, bytes, 0);
                StringBuffer.Release(sendBuffer);
                sendBuffer = null;

                return byteStreamHandler.SendAsync(
                    new MemoryOfByte(bytes, 0, sendMessageByteLength));
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
        public override void OnReceivedBytes(bool isNewConnection, ReadOnlySpanOfByte receivedBytes)
        {
            // Clear receive buffer on new connections
            if (isNewConnection)
            {
                _receiveStringBuffer.Clear();
                _decoder.Reset();
            }

            // Parse characters
            if (receivedBytes.Length == 0) { return; }
            var addedChars = _receiveStringBuffer.Append(receivedBytes, _decoder);
            if (addedChars == 0) { return; }

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
                    if (rawMessageLength > 0)
                    {
                        recognizedMessage.RawMessage.Append(_receiveStringBuffer.GetPartReadOnly(
                            delimiterIndex + 1, rawMessageLength));
                    }
                    receiveHandler.OnMessageReceived(recognizedMessage);
                }

                // Remove the message with endsymbols from receive buffer
                _receiveStringBuffer.RemoveFromStart(fullMessageLength);
            }
        }
    }
}
