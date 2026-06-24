#nullable enable

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using R3;
using Reactive.Bindings.R3;

namespace ReactiveProperty.Tests;

[TestClass]
public sealed class CommandTest
{
    [TestMethod]
    public async Task AsyncReactiveCommandDisablesWhileRunning()
    {
        using var command = new AsyncReactiveCommand();
        var gate = new TaskCompletionSource();
        var executed = 0;
        using var subscription = command.Subscribe(async () =>
        {
            executed++;
            await gate.Task;
        });

        var task = command.ExecuteAsync();
        await SpinWaitUntil(() => !command.CanExecute());

        command.CanExecute().IsFalse();
        command.Execute();
        executed.Is(1);

        gate.SetResult();
        await task;

        command.CanExecute().IsTrue();
    }

    [TestMethod]
    public void CanExecuteChangedUsesCapturedSynchronizationContext()
    {
        var callbackExecuted = false;
        var originalContext = SynchronizationContext.Current;
        var context = new TestSynchronizationContext();
        SynchronizationContext.SetSynchronizationContext(context);

        try
        {
            using var command = new AsyncReactiveCommand();
            command.CanExecuteChanged += (_, _) =>
            {
                callbackExecuted = true;
            };

            Task.Run(() => command.Dispose()).GetAwaiter().GetResult();

            callbackExecuted.IsTrue();
            Assert.IsTrue(context.PostCount > 0);
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(originalContext);
        }
    }

    [TestMethod]
    public async Task SharedCanExecuteDisablesMultipleAsyncCommands()
    {
        using var sharedCanExecute = new ReactiveProperty<bool>(true);
        using var first = sharedCanExecute.ToAsyncReactiveCommand();
        using var second = sharedCanExecute.ToAsyncReactiveCommand();
        var gate = new TaskCompletionSource();
        using var subscription = first.Subscribe(async () => await gate.Task);

        var task = first.ExecuteAsync();
        await SpinWaitUntil(() => !first.CanExecute());

        first.CanExecute().IsFalse();
        second.CanExecute().IsFalse();

        gate.SetResult();
        await task;

        first.CanExecute().IsTrue();
        second.CanExecute().IsTrue();
    }

    [TestMethod]
    public void CanExecuteSourceRaisesChangedAndDisposeDisables()
    {
        using var source = new ReactiveProperty<bool>(false);
        using var command = source.ToAsyncReactiveCommand();
        var changedCount = 0;
        command.CanExecuteChanged += (_, _) => changedCount++;

        source.Value = true;

        command.CanExecute().IsTrue();
        changedCount.Is(1);

        command.Dispose();
        command.CanExecute().IsFalse();
        command.Execute();
    }

    [TestMethod]
    public void DisposingOneSharedCommandDoesNotDisableSiblings()
    {
        using var sharedCanExecute = new ReactiveProperty<bool>(true);
        using var first = sharedCanExecute.ToAsyncReactiveCommand();
        using var second = sharedCanExecute.ToAsyncReactiveCommand();

        first.Dispose();

        sharedCanExecute.Value.IsTrue();
        first.CanExecute().IsFalse();
        second.CanExecute().IsTrue();
    }

    [TestMethod]
    public void WithSubscribeAddsHandlerToR3ReactiveCommand()
    {
        using var command = new ReactiveCommand<int>();
        var values = new List<int>();

        command.WithSubscribe(values.Add);
        command.Execute(123);

        values.Is(123);
    }

    [TestMethod]
    public async Task GenericAsyncReactiveCommandPassesParameter()
    {
        using var command = new AsyncReactiveCommand<int>();
        var values = new List<int>();
        using var subscription = command.Subscribe(x =>
        {
            values.Add(x);
            return Task.CompletedTask;
        });

        await command.ExecuteAsync(10);

        values.Is(10);
    }

    [TestMethod]
    public async Task AsyncReactiveCommandTreatsNullTaskAsCompletedTask()
    {
        using var command = new AsyncReactiveCommand<int>();
        using var subscription = command.Subscribe(_ => null!);

        await command.ExecuteAsync(10);

        command.CanExecute().IsTrue();
    }

    private sealed class TestSynchronizationContext : SynchronizationContext
    {
        public int PostCount { get; private set; }

        public override void Post(SendOrPostCallback d, object? state)
        {
            PostCount++;
            d(state);
        }
    }

    private static async Task SpinWaitUntil(Func<bool> condition)
    {
        for (var i = 0; i < 100; i++)
        {
            if (condition())
            {
                return;
            }

            await Task.Delay(10);
        }

        Assert.Fail("Condition was not met.");
    }
}
