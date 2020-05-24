using System;
using System.Collections.Generic;
using System.Text;

namespace TcpCommunicator
{
    public abstract class ReconnectWaitTimeGetter
    {
        public abstract TimeSpan GetWaitTime(int errorCountSinceLastConnect);
    }
}
