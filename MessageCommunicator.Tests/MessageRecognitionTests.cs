using System;
using System.Collections.Generic;
using System.Text;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MessageCommunicator.Tests
{
    [TestClass]
    public class MessageRecognitionTests
    {
        private Encoding[] _encodings =
        {
            Encoding.GetEncoding("utf-32"),
            Encoding.GetEncoding("utf-16"),
            Encoding.GetEncoding("utf-8"),
            Encoding.GetEncoding("utf-7"),
            Encoding.GetEncoding("us-ascii"),
            Encoding.GetEncoding("iso-8859-1")
        };

        [TestMethod]
        [DataRow("Test", "Test")]
        [DataRow("<1|a>", "<1|a>")]
        [DataRow("#", "#")]
        public void Check_ByUnderlyingPackageMessageRecognizer(string receivedData, string expectedMessage)
        {
            foreach (var actEncoding in _encodings)
            {
                // Test full receiving
                GenericTestMethod_FullReceive(
                    new ByUnderlyingPackageMessageRecognizer(actEncoding),
                    actEncoding, receivedData, expectedMessage);
            }
        }

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

        [TestMethod]
        [DataRow("22|This is a test message>")]                 // No start symbol
        [DataRow("<12345678912345|This is a test message>")]    // Length field too long
        [DataRow("<22|This is a test message_")]                // Wrong end symbol symbol
        [DataRow("<22_This is a test message>")]                // Missing separator
        public void Check_DefaultMessageRecognizer_Errors(string receivedData)
        {
            foreach (var actEncoding in _encodings)
            {
                // Test full receiving
                MessageRecognitionException? catchedException = null;
                try
                {
                    GenericTestMethod_FullReceive(
                        new DefaultMessageRecognizer(actEncoding),
                        actEncoding, receivedData, "");
                }
                catch (MessageRecognitionException e)
                {
                    catchedException = e;
                }
                Assert.IsNotNull(catchedException,
                    $"No exception at encoding {actEncoding.EncodingName} on full receive!");

                // Test receiving of only single bytes
                catchedException = null;
                try
                {
                    GenericTestMethod_SingleBytesReceive(
                        new DefaultMessageRecognizer(actEncoding),
                        actEncoding, receivedData, "");
                }
                catch (MessageRecognitionException e)
                {
                    catchedException = e;
                }
                Assert.IsNotNull(catchedException,
                    $"No exception at encoding {actEncoding.EncodingName} on single bytes receive!");
            }
        }

        [TestMethod]
        [DataRow("##", "This is a test message##", "This is a test message")]
        [DataRow("##", "This is # a test message##", "This is # a test message")]
        [DataRow("#", "This is a test message#", "This is a test message")]
        [DataRow("#C", "This is a test message#C", "This is a test message")]
        [DataRow("\x03", "This is a test message\x03", "This is a test message")]
        [DataRow("##", "a##", "a")]
        [DataRow("##", "##", "")]
        public void Check_EndSymbolMessageRecognizer(string endSymbols, string receivedData, string expectedMessage)
        {
            foreach (var actEncoding in _encodings)
            {
                // Test full receiving
                GenericTestMethod_FullReceive(
                    new EndSymbolsMessageRecognizer(actEncoding, endSymbols), 
                    actEncoding, receivedData, expectedMessage);

                // Test receiving of only single bytes
                GenericTestMethod_SingleBytesReceive(
                    new EndSymbolsMessageRecognizer(actEncoding, endSymbols),
                    actEncoding, receivedData, expectedMessage);
            }
        }

        [TestMethod]
        [DataRow(10, '.', "123##...##", "123##...##")]
        [DataRow(10, '.', "12345.....", "12345")]
        [DataRow(10, '.', "..........", "")]
        public void Check_FixedLengthMessageRecognizer(int lengthIncludingEndSymbols, char fillSymbol, string receivedData, string expectedMessage)
        {
            foreach (var actEncoding in _encodings)
            {
                // Test full receiving
                GenericTestMethod_FullReceive(
                    new FixedLengthMessageRecognizer(actEncoding, lengthIncludingEndSymbols, fillSymbol), 
                    actEncoding, receivedData, expectedMessage);

                // Test receiving of only single bytes
                GenericTestMethod_SingleBytesReceive(
                    new FixedLengthMessageRecognizer(actEncoding, lengthIncludingEndSymbols, fillSymbol),
                    actEncoding, receivedData, expectedMessage);
            }
        }

        [TestMethod]
        [DataRow("##", 10, '.', "123##...##", "123##")]
        [DataRow("##", 10, '.', "12345...##", "12345")]
        [DataRow("##", 10, '.', "12345.####", "12345.##")]
        [DataRow("\x03", 10, '.', "12345....\x03", "12345")]
        [DataRow("##", 10, '.', "........##", "")]
        public void Check_FixedLengthAndEndSymbolMessageRecognizer(string endSymbols, int lengthIncludingEndSymbols, char fillSymbol, string receivedData, string expectedMessage)
        {
            foreach (var actEncoding in _encodings)
            {
                // Test full receiving
                GenericTestMethod_FullReceive(
                    new FixedLengthAndEndSymbolsMessageRecognizer(actEncoding, endSymbols, lengthIncludingEndSymbols, fillSymbol), 
                    actEncoding, receivedData, expectedMessage);

                // Test receiving of only single bytes
                GenericTestMethod_SingleBytesReceive(
                    new FixedLengthAndEndSymbolsMessageRecognizer(actEncoding, endSymbols, lengthIncludingEndSymbols, fillSymbol),
                    actEncoding, receivedData, expectedMessage);
            }
        }

        [TestMethod]
        [DataRow("##", 10, '.', "123##...__")]  // Wrong end symbols
        public void Check_FixedLengthAndEndSymbolMessageRecognizer_Errors(string endSymbols, int lengthIncludingEndSymbols, char fillSymbol, string receivedData)
        {
            foreach (var actEncoding in _encodings)
            {
                // Test full receiving
                MessageRecognitionException? catchedException = null;
                try
                {
                    GenericTestMethod_FullReceive(
                        new FixedLengthAndEndSymbolsMessageRecognizer(actEncoding, endSymbols,
                            lengthIncludingEndSymbols, fillSymbol),
                        actEncoding, receivedData, "");
                }
                catch (MessageRecognitionException e)
                {
                    catchedException = e;
                }
                Assert.IsNotNull(catchedException,
                    $"No exception at encoding {actEncoding.EncodingName} on full receive!");

                // Test receiving of only single bytes
                catchedException = null;
                try
                {
                    GenericTestMethod_SingleBytesReceive(
                        new FixedLengthAndEndSymbolsMessageRecognizer(actEncoding, endSymbols,
                            lengthIncludingEndSymbols, fillSymbol),
                        actEncoding, receivedData, "");
                }
                catch (MessageRecognitionException e)
                {
                    catchedException = e;
                }
                Assert.IsNotNull(catchedException,
                    $"No exception at encoding {actEncoding.EncodingName} on single bytes receive!");
            }
        }

        [TestMethod]
        [DataRow("<", ">", "<This is a test message>", "This is a test message")]
        [DataRow("<", ">", "<<This is a test message>", "<This is a test message")]
        [DataRow("<", ">", "<This is a < test message>", "This is a < test message")]
        [DataRow("\x02", "\x03", "\x02This is a test message\x03", "This is a test message")]
        [DataRow("||", "##", "||This is a test message##", "This is a test message")]
        [DataRow("<", ">", "<>", "")]
        [DataRow("<", ">", "<a>", "a")]
        public void Check_StartAndEndSymbolMessageRecognizer(string startSymbols, string endSymbols, string receivedData, string expectedMessage)
        {
            foreach (var actEncoding in _encodings)
            {
                // Test full receiving
                GenericTestMethod_FullReceive(
                    new StartAndEndSymbolsRecognizer(actEncoding, startSymbols,endSymbols), 
                    actEncoding, receivedData, expectedMessage);

                // Test receiving of only single bytes
                GenericTestMethod_SingleBytesReceive(
                    new StartAndEndSymbolsRecognizer(actEncoding, startSymbols,endSymbols),
                    actEncoding, receivedData, expectedMessage);
            }
        }

        [TestMethod]
        [DataRow("<", ">", "_This is a test message>")]   // Invalid start symbol
        public void Check_StartAndEndSymbolMessageRecognizer_Errors(string startSymbols, string endSymbols, string receivedData)
        {
            foreach (var actEncoding in _encodings)
            {
                // Test full receiving
                MessageRecognitionException? catchedException = null;
                try
                {
                    GenericTestMethod_FullReceive(
                        new StartAndEndSymbolsRecognizer(actEncoding, startSymbols, endSymbols),
                        actEncoding, receivedData, "");
                }
                catch (MessageRecognitionException e)
                {
                    catchedException = e;
                }
                Assert.IsNotNull(catchedException,
                    $"No exception at encoding {actEncoding.EncodingName} on full receive!");

                // Test receiving of only single bytes
                catchedException = null;
                try
                {
                    GenericTestMethod_SingleBytesReceive(
                        new StartAndEndSymbolsRecognizer(actEncoding, startSymbols, endSymbols),
                        actEncoding, receivedData, "");
                }
                catch (MessageRecognitionException e)
                {
                    catchedException = e;
                }
                Assert.IsNotNull(catchedException,
                    $"No exception at encoding {actEncoding.EncodingName} on single bytes receive!");
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
            testObject.OnReceivedBytes(true, new ArraySegment<byte>(encoding.GetBytes(receivedData)));

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
                testObject.OnReceivedBytes(loop == 0, new ArraySegment<byte>(
                    bytesToReceive, loop, 1));
            }

            Assert.IsNotNull(recognizedMessage, "No message recognized!");
            Assert.IsTrue(recognizedMessage!.ToString() == expectedMessage);
        }
    }
}
