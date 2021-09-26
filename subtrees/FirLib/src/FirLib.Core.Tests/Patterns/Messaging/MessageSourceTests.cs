using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirLib.Core;
using FirLib.Core.Patterns.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FirLib.Tests.Core.Patterns.Messaging
{
    [TestClass]
    public class MessageSourceTests
    {
        [TestMethod]
        public void MessageSourceWithCustomTarget_Class()
        {
            // Prepare
            var customHandlerCalled = false;
            var messageSource = new FirLibMessageSource<TestMessageClass>(FirLibConstants.MESSENGER_NAME_GUI);
            messageSource.UnitTesting_ReplaceByCustomMessageTarget(
                _ => customHandlerCalled = true);

            // Execute test
            messageSource.Publish(new TestMessageClass("Testing argument"));

            // Check results
            Assert.IsTrue(customHandlerCalled);
        }

        [TestMethod]
        public void MessageSourceWithRealTarget_Class()
        {
            // Prepare
            var dummyMessenger = new FirLibMessenger();
            dummyMessenger.ConnectToGlobalMessaging(
                FirLibMessengerThreadingBehavior.Ignore,
                "DummyMessenger",
                null);
            try
            {
                var realHandlerCalled = false;
                dummyMessenger.Subscribe<TestMessageClass>(_ => realHandlerCalled = true);

                // Execute test
                var messageSource = new FirLibMessageSource<TestMessageClass>("DummyMessenger");
                messageSource.Publish(new TestMessageClass("Testing argument"));

                // Check results
                Assert.IsTrue(realHandlerCalled);
            }
            finally
            {
                // Cleanup
                dummyMessenger.DisconnectFromGlobalMessaging();
            }
        }

        [TestMethod]
        public void MessageSourceWithCustomTarget_Struct()
        {
            // Prepare
            var customHandlerCalled = false;
            var messageSource = new FirLibMessageSource<TestMessageStruct>(FirLibConstants.MESSENGER_NAME_GUI);
            messageSource.UnitTesting_ReplaceByCustomMessageTarget(
                _ => customHandlerCalled = true);

            // Execute test
            messageSource.Publish(new TestMessageStruct("Testing argument"));

            // Check results
            Assert.IsTrue(customHandlerCalled);
        }

        [TestMethod]
        public void MessageSourceWithRealTarget_Struct()
        {
            // Prepare
            var dummyMessenger = new FirLibMessenger();
            dummyMessenger.ConnectToGlobalMessaging(
                FirLibMessengerThreadingBehavior.Ignore,
                "DummyMessenger",
                null);
            try
            {
                var realHandlerCalled = false;
                dummyMessenger.Subscribe<TestMessageStruct>(_ => realHandlerCalled = true);

                // Execute test
                var messageSource = new FirLibMessageSource<TestMessageStruct>("DummyMessenger");
                messageSource.Publish(new TestMessageStruct("Testing argument"));

                // Check results
                Assert.IsTrue(realHandlerCalled);
            }
            finally
            {
                // Cleanup
                dummyMessenger.DisconnectFromGlobalMessaging();
            }
        }

#if NET5_0_OR_GREATER
        [TestMethod]
        public void MessageSourceWithCustomTarget_Record()
        {
            // Prepare
            var customHandlerCalled = false;
            var messageSource = new FirLibMessageSource<TestMessageRecord>(FirLibConstants.MESSENGER_NAME_GUI);
            messageSource.UnitTesting_ReplaceByCustomMessageTarget(
                _ => customHandlerCalled = true);

            // Execute test
            messageSource.Publish(new TestMessageRecord("Testing argument"));

            // Check results
            Assert.IsTrue(customHandlerCalled);
        }

        [TestMethod]
        public void MessageSourceWithRealTarget_Record()
        {
            // Prepare
            var dummyMessenger = new FirLibMessenger();
            dummyMessenger.ConnectToGlobalMessaging(
                FirLibMessengerThreadingBehavior.Ignore,
                "DummyMessenger",
                null);
            try
            {
                var realHandlerCalled = false;
                dummyMessenger.Subscribe<TestMessageRecord>(_ => realHandlerCalled = true);

                // Execute test
                var messageSource = new FirLibMessageSource<TestMessageRecord>("DummyMessenger");
                messageSource.Publish(new TestMessageRecord("Testing argument"));

                // Check results
                Assert.IsTrue(realHandlerCalled);
            }
            finally
            {
                // Cleanup
                dummyMessenger.DisconnectFromGlobalMessaging();
            }
        }
#endif
    }
}
