using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageCommunicator.TestGui
{
    public class MessageThemeChangeRequest
    {
        public MessageCommunicatorTheme NewTheme { get; }

        public MessageThemeChangeRequest(MessageCommunicatorTheme newTheme)
        {
            this.NewTheme = newTheme;
        }
    }
}
