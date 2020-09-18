using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace MessageCommunicator
{
    /// <summary>
    /// This class provides all relevant settings for <see cref="TcpActiveByteStreamHandler"/>.
    /// </summary>
    public class TcpActiveByteStreamHandlerSettings : ByteStreamHandlerSettings
    {
        /// <summary>
        /// Gets or sets the hostname or ip address of the remote host.
        /// </summary>
        public string RemoteHost
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="IPAddress"/> of the remote host.
        /// </summary>
        public IPAddress RemoteIP
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the remote port.
        /// </summary>
        public ushort RemotePort
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
        /// Creates a new <see cref="TcpActiveByteStreamHandlerSettings"/> instance.
        /// </summary>
        /// <param name="remoteHost">The dns name or string encoded ip address of the remote host.</param>
        /// <param name="remotePort">The remote port.</param>
        /// <param name="reconnectWaitTimeGetter">The <see cref="ReconnectWaitTimeGetter"/> which generates wait times after broke connection and before reconnect.</param>
        public TcpActiveByteStreamHandlerSettings(
            string remoteHost, ushort remotePort,
            ReconnectWaitTimeGetter? reconnectWaitTimeGetter = null)
        {
            this.RemoteHost = remoteHost;
            this.RemoteIP = IPAddress.None;
            this.RemotePort = remotePort;

            this.ReconnectWaitTimeGetter =
                reconnectWaitTimeGetter ?? new FixedReconnectWaitTimeGetter(TimeSpan.FromSeconds(1.0));
        }

        /// <summary>
        /// Creates a new <see cref="TcpActiveByteStreamHandlerSettings"/> instance.
        /// </summary>
        /// <param name="remoteIP">The ip address of the remote host.</param>
        /// <param name="remotePort">The remote port.</param>
        /// <param name="reconnectWaitTimeGetter">The <see cref="ReconnectWaitTimeGetter"/> which generates wait times after broke connection and before reconnect.</param>
        public TcpActiveByteStreamHandlerSettings(
            IPAddress remoteIP, ushort remotePort,
            ReconnectWaitTimeGetter? reconnectWaitTimeGetter = null)
        {
            this.RemoteHost = string.Empty;
            this.RemoteIP = remoteIP;
            this.RemotePort = remotePort;

            this.ReconnectWaitTimeGetter =
                reconnectWaitTimeGetter ?? new FixedReconnectWaitTimeGetter(TimeSpan.FromSeconds(1.0));
        }

        /// <inheritdoc />
        public override ByteStreamHandler CreateByteStreamHandler()
        {
            if (!ReferenceEquals(this.RemoteIP, IPAddress.None))
            {
                return new TcpActiveByteStreamHandler(
                    this.RemoteIP, this.RemotePort,
                    this.ReconnectWaitTimeGetter);
            }
            else
            {
                return new TcpActiveByteStreamHandler(
                    this.RemoteHost, this.RemotePort,
                    this.ReconnectWaitTimeGetter);
            }
        }
    }
}
