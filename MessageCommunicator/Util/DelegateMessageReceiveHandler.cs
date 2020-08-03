using System;
using System.Collections.Generic;
using System.Text;

namespace MessageCommunicator.Util
{
    public class DelegateMessageReceiveHandler : IMessageReceiveHandler
    {
        private Action<Message> _receiveHandler;

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
