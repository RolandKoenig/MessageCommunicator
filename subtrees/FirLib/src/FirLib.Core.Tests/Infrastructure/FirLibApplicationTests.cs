using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FirLib.Core.Infrastructure;
using FirLib.Core.Patterns;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FirLib.Core.Tests.Infrastructure
{
    [TestClass]
    [DoNotParallelize]
    public class FirLibApplicationTests
    {
        [TestMethod]
        public void LoadAndUnload()
        {
            using (_ = FirLibApplication.GetLoader().Load())
            {
                Assert.IsTrue(FirLibApplication.IsLoaded);
                Assert.IsNotNull(FirLibApplication.Current);
            }
            Assert.IsFalse(FirLibApplication.IsLoaded);
        }

        [TestMethod]
        public void LoadAndUnload_Multiple()
        {
            FirLibApplication? firstAppObject = null;
            using (_ = FirLibApplication.GetLoader().Load())
            {
                Assert.IsTrue(FirLibApplication.IsLoaded);
                Assert.IsNotNull(FirLibApplication.Current);

                firstAppObject = FirLibApplication.Current;
            }
            Assert.IsFalse(FirLibApplication.IsLoaded);

            FirLibApplication? secondAppObject = null;
            using (_ = FirLibApplication.GetLoader().Load())
            {
                Assert.IsTrue(FirLibApplication.IsLoaded);
                Assert.IsNotNull(FirLibApplication.Current);

                secondAppObject = FirLibApplication.Current;
            }
            Assert.IsFalse(FirLibApplication.IsLoaded);

            Assert.AreNotEqual(firstAppObject, secondAppObject, "First and second application object");
        }

        [TestMethod]
        public void LoadAndUnloadMethods()
        {
            var loadActionCalled = false;
            var unloadActionCalled = false;
            using (_ = FirLibApplication.GetLoader()
                .AddLoadAction(() => loadActionCalled = true)
                .AddUnloadAction(() => unloadActionCalled = true)
                .Load())
            {
                Assert.IsTrue(loadActionCalled, nameof(loadActionCalled));
                Assert.IsFalse(unloadActionCalled, nameof(unloadActionCalled));
            }
            Assert.IsTrue(loadActionCalled, nameof(loadActionCalled));
            Assert.IsTrue(unloadActionCalled, nameof(unloadActionCalled));
        }

        [TestMethod]
        public void DisposeServiceOnUnload()
        {
            var disposeOnServiceCalled = false;
            var dummyDisposable = new DummyDisposable(
                () => disposeOnServiceCalled = true);

            using (_ = FirLibApplication.GetLoader()
                .AddService(typeof(IDisposable), dummyDisposable)
                .Load())
            {
                Assert.IsFalse(disposeOnServiceCalled, nameof(disposeOnServiceCalled));
            }
            Assert.IsTrue(disposeOnServiceCalled, nameof(disposeOnServiceCalled));
        }

        [TestMethod]
        public void ConfigureCurrentThreadAsMainGuiThread()
        {
            Exception? occurredException = null;

            var dummyThread = new Thread(() =>
            {
                // Delegate this test to an own thread because change of Thread.Name can not be reversed

                try
                {
                    using (_ = FirLibApplication.GetLoader()
                        .ConfigureCurrentThreadAsMainGuiThread()
                        .Load())
                    {
                        Assert.AreEqual(FirLibConstants.MESSENGER_NAME_GUI, Thread.CurrentThread.Name);
                    }
                    Assert.AreEqual(FirLibConstants.MESSENGER_NAME_GUI, Thread.CurrentThread.Name);
                }
                catch (Exception e)
                {
                    occurredException = e;
                }
            });
            dummyThread.Start();
            dummyThread.Join();

            Assert.IsNull(occurredException, occurredException?.ToString() ?? string.Empty);
        }
    }
}
