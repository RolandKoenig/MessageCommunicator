using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MessageCommunicator.Tests
{
    [TestClass]
    public class DefaultMessageFormattingTests
    {
        private Encoding[] _encodings = Encoding.GetEncodings()
            .Select(actEncodingInfo => actEncodingInfo.GetEncoding())
            .ToArray();

        [TestMethod]
        [DataRow(data1: "This is a dummy message", "<23|This is a dummy message>")]
        [DataRow(data1: "a", "<1|a>")]
        [DataRow(data1: "\r\n", "<2|\r\n>")]
        public async Task Check_DefaultMessageRecognizer(string sendMessage, string expectedMessage)
        {
            foreach (var actEncoding in _encodings)
            {
                var testObject = new DefaultMessageRecognizer(actEncoding);
                await GenericTestMethodAsync(testObject, actEncoding, sendMessage, expectedMessage);
            }
        }

        [TestMethod]
        [DataRow("##", "This is a dummy message", "This is a dummy message##")]
        [DataRow("\x03", "This is a dummy message", "This is a dummy message\x03")]
        public async Task Check_EndSymbolMessageRecognizer(string endSymbols, string sendMessage, string expectedMessage)
        {
            foreach (var actEncoding in _encodings)
            {
                var testObject = new EndSymbolsMessageRecognizer(actEncoding, endSymbols);
                await GenericTestMethodAsync(testObject, actEncoding, sendMessage, expectedMessage);
            }
        }

        [TestMethod]
        [DataRow("##", 30, '.', "This is a dummy message", "This is a dummy message.....##")]
        [DataRow("\x03", 30, '.', "This is a dummy message", "This is a dummy message......\x03")]
        public async Task Check_FixedLengthWithEndSymbolMessageRecognizer(string endSymbols, int lengthIncludingEndSymbols, char fillSymbol, string sendMessage, string expectedMessage)
        {
            foreach (var actEncoding in _encodings)
            {
                var testObject = new FixedLengthAndEndSymbolsMessageRecognizer(actEncoding, endSymbols, lengthIncludingEndSymbols, fillSymbol);
                await GenericTestMethodAsync(testObject, actEncoding, sendMessage, expectedMessage);
            }
        }

        private static async Task GenericTestMethodAsync(MessageRecognizer testObject, Encoding testEncoding, string sendMessage, string expectedMessage)
        {
            // Prepare test
            var generatedMessage = "";
            var fakedByteStreamHandler = A.Fake<IByteStreamHandler>();
            A.CallTo(fakedByteStreamHandler)
                .WithReturnType<Task<bool>>()
                .Where(info => info.Method.Name == nameof(IByteStreamHandler.SendAsync))
                .Invokes(args =>
                {
                    var bytesToSend = (ReadOnlyMemory<byte>) args.Arguments[0];
                    generatedMessage = testEncoding.GetString(bytesToSend.Span);
                })
                .Returns(Task.FromResult(true));
            testObject.ByteStreamHandler = fakedByteStreamHandler;

            // Call test method
            var sendResult = await testObject.SendAsync(sendMessage);

            // Check results
            Assert.IsTrue(sendResult, "Send returned false");
            Assert.IsTrue(generatedMessage == expectedMessage, "Wrong message formatting");
        }
    }
}
