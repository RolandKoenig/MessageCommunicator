using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace TcpCommunicator
{
    internal static class MessagePool
    {
        private static ConcurrentBag<Message> s_pool;

        static MessagePool()
        {
            s_pool = new ConcurrentBag<Message>();
        }

        public static Message Take(int capacity)
        {
            if (s_pool.TryTake(out var result))
            {
                result.EnsureCapacity(capacity);
                return result;
            }
            else
            {
                return new Message(capacity);
            }
        }

        public static void ClearAndReturn(Message message)
        {
            message.Clear();
            s_pool.Add(message);
        }
    }
}
