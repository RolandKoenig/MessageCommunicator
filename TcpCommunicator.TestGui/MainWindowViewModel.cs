using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace TcpCommunicator.TestGui
{
    public class MainWindowViewModel : PropertyChangedBase
    {
        public ObservableCollection<LoggingMessage> Logging { get; } = new ObservableCollection<LoggingMessage>();


    }
}
