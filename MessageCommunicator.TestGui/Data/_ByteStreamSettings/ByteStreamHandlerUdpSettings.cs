﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MessageCommunicator.TestGui.Data
{
    [TypeAlias("ByteStreamHandlerUdpSettings")]
    public class ByteStreamHandlerUdpSettings : IByteStreamHandlerAppSettings
    {
        private const string CATEGORY = "ByteStreamHandler Udp";

        [Category(CATEGORY)]
        public ushort LocalPort { get; set; } = 12000;

        [Required]
        [Category(CATEGORY)]
        public string RemoteHost { get; set; } = "127.0.0.1";

        [Category(CATEGORY)]
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
