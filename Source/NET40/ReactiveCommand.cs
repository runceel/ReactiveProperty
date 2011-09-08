using System;
using System.Reactive.Subjects;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using System.Diagnostics.CodeAnalysis;

namespace Codeplex.Reactive
{
    public class ReactiveCommand<T> : IObservable<T>, ICommand, IDisposable
    {
        public event EventHandler CanExecuteChanged;

        Func<T, bool> canExecute;
        Subject<T> trigger = new Subject<T>();

        public ReactiveCommand()
            : this(null)
        { }

        public ReactiveCommand(Func<T, bool> canExecute)
        {
            this.canExecute = canExecute;
        }

        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "This cannot be an event")]
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
        }
    }
}