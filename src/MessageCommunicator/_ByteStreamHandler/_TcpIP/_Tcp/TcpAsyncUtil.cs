using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using Light.GuardClauses;
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

        public static void SafeDispose(ref UdpClient? disposable)
        {
            if(disposable == null){ return; }

            try
            {
                disposable.Dispose();
            }
            catch 
            { 
                // ignored
            }

            disposable = null;
        }

        public static void SafeDispose(UdpClient? disposable)
        {
            SafeDispose(ref disposable);
        }

        public static void SafeDispose(ref TcpClient? disposable)
        {
            if(disposable == null){ return; }

            try
            {
                disposable.Dispose();
            }
            catch 
            { 
                // ignored
            }

            disposable = null;
        }

        public static void SafeDispose(TcpClient? disposable)
        {
            SafeDispose(ref disposable);
        }

        public static void SafeDispose(ref IDisposable? disposable)
        {
            if(disposable == null){ return; }

            try
            {
                disposable.Dispose();
            }
            catch 
            { 
                // ignored
            }

            disposable = null;
        }

        public static void SafeDispose(IDisposable? disposable)
        {
            SafeDispose(ref disposable);
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
    }
}
