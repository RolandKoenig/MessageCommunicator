using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace MessageCommunicator
{
    /// <summary>
    /// This class provides all relevant settings for <see cref="TcpPassiveByteStreamHandler"/>.
    /// </summary>
    public class TcpPassiveByteStreamHandlerSettings : ByteStreamHandlerSettings
    {
        /// <summary>
        /// Gets or sets the <see cref="IPAddress"/> to listening on.
        /// </summary>
        public IPAddress ListeningIPAddress
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the port to listen on.
        /// </summary>
        public ushort ListeningPort
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="ReconnectWaitTimeGetter"/> instance.
        /// </summary>
        public ReconnectWaitTimeGetter ReconnectWaitTimeGetter
        {
            get;
            set;
        }

        /// <summary>
        /// Create a new <see cref="TcpPassiveByteStreamHandler"/> instance.
        /// </summary>
        /// <param name="listeningIPAddress">The <see cref="IPAddress"/> to listen on.</param>
        /// <param name="listeningPort">The port to listen on. Pass 0 here if the OS should decide which port to use.</param>
        /// <param name="reconnectWaitTimeGetter">The <exception cref="ReconnectWaitTimeGetter"> which generates wait times after broke connection and before reconnect.</exception></param>
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
