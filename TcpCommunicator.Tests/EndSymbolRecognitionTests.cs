using System;
using System.Collections.Generic;
using System.Text;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TcpCommunicator.Tests
{
    [TestClass]
    public class EndSymbolRecognitionTests
    {
        [TestMethod]
        public void Test_EndSymbols_StandardMessage()
        {
            var tcpCommunicatorMock = A.Fake<ITcpCommunicator>();
            
            var messageRecognizer = new EndSymbolMessageRecognizer(
                tcpCommunicatorMock,
                Encoding.Unicode, new char[]{'#', '#'});

            Message? currentMessage = null;
            messageRecognizer.ReceiveHandler = (msg) => currentMessage = msg;

            tcpCommunicatorMock.ReceiveHandler.Invoke(Encoding.Unicode.GetBytes("Test##"));
            Assert.IsTrue(currentMessage.RawMessage.ToString() == "Test");
        }

        [TestMethod]
        public void Test_EndSymbols_TwoStandardMessages()
        {
            var tcpCommunicatorMock = A.Fake<ITcpCommunicator>();
            
            var messageRecognizer = new EndSymbolMessageRecognizer(
                tcpCommunicatorMock,
                Encoding.Unicode, new char[]{'#', '#'});

            var receivedMessages = new List<Message>(2);
            messageRecognizer.ReceiveHandler = (msg) => receivedMessages.Add(msg);

            tcpCommunicatorMock.ReceiveHandler.Invoke(Encoding.Unicode.GetBytes("Test1##Test2##"));
            Assert.IsTrue(receivedMessages[0].RawMessage.ToString() == "Test1");
            Assert.IsTrue(receivedMessages[1].RawMessage.ToString() == "Test2");
        }

        [TestMethod]
        public void Test_EndSymbols_SpecialCase_TwoParts()
        {
            var tcpCommunicatorMock = A.Fake<ITcpCommunicator>();
            
            var messageRecognizer = new EndSymbolMessageRecognizer(
                tcpCommunicatorMock,
                Encoding.Unicode, new char[]{'#', '#'});

            var receivedMessages = new List<Message>(2);
            messageRecognizer.ReceiveHandler = (msg) => receivedMessages.Add(msg);

            tcpCommunicatorMock.ReceiveHandler.Invoke(Encoding.Unicode.GetBytes("Tes"));
            tcpCommunicatorMock.ReceiveHandler.Invoke(Encoding.Unicode.GetBytes("t##"));
            Assert.IsTrue(receivedMessages[0].RawMessage.ToString() == "Test");
        }

        [TestMethod]
        public void Test_EndSymbols_SpecialCase_CuttedEndSymbol()
        {
            var tcpCommunicatorMock = A.Fake<ITcpCommunicator>();
            
            var messageRecognizer = new EndSymbolMessageRecognizer(
                tcpCommunicatorMock,
                Encoding.Unicode, new char[]{'#', '#'});

            var receivedMessages = new List<Message>(2);
            messageRecognizer.ReceiveHandler = (msg) => receivedMessages.Add(msg);

            tcpCommunicatorMock.ReceiveHandler.Invoke(Encoding.Unicode.GetBytes("Tes"));
            tcpCommunicatorMock.ReceiveHandler.Invoke(Encoding.Unicode.GetBytes("t#"));
            tcpCommunicatorMock.ReceiveHandler.Invoke(Encoding.Unicode.GetBytes("#"));
            Assert.IsTrue(receivedMessages[0].RawMessage.ToString() == "Test");
        }
    }
}
