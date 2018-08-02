using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reactive.Bindings;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReactiveProperty.Tests
{
    [TestClass]
    public class AwaiterTest
    {
        [TestMethod]
        public async Task AwaitProperty()
        {
            IReactiveProperty<int> prop = new ReactiveProperty<int>();
            prop.Value = 999;

            var __ = Task.Delay(1000).ContinueWith(_ => prop.Value = 1000);

            var a = await prop;

            a.Is(1000);
        }

        [TestMethod]
        public async Task AwaitCmd()
        {
            ReactiveCommand<int> cmd = new ReactiveCommand<int>();

            var __ = Task.Delay(1000).ContinueWith(_ => cmd.Execute(9999));

            var a = await cmd;

            a.Is(9999);
        }

        [TestMethod]
        public async Task MultipleAwaitProperty()
        {
            IReactiveProperty<int> prop = new ReactiveProperty<int>();
            prop.Value = 999;

            var values = new ConcurrentQueue<int>();
            var rpAwaitTasks = new ConcurrentQueue<Task>();
            var tasks = Enumerable.Range(1, 3)
                .Select(x => Task.Run(new Action(async () =>
                {
                    var tcs = new TaskCompletionSource<int>();
                    rpAwaitTasks.Enqueue(tcs.Task);
                    values.Enqueue(await prop);
                    tcs.SetResult(0);
                })));
            await Task.WhenAll(tasks);

            prop.Value = 1000;
            prop.Value = 1001;

            await Task.WhenAll(rpAwaitTasks);
            values.Is(1000, 1000, 1000);
        }

        [TestMethod]
        public async Task MultipleAwaitCommand()
        {
            var cmd = new ReactiveCommand<int>();
            cmd.Execute(999);

            var values = new ConcurrentQueue<int>();
            var cmdAwaitTasks = new ConcurrentQueue<Task>();
            var tasks = Enumerable.Range(1, 3)
                .Select(x => Task.Run(new Action(async () =>
                {
                    var tcs = new TaskCompletionSource<int>();
                    cmdAwaitTasks.Enqueue(tcs.Task);
                    values.Enqueue(await cmd);
                    tcs.SetResult(0);
                })));
            await Task.WhenAll(tasks);

            cmd.Execute(1000);
            cmd.Execute(1001);

            await Task.WhenAll(cmdAwaitTasks);
            values.Is(1000, 1000, 1000);
        }
    }
}
