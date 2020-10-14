using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reactive.Bindings;
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
        public void PropertyChangedWithEmptyPropertyName()
        {
            var testScheduler = new TestScheduler();
            var recorderForName = testScheduler.CreateObserver<string>();
            var recorderForAge = testScheduler.CreateObserver<int>();

            var m = new Model { Name = "aaa", Age = 10 };
            m.ObserveProperty(x => x.Name)
                .Do(x => recorderForName.OnNext(x))
                .Subscribe();
            m.ObserveProperty(x => x.Age)
                .Do(x => recorderForAge.OnNext(x))
                .Subscribe();

            m.BatchUpdateProperties("bbb", 100);

            recorderForName.Messages.Is(
                OnNext(0, "aaa"),
                OnNext(0, "bbb"));
            recorderForAge.Messages.Is(
                OnNext(0, 10),
                OnNext(0, 100));
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
            var model = new PointModel { Point = (0, 1) };
            var propX = model.ToReactivePropertyAsSynchronized(
                x => x.Point,
                x => x.x,
                x => (x, model.Point.y));
            var propY = model.ToReactivePropertyAsSynchronized(
                x => x.Point,
                x => x.y,
                x => (model.Point.x, x));

            propX.Value.Is(0);
            propY.Value.Is(1);

            model.Point = (10, 20);
            propX.Value.Is(10);
            propY.Value.Is(20);

            propX.Value = 100;
            model.Point.Is(x => x.Item1 == 100 && x.Item2 == 20);

            propY.Value = 200;
            model.Point.Is(x => x.Item1 == 100 && x.Item2 == 200);
        }

        [TestMethod]
        public void ToReactivePropertyAsSynchronizedObservableCase()
        {
            var source = new ReactivePropertySlim<int>(100);
            var target = source.ToReactivePropertyAsSynchronized(x => x.Value,
                ox => ox.Select(x => x.ToString()),
                ox => ox.Where(x => int.TryParse(x, out _)).Select(x => int.Parse(x)));

            target.Value.Is("100");
            target.Value = "10";
            source.Value.Is(10);
            target.Value = "xxx";
            source.Value.Is(10);
        }

        [TestMethod]
        public void ToReactivePropertyAsSynchronizedObservableWithIgnoreValidationErrorValueCase()
        {
            var source = new ReactivePropertySlim<int>(100);
            var target = source.ToReactivePropertyAsSynchronized(x => x.Value,
                ox => ox.Select(x => x.ToString()),
                ox => ox.Select(x => int.Parse(x)),
                ignoreValidationErrorValue: true);
            target.SetValidateNotifyError(x => int.TryParse(x, out _) ? null : "Error");

            target.Value.Is("100");
            target.Value = "10";
            source.Value.Is(10);
            target.Value = "xxx";
            source.Value.Is(10);
        }

        // ToReactivePropertySlimSynchronized
        [TestMethod]
        public void ToReactivePropertyAsSynchronizedSlim()
        {
            var model = new Model() { Name = "homuhomu" };
            var prop = model.ToReactivePropertySlimAsSynchronized(x => x.Name);

            prop.Value.Is("homuhomu");

            prop.Value = "madomado";
            model.Name.Is("madomado");

            model.Name = "mamimami";
            prop.Value.Is("mamimami");
        }

        [TestMethod]
        public void ToReactivePropertySlimAsSynchronizedConvert()
        {
            var model = new Model() { Age = 30 };
            var prop = model.ToReactivePropertySlimAsSynchronized(
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
        public void ToReactivePropertySlimAsSynchronizedMultipleConvertBack()
        {
            var model = new PointModel { Point = (0, 1) };
            var propX = model.ToReactivePropertySlimAsSynchronized(
                x => x.Point,
                x => x.x,
                x => (x, model.Point.y));
            var propY = model.ToReactivePropertySlimAsSynchronized(
                x => x.Point,
                x => x.y,
                x => (model.Point.x, x));

            propX.Value.Is(0);
            propY.Value.Is(1);

            model.Point = (10, 20);
            propX.Value.Is(10);
            propY.Value.Is(20);

            propX.Value = 100;
            model.Point.Is(x => x.Item1 == 100 && x.Item2 == 20);

            propY.Value = 200;
            model.Point.Is(x => x.Item1 == 100 && x.Item2 == 200);
        }

        [TestMethod]
        public void ToReactivePropertySlimAsSynchronizedObservableCase()
        {
            var source = new ReactivePropertySlim<int>(100);
            var target = source.ToReactivePropertySlimAsSynchronized(x => x.Value,
                ox => ox.Select(x => x.ToString()),
                ox => ox.Where(x => int.TryParse(x, out _)).Select(x => int.Parse(x)));

            target.Value.Is("100");
            target.Value = "10";
            source.Value.Is(10);
            target.Value = "xxx";
            source.Value.Is(10);
        }

        [TestMethod]
        public void ToReactivePropertySlimAsSynchronizedEnsureClearEventHandler()
        {
            var source = new PointModel();
            var rp = source.ToReactivePropertySlimAsSynchronized(x => x.Point);
            var scheduler = new TestScheduler();
            var observer = scheduler.CreateObserver<(int x, int y)>();
            rp.Subscribe(observer); // OnNext((0, 0))

            source.Handlers.Count.Is(1);
            observer.Messages.Is(OnNext(0, (0, 0)));

            rp.Dispose();
            source.Handlers.Any().IsFalse();
            source.Point = (10, 10);
            observer.Messages.Is(OnNext(0, (0, 0)), OnCompleted<(int, int)>(0));
        }

        [TestMethod]
        public void ObservePropertyNestedPropertyPath()
        {
            var source = new Model
            {
                Child = new Model
                {
                    Child = new Model
                    {
                        Name = "name",
                    },
                },
            };

            var rp = source.ObserveProperty(x => x.Child.Child.Name)
                .ToReadOnlyReactivePropertySlim();
            rp.Value.Is("name");
            source.Child.Child.Name = "changed";
            rp.Value.Is("changed");

            source.Child = new Model();
            rp.Value.IsNull();
            source.Child = new Model
            {
                Child = new Model
                {
                    Name = "replaced",
                },
            };
            rp.Value.Is("replaced");
            source.IsPropertyChangedEmpty.IsFalse();
            rp.Dispose();
            source.IsPropertyChangedEmpty.IsTrue();
        }

        [TestMethod]
        public void ObservePropertyNestedPropertyPath_PartOfPathNodesIsNullCase()
        {
            var source = new Model
            {
                Child = new Model
                {
                    Child = new Model
                    {
                        Name = "name",
                    },
                },
            };

            var rp = source.ObserveProperty(x => x.Child.Child.Name)
                .ToReadOnlyReactivePropertySlim();
            rp.Value.Is("name");

            var oldChild = source.Child;
            oldChild.IsPropertyChangedEmpty.IsFalse();
            source.Child = new Model();
            oldChild.IsPropertyChangedEmpty.IsTrue();
            rp.Value.IsNull();

            source.IsPropertyChangedEmpty.IsFalse();
            source.Child.IsPropertyChangedEmpty.IsFalse();
            rp.Dispose();
            source.IsPropertyChangedEmpty.IsTrue();
            source.Child.IsPropertyChangedEmpty.IsTrue();
        }

        [TestMethod]
        public void ObservePropertyNestedPropertyPath_PartOfPathNodesIsNullCaseWithValueType()
        {
            var source = new Model
            {
                Child = new Model
                {
                    Child = new Model
                    {
                        Age = 10,
                    },
                },
            };

            var rp = source.ObserveProperty(x => x.Child.Child.Age)
                .ToReadOnlyReactivePropertySlim();
            rp.Value.Is(10);

            var oldChild = source.Child;
            oldChild.IsPropertyChangedEmpty.IsFalse();
            source.Child = new Model();
            oldChild.IsPropertyChangedEmpty.IsTrue();
            rp.Value.Is(default(int));

            source.IsPropertyChangedEmpty.IsFalse();
            source.Child.IsPropertyChangedEmpty.IsFalse();
            rp.Dispose();
            source.IsPropertyChangedEmpty.IsTrue();
            source.Child.IsPropertyChangedEmpty.IsTrue();
        }

        [TestMethod]
        public void ToSynchronizedReactiveProperty_NestedPropertyCase()
        {
            var source = new Model
            {
                Child = new Model
                {
                    Child = new Model
                    {
                        Age = 10,
                    },
                },
            };

            var rp = source.ToReactivePropertyAsSynchronized(x => x.Child.Child.Age);
            rp.Value.Is(10);
            rp.Value = 100;
            source.Child.Child.Age.Is(100);

            source.IsPropertyChangedEmpty.IsFalse();
            source.Child.IsPropertyChangedEmpty.IsFalse();
            source.Child.Child.IsPropertyChangedEmpty.IsFalse();
            rp.Dispose();
            source.IsPropertyChangedEmpty.IsTrue();
            source.Child.IsPropertyChangedEmpty.IsTrue();
            source.Child.Child.IsPropertyChangedEmpty.IsTrue();
        }

        [TestMethod]
        public void ToSynchronizedReactiveProperty_NestedPropertyCaseWithScheduler()
        {
            var source = new Model
            {
                Child = new Model
                {
                    Child = new Model
                    {
                        Age = 10,
                    },
                },
            };

            var rp = source.ToReactivePropertyAsSynchronized(x => x.Child.Child.Age, raiseEventScheduler: new TestScheduler());
            rp.Value.Is(10);
            rp.Value = 100;
            source.Child.Child.Age.Is(100);

            source.IsPropertyChangedEmpty.IsFalse();
            source.Child.IsPropertyChangedEmpty.IsFalse();
            source.Child.Child.IsPropertyChangedEmpty.IsFalse();
            rp.Dispose();
            source.IsPropertyChangedEmpty.IsTrue();
            source.Child.IsPropertyChangedEmpty.IsTrue();
            source.Child.Child.IsPropertyChangedEmpty.IsTrue();
        }

        [TestMethod]
        public void ToSynchronizedReactiveProperty_NestedProperty_NullValue_Case()
        {
            var source = new Model
            {
                Child = new Model
                {
                    Child = new Model
                    {
                        Age = 10,
                    },
                },
            };

            var rp = source.ToReactivePropertyAsSynchronized(x => x.Child.Child.Age);
            var firstChild = source.Child;
            source.Child = new Model();
            rp.Value.Is(0);
            rp.Value = 100;
            rp.Value.Is(100);
            var secondChild = source.Child;
            source.Child = new Model
            {
                Child = new Model
                {
                    Age = 9999,
                },
            };

            rp.Value.Is(9999);
            firstChild.IsPropertyChangedEmpty.IsTrue();
            secondChild.IsPropertyChangedEmpty.IsTrue();
            source.Child.IsPropertyChangedEmpty.IsFalse();
            rp.Dispose();
            firstChild.IsPropertyChangedEmpty.IsTrue();
            secondChild.IsPropertyChangedEmpty.IsTrue();
            source.Child.IsPropertyChangedEmpty.IsTrue();
        }

        [TestMethod]
        public void ToSynchronizedReactiveProperty_NestedPropertyWithConvertLogic_NullValue_Case()
        {
            var source = new Model
            {
                Child = new Model
                {
                    Child = new Model
                    {
                        Age = 10,
                    },
                },
            };

            var rp = source.ToReactivePropertyAsSynchronized(x => x.Child.Child.Age,
                convert: x => x.ToString(),
                convertBack: x => int.Parse(x));
            var firstChild = source.Child;
            source.Child = new Model();
            rp.Value.Is("0");
            rp.Value = "100";
            rp.Value.Is("100");
            var secondChild = source.Child;
            source.Child = new Model
            {
                Child = new Model
                {
                    Age = 9999,
                },
            };

            rp.Value.Is("9999");
            firstChild.IsPropertyChangedEmpty.IsTrue();
            secondChild.IsPropertyChangedEmpty.IsTrue();
            source.Child.IsPropertyChangedEmpty.IsFalse();
            rp.Dispose();
            firstChild.IsPropertyChangedEmpty.IsTrue();
            secondChild.IsPropertyChangedEmpty.IsTrue();
            source.Child.IsPropertyChangedEmpty.IsTrue();
        }

        [TestMethod]
        public void ToSynchronizedReactiveProperty_NestedPropertyWithConvertLogicAndScheduler_NullValue_Case()
        {
            var source = new Model
            {
                Child = new Model
                {
                    Child = new Model
                    {
                        Age = 10,
                    },
                },
            };

            var scheduler = new TestScheduler();

            var rp = source.ToReactivePropertyAsSynchronized(x => x.Child.Child.Age,
                convert: x => x.ToString(),
                convertBack: x => int.Parse(x),
                raiseEventScheduler: scheduler);
            var firstChild = source.Child;
            source.Child = new Model();
            rp.Value.Is("0");
            rp.Value = "100";
            rp.Value.Is("100");
            var secondChild = source.Child;
            source.Child = new Model
            {
                Child = new Model
                {
                    Age = 9999,
                },
            };

            rp.Value.Is("9999");
            firstChild.IsPropertyChangedEmpty.IsTrue();
            secondChild.IsPropertyChangedEmpty.IsTrue();
            source.Child.IsPropertyChangedEmpty.IsFalse();
            rp.Dispose();
            firstChild.IsPropertyChangedEmpty.IsTrue();
            secondChild.IsPropertyChangedEmpty.IsTrue();
            source.Child.IsPropertyChangedEmpty.IsTrue();
        }

        private class Model : INotifyPropertyChanged
        {
            private void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
            {
                if (EqualityComparer<T>.Default.Equals(field, value)) { return; }
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            private string _name;

            public string Name
            {
                get => _name;
                set => SetProperty(ref _name, value);
            }

            private int _age;

            public int Age
            {
                get => _age;
                set => SetProperty(ref _age, value);
            }

            public void BatchUpdateProperties(string name, int age)
            {
                (this._name, this._age) = (name, age);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
            }

            private Model _model;
            public Model Child
            {
                get => _model;
                set => SetProperty(ref _model, value);
            }

            public event PropertyChangedEventHandler PropertyChanged;

            public bool IsPropertyChangedEmpty => PropertyChanged == null;
        }

        private class PointModel : INotifyPropertyChanged
        {
            private readonly object _lock = new object();
            public List<PropertyChangedEventHandler> Handlers { get; } = new List<PropertyChangedEventHandler>();
            public event PropertyChangedEventHandler PropertyChanged
            {
                add
                {
                    lock (_lock)
                    {
                        Handlers.Add(value);
                    }
                }
                remove
                {
                    lock (_lock)
                    {
                        Handlers.Remove(value);
                    }
                }
            }

            private static readonly PropertyChangedEventArgs PointPropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(Point));

            private (int x, int y) _point;

            public (int x, int y) Point
            {
                get
                {
                    return _point;
                }

                set
                {
                    if (_point == value) { return; }
                    _point = value;
                    lock (_lock)
                    {
                        var handlers = Handlers.ToArray();
                        foreach (var h in handlers)
                        {
                            h(this, PointPropertyChangedEventArgs);
                        }
                    }
                }
            }
        }
    }
}
