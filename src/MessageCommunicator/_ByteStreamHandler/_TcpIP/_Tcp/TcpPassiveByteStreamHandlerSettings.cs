using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Light.GuardClauses;

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
        /// Connection will be closed when we don't receive anything in this period of time.
        /// </summary>
        public TimeSpan ReceiveTimeout
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
        /// <param name="receiveTimeoutMS">Connection will be closed when we don't receive anything in this period of time.</param>
        public TcpPassiveByteStreamHandlerSettings(
            IPAddress listeningIPAddress,
            ushort listeningPort,
            ReconnectWaitTimeGetter? reconnectWaitTimeGetter = null,
            int receiveTimeoutMS = 40000)
        {
            listeningIPAddress.MustNotBeNull(nameof(listeningIPAddress));
            receiveTimeoutMS.MustBeGreaterThanOrEqualTo(0, nameof(receiveTimeoutMS));

            this.ListeningIPAddress = listeningIPAddress;
            this.ListeningPort = listeningPort;
            this.ReceiveTimeout = TimeSpan.FromMilliseconds(receiveTimeoutMS);

            this.ReconnectWaitTimeGetter =
                reconnectWaitTimeGetter ?? new FixedReconnectWaitTimeGetter(TimeSpan.FromSeconds(1.0));
        }

        /// <inheritdoc />
        public override ByteStreamHandler CreateByteStreamHandler()
        {
            return new TcpPassiveByteStreamHandler(
                this.ListeningIPAddress, this.ListeningPort,
                this.ReconnectWaitTimeGetter,
                this.ReceiveTimeout);
        }
    }
}
