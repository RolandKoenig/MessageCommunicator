using System;
using System.Collections.Generic;

namespace MessageCommunicator
{
    /// <summary>
    /// This class is responsible to get the wait time before reconnect when a connection on a <see cref="IByteStreamHandler"/> got lost.
    /// In this implementation we use always the same wait time.
    /// </summary>
    public class FixedReconnectWaitTimeGetter : ReconnectWaitTimeGetter
    {
        private readonly TimeSpan _fixedWaitTime;

        /// <summary>
        /// Creates a new <see cref="FixedReconnectWaitTimeGetter"/>.
        /// </summary>
        /// <param name="fixedWaitTime">The time to be waited before each reconnect.</param>
        public FixedReconnectWaitTimeGetter(TimeSpan fixedWaitTime)
        {
            _fixedWaitTime = fixedWaitTime;
        }

        /// <inheritdoc />
        public override TimeSpan GetWaitTime(int errorCountSinceLastConnect)
        {
            return _fixedWaitTime;
        }
    }
}
