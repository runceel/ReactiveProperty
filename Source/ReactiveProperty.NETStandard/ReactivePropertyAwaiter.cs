using Reactive.Bindings;
using Reactive.Bindings.Internals;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Reactive.Bindings
{
    // Reusable
    public class ReactivePropertyAwaiter<T> : ICriticalNotifyCompletion
    {
        T result;
        object continuation; // Action or Queue<Action>
        Queue<(int, T)> queueValues;
        bool running;
        int waitingContinuationCount;

        public bool IsCompleted => false;

        public T GetResult()
        {
            return result;
        }

        public void InvokeContinuation(ref T value)
        {
            if (continuation == null) return;

            if (continuation is Action act)
            {
                this.result = value;
                continuation = null;
                act();
            }
            else
            {
                if (waitingContinuationCount == 0) return;

                var q = (Queue<Action>)continuation;
                if (queueValues == null) queueValues = new Queue<(int, T)>(4);
                queueValues.Enqueue((waitingContinuationCount, value));
                waitingContinuationCount = 0;

                if (!running)
                {
                    running = true;
                    try
                    {
                        while (queueValues.Count != 0)
                        {
                            var (runCount, v) = queueValues.Dequeue();
                            this.result = v;
                            for (int i = 0; i < runCount; i++)
                            {
                                q.Dequeue().Invoke();
                            }
                        }
                    }
                    finally
                    {
                        running = false;
                    }
                }
            }
        }

        public void OnCompleted(Action continuation)
        {
            UnsafeOnCompleted(continuation);
        }

        public void UnsafeOnCompleted(Action action)
        {
            if (continuation == null)
            {
                continuation = action;
                return;
            }
            else
            {
                if (continuation is Action act)
                {
                    var q = new Queue<Action>(4);
                    q.Enqueue(act);
                    q.Enqueue(action);
                    continuation = q;
                    waitingContinuationCount = 2;
                    return;
                }
                else
                {
                    ((Queue<Action>)continuation).Enqueue(action);
                    waitingContinuationCount++;
                }
            }
        }
    }

    public static class ReactivePropertyAsyncExtensions
    {
        public static ReactivePropertyAwaiter<T> GetAwaiter<T>(this IReactiveProperty<T> source)
        {
            if (source is ReactiveProperty<T> prop)
            {
                return prop.GetAwaiter();
            }
            else if (source is ReactivePropertySlim<T> prop2)
            {
                return prop2.GetAwaiter();
            }
            else if (source is ReadOnlyReactivePropertySlim<T> prop3)
            {
                return prop3.GetAwaiter();
            }
            else if (source is ReadOnlyReactiveProperty<T> prop4)
            {
                return prop4.GetAwaiter();
            }

            throw new NotSupportedException();
        }

        public static ReactivePropertyAwaiter<T> GetAwaiter<T>(this IReadOnlyReactiveProperty<T> source)
        {
            if (source is ReactiveProperty<T> prop)
            {
                return prop.GetAwaiter();
            }
            else if (source is ReactivePropertySlim<T> prop2)
            {
                return prop2.GetAwaiter();
            }
            else if (source is ReadOnlyReactivePropertySlim<T> prop3)
            {
                return prop3.GetAwaiter();
            }
            else if (source is ReadOnlyReactiveProperty<T> prop4)
            {
                return prop4.GetAwaiter();
            }

            throw new NotSupportedException();
        }

        // for Task.WhenAll support.

        public static async Task<T> WaitUntilValueChangedAsync<T>(this IReactiveProperty<T> source)
        {
            return await source;
        }

        public static async Task<T> WaitUntilValueChangedAsync<T>(this IReadOnlyReactiveProperty<T> source)
        {
            return await source;
        }

        public static async Task<T> WaitUntilValueChangedAsync<T>(this ReactiveCommand<T> command)
        {
            return await command;
        }

        public static async Task<T> WaitUntilValueChangedAsync<T>(this AsyncReactiveCommand<T> command)
        {
            return await command;
        }
    }
}
