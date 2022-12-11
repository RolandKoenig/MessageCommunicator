using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirLib.Core.Patterns.ErrorAnalysis.Analyzers;

namespace FirLib.Core.Patterns.ErrorAnalysis;

public class ExceptionInfo
{
    /// <summary>
    /// Gets a collection containing all child nodes.
    /// </summary>
    public List<ExceptionInfoNode> ChildNodes { get; } = new();

    /// <summary>
    /// Gets or sets the main message.
    /// </summary>
    public string MainMessage
    {
        get;
        set;
    }

    public string Description
    {
        get;
        set;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionInfo"/> class.
    /// </summary>
    public ExceptionInfo(Exception ex, IEnumerable<IExceptionAnalyzer>? exceptionAnalyzers = null)
    {
        exceptionAnalyzers ??= CreateDefaultAnalyzers();

        this.MainMessage = "Unexpected Error";
        this.Description = ex.Message;

        // Analyze the given exception 
        ExceptionInfoNode newNode = new(ex);
        this.ChildNodes.Add(newNode);

        AnalyzeException(ex, newNode, exceptionAnalyzers);
    }

    public static IEnumerable<IExceptionAnalyzer> CreateDefaultAnalyzers()
    {
        yield return new DefaultExceptionAnalyzer();
        yield return new SystemIOExceptionAnalyzer();
    }

    /// <summary>
    /// Analyzes the given exception.
    /// </summary>
    /// <param name="ex">The exception to be analyzed.</param>
    /// <param name="targetNode">The target node where to put all data to.</param>
    /// <param name="exceptionAnalyzers">All loaded analyzer objects.</param>
    private static void AnalyzeException(Exception ex, ExceptionInfoNode targetNode, IEnumerable<IExceptionAnalyzer> exceptionAnalyzers)
    {
        // Query over all exception data
        foreach(IExceptionAnalyzer actAnalyzer in exceptionAnalyzers)
        {
            // Read all properties of the current exception
            var exceptionInfos = actAnalyzer.ReadExceptionInfo(ex);
            if (exceptionInfos != null)
            {
                foreach (ExceptionProperty actProperty in exceptionInfos)
                {
                    if (actProperty == null) { continue; }
                    if (string.IsNullOrEmpty(actProperty.Name)) { continue; }

                    ExceptionInfoNode propertyNode = new(actProperty);
                    targetNode.ChildNodes.Add(propertyNode);
                }
            }

            // Read all inner exception information
            var innerExceptions = actAnalyzer.GetInnerExceptions(ex);
            if (innerExceptions != null)
            {
                foreach (Exception actInnerException in innerExceptions)
                {
                    if (actInnerException == null) { continue; }

                    var exceptionExists = false;
                    foreach (var actExistingExInfo in targetNode.ChildNodes)
                    {
                        if (actExistingExInfo.Exception == actInnerException)
                        {
                            exceptionExists = true;
                            break;
                        }
                    }
                    if (exceptionExists) { continue; }

                    ExceptionInfoNode actInfoNode = new(actInnerException);
                    AnalyzeException(actInnerException, actInfoNode, exceptionAnalyzers);
                    targetNode.ChildNodes.Add(actInfoNode);
                }
            }
        }

        // Sort all generated nodes
        targetNode.ChildNodes.Sort();
    }
}