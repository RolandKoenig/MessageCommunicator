using System;
using FirLib.Core.Patterns.PropertiesContainer;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FirLib.Tests.Core.Patterns.PropertiesContainer;

[TestClass]
public class ConcurrentMode
{
    [TestMethod]
    public void MassiveParallelAccess()
    {
        var propContainer = new ConcurrentPropertiesContainer();

        Parallel.For(0, 1000000, index =>
        {
            propContainer.SetProperty("KEY", index);
            propContainer.SetProperty("KEY_2", index);

            propContainer.GetProperty("KEY_3");
            propContainer.SetProperty("KEY_3", index);

            propContainer.OvertakeAllProperties(propContainer);
        });
    }
}