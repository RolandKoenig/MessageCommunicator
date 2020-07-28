using System;
using System.Collections.Generic;
using TcpCommunicator.Util;

namespace TcpCommunicator
{
    public class Message
    {
        internal StringBuffer RawMessage { get; }

        public bool IsMessageFromPool { get; internal set; }

        public Message(int capacity)
        {
            this.RawMessage = new StringBuffer(capacity);
        }

        public Message(string rawMessage)
        {
            this.RawMessage = new StringBuffer(rawMessage);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return this.RawMessage.ToString();
        }

        public void EnsureCapacity(int capacity)
        {
            this.RawMessage.EnsureCapacity(capacity);
        }

        public void Clear()
        {
            this.RawMessage.Clear();
        }

        public void ClearAndReturnToPool()
        {
            if (this.IsMessageFromPool)
            {
                MessagePool.ClearAndReturn(this);
            }
        }
    }
}
