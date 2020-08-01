using System;
using System.Collections.Generic;
using MessageCommunicator.Util;

namespace MessageCommunicator
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
            }
            else
            {
                result.Clear();
            }

            result.IsMessagePooled = false;
            return result;
        }

        public static void Return(Message message)
        {
            if (message.IsMessagePooled)
            {
                throw new InvalidOperationException("Message is already pooled!");
            }

            message.IsMessagePooled = true;
            s_pool.Return(message);
        }

        public static int CountCachedMessages => s_pool.CountItems;
    }
}
