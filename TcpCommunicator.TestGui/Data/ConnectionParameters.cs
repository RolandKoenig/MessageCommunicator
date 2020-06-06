using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
namespace TcpCommunicator.TestGui.Data
{
    public class ConnectionParameters
    {
        [Required]
        public string Name { get; set; } = "New Profile";

        public string Target { get; set; } = "127.0.0.1";

        public ushort Port { get; set; } = 12000;

        public ConnectionMode Mode { get; set; } = ConnectionMode.Passive;
    }
}
