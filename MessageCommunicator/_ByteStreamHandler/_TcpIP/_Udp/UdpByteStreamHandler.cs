using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MessageCommunicator.Util;
using Light.GuardClauses;

namespace MessageCommunicator
{
    public class UdpByteStreamHandler : TcpIPByteStreamHandler
    {
        private const int RUNNING_LOOP_COUNTER_MAX = 1000;

        private readonly object _startStopLock;
        private int _runningLoopCounter;
        private bool _isRunning;
        private ConnectionState _connState;
        private UdpClient? _udpClient;

        public ushort ListeningPort { get; }

        public string RemoteHost { get; }

        public IPAddress RemoteIPAddress { get; }

        public ushort RemotePort { get; }

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
        public override bool IsRunning => _isRunning;

        internal UdpByteStreamHandler(
            ushort listeningPort,
            IPAddress remoteIPAddress, ushort remotePort,
            ReconnectWaitTimeGetter reconnectWaitTimeGetter)
            : base(reconnectWaitTimeGetter, TimeSpan.Zero)
        {
            remoteIPAddress.MustNotBeNull(nameof(remoteIPAddress));
            reconnectWaitTimeGetter.MustNotBeNull(nameof(reconnectWaitTimeGetter));

            this.ListeningPort = listeningPort;
            this.RemoteHost = remoteIPAddress.ToString();
            this.RemoteIPAddress = remoteIPAddress;
            this.RemotePort = remotePort;

            _startStopLock = new object();
            _udpClient = null;
            _isRunning = false;
        }

        internal UdpByteStreamHandler(
            ushort listeningPort,
            string remoteHost, ushort remotePort,
            ReconnectWaitTimeGetter reconnectWaitTimeGetter)  
            : base(reconnectWaitTimeGetter, TimeSpan.Zero)
        {
            remoteHost.MustNotBeNullOrEmpty(nameof(remoteHost));
            reconnectWaitTimeGetter.MustNotBeNull(nameof(reconnectWaitTimeGetter));

            this.ListeningPort = listeningPort;
            this.RemoteHost = remoteHost;
            this.RemoteIPAddress = IPAddress.None;
            this.RemotePort = remotePort;

            _startStopLock = new object();
            _udpClient = null;
            _isRunning = false;
        }

        /// <inheritdoc />
        protected override Task StartInternalAsync()
        {
            // Simple lock here to guard start and stop phase
            var loopId = 0;
            lock (_startStopLock)
            {
                if (_isRunning)
                {
                    throw new InvalidOperationException(
                        $"Unable to start {nameof(TcpActiveByteStreamHandler)} for host {this.RemoteHost} and port {this.RemotePort}: This object is started already!");
                }

                _isRunning = true;
                _connState = ConnectionState.Connecting;
                _runningLoopCounter++;
                if (_runningLoopCounter > RUNNING_LOOP_COUNTER_MAX)
                {
                    _runningLoopCounter = 1;
                }

                loopId = _runningLoopCounter;
            }

            this.RunConnectionMainLoop(loopId);

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        protected override Task StopInternalAsync()
        {
            // Simple lock here to guard start and stop phase
            UdpClient? lastSendSocket = null;
            lock (_startStopLock)
            {
                if (!_isRunning)
                {
                    throw new InvalidOperationException(
                        $"Unable to stop {nameof(TcpActiveByteStreamHandler)} for host {this.RemoteHost} and port {this.RemotePort}: This object is stopped already!");
                }

                _isRunning = false;
                _connState = ConnectionState.Stopped;
                _runningLoopCounter++;
                if (_runningLoopCounter > RUNNING_LOOP_COUNTER_MAX)
                {
                    _runningLoopCounter = 1;
                }
                lastSendSocket = _udpClient;
                _udpClient = null;
            }

            // Dispose previous sockets
            TcpAsyncUtil.SafeDispose(ref lastSendSocket);

            this.Log(LoggingMessageType.Info, "UDP communication stopped");

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        protected override Socket? GetCurrentSendSocket()
        {
            var udpClient = _udpClient;
            return udpClient?.Client;
        }

        private async void RunConnectionMainLoop(int loopId)
        {
            this.Log(LoggingMessageType.Info, "TcpCommunicator started in active mode");

            // Ensure that we switch to the ThreadPool
            await Task.CompletedTask.ConfigureAwait(false);

            var reconnectErrorCount = 0;
            var remoteAddressStr = this.RemoteHost;
            while (loopId == _runningLoopCounter)
            {
                UdpClient? newClient = null;
                try
                {
                    _connState = ConnectionState.Connecting;
                    if (this.IsLoggerSet)
                    {
                        this.Log(
                            LoggingMessageType.Info,
                            StringBuffer.Format("Connecting to host {0} on port {1}", remoteAddressStr,
                                this.RemotePort));
                    }

                    newClient = new UdpClient(this.ListeningPort);
                    if (!ReferenceEquals(this.RemoteIPAddress, IPAddress.None))
                    {
                        newClient.Connect(this.RemoteIPAddress, this.RemotePort);
                    }
                    else
                    {
                        newClient.Connect(this.RemoteHost, this.RemotePort);
                    }
                    _udpClient = newClient;
                    reconnectErrorCount = 0;

                    _connState = ConnectionState.Connected;
                    if (this.IsLoggerSet)
                    {
                        this.Log(
                            LoggingMessageType.Info,
                            StringBuffer.Format("Successfully connected to host {0} on port {1}", remoteAddressStr,
                                this.RemotePort));
                    }
                }
                catch (Exception ex)
                {
                    TcpAsyncUtil.SafeDispose(ref newClient);
                    _udpClient = null;

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
                if (_udpClient == null) { continue; }

                // Normal receive loop handling
                try
                {
                    await this.RunReceiveLoopAsync(
                        _udpClient, _udpClient.Client, 
                        (IPEndPoint)_udpClient.Client.LocalEndPoint!,(IPEndPoint)_udpClient.Client.RemoteEndPoint!,
                        CancellationToken.None);
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
                _udpClient = null;
            }
        }
    }
}
