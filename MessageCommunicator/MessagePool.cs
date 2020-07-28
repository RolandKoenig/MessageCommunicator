using System;
using System.Collections.Generic;
using TcpCommunicator.Util;

namespace TcpCommunicator
{
    public static class MessagePool
    {
        public static readonly int MAX_POOL_SIZE = 16;

        private static ConcurrentObjectPool<Message> s_pool;

        static MessagePool()
        {
            s_pool = new ConcurrentObjectPool<Message>(MAX_POOL_SIZE);
        }

        public static Message Take(int capacity)
        {
            var result = s_pool.TryRent();
            if (result == null)
            {
                result = new Message(capacity);
                result.IsMessageFromPool = true;
            }
            return result;
        }

        public static void ClearAndReturn(Message message)
        {
            message.Clear();

            if (message.IsMessageFromPool)
            {
                message.IsMessageFromPool = s_pool.Return(message);
            }
        }

        public static int CountCachedMessages => s_pool.CountItems;
    }
}
