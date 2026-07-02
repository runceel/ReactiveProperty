# 拡張メソッド

`Reactive.Bindings.Extensions` 名前空間は便利な拡張メソッドを提供します。

## `AddTo`

この名前空間の中でも、とても便利な拡張メソッドです。
メソッド チェーンの中で `IDisposable` インスタンスを集約できます。

このメソッドがない場合、インスタンス作成用と `IDisposable` インスタンス追加用の 2 つの文が必要です。

```csharp
// 初期化
var d = new CompositeDisposable();

Name = model.ObserveProperty(x => x.Name)
    .ToReadOnlyReactiveProperty();
d.Add(Name);

Age = model.ObserveProperty(x => x.Age)
    .ToReadOnlyReactiveProperty();
d.Add(Age);

// すべて破棄
d.Dispose();
```

`AddTo` 拡張メソッドのサンプル コードです。

```csharp
// 初期化
var d = new CompositeDisposable();

Name = model.ObserveProperty(x => x.Name)
    .ToReadOnlyReactiveProperty()
    .AddTo(d);

Age = model.ObserveProperty(x => x.Age)
    .ToReadOnlyReactiveProperty()
    .AddTo(d);

// すべて破棄
d.Dispose();
```

とても便利です。

## `CatchIgnore`

この拡張メソッドは例外を捕捉し、`Observable.Empty` を返します。

```csharp
source.CatchIgnore((Exception ex) => { ... error action ... })
    .Subscribe();
```

## `CombineLatestValuesAreAllXXXX`

2 つのメソッドを提供します。

- `CombineLatestValuesAreAllTrue`
- `CombineLatestValuesAreAllFalse`

これらは単なるショートカットです。

```csharp
/// <summary>
/// 各シーケンスの最新値がすべて true です。
/// </summary>
public static IObservable<bool> CombineLatestValuesAreAllTrue(
    this IEnumerable<IObservable<bool>> sources) =>
    sources.CombineLatest(xs => xs.All(x => x));


/// <summary>
/// 各シーケンスの最新値がすべて false です。
/// </summary>
public static IObservable<bool> CombineLatestValuesAreAllFalse(
    this IEnumerable<IObservable<bool>> sources) =>
    sources.CombineLatest(xs => xs.All(x => !x));
```

## DisposePreviousValue

この拡張メソッドは、`IObservable<T>` シーケンスの前の値に対して `Dispose` メソッドを呼び出します。

```csharp
var source = new Subject<string>();
var rrp = source.Select(x => new SomeDisposableClass(x))
    .DisposePreviousValue()
    .ToReadOnlyReactivePropertySlim();

source.OnNext("first"); // first の SomeDisposableClass が作成されます。
source.OnNext("second"); // second の SomeDisposableClass が作成され、first は破棄されます。
source.OnCompleted(); // second も破棄されます。
```

## `CanExecuteChangedAsObservable`

これは `ICommand` インターフェイスの拡張メソッドです。
`Observable.FromEvent` のショートカットです。

```csharp
/// <summary>CanExecuteChanged を Observable シーケンスに変換します。</summary>
public static IObservable<EventArgs> CanExecuteChangedAsObservable<T>(this T source)
    where T : ICommand =>
    Observable.FromEvent<EventHandler, EventArgs>(
        h => (sender, e) => h(e),
        h => source.CanExecuteChanged += h,
        h => source.CanExecuteChanged -= h);
```

## `INotifyCollectionChanged` 拡張メソッド

`CollectionChanged` イベントを `IObservable` に変換します。

```csharp
/// <summary>CollectionChanged:Remove を監視し、単一の項目を取り出します。</summary>
public static IObservable<T> ObserveRemoveChanged<T>(this INotifyCollectionChanged source) =>
    source.CollectionChangedAsObservable()
        .Where(e => e.Action == NotifyCollectionChangedAction.Remove)
        .Select(e => (T)e.OldItems[0]);

/// <summary>CollectionChanged:Remove を監視します。</summary>
public static IObservable<T[]> ObserveRemoveChangedItems<T>(this INotifyCollectionChanged source) =>
    source.CollectionChangedAsObservable()
        .Where(e => e.Action == NotifyCollectionChangedAction.Remove)
        .Select(e => e.OldItems.Cast<T>().ToArray());

/// <summary>CollectionChanged:Move を監視し、単一の項目を取り出します。</summary>
public static IObservable<OldNewPair<T>> ObserveMoveChanged<T>(this INotifyCollectionChanged source) =>
    source.CollectionChangedAsObservable()
        .Where(e => e.Action == NotifyCollectionChangedAction.Move)
        .Select(e => new OldNewPair<T>((T)e.OldItems[0], (T)e.NewItems[0]));

/// <summary>CollectionChanged:Move を監視します。</summary>
public static IObservable<OldNewPair<T[]>> ObserveMoveChangedItems<T>(this INotifyCollectionChanged source) =>
    source.CollectionChangedAsObservable()
        .Where(e => e.Action == NotifyCollectionChangedAction.Move)
        .Select(e => new OldNewPair<T[]>(e.OldItems.Cast<T>().ToArray(), e.NewItems.Cast<T>().ToArray()));

/// <summary>CollectionChanged:Replace を監視し、単一の項目を取り出します。</summary>
public static IObservable<OldNewPair<T>> ObserveReplaceChanged<T>(this INotifyCollectionChanged source) =>
    source.CollectionChangedAsObservable()
        .Where(e => e.Action == NotifyCollectionChangedAction.Replace)
        .Select(e => new OldNewPair<T>((T)e.OldItems[0], (T)e.NewItems[0]));

/// <summary>CollectionChanged:Replace を監視します。</summary>
public static IObservable<OldNewPair<T[]>> ObserveReplaceChangedItems<T>(this INotifyCollectionChanged source) =>
    source.CollectionChangedAsObservable()
        .Where(e => e.Action == NotifyCollectionChangedAction.Replace)
        .Select(e => new OldNewPair<T[]>(e.OldItems.Cast<T>().ToArray(), e.NewItems.Cast<T>().ToArray()));

/// <summary>CollectionChanged:Reset を監視します。</summary>
public static IObservable<Unit> ObserveResetChanged<T>(this INotifyCollectionChanged source) =>
    source.CollectionChangedAsObservable()
        .Where(e => e.Action == NotifyCollectionChangedAction.Reset)
        .Select(_ => new Unit());
```

