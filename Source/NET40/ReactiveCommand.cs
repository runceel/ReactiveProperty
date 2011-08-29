using System;
using System.Reactive.Subjects;
using System.Windows.Input;

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