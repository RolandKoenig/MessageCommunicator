using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace MessageCommunicator.Tests.Util
{
    internal static class TestUtility
    {
        public static ushort GetFreeTcpPort()
        {
            // Simple method do find a free port
            //  see https://stackoverflow.com/questions/138043/find-the-next-tcp-port-in-net

            var dummyListener = new TcpListener(IPAddress.Loopback, 0);

            dummyListener.Start();
            var port = (ushort)((IPEndPoint)dummyListener.LocalEndpoint).Port;
            dummyListener.Stop();

            return port;
        }
    }
}
