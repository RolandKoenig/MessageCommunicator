using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageCommunicator.TestGui.Data
{
    public interface IByteStreamHandlerAppSettings
    {
        ByteStreamHandlerSettings CreateLibSettings();
    }
}
