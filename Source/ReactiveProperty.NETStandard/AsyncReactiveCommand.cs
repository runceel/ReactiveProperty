﻿using System;
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
        /// <summary>
        /// CanExecute is automatically changed when executing to false and finished to true.
        /// </summary>
        public AsyncReactiveCommand()
            : base()
        {
        }

        /// <summary>
        /// CanExecute is automatically changed when executing to false and finished to true.
        /// </summary>
        public AsyncReactiveCommand(IObservable<bool> canExecuteSource)
            : base(canExecuteSource)
        {
        }

        /// <summary>
        /// CanExecute is automatically changed when executing to false and finished to true.
        /// </summary>
        public AsyncReactiveCommand(IObservable<bool> canExecuteSource, IReactiveProperty<bool> sharedCanExecute)
            : base(canExecuteSource, sharedCanExecute)
        {
        }

        /// <summary>
        /// CanExecute is automatically changed when executing to false and finished to true. The
        /// source is shared between other AsyncReactiveCommand.
        /// </summary>
        public AsyncReactiveCommand(IReactiveProperty<bool> sharedCanExecute)
            : base(sharedCanExecute)
        {
        }

        /// <summary>
        /// Push null to subscribers.
        /// </summary>
        public void Execute() => Execute(null);

        /// <summary>
        /// Push null to subscribers.
        /// </summary>
        public Task ExecuteAsync() => ExecuteAsync(null);

        /// <summary>
        /// Subscribe execute.
        /// </summary>
        public IDisposable Subscribe(Func<Task> asyncAction)
            => Subscribe(async _ => await asyncAction());
    }

    /// <summary>
    /// Async version ReactiveCommand
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AsyncReactiveCommand<T> : ICommand, IDisposable
    {
        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        /// <returns></returns>
        public event EventHandler? CanExecuteChanged;

        private readonly object gate = new object();
        private readonly IReactiveProperty<bool> canExecute;
        private readonly IDisposable sourceSubscription;
        private bool isCanExecute;
        private bool isDisposed = false;
        private Notifiers.ImmutableList<Func<T, Task>> asyncActions = Notifiers.ImmutableList<Func<T, Task>>.Empty;

        /// <summary>
        /// CanExecute is automatically changed when executing to false and finished to true.
        /// </summary>
        public AsyncReactiveCommand()
        {
            canExecute = new ReactivePropertySlim<bool>(true);
            sourceSubscription = canExecute.Subscribe(x =>
            {
                isCanExecute = x;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            });
        }

        /// <summary>
        /// CanExecute is automatically changed when executing to false and finished to true.
        /// </summary>
        public AsyncReactiveCommand(IObservable<bool> canExecuteSource)
            : this(canExecuteSource, null)
        {
        }

        /// <summary>
        /// CanExecute is automatically changed when executing to false and finished to true.
        /// </summary>
        public AsyncReactiveCommand(IObservable<bool> canExecuteSource, IReactiveProperty<bool>? sharedCanExecute)
        {
            canExecute = sharedCanExecute ?? new ReactivePropertySlim<bool>(true);
            sourceSubscription = canExecute.CombineLatest(canExecuteSource, (x, y) => x && y)
                .DistinctUntilChanged()
                .Subscribe(x =>
                {
                    isCanExecute = x;
                    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                });
        }

        /// <summary>
        /// CanExecute is automatically changed when executing to false and finished to true. The
        /// source is shared between other AsyncReactiveCommand.
        /// </summary>
        public AsyncReactiveCommand(IReactiveProperty<bool> sharedCanExecute)
        {
            canExecute = sharedCanExecute;
            sourceSubscription = canExecute.Subscribe(x =>
            {
                isCanExecute = x;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            });
        }

        /// <summary>
        /// Return current canExecute status.
        /// </summary>
        public bool CanExecute()
        {
            return isDisposed ? false : isCanExecute;
        }

        /// <summary>
        /// Return current canExecute status. parameter is ignored.
        /// </summary>
        bool ICommand.CanExecute(object? parameter)
        {
            return isDisposed ? false : isCanExecute;
        }

        /// <summary>
        /// Push parameter to subscribers, when executing CanExecuting is changed to false.
        /// </summary>
        public async void Execute(T? parameter) => await ExecuteAsync(parameter);

        /// <summary>
        /// Push parameter to subscribers, when executing CanExecuting is changed to false.
        /// </summary>
        public async Task ExecuteAsync(T? parameter)
        {
            if (isCanExecute)
            {
                canExecute.Value = false;
                var a = asyncActions.Data;

                if (a.Length == 1)
                {
                    try
                    {
                        var asyncState = a[0].Invoke(parameter) ?? Task.CompletedTask;
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
                        for (var i = 0; i < a.Length; i++)
                        {
                            xs[i] = a[i].Invoke(parameter) ?? Task.CompletedTask;
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

        /// <summary>
        /// Push parameter to subscribers, when executing CanExecuting is changed to false.
        /// </summary>
        void ICommand.Execute(object? parameter) => Execute((T?)parameter);

        /// <summary>
        /// Subscribe execute.
        /// </summary>
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
            if (isDisposed)
            {
                return;
            }

            isDisposed = true;
            sourceSubscription.Dispose();
            if (isCanExecute)
            {
                isCanExecute = false;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private class Subscription : IDisposable
        {
            private readonly AsyncReactiveCommand<T> parent;
            private readonly Func<T, Task> asyncAction;

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

    /// <summary>
    /// AsyncReactiveCommand factory and extension methods.
    /// </summary>
    public static class AsyncReactiveCommandExtensions
    {
        /// <summary>
        /// CanExecute is automatically changed when executing to false and finished to true.
        /// </summary>
        public static AsyncReactiveCommand ToAsyncReactiveCommand(this IObservable<bool> canExecuteSource) =>
            new AsyncReactiveCommand(canExecuteSource);

        /// <summary>
        /// CanExecute is automatically changed when executing to false and finished to true.
        /// </summary>
        public static AsyncReactiveCommand ToAsyncReactiveCommand(this IObservable<bool> canExecuteSource, IReactiveProperty<bool> sharedCanExecute) =>
            new AsyncReactiveCommand(canExecuteSource, sharedCanExecute);

        /// <summary>
        /// CanExecute is automatically changed when executing to false and finished to true.
        /// </summary>
        public static AsyncReactiveCommand<T> ToAsyncReactiveCommand<T>(this IObservable<bool> canExecuteSource) =>
            new AsyncReactiveCommand<T>(canExecuteSource);

        /// <summary>
        /// CanExecute is automatically changed when executing to false and finished to true.
        /// </summary>
        public static AsyncReactiveCommand<T> ToAsyncReactiveCommand<T>(this IObservable<bool> canExecuteSource, IReactiveProperty<bool> sharedCanExecute) =>
            new AsyncReactiveCommand<T>(canExecuteSource, sharedCanExecute);

        /// <summary>
        /// CanExecute is automatically changed when executing to false and finished to true. The
        /// source is shared between other AsyncReactiveCommand.
        /// </summary>
        public static AsyncReactiveCommand ToAsyncReactiveCommand(this IReactiveProperty<bool> sharedCanExecute) =>
            new AsyncReactiveCommand(sharedCanExecute);

        /// <summary>
        /// CanExecute is automatically changed when executing to false and finished to true. The
        /// source is shared between other AsyncReactiveCommand.
        /// </summary>
        public static AsyncReactiveCommand<T> ToAsyncReactiveCommand<T>(this IReactiveProperty<bool> sharedCanExecute) =>
            new AsyncReactiveCommand<T>(sharedCanExecute);

        /// <summary>
        /// Subscribe execute.
        /// </summary>
        /// <param name="self">AsyncReactiveCommand</param>
        /// <param name="asyncAction">Action</param>
        /// <param name="postProcess">Handling of the subscription.</param>
        /// <returns>Same of self argument</returns>
        public static AsyncReactiveCommand WithSubscribe(this AsyncReactiveCommand self, Func<Task> asyncAction, Action<IDisposable>? postProcess = null)
        {
            var d = self.Subscribe(asyncAction);
            postProcess?.Invoke(d);
            return self;
        }

        /// <summary>
        /// Subscribe execute.
        /// </summary>
        /// <typeparam name="T">AsyncReactiveCommand type argument.</typeparam>
        /// <param name="self">AsyncReactiveCommand</param>
        /// <param name="asyncAction">Action</param>
        /// <param name="postProcess">Handling of the subscription.</param>
        /// <returns>Same of self argument</returns>
        public static AsyncReactiveCommand<T> WithSubscribe<T>(this AsyncReactiveCommand<T> self, Func<T, Task> asyncAction, Action<IDisposable>? postProcess = null)
        {
            var d = self.Subscribe(asyncAction);
            postProcess?.Invoke(d);
            return self;
        }

        /// <summary>
        /// Subscribe execute.
        /// </summary>
        /// <param name="self">AsyncReactiveCommand</param>
        /// <param name="asyncAction">Action</param>
        /// <param name="disposable">The return value of self.Subscribe(asyncAction)</param>
        /// <returns>Same of self argument</returns>
        public static AsyncReactiveCommand WithSubscribe(this AsyncReactiveCommand self, Func<Task> asyncAction, out IDisposable disposable)
        {
            disposable = self.Subscribe(asyncAction);
            return self;
        }

        /// <summary>
        /// Subscribe execute.
        /// </summary>
        /// <typeparam name="T">AsyncReactiveCommand type argument.</typeparam>
        /// <param name="self">AsyncReactiveCommand</param>
        /// <param name="asyncAction">Action</param>
        /// <param name="disposable">The return value of self.Subscribe(asyncAction)</param>
        /// <returns>Same of self argument</returns>
        public static AsyncReactiveCommand<T> WithSubscribe<T>(this AsyncReactiveCommand<T> self, Func<T, Task> asyncAction, out IDisposable disposable)
        {
            disposable = self.Subscribe(asyncAction);
            return self;
        }
    }
}
