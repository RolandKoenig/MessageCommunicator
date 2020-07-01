using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace TcpCommunicator.Util
{
    public static class ByteArrayPool
    {
        private static ConcurrentBag<byte[]> s_pool;

        static ByteArrayPool()
        {
            s_pool = new ConcurrentBag<byte[]>();
        }

        public static byte[] Take(int capacity)
        {
            if (s_pool.TryTake(out var result))
            {
                Array.Resize(ref result, capacity);
                return result;
            }
            else
            {
                return new byte[capacity];
            }
        }

        public static void Return(byte[] array)
        {
            s_pool.Add(array);
        }
    }
}
