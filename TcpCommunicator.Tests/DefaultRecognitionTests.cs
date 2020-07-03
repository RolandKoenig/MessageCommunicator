using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TcpCommunicator.Tests
{
    [TestClass]
    public class DefaultRecognitionTests
    {
        [TestMethod]
        public async Task Test_DefaultRecognition_Send_Standard()
        {
            var fullMessageSent = string.Empty;

            var tcpCommunicatorMock = A.Fake<ITcpCommunicator>();
            A.CallTo(tcpCommunicatorMock)
                .Where(info => info.Method.Name == nameof(TcpCommunicatorBase.SendAsync))
                .Invokes(args =>
                {
                    var bytesToSend = (ArraySegment<byte>)args.Arguments[0];
                    fullMessageSent = Encoding.UTF8.GetString(bytesToSend.Array, bytesToSend.Offset, bytesToSend.Count);
                });

            var messageRecognizer = new DefaultMessageRecognizer(
                tcpCommunicatorMock,
                Encoding.UTF8);

            await messageRecognizer.SendAsync("Test");

            Assert.IsTrue(fullMessageSent == "<4|Test>");
        }

        [TestMethod]
        public void Test_DefaultRecognition_Receive_Standard()
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
        public void Test_DefaultRecognition_Receive_TwoStandardMessages()
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
