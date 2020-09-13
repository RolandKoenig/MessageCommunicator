using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using MessageCommunicator.Util;

namespace MessageCommunicator
{
    public class TcpPassiveByteStreamHandler : TcpByteStreamHandler
    {
        private const int RUNNING_LOOP_COUNTER_MAX = 1000;

        private readonly object _startStopLock;
        private int _runningLoopCounter;
        private bool _isRunning;

        private TcpClient? _currentSendSocket;
        private TcpListener? _currentListener;

        public IPAddress ListeningIPAddress { get; }

        public ushort ListeningPort { get; }

        /// <summary>
        /// Gets the true listening port in case ListeningPort is set to 0.
        /// </summary>
        public ushort ActualListeningPort { get; private set; }

        public override bool IsRunning => _isRunning;

        /// <inheritdoc />
        public override ConnectionState State
        {
            get
            {
                if (!_isRunning) { return ConnectionState.Stopped; }

                var currentSendSocket = _currentSendSocket;
                if (currentSendSocket == null) { return ConnectionState.Connecting; }
                else if (!currentSendSocket.Connected) { return ConnectionState.Connecting;}
                else
                {
                    return ConnectionState.Connected;
                }
            }
        }

        internal TcpPassiveByteStreamHandler(
            IPAddress listeningIPAddress,
            ushort listeningPort, 
            ReconnectWaitTimeGetter reconnectWaitTimeGetter)
            : base(reconnectWaitTimeGetter)
        {
            _startStopLock = new object();

            this.ListeningIPAddress = listeningIPAddress;
            this.ListeningPort = listeningPort;
            this.ActualListeningPort = listeningPort;
        }

