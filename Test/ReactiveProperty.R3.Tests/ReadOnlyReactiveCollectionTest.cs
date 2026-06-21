#nullable enable

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using R3;
using Reactive.Bindings.R3;
using Reactive.Bindings.R3.Extensions;

namespace ReactiveProperty.Tests;

[TestClass]
public sealed class ReadOnlyReactiveCollectionTest
{
    [TestMethod]
    public void MirrorsObservableCollectionWithConverterAndDisposesRemovedElements()
    {
        var source = new ObservableCollection<Item> { new("a"), new("b") };
        var collection = source.ToReadOnlyReactiveCollection(x => new DisposableItem(x.Name));
        var actions = new List<NotifyCollectionChangedAction>();

        ((INotifyCollectionChanged)collection).CollectionChanged += (_, e) => actions.Add(e.Action);

        source.Add(new("c"));
        var removed = collection[0];
        source.RemoveAt(0);
        var replaced = collection[0];
        source[0] = new("d");

        collection.Select(x => x.Name).ToArray().Is("d", "c");
        removed.IsDisposed.IsTrue();
        replaced.IsDisposed.IsTrue();
        actions.Is(NotifyCollectionChangedAction.Add, NotifyCollectionChangedAction.Remove, NotifyCollectionChangedAction.Replace);

        collection.Dispose();

        collection.All(x => x.IsDisposed).IsTrue();
    }

    [TestMethod]
    public void AppliesObservableChangeStreamAndResetStream()
    {
        var add = new Subject<string>();
        var reset = new Subject<Unit>();
        var collection = add.ToReadOnlyReactiveCollection(reset);
        var actions = new List<NotifyCollectionChangedAction>();

        ((INotifyCollectionChanged)collection).CollectionChanged += (_, e) => actions.Add(e.Action);

        add.OnNext("a");
        add.OnNext("b");
        reset.OnNext(Unit.Default);

        collection.Count.Is(0);
        actions.Is(NotifyCollectionChangedAction.Add, NotifyCollectionChangedAction.Add, NotifyCollectionChangedAction.Reset);
    }

    [TestMethod]
    public void PostsChangesToSynchronizationContext()
    {
        var subject = new Subject<CollectionChanged<string>>();
        var context = new QueuedSynchronizationContext();
        var collection = subject.ToReadOnlyReactiveCollection(raiseEventContext: context);
        var eventThreadIds = new List<int>();

        ((INotifyCollectionChanged)collection).CollectionChanged += (_, _) => eventThreadIds.Add(Environment.CurrentManagedThreadId);
        subject.OnNext(CollectionChanged<string>.Add(0, "a"));

        collection.Count.Is(0);
        context.RunOne();

        collection.Is("a");
        eventThreadIds.Is(context.ThreadId);
    }

    private sealed record Item(string Name);

    private sealed class DisposableItem(string name) : IDisposable
    {
        public string Name { get; } = name;

        public bool IsDisposed { get; private set; }

        public void Dispose() => IsDisposed = true;
    }

    private sealed class QueuedSynchronizationContext : SynchronizationContext
    {
        private readonly Queue<(SendOrPostCallback Callback, object? State)> _queue = new();

        public int ThreadId { get; private set; }

        public override void Post(SendOrPostCallback d, object? state) => _queue.Enqueue((d, state));

        public void RunOne()
        {
            ThreadId = Environment.CurrentManagedThreadId;
            var item = _queue.Dequeue();
            item.Callback(item.State);
        }
    }
}
