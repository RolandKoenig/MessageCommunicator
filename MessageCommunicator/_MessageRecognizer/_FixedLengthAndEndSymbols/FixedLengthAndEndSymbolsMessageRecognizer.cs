using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MessageCommunicator.Util;

namespace MessageCommunicator
{
    public class FixedLengthAndEndSymbolsMessageRecognizer : MessageRecognizer
    {
        private Encoding _encoding;
        private string _endSymbols;
        private int _lengthIncludingEndSymbols;
        private int _lengthExcludingEndSymbols;
        private char _fillSymbol;
        private StringBuffer _receiveStringBuffer;

        public FixedLengthAndEndSymbolsMessageRecognizer(Encoding encoding, string endSymbols, int lengthIncludingEndSymbols, char fillSymbol)
        {
            _encoding = encoding;
            _endSymbols = endSymbols;
            _receiveStringBuffer = new StringBuffer(1024);
            _lengthIncludingEndSymbols = lengthIncludingEndSymbols;
            _lengthExcludingEndSymbols = lengthIncludingEndSymbols - endSymbols.Length;
            _fillSymbol = fillSymbol;

            if (_lengthIncludingEndSymbols < _endSymbols.Length + 1)
            {
                throw new ArgumentException(
                    "Length muss be longer than endsymbol length!", nameof(lengthIncludingEndSymbols));
            }
        }

        /// <inheritdoc />
        protected override async Task<bool> SendInternalAsync(ByteStreamHandler byteStreamHandler, string rawMessage)
        {
            // Check for valid message length
            if (rawMessage.Length > _lengthExcludingEndSymbols)
            {
                throw new MessageRecognitionException(
                    "Given message too long. " +
                    $"Maximum length: {(_lengthExcludingEndSymbols)}, " +
                    $"given message length: {rawMessage.Length}");
            }
            if (rawMessage.Length <= 0)
            {
                return false;
            }

            var sendBuffer = StringBuffer.Acquire(_lengthIncludingEndSymbols);
            byte[]? bytes = null;
            try
            {
                sendBuffer.Append(rawMessage);
                while (sendBuffer.Count < _lengthExcludingEndSymbols)
                {
                    sendBuffer.Append(_fillSymbol);
                }
                sendBuffer.Append(_endSymbols, 0, _endSymbols.Length);
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

        public override void OnReceivedBytes(bool isNewConnection, ReadOnlySpan<byte> receivedSegment)
        {
            // Clear receive buffer on new connections
            if (isNewConnection) { _receiveStringBuffer.Clear(); }

            if (receivedSegment.Length <= 0) { return; }
            _receiveStringBuffer.Append(receivedSegment, _encoding);

            while (_receiveStringBuffer.Count >= _lengthIncludingEndSymbols)
            {
                // Check for correct end symbol
                for (var loop = _lengthExcludingEndSymbols; loop < _lengthIncludingEndSymbols; loop++)
                {
                    var endSymbolIndex = loop - _lengthExcludingEndSymbols;
                    if (_receiveStringBuffer[loop] != _endSymbols[endSymbolIndex])
                    {
                        throw new MessageRecognitionException($"Invalid end symbol at index {loop}: Got {_receiveStringBuffer[loop]}, expected {_endSymbols[endSymbolIndex]}!");
                    }
                }

                // Check for fill symbols
                var messageLength = 0;
                for (var loop = _lengthExcludingEndSymbols - 1; loop >= 0; loop--)
                {
                    if (_receiveStringBuffer[loop] != _fillSymbol)
                    {
                        messageLength = loop + 1;
                        break;
                    }
                }

                // Cut out received message
                var receiveHandler = this.ReceiveHandler;
                if ((receiveHandler != null) && (messageLength > 0))
                {
                    var recognizedMessage = MessagePool.Rent(messageLength);
                    recognizedMessage.RawMessage.Append(_receiveStringBuffer.GetPart(0, messageLength));
                    receiveHandler.OnMessageReceived(recognizedMessage);
                }

                // Remove the message with endsymbols from receive buffer
                _receiveStringBuffer.RemoveFromStart(_lengthIncludingEndSymbols);
            }
        }
    }
}
