using Microsoft.VisualStudio.TestTools.UnitTesting;
using R3;
using Reactive.Bindings.R3Compat;
using Reactive.Bindings.R3Compat.Commands;
using System.Windows.Input;

namespace ReactiveProperty.R3.Compatibility.Tests;

[TestClass]
public class CommandCompatTest
{
    [TestMethod]
    public async Task CanExecuteIsFalseWhileRunningAndTrueOnCompletion()
    {
        using var command = new AsyncReactiveCommandCompat<int>();
        var tcs = new TaskCompletionSource();
        command.WithSubscribe(async x =>
        {
            await tcs.Task;
        });

        var execution = command.ExecuteAsync(1);
        Assert.IsFalse(((ICommand)command).CanExecute(null));

        tcs.SetResult();
        await execution;

        Assert.IsTrue(((ICommand)command).CanExecute(null));
    }

    [TestMethod]
    public async Task TwoCommandsShareCanExecuteState()
    {
        using var shared = new ReactivePropertyCompat<bool>(true);
        using var first = new AsyncReactiveCommandCompat<int>(shared);
        using var second = new AsyncReactiveCommandCompat<int>(shared);
        var tcs = new TaskCompletionSource();
        first.WithSubscribe(async _ => await tcs.Task);

        var execution = first.ExecuteAsync(1);

        Assert.IsFalse(((ICommand)first).CanExecute(null));
        Assert.IsFalse(((ICommand)second).CanExecute(null));

        tcs.SetResult();
        await execution;

        Assert.IsTrue(((ICommand)first).CanExecute(null));
        Assert.IsTrue(((ICommand)second).CanExecute(null));
    }

    [TestMethod]
    public async Task ExecuteIsNoOpAfterDispose()
    {
        var count = 0;
        var asyncCommand = new AsyncReactiveCommandCompat<int>().WithSubscribe(x =>
        {
            count++;
            return ValueTask.CompletedTask;
        });
        asyncCommand.Dispose();

        await asyncCommand.ExecuteAsync(1);

        Assert.AreEqual(0, count);

        var slimCommand = new ReactiveCommandSlimCompat<int>().WithSubscribe(_ => count++);
        slimCommand.Dispose();
        slimCommand.Execute(1);

        Assert.AreEqual(0, count);
    }

    [TestMethod]
    public void CanExecuteChangedFiresOnCanExecuteSourceUpdates()
    {
        var source = new Subject<bool>();
        using var command = new ReactiveCommandSlimCompat<int>(source);
        var count = 0;
        command.CanExecuteChanged += (_, _) => count++;

        source.OnNext(false);
        source.OnNext(true);

        Assert.AreEqual(2, count);
        Assert.IsTrue(command.CanExecute(0));
    }
}
