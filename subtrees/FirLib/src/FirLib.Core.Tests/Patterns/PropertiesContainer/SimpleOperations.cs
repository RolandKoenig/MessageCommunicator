using System;
using FirLib.Core.Patterns.PropertiesContainer;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FirLib.Tests.Core.Patterns.PropertiesContainer;

[TestClass]
public class SimpleOperations
{
    [TestMethod]
    [DataRow("Default")]
    [DataRow("Concurrent")]
    public void Add(string containerType)
    {
        var propContainer = TestUtilities.CreatePropertiesContainer(containerType);

        propContainer.SetProperty("KEY", "TestValue");
        propContainer.SetProperty("KEY2", "TestValue");

        Assert.AreEqual(2, propContainer.CountProperties());
        Assert.AreEqual("TestValue", (string)propContainer.GetProperty("KEY"));
    }

    [TestMethod]
    [DataRow("Default")]
    [DataRow("Concurrent")]
    public void AddEmpty(string containerType)
    {
        var propContainer = TestUtilities.CreatePropertiesContainer(containerType);

        propContainer.SetProperty("KEY", string.Empty);

        Assert.AreEqual(0, propContainer.CountProperties());
        Assert.AreEqual(string.Empty, (string)propContainer.GetProperty("KEY"));
    }

    [TestMethod]
    [DataRow("Default")]
    [DataRow("Concurrent")]
    public void SetToEmpty(string containerType)
    {
        var propContainer = TestUtilities.CreatePropertiesContainer(containerType);

        propContainer.SetProperty("KEY", "TestValue");
        propContainer.SetProperty("KEY", string.Empty);

        Assert.AreEqual(0, propContainer.CountProperties());
        Assert.AreEqual(string.Empty, (string)propContainer.GetProperty("KEY"));
    }

    [TestMethod]
    [DataRow("Default")]
    [DataRow("Concurrent")]
    public void SetBoolean(string containerType)
    {
        var propContainer = TestUtilities.CreatePropertiesContainer(containerType);

        propContainer.SetProperty("KEY_TRUE", true);
        propContainer.SetProperty("KEY_FALSE", false);

        Assert.AreEqual("X", (string)propContainer.GetProperty("KEY_TRUE"));
        Assert.AreEqual("", (string)propContainer.GetProperty("KEY_FALSE"));
        Assert.AreEqual(1, propContainer.CountProperties());
    }

    [TestMethod]
    [DataRow("Default")]
    [DataRow("Concurrent")]
    public void SetInt(string containerType)
    {
        var propContainer = TestUtilities.CreatePropertiesContainer(containerType);

        propContainer.SetProperty("KEY_1", 1);
        propContainer.SetProperty("KEY_0", 0);

        Assert.AreEqual(1, propContainer.GetProperty("KEY_1").AsInt32());
        Assert.AreEqual(0, propContainer.GetProperty("KEY_0").AsInt32());
        Assert.AreEqual(1, propContainer.CountProperties());
    }

    [TestMethod]
    [DataRow("Default")]
    [DataRow("Concurrent")]
    public void AddAndRemove(string containerType)
    {
        var propContainer = TestUtilities.CreatePropertiesContainer(containerType);

        propContainer.SetProperty("KEY", "TestValue");
        Assert.IsTrue(propContainer.ContainsProperty("KEY"));

        propContainer.SetProperty("KEY", PropertyValue.Empty);
        propContainer.SetProperty("KEY", PropertyValue.Empty);

        Assert.IsFalse(propContainer.ContainsProperty("KEY"));
    }

    [TestMethod]
    [DataRow("Default")]
    [DataRow("Concurrent")]
    public void OvertakeProperties(string containerType)
    {
        var propContainer1 = TestUtilities.CreatePropertiesContainer(containerType);
        var propContainer2 = TestUtilities.CreatePropertiesContainer(containerType);

        propContainer1.SetProperty("KEY_1", "test");
        propContainer1.SetProperty("KEY_2", 5);
        propContainer2.OvertakeAllProperties(propContainer1);

        Assert.AreEqual(2, propContainer2.CountProperties(), "CountProperties");
        Assert.AreEqual("test", (string)propContainer2.GetProperty("KEY_1"), "Value for KEY_1");
        Assert.AreEqual("5", (string)propContainer2.GetProperty("KEY_2"), "Value for KEY_2");
    }
}