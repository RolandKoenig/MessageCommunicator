using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.IO.Ports;
using System.Text;
using MessageCommunicator.SerialPorts;

namespace MessageCommunicator.TestGui.Data
{
    [TypeAlias("ByteStreamHandlerSerialPortSettings")]
    public class ByteStreamHandlerSerialPortSettings : IByteStreamHandlerAppSettings
    {
        private const string CATEGORY = "ByteStreamHandler SerialPort";

        [Required]
        [Category(CATEGORY)]
        [DisplayName("Port")]
        [FixedPossibleValues(nameof(GetPossiblePortNames))]
        public string PortName
        {
            get;
            set;
        } = string.Empty;

        [Category(CATEGORY)]
        [DisplayName("Baud Rate")]
        public int BaudRate { get; set; } = 9600;

        [Category(CATEGORY)]
        [DisplayName("Data Bits")]
        public int DataBits { get; set; } = 8;

        [Category(CATEGORY)]
        [DisplayName("Stop Bits")]
        public StopBits StopBits { get; set; } = StopBits.One;

        [Category(CATEGORY)]
        public Handshake Handshake { get; set; } = Handshake.None;

        [Category(CATEGORY)]
        [DisplayName("Read Timeout")]
        public int ReadTimeout { get; set; } = -1;

        [Category(CATEGORY)]
        [DisplayName("Write Timeout")]
        public int WriteTimeout { get; set; } = -1;

        [Category(CATEGORY)]
        public Parity Parity { get; set; } = Parity.None;

        [Category(CATEGORY)]
        [DisplayName("Dtr Enabled")]
        public bool DtrEnable { get; set; } = false;

        [Category(CATEGORY)]
        [DisplayName("Rts Enabled")]
        public bool RtsEnable { get; set; } = false;

        /// <inheritdoc />
        public ByteStreamHandlerSettings CreateLibSettings()
        {
            return new SerialPortByteStreamHandlerSettings(
                this.PortName, this.BaudRate, this.DataBits, this.StopBits, this.Handshake,
                this.ReadTimeout, this.WriteTimeout, this.Parity, this.DtrEnable, this.RtsEnable);
        }

        /// <summary>
        /// Gets a collection containing all available port names.
        /// </summary>
        public static string[] GetPossiblePortNames()
        {
            var portNames = SerialPort.GetPortNames();
            
            var result = new string[portNames.Length + 1];
            result[0] = string.Empty;
            if (portNames.Length > 0)
            {
                Array.Copy(portNames, 0, result, 1, portNames.Length);
            }
            return result;
        }
    }
}
