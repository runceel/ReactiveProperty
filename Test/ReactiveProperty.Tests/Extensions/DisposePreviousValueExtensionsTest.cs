using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using Reactive.Bindings.Extensions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ReactiveProperty.Tests.Extensions
{
    [TestClass]
    public class DisposePreviousValueExtensionsTest
    {
        [TestMethod]
        public void DisposePrevious()
        {
            var source = new Subject<DisposableObject>();
            source.DisposePreviousValue().Subscribe();

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
        }

        [TestMethod]
        public void DisposeCurrentValueOnCompleted()
        {
            var source = new Subject<DisposableObject>();
            var dispose = source.DisposePreviousValue().Subscribe();

            var value = new DisposableObject();
            source.OnNext(value);

            value.IsDisposed.IsFalse();
            dispose.Dispose();
            value.IsDisposed.IsTrue();
        }

        [TestMethod]
        public void DisposeCurrentValueOnError()
        {
            var source = new Subject<DisposableObject>();
            source.DisposePreviousValue().Subscribe();

            var value = new DisposableObject();
            source.OnNext(value);

            value.IsDisposed.IsFalse();
            source.OnError(new Exception());
            value.IsDisposed.IsTrue();
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
