using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using TcpCommunicator.Util;

namespace TcpCommunicator
{
    public class TcpCommunicatorPassive : TcpCommunicatorBase
    {
        private const int RUNNING_LOOP_COUNTER_MAX = 1000;

        private readonly object _startStopLock;
        private int _runningLoopCounter;
        private bool _isRunning;

        private TcpClient? _currentSendSocket;
        private TcpListener? _currentListener;

        public ushort ListeningPort { get; }

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

        public TcpCommunicatorPassive(
            ushort listeningPort, 
            ReconnectWaitTimeGetter? reconnectWaitTimeGetter = null)
            : base(reconnectWaitTimeGetter)
        {
            _startStopLock = new object();

            this.ListeningPort = listeningPort;
        }

        /// <inheritdoc />
        public override void Start()
        {
            // Simple lock here to guard start and stop phase
            var loopId = 0;
            lock (_startStopLock)
            {
                if(_isRunning){ throw new ApplicationException($"Unable to start {nameof(TcpCommunicatorPassive)} for port {this.ListeningPort}: This object is started already!"); }

                _isRunning = true;
                _runningLoopCounter++;
                if (_runningLoopCounter > RUNNING_LOOP_COUNTER_MAX)
                {
                    _runningLoopCounter = 1;
                }

                loopId = _runningLoopCounter;
            }

            this.StartInternal(loopId);
        }

        private async void StartInternal(int loopId)
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
                    try
                    {
                        if (this.IsLoggerSet)
                        {
                            this.Log(
                                LoggingMessageType.Info, 
                                StringBuffer.Format("Creating TcpListener socket for port {0}...", this.ListeningPort));
                        }
                        tcpListener = new TcpListener(IPAddress.Loopback, this.ListeningPort);
                        tcpListener.Start();

                        reconnectErrorCount = 0;
                        _currentListener = tcpListener;

                        if (this.IsLoggerSet)
                        {
                            this.Log(
                                LoggingMessageType.Info, 
                                StringBuffer.Format("TcpListener created for port {0}", this.ListeningPort));
                        }
                    }
                    catch (Exception ex)
                    {
                        if (this.IsLoggerSet)
                        {
                            this.Log(
                                LoggingMessageType.Error, 
                                StringBuffer.Format("Error while creating TcpListener for port {0}: {1}", this.ListeningPort, ex.Message),
                                ex);
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
                                this.ListeningPort));
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
                                this.ListeningPort, actLocalEndPoint.ToString(), actPartnerEndPoint.ToString()));
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
                            StringBuffer.Format("Error while listening for incoming connections on port {0}: {1}", this.ListeningPort, ex.Message),
                            ex);
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

        /// <inheritdoc />
        public override void Stop()
        {
            // Simple lock here to guard start and stop phase
            TcpListener? lastListener = null;
            TcpClient? lastSendSocket = null;
            lock (_startStopLock)
            {
                if (!_isRunning)
                {
                    throw new ApplicationException($"Unable to stop {nameof(TcpCommunicatorPassive)} for port {this.ListeningPort}: This object is stopped already!");
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
        }

        protected override TcpClient? GetCurrentSendSocket() => _currentSendSocket;
    }
}
