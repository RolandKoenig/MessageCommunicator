using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TcpCommunicator.Util;

namespace TcpCommunicator
{
    public class EndSymbolMessageRecognizer : MessageRecognizerBase
    {
        private Encoding _encoding;
        private char[] _endSymbols;
        private StringBuffer _receiveStringBuffer;

        public Action<Message>? ReceiveHandler { get; set; }

        public EndSymbolMessageRecognizer(ITcpCommunicator communicator, Encoding encoding, char[] endSymbols)
            : base(communicator)
        {
            _encoding = encoding;
            _endSymbols = endSymbols;
            _receiveStringBuffer = new StringBuffer(1024);

            communicator.ReceiveHandler = this.OnReceiveBytes;
        }

        /// <inheritdoc />
        public override async Task SendAsync(string rawMessage)
        {
            var sendBuffer = StringBuffer.Acquire(rawMessage.Length + _endSymbols.Length);
            byte[]? bytes = null;
            try
            {
                sendBuffer.Append(rawMessage);
                sendBuffer.Append(_endSymbols, 0, _endSymbols.Length);
                sendBuffer.GetInternalData(out var buffer, out var currentCount);

                var sendMessageByteLength = _encoding.GetByteCount(buffer, 0, currentCount);
                bytes = ByteArrayPool.Take(sendMessageByteLength);

                _encoding.GetBytes(buffer, 0, currentCount, bytes, 0);
                StringBuffer.Release(sendBuffer);
                sendBuffer = null;

                await this.Communicator.SendAsync(
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

        private void OnReceiveBytes(ArraySegment<byte> receivedSegment)
        {
            if (receivedSegment.Array == null) { return; }
            if (receivedSegment.Count == 0) { return; }

            _receiveStringBuffer.Append(receivedSegment, _encoding);

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
                            var recognizedMessage = MessagePool.Take(endSymbolIndex);
                            recognizedMessage.RawMessage.Append(_receiveStringBuffer.GetPart(0, endSymbolIndex));
                            receiveHandler(recognizedMessage);
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
