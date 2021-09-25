using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;

namespace MessageCommunicator.TestGui.Data
{
    [TypeAlias("ByteStreamHandlerTcpSettings")]
    public class ByteStreamHandlerTcpSettings : IByteStreamHandlerAppSettings
    {
        private const string CATEGORY = "ByteStreamHandler Tcp";

        [Required]
        [Category(CATEGORY)]
        [Description("IP or hostname of the target host. This parameter is not relevant in passive mode.")]
        public string Target { get; set; } = "127.0.0.1";

        [Category(CATEGORY)]
        [Description("The port to listen or to connect to (depending on the mode).")]
        public ushort Port { get; set; } = 12000;

        [Category(CATEGORY)]
        [Description("Connect to a remote host and port in active mode. " +
                     "Listen on a local port for incoming connections " +
                     "(Target parameter not relevant).")]
        public ConnectionMode Mode { get; set; }

        [Category(CATEGORY)]
        [DisplayName("Receive Timeout (Sec)")]
        [Description("When no packages are received during the given timespan, than a reconnect will be triggered")]
        public int ReceiveTimeoutSec { get; set; } = 40;

        /// <inheritdoc />
        public ByteStreamHandlerSettings CreateLibSettings()
        {
            switch (this.Mode)
            {
                case ConnectionMode.Active:
                    return new TcpActiveByteStreamHandlerSettings(
                        this.Target, this.Port,
                        receiveTimeoutMS: this.ReceiveTimeoutSec * 1000);

                case ConnectionMode.Passive:
                    return new TcpPassiveByteStreamHandlerSettings(
                        IPAddress.Any, this.Port,
                        receiveTimeoutMS: this.ReceiveTimeoutSec * 1000);

                default:
                    throw new ArgumentOutOfRangeException($"Unknown connection mode: {this.Mode}");
            }
        }
    }
}
