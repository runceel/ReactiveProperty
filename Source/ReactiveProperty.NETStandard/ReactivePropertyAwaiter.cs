using Reactive.Bindings;
using ReactiveProperty.Internals;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Reactive.Bindings
{
    public class ReactivePropertyAwaiter<T> : ICriticalNotifyCompletion
    {
        object continuationField;
        T value;

        public bool IsCompleted => false;

        public void InvokeContinuation(ref T value)
        {
            this.value = value;
            CompletionHelper.InvokeContinuation(ref continuationField);
        }

        public T GetResult()
        {
            return value;
        }

        public void OnCompleted(Action continuation)
        {
            CompletionHelper.AppendContinuation(ref continuationField, continuation);
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            CompletionHelper.AppendContinuation(ref continuationField, continuation);
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

        public static async Task<T> ToTask<T>(this IReactiveProperty<T> source)
        {
            return await source;
        }

        public static async Task<T> ToTask<T>(this IReadOnlyReactiveProperty<T> source)
        {
            return await source;
        }

        public static async Task<T> ToTask<T>(this ReactiveCommand<T> command)
        {
            return await command;
        }

        public static async Task<T> ToTask<T>(this AsyncReactiveCommand<T> command)
        {
            return await command;
        }

    }
}
