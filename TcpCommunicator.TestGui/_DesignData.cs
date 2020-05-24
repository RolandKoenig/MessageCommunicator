using System;
using System.Collections.Generic;
using System.Text;

namespace TcpCommunicator.TestGui
{
    public static class DesignData
    {
        public static MainWindowViewModel MainWindowVM
        {
            get
            {
                var result = new MainWindowViewModel();

                result.Logging.Add(new LoggingMessage("Message 1"));
                result.Logging.Add(new LoggingMessage("Message 2"));
                result.Logging.Add(new LoggingMessage("Message 3"));

                return result;
            }
        }
    }
}
