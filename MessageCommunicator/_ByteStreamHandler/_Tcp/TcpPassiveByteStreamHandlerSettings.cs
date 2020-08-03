using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace MessageCommunicator
{
    public class TcpPassiveByteStreamHandlerSettings : ByteStreamHandlerSettings
    {
        public IPAddress ListeningIPAddress
        {
            get;
            set;
        }

        public ushort ListeningPort
        {
            get;
            set;
        }

        public ReconnectWaitTimeGetter ReconnectWaitTimeGetter
        {
            get;
            set;
        }

        public TcpPassiveByteStreamHandlerSettings(
            IPAddress listeningIPAddress,
            ushort listeningPort,
            ReconnectWaitTimeGetter? reconnectWaitTimeGetter = null)
        {
            this.ListeningIPAddress = listeningIPAddress;
            this.ListeningPort = listeningPort;

            this.ReconnectWaitTimeGetter =
                reconnectWaitTimeGetter ?? new FixedReconnectWaitTimeGetter(TimeSpan.FromSeconds(1.0));
        }

        /// <inheritdoc />
        public override ByteStreamHandler CreateByteStreamHandler()
        {
            return new TcpPassiveByteStreamHandler(
                this.ListeningIPAddress, this.ListeningPort,
                this.ReconnectWaitTimeGetter);
        }
    }
}
