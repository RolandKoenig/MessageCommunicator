using System;
using System.Collections.Generic;

namespace MessageCommunicator
{
    /// <summary>
    /// This class is responsible to get the wait time before reconnect when a connection on a <see cref="IByteStreamHandler"/> got lost.
    /// </summary>
    public abstract class ReconnectWaitTimeGetter
    {
        /// <summary>
        /// Gets the next wait time before trying to reconnect.
        /// </summary>
        /// <param name="errorCountSinceLastConnect">Total count of connection errors since last established connection.</param>
        public abstract TimeSpan GetWaitTime(int errorCountSinceLastConnect);
    }
}
