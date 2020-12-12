using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageCommunicator.TestGui
{
    public interface IWeakMessageTarget<in T>
    {
        void OnMessage(T message);
    }
}
