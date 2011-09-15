using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;
using System.Reactive.Disposables;

namespace Codeplex.Reactive
{
    public class ReactiveCommand : ReactiveCommand<object>
    {
        public ReactiveCommand()
            : base()
        { }

        public ReactiveCommand(Func<object, bool> canExecute)
            : base(canExecute)
        { }

        public ReactiveCommand(IObservable<bool> canExecute, bool initialValue = true)
            : base(canExecute, initialValue)
        { }
    }

    public class ReactiveCommand<T> : IObservable<T>, ICommand, IDisposable
    {
        public event EventHandler CanExecuteChanged;

        readonly Func<T, bool> canExecute;
        readonly Subject<T> trigger = new Subject<T>();
        readonly IDisposable canExecuteSubscription;

        public ReactiveCommand()
            : this((Func<T, bool>)null)
        { }

        public ReactiveCommand(Func<T, bool> canExecute)
        {
            this.canExecute = canExecute;
            this.canExecuteSubscription = Disposable.Empty;
        }

        public ReactiveCommand(IObservable<bool> canExecute, bool initialValue = true)
        {
            var value = initialValue;
            this.canExecuteSubscription = canExecute
                .DistinctUntilChanged()
                .ObserveOnDispatcherEx()
                .Subscribe(b =>
                {
                    value = b;
                    RaiseCanExecuteChanged();
                });
            this.canExecute = _ => value;
        }

        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
        public void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter)
        {
            return (canExecute == null) ? true : canExecute((T)parameter);
        }

        public void Execute(object parameter)
        {
            trigger.OnNext((T)parameter);
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return trigger.Subscribe(observer);
        }

        public void Dispose()
        {
            trigger.Dispose();
            canExecuteSubscription.Dispose();
        }
    }

    public static class ReactiveCommandExtensions
    {
        public static ReactiveCommand ToReactiveCommand(this IObservable<bool> canExecuteSource)
        {
            return new ReactiveCommand(canExecuteSource);
        }

        public static ReactiveCommand ToReactiveCommand(this IObservable<bool> canExecuteSource, bool initialValue = true)
        {
            return new ReactiveCommand(canExecuteSource, initialValue);
        }

        public static ReactiveCommand<T> ToReactiveCommand<T>(this IObservable<bool> canExecuteSource)
        {
            return new ReactiveCommand<T>(canExecuteSource);
        }

        public static ReactiveCommand<T> ToReactiveCommand<T>(this IObservable<bool> canExecuteSource, bool initialValue = true)
        {
            return new ReactiveCommand<T>(canExecuteSource, initialValue);
        }
    }
}