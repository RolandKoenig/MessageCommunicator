using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MessageCommunicator.Util;

namespace MessageCommunicator
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

        public static string ToHexString(ReadOnlySendOrReceiveBuffer<byte> bytes)
        {
            if (bytes.Length == 0) { return string.Empty; }

            const string HEX_ALPHABET = "0123456789ABCDEF";

            var length = bytes.Length;
            if (length > 1) { length += (length - 1); }
            var stringBuffer = StringBuffer.Acquire(length);
            var bytesSpan = bytes.Span;
            try
            {
                for (var loop = 0; loop < bytesSpan.Length; loop++)
                {
                    if(stringBuffer.Count > 0){ stringBuffer.Append(' '); }

                    var actByte = bytesSpan[loop];
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
