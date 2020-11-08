using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Light.GuardClauses;
using MessageCommunicator.Util;

namespace MessageCommunicator
{
    /// <summary>
    /// This <see cref="MessageRecognizer"/> implementation recognizes messages just as they come from the underlying messaging layer.
    /// </summary>
    public class ByUnderlyingPackageMessageRecognizer : MessageRecognizer
    {
        private Encoding _encoding;
        private Decoder _decoder;
        private StringBuffer _receiveStringBuffer;

        /// <summary>
        /// Creates a new <see cref="ByUnderlyingPackageMessageRecognizer"/> instance.
        /// </summary>
        /// <param name="encoding">The <see cref="Encoding"/> to be used when convert characters to/from bytes.</param>
        public ByUnderlyingPackageMessageRecognizer(Encoding encoding)
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
            rawMessage.MustBeLongerThan(0, nameof(rawMessage));

            // Perform message formatting
            var sendBuffer = StringBuffer.Acquire(rawMessage.Length);
            byte[]? bytes = null;
            try
            {
                sendBuffer.Append(rawMessage);
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

            var messageLength = _receiveStringBuffer.Count;
            if (messageLength > 0)
            {
                // Raise found message
                base.NotifyRecognizedMessage(
                    _receiveStringBuffer.GetPartReadOnly(0, messageLength));

                // Clear current receive buffer
                _receiveStringBuffer.Clear();
            }
        }
    }
}
