﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reactive.Bindings;

namespace ReactiveProperty.Tests
{
    [TestClass]
    public class ReadOnlyReactivePropertySlimTest
    {
        [TestMethod]
        public void NormalPattern()
        {
            var s = new Subject<string>();

            var rp = s.ToReadOnlyReactivePropertySlim();
            var buffer = new List<string>();
            rp.Subscribe(buffer.Add);

            rp.Value.IsNull();
            buffer.Count.Is(1);
            buffer[0].IsNull();

            s.OnNext("Hello");
            rp.Value.Is("Hello");
            buffer.Count.Is(2);
            buffer.Is(default(string), "Hello");

            s.OnNext("Hello");
            rp.Value.Is("Hello");
            buffer.Count.Is(2); // distinct until changed.
        }

        [TestMethod]
        public void MultiSubscribeTest()
        {
            var s = new Subject<string>();

            var rp = s.ToReadOnlyReactivePropertySlim();
            var buffer1 = new List<string>();
            rp.Subscribe(buffer1.Add);

            buffer1.Count.Is(1);
            s.OnNext("Hello world");
            buffer1.Count.Is(2);
            buffer1.Is(default(string), "Hello world");

            var buffer2 = new List<string>();
            rp.Subscribe(buffer2.Add);
            buffer1.Is(default(string), "Hello world");
            buffer2.Is("Hello world");

            s.OnNext("ReactiveProperty");
            buffer1.Is(default(string), "Hello world", "ReactiveProperty");
            buffer2.Is("Hello world", "ReactiveProperty");
        }

        [TestMethod]
        public void NormalPatternNoDistinctUntilChanged()
        {
            var s = new Subject<string>();

            var rp = s.ToReadOnlyReactivePropertySlim(
                mode: ReactivePropertyMode.RaiseLatestValueOnSubscribe);
            var buffer = new List<string>();
            rp.Subscribe(buffer.Add);

            rp.Value.IsNull();
            buffer.Count.Is(1);
            buffer[0].IsNull();

            s.OnNext("Hello");
            rp.Value.Is("Hello");
            buffer.Count.Is(2);
            buffer.Is(default(string), "Hello");

            s.OnNext("Hello");
            rp.Value.Is("Hello");
            buffer.Count.Is(3); // not distinct until changed.
        }

        [TestMethod]
        public void PropertyChangedTest()
        {
            var s = new Subject<string>();
            var rp = s.ToReadOnlyReactivePropertySlim();
            var buffer = new List<string>();
            rp.PropertyChanged += (_, args) =>
            {
                buffer.Add(args.PropertyName);
            };

            buffer.Count.Is(0);

            s.OnNext("Hello");
            buffer.Count.Is(1);

            s.OnNext("Hello");
            buffer.Count.Is(1);

            s.OnNext("World");
            buffer.Count.Is(2);
        }

        [TestMethod]
        public void PropertyChangedNoDistinctUntilChangedTest()
        {
            var s = new Subject<string>();
            var rp = s.ToReadOnlyReactivePropertySlim(
                mode: ReactivePropertyMode.RaiseLatestValueOnSubscribe);
            var buffer = new List<string>();
            rp.PropertyChanged += (_, args) =>
            {
                buffer.Add(args.PropertyName);
            };

            buffer.Count.Is(0);

            s.OnNext("Hello");
            buffer.Count.Is(1);

            s.OnNext("Hello");
            buffer.Count.Is(2);

            s.OnNext("World");
            buffer.Count.Is(3);
        }

        [TestMethod]
        public void BehaviorSubjectTest()
        {
            var s = new BehaviorSubject<string>("initial value");
            var rp = s.ToReadOnlyReactivePropertySlim();
            rp.Value.Is("initial value");
        }

        [TestMethod]
        public void ObservableCreateTest()
        {
            var i = 0;
            var s = Observable.Create<int>(ox =>
            {
                i++;
                return Disposable.Empty;
            });

            i.Is(0);
            var rp = s.ToReadOnlyReactivePropertySlim();
            i.Is(1);
        }

        [TestMethod]
        public void UnsubscribeTest()
        {
            var source = new ReactivePropertySlim<int>(mode: ReactivePropertyMode.None);
            var rp = source.ToReadOnlyReactivePropertySlim(mode: ReactivePropertyMode.None);

            var collector = new List<(string, int)>();
            var a = rp.Select(x => ("a", x)).Subscribe(collector.Add);
            var b = rp.Select(x => ("b", x)).Subscribe(collector.Add);
            var c = rp.Select(x => ("c", x)).Subscribe(collector.Add);

            source.Value = 99;
            collector.Is(("a", 99), ("b", 99), ("c", 99));

            collector.Clear();
            a.Dispose();

            source.Value = 40;
            collector.Is(("b", 40), ("c", 40));

            collector.Clear();
            c.Dispose();

            source.Value = 50;
            collector.Is(("b", 50));

            collector.Clear();
            b.Dispose();

            source.Value = 9999;
            collector.Count.Is(0);

            var d = rp.Select(x => ("d", x)).Subscribe(collector.Add);

            source.Value = 9;
            collector.Is(("d", 9));

            rp.Dispose();
        }

        [TestMethod]
        public void CreateFromObservableThatCompleteImmediately()
        {
            var x = Observable.Return(1).ToReadOnlyReactivePropertySlim();
            x.Value.Is(1);
        }

        [TestMethod]
        public void NormalPatternDisposeChangeValue()
        {
            var s = new Subject<Disposable>();

            var rp = s.ToReadOnlyReactivePropertySlim(mode: ReactivePropertyMode.DisposeChangedValue);
            var buffer = new List<Disposable>();
            rp.Subscribe(buffer.Add);

            s.OnNext(new Disposable());
            buffer[0].Is(d => !d.IsDisposed);

            s.OnNext(new Disposable());
            buffer[0].Is(d => d.IsDisposed);
        }

        private class Disposable : IDisposable
        {
            public bool IsDisposed { get; private set; }

            public void Dispose()
            {
                IsDisposed = true;
            }
        }
    }
}
