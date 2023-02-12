using System;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reactive.Bindings;

namespace ReactiveProperty.Tests;

[TestClass]
public class ReactiveCommandSlimTest : ReactiveTest
{
    [TestMethod]
    public void ReactiveCommandAllFlow()
    {
        var testScheduler = new TestScheduler();
        var @null = (object)null;
        var recorder1 = testScheduler.CreateObserver<object>();
        var recorder2 = testScheduler.CreateObserver<object>();

        var cmd = new ReactiveCommandSlim();
        cmd.Subscribe(recorder1);
        cmd.Subscribe(recorder2);

        cmd.CanExecute().Is(true);
        cmd.Execute(); testScheduler.AdvanceBy(10);
        cmd.Execute(); testScheduler.AdvanceBy(10);
        cmd.Execute(); testScheduler.AdvanceBy(10);

        cmd.Dispose();
        cmd.CanExecute().Is(false);

        cmd.Dispose(); // dispose again

        recorder1.Messages.Is(
            OnNext(0, @null),
            OnNext(10, @null),
            OnNext(20, @null),
            OnCompleted<object>(30));

        recorder2.Messages.Is(
            OnNext(0, @null),
            OnNext(10, @null),
            OnNext(20, @null),
            OnCompleted<object>(30));
    }

    [TestMethod]
    public void ReactiveCommandSubscribe()
    {
        var testScheduler = new TestScheduler();
        var recorder1 = testScheduler.CreateObserver<int>();
        var recorder2 = testScheduler.CreateObserver<int>();

        var cmd = new ReactiveCommandSlim();
        var counter = 0;
        Action countUp = () => counter++;
        cmd.Subscribe(countUp);
        Action recordAction1 = () => recorder1.OnNext(counter);
        cmd.Subscribe(recordAction1);
        Action recordAction2 = () => recorder2.OnNext(counter);
        cmd.Subscribe(recordAction2);

        cmd.Execute(); testScheduler.AdvanceBy(10);
        cmd.Execute(); testScheduler.AdvanceBy(10);
        cmd.Execute(); testScheduler.AdvanceBy(10);

        recorder1.Messages.Is(
            OnNext(0, 1),
            OnNext(10, 2),
            OnNext(20, 3));
        recorder2.Messages.Is(
            OnNext(0, 1),
            OnNext(10, 2),
            OnNext(20, 3));
    }

    [TestMethod]
    public void WithSubscribe()
    {
        var testScheduler = new TestScheduler();
        var recorder1 = testScheduler.CreateObserver<string>();
        var recorder2 = testScheduler.CreateObserver<string>();
        var recorder3 = testScheduler.CreateObserver<string>();

        var disposable1 = new CompositeDisposable();
        var disposable2 = new CompositeDisposable();
        var cmd = new ReactiveCommandSlim()
            .WithSubscribe(() => recorder1.OnNext("x"), disposable1.Add)
            .WithSubscribe(() => recorder2.OnNext("x"), disposable2.Add)
            .WithSubscribe(() => recorder3.OnNext("x"));

        cmd.Execute();
        testScheduler.AdvanceBy(10);

        disposable1.Dispose();
        cmd.Execute();
        testScheduler.AdvanceBy(10);

        disposable2.Dispose();
        cmd.Execute();

        recorder1.Messages.Is(
            OnNext(0, "x"));
        recorder2.Messages.Is(
            OnNext(0, "x"),
            OnNext(10, "x"));
        recorder3.Messages.Is(
            OnNext(0, "x"),
            OnNext(10, "x"),
            OnNext(20, "x"));
    }

    [TestMethod]
    public void WithSubscribeGenericVersion()
    {
        var testScheduler = new TestScheduler();
        var recorder1 = testScheduler.CreateObserver<string>();
        var recorder2 = testScheduler.CreateObserver<string>();
        var recorder3 = testScheduler.CreateObserver<string>();

        var disposable1 = new CompositeDisposable();
        var disposable2 = new CompositeDisposable();
        var cmd = new ReactiveCommandSlim<string>()
            .WithSubscribe(x => recorder1.OnNext(x), disposable1.Add)
            .WithSubscribe(x => recorder2.OnNext(x), disposable2.Add)
            .WithSubscribe(x => recorder3.OnNext(x));

        cmd.Execute("a");
        testScheduler.AdvanceBy(10);

        disposable1.Dispose();
        cmd.Execute("b");
        testScheduler.AdvanceBy(10);

        disposable2.Dispose();
        cmd.Execute("c");

        recorder1.Messages.Is(
            OnNext(0, "a"));
        recorder2.Messages.Is(
            OnNext(0, "a"),
            OnNext(10, "b"));
        recorder3.Messages.Is(
            OnNext(0, "a"),
            OnNext(10, "b"),
            OnNext(20, "c"));
    }

    [TestMethod]
    public void WithSubscribeDisposableOverride()
    {
        var testScheduler = new TestScheduler();
        var recorder1 = testScheduler.CreateObserver<string>();
        var recorder2 = testScheduler.CreateObserver<string>();
        var recorder3 = testScheduler.CreateObserver<string>();
        var cmd = new ReactiveCommandSlim()
            .WithSubscribe(() => recorder1.OnNext("x"), out var disposable1)
            .WithSubscribe(() => recorder2.OnNext("x"), out var disposable2)
            .WithSubscribe(() => recorder3.OnNext("x"));

        cmd.Execute();
        testScheduler.AdvanceBy(10);

        disposable1.Dispose();
        cmd.Execute();
        testScheduler.AdvanceBy(10);

        disposable2.Dispose();
        cmd.Execute();

        recorder1.Messages.Is(
            OnNext(0, "x"));
        recorder2.Messages.Is(
            OnNext(0, "x"),
            OnNext(10, "x"));
        recorder3.Messages.Is(
            OnNext(0, "x"),
            OnNext(10, "x"),
            OnNext(20, "x"));
    }

