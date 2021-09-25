using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MessageCommunicator.TestGui.Tests
{
    [TestClass]
    public class IntegratedDocRepositoryTests
    {
        [TestMethod]
        public void ParseCommonInfoFromMDFile_Valid()
        {
            var testFile =
                "# TestTitle" + Environment.NewLine +
                "test test test test test" + Environment.NewLine +
                "test test test test test";

            var integratedDocFile = IntegratedDocRepository.ProcessMDFile(() => new StringReader(testFile));
            
            Assert.IsNotNull(integratedDocFile);
            Assert.AreEqual("TestTitle", integratedDocFile!.Title);
        }
        
        [TestMethod]
        public void ParseCommonInfoFromMDFile_Valid_WithSpaces()
        {
            var testFile =
                "# Test Title 2" + Environment.NewLine +
                "test test test test test" + Environment.NewLine +
                "test test test test test";

            var integratedDocFile = IntegratedDocRepository.ProcessMDFile(() => new StringReader(testFile));
            
            Assert.IsNotNull(integratedDocFile);
            Assert.AreEqual("Test Title 2", integratedDocFile!.Title);
        }
        
        [TestMethod]
        public void ParseCommonInfoFromMDFile_Invalid_FirstLineEmpty()
        {
            var testFile =
                "" + Environment.NewLine +
                "# Test Title 2" + Environment.NewLine +
                "test test test test test" + Environment.NewLine +
                "test test test test test";

            var integratedDocFile = IntegratedDocRepository.ProcessMDFile(() => new StringReader(testFile));
            
            Assert.IsNull(integratedDocFile);
        }
        
        [TestMethod]
        public void ParseCommonInfoFromMDFile_Invalid_EmptyFile()
        {
            var integratedDocFile = IntegratedDocRepository.ProcessMDFile(() => new StringReader(string.Empty));
            
            Assert.IsNull(integratedDocFile);
        }
    }
}