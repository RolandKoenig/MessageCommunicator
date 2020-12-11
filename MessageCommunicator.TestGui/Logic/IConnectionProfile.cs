using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using MessageCommunicator.TestGui.Data;

namespace MessageCommunicator.TestGui.Logic
{
    public interface IConnectionProfile
    {
        string Name { get; }

        ConnectionParameters Parameters { get; }

        ObservableCollection<LoggingMessageWrapper> DetailLogging { get; }

        ObservableCollection<LoggingMessageWrapper> Messages { get; }

        bool IsRunning { get; }

        ConnectionState State { get; }

        string RemoteEndpointDescription { get; }

        string LocalEndpointDescription { get; }

        public int CountMessagesIn { get; }

        public int CountMessagesOut { get; }

        public int CountErrors { get; }

        Task ChangeParametersAsync(ConnectionParameters newConnParameters);

        Task SendMessageAsync(string message);

        Task StartAsync();

        Task StopAsync();
    }
}
