using System;
using System.Collections.Generic;
using System.Text;

namespace MessageCommunicator
{
    public class TcpActiveByteSteamHandlerSettings : ByteStreamHandlerSettings
    {
        public string RemoteHost
        {
            get;
            set;
        }

        public ushort RemotePort
        {
            get;
            set;
        }

        public ReconnectWaitTimeGetter ReconnectWaitTimeGetter
        {
            get;
            set;
        }

        public TcpActiveByteSteamHandlerSettings(
            string remoteHost, ushort remotePort,
            ReconnectWaitTimeGetter? reconnectWaitTimeGetter = null)
        {
            this.RemoteHost = remoteHost;
            this.RemotePort = remotePort;

            this.ReconnectWaitTimeGetter =
                reconnectWaitTimeGetter ?? new FixedReconnectWaitTimeGetter(TimeSpan.FromSeconds(1.0));
        }

        /// <inheritdoc />
        public override ByteStreamHandler CreateByteStreamHandler()
        {
            return new TcpActiveByteStreamHandler(
                this.RemoteHost, this.RemotePort,
                this.ReconnectWaitTimeGetter);
        }
    }
}