## `ObservableCollection` 拡張メソッド

これは `INotifyPropertyChanged` 拡張メソッドの型安全なバージョンです。

```csharp
/// <summary>CollectionChanged:Add を監視し、単一の項目を取り出します。</summary>
public static IObservable<T> ObserveAddChanged<T>(this ObservableCollection<T> source) =>
    ((INotifyCollectionChanged)source).ObserveAddChanged<T>();

/// <summary>CollectionChanged:Add を監視します。</summary>
public static IObservable<T[]> ObserveAddChangedItems<T>(this ObservableCollection<T> source) =>
    ((INotifyCollectionChanged)source).ObserveAddChangedItems<T>();

/// <summary>CollectionChanged:Remove を監視し、単一の項目を取り出します。</summary>
public static IObservable<T> ObserveRemoveChanged<T>(this ObservableCollection<T> source) =>
     ((INotifyCollectionChanged)source).ObserveRemoveChanged<T>();

/// <summary>CollectionChanged:Remove を監視します。</summary>
public static IObservable<T[]> ObserveRemoveChangedItems<T>(this ObservableCollection<T> source) =>
    ((INotifyCollectionChanged)source).ObserveRemoveChangedItems<T>();

/// <summary>CollectionChanged:Move を監視し、単一の項目を取り出します。</summary>
public static IObservable<OldNewPair<T>> ObserveMoveChanged<T>(this ObservableCollection<T> source) =>
    ((INotifyCollectionChanged)source).ObserveMoveChanged<T>();

/// <summary>CollectionChanged:Move を監視します。</summary>
public static IObservable<OldNewPair<T[]>> ObserveMoveChangedItems<T>(this ObservableCollection<T> source) =>
    ((INotifyCollectionChanged)source).ObserveMoveChangedItems<T>();

/// <summary>CollectionChanged:Replace を監視し、単一の項目を取り出します。</summary>
public static IObservable<OldNewPair<T>> ObserveReplaceChanged<T>(this ObservableCollection<T> source) =>
    ((INotifyCollectionChanged)source).ObserveReplaceChanged<T>();

/// <summary>CollectionChanged:Replace を監視します。</summary>
public static IObservable<OldNewPair<T[]>> ObserveReplaceChangedItems<T>(this ObservableCollection<T> source) =>
    ((INotifyCollectionChanged)source).ObserveReplaceChangedItems<T>();

/// <summary>CollectionChanged:Reset を監視します。</summary>
public static IObservable<Unit> ObserveResetChanged<T>(this ObservableCollection<T> source) =>
    ((INotifyCollectionChanged)source).ObserveResetChanged<T>();
```

## `ObservableCollection` と `IFilteredReadOnlyObservableCollection` の要素の `PropertyChanged` イベントを監視する

`ObservableCollection` と `IFilteredReadOnlyObservableCollection` 内の要素の `PropertyChanged` イベントを監視します。
`ObserveElementProperty` 拡張メソッドでは、特定のプロパティの `PropertyChanged` イベントを監視できます。

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

対象オブジェクトのプロパティ型が `ReactiveProperty` の場合は、`ObserveElementObservableProperty` 拡張メソッドを使います。

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

## `INotifyDataErrorInfo` 拡張メソッド

`ErrorsChanged` イベントを `IObservable<DataErrorsChangedEventArgs>` に変換します。
`FromEvent` メソッドのショートカットです。

```csharp
/// <summary>ErrorsChanged を Observable シーケンスに変換します。</summary>
public static IObservable<DataErrorsChangedEventArgs> ErrorsChangedAsObservable<T>(this T subject)
    where T : INotifyDataErrorInfo =>
    Observable.FromEvent<EventHandler<DataErrorsChangedEventArgs>, DataErrorsChangedEventArgs>(
        h => (sender, e) => h(e),
        h => subject.ErrorsChanged += h,
        h => subject.ErrorsChanged -= h);
```

`ObserveErrorInfo` 拡張メソッドは、`ErrorsChanged` イベントが発生したときにプロパティ値を発行します。

## `Inverse`

`IObservable<bool>` シーケンスの bool 値を反転します。

```csharp
IObservable<bool> boolSequence = ...;
IObservable<bool> inversedBoolSequence = boolSequence.Inverse();
```

これは次のコードと同じです。

```csharp
IObservable<bool> boolSequence = ...;
IObservable<bool> inversedBoolSequence = boolSequence.Select(x => !x);
```

