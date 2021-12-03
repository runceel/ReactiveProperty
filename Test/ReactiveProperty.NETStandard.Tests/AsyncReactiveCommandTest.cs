using System;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reactive.Bindings;

namespace ReactiveProperty.Tests;

[TestClass]
public class AsyncReactiveCommandTest
{
    [TestMethod]
    public void ExecuteAsync()
    {
        var tcs = new TaskCompletionSource<bool>();
        var cmd = new AsyncReactiveCommand().WithSubscribe(async () =>
        {
            await tcs.Task;
        });

        Task taskForExecuteAsync = cmd.ExecuteAsync();

        taskForExecuteAsync.IsCompleted.IsFalse();
        tcs.SetResult(true);
        taskForExecuteAsync.IsCompleted.IsTrue();
    }

    [TestMethod]
    public void ExecuteAsyncForGeneric()
    {
        var tcs = new TaskCompletionSource<bool>();
        var expectedCommandParameter = new object();
        var actualCommandParameter = default(object);
        var cmd = new AsyncReactiveCommand<object>().WithSubscribe(async x =>
        {
            actualCommandParameter = x;
            await tcs.Task;
        });

        Task taskForExecuteAsync = cmd.ExecuteAsync(expectedCommandParameter);

        taskForExecuteAsync.IsCompleted.IsFalse();
        tcs.SetResult(true);
        taskForExecuteAsync.IsCompleted.IsTrue();
        actualCommandParameter.IsSameReferenceAs(expectedCommandParameter);
    }

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

    [TestMethod]
    public void SubscribeAction()
    {
        var command = new AsyncReactiveCommand();
        var task1 = new TaskCompletionSource<object>();
        var task2 = new TaskCompletionSource<object>();
        Func<Task> asyncAction1 = () => task1.Task;
        Func<Task> asyncAction2 = () => task2.Task;

        command.Subscribe(asyncAction1);
        command.Subscribe(asyncAction2);

        command.Execute();
        command.CanExecute().IsFalse();
        task1.SetResult(null);
        command.CanExecute().IsFalse();
        task2.SetResult(null);
        command.CanExecute().IsTrue();
    }

    [TestMethod]
    public void WithSubscribe()
    {
        var task1 = new TaskCompletionSource<object>();
        var task2 = new TaskCompletionSource<object>();

        var subscription = new CompositeDisposable();
        var command = new AsyncReactiveCommand()
            .WithSubscribe(() => task1.Task)
            .WithSubscribe(() => task2.Task, subscription.Add);

        subscription.Dispose();

        command.Execute();
        command.CanExecute().IsFalse();
        task1.SetResult(null);
        command.CanExecute().IsTrue();
    }

    [TestMethod]
    public void WithSubscribeGenericVersion()
    {
        var task1 = new TaskCompletionSource<object>();
        var task2 = new TaskCompletionSource<object>();

        var subscription = new CompositeDisposable();
        var command = new AsyncReactiveCommand<string>()
            .WithSubscribe(_ => task1.Task)
            .WithSubscribe(_ => task2.Task, subscription.Add);

        subscription.Dispose();

        command.Execute("x");
        command.CanExecute().IsFalse();
        task1.SetResult(null);
        command.CanExecute().IsTrue();
    }

    [TestMethod]
    public void WithSubscribeDisposableOverride()
    {
        var task1 = new TaskCompletionSource<object>();
        var task2 = new TaskCompletionSource<object>();

        var command = new AsyncReactiveCommand()
            .WithSubscribe(() => task1.Task)
            .WithSubscribe(() => task2.Task, out var subscription);

        subscription.Dispose();

        command.Execute();
        command.CanExecute().IsFalse();
        task1.SetResult(null);
        command.CanExecute().IsTrue();
    }

    [TestMethod]
    public void WithSubscribeDisposableOverrideGenericVersion()
    {
        var task1 = new TaskCompletionSource<object>();
        var task2 = new TaskCompletionSource<object>();

        var command = new AsyncReactiveCommand<string>()
            .WithSubscribe(_ => task1.Task)
            .WithSubscribe(_ => task2.Task, out var subscription);

        subscription.Dispose();

        command.Execute("x");
        command.CanExecute().IsFalse();
        task1.SetResult(null);
        command.CanExecute().IsTrue();
    }

    [TestMethod]
    public void SourceAndSharedCanExecuteTest()
    {
        var task1 = new TaskCompletionSource<object>();
        var task2 = new TaskCompletionSource<object>();

        var source = new BehaviorSubject<bool>(false);
        var sharedCanExecute = new ReactivePropertySlim<bool>(true);

        var cmd1CanExecuteChangedCounter = 0;
        var cmd1 = source.ToAsyncReactiveCommand(sharedCanExecute);
        cmd1.Subscribe(() => task1.Task);
        cmd1.CanExecuteChanged += (_, __) => cmd1CanExecuteChangedCounter++;
        var cmd2CanExecuteChangedCounter = 0;
        var cmd2 = source.ToAsyncReactiveCommand(sharedCanExecute);
        cmd2.Subscribe(() => task2.Task);
        cmd2.CanExecuteChanged += (_, __) => cmd2CanExecuteChangedCounter++;

        // check for initial state
        cmd1.CanExecute().IsFalse();
        cmd2.CanExecute().IsFalse();
        cmd1CanExecuteChangedCounter.Is(0);
        cmd2CanExecuteChangedCounter.Is(0);

        // change source status
        source.OnNext(true);
        cmd1.CanExecute().IsTrue();
        cmd2.CanExecute().IsTrue();
        cmd1CanExecuteChangedCounter.Is(1);
        cmd2CanExecuteChangedCounter.Is(1);

        // execute cmd1 and check status
        cmd1.Execute();
        cmd1.CanExecute().IsFalse();
        cmd2.CanExecute().IsFalse();
        cmd1CanExecuteChangedCounter.Is(2);
        cmd2CanExecuteChangedCounter.Is(2);

        // change status from source, but can't execute because still cmd1 is executing
        source.OnNext(false);
        source.OnNext(true);
        cmd1.CanExecute().IsFalse();
        cmd2.CanExecute().IsFalse();
        cmd1CanExecuteChangedCounter.Is(2);
        cmd2CanExecuteChangedCounter.Is(2);

        // after cmd1 finished, then can execute will be true
        task1.SetResult(null);
        cmd1.CanExecute().IsTrue();
        cmd2.CanExecute().IsTrue();
        cmd1CanExecuteChangedCounter.Is(3);
        cmd2CanExecuteChangedCounter.Is(3);

        // still, woks fine.
        source.OnNext(false);
        cmd1.CanExecute().IsFalse();
        cmd2.CanExecute().IsFalse();
        cmd1CanExecuteChangedCounter.Is(4);
        cmd2CanExecuteChangedCounter.Is(4);

        source.OnNext(true);
        cmd1.CanExecute().IsTrue();
        cmd2.CanExecute().IsTrue();
        cmd1CanExecuteChangedCounter.Is(5);
        cmd2CanExecuteChangedCounter.Is(5);

        cmd2.Execute();
        cmd1.CanExecute().IsFalse();
        cmd2.CanExecute().IsFalse();
        cmd1CanExecuteChangedCounter.Is(6);
        cmd2CanExecuteChangedCounter.Is(6);

        task2.SetResult(null);
        cmd1.CanExecute().IsTrue();
        cmd2.CanExecute().IsTrue();
        cmd1CanExecuteChangedCounter.Is(7);
        cmd2CanExecuteChangedCounter.Is(7);
    }
}
