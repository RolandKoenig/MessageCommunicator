using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text;

namespace MessageCommunicator.TestGui.Data
{
    public class ByteStreamHandlerTcpSettings : IByteStreamHandlerAppSettings
    {
        private const string CATEGORY = "TCP";

        [Required]
        [Category(CATEGORY)]
        public string Target { get; set; } = "127.0.0.1";

        [Category(CATEGORY)]
        public ushort Port { get; set; } = 12000;

        [Category(CATEGORY)]
        public ConnectionMode Mode { get; set; }

        [Category(CATEGORY)]
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
