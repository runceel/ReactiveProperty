using System;
using System.ComponentModel;
using System.Reactive.Linq;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reactive.Bindings.Extensions;

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

        [TestMethod]
        public void ToReactivePropertyAsSynchronizedConvertBackWhenHasErrorsFalse()
        {
            var model = new Model() { Age = 30 };
            var prop = model.ToReactivePropertyAsSynchronized(
                x => x.Age, // property selector
                x => "Age:" + x,  // convert
                x => int.Parse(x.Replace("Age:", "")), // convertBack
                ignoreValidationErrorValue: true)
                .SetValidateNotifyError((string x) =>
                {
                    // no use
                    return int.TryParse(x.Replace("Age:", ""), out var result) ? null : "error";
                });

            prop.Value.Is("Age:30");

            prop.Value = "Age:50";
            model.Age.Is(50);

            model.Age = 80;
            prop.Value.Is("Age:80");

            // ignore validation error.
            prop.Value = "xxxx";
            model.Age.Is(80);

            prop.Value = "Age:10";
            model.Age.Is(10);
        }

        [TestMethod]
        public void ToReactivePropertyAsSynchronizedMultipleConvertBack()
        {
            var model = new PointModel { Point = Tuple.Create(0, 1) };
            var propX = model.ToReactivePropertyAsSynchronized(
                x => x.Point,
                x => x.Item1,
                x => Tuple.Create(x, model.Point.Item2));
            var propY = model.ToReactivePropertyAsSynchronized(
                x => x.Point,
                x => x.Item2,
                x => Tuple.Create(model.Point.Item1, x));

            propX.Value.Is(0);
            propY.Value.Is(1);

            model.Point = Tuple.Create(10, 20);
            propX.Value.Is(10);
            propY.Value.Is(20);

            propX.Value = 100;
            model.Point.Is(x => x.Item1 == 100 && x.Item2 == 20);

            propY.Value = 200;
            model.Point.Is(x => x.Item1 == 100 && x.Item2 == 200);
        }

        private class Model : INotifyPropertyChanged
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

        private class PointModel : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            private static readonly PropertyChangedEventArgs PointPropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(Point));

            private Tuple<int, int> point;

            public Tuple<int, int> Point
            {
                get
                {
                    return point;
                }

                set
                {
                    if (point == value) { return; }
                    point = value;
                    PropertyChanged?.Invoke(this, PointPropertyChangedEventArgs);
                }
            }
        }
    }
}
