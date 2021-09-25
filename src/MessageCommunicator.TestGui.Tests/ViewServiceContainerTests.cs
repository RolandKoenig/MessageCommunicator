using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using Avalonia.Controls;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MessageCommunicator.TestGui.Tests
{
    [TestClass]
    public class ViewServiceContainerTests
    {
        [TestMethod]
        public void Check_EmptyObject()
        {
            var fakeOwner = A.Fake<IControl>();
            var viewServiceContainer = new ViewServiceContainer(fakeOwner);

            Assert.IsFalse(viewServiceContainer.IsObserving);
        }

        [TestMethod]
        public void Check_RegisterNullViewModel()
        {
            var fakeOwner = A.Fake<IControl>();
            var viewServiceContainer = new ViewServiceContainer(fakeOwner);

            var compositeDisposable = new CompositeDisposable();
            viewServiceContainer.StartObserving(compositeDisposable, null);

            Assert.IsTrue(compositeDisposable.Count == 1);
            Assert.IsTrue(viewServiceContainer.IsObserving);
        }

        [TestMethod]
        public void Check_RegisterNullViewModel_WithDeregister()
        {
            var fakeOwner = A.Fake<IControl>();
            var viewServiceContainer = new ViewServiceContainer(fakeOwner);

            var compositeDisposable = new CompositeDisposable();
            viewServiceContainer.StartObserving(compositeDisposable, null);

            compositeDisposable.Dispose();

            Assert.IsTrue(compositeDisposable.Count == 0);
            Assert.IsFalse(viewServiceContainer.IsObserving);
        }

        [TestMethod]
        public void Check_RegisterNullViewModel_WithViewService()
        {
            var fakeOwner = A.Fake<IControl>();
            var viewServiceContainer = new ViewServiceContainer(fakeOwner);
            var fakeViewService1 = A.Fake<IViewService>();
            var fakeViewService2 = A.Fake<IViewService>();

            viewServiceContainer.ViewServices.Add(fakeViewService1);

            var compositeDisposable = new CompositeDisposable();
            viewServiceContainer.StartObserving(compositeDisposable, null);

            viewServiceContainer.ViewServices.Add(fakeViewService2);

            Assert.IsTrue(compositeDisposable.Count == 1);
            Assert.IsTrue(viewServiceContainer.IsObserving);
            Assert.IsTrue(Fake.GetCalls(fakeViewService1).Any(actCall => actCall.Method.Name == $"add_{nameof(IViewService.ViewServiceRequest)}"));
            Assert.IsTrue(Fake.GetCalls(fakeViewService2).Any(actCall => actCall.Method.Name == $"add_{nameof(IViewService.ViewServiceRequest)}"));
        }

        [TestMethod]
        public void Check_RegisterNullViewModel__WithDeregister_WithViewService()
        {
            var fakeOwner = A.Fake<IControl>();
            var viewServiceContainer = new ViewServiceContainer(fakeOwner);
            var fakeViewService1 = A.Fake<IViewService>();
            var fakeViewService2 = A.Fake<IViewService>();
            
            viewServiceContainer.ViewServices.Add(fakeViewService1);

            var compositeDisposable = new CompositeDisposable();
            viewServiceContainer.StartObserving(compositeDisposable, null);

            viewServiceContainer.ViewServices.Add(fakeViewService2);

            compositeDisposable.Dispose();

            Assert.IsTrue(compositeDisposable.Count == 0);
            Assert.IsFalse(viewServiceContainer.IsObserving);
            Assert.IsTrue(Fake.GetCalls(fakeViewService1).Any(actCall => actCall.Method.Name == $"add_{nameof(IViewService.ViewServiceRequest)}"));
            Assert.IsTrue(Fake.GetCalls(fakeViewService1).Any(actCall => actCall.Method.Name == $"remove_{nameof(IViewService.ViewServiceRequest)}"));
            Assert.IsTrue(Fake.GetCalls(fakeViewService2).Any(actCall => actCall.Method.Name == $"add_{nameof(IViewService.ViewServiceRequest)}"));
            Assert.IsTrue(Fake.GetCalls(fakeViewService2).Any(actCall => actCall.Method.Name == $"remove_{nameof(IViewService.ViewServiceRequest)}"));
        }
    }
}
