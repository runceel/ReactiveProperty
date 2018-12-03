using System;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;

namespace Reactive.Bindings.Notifiers
{
    /// <summary>
    /// Event kind of CountNotifier.
    /// </summary>
    public enum CountChangedStatus
    {
        /// <summary>
        /// Count incremented.
        /// </summary>
        Increment,

        /// <summary>
        /// Count decremented.
        /// </summary>
        Decrement,

        /// <summary>
        /// Count is zero.
        /// </summary>
        Empty,

        /// <summary>
        /// Count arrived max.
        /// </summary>
        Max
    }

    /// <summary>
    /// Notify event of count flag.
    /// </summary>
    public class CountNotifier : IObservable<CountChangedStatus>, INotifyPropertyChanged
    {
        private readonly object lockObject = new object();
        private readonly Subject<CountChangedStatus> statusChanged = new Subject<CountChangedStatus>();

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        /// <returns></returns>
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly int max;
        private int count;

        /// <summary>
        /// Gets the maximum.
        /// </summary>
        /// <value>The maximum.</value>
        public int Max => max;

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get
            {
                return count;
            }

            private set
            {
                count = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Setup max count of signal.
        /// </summary>
        public CountNotifier(int max = int.MaxValue)
        {
            if (max <= 0) {
                throw new ArgumentException(nameof(max));
            }

            this.max = max;
        }

        /// <summary>
        /// Increment count and notify status.
        /// </summary>
        public IDisposable Increment(int incrementCount = 1)
        {
            if (incrementCount < 0) {
                throw new ArgumentException(nameof(incrementCount));
            }

            lock (lockObject) {
                if (Count == Max) {
                    return Disposable.Empty;
                } else if (incrementCount + Count > Max) {
                    Count = Max;
                } else {
                    Count += incrementCount;
                }

                statusChanged.OnNext(CountChangedStatus.Increment);
                if (Count == Max) {
                    statusChanged.OnNext(CountChangedStatus.Max);
                }

                return Disposable.Create(() => Decrement(incrementCount));
            }
        }

        /// <summary>
        /// Decrement count and notify status.
        /// </summary>
        public void Decrement(int decrementCount = 1)
        {
            if (decrementCount < 0) {
                throw new ArgumentException(nameof(decrementCount));
            }

            lock (lockObject) {
                if (Count == 0) {
                    return;
                } else if (Count - decrementCount < 0) {
                    Count = 0;
                } else {
                    Count -= decrementCount;
                }

                statusChanged.OnNext(CountChangedStatus.Decrement);
                if (Count == 0) {
                    statusChanged.OnNext(CountChangedStatus.Empty);
                }
            }
        }

        /// <summary>
        /// Subscribe observer.
        /// </summary>
        public IDisposable Subscribe(IObserver<CountChangedStatus> observer) => statusChanged.Subscribe(observer);

        private void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
