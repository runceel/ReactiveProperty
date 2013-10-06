using System;
using System.Windows.Input;
using System.Diagnostics.Contracts;
using Codeplex.Reactive.Extensions;
#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#else
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
#endif

namespace Codeplex.Reactive
{
    /// <summary>
    /// Represents ReactiveCommand&lt;object&gt;
    /// </summary>
    public class ReactiveCommand : ReactiveCommand<object>
    {
        /// <summary>
        /// CanExecute is always true. When disposed CanExecute change false called on UIDispatcherScheduler.
        /// </summary>
        public ReactiveCommand()
            : base()
        { }

        /// <summary>
        /// CanExecute is always true. When disposed CanExecute change false called on scheduler.
        /// </summary>
        public ReactiveCommand(IScheduler scheduler)
            : base(scheduler)
        {
        }

        /// <summary>
        /// CanExecuteChanged is called from canExecute sequence on UIDispatcherScheduler.
        /// </summary>
        public ReactiveCommand(IObservable<bool> canExecuteSource, bool initialValue = true)
            : base(canExecuteSource, initialValue)
        {
        }

        /// <summary>
        /// CanExecuteChanged is called from canExecute sequence on scheduler.
        /// </summary>
        public ReactiveCommand(IObservable<bool> canExecuteSource, IScheduler scheduler, bool initialValue = true)
            : base(canExecuteSource, scheduler, initialValue)
        {
        }

        /// <summary>Push null to subscribers.</summary>
        public void Execute()
        {
            Execute(null);
        }
    }

    public class ReactiveCommand<T> : IObservable<T>, ICommand, IDisposable
    {
        public event EventHandler CanExecuteChanged;

        readonly Subject<T> trigger = new Subject<T>();
        readonly IDisposable canExecuteSubscription;
        readonly IScheduler scheduler;
        bool isCanExecute;
        bool isDisposed = false;

        /// <summary>
        /// CanExecute is always true. When disposed CanExecute change false called on UIDispatcherScheduler.
        /// </summary>
        public ReactiveCommand()
            : this(Observable.Never<bool>())
        {
        }

        /// <summary>
        /// CanExecute is always true. When disposed CanExecute change false called on scheduler.
        /// </summary>
        public ReactiveCommand(IScheduler scheduler)
            : this(Observable.Never<bool>(), scheduler)
        {
        }

        /// <summary>
        /// CanExecuteChanged is called from canExecute sequence on UIDispatcherScheduler.
        /// </summary>
        public ReactiveCommand(IObservable<bool> canExecuteSource, bool initialValue = true)
            : this(canExecuteSource, UIDispatcherScheduler.Default, initialValue)
        {
        }

        /// <summary>
        /// CanExecuteChanged is called from canExecute sequence on scheduler.
        /// </summary>
        public ReactiveCommand(IObservable<bool> canExecuteSource, IScheduler scheduler, bool initialValue = true)
        {
            this.isCanExecute = initialValue;
            this.scheduler = scheduler;
            this.canExecuteSubscription = canExecuteSource
                .DistinctUntilChanged()
                .ObserveOn(scheduler)
                .Subscribe(b =>
                {
                    isCanExecute = b;
                    var handler = CanExecuteChanged;
                    if (handler != null) handler(this, EventArgs.Empty);
                });
        }

        /// <summary>Return current canExecute status.</summary>
        public bool CanExecute()
        {
            return isCanExecute;
        }

        /// <summary>Return current canExecute status. parameter is ignored.</summary>
        bool ICommand.CanExecute(object parameter)
        {
            return isCanExecute;
        }

        /// <summary>Push parameter to subscribers.</summary>
        public void Execute(T parameter)
        {
            trigger.OnNext(parameter);
        }

        /// <summary>Push parameter to subscribers.</summary>
        void ICommand.Execute(object parameter)
        {
            trigger.OnNext((T)parameter);
        }

        /// <summary>Subscribe execute.</summary>
        public IDisposable Subscribe(IObserver<T> observer)
        {
            return trigger.Subscribe(observer);
        }

        /// <summary>
        /// Stop all subscription and lock CanExecute is false.
        /// </summary>
        public void Dispose()
        {
            if (isDisposed) return;

            isDisposed = true;
            trigger.OnCompleted();
            trigger.Dispose();
            canExecuteSubscription.Dispose();

            if (isCanExecute)
            {
                isCanExecute = false;
                scheduler.Schedule(() =>
                {
                    isCanExecute = false;
                    var handler = CanExecuteChanged;
                    if (handler != null) handler(this, EventArgs.Empty);
                });
            }
        }
    }

    public static class ReactiveCommandExtensions
    {
        /// <summary>
        /// CanExecuteChanged is called from canExecute sequence on UIDispatcherScheduler.
        /// </summary>
        public static ReactiveCommand ToReactiveCommand(this IObservable<bool> canExecuteSource, bool initialValue = true)
        {
            return new ReactiveCommand(canExecuteSource, initialValue);
        }

        /// <summary>
        /// CanExecuteChanged is called from canExecute sequence on scheduler.
        /// </summary>
        public static ReactiveCommand ToReactiveCommand(this IObservable<bool> canExecuteSource, IScheduler scheduler, bool initialValue = true)
        {
            return new ReactiveCommand(canExecuteSource, scheduler, initialValue);
        }

        /// <summary>
        /// CanExecuteChanged is called from canExecute sequence on UIDispatcherScheduler.
        /// </summary>
        public static ReactiveCommand<T> ToReactiveCommand<T>(this IObservable<bool> canExecuteSource, bool initialValue = true)
        {
            return new ReactiveCommand<T>(canExecuteSource, initialValue);
        }

        /// <summary>
        /// CanExecuteChanged is called from canExecute sequence on scheduler.
        /// </summary>
        public static ReactiveCommand<T> ToReactiveCommand<T>(this IObservable<bool> canExecuteSource, IScheduler scheduler, bool initialValue = true)
        {
            return new ReactiveCommand<T>(canExecuteSource, scheduler, initialValue);
        }
    }
}