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
    public class INotifyPropertyChangedExtensionsTest : ReactiveTest
    {
        [TestMethod]
        public void ObserveProperty()
        {
            var testScheduler = new TestScheduler();
            var recorder = testScheduler.CreateObserver<string>();

            var m = new Model() { Name = "aaa" };
            m.ObserveProperty(x => x.Name).Subscribe(recorder);

            testScheduler.AdvanceTo(1000);
            m.Name = "bbb";

            recorder.Messages.Is(
                OnNext(0, "aaa"),
                OnNext(1000, "bbb"));
        }

        [TestMethod]
        public void ObservePropertyFalse()
        {
            var testScheduler = new TestScheduler();
            var recorder = testScheduler.CreateObserver<string>();

            var m = new Model() { Name = "aaa" };
            m.ObserveProperty(x => x.Name, false).Subscribe(recorder);

            testScheduler.AdvanceTo(1000);
            m.Name = "bbb";

            recorder.Messages.Is(
                OnNext(1000, "bbb"));
        }

        [TestMethod]
        public void ObservePropertyException()
        {
            var testScheduler = new TestScheduler();
            var recorder = testScheduler.CreateObserver<string>();
            var commonEx = new Exception();

            var m = new Model() { Name = "aaa" };
            m.ObserveProperty(x => x.Name)
                .Do(x => recorder.OnNext(x))
                .Do(_ => { throw commonEx; })
                .OnErrorRetry((Exception e) => recorder.OnError(e))
                .Subscribe();

            testScheduler.AdvanceTo(1000);
            m.Name = "bbb";

            recorder.Messages.Is(
                OnNext(0, "aaa"),
                OnError<string>(0, commonEx),
                OnNext(1000, "bbb"),
                OnError<string>(1000, commonEx));
        }

        [TestMethod]
        public void ObservePropertyExceptionFalse()
        {
            var testScheduler = new TestScheduler();
            var recorder = testScheduler.CreateObserver<string>();
            var commonEx = new Exception();

            var m = new Model() { Name = "aaa" };
            m.ObserveProperty(x => x.Name, false)
                .Do(x => recorder.OnNext(x))
                .Do(_ => { throw commonEx; })
                .OnErrorRetry((Exception e) => recorder.OnError(e))
                .Subscribe();

            testScheduler.AdvanceTo(1000);
            m.Name = "bbb";

            recorder.Messages.Is(
                OnNext(1000, "bbb"),
                OnError<string>(1000, commonEx));
        }

        [TestMethod]
        public void ToReactivePropertyAsSynchronized()
        {
            var model = new Model() { Name = "homuhomu" };
            var prop = model.ToReactivePropertyAsSynchronized(x => x.Name);

            prop.Value.Is("homuhomu");

            prop.Value = "madomado";
            model.Name.Is("madomado");

            model.Name = "mamimami";
            prop.Value.Is("mamimami");
        }

        [TestMethod]
        public void ToReactivePropertyAsSynchronizedConvert()
        {
            var model = new Model() { Age = 30 };
            var prop = model.ToReactivePropertyAsSynchronized(
                x => x.Age, // property selector
                x => "Age:" + x,  // convert
                x => int.Parse(x.Replace("Age:", ""))); // convertBack

            prop.Value.Is("Age:30");

            prop.Value = "Age:50";
            model.Age.Is(50);

            model.Age = 80;
            prop.Value.Is("Age:80");
        }

        class Model : INotifyPropertyChanged
        {
            private string name;
            public string Name
            {
                get { return name; }
                set { name = value; PropertyChanged(this, new PropertyChangedEventArgs("Name")); }
            }

            private int age;
            public int Age
            {
                get { return age; }
                set { age = value; PropertyChanged(this, new PropertyChangedEventArgs("Age")); }
            }

            public event PropertyChangedEventHandler PropertyChanged = (_, __) => { };
        }


    }
}
