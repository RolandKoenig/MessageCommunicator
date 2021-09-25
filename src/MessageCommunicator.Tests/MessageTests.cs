﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MessageCommunicator.Tests
{
    [TestClass]
    public class MessageTests
    {
        [TestMethod]
        public void CheckReturnToPoolAndRent()
        {
            try
            {
                var dummyMessage = new Message("Message for testing");
                Assert.IsFalse(dummyMessage.IsMessagePooled);

                dummyMessage.ReturnToPool();
                Assert.IsTrue(dummyMessage.IsMessagePooled);

                var dummyMessage2 = MessagePool.Rent(10);
                Assert.IsFalse(dummyMessage2.IsMessagePooled);
            }
            finally
            {
                // Cleanup
                MessagePool.Clear();
            }
        }

        [TestMethod]
        public void CheckDuplicateReturnToPool()
        {
            try
            {
                var dummyMessage = new Message("Message for testing");

                dummyMessage.ReturnToPool();

                InvalidOperationException? expectedException = null;
                try
                {
                    dummyMessage.ReturnToPool();
                }
                catch (InvalidOperationException ex)
                {
                    expectedException = ex;
                }
            
                Assert.IsNotNull(expectedException);
            }
            finally
            {
                // Cleanup
                MessagePool.Clear();
            }
        }

        [TestMethod]
        public void CheckMessagePooledError()
        {
            try
            {
                var dummyMessage = new Message("Message for testing");

                dummyMessage.ReturnToPool();

                InvalidOperationException? expectedException = null;
                try
                {
                    dummyMessage.EnsureCapacity(500);
                }
                catch (InvalidOperationException ex)
                {
                    expectedException = ex;
                }
            
                Assert.IsNotNull(expectedException);
            }
            finally
            {
                // Cleanup
                MessagePool.Clear();
            }
        }

        [TestMethod]
        public void CheckAccessBySpan()
        {
            var testObject = new Message("This is a dummy message");

            var testSpan = testObject.GetSpan(5, 2);
            testSpan[0] = '_';
            testSpan[1] = '_';

            Assert.IsTrue(testObject.ToString() == "This __ a dummy message");
        }

        [TestMethod]
        public void CheckEmptyMessage()
        {
            var testObject = new Message("");

            var testSpan = testObject.GetSpan();
            var testSpanReadOnly = testObject.GetSpanReadOnly();

            Assert.IsTrue(testObject.ToString() == "");
            Assert.IsTrue(testSpan.ToString() == "");
            Assert.IsTrue(testSpanReadOnly.ToString() == "");
        }
    }
}
