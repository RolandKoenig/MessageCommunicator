using System;
using System.Collections.Generic;
using System.Text;

namespace MessageCommunicator.Util
{
    /// <summary>
    /// Helper class for <see cref="MessageChannel"/>.
    /// </summary>
    public class DelegateMessageReceiveHandler : IMessageReceiveHandler
    {
        private Action<Message> _receiveHandler;

        /// <summary>
        /// Builds an <see cref="IMessageReceiveHandler"/> implementation based on the given delegate.
        /// </summary>
        /// <param name="receiveHandler">The delegate to be called on received messages.</param>
        public DelegateMessageReceiveHandler(Action<Message> receiveHandler)
        {
            _receiveHandler = receiveHandler;
        }

        /// <inheritdoc />
        public void OnMessageReceived(Message message)
        {
            _receiveHandler(message);
        }
    }
}
