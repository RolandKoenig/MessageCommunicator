using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirLib.Core.Patterns.ObjectPooling;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FirLib.Tests.Core.Patterns.ObjectPooling;

[TestClass]
public class Pooling
{
    [TestMethod]
    public void StringBuilderPooling()
    {
        var pool = new PooledStringBuilders();

        var strBuilder = pool.TakeStringBuilder();
        strBuilder.Append("Test Test Test Test");
        strBuilder.Append("Test Test Test");
        pool.ReRegisterStringBuilder(strBuilder);

        strBuilder = pool.TakeStringBuilder();
        strBuilder.Append("Test Test Test Test");
        strBuilder.Append("Test Test Test");
        pool.ReRegisterStringBuilder(strBuilder);

        Assert.AreEqual(1, pool.Count, "Count StringBuilders");
        Assert.AreEqual(0, pool.TakeStringBuilder().Length, "Initial length");
    }
}