using System;
using System.Linq;
using FirLib.Core.Patterns.ErrorAnalysis.Analyzers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FirLib.Tests.Core.Patterns.ErrorAnalysis;

[TestClass]
public class Analyzer
{
    [TestMethod]
    public void DefaultAnalyzer()
    {
        Exception catchedException;
        try
        {
            throw new ApplicationException("Dummy Exception");
        }
        catch (Exception e)
        {
            catchedException = e;
        }

        var exAnalyzer = new DefaultExceptionAnalyzer();
        var infos = exAnalyzer.ReadExceptionInfo(catchedException);
        var innerExceptions = exAnalyzer.GetInnerExceptions(catchedException);

        Assert.IsNotNull(infos);

        var infoDict = TestUtilities.ToDictionary(infos!);
        Assert.AreEqual(0, innerExceptions?.Count() ?? 0, "Inner exceptions");
        Assert.AreEqual("System.ApplicationException", infoDict["Type"], "Type");
        Assert.AreEqual("Dummy Exception", infoDict["Message"], "Message");
        Assert.AreEqual(nameof(this.DefaultAnalyzer), infoDict["SourceMethod.Name"], "SourceMethod.Name");
    }
}