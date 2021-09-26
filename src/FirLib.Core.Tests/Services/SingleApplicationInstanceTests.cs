using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirLib.Core.Services.SingleApplicationInstance;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FirLib.Core.Tests.Services
{
    [TestClass]
    public class SingleApplicationInstanceTests
    {
        [TestMethod]
        public void MutexBased_DefaultCase()
        {
            var mutexName = Guid.NewGuid().ToString();
            using var srvSingleInstance1 = new MutexBasedSingleApplicationInstanceService(mutexName);
            using var srvSingleInstance2 = new MutexBasedSingleApplicationInstanceService(mutexName);
         
            Assert.IsTrue(
                srvSingleInstance1.IsMainInstance,
                nameof(srvSingleInstance1.IsMainInstance));
            Assert.IsFalse(
                srvSingleInstance2.IsMainInstance,
                nameof(srvSingleInstance2.IsMainInstance));
        }

        [TestMethod]
        public void MutexBased_Dispose()
        {
            var mutexName = Guid.NewGuid().ToString();
            using (var srvSingleInstance1 = new MutexBasedSingleApplicationInstanceService(mutexName))
            {
                Assert.IsTrue(
                    srvSingleInstance1.IsMainInstance,
                    nameof(srvSingleInstance1.IsMainInstance));
            }

            using (var srvSingleInstance2 = new MutexBasedSingleApplicationInstanceService(mutexName))
            {
                Assert.IsTrue(
                    srvSingleInstance2.IsMainInstance,
                    nameof(srvSingleInstance2.IsMainInstance));
            }
        }

        [TestMethod]
        public void MutexBased_MessagesNotSupported()
        {
            var mutexName = Guid.NewGuid().ToString();
            using var srvSingleInstance = new MutexBasedSingleApplicationInstanceService(mutexName);

            Assert.IsTrue(
                srvSingleInstance.IsMainInstance, 
                nameof(srvSingleInstance.IsMainInstance));
            Assert.IsFalse(
                srvSingleInstance.CanSendReceiveMessages, 
                nameof(srvSingleInstance.CanSendReceiveMessages));
            Assert.IsFalse(
                srvSingleInstance.TrySendMessageToMainInstance("DummyMessage"),
                nameof(srvSingleInstance.TrySendMessageToMainInstance));
        }
    }
}
