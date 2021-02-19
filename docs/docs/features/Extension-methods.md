# Extension methods

The `Reactive.Bindings.Extensions` namespace provides useful extension methods.

## `AddTo`

This is a very useful extension method in this namespace.
It collects `IDisposable` instances in the method chain.

In the case which doesn't use this, two statements between instance creation and adding an `IDisposable` instance.

```csharp
// init
var d = new CompositeDisposable();

Name = model.ObserveProperty(x => x.Name)
    .ToReadOnlyReactiveProperty();
d.Add(Name);

Age = model.ObserveProperty(x => x.Age)
    .ToReadOnlyReactiveProperty();
d.Add(Age);

// dispose all
d.Dispose();
```

`AddTo` extension method's sample code:

```csharp
// init
var d = new CompositeDisposable();

Name = model.ObserveProperty(x => x.Name)
    .ToReadOnlyReactiveProperty()
    .AddTo(d);

Age = model.ObserveProperty(x => x.Age)
    .ToReadOnlyReactiveProperty()
    .AddTo(d);

// dispose all
d.Dispose();
```

It's very cool!

## `CatchIgnore`

This extension method catches exceptions and returns `Observable.Empty`.

```csharp
source.CatchIgnore((Exception ex) => { ... error action ... })
    .Subscribe();
```

## `CombineLatestsValuesAreAllXXXX` 

Provides two methods.

- `CombineLatestValuesAreAllTrue`
- `CombineLatestValuesAreAllFalse`

These are just shortcuts:

```csharp
/// <summary>
/// Lastest values of each sequence are all true.
/// </summary>
public static IObservable<bool> CombineLatestValuesAreAllTrue(
    this IEnumerable<IObservable<bool>> sources) => 
    sources.CombineLatest(xs => xs.All(x => x));


/// <summary>
/// Lastest values of each sequence are all false.
/// </summary>
public static IObservable<bool> CombineLatestValuesAreAllFalse(
    this IEnumerable<IObservable<bool>> sources) =>
    sources.CombineLatest(xs => xs.All(x => !x));
```

## DisposePreviousValue

This is an extension method to call `Dispose` method for previous values of a `IObservable<T>` sequence.

```csharp
var source = new Subject<string>();
var rrp = source.Select(x => new SomeDisposableClass(x))
    .DisposePreviousValue()
    .ToReadOnlyReactivePropertySlim();

source.OnNext("first"); // first SomeDisposableClass is created.
source.OnNext("second"); // second SomeDisposableClass is created, and first one is disposed.
source.OnComplete(); // second one is also disposed.
```

## `CanExecuteChangedAsObservable`

This is an extension method of the `ICommand` interface.
It is a shortcut for the `Observable.FromEvent`.

```csharp
/// <summary>Converts CanExecuteChanged to an observable sequence.</summary>
public static IObservable<EventArgs> CanExecuteChangedAsObservable<T>(this T source)
    where T : ICommand =>
    Observable.FromEvent<EventHandler, EventArgs>(
        h => (sender, e) => h(e),
        h => source.CanExecuteChanged += h,
        h => source.CanExecuteChanged -= h);
```

## `INotifyCollectionChanged` extension methods

Convert `CollectionChanged` event to `IObservable`.

