using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Reactive.Bindings
{
    /// <summary>
    /// Represents AsyncReactiveCommand&lt;object&gt;
    /// </summary>
    public class AsyncReactiveCommand : AsyncReactiveCommand<object>
    {
        public AsyncReactiveCommand(bool initialValue = true)
            : this(new ReactiveProperty<bool>(initialValue))
        {

        }

        public AsyncReactiveCommand(IReactiveProperty<bool> sharedCanExecuteSource) : base(sharedCanExecuteSource)
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
        readonly IReactiveProperty<bool> canExecute;
        Notifiers.ImmutableList<Func<T, Task>> asyncActions = Notifiers.ImmutableList<Func<T, Task>>.Empty;
        bool isDisposed = false;

        /// <summary>
        /// CanExecute is automatically changed when executing to false and finished to true.
        /// </summary>
        public AsyncReactiveCommand(bool initialValue = true)
            : this(new ReactiveProperty<bool>(initialValue))
        {

        }

        /// <summary>
        /// CanExecute is automatically changed when executing to false and finished to true.
        /// The source is shared between other AsyncReactiveCommand.
        /// </summary>
        public AsyncReactiveCommand(IReactiveProperty<bool> sharedCanExecuteSource)
        {
            this.canExecute = sharedCanExecuteSource;
        }

        /// <summary>Return current canExecute status.</summary>
        public bool CanExecute() => isDisposed ? false : canExecute.Value;

        /// <summary>Return current canExecute status. parameter is ignored.</summary>
        bool ICommand.CanExecute(object parameter) => isDisposed ? false : canExecute.Value;

        /// <summary>Push parameter to subscribers, when executing CanExecuting is changed to false.</summary>
        public async void Execute(T parameter)
        {
            if (canExecute.Value)
            {
                canExecute.Value = false;
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
                        canExecute.Value = true;
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
                        canExecute.Value = true;
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
            if (canExecute.Value)
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
        /// The source is shared between other AsyncReactiveCommand.
        /// </summary>
        public static AsyncReactiveCommand ToAsyncReactiveCommand(this IReactiveProperty<bool> sharedCanExecuteSource) =>
            new AsyncReactiveCommand(sharedCanExecuteSource);

        /// <summary>
        /// CanExecute is automatically changed when executing to false and finished to true.
        /// The source is shared between other AsyncReactiveCommand.
        /// </summary>
        public static AsyncReactiveCommand<T> ToAsyncReactiveCommand<T>(this IReactiveProperty<bool> sharedCanExecuteSource) =>
            new AsyncReactiveCommand<T>(sharedCanExecuteSource);
    }
}