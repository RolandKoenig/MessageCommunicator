using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace MessageCommunicator.TestGui.Data
{
    [TypeAlias("ByteStreamHandlerUdpSettings")]
    public class ByteStreamHandlerUdpSettings : IByteStreamHandlerAppSettings
    {
        private const string CATEGORY = "ByteStreamHandler Udp";

        [Category(CATEGORY)]
        [Description("The port on which to listen for incoming packages.")]
        public ushort LocalPort { get; set; } = 12000;

        [Required]
        [Category(CATEGORY)]
        [Description("The remote host (hostname or ip address) to which to send outgoing packages.")]
        public string RemoteHost { get; set; } = "127.0.0.1";

        [Category(CATEGORY)]
        [Description("The remote port to which to send outgoing packages.")]
        public ushort RemotePort { get; set; } = 12000;

        /// <inheritdoc />
        public ByteStreamHandlerSettings CreateLibSettings()
        {
            return new UdpByteStreamHandlerSettings(
                this.LocalPort,
                this.RemoteHost, this.RemotePort);
        }
    }
}