```csharp
/// <summary>Observe CollectionChanged:Remove and take single item.</summary>
public static IObservable<T> ObserveRemoveChanged<T>(this INotifyCollectionChanged source) =>
    source.CollectionChangedAsObservable()
        .Where(e => e.Action == NotifyCollectionChangedAction.Remove)
        .Select(e => (T)e.OldItems[0]);

/// <summary>Observe CollectionChanged:Remove.</summary>
public static IObservable<T[]> ObserveRemoveChangedItems<T>(this INotifyCollectionChanged source) =>
    source.CollectionChangedAsObservable()
        .Where(e => e.Action == NotifyCollectionChangedAction.Remove)
        .Select(e => e.OldItems.Cast<T>().ToArray());

/// <summary>Observe CollectionChanged:Move and take single item.</summary>
public static IObservable<OldNewPair<T>> ObserveMoveChanged<T>(this INotifyCollectionChanged source) =>
    source.CollectionChangedAsObservable()
        .Where(e => e.Action == NotifyCollectionChangedAction.Move)
        .Select(e => new OldNewPair<T>((T)e.OldItems[0], (T)e.NewItems[0]));

/// <summary>Observe CollectionChanged:Move.</summary>
public static IObservable<OldNewPair<T[]>> ObserveMoveChangedItems<T>(this INotifyCollectionChanged source) =>
    source.CollectionChangedAsObservable()
        .Where(e => e.Action == NotifyCollectionChangedAction.Move)
        .Select(e => new OldNewPair<T[]>(e.OldItems.Cast<T>().ToArray(), e.NewItems.Cast<T>().ToArray()));

/// <summary>Observe CollectionChanged:Replace and take single item.</summary>
public static IObservable<OldNewPair<T>> ObserveReplaceChanged<T>(this INotifyCollectionChanged source) =>
    source.CollectionChangedAsObservable()
        .Where(e => e.Action == NotifyCollectionChangedAction.Replace)
        .Select(e => new OldNewPair<T>((T)e.OldItems[0], (T)e.NewItems[0]));

/// <summary>Observe CollectionChanged:Replace.</summary>
public static IObservable<OldNewPair<T[]>> ObserveReplaceChangedItems<T>(this INotifyCollectionChanged source) =>
    source.CollectionChangedAsObservable()
        .Where(e => e.Action == NotifyCollectionChangedAction.Replace)
        .Select(e => new OldNewPair<T[]>(e.OldItems.Cast<T>().ToArray(), e.NewItems.Cast<T>().ToArray()));

/// <summary>Observe CollectionChanged:Reset.</summary>
public static IObservable<Unit> ObserveResetChanged<T>(this INotifyCollectionChanged source) =>
    source.CollectionChangedAsObservable()
        .Where(e => e.Action == NotifyCollectionChangedAction.Reset)
        .Select(_ => new Unit());
```

## `ObservableCollection` extension methods

It is a typesafe version of `INotifyPropertyChanged` extension methods.

```csharp
/// <summary>Observe CollectionChanged:Add and take single item.</summary>
public static IObservable<T> ObserveAddChanged<T>(this ObservableCollection<T> source) =>
    ((INotifyCollectionChanged)source).ObserveAddChanged<T>();

/// <summary>Observe CollectionChanged:Add.</summary>
public static IObservable<T[]> ObserveAddChangedItems<T>(this ObservableCollection<T> source) =>
    ((INotifyCollectionChanged)source).ObserveAddChangedItems<T>();

/// <summary>Observe CollectionChanged:Remove and take single item.</summary>
public static IObservable<T> ObserveRemoveChanged<T>(this ObservableCollection<T> source) =>
     ((INotifyCollectionChanged)source).ObserveRemoveChanged<T>();

/// <summary>Observe CollectionChanged:Remove.</summary>
public static IObservable<T[]> ObserveRemoveChangedItems<T>(this ObservableCollection<T> source) =>
    ((INotifyCollectionChanged)source).ObserveRemoveChangedItems<T>();

/// <summary>Observe CollectionChanged:Move and take single item.</summary>
public static IObservable<OldNewPair<T>> ObserveMoveChanged<T>(this ObservableCollection<T> source) =>
    ((INotifyCollectionChanged)source).ObserveMoveChanged<T>();

/// <summary>Observe CollectionChanged:Move.</summary>
public static IObservable<OldNewPair<T[]>> ObserveMoveChangedItems<T>(this ObservableCollection<T> source) =>
    ((INotifyCollectionChanged)source).ObserveMoveChangedItems<T>();

/// <summary>Observe CollectionChanged:Replace and take single item.</summary>
public static IObservable<OldNewPair<T>> ObserveReplaceChanged<T>(this ObservableCollection<T> source) =>
    ((INotifyCollectionChanged)source).ObserveReplaceChanged<T>();

/// <summary>Observe CollectionChanged:Replace.</summary>
public static IObservable<OldNewPair<T[]>> ObserveReplaceChangedItems<T>(this ObservableCollection<T> source) =>
    ((INotifyCollectionChanged)source).ObserveReplaceChangedItems<T>();

/// <summary>Observe CollectionChanged:Reset.</summary>
public static IObservable<Unit> ObserveResetChanged<T>(this ObservableCollection<T> source) =>
    ((INotifyCollectionChanged)source).ObserveResetChanged<T>();
```

## Observe `PropertyChanged` events of elements of `ObservableCollection` and `IFilteredReadOnlyObservableCollection`

