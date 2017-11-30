using System;
using System.Reactive.Subjects;
using System.Reactive.Disposables;
using System.Reactive.Concurrency;
using System.ComponentModel;

namespace Reactive.Bindings
{
    /// <summary>
    /// Hot(stoppable/continuable) observable timer.
    /// </summary>
    public class ReactiveTimer : IObservable<long>, IDisposable, INotifyPropertyChanged
    {
        private static PropertyChangedEventArgs IsEnabledPropertyChangedEventArs { get; } = new PropertyChangedEventArgs(nameof(IsEnabled));
        private static PropertyChangedEventArgs IntervalPropertyChangedEventArgs { get; } = new PropertyChangedEventArgs(nameof(Interval));
        private long Count { get; set; } = 0;
        private bool IsDisposed { get; set; } = false;
        private SerialDisposable Disposable { get; } = new SerialDisposable();
        private IScheduler Scheduler { get; }
        private Subject<long> Subject { get; } = new Subject<long>();

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>Operate scheduler ThreadPoolScheduler.</summary>
        public ReactiveTimer(TimeSpan interval)
            : this(interval, System.Reactive.Concurrency.Scheduler.Default)
        { }

        /// <summary>Operate scheduler is argument's scheduler.</summary>
        public ReactiveTimer(TimeSpan interval, IScheduler scheduler)
        {
            Interval = interval;
            this.Scheduler = scheduler;
        }

        private TimeSpan interval;

        /// <summary>Timer interval.</summary>
        public TimeSpan Interval
        {
            get { return this.interval; }
            set
            {
                if (this.interval == value)
                {
                    return;
                }

                this.interval = value;
                this.PropertyChanged?.Invoke(this, IntervalPropertyChangedEventArgs);
            }
        }



        private bool isEnabled;

        public bool IsEnabled
        {
            get => this.isEnabled;
            private set
            {
                if (this.isEnabled == value)
                {
                    return;
                }

                this.isEnabled = value;
                this.PropertyChanged?.Invoke(this, IsEnabledPropertyChangedEventArs);
            }
        }

        /// <summary>Start timer immediately.</summary>
        public void Start() => Start(TimeSpan.Zero);

        /// <summary>Start timer after dueTime.</summary>
        public void Start(TimeSpan dueTime)
        {
            Disposable.Disposable = new CompositeDisposable
            {
                Scheduler.Schedule(dueTime,
                    self =>
                    {
                        Subject.OnNext(Count++);
                        self(Interval);
                    }),
                System.Reactive.Disposables.Disposable.Create(() => this.IsEnabled = false),
            };
            this.IsEnabled = true;
        }

        /// <summary>Stop timer.</summary>
        public void Stop() =>
            Disposable.Disposable = System.Reactive.Disposables.Disposable.Empty;

        /// <summary>Stop timer and reset count.</summary>
        public void Reset()
        {
            Count = 0;
            Disposable.Disposable = System.Reactive.Disposables.Disposable.Empty;
        }

        /// <summary>Subscribe observer.</summary>
        public IDisposable Subscribe(IObserver<long> observer) =>
            Subject.Subscribe(observer);

        /// <summary>
        /// Send OnCompleted to subscribers and unsubscribe all.
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed) return;

            IsDisposed = true;
            Subject.OnCompleted();
            Subject.Dispose();
            Disposable.Dispose();
        }
    }
}