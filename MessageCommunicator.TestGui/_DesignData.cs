using System.Threading;
using MessageCommunicator.TestGui.Data;
using MessageCommunicator.TestGui.Logic;
using MessageCommunicator.TestGui.Views;
using MessageCommunicator.TestGui.ViewServices;

namespace MessageCommunicator.TestGui
{
    public static class DesignData
    {
        public static MainWindowViewModel MainWindowVM
        {
            get
            {
                var result = new MainWindowViewModel();
                var dummyConnParams = new ConnectionParameters { Name = "Dummy Profile" };

                var dummyContext = new DummySynchronizationContext();
                result.Profiles.Add(new ConnectionProfileViewModel(new ConnectionProfile(dummyContext, dummyConnParams)));
                result.Profiles.Add(new ConnectionProfileViewModel(new ConnectionProfile(dummyContext, dummyConnParams)));
                result.Profiles.Add(new ConnectionProfileViewModel(new ConnectionProfile(dummyContext, dummyConnParams)));
                result.Profiles.Add(new ConnectionProfileViewModel(new ConnectionProfile(dummyContext, dummyConnParams)));

                return result;
            }
        }

        public static ConnectionConfigControlViewModel ConnectionConfigVM => new ConnectionConfigControlViewModel();

        public static ErrorDialogViewModel ErrorDialogVM
        {
            get
            {
                return new ErrorDialogViewModel(
                    "Dummy-Title",
                    "Dummy-Message, Dummy-Message, Dummy-Message, Dummy-Message, ...");
            }
        }

        //*****************************************************************
        //*****************************************************************
        //*****************************************************************
        internal class DummySynchronizationContext : SynchronizationContext
        {
            /// <inheritdoc />
            public override void Send(SendOrPostCallback d, object? state)
            {
                
            }

            /// <inheritdoc />
            public override void Post(SendOrPostCallback d, object? state)
            {
                
            }
        }
    }
}
