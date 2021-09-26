using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;

namespace FirLib.Core.Patterns.Messaging
{
    /// <summary>
    /// Base class of all messages sent and received through ApplicationMessenger class.
    /// </summary>
    [FirLibMessage]
    public record FirLibMessage { }
}
