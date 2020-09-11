using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MessageCommunicator.Tests
{
    [TestClass]
    public class MessageRecognitionTests
    {
        private Encoding[] _encodings = Encoding.GetEncodings()
            .Select(actEncodingInfo => actEncodingInfo.GetEncoding())
            .ToArray();

        [TestMethod]
        [DataRow("<22|This is a test message>", "This is a test message")]
        [DataRow("<1|a>", "a")]
        [DataRow("<0|>", "")]
        public void Check_DefaultMessageRecognizer(string receivedData, string expectedMessage)
        {
            foreach (var actEncoding in _encodings)
            {
                // Test full receiving
                GenericTestMethod_FullReceive(
                    new DefaultMessageRecognizer(actEncoding),
                    actEncoding, receivedData, expectedMessage);

                // Test receiving of only single bytes
                GenericTestMethod_SingleBytesReceive(
                    new DefaultMessageRecognizer(actEncoding),
                    actEncoding, receivedData, expectedMessage);
            }
        }

        private static void GenericTestMethod_FullReceive(MessageRecognizer testObject, Encoding encoding, string receivedData, string expectedMessage)
        {
            Message? recognizedMessage = null;

            var fakeReceiveHandler = A.Fake<IMessageReceiveHandler>();
            A.CallTo(fakeReceiveHandler)
                .Where(actCall => actCall.Method.Name == nameof(fakeReceiveHandler.OnMessageReceived))
                .Invokes(actCall =>
                {
                    recognizedMessage = (Message)actCall.Arguments[0]!;
                });

            testObject.ReceiveHandler = fakeReceiveHandler;
            testObject.OnReceivedBytes(true, encoding.GetBytes(receivedData));

            Assert.IsNotNull(recognizedMessage, "No message recognized!");
            Assert.IsTrue(recognizedMessage!.ToString() == expectedMessage);
        }

        private static void GenericTestMethod_SingleBytesReceive(MessageRecognizer testObject, Encoding encoding, string receivedData, string expectedMessage)
        {
            Message? recognizedMessage = null;

            var fakeReceiveHandler = A.Fake<IMessageReceiveHandler>();
            A.CallTo(fakeReceiveHandler)
                .Where(actCall => actCall.Method.Name == nameof(fakeReceiveHandler.OnMessageReceived))
                .Invokes(actCall =>
                {
                    recognizedMessage = (Message)actCall.Arguments[0]!;
                });

            testObject.ReceiveHandler = fakeReceiveHandler;

            var bytesToReceive = encoding.GetBytes(receivedData);
            for (var loop = 0; loop < bytesToReceive.Length; loop++)
            {
                testObject.OnReceivedBytes(loop == 0, bytesToReceive.AsSpan(loop, 1));
            }

            Assert.IsNotNull(recognizedMessage, "No message recognized!");
            Assert.IsTrue(recognizedMessage!.ToString() == expectedMessage);
        }
    }
}
