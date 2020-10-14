using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Threading.Tasks;

namespace MessageCommunicator.Util
{
    public class DefaultMessageChannelConnectionObserver : IMessageChannelConnectionObserver, IDisposable
    {
        private readonly TimeSpan _connectionTimeout;
        private readonly TimeSpan _heartbeat;

        private readonly object _channelsLock;
        private ImmutableList<MessageChannel> _channels;
        private bool _isDisposed;

        public Task ObserverTask { get; }

        public DefaultMessageChannelConnectionObserver()
            : this(TimeSpan.FromSeconds(40.0), TimeSpan.FromSeconds(1))
        {

        }

        public DefaultMessageChannelConnectionObserver(TimeSpan connectionTimeout, TimeSpan heartbeat)
        {
            _connectionTimeout = connectionTimeout;
            _heartbeat = heartbeat;

            _channels = ImmutableList<MessageChannel>.Empty;
            _channelsLock = new object();
            _isDisposed = false;

            this.ObserverTask = this.ObserverMainLoopAsync();
        }

        /// <inheritdoc />
        public void RegisterMessageChannel(MessageChannel channel)
        {
            lock (_channelsLock)
            {
                if (_isDisposed) { return; }

                _channels = _channels.Add(channel);
            }
        }

        /// <inheritdoc />
        public void DeregisterMessageChannel(MessageChannel channel)
        {
            lock (_channelsLock)
            {
                if (_isDisposed) { return; }

                _channels = _channels.Remove(channel);
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _isDisposed = true;

            lock (_channelsLock)
            {
                _channels = ImmutableList<MessageChannel>.Empty;
            }
        }

        private async Task ObserverMainLoopAsync()
        {
            while (!_isDisposed)
            {
                await Task.Delay(_heartbeat)
                    .ConfigureAwait(false);

                var currentTimeStamp = DateTime.UtcNow;
                foreach (var actChannel in _channels)
                {
                    if ((actChannel.State == ConnectionState.Connected) &&
                        (currentTimeStamp - actChannel.LastSuccessfulConnectTimestampUtc > _connectionTimeout) &&
                        (currentTimeStamp - actChannel.LastReceivedTimestampUtc > _connectionTimeout))
                    {
                        actChannel.TriggerReconnect();
                    }
                }
            }
        }
    }
}
