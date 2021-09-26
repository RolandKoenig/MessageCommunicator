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
        public void WmCopyDataBased_DefaultCase()
        {
            var controlName = Guid.NewGuid().ToString();
            using var srvSingleInstance1 = new WmCopyDataSingleApplicationInstanceService(controlName);
            using var srvSingleInstance2 = new WmCopyDataSingleApplicationInstanceService(controlName);

            Assert.IsTrue(srvSingleInstance1.IsMainInstance, nameof(srvSingleInstance1.IsMainInstance));
            Assert.IsFalse(srvSingleInstance2.IsMainInstance, nameof(srvSingleInstance2.IsMainInstance));
        }

        [TestMethod]
        public void WmCopyDataBased_Dispose()
        {
            var controlName = Guid.NewGuid().ToString();
            using (var srvSingleInstance1 = new WmCopyDataSingleApplicationInstanceService(controlName))
            {
                Assert.IsTrue(srvSingleInstance1.IsMainInstance, nameof(srvSingleInstance1.IsMainInstance));
            }

            using (var srvSingleInstance2 = new WmCopyDataSingleApplicationInstanceService(controlName))
            {
                Assert.IsTrue(srvSingleInstance2.IsMainInstance, nameof(srvSingleInstance2.IsMainInstance));
            }
        }

        [TestMethod]
        public void WmCopyDataBased_MessageSending()
        {
            var controlName = Guid.NewGuid().ToString();
            using var srvSingleInstance1 = new WmCopyDataSingleApplicationInstanceService(controlName);
            using var srvSingleInstance2 = new WmCopyDataSingleApplicationInstanceService(controlName);

            Assert.IsTrue(srvSingleInstance1.IsMainInstance, nameof(srvSingleInstance1.IsMainInstance));
            Assert.IsFalse(srvSingleInstance2.IsMainInstance, nameof(srvSingleInstance2.IsMainInstance));

            Assert.IsTrue(srvSingleInstance1.CanSendReceiveMessages, nameof(srvSingleInstance1.CanSendReceiveMessages));
            Assert.IsTrue(srvSingleInstance2.CanSendReceiveMessages, nameof(srvSingleInstance2.CanSendReceiveMessages));

            var receivedMessage = "";
            srvSingleInstance1.MessageReceived += (_, eArgs) =>
            {
                receivedMessage = eArgs.Message;
            };
            var messageSent = srvSingleInstance2.TrySendMessageToMainInstance("DummyMessage");

            Assert.IsTrue(messageSent, nameof(messageSent));
            Assert.AreEqual("DummyMessage", receivedMessage, nameof(receivedMessage));
        }
    }
}
