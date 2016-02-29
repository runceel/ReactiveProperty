using System;
using System.Windows.Input;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;

namespace Reactive.Bindings
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

        private Subject<T> Trigger { get; } = new Subject<T>();
        private IDisposable CanExecuteSubscription { get; }
        private IScheduler Scheduler { get; }
        private bool IsCanExecute { get; set; }
        private bool IsDisposed { get; set; } = false;

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
        /// CanExecuteChanged is called from canExecute sequence on ReactivePropertyScheduler.
        /// </summary>
        public ReactiveCommand(IObservable<bool> canExecuteSource, bool initialValue = true)
            : this(canExecuteSource, ReactivePropertyScheduler.Default, initialValue)
        {
        }

        /// <summary>
        /// CanExecuteChanged is called from canExecute sequence on scheduler.
        /// </summary>
        public ReactiveCommand(IObservable<bool> canExecuteSource, IScheduler scheduler, bool initialValue = true)
        {
            this.IsCanExecute = initialValue;
            this.Scheduler = scheduler;
            this.CanExecuteSubscription = canExecuteSource
                .DistinctUntilChanged()
                .ObserveOn(scheduler)
                .Subscribe(b =>
                {
                    IsCanExecute = b;
                    this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                });
        }

        /// <summary>Return current canExecute status.</summary>
        public bool CanExecute() => IsCanExecute;

        /// <summary>Return current canExecute status. parameter is ignored.</summary>
        bool ICommand.CanExecute(object parameter) => IsCanExecute;

        /// <summary>Push parameter to subscribers.</summary>
        public void Execute(T parameter) => Trigger.OnNext(parameter);

        /// <summary>Push parameter to subscribers.</summary>
        void ICommand.Execute(object parameter) => Trigger.OnNext((T)parameter);

        /// <summary>Subscribe execute.</summary>
        public IDisposable Subscribe(IObserver<T> observer) => Trigger.Subscribe(observer);

        /// <summary>
        /// Stop all subscription and lock CanExecute is false.
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed) return;

            IsDisposed = true;
            Trigger.OnCompleted();
            Trigger.Dispose();
            CanExecuteSubscription.Dispose();

            if (IsCanExecute)
            {
                IsCanExecute = false;
                Scheduler.Schedule(() =>
                {
                    IsCanExecute = false;
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
        public static ReactiveCommand ToReactiveCommand(this IObservable<bool> canExecuteSource, bool initialValue = true) => 
            new ReactiveCommand(canExecuteSource, initialValue);

        /// <summary>
        /// CanExecuteChanged is called from canExecute sequence on scheduler.
        /// </summary>
        public static ReactiveCommand ToReactiveCommand(this IObservable<bool> canExecuteSource, IScheduler scheduler, bool initialValue = true) =>
            new ReactiveCommand(canExecuteSource, scheduler, initialValue);

        /// <summary>
        /// CanExecuteChanged is called from canExecute sequence on UIDispatcherScheduler.
        /// </summary>
        public static ReactiveCommand<T> ToReactiveCommand<T>(this IObservable<bool> canExecuteSource, bool initialValue = true) =>
            new ReactiveCommand<T>(canExecuteSource, initialValue);

        /// <summary>
        /// CanExecuteChanged is called from canExecute sequence on scheduler.
        /// </summary>
        public static ReactiveCommand<T> ToReactiveCommand<T>(this IObservable<bool> canExecuteSource, IScheduler scheduler, bool initialValue = true) =>
            new ReactiveCommand<T>(canExecuteSource, scheduler, initialValue);
    }
}