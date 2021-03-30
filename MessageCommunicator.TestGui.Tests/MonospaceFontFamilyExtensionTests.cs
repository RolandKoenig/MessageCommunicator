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
            Assert.IsNotNull(MonospaceFontFamilyExtension.TryParseFontFamily("Courier New"));
        }

        [TestMethod]
        public void CheckAvailabilityForInvalidFontFamily()
        {
            Assert.IsNull(MonospaceFontFamilyExtension.TryParseFontFamily("This family is not available"));
        }
    }
}
