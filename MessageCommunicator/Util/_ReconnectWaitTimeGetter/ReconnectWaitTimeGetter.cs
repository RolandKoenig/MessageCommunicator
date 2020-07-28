using System;
using System.Collections.Generic;

namespace TcpCommunicator
{
    public abstract class ReconnectWaitTimeGetter
    {
        public abstract TimeSpan GetWaitTime(int errorCountSinceLastConnect);
    }
}
