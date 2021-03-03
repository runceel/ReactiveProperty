﻿using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;

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

        /// <summary>
        /// Push null to subscribers.
        /// </summary>
        public void Execute()
        {
            Execute(null);
        }

        /// <summary>
        /// Subscribe execute.
        /// </summary>
        public IDisposable Subscribe(Action onNext)
            => this.Subscribe(_ => onNext());
    }

    /// <summary>
    /// ICommand and IObservable&lt;T&gt; implementation class.
    /// </summary>
    /// <typeparam name="T">Type of command argument.</typeparam>
    public class ReactiveCommand<T> : IObservable<T>, ICommand, IDisposable
    {
        /// <summary>
        /// ICommand#CanExecuteChanged
        /// </summary>
        public event EventHandler? CanExecuteChanged;

        private Subject<T?> Trigger { get; } = new Subject<T?>();

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
            IsCanExecute = initialValue;
            Scheduler = scheduler;
            CanExecuteSubscription = canExecuteSource
                .DistinctUntilChanged()
                .ObserveOn(scheduler)
                .Subscribe(b =>
                {
                    IsCanExecute = b;
                    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                });
        }

        /// <summary>
        /// Return current canExecute status.
        /// </summary>
        public bool CanExecute() => IsCanExecute;

        /// <summary>
        /// Return current canExecute status. parameter is ignored.
        /// </summary>
        bool ICommand.CanExecute(object? parameter) => IsCanExecute;

        /// <summary>
        /// Push parameter to subscribers.
        /// </summary>
        public void Execute(T? parameter)
        {
            Trigger.OnNext(parameter);
        }

        /// <summary>
        /// Push parameter to subscribers.
        /// </summary>
        void ICommand.Execute(object? parameter) => Execute((T?)parameter);

        /// <summary>
        /// Subscribe execute.
        /// </summary>
        public IDisposable Subscribe(IObserver<T> observer) => Trigger.Subscribe(observer);

        /// <summary>
        /// Stop all subscription and lock CanExecute is false.
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

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
                    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                });
            }
        }
    }

    /// <summary>
    /// ReactiveCommand factory extension methods.
    /// </summary>
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

        /// <summary>
        /// Subscribe execute.
        /// </summary>
        /// <param name="self">ReactiveCommand</param>
        /// <param name="onNext">Action</param>
        /// <param name="postProcess">Handling of the subscription.</param>
        /// <returns>Same of self argument</returns>
        public static ReactiveCommand WithSubscribe(this ReactiveCommand self, Action onNext, Action<IDisposable>? postProcess = null)
        {
            var d = self.Subscribe(onNext);
            postProcess?.Invoke(d);
            return self;
        }

        /// <summary>
        /// Subscribe execute.
        /// </summary>
        /// <typeparam name="T">ReactiveCommand type argument.</typeparam>
        /// <param name="self">ReactiveCommand</param>
        /// <param name="onNext">Action</param>
        /// <param name="postProcess">Handling of the subscription.</param>
        /// <returns>Same of self argument</returns>
        public static ReactiveCommand<T> WithSubscribe<T>(this ReactiveCommand<T> self, Action<T> onNext, Action<IDisposable>? postProcess = null)
        {
            var d = self.Subscribe(onNext);
            postProcess?.Invoke(d);
            return self;
        }

        /// <summary>
        /// Subscribe execute.
        /// </summary>
        /// <param name="self">ReactiveCommand</param>
        /// <param name="onNext">Action</param>
        /// <param name="disposable">The return value of self.Subscribe(onNext)</param>
        /// <returns>Same of self argument</returns>
        public static ReactiveCommand WithSubscribe(this ReactiveCommand self, Action onNext, out IDisposable disposable)
        {
            disposable = self.Subscribe(onNext);
            return self;
        }

        /// <summary>
        /// Subscribe execute.
        /// </summary>
        /// <typeparam name="T">ReactiveCommand type argument.</typeparam>
        /// <param name="self">ReactiveCommand</param>
        /// <param name="onNext">Action</param>
        /// <param name="disposable">The return value of self.Subscribe(onNext)</param>
        /// <returns>Same of self argument</returns>
        public static ReactiveCommand<T> WithSubscribe<T>(this ReactiveCommand<T> self, Action<T> onNext, out IDisposable disposable)
        {
            disposable = self.Subscribe(onNext);
            return self;
        }
    }
}
