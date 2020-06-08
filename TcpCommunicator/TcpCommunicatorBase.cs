using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TcpCommunicator
{
    public abstract class TcpCommunicatorBase
    {
        public ReconnectWaitTimeGetter ReconnectWaitTimeGetter { get; set; }

        public abstract bool IsRunning { get; }

        /// <summary>
        /// A custom logger. If set, this delegate will be called with all relevant events.
        /// </summary>
        public Action<LoggingMessage>? Logger { get; set; }

        public SynchronizationContext? SynchronizationContext { get; set; }

        protected TcpCommunicatorBase(ReconnectWaitTimeGetter? reconnectWaitTimeGetter)
        {
            this.ReconnectWaitTimeGetter = reconnectWaitTimeGetter ?? new FixedReconnectWaitTimeGetter(TimeSpan.FromSeconds(1.0));
        }

        /// <summary>
        /// Calls current logger with the given message.
        /// </summary>
        protected void Log(LoggingMessageType messageType, string message, Exception? exception = null)
        {
            var logger = this.Logger;

            var syncContext = this.SynchronizationContext;
            if (syncContext != null)
            {
                // TODO: Reduce allocations
                syncContext.Post(new SendOrPostCallback(
                    arg => logger?.Invoke(new LoggingMessage(this, messageType, message, exception))), null);
            }
            else
            {
                logger?.Invoke(new LoggingMessage(this, messageType, message, exception));
            }
        }

        protected Task WaitByReconnectWaitTimeAsync(int errorCountSinceLastConnect)
        {
            var waitTime = this.ReconnectWaitTimeGetter.GetWaitTime(errorCountSinceLastConnect);
            if (waitTime <= TimeSpan.Zero)
            {
                return TcpAsyncUtil.DummyFinishedTask;
            }

            return Task.Delay(waitTime);
        }

        public abstract void Start();

        public abstract void Stop();

        public async Task SendAsync(ArraySegment<byte> buffer)
        {
            var currentClient = this.GetCurrentSendSocket();
            if(currentClient == null)
            {
                this.Log(LoggingMessageType.Error, "Unable to send message: Connection is not established currently!");
                return;
            }

            try
            {
                await currentClient.Client.SendAsync(buffer, SocketFlags.None)
                    .ConfigureAwait(false);
            }
            catch (Exception sendException)
            {
                throw;
            }
        }

        protected abstract TcpClient? GetCurrentSendSocket();
         
        protected async Task RunReceiveLoopAsync(TcpClient tcpClient, CancellationToken cancelToken)
        {
            var receiveBuffer = new byte[1024];
            while (!cancelToken.IsCancellationRequested)
            {
                int lastReceiveResult;
                try
                {
                    lastReceiveResult = await tcpClient.Client.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), SocketFlags.None)
                        .ConfigureAwait(false);
                }
                catch (SocketException socketException)
                {
                    // TODO: Notify disconnected event
                    break;
                }
                catch (ObjectDisposedException disposedException)
                {
                    // TODO: Notify disconnected event
                    break;
                }
                if (lastReceiveResult <= 0) { break; }

                // TODO: Reduce allocations
                this.Log(
                    LoggingMessageType.Info,
                    $"Received: {Encoding.Default.GetString(receiveBuffer, 0, lastReceiveResult)}");
            }

            // Ensure that the socket is closed after ending this method
            if (tcpClient.Connected)
            {
                tcpClient.Client.Disconnect(false);
                tcpClient.Client.Dispose();
                tcpClient.Dispose();
            }
        }
    }
}
