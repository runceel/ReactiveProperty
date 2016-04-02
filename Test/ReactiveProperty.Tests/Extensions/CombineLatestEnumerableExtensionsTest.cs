using Reactive.Bindings.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Reactive.Bindings;
using Microsoft.Reactive.Testing;
using System.Linq;

namespace ReactiveProperty.Tests
{
    [TestClass()]
    public class CombineLatestEnumerableExtensionsTest : ReactiveTest
    {
        [TestMethod()]
        public void CombineLatestValuesAreAllTrueTest()
        {
            var m = ReactivePropertyMode.RaiseLatestValueOnSubscribe;
            var recorder = new TestScheduler().CreateObserver<bool>();

            var x = new ReactiveProperty<bool>(mode: m);
            var y = new ReactiveProperty<bool>(mode: m);
            var z = new ReactiveProperty<bool>(mode: m);

            new[] { x, y, z }.CombineLatestValuesAreAllTrue().Subscribe(recorder);
            recorder.Messages.First().Is(OnNext(0, false));

            x.Value = true; recorder.Messages.Last().Is(OnNext(0, false));
            y.Value = true; recorder.Messages.Last().Is(OnNext(0, false));
            z.Value = true; recorder.Messages.Last().Is(OnNext(0, true));
            y.Value = false; recorder.Messages.Last().Is(OnNext(0, false));
            z.Value = false; recorder.Messages.Last().Is(OnNext(0, false));
            x.Value = true; recorder.Messages.Last().Is(OnNext(0, false));
            z.Value = true; recorder.Messages.Last().Is(OnNext(0, false));
            y.Value = true; recorder.Messages.Last().Is(OnNext(0, true));
            x.Value = false; recorder.Messages.Last().Is(OnNext(0, false));
            y.Value = false; recorder.Messages.Last().Is(OnNext(0, false));
            z.Value = false; recorder.Messages.Last().Is(OnNext(0, false));
            z.Value = true; recorder.Messages.Last().Is(OnNext(0, false));
        }

        [TestMethod()]
        public void CombineLatestValuesAreAllFalseTest()
        {
            var m = ReactivePropertyMode.RaiseLatestValueOnSubscribe;
            var recorder = new TestScheduler().CreateObserver<bool>();

            var x = new ReactiveProperty<bool>(mode: m);
            var y = new ReactiveProperty<bool>(mode: m);
            var z = new ReactiveProperty<bool>(mode: m);

            new[] { x, y, z }.CombineLatestValuesAreAllFalse().Subscribe(recorder);
            recorder.Messages.Last().Is(OnNext(0, true));
            x.Value = true;
            recorder.Messages.Last().Is(OnNext(0, false));
            y.Value = true;
            recorder.Messages.Last().Is(OnNext(0, false));
            z.Value = true;
            recorder.Messages.Last().Is(OnNext(0, false));
            x.Value = false;
            recorder.Messages.Last().Is(OnNext(0, false));
            y.Value = false;
            recorder.Messages.Last().Is(OnNext(0, false));
            z.Value = false;
            recorder.Messages.Last().Is(OnNext(0, true));
        }
    }
}
