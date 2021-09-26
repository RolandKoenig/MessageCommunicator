using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirLib.Core.Patterns.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FirLib.Tests.Core.Patterns.Messaging
{
    [TestClass]
    public class MessageValidationTests
    {
        [TestMethod]
        public void GoodCase_MessageClass()
        {
            var msg = new TestMessageClass("TestArg");

            var resultType = FirLibMessageHelper.ValidateMessageType(msg.GetType(), out _);
            var resultTypeAndValue = FirLibMessageHelper.ValidateMessageTypeAndValue(msg, out _);

            Assert.IsTrue(resultType);
            Assert.IsTrue(resultTypeAndValue);
        }

        [TestMethod]
        public void GoodCase_MessageStruct()
        {
            var msg = new TestMessageStruct("TestArg");

            var resultType = FirLibMessageHelper.ValidateMessageType(msg.GetType(), out _);
            var resultTypeAndValue = FirLibMessageHelper.ValidateMessageTypeAndValue(msg, out _);

            Assert.IsTrue(resultType);
            Assert.IsTrue(resultTypeAndValue);
        }

#if NET5_0_OR_GREATER
        [TestMethod]
        public void GoodCase_MessageRecord()
        {
            var msg = new TestMessageRecord("TestArg");

            var resultType = FirLibMessageHelper.ValidateMessageType(msg.GetType(), out _);
            var resultTypeAndValue = FirLibMessageHelper.ValidateMessageTypeAndValue(msg, out _);

            Assert.IsTrue(resultType);
            Assert.IsTrue(resultTypeAndValue);
        }
#endif

        [TestMethod]
        public void BadCase_MessageClass_NoAttributeOnClass()
        {
            var msg = new TestMessageClass_WithoutAttribute("TestArg");

            var resultType = FirLibMessageHelper.ValidateMessageType(msg.GetType(), out _);
            var resultTypeAndValue = FirLibMessageHelper.ValidateMessageTypeAndValue(msg, out _);

            Assert.IsFalse(resultType);
            Assert.IsFalse(resultTypeAndValue);
        }

        [TestMethod]
        public void GoodCase_MessageStruct_NoAttributeOnStruct()
        {
            var msg = new TestMessageStruct_WithoutAttribute("TestArg");

            var resultType = FirLibMessageHelper.ValidateMessageType(msg.GetType(), out _);
            var resultTypeAndValue = FirLibMessageHelper.ValidateMessageTypeAndValue(msg, out _);

            Assert.IsFalse(resultType);
            Assert.IsFalse(resultTypeAndValue);
        }

#if NET5_0_OR_GREATER
        [TestMethod]
        public void BadCase_MessageRecord_NoAttributeOnRecord()
        {
            var msg = new TestMessageRecord_WithoutAttribute("TestArg");

            var resultType = FirLibMessageHelper.ValidateMessageType(msg.GetType(), out _);
            var resultTypeAndValue = FirLibMessageHelper.ValidateMessageTypeAndValue(msg, out _);

            Assert.IsFalse(resultType);
            Assert.IsFalse(resultTypeAndValue);
        }
#endif

        [TestMethod]
        public void BadCase_MessageIsNull()
        {
            var resultTypeAndValue = FirLibMessageHelper.ValidateMessageTypeAndValue<TestMessageClass>(null!, out _);
            Assert.IsFalse(resultTypeAndValue);
        }

        [TestMethod]
        public void BadCase_InvalidMessageType()
        {
            var message = "Test Message";

            var resultType = FirLibMessageHelper.ValidateMessageType(message.GetType(), out _);
            var resultTypeAndValue = FirLibMessageHelper.ValidateMessageTypeAndValue(message, out _);

            Assert.IsFalse(resultType);
            Assert.IsFalse(resultTypeAndValue);
        }
    }
}
