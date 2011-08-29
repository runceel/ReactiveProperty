using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reactive.Subjects;
using System.Reactive.Linq;

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

        public int Max { get; private set; }
        public int Count { get; private set; }

        public SignalNotifier(int max = int.MaxValue)
        {
            if (max <= 0) throw new ArgumentException("1以上の値を指定してください。");

            Max = max;
        }

        public void Increment(int incrementCount = 1)
        {
            if (incrementCount < 1) throw new ArgumentException("1以上の値を指定してください。");

            lock (lockObject)
            {
                if (incrementCount + Count > Max) throw new InvalidOperationException("指定した最大値を超えてインクリメントしようとしました。");

                Count += incrementCount;
                statusChanged.OnNext(SignalChangedStatus.Increment);
                if (Count == Max) statusChanged.OnNext(SignalChangedStatus.Max);
            }
        }

        public void Decrement(int decrementCount = 1)
        {
            if (decrementCount < 1) throw new ArgumentException("1以上の値を指定してください。");

            lock (lockObject)
            {
                if (Count - decrementCount < 0) throw new InvalidOperationException("0以下にデクリメントしようとしました。");

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