using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Light.GuardClauses;
using MessageCommunicator.Util;

namespace MessageCommunicator
{
    /// <summary>
    /// This <see cref="MessageRecognizer"/> implementation recognizes messages with one or more start and end symbols.
    /// </summary>
    public class StartAndEndSymbolsRecognizer : MessageRecognizer
    {
        private Encoding _encoding;
        private Decoder _decoder;
        private string _startSymbols;
        private string _endSymbols;
        private StringBuffer _receiveStringBuffer;

        /// <summary>
        /// Creates a new <see cref="EndSymbolsMessageRecognizer"/> instance.
        /// </summary>
        /// <param name="encoding">The <see cref="Encoding"/> to be used when convert characters to/from bytes.</param>
        /// <param name="startSymbols">The start symbols of received/sent messages.</param>
        /// <param name="endSymbols">The end symbols of received/sent messages.</param>
        public StartAndEndSymbolsRecognizer(Encoding encoding, string startSymbols, string endSymbols)
        {
            encoding.MustNotBeNull(nameof(encoding));
            startSymbols.MustNotBeNullOrEmpty(nameof(startSymbols));
            endSymbols.MustNotBeNullOrEmpty(nameof(endSymbols));

            _encoding = encoding;
            _startSymbols = startSymbols;
            _endSymbols = endSymbols;

            _decoder = encoding.GetDecoder();
            _receiveStringBuffer = new StringBuffer(1024);
        }

        /// <inheritdoc />
        protected override Task<bool> SendInternalAsync(IByteStreamHandler byteStreamHandler, ReadOnlySpan<char> rawMessage)
        {
            byteStreamHandler.MustNotBeNull(nameof(byteStreamHandler));

            // Check for start- and endsymbols inside the message
            TcpCommunicatorUtil.EnsureNoEndsymbolsInMessage(rawMessage, _endSymbols);

            // Perform message formatting
            var sendBuffer = StringBuffer.Acquire(rawMessage.Length + _endSymbols.Length);
            byte[]? bytes = null;
            try
            {
                sendBuffer.Append(_startSymbols, 0, _startSymbols.Length);
                if(rawMessage.Length > 0){ sendBuffer.Append(rawMessage); }
                sendBuffer.Append(_endSymbols, 0, _endSymbols.Length);
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
            if (receivedBytes.Count <= 0) { return; }
            var addedChars = _receiveStringBuffer.Append(receivedBytes, _decoder);
            if (addedChars == 0) { return; }

            // Check for correct start symbol
            if (_receiveStringBuffer.Count > _startSymbols.Length)
            {
                for (var loop = 0; loop < _startSymbols.Length; loop++)
                {
                    if (_receiveStringBuffer[loop] != _startSymbols[loop])
                    {
                        throw new MessageRecognitionException($"Invalid start symbol at index {loop}: Got {_receiveStringBuffer[loop]}, expected {_startSymbols[loop]}!");
                    }
                }
            }

            // Search for end symbol
            bool endSymbolsMatch;
            do
            {
                endSymbolsMatch = false;

                var receiveBufferCount = _receiveStringBuffer.Count;
                for (var indexReceiveBuffer = 0; indexReceiveBuffer < receiveBufferCount; indexReceiveBuffer++)
                {
                    if (_receiveStringBuffer[indexReceiveBuffer] != _endSymbols[0]){ continue; }

                    endSymbolsMatch = true;
                    var endSymbolIndex = indexReceiveBuffer;

                    // Check rest of endsymbol collection
                    for (var indexEndSymbol = 1; indexEndSymbol < _endSymbols.Length; indexEndSymbol++)
                    {
                        var actReceiveBufferIndex = indexReceiveBuffer + indexEndSymbol;
                        if ((actReceiveBufferIndex >= _receiveStringBuffer.Count) ||
                            (_receiveStringBuffer[actReceiveBufferIndex] != _endSymbols[indexEndSymbol]))
                        {
                            endSymbolsMatch = false;
                            endSymbolIndex = -1;
                            break;
                        }
                    }

                    // Raise found message
                    if (endSymbolsMatch)
                    {
                        // Raise found message
                        var messageLength = endSymbolIndex - _startSymbols.Length;
                        base.NotifyRecognizedMessage(
                            _receiveStringBuffer.GetPartReadOnly(_startSymbols.Length, messageLength));

                        // Remove the message with endsymbols from receive buffer
                        _receiveStringBuffer.RemoveFromStart(endSymbolIndex + _endSymbols.Length);
                        break;
                    }
                }

            } while (endSymbolsMatch);
        }
    }
}
