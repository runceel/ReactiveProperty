using System;
using System.Diagnostics.Contracts;
#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
using SerialDisposable = Microsoft.Phone.Reactive.MutableDisposable;
#else
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Disposables;
using System.Reactive.Concurrency;
#endif

namespace Codeplex.Reactive
{
    /// <summary>
    /// Hot(stoppable/continuable) observable timer.
    /// </summary>
    public class ReactiveTimer : IObservable<long>, IDisposable
    {
        long count = 0;
        bool isDisposed = false;
        readonly SerialDisposable disposable = new SerialDisposable();
        readonly IScheduler scheduler;
        readonly Subject<long> subject = new Subject<long>();

        /// <summary>Operate scheduler ThreadPoolScheduler.</summary>
        public ReactiveTimer(TimeSpan interval)
            : this(interval, Scheduler.Default)
        { }

        /// <summary>Operate scheduler is argument's scheduler.</summary>
        public ReactiveTimer(TimeSpan interval, IScheduler scheduler)
        {
            Interval = interval;
            this.scheduler = scheduler;
        }

        /// <summary>Timer interval.</summary>
        public TimeSpan Interval { get; set; }

        /// <summary>Start timer immediately.</summary>
        public void Start()
        {
            Start(TimeSpan.Zero);
        }

        /// <summary>Start timer after dueTime.</summary>
        public void Start(TimeSpan dueTime)
        {
            disposable.Disposable = scheduler.Schedule(dueTime,
                self =>
                {
                    subject.OnNext(count++);
                    self(Interval);
                });
        }

        /// <summary>Stop timer.</summary>
        public void Stop()
        {
            disposable.Disposable = Disposable.Empty;
        }

        /// <summary>Stop timer and reset count.</summary>
        public void Reset()
        {
            count = 0;
            disposable.Disposable = Disposable.Empty;
        }

        /// <summary>Subscribe observer.</summary>
        public IDisposable Subscribe(IObserver<long> observer)
        {
            return subject.Subscribe(observer);
        }

        /// <summary>
        /// Send OnCompleted to subscribers and unsubscribe all.
        /// </summary>
        public void Dispose()
        {
            if (isDisposed) return;

            isDisposed = true;
            subject.OnCompleted();
            subject.Dispose();
            disposable.Dispose();
        }
    }
}