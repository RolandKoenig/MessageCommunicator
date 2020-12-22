using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MessageCommunicator.TestGui.Tests
{
    [TestClass]
    public class IntegratedDocUtilTests
    {
        [TestMethod]
        public void GetIntegratedDocFile_EmbeddedResourceAsString()
        {
            var testObject = new IntegratedDocUtil();
            testObject.AddQueryAssembly(Assembly.GetExecutingAssembly());

            var testDoc = testObject.GetIntegratedDoc("TestDoc.md");

            Assert.AreEqual("# Testing", testDoc);
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void GetIntegratedDocFile_FileNotFound_UnknownFile()
        {
            var testObject = new IntegratedDocUtil();
            testObject.AddQueryAssembly(Assembly.GetExecutingAssembly());

            testObject.GetIntegratedDoc("DoesNotExist.md");
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void GetIntegratedDocFile_FileNotFound_AssemblyNotAdded()
        {
            var testObject = new IntegratedDocUtil();
            
            // Following call is missing
            // testObject.AddQueryAssembly(Assembly.GetExecutingAssembly());

            testObject.GetIntegratedDoc("TestDoc.md");
        }
    }
}
