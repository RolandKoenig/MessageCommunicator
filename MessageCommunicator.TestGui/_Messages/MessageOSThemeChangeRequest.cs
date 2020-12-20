using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageCommunicator.TestGui
{
    public class MessageOSThemeChangeRequest
    {
        public MessageCommunicatorTheme NewTheme { get; }

        public MessageOSThemeChangeRequest(MessageCommunicatorTheme newTheme)
        {
            this.NewTheme = newTheme;
        }
    }
}
