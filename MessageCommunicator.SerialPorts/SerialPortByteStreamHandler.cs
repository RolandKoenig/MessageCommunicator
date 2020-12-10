using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Threading.Tasks;
using MessageCommunicator.Util;
using Light.GuardClauses;

namespace MessageCommunicator.SerialPorts
{
    public class SerialPortByteStreamHandler : ByteStreamHandler
    {
        // Configuration
        private string _portName;
        private int _baudRate;
        private int _dataBits;
        private StopBits _stopBits;
        private Handshake _handshake;
        private int _readTimeout;
        private int _writeTimeout;
        private Parity _parity;
        private bool _dtrEnable;
        private bool _rtsEnable;

        // Runtime
        private object _startStopLock;
        private SerialPort? _serialPort;
        private byte[] _receiveBuffer;
        private bool _isNewConnection;
        private DateTime _lastConnectTimestampUtc;
        private DateTime _lastReceivedDataBlockTimestampUtc;

        /// <inheritdoc />
        public override ConnectionState State
        {
            get
            {
                var serialPort = _serialPort;
                if (serialPort == null) { return ConnectionState.Stopped; }
                else if (!serialPort.IsOpen) { return ConnectionState.Connecting; }
                else { return ConnectionState.Connected; }
            }
        }

        /// <inheritdoc />
        public override bool IsRunning => _serialPort != null;

        /// <inheritdoc />
        public override string LocalEndpointDescription => _serialPort?.PortName ?? string.Empty;

        /// <inheritdoc />
        public override string RemoteEndpointDescription => string.Empty;

        /// <inheritdoc />
        public override DateTime LastConnectTimestampUtc => _lastConnectTimestampUtc;

        /// <inheritdoc />
        public override DateTime LastReceivedDataBlockTimestampUtc => _lastReceivedDataBlockTimestampUtc;

        /// <summary>
        /// Gets or sets the <see cref="ReconnectWaitTimeGetter"/> which controls the wait time before reconnect after lost connections.
        /// </summary>
        public ReconnectWaitTimeGetter ReconnectWaitTimeGetter { get; set; }

        public SerialPortByteStreamHandler(
            string portName, int baudRate, int dataBits, StopBits stopBits, Handshake handshake, 
            int readTimeout, int writeTimeout, Parity parity, bool dtrEnable, bool rtsEnable,
            ReconnectWaitTimeGetter reconnectWaitTimeGetter)
        {
            _portName = portName;
            _baudRate = baudRate;
            _dataBits = dataBits;
            _stopBits = stopBits;
            _handshake = handshake;
            _readTimeout = readTimeout;
            _writeTimeout = writeTimeout;
            _parity = parity;
            _dtrEnable = dtrEnable;
            _rtsEnable = rtsEnable;

            this.ReconnectWaitTimeGetter = reconnectWaitTimeGetter;

            _receiveBuffer = new byte[1024];
            _startStopLock = new object();
        }

        /// <inheritdoc />
        public override Task<bool> SendAsync(ArraySegment<byte> buffer)
        {
            var serialPort = _serialPort;
            if (serialPort != null)
            {
                try
                {
                    serialPort.Write(buffer.Array, buffer.Offset, buffer.Count);
                    return Task.FromResult(true);
                }
                catch (Exception e)
                {
                    return Task.FromException<bool>(e);
                }
            }

            return Task.FromResult(false);
        }

        /// <inheritdoc />
        protected override Task StartInternalAsync()
        {
            lock (_startStopLock)
            {
                if (_serialPort != null)
                {
                    throw new InvalidOperationException($"Unable to start {nameof(SerialPortByteStreamHandler)} on port {_portName}: This object is started already!");
                }

                _serialPort = new SerialPort();
                _serialPort.PortName = _portName;
                _serialPort.BaudRate = _baudRate;
                _serialPort.DataBits = _dataBits;
                _serialPort.StopBits = _stopBits;
                _serialPort.Handshake = _handshake;
                _serialPort.ReadTimeout = _readTimeout;
                _serialPort.WriteTimeout = _writeTimeout;
                _serialPort.Parity = _parity;
                _serialPort.DtrEnable = _dtrEnable;
                _serialPort.RtsEnable = _rtsEnable;

                _isNewConnection = true;
                _serialPort.DataReceived += this.OnSerialPort_DataReceived;
            }

            this.TriggerOpenConnection(_serialPort);

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        protected override Task StopInternalAsync()
        {
            lock (_startStopLock)
            {
                if (_serialPort == null)
                {
                    throw new InvalidOperationException($"Unable to stop {nameof(SerialPortByteStreamHandler)} on port {_portName}: This object is already stopped!");
                }

                _serialPort.DataReceived -= this.OnSerialPort_DataReceived;

                _serialPort.Close();
                TcpAsyncUtil.SafeDispose(_serialPort);

                _isNewConnection = false;
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Try to open the serial port.
        /// </summary>
        private async void TriggerOpenConnection(SerialPort serialPort)
        {
            var opened = false;
            var connectionErrorCount = 0;
            while ((!opened) && (_serialPort == serialPort))
            {
                try
                {
                    serialPort.Open();
                    opened = true;
                    connectionErrorCount = 0;
                    _lastConnectTimestampUtc = DateTime.UtcNow;
                }
                catch (Exception)
                {
                    connectionErrorCount++;
                    await Task.Delay(this.ReconnectWaitTimeGetter.GetWaitTime(connectionErrorCount));
                }
            }
        }

        private void ProcessReceivedBytes(bool newConnection, byte[] buffer, int receivedBytesCount)
        {
            buffer.MustNotBeNull(nameof(buffer));

            var receiveBuffer = new ArraySegment<byte>(buffer, 0, receivedBytesCount);

            // Log currently received bytes
            if (this.IsLoggerSet)
            {
                this.Log(
                    LoggingMessageType.Info,
                    StringBuffer.Format("Received {0} bytes: {1}", receiveBuffer.Count, HexFormatUtil.ToHexString(receiveBuffer)));
            }

            // Notify received bytes
            var messageRecognizer = this.MessageRecognizer;
            messageRecognizer?.OnReceivedBytes(newConnection, receiveBuffer);
        }

        private void OnSerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (sender != _serialPort) { return; }

            var currentBytesToRead = _serialPort.BytesToRead;
            while (currentBytesToRead > 0)
            {
                var readBlockSize = currentBytesToRead < _receiveBuffer.Length
                    ? currentBytesToRead : _receiveBuffer.Length;

                var readBytes = _serialPort.Read(_receiveBuffer, 0, readBlockSize);
                if (readBytes > 0)
                {
                    _lastReceivedDataBlockTimestampUtc = DateTime.UtcNow;

                    var isNewConnection = _isNewConnection;
                    _isNewConnection = false;

                    this.ProcessReceivedBytes(isNewConnection, _receiveBuffer, readBytes);
                }

                currentBytesToRead = _serialPort.BytesToRead;
            }
        }
    }
}
