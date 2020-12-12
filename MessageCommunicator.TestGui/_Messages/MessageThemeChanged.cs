using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageCommunicator.TestGui
{
    public class MessageThemeChanged
    {
        public MessageCommunicatorTheme NewTheme { get; }

        public MessageThemeChanged(MessageCommunicatorTheme newTheme)
        {
            this.NewTheme = newTheme;
        }
    }
}
