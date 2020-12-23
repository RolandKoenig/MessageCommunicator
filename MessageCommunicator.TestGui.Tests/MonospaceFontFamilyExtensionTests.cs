using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MessageCommunicator.TestGui.Tests
{
    [TestClass]
    public class MonospaceFontFamilyExtensionTests
    {
        [TestMethod]
        public void CheckAvailabilityForAvailableFontFamily()
        {
            Assert.IsTrue(MonospaceFontFamilyExtension.IsFontFamilyAvailable("Courier New"));
        }

        [TestMethod]
        public void CheckAvailabilityForInvalidFontFamily()
        {
            Assert.IsTrue(MonospaceFontFamilyExtension.IsFontFamilyAvailable("This family is not available"));
        }
    }
}
