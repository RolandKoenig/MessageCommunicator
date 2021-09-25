using System;
using System.Collections.Generic;
using Light.GuardClauses;
using MessageCommunicator.Util;

namespace MessageCommunicator
{
    /// <summary>
    /// This class provides an integrated pooling mechanism to avoid object allocations during communication between two partners.
    /// </summary>
    public static class MessagePool
    {
        public static readonly int MAX_POOL_SIZE = 16;

        private static ConcurrentObjectPool<Message> s_pool;

        static MessagePool()
        {
            s_pool = new ConcurrentObjectPool<Message>(MAX_POOL_SIZE);
        }

        /// <summary>
        /// Clears current message pool.
        /// </summary>
        public static void Clear()
        {
            s_pool = new ConcurrentObjectPool<Message>(MAX_POOL_SIZE);
        }

        /// <summary>
        /// Rents a message from the pool. A new message will be created if there is no one inside the pool.
        /// </summary>
        /// <param name="capacity">The capacity of the returned message.</param>
        public static Message Rent(int capacity)
        {
            var result = s_pool.TryRent();
            if (result == null)
            {
                result = new Message(capacity);
            }
            else
            {
                result.IsMessagePooled = false;
                result.Clear();
            }

            return result;
        }

        /// <summary>
        /// Returns the given message to the message pool.
        /// </summary>
        /// <param name="message">The message to be returned.</param>
        /// <exception cref="InvalidOperationException">The given message is already pooled.</exception>
        public static void Return(Message message)
        {
            message.MustNotBeNull(nameof(message));

            if (message.IsMessagePooled)
            {
                throw new InvalidOperationException("Message is already pooled!");
            }

            message.IsMessagePooled = true;
            s_pool.Return(message);
        }

        /// <summary>
        /// Total count of messages within the pool.
        /// </summary>
        public static int CountCachedMessages => s_pool.CountItems;
    }
}
