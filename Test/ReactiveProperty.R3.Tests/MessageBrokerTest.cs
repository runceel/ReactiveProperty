#nullable enable

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using R3;
using Reactive.Bindings.R3.Notifiers;

namespace ReactiveProperty.Tests;

[TestClass]
public sealed class MessageBrokerTest
{
    private sealed record Message(int Value);

    [TestMethod]
    public void PublishSubscribeUnsubscribe()
    {
        using var broker = new MessageBroker();
        var received = new List<int>();
        var subscription = broker.Subscribe<Message>(m => received.Add(m.Value));

        broker.Publish(new Message(1));
        broker.Publish(new Message(2));
        received.Is(1, 2);

        subscription.Dispose();
        broker.Publish(new Message(3));
        received.Is(1, 2);
    }

    [TestMethod]
    public void PublishWithoutSubscriberIsNoOp()
    {
        using var broker = new MessageBroker();
        broker.Publish(new Message(1));
    }

    [TestMethod]
    public void FiltersByType()
    {
        using var broker = new MessageBroker();
        var ints = new List<int>();
        var strings = new List<string>();
        using var s1 = broker.Subscribe<int>(ints.Add);
        using var s2 = broker.Subscribe<string>(strings.Add);

        broker.Publish(1);
        broker.Publish("hello");

        ints.Is(1);
        strings.Is("hello");
    }

    [TestMethod]
    public void PublishAfterDisposeThrows()
    {
        var broker = new MessageBroker();
        broker.Dispose();
        Assert.ThrowsExactly<ObjectDisposedException>(() => broker.Publish(1));
    }

    [TestMethod]
    public void ToObservable()
    {
        using var broker = new MessageBroker();
        var received = new List<int>();
        using var subscription = broker.ToObservable<Message>().Subscribe(m => received.Add(m.Value));

        broker.Publish(new Message(1));
        broker.Publish(new Message(2));

        received.Is(1, 2);
    }

    [TestMethod]
    public async Task AsyncPublishSubscribeUnsubscribe()
    {
        using var broker = new AsyncMessageBroker();
        var received = new List<int>();
        var subscription = broker.Subscribe<Message>(async m =>
        {
            await Task.Yield();
            received.Add(m.Value);
        });

        await broker.PublishAsync(new Message(1));
        received.Is(1);

        subscription.Dispose();
        await broker.PublishAsync(new Message(2));
        received.Is(1);
    }

    [TestMethod]
    public async Task AsyncPublishWithoutSubscriberIsNoOp()
    {
        using var broker = new AsyncMessageBroker();
        await broker.PublishAsync(new Message(1));
    }

    [TestMethod]
    public async Task AsyncPublishAfterDisposeThrows()
    {
        var broker = new AsyncMessageBroker();
        broker.Dispose();
        await Assert.ThrowsExactlyAsync<ObjectDisposedException>(() => broker.PublishAsync(1));
    }
}
