using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace MessageCommunicator
{
    public class UdpByteStreamHandlerSettings : ByteStreamHandlerSettings
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
        public IPAddress RemoteIPAddress
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
        /// Gets or sets the port to listen on.
        /// </summary>
        public ushort ListeningPort
        {
            get;
            set;
        }

        public UdpByteStreamHandlerSettings(
            ushort listeningPort,
            IPAddress remoteIPAddressAddress, ushort remotePort)
        {
            this.ListeningPort = listeningPort;
            this.RemoteIPAddress = remoteIPAddressAddress;
            this.RemotePort = remotePort;
            this.RemoteHost = remoteIPAddressAddress.ToString();
        }

        public UdpByteStreamHandlerSettings(
            ushort listeningPort,
            string remoteHost, ushort remotePort)
        {
            this.ListeningPort = listeningPort;
            this.RemoteIPAddress = IPAddress.None;
            this.RemotePort = remotePort;
            this.RemoteHost = remoteHost;
        }

        /// <inheritdoc />
        public override ByteStreamHandler CreateByteStreamHandler()
        {
            if (!ReferenceEquals(this.RemoteIPAddress, IPAddress.None))
            {
                return new UdpByteStreamHandler(
                    this.ListeningPort, 
                    this.RemoteIPAddress, this.RemotePort);
            }
            else
            {
                return new UdpByteStreamHandler(
                    this.ListeningPort, 
                    this.RemoteHost, this.RemotePort);
            }
        }
    }
}
