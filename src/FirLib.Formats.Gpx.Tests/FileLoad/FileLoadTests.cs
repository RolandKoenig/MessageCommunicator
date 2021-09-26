using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FirLib.Core.Utils.IO.AssemblyResources;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FirLib.Formats.Gpx.Tests.FileLoad
{
    [TestClass]
    public class FileLoadTests
    {
        [TestMethod]
        public void GpxVersion1_1_CompatibilityMode()
        {
            var resLink = new AssemblyResourceLink(
                typeof(FileLoadTests),
                "Test_Gpx1_1.gpx");
            using var inStream = resLink.OpenRead();

            var gpxFile = GpxFile.Deserialize(inStream, GpxFileDeserializationMethod.Compatibility);

            Assert.IsNotNull(gpxFile);
            Assert.IsNotNull(gpxFile.Metadata);
            Assert.AreEqual("Kösseine", gpxFile!.Metadata!.Name);
            Assert.AreEqual(1, gpxFile.Tracks.Count);
        }

        [TestMethod]
        public void GpxVersion1_1_Gpx1_1Mode()
        {
            var resLink = new AssemblyResourceLink(
                typeof(FileLoadTests),
                "Test_Gpx1_1.gpx");
            using var inStream = resLink.OpenRead();

            var gpxFile = GpxFile.Deserialize(inStream, GpxFileDeserializationMethod.OnlyGpx1_1);

            Assert.IsNotNull(gpxFile);
            Assert.IsNotNull(gpxFile.Metadata);
            Assert.AreEqual("Kösseine", gpxFile!.Metadata!.Name);
            Assert.AreEqual(1, gpxFile.Tracks.Count);
        }

        [TestMethod]
        public void GpxVersion1_1_on_xml_1_1()
        {
            var resLink = new AssemblyResourceLink(
                typeof(FileLoadTests),
                "Test_Gpx1_1_on_xml_1_1.gpx");
            using var inStream = resLink.OpenRead();

            var gpxFile = GpxFile.Deserialize(inStream, GpxFileDeserializationMethod.Compatibility);

            Assert.IsNotNull(gpxFile);
            Assert.IsNotNull(gpxFile.Metadata);
            Assert.AreEqual("Kösseine", gpxFile!.Metadata!.Name);
            Assert.AreEqual(1, gpxFile.Tracks.Count);
        }

        [TestMethod]
        public void GpxVersion1_0()
        {
            var resLink = new AssemblyResourceLink(
                typeof(FileLoadTests),
                "Test_Gpx1_0.gpx");
            using var inStream = resLink.OpenRead();

            var gpxFile = GpxFile.Deserialize(inStream, GpxFileDeserializationMethod.Compatibility);

            Assert.IsNotNull(gpxFile);
            Assert.IsNotNull(gpxFile.Metadata);
            Assert.AreEqual("Kösseine", gpxFile!.Metadata!.Name);
            Assert.AreEqual(1, gpxFile.Tracks.Count);
        }

        [TestMethod]
        public void GpxVersion1_0_SaveAs1_1()
        {            
            var resLink = new AssemblyResourceLink(
                typeof(FileLoadTests),
                "Test_Gpx1_0.gpx");
            using var inStream = resLink.OpenRead();

            var gpxFile = GpxFile.Deserialize(inStream, GpxFileDeserializationMethod.Compatibility);
            var outStrBuilder = new StringBuilder(33000);
            using (var strWriter = new StringWriter(outStrBuilder))
            {
                GpxFile.Serialize(gpxFile, strWriter);
            }
            var writtenFile = outStrBuilder.ToString();

            // Check output
            Assert.IsTrue(writtenFile.Contains("version=\"1.1\""), "Version attribute");
            Assert.IsTrue(writtenFile.Contains("xmlns=\"http://www.topografix.com/GPX/1/1\""), "Default namespace");

            // Check original data
            Assert.AreEqual("1.0", gpxFile.Version, "Original version");
        }
    }
}
