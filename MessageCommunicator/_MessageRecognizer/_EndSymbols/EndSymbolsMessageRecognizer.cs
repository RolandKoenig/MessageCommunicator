using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MessageCommunicator.Util;

namespace MessageCommunicator
{
    public class EndSymbolsMessageRecognizer : MessageRecognizer
    {
        private Encoding _encoding;
        private Decoder _decoder;
        private string _endSymbols;
        private StringBuffer _receiveStringBuffer;

        public EndSymbolsMessageRecognizer(Encoding encoding, string endSymbols)
        {
            if (string.IsNullOrEmpty(endSymbols))
            {
                throw new ArgumentException("Endsymbols must not be empty!", nameof(endSymbols));
            }

            _encoding = encoding;
            _decoder = encoding.GetDecoder();
            _endSymbols = endSymbols;
            _receiveStringBuffer = new StringBuffer(1024);
        }

        /// <inheritdoc />
        protected override Task<bool> SendInternalAsync(IByteStreamHandler byteStreamHandler, ReadOnlySpan<char> rawMessage)
        {
            // Check for endsymbols inside the message
            TcpCommunicatorUtil.EnsureNoEndsymbolsInMessage(rawMessage, _endSymbols);

            // Perform message formatting
            var sendBuffer = StringBuffer.Acquire(rawMessage.Length + _endSymbols.Length);
            byte[]? bytes = null;
            try
            {
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

        public override void OnReceivedBytes(bool isNewConnection, ArraySegment<byte> receivedSegment)
        {
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
                        // Cut out received message
                        var receiveHandler = this.ReceiveHandler;
                        if (receiveHandler != null)
                        {
                            var recognizedMessage = MessagePool.Rent(endSymbolIndex);
                            if (endSymbolIndex > 0)
                            {
                                recognizedMessage.RawMessage.Append(
                                    _receiveStringBuffer.GetPartReadOnly(0, endSymbolIndex));
                            }
                            receiveHandler.OnMessageReceived(recognizedMessage);
                        }

                        // Remove the message with endsymbols from receive buffer
                        _receiveStringBuffer.RemoveFromStart(endSymbolIndex + _endSymbols.Length);
                        break;
                    }
                }

            } while (endSymbolsMatch);
        }
    }
}
