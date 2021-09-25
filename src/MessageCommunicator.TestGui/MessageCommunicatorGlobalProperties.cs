using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageCommunicator.TestGui
{
    public class MessageCommunicatorGlobalProperties
    {
        public static MessageCommunicatorGlobalProperties Current
        {
            get;
            set;
        } = new MessageCommunicatorGlobalProperties();

        public MessageCommunicatorTheme CurrentTheme
        {
            get;
            set;
        }

        public MessageCommunicatorGlobalProperties()
        {
            this.CurrentTheme = MessageCommunicatorTheme.Light;
        }
    }
}
