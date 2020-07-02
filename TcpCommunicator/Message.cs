using System;
using System.Collections.Generic;
using System.Text;
using TcpCommunicator.Util;

namespace TcpCommunicator
{
    public class Message
    {
        public StringBuffer RawMessage { get; }

        public Message(int capacity)
        {
            this.RawMessage = new StringBuffer(capacity);
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
            MessagePool.ClearAndReturn(this);
        }
    }
}
