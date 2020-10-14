using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reactive.Bindings.Extensions;

namespace ReactiveProperty.Tests.Extensions
{
    [TestClass]
    public class DisposePreviousValueExtensionsTest : ReactiveTest
    {
        [TestMethod]
        public void DisposePrevious()
        {
            var testScheduler = new TestScheduler();
            var recorder = testScheduler.CreateObserver<DisposableObject>();
            var source = new Subject<DisposableObject>();
            source.DisposePreviousValue().Subscribe(recorder);

            var first = new DisposableObject();
            var second = new DisposableObject();

            first.IsDisposed.IsFalse();
            second.IsDisposed.IsFalse();

            source.OnNext(first);
            first.IsDisposed.IsFalse();
            second.IsDisposed.IsFalse();

            source.OnNext(second);
            first.IsDisposed.IsTrue();
            second.IsDisposed.IsFalse();

            recorder.Messages.Is(
               OnNext<DisposableObject>(0, x => x == first),
               OnNext<DisposableObject>(0, x => x == second));
        }

        [TestMethod]
        public void DisposeCurrentValueDispose()
        {
            var testScheduler = new TestScheduler();
            var recorder = testScheduler.CreateObserver<DisposableObject>();
            var source = new Subject<DisposableObject>();
            var dispose = source.DisposePreviousValue().Subscribe(recorder);

            var value = new DisposableObject();
            source.OnNext(value);

            value.IsDisposed.IsFalse();
            dispose.Dispose();
            value.IsDisposed.IsTrue();

            recorder.Messages.Is(
                OnNext<DisposableObject>(0, x => x == value));
        }

        [TestMethod]
        public void DisposeCurrentValueOnCompleted()
        {
            var testScheduler = new TestScheduler();
            var recorder = testScheduler.CreateObserver<DisposableObject>();
            var source = new Subject<DisposableObject>();
            source.DisposePreviousValue().Subscribe(recorder);

            var value = new DisposableObject();
            source.OnNext(value);

            value.IsDisposed.IsFalse();
            source.OnCompleted();
            value.IsDisposed.IsTrue();

            recorder.Messages.Is(
                OnNext<DisposableObject>(0, x => x == value),
                OnCompleted<DisposableObject>(0));
        }

        [TestMethod]
        public void DisposeCurrentValueOnError()
        {
            var testScheduler = new TestScheduler();
            var recorder = testScheduler.CreateObserver<DisposableObject>();
            var source = new Subject<DisposableObject>();
            source.DisposePreviousValue().Subscribe(recorder);

            var value = new DisposableObject();
            source.OnNext(value);

            value.IsDisposed.IsFalse();
            source.OnError(new Exception("test error"));
            value.IsDisposed.IsTrue();

            recorder.Messages.Is(
                OnNext<DisposableObject>(0, x => x == value),
                OnError<DisposableObject>(0, x => x.Message == "test error"));
        }
    }

    class DisposableObject : IDisposable
    {
        public bool IsDisposed { get; set; }
        public void Dispose()
        {
            IsDisposed = true;
        }
    }
}
