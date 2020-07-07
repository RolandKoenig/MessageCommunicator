using Microsoft.VisualStudio.TestTools.UnitTesting;
using TcpCommunicator.Util;

namespace TcpCommunicator.Tests
{
    [TestClass]
    public class StringBufferTests
    {
        [TestMethod]
        public void Test_SomeCases()
        {
            var test1 = StringBuffer.Format(
                "Error while connecting to host {0} on port {1}: {2}",
                "127.0.0.1", 12000,
                "Es konnte keine Verbindung hergestellt werden, da der Zielcomputer die Verbindung verweigerte. [::ffff:127.0.0.1]:12000");

        }
    }
}
