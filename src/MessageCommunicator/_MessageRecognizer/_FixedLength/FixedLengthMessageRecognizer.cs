using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Light.GuardClauses;
using MessageCommunicator.Util;

namespace MessageCommunicator
{
    /// <summary>
    /// This <see cref="MessageRecognizer"/> implementation recognizes messages with a fixed length.
    /// </summary>
    public class FixedLengthMessageRecognizer : MessageRecognizer
    {
        private Encoding _encoding;
        private Decoder _decoder;
        private int _length;
        private char _fillSymbol;
        private StringBuffer _receiveStringBuffer;

        /// <summary>
        /// Creates a new <see cref="FixedLengthMessageRecognizer"/> instance.
        /// </summary>
        /// <param name="encoding">The <see cref="Encoding"/> to be used when convert characters to/from bytes.</param>
        /// <param name="length">Total length of received/sent messages.</param>
        /// <param name="fillSymbol">Fill symbol for messages shorter than the fixed length.</param>
        public FixedLengthMessageRecognizer(Encoding encoding, int length, char fillSymbol)
        {
            encoding.MustNotBeNull(nameof(encoding));
            length.MustBeGreaterThan(0, nameof(length));

            _encoding = encoding;
            _decoder = _encoding.GetDecoder();
            _receiveStringBuffer = new StringBuffer(1024);
            _length = length;
            _fillSymbol = fillSymbol;
        }

        /// <inheritdoc />
        protected override Task<bool> SendInternalAsync(IByteStreamHandler byteStreamHandler, ReadOnlySpan<char> rawMessage)
        {
            byteStreamHandler.MustNotBeNull(nameof(byteStreamHandler));

            // Check for valid message
            if (rawMessage.Length > _length)
            {
                throw new ArgumentException(
                    "Given message too long. " +
                    $"Maximum length: {(_length)}, " +
                    $"given message length: {rawMessage.Length}",
                    nameof(rawMessage));
            }

            // Perform message formatting
            var sendBuffer = StringBuffer.Acquire(_length);
            byte[]? bytes = null;
            try
            {
                if(rawMessage.Length > 0){ sendBuffer.Append(rawMessage); }
                while (sendBuffer.Count < _length)
                {
                    sendBuffer.Append(_fillSymbol);
                }
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
        public override void OnReceivedBytes(bool isNewConnection, ArraySegment<byte> receivedSegment)
        {
            receivedSegment.MustNotBeDefault(nameof(receivedSegment));

            // Clear receive buffer on new connections
            if (isNewConnection)
            {
                _receiveStringBuffer.Clear();
                _decoder.Reset();
            }

            // Parse characters
            if (receivedSegment.Count <= 0) { return; }
            var addedChars = _receiveStringBuffer.Append(receivedSegment, _decoder);
            if (addedChars == 0) { return; }

            while (_receiveStringBuffer.Count >= _length)
            {
                // Check for fill symbols
                var messageLength = 0;
                for (var loop = _length - 1; loop >= 0; loop--)
                {
                    if (_receiveStringBuffer[loop] != _fillSymbol)
                    {
                        messageLength = loop + 1;
                        break;
                    }
                }

                // Raise found message
                base.NotifyRecognizedMessage(
                    _receiveStringBuffer.GetPartReadOnly(0, messageLength));

                // Remove the message with endsymbols from receive buffer
                _receiveStringBuffer.RemoveFromStart(_length);
            }
        }
    }
}
