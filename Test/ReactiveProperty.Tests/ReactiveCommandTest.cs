using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reactive.Bindings;
using System.Reactive.Linq;
using Microsoft.Reactive.Testing;
using System.Reactive.Subjects;
using System.Threading;
using System.Reactive.Disposables;

namespace ReactiveProperty.Tests
{
    [TestClass]
    public class ReactiveCommandTest : ReactiveTest
    {
        [TestMethod]
        public void ReactiveCommandAllFlow()
        {
            var testScheduler = new TestScheduler();
            var @null = (object)null;
            var recorder1 = testScheduler.CreateObserver<object>();
            var recorder2 = testScheduler.CreateObserver<object>();

            var cmd = new ReactiveCommand();
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

            var cmd = new ReactiveCommand();
            int counter = 0;
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
            var cmd = new ReactiveCommand()
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
            var cmd = new ReactiveCommand<string>()
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

            IDisposable disposable1;
            IDisposable disposable2;
            var cmd = new ReactiveCommand()
                .WithSubscribe(() => recorder1.OnNext("x"), out disposable1)
                .WithSubscribe(() => recorder2.OnNext("x"), out disposable2)
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

            IDisposable disposable1;
            IDisposable disposable2;
            var cmd = new ReactiveCommand<string>()
                .WithSubscribe(x => recorder1.OnNext(x), out disposable1)
                .WithSubscribe(x => recorder2.OnNext(x), out disposable2)
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

    }
}
