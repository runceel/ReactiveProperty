using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reactive.Bindings;
using Reactive.Bindings.Internals;

namespace ReactiveProperty.Tests.Internals;

[TestClass]
public class PropertyObservableTest : ReactiveTest
{
    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void PathAccessorPreservesNullAndReflectionConversions(bool legacy)
    {
        var source = new AccessorItem();
        using var node = new PathNodeHarness(legacy, nameof(AccessorItem.Number));
        node.Update(source);

        node.Set((short)7).IsTrue();
        source.Number.Is(7);
        node.Set(null).IsTrue();
        source.Number.Is(0);
        Assert.ThrowsExactly<ArgumentException>(() => node.Set("invalid"));

        node.Update(null);
        node.Get().IsNull();
        node.Set(1).IsFalse();
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void PathAccessorPreservesNullableAndBoxedStructValues(bool legacy)
    {
        var source = new AccessorItem();
        using var nullable = new PathNodeHarness(legacy, nameof(AccessorItem.Optional));
        nullable.Update(source);
        nullable.Set(5).IsTrue();
        source.Optional.Is(5);
        nullable.Set(null).IsTrue();
        Assert.IsNull(source.Optional);

        object boxed = new AccessorStruct { Number = 1 };
        using var valueType = new PathNodeHarness(legacy, nameof(AccessorStruct.Number));
        valueType.Update(boxed);
        valueType.Get().Is(1);
        valueType.Set(2).IsTrue();
        ((AccessorStruct)boxed).Number.Is(1);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void PathAccessorPreservesGetterAndSetterExceptions(bool legacy)
    {
        using var node = new PathNodeHarness(legacy, nameof(ThrowingItem.Value));
        node.Update(new ThrowingItem());

        var getError = Assert.ThrowsExactly<TargetInvocationException>(() => node.Get());
        Assert.IsInstanceOfType<InvalidOperationException>(getError.InnerException);
        var setError = Assert.ThrowsExactly<TargetInvocationException>(() => node.Set(1));
        Assert.IsInstanceOfType<InvalidOperationException>(setError.InnerException);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void PathAccessorPreservesMissingMembersAndChangedSourceType(bool legacy)
    {
        using var missing = new PathNodeHarness(legacy, "Missing");
        missing.Update(new AccessorItem());
        Assert.ThrowsExactly<ArgumentException>(() => missing.Get());

        using var node = new PathNodeHarness(legacy, nameof(AccessorItem.Number));
        node.Update(new AccessorItem());
        node.Set(1).IsTrue();
        node.Update(new OtherAccessorItem { Number = 9 });
        node.Get().Is(9);
        Assert.ThrowsExactly<ArgumentException>(() => node.Set(2));
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void RepeatedPathReadsAvoidExcessAllocation(bool legacy)
    {
        using var node = new PathNodeHarness(legacy, nameof(AccessorItem.Text));
        node.Update(new AccessorItem());
        for (var i = 0; i < 32; i++)
        {
            GC.KeepAlive(node.Get());
        }

        var before = GC.GetAllocatedBytesForCurrentThread();
        for (var i = 0; i < 64; i++)
        {
            GC.KeepAlive(node.Get());
        }

        var allocated = GC.GetAllocatedBytesForCurrentThread() - before;
        Assert.IsTrue(allocated < 64 * 8, $"Allocated {allocated} bytes for 64 path reads.");
    }

    [TestMethod]
    public void PathAccessorCacheSupportsConcurrentFirstUse()
    {
        Parallel.For(0, 64, i =>
        {
            var source = new ConcurrentAccessorItem { Number = i };
            using var node = new PathNodeHarness(i % 2 == 0, nameof(ConcurrentAccessorItem.Number));
            node.Update(source);
            Assert.AreEqual(i, node.Get());
            Assert.IsTrue(node.Set(i + 1));
            Assert.AreEqual(i + 1, source.Number);
        });
    }

    private sealed class PathNodeHarness : IDisposable
    {
        public readonly Action<object> Update;
        public readonly Func<object> Get;
        public readonly Func<object, bool> Set;
        private readonly IDisposable _node;

        public PathNodeHarness(bool legacy, string propertyName)
        {
            if (legacy)
            {
                var node = new PropertyPathNodeLegacy(propertyName);
                _node = node;
                Update = node.UpdateSource;
                Get = node.GetPropertyPathValue;
                Set = node.SetPropertyPathValue;
            }
            else
            {
                var node = new PropertyPathNode(propertyName);
                _node = node;
                Update = node.UpdateSource;
                Get = node.GetPropertyPathValue;
                Set = node.SetPropertyPathValue;
            }
        }

        public void Dispose() => _node.Dispose();
    }

    private sealed class AccessorItem
    {
        public int Number { get; set; }
        public int? Optional { get; set; }
        public string Text { get; set; } = "value";
    }

    private struct AccessorStruct
    {
        public int Number { get; set; }
    }

    private sealed class OtherAccessorItem
    {
        public int Number { get; set; }
    }

    private sealed class ConcurrentAccessorItem
    {
        public int Number { get; set; }
    }

    private sealed class ThrowingItem
    {
        public int Value
        {
            get => throw new InvalidOperationException("getter");
            set => throw new InvalidOperationException("setter");
        }
    }

    [TestMethod]
    public void ObserveProperty()
    {
        var item = new Item
        {
            Child = new Item
            {
                Value = 10,
            },
        };

        var testScheduler = new TestScheduler();
        var testObserver = testScheduler.CreateObserver<int>();

        var path = PropertyObservable.CreateFromPropertySelector(item, x => x.Child.Value);
        path.Subscribe(testObserver);
        path.GetPropertyPathValue().Is(10);
        item.Child.Value = 1;

        testObserver.Messages.Is(OnNext(0, 1));
        item.IsPropertyChangedEmpty.IsFalse();
        path.Dispose();
        item.IsPropertyChangedEmpty.IsTrue();
        item.Child.Value = 100;
        testObserver.Messages.Is(OnNext(0, 1), OnCompleted<int>(0));
    }

    [TestMethod]
    public void ChangeObserveSourcePropertyType()
    {
        var item = new Item
        {
            Child = new Item
            {
                Value = 10,
            },
        };

        var testScheduler = new TestScheduler();
        var testObserver = testScheduler.CreateObserver<int>();

        var path = PropertyObservable.CreateFromPropertySelector(item, x => x.Child.Value);
        path.Subscribe(testObserver);
        path.GetPropertyPathValue().Is(10);
        item.Child.Value = 1;
        testObserver.Messages.Is(OnNext(0, 1));

        item.Child = new AnotherItem
        {
            Value = 9999,
        };
        path.GetPropertyPathValue().Is(9999);
        testObserver.Messages.Is(OnNext(0, 1), OnNext(0, 9999));
    }

    [TestMethod]
    public void ObserveReactiveProperty()
    {
        var rp = new ReactiveProperty<ClassThatDoesNotImplementedINotifyPropertyChanged>();
        var testScheduler = new TestScheduler();
        var testObserver = testScheduler.CreateObserver<string>();
        var path = PropertyObservable.CreateFromPropertySelector(rp, x => x.Value.Name);
        path.Subscribe(testObserver);

        path.GetPropertyPathValue().IsNull();
        rp.Value = new ClassThatDoesNotImplementedINotifyPropertyChanged { Name = "name1" };
        path.GetPropertyPathValue().Is("name1");
    }

    class ItemBase : INotifyPropertyChanged
    {
        public bool IsPropertyChangedEmpty => PropertyChanged == null;
        private void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return;
            }

            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private int _value;
        public int Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value); }
        }

        private ItemBase _child;
        public ItemBase Child
        {
            get { return _child; }
            set { SetProperty(ref _child, value); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
    class Item : ItemBase { }
    class AnotherItem : ItemBase { }

    class ClassThatDoesNotImplementedINotifyPropertyChanged
    {
        public string Name { get; set; }
    }
}
