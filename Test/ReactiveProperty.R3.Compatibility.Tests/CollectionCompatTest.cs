using System.Collections.Specialized;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ObservableCollections;
using R3;
using Reactive.Bindings.R3Compat.Collections;

namespace ReactiveProperty.R3.Compatibility.Tests;

[TestClass]
public class CollectionCompatTest
{
    [TestMethod]
    public void AddProjectionAndResetAreApplied()
    {
        var source = new Subject<CollectionChangedEvent<int>>();
        using var collection = ReadOnlyReactiveCollectionCompat<string>.Create(source, x => $"#{x}", disposeElement: false);

        source.OnNext(new CollectionChangedEvent<int>(NotifyCollectionChangedAction.Add, 1, 0, 0, -1, default));
        source.OnNext(new CollectionChangedEvent<int>(NotifyCollectionChangedAction.Add, 2, 0, 1, -1, default));

        CollectionAssert.AreEqual(new[] { "#1", "#2" }, collection.ToArray());

        source.OnNext(new CollectionChangedEvent<int>(NotifyCollectionChangedAction.Reset, 0, 0, -1, -1, default));

        Assert.AreEqual(0, collection.Count);
    }

    [TestMethod]
    public void ElementsAreDisposedWhenDisposeElementIsTrue()
    {
        var source = new Subject<CollectionChangedEvent<DisposableItem>>();
        using var collection = ReadOnlyReactiveCollectionCompat<DisposableItem>.Create(source, x => x);
        var item = new DisposableItem();

        source.OnNext(new CollectionChangedEvent<DisposableItem>(NotifyCollectionChangedAction.Add, item, null!, 0, -1, default));
        source.OnNext(new CollectionChangedEvent<DisposableItem>(NotifyCollectionChangedAction.Remove, null!, item, -1, 0, default));

        Assert.IsTrue(item.IsDisposed);
    }

    private sealed class DisposableItem : IDisposable
    {
        public bool IsDisposed { get; private set; }

        public void Dispose() => IsDisposed = true;
    }
}
