using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageCommunicator.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MessageCommunicator.Tests
{
    [TestClass]
    public class HexFormatUtilTests
    {
        [TestMethod]
        public void BytesToHexString()
        {
            var testArray = new byte[] {1, 2, 3, 16, 17, 255};

            var hexString = HexFormatUtil.ToHexString(testArray);

            Assert.IsTrue(hexString == "01 02 03 10 11 FF");
        }

        [TestMethod]
        public void HexStringToBytes()
        {
            var hexString = "01 02 03 10 11 FF";

            var byteArray = HexFormatUtil.ToByteArray(hexString);

            Assert.IsTrue(byteArray.Length == 6);
            Assert.IsTrue(byteArray[0] == 1);
            Assert.IsTrue(byteArray[1] == 2);
            Assert.IsTrue(byteArray[2] == 3);
            Assert.IsTrue(byteArray[3] == 16);
            Assert.IsTrue(byteArray[4] == 17);
            Assert.IsTrue(byteArray[5] == 255);
        }

        [TestMethod]
        public void HexStringToBytes_WithoutSpaces()
        {
            var hexString = "0102031011FF";

            var byteArray = HexFormatUtil.ToByteArray(hexString);

            Assert.IsTrue(byteArray.Length == 6);
            Assert.IsTrue(byteArray[0] == 1);
            Assert.IsTrue(byteArray[1] == 2);
            Assert.IsTrue(byteArray[2] == 3);
            Assert.IsTrue(byteArray[3] == 16);
            Assert.IsTrue(byteArray[4] == 17);
            Assert.IsTrue(byteArray[5] == 255);
        }

        [TestMethod]
        public void HexStringToBytes_MixedFormat()
        {
            var hexString = "  010203 1  011fF  ";

            var byteArray = HexFormatUtil.ToByteArray(hexString);

            Assert.IsTrue(byteArray.Length == 6);
            Assert.IsTrue(byteArray[0] == 1);
            Assert.IsTrue(byteArray[1] == 2);
            Assert.IsTrue(byteArray[2] == 3);
            Assert.IsTrue(byteArray[3] == 16);
            Assert.IsTrue(byteArray[4] == 17);
            Assert.IsTrue(byteArray[5] == 255);
        }

        [TestMethod]
        public void HexStringToBytes_EmptyString()
        {
            var hexString = string.Empty;

            var byteArray = HexFormatUtil.ToByteArray(hexString);

            Assert.IsTrue(byteArray.Length == 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void HexStringToBytes_NullString()
        {
            HexFormatUtil.ToByteArray(null!);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void HexStringToBytes_InvalidCharCount()
        {
            HexFormatUtil.ToByteArray("01 0");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void HexStringToBytes_InvalidCharacter()
        {
            HexFormatUtil.ToByteArray("t");
        }
    }
}
