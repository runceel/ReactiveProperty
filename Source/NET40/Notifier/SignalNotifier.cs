using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;
#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#else
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
#endif

namespace Codeplex.Reactive.Notifier
{
    public enum SignalChangedStatus
    {
        Increment, Decrement, Empty, Max
    }

    public class SignalNotifier : IObservable<SignalChangedStatus>
    {
        readonly object lockObject = new object();
        readonly Subject<SignalChangedStatus> statusChanged = new Subject<SignalChangedStatus>();
        readonly int max;

        public int Max { get { return max; } }
        public int Count { get; private set; }

        public SignalNotifier(int max = int.MaxValue)
        {
            Contract.Requires<ArgumentException>(max >= 1, "value allows over 1");
            Contract.Ensures(Max == max);
            Contract.Ensures(Count == 0);

            this.max = max;
        }

        public void Increment(int incrementCount = 1)
        {
            Contract.Requires<ArgumentException>(incrementCount >= 1, "value allows over 1");
            Contract.Ensures(Count == Contract.OldValue(Count) + incrementCount);

            lock (lockObject)
            {
                if (incrementCount + Count > Max) throw new InvalidOperationException("over max value");

                Count += incrementCount;
                statusChanged.OnNext(SignalChangedStatus.Increment);
                if (Count == Max) statusChanged.OnNext(SignalChangedStatus.Max);
            }
        }

        public void Decrement(int decrementCount = 1)
        {
            Contract.Requires<ArgumentException>(decrementCount >= 1, "value allows over 1");
            Contract.Ensures(Count == Contract.OldValue(Count) - decrementCount);

            lock (lockObject)
            {
                if (Count - decrementCount < 0) throw new InvalidOperationException("not allow decrement to under 0");

                Count -= decrementCount;
                statusChanged.OnNext(SignalChangedStatus.Decrement);
                if (Count == 0) statusChanged.OnNext(SignalChangedStatus.Empty);
            }
        }

        public IDisposable Subscribe(IObserver<SignalChangedStatus> observer)
        {
            return statusChanged.Subscribe(observer);
        }
    }
}