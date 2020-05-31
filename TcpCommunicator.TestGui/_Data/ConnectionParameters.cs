using System;
using System.Collections.Generic;
using System.Text;

namespace TcpCommunicator.TestGui
{
    public class ConnectionParameters
    {
        public string Target { get; set; } = string.Empty;

        public ushort Port { get; set; }

        public ConnectionMode Mode { get; set; } = ConnectionMode.Passive;
    }
}
