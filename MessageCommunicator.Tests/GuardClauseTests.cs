using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Light.GuardClauses.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#nullable disable

namespace MessageCommunicator.Tests
{
    [TestClass]
    public class GuardClauseTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Check_DefaultMessageRecognizerSettings_EncodingIsNull()
        {
            _ = new DefaultMessageRecognizerSettings(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Check_DefaultMessageRecognizer_EncodingIsNull()
        {
            _ = new DefaultMessageRecognizer(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentDefaultException))]
        public void Check_DefaultMessageRecognizer_ReceivedBytesAreDefault()
        {
            var newMsgRecognizer = new DefaultMessageRecognizer(Encoding.UTF8);
            newMsgRecognizer.OnReceivedBytes(true, default(ArraySegment<byte>));
        }
    }
}
