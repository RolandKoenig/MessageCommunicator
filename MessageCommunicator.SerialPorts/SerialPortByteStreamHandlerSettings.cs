using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using MessageCommunicator;
using MessageCommunicator.SerialPorts;

namespace MessageCommunicator.SerialPorts
{
    public class SerialPortByteStreamHandlerSettings : ByteStreamHandlerSettings
    {
        public string PortName { get; set; }

        public int BaudRate { get; set; }

        public int DataBits { get; set; }

        public StopBits StopBits { get; set; }

        public Handshake Handshake { get; set; }

        public int ReadTimeout { get; set; }

        public int WriteTimeout { get; set; }

        public Parity Parity { get; set; }

        public bool DtrEnable { get; set; }
 
        public bool RtsEnable { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ReconnectWaitTimeGetter"/> instance.
        /// </summary>
        public ReconnectWaitTimeGetter ReconnectWaitTimeGetter
        {
            get;
            set;
        }

        public SerialPortByteStreamHandlerSettings(
            string portName, int baudRate = 9600, int dataBits = 8, StopBits stopBits = StopBits.One, Handshake handshake = Handshake.None, 
            int readTimeout = -1, int writeTimeout = -1, Parity parity = Parity.None, bool dtrEnable = false, bool rtsEnable = false,
            ReconnectWaitTimeGetter? reconnectWaitTimeGetter = null)
        {
            this.PortName = portName;
            this.BaudRate = baudRate;
            this.DataBits = dataBits;
            this.StopBits = stopBits;
            this.Handshake = handshake;
            this.ReadTimeout = readTimeout;
            this.WriteTimeout = writeTimeout;
            this.Parity = parity;
            this.DtrEnable = dtrEnable;
            this.RtsEnable = rtsEnable;
            this.ReconnectWaitTimeGetter =
                reconnectWaitTimeGetter ?? new FixedReconnectWaitTimeGetter(TimeSpan.FromSeconds(1.0));
        }

        /// <inheritdoc />
        public override ByteStreamHandler CreateByteStreamHandler()
        {
            return new SerialPortByteStreamHandler(
                this.PortName, this.BaudRate, this.DataBits, this.StopBits, this.Handshake, 
                this.ReadTimeout, this.WriteTimeout, this.Parity, this.DtrEnable, this.RtsEnable,
                this.ReconnectWaitTimeGetter);
        }
    }
}
