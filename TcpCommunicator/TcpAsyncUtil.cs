using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using TcpCommunicator.Util;

namespace TcpCommunicator
{
    internal static class TcpAsyncUtil
    {
        public static Task DummyFinishedTask { get; } = Task.FromResult(null as object);

        public static void SafeStop(ref TcpListener? tcpListener)
        {
            if (tcpListener == null) { return; }

            try{ tcpListener.Stop(); }
            catch 
            { 
                // ignored
            }

            tcpListener = null;
        }

        public static void SafeDispose(ref TcpClient? tcpClient)
        {
            if(tcpClient == null){ return; }

            try
            {
                tcpClient.Dispose();
            }
            catch 
            { 
                // ignored
            }

            tcpClient = null;
        }

        public static void SafeDispose(Socket? socket)
        {
            if(socket == null){ return; }

            try
            {
                socket.Dispose();
            }
            catch 
            { 
                // ignored
            }

            socket = null;
        }

        public static string ToHexString(ArraySegment<byte> bytes)
        {
            if (bytes.Array == null) { return string.Empty;}
            const string HEX_ALPHABET = "0123456789ABCDEF";

            var length = bytes.Count;
            if (length > 1) { length += (length - 1); }

            var stringBuffer = StringBuffer.Acquire(length);
            try
            {
                var max = bytes.Offset + bytes.Count;
                for (var loop = bytes.Offset; loop < max; loop++)
                {
                    if(stringBuffer.Count > 0){ stringBuffer.Append(' '); }

                    var actByte = bytes.Array[loop];
                    stringBuffer.Append(HEX_ALPHABET[actByte >> 4]);
                    stringBuffer.Append(HEX_ALPHABET[actByte & 0xF]);
                }

                return stringBuffer.ToString();
            }
            finally
            {
                StringBuffer.Release(stringBuffer);
            }
        }
    }
}
