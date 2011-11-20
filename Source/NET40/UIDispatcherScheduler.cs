using System;
using System.Windows.Threading;
using System.Windows;
#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#else
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Disposables;
using System.Reactive.Concurrency;
#endif

namespace Codeplex.Reactive
{
    /// <summary>
    /// <para>If call Schedule on UIThread then schedule immediately else dispatch BeginInvoke.</para>
    /// <para>UIDIspatcherScheduler is created when access to UIDispatcher.Default first in the whole application.</para>
    /// <para>If you want to explicitly initialize, call UIDispatcherScheduler.Initialize() in App.xaml.cs.</para>
    /// </summary>
    public class UIDispatcherScheduler : IScheduler
    {
        // static

        static readonly UIDispatcherScheduler defaultScheduler = new UIDispatcherScheduler(
#if SILVERLIGHT
(Deployment.Current != null) ? Deployment.Current.Dispatcher : null
#else
Dispatcher.CurrentDispatcher
#endif
);

        /// <summary>
        /// <para>If call Schedule on UIThread then schedule immediately else dispatch BeginInvoke.</para>
        /// <para>UIDIspatcherScheduler is created when access to UIDispatcher.Default first in the whole application.</para>
        /// <para>If you want to explicitly initialize, call UIDispatcherScheduler.Initialize() in App.xaml.cs.</para>
        /// </summary>
        public static UIDispatcherScheduler Default
        {
            get { return defaultScheduler; }
        }

        /// <summary>
        /// Create UIDispatcherSchedule on called thread if is not initialized yet.
        /// </summary>
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
            get { return DateTimeOffset.Now; }
        }

#if !WINDOWS_PHONE

        public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            if (action == null) throw new ArgumentNullException("action");

            return Schedule(state, dueTime - Now, action);
        }

        public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            if (action == null) throw new ArgumentNullException("action");
            if (Dispatcher == null) // for designer mode
            {
                return Scheduler.CurrentThread.Schedule(state, dueTime, action);
            }

            var interval = Scheduler.Normalize(dueTime);
            if (interval.Ticks == 0) return Schedule(state, action); // schedule immediately

            var cancelable = new MultipleAssignmentDisposable();
            DispatcherTimer timer = new DispatcherTimer(
#if !SILVERLIGHT
                DispatcherPriority.Background, Dispatcher
#endif
                )
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

        /// <summary>
        /// <para>If call Schedule on UIThread then schedule immediately else dispatch BeginInvoke.</para>
        /// </summary>
        public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
        {
            if (action == null) throw new ArgumentNullException("action");
            if (Dispatcher == null) return Scheduler.CurrentThread.Schedule(state, action); // for designer mode

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

#else

        public IDisposable Schedule(Action action, TimeSpan dueTime)
        {
            if (action == null) throw new ArgumentNullException("action");
            if (Dispatcher == null) return Scheduler.CurrentThread.Schedule(action, dueTime); // for designer mode

            return Scheduler.Dispatcher.Schedule(action, dueTime);
        }

        /// <summary>
        /// <para>If call Schedule on UIThread then schedule immediately else dispatch BeginInvoke.</para>
        /// </summary>
        public IDisposable Schedule(Action action)
        {
            if (action == null) throw new ArgumentNullException("action");
            if (Dispatcher == null) return Scheduler.CurrentThread.Schedule(action); // for designer mode

            if (Dispatcher.CheckAccess())
            {
                action();
                return Disposable.Empty;
            }
            else
            {
                var cancelable = new BooleanDisposable();

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (!cancelable.IsDisposed)
                    {
                        action();
                    }
                }));

                return cancelable;
            }
        }

#endif
    }
}