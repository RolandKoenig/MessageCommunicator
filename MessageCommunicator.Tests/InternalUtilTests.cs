using System;
using System.Collections.Generic;
using System.Text;
using MessageCommunicator.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MessageCommunicator.Tests
{
    [TestClass]
    public class InternalUtilTests
    {
        [TestMethod]
        public void Check_ParseIntFromStringPart()
        {
            Assert.IsTrue(1 == TcpCommunicatorUtil.ParseInt32FromStringPart(new StringBuffer("<1|...>"), 1, 1));
            Assert.IsTrue(12 == TcpCommunicatorUtil.ParseInt32FromStringPart(new StringBuffer("<12|...>"), 1, 2));
            Assert.IsTrue(123 == TcpCommunicatorUtil.ParseInt32FromStringPart(new StringBuffer("<123|...>"), 1, 3));
        }
    
    }
}
