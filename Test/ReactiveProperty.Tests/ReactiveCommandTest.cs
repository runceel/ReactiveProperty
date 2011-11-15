using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Codeplex.Reactive;
using System.Reactive.Linq;
using Microsoft.Reactive.Testing;
using System.Reactive.Subjects;
using System.Threading;

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
    }
}