    [TestMethod]
    public void WithSubscribeDisposableOverrideGenericVersion()
    {
        var testScheduler = new TestScheduler();
        var recorder1 = testScheduler.CreateObserver<string>();
        var recorder2 = testScheduler.CreateObserver<string>();
        var recorder3 = testScheduler.CreateObserver<string>();
        var cmd = new ReactiveCommandSlim<string>()
            .WithSubscribe(x => recorder1.OnNext(x), out var disposable1)
            .WithSubscribe(x => recorder2.OnNext(x), out var disposable2)
            .WithSubscribe(x => recorder3.OnNext(x));

        cmd.Execute("a");
        testScheduler.AdvanceBy(10);

        disposable1.Dispose();
        cmd.Execute("b");
        testScheduler.AdvanceBy(10);

        disposable2.Dispose();
        cmd.Execute("c");

        recorder1.Messages.Is(
            OnNext(0, "a"));
        recorder2.Messages.Is(
            OnNext(0, "a"),
            OnNext(10, "b"));
        recorder3.Messages.Is(
            OnNext(0, "a"),
            OnNext(10, "b"),
            OnNext(20, "c"));
    }

    [TestMethod]
    public void ShredCanExecuteStateCase()
    {
        var rp = new ReactivePropertySlim<bool>(false);

        var executeCounter1 = 0;
        var canExecutedCounter1 = 0;
        var executeCounter2 = 0;
        var canExecutedCounter2 = 0;

        var command1 = rp.ToReactiveCommandSlim();
        command1.Subscribe(() => executeCounter1++);
        command1.CanExecuteChanged += (_, __) => canExecutedCounter1++;
        var command2 = rp.ToReactiveCommandSlim();
        command2.Subscribe(() => executeCounter2++);
        command2.CanExecuteChanged += (_, __) => canExecutedCounter2++;

        command1.CanExecute().IsFalse();
        command2.CanExecute().IsFalse();
        command1.Execute();
        command2.Execute();
        executeCounter1.Is(0);
        executeCounter2.Is(0);

        rp.Value = true;
        command1.CanExecute().IsTrue();
        command2.CanExecute().IsTrue();
        canExecutedCounter1.Is(1);
        canExecutedCounter2.Is(1);

        command1.Execute();
        command2.Execute();
        command1.CanExecute().IsTrue();
        command2.CanExecute().IsTrue();
        executeCounter1.Is(1);
        executeCounter2.Is(1);
        canExecutedCounter1.Is(1);
        canExecutedCounter1.Is(1);

        rp.Value = false;
        command1.CanExecute().IsFalse();
        command2.CanExecute().IsFalse();
        canExecutedCounter1.Is(2);
        canExecutedCounter1.Is(2);
    }

    [TestMethod]
    public void ShredReactivePropertyCase()
    {
        var rp = new ReactivePropertySlim<bool>(false);

        var executeCounter1 = 0;
        var canExecutedCounter1 = 0;
        var executeCounter2 = 0;
        var canExecutedCounter2 = 0;

        var command1 = new ReactiveCommandSlim(rp);
        command1.Subscribe(() => executeCounter1++);
        command1.CanExecuteChanged += (_, __) => canExecutedCounter1++;
        var command2 = new ReactiveCommandSlim(rp);
        command2.Subscribe(() => executeCounter2++);
        command2.CanExecuteChanged += (_, __) => canExecutedCounter2++;

        command1.CanExecute().IsFalse();
        command2.CanExecute().IsFalse();
        command1.Execute();
        command2.Execute();
        executeCounter1.Is(0);
        executeCounter2.Is(0);

        rp.Value = true;
        command1.CanExecute().IsTrue();
        command2.CanExecute().IsTrue();
        canExecutedCounter1.Is(1);
        canExecutedCounter2.Is(1);

        command1.Execute();
        command2.Execute();
        command1.CanExecute().IsTrue();
        command2.CanExecute().IsTrue();
        executeCounter1.Is(1);
        executeCounter2.Is(1);
        canExecutedCounter1.Is(1);
        canExecutedCounter1.Is(1);

        rp.Value = false;
        command1.CanExecute().IsFalse();
        command2.CanExecute().IsFalse();
        canExecutedCounter1.Is(2);
        canExecutedCounter1.Is(2);
    }

    [TestMethod]
    public void SharedSourceAndReactivePropertyCase()
    {
        var canExecuteSource = new Subject<bool>();
        var sharedCanExecuteSource = new ReactivePropertySlim<bool>(false);
        var canExecuteChangedCounter = 0;

        var command = canExecuteSource.ToReactiveCommandSlim(sharedCanExecuteSource);
        command.CanExecuteChanged += (_, _) => canExecuteChangedCounter++;
        canExecuteChangedCounter.Is(0);
        command.CanExecute().IsFalse();

        sharedCanExecuteSource.Value = true;
        canExecuteChangedCounter.Is(1);
        command.CanExecute().IsTrue();

        canExecuteSource.OnNext(false);
        canExecuteChangedCounter.Is(2);
        command.CanExecute().IsFalse();

        canExecuteSource.OnNext(true);
        canExecuteChangedCounter.Is(3);
        command.CanExecute().IsTrue();
    }
}
