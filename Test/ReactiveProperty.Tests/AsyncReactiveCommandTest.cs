using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Reactive.Bindings;

namespace ReactiveProperty.Tests
{
    [TestClass]
    public class AsyncReactiveCommandTest
    {
        [TestMethod]
        public void DefaultUseCase()
        {
            var source = new Subject<bool>();
            var taskSource = new TaskCompletionSource<object>();
            var canExecuteChangedCounter = 0;

            var command = source.ToAsyncReactiveCommand();
            command.Subscribe(_ => taskSource.Task);
            command.CanExecuteChanged += (_, __) => canExecuteChangedCounter++;

            canExecuteChangedCounter.Is(0);
            command.CanExecute().Is(false);

            source.OnNext(true);
            command.CanExecute().Is(true);
            canExecuteChangedCounter.Is(1);

            command.Execute();
            command.CanExecute().Is(false);
            canExecuteChangedCounter.Is(2);

            taskSource.SetResult(null);
            command.CanExecute().Is(true);
            canExecuteChangedCounter.Is(3);

            command.Dispose();
            command.CanExecute().Is(false);
        }

        [TestMethod]
        public void ShredReactivePropertyCase()
        {
            var rp = new ReactiveProperty<bool>(false);

            var taskSource1 = new TaskCompletionSource<object>();
            var taskSource2 = new TaskCompletionSource<object>();

            var canExecutedCounter1 = 0;
            var canExecutedCounter2 = 0;

            var command1 = rp.ToAsyncReactiveCommand();
            command1.CanExecuteChanged += (_, __) => canExecutedCounter1++;
            command1.Subscribe(_ => taskSource1.Task);
            var command2 = rp.ToAsyncReactiveCommand();
            command2.CanExecuteChanged += (_, __) => canExecutedCounter2++;
            command2.Subscribe(_ => taskSource2.Task);

            command1.CanExecute().IsFalse();
            command2.CanExecute().IsFalse();

            rp.Value = true;
            command1.CanExecute().IsTrue();
            command2.CanExecute().IsTrue();
            canExecutedCounter1.Is(1);
            canExecutedCounter2.Is(1);

            command1.Execute();
            command1.CanExecute().IsFalse();
            canExecutedCounter1.Is(2);
            command2.CanExecute().IsFalse();
            canExecutedCounter1.Is(2);

            taskSource1.SetResult(null);
            command1.CanExecute().IsTrue();
            canExecutedCounter1.Is(3);
            command2.CanExecute().IsTrue();
            canExecutedCounter1.Is(3);

            command2.Execute();
            command1.CanExecute().IsFalse();
            canExecutedCounter1.Is(4);
            command2.CanExecute().IsFalse();
            canExecutedCounter1.Is(4);

            taskSource2.SetResult(null);
            command1.CanExecute().IsTrue();
            canExecutedCounter1.Is(5);
            command2.CanExecute().IsTrue();
            canExecutedCounter1.Is(5);

        }

        [TestMethod]
        public void DefaultConstructor()
        {
            var command = new AsyncReactiveCommand();
            var task1 = new TaskCompletionSource<object>();
            var task2 = new TaskCompletionSource<object>();

            command.Subscribe(_ => task1.Task);
            command.Subscribe(_ => task2.Task);

            command.Execute();
            command.CanExecute().IsFalse();
            task1.SetResult(null);
            command.CanExecute().IsFalse();
            task2.SetResult(null);
            command.CanExecute().IsTrue();
        }

        [TestMethod]
        public void RemoveSubscription()
        {
            var command = new AsyncReactiveCommand();
            var task1 = new TaskCompletionSource<object>();
            var task2 = new TaskCompletionSource<object>();

            var subscription1 = command.Subscribe(_ => task1.Task);
            var subscription2 = command.Subscribe(_ => task2.Task);

            subscription2.Dispose();

            command.Execute();
            command.CanExecute().IsFalse();
            task1.SetResult(null);
            command.CanExecute().IsTrue();
        }
    }
}
