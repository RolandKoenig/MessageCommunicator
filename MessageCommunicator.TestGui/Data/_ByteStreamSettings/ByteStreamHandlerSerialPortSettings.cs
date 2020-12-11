using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO.Ports;
using System.Text;
using MessageCommunicator.SerialPorts;

namespace MessageCommunicator.TestGui.Data
{
    public class ByteStreamHandlerSerialPortSettings : IByteStreamHandlerAppSettings
    {
        private const string CATEGORY = "Serial Port";

        [Required]
        [Category(CATEGORY)]
        [FixedPossibleValues(nameof(GetPossiblePortNames))]
        public string PortName
        {
            get;
            set;
        } = string.Empty;

        [Category(CATEGORY)]
        public int BaudRate { get; set; } = 9600;

        [Category(CATEGORY)]
        public int DataBits { get; set; } = 8;

        [Category(CATEGORY)]
        public StopBits StopBits { get; set; } = StopBits.One;

        [Category(CATEGORY)]
        public Handshake Handshake { get; set; } = Handshake.None;

        [Category(CATEGORY)]
        public int ReadTimeout { get; set; } = -1;

        [Category(CATEGORY)]
        public int WriteTimeout { get; set; } = -1;

        [Category(CATEGORY)]
        public Parity Parity { get; set; } = Parity.None;

        [Category(CATEGORY)]
        public bool DtrEnable { get; set; } = false;

        [Category(CATEGORY)]
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
