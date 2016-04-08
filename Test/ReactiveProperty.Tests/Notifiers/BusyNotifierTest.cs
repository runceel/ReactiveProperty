using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reactive.Bindings.Notifiers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveProperty.Tests.Notifiers
{
    [TestClass]
    public class BusyNotifierTest
    {
        [TestMethod]
        public void SimpleCase()
        {
            var n = new BusyNotifier();
            var notifyPropertyChangedCounter = 0;
            n.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(BusyNotifier.IsBusy))
                {
                    notifyPropertyChangedCounter++;
                }
            };
            var observeNotifyIsBusyCounter = 0;
            var latestNotifyIsBusyValue = false;
            n.Subscribe(x =>
            {
                observeNotifyIsBusyCounter++;
                latestNotifyIsBusyValue = x;
            });

            notifyPropertyChangedCounter.Is(0);
            observeNotifyIsBusyCounter.Is(1);
            latestNotifyIsBusyValue.IsFalse();

            var d = n.ProcessStart();

            notifyPropertyChangedCounter.Is(1);
            observeNotifyIsBusyCounter.Is(2);
            latestNotifyIsBusyValue.IsTrue();
            n.IsBusy.IsTrue();

            d.Dispose();

            notifyPropertyChangedCounter.Is(2);
            observeNotifyIsBusyCounter.Is(3);
            latestNotifyIsBusyValue.IsFalse();
            n.IsBusy.IsFalse();
        }

        [TestMethod]
        public void MultipleCase()
        {
            var n = new BusyNotifier();
            var notifyPropertyChangedCounter = 0;
            n.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(BusyNotifier.IsBusy))
                {
                    notifyPropertyChangedCounter++;
                }
            };
            var observeNotifyIsBusyCounter = 0;
            var latestNotifyIsBusyValue = false;
            n.Subscribe(x =>
            {
                observeNotifyIsBusyCounter++;
                latestNotifyIsBusyValue = x;
            });

            notifyPropertyChangedCounter.Is(0);
            observeNotifyIsBusyCounter.Is(1);
            latestNotifyIsBusyValue.IsFalse();

            var d1 = n.ProcessStart();
            var d2 = n.ProcessStart();

            notifyPropertyChangedCounter.Is(1);
            observeNotifyIsBusyCounter.Is(2);
            latestNotifyIsBusyValue.IsTrue();
            n.IsBusy.IsTrue();

            d1.Dispose();

            notifyPropertyChangedCounter.Is(1);
            observeNotifyIsBusyCounter.Is(2);
            latestNotifyIsBusyValue.IsTrue();
            n.IsBusy.IsTrue();

            d2.Dispose();

            notifyPropertyChangedCounter.Is(2);
            observeNotifyIsBusyCounter.Is(3);
            latestNotifyIsBusyValue.IsFalse();
            n.IsBusy.IsFalse();
        }

        [TestMethod]
        public void NotifyCase()
        {
            var n = new BusyNotifier();

            var d = n.ProcessStart();

            var observeNotifyIsBusyCounter = 0;
            var latestNotifyIsBusyValue = false;
            n.Subscribe(x =>
            {
                observeNotifyIsBusyCounter++;
                latestNotifyIsBusyValue = x;
            });

            observeNotifyIsBusyCounter.Is(1);
            latestNotifyIsBusyValue.IsTrue();
            n.IsBusy.IsTrue();

            d.Dispose();

            observeNotifyIsBusyCounter.Is(2);
            latestNotifyIsBusyValue.IsFalse();
            n.IsBusy.IsFalse();
        }
    }
}
