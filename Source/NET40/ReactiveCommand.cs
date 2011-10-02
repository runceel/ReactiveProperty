using System;
using System.Windows.Input;
using Codeplex.Reactive.Extensions;
#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#else
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
#endif

namespace Codeplex.Reactive
{
    public class ReactiveCommand : ReactiveCommand<object>
    {
        public ReactiveCommand()
            : base()
        { }

        public ReactiveCommand(IObservable<bool> canExecute, bool initialValue = true)
            : base(canExecute, initialValue)
        { }
    }

    public class ReactiveCommand<T> : IObservable<T>, ICommand, IDisposable
    {
        public event EventHandler CanExecuteChanged;

        readonly Subject<T> trigger = new Subject<T>();
        readonly IDisposable canExecuteSubscription;
        bool isCanExecute;

        public ReactiveCommand()
            : this(Observable.Never<bool>())
        { }

        public ReactiveCommand(IObservable<bool> canExecute, bool initialValue = true)
        {
            this.isCanExecute = initialValue;
            this.canExecuteSubscription = canExecute
                .DistinctUntilChanged()
                .ObserveOnUIDispatcher()
                .Subscribe(b =>
                {
                    isCanExecute = b;
                    var handler = CanExecuteChanged;
                    if (handler != null) handler(this, EventArgs.Empty);
                });
        }

        public bool CanExecute(object parameter)
        {
            return isCanExecute;
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

            if (isCanExecute)
            {
                isCanExecute = false;
                UIDispatcherScheduler.Default.Schedule(() =>
                {
                    var handler = CanExecuteChanged;
                    if (handler != null) handler(this, EventArgs.Empty);
                });
            }
        }
    }

    public static class ReactiveCommandExtensions
    {
        public static ReactiveCommand ToReactiveCommand(this IObservable<bool> canExecuteSource, bool initialValue = true)
        {
            return new ReactiveCommand(canExecuteSource, initialValue);
        }

        public static ReactiveCommand<T> ToReactiveCommand<T>(this IObservable<bool> canExecuteSource, bool initialValue = true)
        {
            return new ReactiveCommand<T>(canExecuteSource, initialValue);
        }
    }
}