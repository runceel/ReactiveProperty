using Codeplex.Reactive.Notifier;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System;

namespace ReactiveProperty.Tests
{
    [TestClass]
    public class ProgressNotifierTest : ReactiveTest
    {
        [TestMethod]
        public void Test()
        {
            var prog = new ProgressNotifier();
            var recorder = new TestScheduler().CreateObserver<int>();
            prog.Subscribe(recorder);

            prog.IsCompleted.Is(false);
            prog.Report(30);
            prog.Report(60);
            prog.Report(99);
            prog.Report(100);
            prog.IsCompleted.Is(true);
            prog.Report(120);

            recorder.Messages.Is(
                OnNext(0, 0),
                OnNext(0, 30),
                OnNext(0, 60),
                OnNext(0, 99),
                OnNext(0, 100),
                OnCompleted<int>(0));
        }
    }
}
