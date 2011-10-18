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
    /// <summary>Event kind of SignalNotifier.</summary>
    public enum SignalChangedStatus
    {
        /// <summary>Count incremented.</summary>
        Increment,
        /// <summary>Count decremented.</summary>
        Decrement,
        /// <summary>Count is zero.</summary>
        Empty,
        /// <summary>Count arrived max.</summary>
        Max
    }

    /// <summary>
    /// Notify event of count signals.
    /// </summary>
    public class SignalNotifier : IObservable<SignalChangedStatus>
    {
        readonly object lockObject = new object();
        readonly Subject<SignalChangedStatus> statusChanged = new Subject<SignalChangedStatus>();
        readonly int max;

        public int Max { get { return max; } }
        public int Count { get; private set; }

        /// <summary>
        /// Setup max count of signal.
        /// </summary>
        public SignalNotifier(int max = int.MaxValue)
        {
            Contract.Requires<ArgumentException>(max >= 1, "value allows over 1");
            Contract.Ensures(Max == max);

            this.max = max;
        }

        /// <summary>
        /// Increment count and notify status.
        /// </summary>
        public void Increment(int incrementCount = 1)
        {
            Contract.Requires<ArgumentException>(incrementCount >= 1, "value allows over 1");

            lock (lockObject)
            {
                if (Count == Max) return;
                else if (incrementCount + Count > Max) Count = Max;
                else Count += incrementCount;

                statusChanged.OnNext(SignalChangedStatus.Increment);
                if (Count == Max) statusChanged.OnNext(SignalChangedStatus.Max);
            }
        }

        /// <summary>
        /// Decrement count and notify status.
        /// </summary>
        public void Decrement(int decrementCount = 1)
        {
            Contract.Requires<ArgumentException>(decrementCount >= 1, "value allows over 1");

            lock (lockObject)
            {
                if (Count == 0) return;
                else if (Count - decrementCount < 0) Count = 0;
                else Count -= decrementCount;

                statusChanged.OnNext(SignalChangedStatus.Decrement);
                if (Count == 0) statusChanged.OnNext(SignalChangedStatus.Empty);
            }
        }

        /// <summary>
        /// Subscribe observer.
        /// </summary>
        public IDisposable Subscribe(IObserver<SignalChangedStatus> observer)
        {
            return statusChanged.Subscribe(observer);
        }
    }
}