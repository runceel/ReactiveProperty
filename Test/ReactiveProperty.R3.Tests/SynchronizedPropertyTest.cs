#nullable enable

using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using R3;
using Reactive.Bindings.R3.Extensions;

namespace ReactiveProperty.Tests;

[TestClass]
public sealed class SynchronizedPropertyTest
{
    [TestMethod]
    public void ToReactivePropertyAsSynchronizedWritesBothWays()
    {
        var source = new Person { Name = "Alice" };
        using var property = source.ToReactivePropertyAsSynchronized(x => x.Name);
        var values = new List<string?>();
        using var subscription = property.Subscribe(values.Add);

        source.Name = "Bob";
        property.Value = "Charlie";

        source.Name.Is("Charlie");
        property.Value.Is("Charlie");
        values.Is("Alice", "Bob", "Charlie");
    }

    [TestMethod]
    public void ToReactivePropertyAsSynchronizedConvertsBothWays()
    {
        var source = new Person { Age = 10 };
        using var property = source.ToReactivePropertyAsSynchronized(x => x.Age, x => x.ToString(), int.Parse);

        property.Value = "20";

        source.Age.Is(20);
    }

    [TestMethod]
    public void ToReactivePropertySlimAsSynchronizedWritesBothWays()
    {
        var source = new Person { Name = "Alice" };
        using var property = source.ToReactivePropertySlimAsSynchronized(x => x.Name);

        source.Name = "Bob";
        property.Value = "Charlie";

        source.Name.Is("Charlie");
        property.Value.Is("Charlie");
    }

    [TestMethod]
    public void NestedToReactivePropertyAsSynchronizedWritesLeafAndToleratesNullParent()
    {
        var source = new Person { Child = new Child { Name = "Initial" } };
        using var property = source.ToReactivePropertyAsSynchronized(x => x.Child!, x => x.Name);
        var values = new List<string>();
        using var subscription = property.Subscribe(values.Add);

        source.Child.Name = "Changed";
        property.Value = "Written";
        source.Child = new Child { Name = "Replacement" };
        source.Child.Name = "ReplacementChanged";
        property.Value = "WrittenAgain";
        source.Child = null;
        property.Value = "WrittenButNotApplied";

        source.Child.IsNull();
        values.Is("Initial", "Changed", "Written", "Replacement", "ReplacementChanged", "WrittenAgain", "WrittenButNotApplied");
    }

    private sealed class Person : INotifyPropertyChanged
    {
        private string _name = "";
        private int _age;
        private Child? _child;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }

        public int Age
        {
            get => _age;
            set
            {
                _age = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Age)));
            }
        }

        public Child? Child
        {
            get => _child;
            set
            {
                _child = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Child)));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    private sealed class Child : INotifyPropertyChanged
    {
        private string _name = "";

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
