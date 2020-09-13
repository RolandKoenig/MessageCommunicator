using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using MessageCommunicator.Util;

namespace MessageCommunicator
{
    public class TcpActiveByteStreamHandler : TcpByteStreamHandler
    {
        private const int RUNNING_LOOP_COUNTER_MAX = 1000;

        private readonly object _startStopLock;
        private int _runningLoopCounter;
        private bool _isRunning;
        private ConnectionState _connState;

        private TcpClient? _currentClient;

        public string RemoteHost { get; }

        public ushort RemotePort { get; }

        /// <inheritdoc />
        public override bool IsRunning => _isRunning;

        /// <inheritdoc />
        public override ConnectionState State
        {
            get
            {
                if (!_isRunning) { return ConnectionState.Stopped; }
                return _connState;
            }
        }

        /// <inheritdoc />
        internal TcpActiveByteStreamHandler(
            string remoteHost, ushort remotePort, 
            ReconnectWaitTimeGetter reconnectWaitTimeGetter) 
            : base(reconnectWaitTimeGetter)
        {
            _startStopLock = new object();
            this.RemoteHost = remoteHost;
            this.RemotePort = remotePort;
        }

        /// <inheritdoc />
        protected override Task StartInternalAsync()
        {
            // Simple lock here to guard start and stop phase
            var loopId = 0;
            lock (_startStopLock)
            {
                if(_isRunning){ throw new InvalidOperationException($"Unable to start {nameof(TcpActiveByteStreamHandler)} for host {this.RemoteHost} and port {this.RemotePort}: This object is started already!"); }

                _isRunning = true;
                _connState = ConnectionState.Connecting;
                _runningLoopCounter++;
                if (_runningLoopCounter > RUNNING_LOOP_COUNTER_MAX)
                {
                    _runningLoopCounter = 1;
                }

                loopId = _runningLoopCounter;
            }

            // Trigger async main loop which handles the connection
            this.RunConnectionMainLoop(loopId);

            return Task.FromResult<object?>(null);
        }

        /// <inheritdoc />
        protected override Task StopInternalAsync()
        {
            // Simple lock here to guard start and stop phase
            TcpClient? lastSendSocket = null;
            lock (_startStopLock)
            {
                if (!_isRunning)
                {
                    throw new InvalidOperationException($"Unable to stop {nameof(TcpActiveByteStreamHandler)} for host {this.RemoteHost} and port {this.RemotePort}: This object is stopped already!");
                }

                _isRunning = false;
                _connState = ConnectionState.Stopped;
                _runningLoopCounter++;
                if (_runningLoopCounter > RUNNING_LOOP_COUNTER_MAX)
                {
                    _runningLoopCounter = 1;
                }
                lastSendSocket = _currentClient;
                _currentClient = null;
            }

            // Dispose previous sockets
            TcpAsyncUtil.SafeDispose(ref lastSendSocket);

            this.Log(LoggingMessageType.Info, "TcpCommunicator stopped");

            return Task.FromResult<object?>(null);
        }

        private async void RunConnectionMainLoop(int loopId)
        {
            this.Log(LoggingMessageType.Info, "TcpCommunicator started in active mode");

            var reconnectErrorCount = 0;
            var remoteAddressStr = this.RemoteHost;
            while (loopId == _runningLoopCounter)
            {
                TcpClient? newClient = null;
                try
                {
                    _connState = ConnectionState.Connecting;
                    if (this.IsLoggerSet)
                    {
                        this.Log(
                            LoggingMessageType.Info,
                            StringBuffer.Format("Connecting to host {0} on port {1}", remoteAddressStr, this.RemotePort));
                    }

                    newClient = new TcpClient();
                    await newClient.ConnectAsync(this.RemoteHost, this.RemotePort);
                    _currentClient = newClient;
                    reconnectErrorCount = 0;

                    _connState = ConnectionState.Connected;
                    if (this.IsLoggerSet)
                    {
                        this.Log(
                            LoggingMessageType.Info,
                            StringBuffer.Format("Successfully connected to host {0} on port {1}", remoteAddressStr, this.RemotePort));
                    }
                }
                catch (Exception ex)
                {
                    TcpAsyncUtil.SafeDispose(ref newClient);
                    _currentClient = null;

                    if (this.IsLoggerSet)
                    {
                        this.Log(
                            LoggingMessageType.Error,
                            StringBuffer.Format("Error while connecting to host {0} on port {1}: {2}", 
                                remoteAddressStr, this.RemotePort, ex.Message),
                            exception: ex);
                    }

                    _connState = ConnectionState.Connecting;
                    await this.WaitByReconnectWaitTimeAsync(reconnectErrorCount)
                        .ConfigureAwait(false);
                    reconnectErrorCount++;
                }
                if(_currentClient == null){ continue; }

                // Normal receive loop handling
                try
                {
                    await this.RunReceiveLoopAsync(
                        _currentClient, (IPEndPoint)_currentClient.Client.LocalEndPoint,
                        (IPEndPoint)_currentClient.Client.RemoteEndPoint, CancellationToken.None);
                }
                catch (Exception ex)
                {
                    if (this.IsLoggerSet)
                    {
                        this.Log(
                            LoggingMessageType.Error,
                            StringBuffer.Format(
                                "Error while running receive loop for host {0} on port {1}: {2}", 
                                remoteAddressStr, this.RemotePort, ex.Message),
                            exception: ex);
                    }
                }

                // Client gets already disposed in receive loop
                _currentClient = null;
            }
        }

        /// <inheritdoc />
        protected override TcpClient? GetCurrentSendSocket()
        {
            return _currentClient;
        }
    }
}
