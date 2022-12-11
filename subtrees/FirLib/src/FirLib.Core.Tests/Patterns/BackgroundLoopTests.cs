using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FirLib.Core.Patterns.BackgroundLoops;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FirLib.Core.Tests.Patterns;

[TestClass]
public class BackgroundLoopTests
{
    [TestMethod]
    public async Task StartAndStop()
    {
        var firstTickTaskSource = new TaskCompletionSource<object?>();

        var startingCalled = false;
        var tickCalled = false;
        var stoppingCalled = false;

        var backgroundLoop = new BackgroundLoop();
        backgroundLoop.Starting += (_, _) => { startingCalled = true; };
        backgroundLoop.Tick += (_, _) =>
        {
            tickCalled = true;
            firstTickTaskSource.TrySetResult(null);
        };
        backgroundLoop.Stopping += (_, _) => { stoppingCalled = true; };

        await backgroundLoop.StartAsync();
        await firstTickTaskSource.Task;
        await backgroundLoop.StopAsync(5000);

        Assert.IsTrue(startingCalled, nameof(startingCalled));
        Assert.IsTrue(tickCalled, nameof(tickCalled));
        Assert.IsTrue(stoppingCalled, nameof(stoppingCalled));
    }

    [TestMethod]
    public async Task TicksMoreTimes()
    {
        var firstTickTaskSource = new TaskCompletionSource<object?>();

        var tickCount = 0;

        var backgroundLoop = new BackgroundLoop(string.Empty, 10);
        backgroundLoop.Tick += (_, _) =>
        {
            tickCount++;
            if (tickCount == 5)
            {
                firstTickTaskSource.TrySetResult(null);
            }
        };

        await backgroundLoop.StartAsync();
        await firstTickTaskSource.Task;
        await backgroundLoop.StopAsync(5000);

        Assert.IsTrue(tickCount >= 5, nameof(tickCount));
    }

    [TestMethod]
    public async Task IsSynchronizationContextSet()
    {
        var firstTickTaskSource = new TaskCompletionSource<object?>();

        var isSyncContextSet = false;

        var backgroundLoop = new BackgroundLoop(string.Empty, 500);
        backgroundLoop.Tick += (_, _) =>
        {
            isSyncContextSet =
                SynchronizationContext.Current is BackgroundLoop.BackgroundLoopSynchronizationContext;
            firstTickTaskSource.TrySetResult(null);
        };

        await backgroundLoop.StartAsync();
        await firstTickTaskSource.Task;
        await backgroundLoop.StopAsync(5000);

        Assert.IsTrue(isSyncContextSet, nameof(isSyncContextSet));
    }

    [TestMethod]
    public async Task IsThreadNameSet()
    {
        var firstTickTaskSource = new TaskCompletionSource<object?>();

        var isThreadNameSet = false;

        var backgroundLoop = new BackgroundLoop("TestThreadName", 10);
        backgroundLoop.Tick += (_, _) =>
        {
            isThreadNameSet = Thread.CurrentThread.Name == "TestThreadName";
            firstTickTaskSource.TrySetResult(null);
        };

        await backgroundLoop.StartAsync();
        await firstTickTaskSource.Task;
        await backgroundLoop.StopAsync(5000);

        Assert.IsTrue(isThreadNameSet, nameof(isThreadNameSet));
    }

    [TestMethod]
    public async Task InvokeMethod_AfterStart()
    {
        var firstTickTaskSource = new TaskCompletionSource<object?>();

        var firstTickPassed = false;

        var backgroundLoop = new BackgroundLoop(string.Empty, 500);
        backgroundLoop.Tick += (_, _) =>
        {
            if (!firstTickPassed)
            {
                firstTickPassed = true;
                firstTickTaskSource.TrySetResult(null);
            }
        };

        await backgroundLoop.StartAsync();
        await firstTickTaskSource.Task;

        var methodInvoked = false;
        await backgroundLoop.InvokeAsync(() => methodInvoked = true);

        await backgroundLoop.StopAsync(5000);

        Assert.IsTrue(methodInvoked, nameof(methodInvoked));
    }

    [TestMethod]
    public async Task InvokeMethod_BeforeStart()
    {
        var backgroundLoop = new BackgroundLoop(string.Empty, 500);

        var methodInvoked = false;
        var invokeTask = backgroundLoop.InvokeAsync(() => methodInvoked = true);
        await backgroundLoop.StartAsync();
        await invokeTask;
        await backgroundLoop.StopAsync(5000);

        Assert.IsTrue(methodInvoked, nameof(methodInvoked));
    }
}