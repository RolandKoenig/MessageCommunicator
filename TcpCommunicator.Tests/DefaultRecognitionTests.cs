using System;
using System.Collections.Generic;
using System.Text;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TcpCommunicator.Tests
{
    [TestClass]
    public class DefaultRecognitionTests
    {
        [TestMethod]
        public void Test_DefaultRecognition_Standard()
        {
            var tcpCommunicatorMock = A.Fake<ITcpCommunicator>();
            
            var messageRecognizer = new DefaultMessageRecognizer(
                tcpCommunicatorMock,
                Encoding.UTF8);

            Message? currentMessage = null;
            messageRecognizer.ReceiveHandler = (msg) => currentMessage = msg;

            tcpCommunicatorMock.ReceiveHandler.Invoke(Encoding.UTF8.GetBytes("<4|Test>"));
            Assert.IsTrue(currentMessage.RawMessage.ToString() == "Test");
        }

        [TestMethod]
        public void Test_DefaultRecognition_TwoStandardMessages()
        {
            var tcpCommunicatorMock = A.Fake<ITcpCommunicator>();
            
            var messageRecognizer = new DefaultMessageRecognizer(
                tcpCommunicatorMock,
                Encoding.UTF8);

            var receivedMessages = new List<Message>(2);
            messageRecognizer.ReceiveHandler = (msg) => receivedMessages.Add(msg);

            tcpCommunicatorMock.ReceiveHandler.Invoke(Encoding.UTF8.GetBytes("<5|Test1><5|Test2>"));
            Assert.IsTrue(receivedMessages[0].RawMessage.ToString() == "Test1");
            Assert.IsTrue(receivedMessages[1].RawMessage.ToString() == "Test2");
        }
    }
}
