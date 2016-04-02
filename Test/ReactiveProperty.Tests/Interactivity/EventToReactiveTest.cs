using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reactive.Bindings;
using System.Reactive.Linq;
using Microsoft.Reactive.Testing;
using System.Reactive.Subjects;
using System.Threading;
using Reactive.Bindings.Interactivity;
using System.Reactive;

namespace ReactiveProperty.Tests
{
    [TestClass]
    public class EventToReactiveTest : ReactiveTest
    {
        [TestMethod]
        public void Invoke()
        {
            var etr = new EventToReactive();
            var rp = new ReactiveProperty<int>(mode: ReactivePropertyMode.None);

            etr.ReactiveProperty = rp;

            etr.AsDynamic().Invoke((object)100);
            rp.Value.Is(100);

            etr.AsDynamic().Invoke((object)1000);
            rp.Value.Is(1000);
        }

        [TestMethod]
        public void Ignore()
        {
            var recorder = new TestScheduler().CreateObserver<Unit>();
            var etr = new EventToReactive();
            var rp = new ReactiveProperty<Unit>(mode: ReactivePropertyMode.None);
            rp.Subscribe(recorder);

            etr.ReactiveProperty = rp;
            etr.IgnoreEventArgs = true;

            etr.AsDynamic().Invoke((object)100);
            etr.AsDynamic().Invoke((object)200);
            etr.AsDynamic().Invoke((object)"hogehoge");

            recorder.Messages.Is(
                OnNext(0, Unit.Default),
                OnNext(0, Unit.Default),
                OnNext(0, Unit.Default));
        }
    }
}
