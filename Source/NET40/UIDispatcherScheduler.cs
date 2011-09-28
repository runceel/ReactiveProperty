using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Threading;
using System.Diagnostics.Contracts;

namespace Codeplex.Reactive
{
    public class UIDispatcherScheduler : IScheduler
    {
        // static

        static readonly UIDispatcherScheduler defaultScheduler =
            new UIDispatcherScheduler(Dispatcher.CurrentDispatcher);

        public static UIDispatcherScheduler Default
        {
            get { return defaultScheduler; }
        }

        public static void Initialize()
        {
            var _ = defaultScheduler; // initialize
        }

        // instance

        UIDispatcherScheduler(Dispatcher dispatcher)
        {
            this.Dispatcher = dispatcher;
        }

        public Dispatcher Dispatcher { get; private set; }

        public DateTimeOffset Now
        {
            get { return Scheduler.Now; }
        }

        public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            if (action == null) throw new ArgumentNullException("action");

            return Schedule(state, dueTime - Now, action);
        }

        public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            if (action == null) throw new ArgumentNullException("action");

            var interval = Scheduler.Normalize(dueTime);
            if (interval.Ticks == 0) return Schedule(state, action); // schedule immediately

            var cancelable = new MultipleAssignmentDisposable();
            DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Background, Dispatcher)
            {
                Interval = interval
            };
            timer.Tick += (sender, e) =>
            {
                var _timer = timer;
                if (_timer != null) _timer.Stop();
                timer = null;
                cancelable.Disposable = action(this, state);
            };

            timer.Start();

            cancelable.Disposable = Disposable.Create(() =>
            {
                var _timer = timer;
                if (_timer != null) _timer.Stop();
                timer = null;
            });

            return cancelable;
        }

        public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
        {
            if (action == null) throw new ArgumentNullException("action");

            if (Dispatcher.CheckAccess())
            {
                return action(this, state);
            }
            else
            {
                var cancelable = new SingleAssignmentDisposable();

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (!cancelable.IsDisposed)
                    {
                        cancelable.Disposable = action(this, state);
                    }
                }));

                return cancelable;
            }
        }
    }

    public static class UIDispatcherObservableExtensions
    {
        public static IObservable<T> ObserveOnUIDispatcher<T>(this IObservable<T> source)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Ensures(Contract.Result<IObservable<T>>() != null);

            var result = source.ObserveOn(UIDispatcherScheduler.Default);
            
            Contract.Assume(result != null);
            return result;
        }

        public static IObservable<T> SubscribeOnUIDispatcher<T>(this IObservable<T> source)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Ensures(Contract.Result<IObservable<T>>() != null);

            var result = source.SubscribeOn(UIDispatcherScheduler.Default);

            Contract.Assume(result != null);
            return result;
        }
    }
}