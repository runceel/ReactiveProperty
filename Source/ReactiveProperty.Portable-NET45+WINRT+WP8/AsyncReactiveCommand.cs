using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Reactive.Bindings
{
    /// <summary>
    /// Represents AsyncReactiveCommand&lt;object&gt;
    /// </summary>
    public class AsyncReactiveCommand : AsyncReactiveCommand<object>
    {
        public AsyncReactiveCommand()
            : base()
        {

        }

        public AsyncReactiveCommand(IObservable<bool> canExecuteSource, bool initialValue = true)
            : base(canExecuteSource, initialValue)
        {
        }

        /// <summary>Push null to subscribers.</summary>
        public void Execute()
        {
            Execute(null);
        }
    }

    public class AsyncReactiveCommand<T> : ICommand, IDisposable
    {
        static readonly Task EmptyTask = Task.FromResult<object>(null);

        public event EventHandler CanExecuteChanged;

        readonly object gate = new object();

        readonly ReactiveProperty<bool> isExecuting;
        readonly IDisposable sourceSubscription;
        bool canExecute = true;
        bool isDisposed = false;
        Notifiers.ImmutableList<Func<T, Task>> asyncActions = Notifiers.ImmutableList<Func<T, Task>>.Empty;

        /// <summary>
        /// CanExecute is automatically changed when executing to false and finished to true.
        /// </summary>
        public AsyncReactiveCommand()
        {
            this.isExecuting = new ReactiveProperty<bool>(false);
            this.sourceSubscription = this.isExecuting.Subscribe(x =>
            {
                canExecute = !x;
                this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            });
        }

        /// <summary>
        /// CanExecute is automatically changed when executing to false and finished to true.
        /// The source is shared between other AsyncReactiveCommand.
        /// </summary>
        public AsyncReactiveCommand(IObservable<bool> canExecuteSource, bool initialValue = true)
        {
            this.isExecuting = new ReactiveProperty<bool>(false);
            this.canExecute = initialValue;
            this.sourceSubscription = isExecuting.CombineLatest(canExecuteSource, (x, y) => !x && y)
                .DistinctUntilChanged()
                .Subscribe(x =>
                {
                    canExecute = x;
                    this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                });
        }

        /// <summary>Return current canExecute status.</summary>
        public bool CanExecute()
        {
            return isDisposed ? false : canExecute;
        }

        /// <summary>Return current canExecute status. parameter is ignored.</summary>
        bool ICommand.CanExecute(object parameter)
        {
            return isDisposed ? false : canExecute;
        }

        /// <summary>Push parameter to subscribers, when executing CanExecuting is changed to false.</summary>
        public async void Execute(T parameter)
        {
            if (canExecute)
            {
                isExecuting.Value = true;
                var a = asyncActions.Data;
                if (a.Length == 1)
                {
                    try
                    {
                        var asyncState = a[0].Invoke(parameter) ?? EmptyTask;
                        await asyncState;
                    }
                    finally
                    {
                        isExecuting.Value = false;
                    }
                }
                else
                {
                    var xs = new Task[a.Length];
                    try
                    {
                        for (int i = 0; i < a.Length; i++)
                        {
                            xs[i] = a[i].Invoke(parameter) ?? EmptyTask;
                        }

                        await Task.WhenAll(xs);
                    }
                    finally
                    {
                        isExecuting.Value = false;
                    }
                }
            }
        }

        /// <summary>Push parameter to subscribers, when executing CanExecuting is changed to false.</summary>
        void ICommand.Execute(object parameter) => Execute((T)parameter);

        /// <summary>Subscribe execute.</summary>
        public IDisposable Subscribe(Func<T, Task> asyncAction)
        {
            lock (gate)
            {
                asyncActions = asyncActions.Add(asyncAction);
            }

            return new Subscription(this, asyncAction);
        }

        /// <summary>
        /// Stop all subscription and lock CanExecute is false.
        /// </summary>
        public void Dispose()
        {
            if (isDisposed) return;

            isDisposed = true;
            this.sourceSubscription.Dispose();
            if (canExecute)
            {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        class Subscription : IDisposable
        {
            readonly AsyncReactiveCommand<T> parent;
            readonly Func<T, Task> asyncAction;

            public Subscription(AsyncReactiveCommand<T> parent, Func<T, Task> asyncAction)
            {
                this.parent = parent;
                this.asyncAction = asyncAction;
            }

            public void Dispose()
            {
                lock (parent.gate)
                {
                    parent.asyncActions = parent.asyncActions.Remove(asyncAction);
                }
            }
        }
    }

    public static class AsyncReactiveCommandExtensions
    {
        /// <summary>
        /// CanExecute is automatically changed when executing to false and finished to true.
        /// </summary>
        public static AsyncReactiveCommand ToAsyncReactiveCommand(this IObservable<bool> canExecuteSource, bool initialValue = true) =>
            new AsyncReactiveCommand(canExecuteSource, initialValue);

        /// <summary>
        /// CanExecute is automatically changed when executing to false and finished to true.
        /// </summary>
        public static AsyncReactiveCommand<T> ToAsyncReactiveCommand<T>(this IObservable<bool> canExecuteSource, bool initialValue = true) =>
            new AsyncReactiveCommand<T>(canExecuteSource, initialValue);
    }
}