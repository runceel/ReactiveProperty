using System;
using System.Reactive.Linq;
using Reactive.Bindings.Notifiers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Reactive.Testing;

namespace ReactiveProperty.Tests
{
    [TestClass]
    public class BooleanNotifierTest : ReactiveTest
    {
        [TestMethod]
        public void TestInitialTrue()
        {
            var notifier = new BooleanNotifier(true);
            var recorder = new TestScheduler().CreateObserver<bool>();
            notifier.Subscribe(recorder);

            notifier.Value.Is(true);

            notifier.TurnOn();
            recorder.Messages.Count.Is(0);

            notifier.TurnOff();
            notifier.Value = true;
            notifier.Value = true;
            notifier.Value = false;
            notifier.TurnOn();
            notifier.SwitchValue();
            notifier.SwitchValue();

            recorder.Messages.Is(
                OnNext(0, false),
                OnNext(0, true),
                OnNext(0, true),
                OnNext(0, false),
                OnNext(0, true),
                OnNext(0, false),
                OnNext(0, true));
        }

        [TestMethod]
        public void TestInitialFalse()
        {
            var notifier = new BooleanNotifier(false);
            var recorder = new TestScheduler().CreateObserver<bool>();
            notifier.Subscribe(recorder);

            notifier.Value.Is(false);

            notifier.TurnOn();
            notifier.TurnOff();
            notifier.Value = true;
            notifier.Value = true;
            notifier.Value = false;
            notifier.TurnOn();
            notifier.SwitchValue();
            notifier.SwitchValue();

            recorder.Messages.Is(
                OnNext(0, true),
                OnNext(0, false),
                OnNext(0, true),
                OnNext(0, true),
                OnNext(0, false),
                OnNext(0, true),
                OnNext(0, false),
                OnNext(0, true));

            recorder.Messages.Clear();

            notifier.TurnOn();
            recorder.Messages.Count.Is(0);
        }
    }
}