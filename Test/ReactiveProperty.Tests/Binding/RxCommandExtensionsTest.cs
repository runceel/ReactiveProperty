using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reactive.Bindings;
using Reactive.Bindings.Binding;
using Reactive.Bindings.Extensions;
using System.Reactive;
using System.Reactive.Linq;

namespace ReactiveProperty.Tests.Binding
{
    [TestClass]
    public class RxCommandExtensionsTest
    {
        [TestMethod]
        public void ToEventHandlerTest()
        {
            var called = false;
            var command = new ReactiveCommand();
            command.Subscribe(_ => called = true);
            var b = new DummyButton();
            b.Click += command.ToEventHandler();

            called.IsFalse();
            b.OnClick();
            called.IsTrue();
        }

        [TestMethod]
        public void ToEventHandlerDetachTest()
        {
            var counter = 0;
            var command = new ReactiveCommand();
            command.Subscribe(_ => counter++);
            var b = new DummyButton();
            var h = command.ToEventHandler();
            b.Click += h;

            counter.Is(0);
            b.OnClick();
            counter.Is(1);

            b.Click -= h;
            b.OnClick();
            counter.Is(1);
        }
    }

    public class DummyButton
    {
        public event EventHandler Click = delegate { };

        public void OnClick()
        {
            this.Click(this, EventArgs.Empty);
        }
    }
}
