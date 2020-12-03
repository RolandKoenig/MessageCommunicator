using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using FakeItEasy;
using MessageCommunicator.TestGui.Data;
using MessageCommunicator.TestGui.Logic;
using MessageCommunicator.TestGui.Views;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MessageCommunicator.TestGui.Tests
{
    [TestClass]
    public class ConnectionProfileViewModelTests
    {
        [TestMethod]
        public void SendPlainMessage()
        {
            var connParams = new ConnectionParameters();
            var connProfile = BuildFakeConnectionProfile(connParams);

            // Catch outgoing message
            string? sentMessage = null;
            A.CallTo(connProfile)
                .Where((callInfo) => callInfo.Method.Name == nameof(IConnectionProfile.SendMessageAsync))
                .Invokes((callInfo) =>
                {
                    sentMessage = (string?)callInfo.Arguments[0];
                });

            var testObject = new ConnectionProfileViewModel(connProfile);
            testObject.SendFormattingMode = SendFormattingMode.Plain;
            testObject.Command_SendMessage.Execute("DummyMessage \\\\");

            Assert.IsTrue(sentMessage == "DummyMessage \\\\");
        }

        [TestMethod]
        public void SendEscapedMessage()
        {
            var connParams = new ConnectionParameters();
            var connProfile = BuildFakeConnectionProfile(connParams);

            // Catch outgoing message
            string? sentMessage = null;
            A.CallTo(connProfile)
                .Where((callInfo) => callInfo.Method.Name == nameof(IConnectionProfile.SendMessageAsync))
                .Invokes((callInfo) =>
                {
                    sentMessage = (string?)callInfo.Arguments[0];
                });

            var testObject = new ConnectionProfileViewModel(connProfile);
            testObject.SendFormattingMode = SendFormattingMode.Escaped;
            testObject.Command_SendMessage.Execute("DummyMessage \\\\");

            Assert.IsTrue(sentMessage == "DummyMessage \\");
        }

        [TestMethod]
        public void SendHexMessage()
        {
            var connParams = new ConnectionParameters();
            var connProfile = BuildFakeConnectionProfile(connParams);

            // Catch outgoing message
            string? sentMessage = null;
            A.CallTo(connProfile)
                .Where((callInfo) => callInfo.Method.Name == nameof(IConnectionProfile.SendMessageAsync))
                .Invokes((callInfo) =>
                {
                    sentMessage = (string?)callInfo.Arguments[0];
                });

            var testObject = new ConnectionProfileViewModel(connProfile);
            testObject.SendFormattingMode = SendFormattingMode.BinaryHex;
            testObject.Command_SendMessage.Execute("41 42 43");

            Assert.IsTrue(sentMessage == "ABC");
        }

        private static IConnectionProfile BuildFakeConnectionProfile(ConnectionParameters connParams)
        {
            var loggingMessages = new ObservableCollection<LoggingMessageWrapper>();
            var sendReceiveMessages = new ObservableCollection<LoggingMessageWrapper>();

            var connProfile = A.Fake<IConnectionProfile>();
            A.CallTo(() => connProfile.Name).Returns("Dummy");
            A.CallTo(() => connProfile.IsRunning).Returns(true);
            A.CallTo(() => connProfile.DetailLogging).Returns(loggingMessages);
            A.CallTo(() => connProfile.Messages).Returns(sendReceiveMessages);
            A.CallTo(() => connProfile.Parameters).Returns(connParams);
            A.CallTo(() => connProfile.RemoteEndpointDescription).Returns("Dummy Remote");
            A.CallTo(() => connProfile.State).Returns(ConnectionState.Connected);

            return connProfile;
        }
    }
}
