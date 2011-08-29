using System;
using System.Reactive.Subjects;

namespace Codeplex.Reactive.Notifier
{
    // this interface borrow from AsyncCTP
    // will replace in the future

    /// <summary>
    /// Defines a provider for progress updates.
    /// </summary>
    /// <typeparam name="T">The type of progress update value.</typeparam>
    public interface IProgress<in T>
    {
        /// <summary>
        /// Reports a progress update.
        /// </summary>
        /// <param name="value">The value of the updated progress.</param>
        void Report(T value);
    }

    public class ProgressNotifier : IObservable<int>, IProgress<int>
    {
        readonly BehaviorSubject<int> notifier;
        readonly int max;

        public int Current { get; private set; }
        public bool IsCompleted { get; private set; }

        public ProgressNotifier(int max = 100, int start = 0)
        {
            this.max = max;
            Current = start;
            notifier = new BehaviorSubject<int>(start);
        }

        /// <summary>Report Progress(send to OnNext)</summary>
        public void Report(int value)
        {
            if (!IsCompleted)
            {
                Current = value;
                notifier.OnNext(value);
                if (value >= max) Complete();
            }
        }

        /// <summary>Report Cancel(send to OnError)</summary>
        public void Cancel(string message = "")
        {
            if (!IsCompleted)
            {
                IsCompleted = true;
                notifier.OnError(new OperationCanceledException(message));
                notifier.Dispose();
            }
        }
        /// <summary>Force Report Complete(send to OnCompleted)</summary>
        public void Complete()
        {
            if (!IsCompleted)
            {
                IsCompleted = true;
                notifier.OnCompleted();
                notifier.Dispose();
            }
        }

        public IDisposable Subscribe(IObserver<int> observer)
        {
            return notifier.Subscribe(observer);
        }
    }
}