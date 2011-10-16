using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;
using Codeplex.Reactive.Extensions;
using Microsoft.Reactive.Testing;
using System.Reactive.Linq;

namespace ReactiveProperty.Tests.Extensions
{
    [TestClass]
    public class INotifyPropertyChangingExtensionsTest : ReactiveTest
    {
        [TestMethod]
        public void ObservePropertyChanging()
        {
            var testScheduler = new TestScheduler();
            var recorder = testScheduler.CreateObserver<string>();

            var m = new Model() { Name = "aaa" };
            m.ObservePropertyChanging(x => x.Name).Subscribe(recorder);

            testScheduler.AdvanceTo(1000);
            m.Name = "bbb";
            m.Name = "ccc";

            recorder.Messages.Is(
                OnNext(0, "aaa"),
                OnNext(1000, "aaa"),
                OnNext(1000, "bbb"));
        }

        [TestMethod]
        public void ObservePropertyChangingFalse()
        {
            var testScheduler = new TestScheduler();
            var recorder = testScheduler.CreateObserver<string>();

            var m = new Model() { Name = "aaa" };
            m.ObservePropertyChanging(x => x.Name, false).Subscribe(recorder);

            testScheduler.AdvanceTo(1000);
            m.Name = "bbb";
            m.Name = "ccc";

            recorder.Messages.Is(
                OnNext(1000, "aaa"),
                OnNext(1000, "bbb")
                );
        }

        [TestMethod]
        public void ObservePropertyChangingException()
        {
            var testScheduler = new TestScheduler();
            var recorder = testScheduler.CreateObserver<string>();
            var commonEx = new Exception();

            var m = new Model() { Name = "aaa" };
            m.ObservePropertyChanging(x => x.Name)
                .Do(x => recorder.OnNext(x))
                .Do(_ => { throw commonEx; })
                .OnErrorRetry((Exception e) => recorder.OnError(e))
                .Subscribe();

            testScheduler.AdvanceTo(1000);
            m.Name = "bbb";
            m.Name = "ccc";

            recorder.Messages.Is(
                OnNext(0, "aaa"),
                OnError<string>(0, commonEx),
                OnNext(1000, "aaa"),
                OnError<string>(1000, commonEx),
                OnNext(1000, "bbb"),
                OnError<string>(1000, commonEx)
                );
        }

        [TestMethod]
        public void ObservePropertyChangingExceptionFalse()
        {
            var testScheduler = new TestScheduler();
            var recorder = testScheduler.CreateObserver<string>();
            var commonEx = new Exception();

            var m = new Model() { Name = "aaa" };
            m.ObservePropertyChanging(x => x.Name, false)
                .Do(x => recorder.OnNext(x))
                .Do(_ => { throw commonEx; })
                .OnErrorRetry((Exception e) => recorder.OnError(e))
                .Subscribe();

            testScheduler.AdvanceTo(1000);
            m.Name = "bbb";
            m.Name = "ccc";

            recorder.Messages.Is(
                OnNext(1000, "aaa"),
                OnError<string>(1000, commonEx),
                OnNext(1000, "bbb"),
                OnError<string>(1000, commonEx)
                );
        }

        class Model : INotifyPropertyChanging
        {
            private string name;
            public string Name
            {
                get { return name; }
                set { PropertyChanging(this, new PropertyChangingEventArgs("Name")); name = value; }
            }

            public event PropertyChangingEventHandler PropertyChanging = (_, __) => { };
        }
    }
}
