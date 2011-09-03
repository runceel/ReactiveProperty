using System;
using System.Reactive.Subjects;

namespace Codeplex.Reactive.Notifier
{
    public class ProgressNotifier : IObservable<int>, IProgress<int>
    {
        readonly BehaviorSubject<int> notifier;
        readonly int max;

        public int Current { get; private set; }
        public bool IsCompleted { get; private set; }

        public ProgressNotifier(int max)
        {
            this.max = max;
            this.Current = 0;
            notifier = new BehaviorSubject<int>(0);
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