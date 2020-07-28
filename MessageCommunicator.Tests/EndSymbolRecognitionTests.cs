using System;
using System.Collections.Generic;
using System.Text;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MessageCommunicator.Tests
{
    [TestClass]
    public class EndSymbolRecognitionTests
    {
        //[TestMethod]
        //public void Test_EndSymbols_StandardMessage()
        //{
        //    var tcpCommunicatorMock = A.Fake<IByteStreamHandler>();
            
        //    var messageRecognizer = new FixedLengthAndEndSymbolMessageRecognizer(
        //        tcpCommunicatorMock,
        //        Encoding.UTF8, new char[]{'#', '#'});

        //    Message? currentMessage = null;
        //    messageRecognizer.ReceiveHandler = (msg) => currentMessage = msg;

        //    tcpCommunicatorMock.ReceiveHandler.Invoke(Encoding.UTF8.GetBytes("Test##"));
        //    Assert.IsTrue(currentMessage.RawMessage.ToString() == "Test");
        //}

        //[TestMethod]
        //public void Test_EndSymbols_TwoStandardMessages()
        //{
        //    var tcpCommunicatorMock = A.Fake<IByteStreamHandler>();
            
        //    var messageRecognizer = new FixedLengthAndEndSymbolMessageRecognizer(
        //        tcpCommunicatorMock,
        //        Encoding.UTF8, new char[]{'#', '#'});

        //    var receivedMessages = new List<Message>(2);
        //    messageRecognizer.ReceiveHandler = (msg) => receivedMessages.Add(msg);

        //    tcpCommunicatorMock.ReceiveHandler.Invoke(Encoding.UTF8.GetBytes("Test1##Test2##"));
        //    Assert.IsTrue(receivedMessages[0].RawMessage.ToString() == "Test1");
        //    Assert.IsTrue(receivedMessages[1].RawMessage.ToString() == "Test2");
        //}

        //[TestMethod]
        //public void Test_EndSymbols_SpecialCase_TwoParts()
        //{
        //    var tcpCommunicatorMock = A.Fake<IByteStreamHandler>();
            
        //    var messageRecognizer = new FixedLengthAndEndSymbolMessageRecognizer(
        //        tcpCommunicatorMock,
        //        Encoding.UTF8, new char[]{'#', '#'});

        //    var receivedMessages = new List<Message>(2);
        //    messageRecognizer.ReceiveHandler = (msg) => receivedMessages.Add(msg);

        //    tcpCommunicatorMock.ReceiveHandler.Invoke(Encoding.UTF8.GetBytes("Tes"));
        //    tcpCommunicatorMock.ReceiveHandler.Invoke(Encoding.UTF8.GetBytes("t##"));
        //    Assert.IsTrue(receivedMessages[0].RawMessage.ToString() == "Test");
        //}

        //[TestMethod]
        //public void Test_EndSymbols_SpecialCase_CuttedEndSymbol()
        //{
        //    var tcpCommunicatorMock = A.Fake<IByteStreamHandler>();
            
        //    var messageRecognizer = new FixedLengthAndEndSymbolMessageRecognizer(
        //        tcpCommunicatorMock,
        //        Encoding.UTF8, new char[]{'#', '#'});

        //    var receivedMessages = new List<Message>(2);
        //    messageRecognizer.ReceiveHandler = (msg) => receivedMessages.Add(msg);

        //    tcpCommunicatorMock.ReceiveHandler.Invoke(Encoding.UTF8.GetBytes("Tes"));
        //    tcpCommunicatorMock.ReceiveHandler.Invoke(Encoding.UTF8.GetBytes("t#"));
        //    tcpCommunicatorMock.ReceiveHandler.Invoke(Encoding.UTF8.GetBytes("#"));
        //    Assert.IsTrue(receivedMessages[0].RawMessage.ToString() == "Test");
        //}
    }
}
