using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MessageCommunicator.Util;

namespace MessageCommunicator
{
    /// <summary>
    /// This <see cref="MessageRecognizer"/> implementation recognizes messages with one or more end symbols and
    /// a fixed length.
    /// </summary>
    public class FixedLengthAndEndSymbolsMessageRecognizer : MessageRecognizer
    {
        private Encoding _encoding;
        private Decoder _decoder;
        private string _endSymbols;
        private int _lengthIncludingEndSymbols;
        private int _lengthExcludingEndSymbols;
        private char _fillSymbol;
        private StringBuffer _receiveStringBuffer;

        /// <summary>
        /// Creates a new <see cref="FixedLengthAndEndSymbolsMessageRecognizer"/> instance.
        /// </summary>
        /// <param name="encoding">The <see cref="Encoding"/> to be used when convert characters to/from bytes.</param>
        /// <param name="endSymbols">The end symbols of received/sent messages.</param>
        /// <param name="lengthIncludingEndSymbols">Total length of received/sent messages.</param>
        /// <param name="fillSymbol">Fill symbol for messages shorter than the fixed length.</param>
        public FixedLengthAndEndSymbolsMessageRecognizer(Encoding encoding, string endSymbols, int lengthIncludingEndSymbols, char fillSymbol)
        {
            _encoding = encoding;
            _decoder = _encoding.GetDecoder();
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
        protected override Task<bool> SendInternalAsync(IByteStreamHandler byteStreamHandler, ReadOnlySpan<char> rawMessage)
        {
            // Check for valid message
            if (rawMessage.Length > _lengthExcludingEndSymbols)
            {
                throw new ArgumentException(
                    "Given message too long. " +
                    $"Maximum length: {(_lengthExcludingEndSymbols)}, " +
                    $"given message length: {rawMessage.Length}",
                    nameof(rawMessage));
            }

            // Perform message formatting
            var sendBuffer = StringBuffer.Acquire(_lengthIncludingEndSymbols);
            byte[]? bytes = null;
            try
            {
                if(rawMessage.Length > 0){ sendBuffer.Append(rawMessage); }
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
                if (receiveHandler != null)
                {
                    var recognizedMessage = MessagePool.Rent(messageLength);
                    if (messageLength > 0)
                    {
                        recognizedMessage.RawMessage.Append(
                            _receiveStringBuffer.GetPartReadOnly(0, messageLength));
                    }
                    receiveHandler.OnMessageReceived(recognizedMessage);
                }

                // Remove the message with endsymbols from receive buffer
                _receiveStringBuffer.RemoveFromStart(_lengthIncludingEndSymbols);
            }
        }
    }
}
