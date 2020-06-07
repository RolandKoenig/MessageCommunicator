using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TcpCommunicator
{
    public class TcpCommunicatorPassive : TcpCommunicatorBase
    {
        private const int RUNNING_LOOP_COUNTER_MAX = 1000;

        //private CancellationTokenSource? _cancelTokenAcceptLoop;
        //private CancellationTokenSource? _cancelTokenReceiveLoop;

        private readonly object _startStopLock;
        private int _runningLoopCounter;
        private bool _isRunning;

        private TcpClient? _currentSendSocket;
        private TcpListener? _currentListener;

        public int ListeningPort { get; }

        public override bool IsRunning => _isRunning;

        public TcpCommunicatorPassive(
            int listeningPort, 
            ReconnectWaitTimeGetter? reconnectWaitTimeGetter = null)
            : base(reconnectWaitTimeGetter)
        {
            _startStopLock = new object();

            this.ListeningPort = listeningPort;
        }

        /// <inheritdoc />
        public override async void Start()
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
                        this.Log(LoggingMessageType.Info, "Start listening...");
                        tcpListener = new TcpListener(IPAddress.Loopback, this.ListeningPort);
                        tcpListener.Start();

                        reconnectErrorCount = 0;
                        _currentListener = tcpListener;

                        this.Log(LoggingMessageType.Info, "Listening started successfully");
                    }
                    catch (Exception ex)
                    {
                        this.Log(LoggingMessageType.Info, "Error while starting listening", ex);

                        _currentListener = null;
                        TcpAsyncUtil.SafeStop(ref tcpListener);
                        TcpAsyncUtil.SafeDispose(ref _currentSendSocket);

                        await base.WaitByReconnectWaitTimeAsync(reconnectErrorCount)
                            .ConfigureAwait(false);
                        reconnectErrorCount++;
                    }

                    if (tcpListener == null) { continue; }
                }

                // Wait for incoming connection
                TcpClient? actTcpClient = null;
                try
                {
                    // TODO: Log start listening for incoming connection
                    actTcpClient = await tcpListener.AcceptTcpClientAsync()
                        .ConfigureAwait(false);

                    // TODO: Log incoming connection established
                }
                catch (Exception)
                {
                    // TODO: Log error while waiting for incoming connection

                    TcpAsyncUtil.SafeStop(ref tcpListener);
                    TcpAsyncUtil.SafeDispose(ref _currentSendSocket);

                    await base.WaitByReconnectWaitTimeAsync(reconnectErrorCount)
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
                this.RunReceiveLoopAsync(lastClient, lastCancelTokenSource.Token);
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
                if(!_isRunning){ throw new ApplicationException($"Unable to stop {nameof(TcpCommunicatorPassive)} for port {this.ListeningPort}: This object is stopped already!"); }

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
        }

        protected override TcpClient? GetCurrentSendSocket() => _currentSendSocket;
    }
}
