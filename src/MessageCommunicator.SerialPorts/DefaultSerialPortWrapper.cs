using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;

namespace MessageCommunicator.SerialPorts
{
    internal class DefaultSerialPortWrapper : ISerialPort
    {
        private SerialPort _wrappedSerialPort;

        /// <inheritdoc />
        public bool IsOpen => _wrappedSerialPort.IsOpen;

        /// <inheritdoc />
        public string PortName
        {
            get => _wrappedSerialPort.PortName;
            set => _wrappedSerialPort.PortName = value;
        }

        /// <inheritdoc />
        public int BaudRate
        {
            get => _wrappedSerialPort.BaudRate;
            set => _wrappedSerialPort.BaudRate = value;
        }

        /// <inheritdoc />
        public int DataBits
        {
            get => _wrappedSerialPort.DataBits;
            set => _wrappedSerialPort.DataBits = value;
        }

        /// <inheritdoc />
        public StopBits StopBits
        {
            get => _wrappedSerialPort.StopBits;
            set => _wrappedSerialPort.StopBits = value;
        }

        /// <inheritdoc />
        public Handshake Handshake
        {
            get => _wrappedSerialPort.Handshake;
            set => _wrappedSerialPort.Handshake = value;
        }

        /// <inheritdoc />
        public int ReadTimeout
        {
            get => _wrappedSerialPort.ReadTimeout;
            set => _wrappedSerialPort.ReadTimeout = value;
        }

        /// <inheritdoc />
        public int WriteTimeout
        {
            get => _wrappedSerialPort.WriteTimeout;
            set => _wrappedSerialPort.WriteTimeout = value;
        }

        /// <inheritdoc />
        public Parity Parity
        {
            get => _wrappedSerialPort.Parity;
            set => _wrappedSerialPort.Parity = value;
        }

        /// <inheritdoc />
        public bool DtrEnable
        {
            get => _wrappedSerialPort.DtrEnable;
            set => _wrappedSerialPort.DtrEnable = value;
        }

        /// <inheritdoc />
        public bool RtsEnable
        {
            get => _wrappedSerialPort.RtsEnable;
            set => _wrappedSerialPort.RtsEnable = value;
        }

        public DefaultSerialPortWrapper()
        {
            _wrappedSerialPort = new SerialPort();
            _wrappedSerialPort.DataReceived += this.OnWrappedSerialPort_DataReceived;
        }

        /// <inheritdoc />
        public int BytesToRead => _wrappedSerialPort.BytesToRead;

        /// <inheritdoc />
        public event SerialDataReceivedEventHandler? DataReceived;

        /// <inheritdoc />
        public void Open()
        {
            _wrappedSerialPort.Open();
        }

        public void Close()
        {
            _wrappedSerialPort.Close();
        }

        /// <inheritdoc />
        public void Write(byte[] buffer, int offset, int count)
        {
            _wrappedSerialPort.Write(buffer, offset, count);
        }

        /// <inheritdoc />
        public int Read(byte[] buffer, int offset, int count)
        {
            return _wrappedSerialPort.Read(buffer, offset, count);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _wrappedSerialPort.Dispose();
        }

        private void OnWrappedSerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            this.DataReceived?.Invoke(this, e);
        }
    }
}
