using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirLib.Core.Patterns.Messaging
{
    public class MessageAsyncRoutingTargetsAttribute : Attribute
    {
        public string[] AsyncTargetMessengers { get; }

        public MessageAsyncRoutingTargetsAttribute(params string[] asyncTargetMessengers)
        {
            this.AsyncTargetMessengers = asyncTargetMessengers;
        }
    }
}