        /// <inheritdoc />
        protected override Task StartInternalAsync()
        {
            // Simple lock here to guard start and stop phase
            var loopId = 0;
            lock (_startStopLock)
            {
                if(_isRunning){ throw new InvalidOperationException($"Unable to start {nameof(TcpPassiveByteStreamHandler)} for port {this.ListeningPort}: This object is started already!"); }

                _isRunning = true;
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
            TcpListener? lastListener = null;
            TcpClient? lastSendSocket = null;
            lock (_startStopLock)
            {
                if (!_isRunning)
                {
                    throw new InvalidOperationException($"Unable to stop {nameof(TcpPassiveByteStreamHandler)} for port {this.ListeningPort}: This object is stopped already!");
                }

                _isRunning = false;
                _runningLoopCounter++;
                if (_runningLoopCounter > RUNNING_LOOP_COUNTER_MAX)
                {
                    _runningLoopCounter = 1;
                }

                lastListener = _currentListener;
                lastSendSocket = _currentSendSocket;
                _currentListener = null;
                _currentSendSocket = null;
            }

            // Dispose previous sockets
            TcpAsyncUtil.SafeStop(ref lastListener);
            TcpAsyncUtil.SafeDispose(ref lastSendSocket);

            this.Log(LoggingMessageType.Info, "TcpCommunicator stopped");

            return Task.FromResult<object?>(null);
        }

        private async void RunConnectionMainLoop(int loopId)
        {
            this.Log(LoggingMessageType.Info, "TcpCommunicator started in passive mode");

            // Start main loop
            TcpClient? lastClient = null;
            CancellationTokenSource? lastCancelTokenSource = null;
            TcpListener? tcpListener = null;
            var reconnectErrorCount = 0;
            while(loopId == _runningLoopCounter)
            {
                if (tcpListener == null)
                {
                    this.ActualListeningPort = this.ListeningPort;
                    try
                    {
                        if (this.IsLoggerSet)
                        {
                            this.Log(
                                LoggingMessageType.Info, 
                                StringBuffer.Format("Creating TcpListener socket for port {0}...", this.ListeningPort));
                        }
                        tcpListener = new TcpListener(this.ListeningIPAddress, this.ListeningPort);
                        tcpListener.Start();
                        this.ActualListeningPort = (ushort)((IPEndPoint)tcpListener.LocalEndpoint).Port;

                        reconnectErrorCount = 0;
                        _currentListener = tcpListener;

                        if (this.IsLoggerSet)
                        {
                            this.Log(
                                LoggingMessageType.Info, 
                                StringBuffer.Format("TcpListener created for port {0}", this.ActualListeningPort));
                        }
                    }
                    catch (Exception ex)
                    {
                        if (this.IsLoggerSet)
                        {
                            this.Log(
                                LoggingMessageType.Error, 
                                StringBuffer.Format("Error while creating TcpListener for port {0}: {1}", this.ActualListeningPort, ex.Message),
                                exception: ex);
                        }

                        _currentListener = null;
                        TcpAsyncUtil.SafeStop(ref tcpListener);
                        TcpAsyncUtil.SafeDispose(ref _currentSendSocket);

                        await this.WaitByReconnectWaitTimeAsync(reconnectErrorCount)
                            .ConfigureAwait(false);
                        reconnectErrorCount++;
                    }

                    if (tcpListener == null) { continue; }
                }

                // Wait for incoming connection
                TcpClient? actTcpClient = null;
                IPEndPoint? actPartnerEndPoint = null;
                IPEndPoint? actLocalEndPoint = null;
                try
                {
                    if (this.IsLoggerSet)
                    {
                        this.Log(
                            LoggingMessageType.Info,
                            StringBuffer.Format("Listening for incoming connections on port {0}...",
                            this.ActualListeningPort));
                    }

                    actTcpClient = await tcpListener.AcceptTcpClientAsync()
                        .ConfigureAwait(false);
                    actLocalEndPoint = (IPEndPoint) actTcpClient.Client.LocalEndPoint;
                    actPartnerEndPoint = (IPEndPoint) actTcpClient.Client.RemoteEndPoint;

                    if (this.IsLoggerSet)
                    {
                        this.Log(
                            LoggingMessageType.Info,
                            StringBuffer.Format(
                                "Got new connection on listening port {0}. Connection established between {1} and {2}", 
                                this.ActualListeningPort, actLocalEndPoint.ToString(), actPartnerEndPoint.ToString()));
                    }
                }
                catch (ObjectDisposedException)
                {
                    // Stop may be called in the meanwhile

                    if (!_isRunning) { continue; }
                    if (loopId != _runningLoopCounter){ continue; }
                }
                catch (Exception ex)
                {
                    if (this.IsLoggerSet)
                    {
                        this.Log(
                            LoggingMessageType.Error, 
                            StringBuffer.Format("Error while listening for incoming connections on port {0}: {1}", this.ActualListeningPort, ex.Message),
                            exception: ex);
                    }

                    TcpAsyncUtil.SafeStop(ref tcpListener);
                    TcpAsyncUtil.SafeDispose(ref _currentSendSocket);

                    await this.WaitByReconnectWaitTimeAsync(reconnectErrorCount)
                        .ConfigureAwait(false);
                    reconnectErrorCount++;
                }
                if(tcpListener == null){ continue; }
                if(actTcpClient == null){ continue; }

                // New connection request coming in... close the previous one
                if (lastClient != null)
                {
                    lastCancelTokenSource!.Cancel();
                    lastCancelTokenSource = null;
                    lastClient = null;
                }

                // Register new client
                _currentSendSocket = actTcpClient;
                lastClient = actTcpClient;
                lastCancelTokenSource = new CancellationTokenSource();

                // Run receive loop for this client (we don't have to await this because we a listening for a new connection request)
#pragma warning disable 4014
                this.RunReceiveLoopAsync(lastClient, actLocalEndPoint!, actPartnerEndPoint!, lastCancelTokenSource.Token);
#pragma warning restore 4014
            }

            if (tcpListener != null)
            {
                TcpAsyncUtil.SafeStop(ref tcpListener);
            }
        }

        protected override TcpClient? GetCurrentSendSocket() => _currentSendSocket;
    }
}
