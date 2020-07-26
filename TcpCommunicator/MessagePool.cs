using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace TcpCommunicator
{
    public static class MessagePool
    {
        public const int MAX_POOL_SIZE = 16;

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
                result = new Message(capacity);
                result.IsMessageFromPool = true;
                return result;
            }
        }

        public static void ClearAndReturn(Message message)
        {
            message.Clear();

            if (s_pool.Count < MAX_POOL_SIZE)
            {
                s_pool.Add(message);
            }
            else
            {
                message.IsMessageFromPool = false;
            }
        }

        public static int CountCachedMessages => s_pool.Count;
    }
}