Watch `PropertyChanged` events of elements of `ObservableCollection` and `IFilteredReadOnlyObservableCollection`. 
`ObserveElementProperty` extension method can observe specific property's `PropertyChanged` events.

```csharp
using Reactive.Bindings.Extensions;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ReactivePropertyEduApp
{
    public class Person : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var c = new ObservableCollection<Person>();
            c.ObserveElementProperty(x => x.Name)
                .Subscribe(x => Console.WriteLine($"Subscribe: {x.Instance}, {x.Property.Name}, {x.Value}"));

            var neuecc = new Person { Name = "neuecc" };
            var xin9le = new Person { Name = "xin9le" };
            var okazuki = new Person { Name = "okazuki" };

            Console.WriteLine("Add items");
            c.Add(neuecc);
            c.Add(xin9le);
            c.Add(okazuki);

            Console.WriteLine("Change okazuki name to Kazuki Ota");
            okazuki.Name = "Kazuki Ota";

            Console.WriteLine("Remove okazuki from collection");
            c.Remove(okazuki);

            Console.WriteLine("Change okazuki name to okazuki");
            okazuki.Name = "okazuki";
        }
    }
}
```

```
Add items
Subscribe: ReactivePropertyEduApp.Person, Name, neuecc
Subscribe: ReactivePropertyEduApp.Person, Name, xin9le
Subscribe: ReactivePropertyEduApp.Person, Name, okazuki
Change okazuki name to Kazuki Ota
Subscribe: ReactivePropertyEduApp.Person, Name, Kazuki Ota
Remove okazuki from collection
Change okazuki name to okazuki
```

If the target object's property type is `ReactiveProperty`, then use the `ObserveElementPropertyChanged` extension method.

```csharp
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ReactivePropertyEduApp
{
    public class Person
    {
        public ReactiveProperty<string> Name { get; }

        public Person(string name)
        {
            Name = new ReactiveProperty<string>(name);
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var c = new ObservableCollection<Person>();
            c.ObserveElementObservableProperty(x => x.Name)
                .Subscribe(x => Console.WriteLine($"Subscribe: {x.Instance}, {x.Property.Name}, {x.Value}"));

            var neuecc = new Person("neuecc");
            var xin9le = new Person("xin9le");
            var okazuki = new Person("okazuki");

            Console.WriteLine("Add items");
            c.Add(neuecc);
            c.Add(xin9le);
            c.Add(okazuki);

            Console.WriteLine("Change okazuki name to Kazuki Ota");
            okazuki.Name.Value = "Kazuki Ota";

            Console.WriteLine("Remove okazuki from collection");
            c.Remove(okazuki);

            Console.WriteLine("Change okazuki name to okazuki");
            okazuki.Name.Value = "okazuki";
        }
    }
}
```

```
Add items
Subscribe: ReactivePropertyEduApp.Person, Name, neuecc
Subscribe: ReactivePropertyEduApp.Person, Name, xin9le
Subscribe: ReactivePropertyEduApp.Person, Name, okazuki
Change okazuki name to Kazuki Ota
Subscribe: ReactivePropertyEduApp.Person, Name, Kazuki Ota
Remove okazuki from collection
Change okazuki name to okazuki
```

## `INotifyDataErrorInfo` extension methods

Convert `ErrorChanged` event to an `IObservable<DataErrorsChangedEventArgs>`.
It is a shortcut of `FromEvent` method.

```csharp
/// <summary>Converts ErrorsChanged to an observable sequence.</summary>
public static IObservable<DataErrorsChangedEventArgs> ErrorsChangedAsObservable<T>(this T subject)
    where T : INotifyDataErrorInfo =>
    Observable.FromEvent<EventHandler<DataErrorsChangedEventArgs>, DataErrorsChangedEventArgs>(
        h => (sender, e) => h(e),
        h => subject.ErrorsChanged += h,
        h => subject.ErrorsChanged -= h);
```

`ObserveErrorInfo` extension method is the version that raises the property value when the `ErrorChanged` event is raised.

## `Inverse`

Inverts the boolean value of an `IObservable<bool>` sequence.

```csharp
IObservable<bool> boolSequence = ...;
IObservable<bool> inversedBoolSequence = boolSequence.Inverse();
```

It is a same as the below code:

```csharp
IObservable<bool> boolSequence = ...;
IObservable<bool> inversedBoolSequence = boolSequence.Select(x => !x);
```




