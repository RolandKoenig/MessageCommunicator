using System;
using System.Text;
using System.Threading.Tasks;
using Light.GuardClauses;
using MessageCommunicator.Util;

namespace MessageCommunicator
{
    /// <summary>
    /// This <see cref="MessageRecognizer"/> implements a custom messages style of the MessageCommunicator
    /// project. 
    /// </summary>
    public class DefaultMessageRecognizer : MessageRecognizer
    {
        private const char SYMBOL_START = '<';
        private const char SYMBOL_END = '>';
        private const char SYMBOL_DELIMITER = '|';

        private Encoding _encoding;
        private Decoder _decoder;
        private StringBuffer _receiveStringBuffer;

        /// <summary>
        /// Creates a new <see cref="DefaultMessageRecognizer"/> instance.
        /// </summary>
        /// <param name="encoding">The <see cref="Encoding"/> to be used when convert characters to/from bytes.</param>
        public DefaultMessageRecognizer(Encoding encoding)
        {
            encoding.MustNotBeNull(nameof(encoding));

            _encoding = encoding;
            _decoder = _encoding.GetDecoder();
            _receiveStringBuffer = new StringBuffer(1024);
        }

        /// <inheritdoc />
        protected override Task<bool> SendInternalAsync(IByteStreamHandler byteStreamHandler, ReadOnlySpan<char> rawMessage)
        {
            byteStreamHandler.MustNotBeNull(nameof(byteStreamHandler));

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
                    new ArraySegment<byte>(bytes, 0, sendMessageByteLength));
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
        public override void OnReceivedBytes(bool isNewConnection, ArraySegment<byte> receivedBytes)
        {
            receivedBytes.MustNotBeDefault(nameof(receivedBytes));

            // Clear receive buffer on new connections
            if (isNewConnection)
            {
                _receiveStringBuffer.Clear();
                _decoder.Reset();
            }

            // Parse characters
            if (receivedBytes.Count == 0) { return; }
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

                    if (!TcpCommunicatorUtil.IsNumeric(_receiveStringBuffer[loop]))
                    {
                        throw new MessageRecognitionException($"Error during message recognition. Symbol in length field is not numeric (current index: {loop})!");
                    }
                    if (loop > 11)
                    {
                        throw new MessageRecognitionException($"Error during message recognition. Length field too long (current index: {loop})!");
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
                base.NotifyRecognizedMessage(_receiveStringBuffer.GetPartReadOnly(
                    delimiterIndex + 1, rawMessageLength));

                // Remove the message with endsymbols from receive buffer
                _receiveStringBuffer.RemoveFromStart(fullMessageLength);
            }
        }
    }
}
