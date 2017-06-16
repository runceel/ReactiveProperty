using System;
using System.Reactive.Subjects;
using System.Reactive.Disposables;
using System.Reactive.Concurrency;

namespace Reactive.Bindings
{
    /// <summary>
    /// Hot(stoppable/continuable) observable timer.
    /// </summary>
    public class ReactiveTimer : IObservable<long>, IDisposable
    {
        private long Count { get; set; } = 0;
        private bool IsDisposed { get; set; } = false;
        private SerialDisposable Disposable { get; } = new SerialDisposable();
        private IScheduler Scheduler { get; }
        private Subject<long> Subject { get; } = new Subject<long>();

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

        /// <summary>Timer interval.</summary>
        public TimeSpan Interval { get; set; }

        /// <summary>Start timer immediately.</summary>
        public void Start() => Start(TimeSpan.Zero);

        /// <summary>Start timer after dueTime.</summary>
        public void Start(TimeSpan dueTime) =>
            Disposable.Disposable = Scheduler.Schedule(dueTime,
                self =>
                {
                    Subject.OnNext(Count++);
                    self(Interval);
                });

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