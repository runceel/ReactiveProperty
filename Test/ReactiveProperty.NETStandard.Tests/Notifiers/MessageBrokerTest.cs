using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reactive.Bindings.Notifiers;

namespace ReactiveProperty.Tests.Notifiers
{
    [TestClass]
    public class MessageBrokerTest
    {
        [TestMethod]
        public void Sync()
        {
            var mb = new MessageBroker();
            var l = new List<string>();

            var d1 = mb.Subscribe<int>(x => l.Add("a:" + x));
            var d2 = mb.Subscribe<int>(x => l.Add("b:" + x));
            var d3 = mb.ToObservable<int>().Subscribe(x => l.Add("c:" + x));

            mb.Publish(100);
            l.Is("a:100", "b:100", "c:100"); l.Clear();

            d2.Dispose();
            mb.Publish(200);
            l.Is("a:200", "c:200"); l.Clear();

            d3.Dispose();
            mb.Publish(300);
            l.Is("a:300"); l.Clear();

            d1.Dispose();
            mb.Publish(400);
            l.Count.Is(0);
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

        [TestMethod]
        public async Task Async()
        {
            var mb = new AsyncMessageBroker();
            var l = new List<string>();

            var d1 = mb.Subscribe<int>(async x => l.Add("a:" + x));
            var d2 = mb.Subscribe<int>(async x => l.Add("b:" + x));

            await mb.PublishAsync(100);
            l.Is("a:100", "b:100"); l.Clear();

            d2.Dispose();
            await mb.PublishAsync(200);
            l.Is("a:200"); l.Clear();

            d1.Dispose();
            await mb.PublishAsync(300);
            l.Count.Is(0);
        }

#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    }
}
