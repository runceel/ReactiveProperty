# Detailed design — `ReactiveProperty.R3` migration bridge + migration skill

- **Status:** Draft for review
- **Audience:** Implementers / maintainers
- **Scope:** The `ReactiveProperty.R3` package, the `migrating-reactiveproperty-to-r3` skill, and the
  implementation policy (実装方針) for both.

> This document is the specification for the work and must be reviewed and approved before
> production code is written.

---

## 1. Overview and goals

The README recommends [R3](https://github.com/Cysharp/R3) for new development. `ReactiveProperty.R3`
is a small, **permanent, production-ready** bridge package that fills the genuine gaps between
ReactiveProperty and R3 (+ `ObservableCollections.R3`) so that an existing ReactiveProperty
application can migrate fully to R3.

Target experience: add the **skill** + **R3** + **`ReactiveProperty.R3`** to a project and request
migration → the migration completes. The skill rewrites everything R3 already provides (under its own
name) to native R3; anything R3 lacks is supplied by `ReactiveProperty.R3`. Users may keep depending
on `ReactiveProperty.R3` indefinitely.

### Design principles

1. **Gap-only.** If R3 or `ObservableCollections.R3` already provides it (even under a different
   name), it is *not* in the library — the skill rewrites it to native R3.
2. **R3-native shapes.** Public observable surfaces return R3 `Observable<T>` (not
   `System.IObservable<T>`); time-based features use `TimeProvider` (not `IScheduler`); shared
   command state uses R3 `ReactiveProperty<bool>`.
3. **Familiar names.** Mirror ReactiveProperty's type and method names so migrated code reads the
   same. The migration is mostly a `using` swap + a package swap for gap types.
4. **Minimal dependencies.** The package depends on **R3 only** (no `System.Reactive`, no
   MessagePipe).
5. **Behavioral parity.** Each feature reproduces the observable, threading and disposal semantics of
   its ReactiveProperty counterpart, verified by tests.

---

## 2. Gap analysis summary (R3 1.3.1 + ObservableCollections.R3 3.3.4)

Full per-feature, citation-backed analysis is the basis for the scope below.

| ReactiveProperty feature | R3 status | Disposition |
|---|---|---|
| `ObserveProperty(x => x.P)` | R3 `ObservePropertyChanged` | **Skill** (rename) |
| `ReactivePropertySlim<T>` / `ReadOnlyReactivePropertySlim<T>` | R3 `ReactiveProperty<T>` / `ReadOnlyReactiveProperty<T>` | **Skill** (rename) |
| `ReactiveProperty<T>` (binding) | R3 `BindableReactiveProperty<T>` | **Skill** (rename) |
| `ReactiveCommand` / `ReactiveCommandSlim` | R3 `ReactiveCommand` | **Skill** (rename) |
| `ToReadOnlyReactiveProperty` / `ToReadOnlyReactivePropertySlim` from observable | R3 `ToReadOnlyReactiveProperty` / `ToBindableReactiveProperty` | **Skill** (rename) |
| `IObservable<T>` ↔ `Observable<T>` | R3 `ToObservable` / `AsSystemObservable` | **Skill** (rename) |
| DataAnnotations validation on a property | R3 `EnableValidation()` | **Skill** (rename) — but see §4.3 for streams |
| `CollectionChangedAsObservable` / `ObserveAddChanged` / … on BCL `INotifyCollectionChanged` | **none** | **Library** §4.1 |
| `ObserveErrorInfo` / `ErrorsChangedAsObservable` / `ObserveHasErrors` / stream `SetValidateNotifyError` | **none** (only sync `Func<T,Exception?>`) | **Library** §4.3 |
| `ReadOnlyReactiveCollection<T>` / `ToReadOnlyReactiveCollection` (BCL source/stream + converter + auto-dispose) | partial (OC.R3 only on Cysharp `IObservableCollection<T>`) | **Library** §4.4 |
| `IFilteredReadOnlyObservableCollection` / `ToFilteredReadOnlyObservableCollection` (BCL source) | partial (same constraint) | **Library** §4.5 |
| `BooleanNotifier` | partial (no `TurnOn/TurnOff/SwitchValue`) | **Library** §4.6 |
| `CountNotifier` | **none** | **Library** §4.6 |
| `BusyNotifier` | **none** | **Library** §4.6 |
| `ScheduledNotifier<T>` | **none** | **Library** §4.6 |
| `ReactiveTimer` | **none** (only `Observable.Timer/Interval`) | **Library** §4.7 |
| `MessageBroker` / `AsyncMessageBroker` | **none** (successor is MessagePipe) | **Library** §4.8 |
| `AsyncReactiveCommand` (auto-disable while running) | partial (R3 uses `AwaitOperation`, no CanExecute toggle) | **Library** §4.9 |
| `AsyncReactiveCommand` shared CanExecute across commands | **none** | **Library** §4.9 |
| `WithSubscribe` (fluent) on commands | **none** | **Library** §4.9 |
| `ToReactivePropertyAsSynchronized` (two-way POCO ↔ property) | **none** (R3 one-way only) | **Library** §4.10 |

---

## 3. Packaging

| Item | Value |
|---|---|
| Package id / assembly | **`ReactiveProperty.R3`** (single package) |
| Target frameworks | `netstandard2.0;net8.0;net9.0;net10.0` (matches R3's reach; netstandard2.0 covers net472 consumers) |
| Dependencies | **`R3` only** (via Central Package Management — no version in the csproj) |
| Strong naming | `SignAssembly=true`, reuse `Source/ReactiveProperty.Core/key.snk` |
| Language / nullability | **`LangVersion=14.0` (C# 14)**, `Nullable=enable` (inherited from `Source/Directory.Build.props`) |
| Other | `GenerateDocumentationFile=true`, SourceLink (inherited from `Source/Directory.Build.props`) |
| Solution | Add to `ReactiveProperty.slnx` |

### 3.0 Repo-wide build settings (.NET 10 + C# 14)

This work also moves the rest of the repository to **.NET 10 + C# 14**:

- **`LangVersion` is raised to `14.0`** centrally in `Source/Directory.Build.props` (currently
  `12.0`). Because it is set explicitly, C# 14 syntax compiles on **every** target including
  `netstandard2.0` and `net472`. Runtime-dependent C# 14 features that need newer BCL support are
  only used where available (or via the existing polyfill pattern); purely syntactic features (e.g.
  the `field` keyword, `nameof`, collection expressions) are used freely. See the repo's
  `dotnet10-features` skill for the per-feature TFM guidance.
- All shipped packages already target `net10.0` (`ReactiveProperty.Core`,
  `ReactiveProperty` (NETStandard), `ReactiveProperty.Blazor`, `ReactiveProperty.WPF`), so no TFM
  change is required beyond raising `LangVersion`.
- New `ReactiveProperty.R3` follows the same settings. The new `ReactiveProperty.R3.Tests` targets
  `net10.0`.

### 3.1 Namespace map

Root namespace **`Reactive.Bindings.R3`**, mirroring ReactiveProperty's sub-namespaces:

| New namespace | Members |
|---|---|
| `Reactive.Bindings.R3` | `AsyncReactiveCommand`, `AsyncReactiveCommand<T>`, `ReactiveTimer`, `ReadOnlyReactiveCollection<T>`, `CollectionChanged<T>`, `ToReadOnlyReactiveCollection`, `ValidatableReactiveProperty<T>` (+ factory) |
| `Reactive.Bindings.R3.Notifiers` | `BooleanNotifier`, `CountNotifier` (+`CountChangedStatus`), `BusyNotifier`, `ScheduledNotifier<T>`, `MessageBroker`, `AsyncMessageBroker` (+ interfaces) |
| `Reactive.Bindings.R3.Extensions` | collection-changed extensions, `INotifyDataErrorInfo` extensions, `ToFilteredReadOnlyObservableCollection`, `ToReactivePropertyAsSynchronized`, command `WithSubscribe`/`ToAsyncReactiveCommand` |

> **Decision (confirmed by owner): `Reactive.Bindings.R3`.** An alternative considered was reusing
> the **exact original** namespaces (`Reactive.Bindings`, `Reactive.Bindings.Notifiers`,
> `Reactive.Bindings.Extensions`). That would make gap-type migration require *zero* `using` changes,
> but (a) types still return R3 `Observable<T>` so downstream operator calls need `using R3;` anyway,
> and (b) if a project transiently references both `ReactiveProperty` and `ReactiveProperty.R3` it
> gets `CS0433` ambiguous-type errors. The `.R3` namespace is safer and self-documenting, so it is
> adopted.

---

## 4. Public API surface (per feature)

All signatures below are the **proposed** public surface. `Observable<T>` means `R3.Observable<T>`.
Behavioral contracts mirror the cited ReactiveProperty source unless noted.

### 4.1 `INotifyCollectionChanged` observation — `Reactive.Bindings.R3.Extensions`

Genuine gap: OC.R3 only observes Cysharp's `IObservableCollection<T>`. We observe **any** BCL
`INotifyCollectionChanged` / `ObservableCollection<T>` / `ReadOnlyObservableCollection<T>`, returning
R3 `Observable<T>` (built with `Observable.FromEvent`).

```csharp
public static class INotifyCollectionChangedExtensions
{
    public static Observable<NotifyCollectionChangedEventArgs> CollectionChangedAsObservable<T>(this T source)
        where T : INotifyCollectionChanged;

    public static Observable<T>            ObserveAddChanged<T>(this INotifyCollectionChanged source);
    public static Observable<T[]>          ObserveAddChangedItems<T>(this INotifyCollectionChanged source);
    public static Observable<T>            ObserveRemoveChanged<T>(this INotifyCollectionChanged source);
    public static Observable<T[]>          ObserveRemoveChangedItems<T>(this INotifyCollectionChanged source);
    public static Observable<OldNewPair<T>>   ObserveMoveChanged<T>(this INotifyCollectionChanged source);
    public static Observable<OldNewPair<T[]>> ObserveMoveChangedItems<T>(this INotifyCollectionChanged source);
    public static Observable<OldNewPair<T>>   ObserveReplaceChanged<T>(this INotifyCollectionChanged source);
    public static Observable<OldNewPair<T[]>> ObserveReplaceChangedItems<T>(this INotifyCollectionChanged source);
    public static Observable<Unit>         ObserveResetChanged<T>(this INotifyCollectionChanged source); // R3.Unit
}
```

- Strongly-typed `ObservableCollection<T>` / `ReadOnlyObservableCollection<T>` overloads delegate to
  the `INotifyCollectionChanged` ones (parity with `ObservableCollectionExtensions`).
- `OldNewPair<T>` is ported (small readonly struct/record with `OldItem`/`NewItem`).
- `Unit` is R3's `Unit`.

#### 4.1.1 Element-property observation

Observe a property (or an observable property) of **each element** in a collection, re-subscribing as
elements are added/removed. Ported from `ObservableCollectionExtensions` + `CollectionUtilities`.
Implemented on top of R3's `ObservePropertyChanged` (uses `Func<,>` + `CallerArgumentExpression`,
**not** expression trees — the call-site lambda `x => x.Name` is identical, so migration is seamless
and there is no `Expression.Compile` cost on `netstandard2.0`).

```csharp
public static class INotifyCollectionChangedExtensions   // (same static class as §4.1)
{
    public static Observable<PropertyPack<TElement, TProperty>> ObserveElementProperty<TElement, TProperty>(
        this ObservableCollection<TElement> source,
        Func<TElement, TProperty> propertySelector,
        bool isPushCurrentValueAtFirst = true,
        [CallerArgumentExpression(nameof(propertySelector))] string? propertyName = null)
        where TElement : class, INotifyPropertyChanged;

    public static Observable<PropertyPack<TElement, TProperty>> ObserveElementObservableProperty<TElement, TProperty>(
        this ObservableCollection<TElement> source,
        Func<TElement, Observable<TProperty>> propertySelector)
        where TElement : class;

    public static Observable<SenderEventArgsPair<TElement, PropertyChangedEventArgs>> ObserveElementPropertyChanged<TElement>(
        this ObservableCollection<TElement> source)
        where TElement : class, INotifyPropertyChanged;

    // identical overloads for ReadOnlyObservableCollection<TElement>
}

public sealed class PropertyPack<TInstance, TValue>
{
    public TInstance     Instance { get; }
    public PropertyInfo  Property { get; }   // resolved from propertyName via reflection
    public TValue        Value { get; }
}

public sealed class SenderEventArgsPair<TSender, TEventArgs>
{
    public TSender    Sender { get; }
    public TEventArgs EventArgs { get; }
}
```

- `propertySelector` accepts a **single-level** element-property lambda (`x => x.Name`). `Property`
  is resolved from the captured `propertyName` via `typeof(TElement).GetProperty(name)`.
- `ObserveElementObservableProperty` projects each element's `Observable<TProperty>` member and flattens.

### 4.2 `OldNewPair<T>` — `Reactive.Bindings.R3`

```csharp
public readonly struct OldNewPair<T>
{
    public OldNewPair(T oldItem, T newItem);
    public T OldItem { get; }
    public T NewItem { get; }
}
```

### 4.3 Validation / error streams — `ValidatableReactiveProperty<T>` + INDEI extensions

R3 only offers synchronous `EnableValidation(Func<T,Exception?>)` and a `HasErrors` bool. Gaps:
stream/async validation, multiple-error aggregation, and **observable** error streams.

`Reactive.Bindings.R3.ValidatableReactiveProperty<T>` — standalone, mirrors RP's type, backed by an
R3 `ReactiveProperty<T>` and exposing R3 observables:

```csharp
public sealed class ValidatableReactiveProperty<T> : Observable<T>, INotifyPropertyChanged, INotifyDataErrorInfo, IDisposable
{
    public ValidatableReactiveProperty(T initialValue = default!, IEqualityComparer<T>? equalityComparer = null);

    public T Value { get; set; }
    public bool HasErrors { get; }
    public string[] GetErrors();
    public string  ErrorMessage { get; }                 // first error or ""

    public Observable<IReadOnlyList<string>> ObserveErrorChanged { get; }
    public Observable<bool>                  ObserveHasErrors { get; }
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    // Fluent validator registration (returns this) — mirrors RP SetValidateNotifyError
    public ValidatableReactiveProperty<T> SetValidateNotifyError(Func<T, string?> validate);                 // sync, single
    public ValidatableReactiveProperty<T> SetValidateNotifyError(Func<T, IEnumerable<string>?> validate);    // sync, multi
    public ValidatableReactiveProperty<T> SetValidateNotifyError(Func<Observable<T>, Observable<IEnumerable?>> validate); // stream/async — the gap
    public ValidatableReactiveProperty<T> SetValidateAttribute(Expression<Func<ValidatableReactiveProperty<T>>> selfSelector); // DataAnnotations, MemberName=Value

    public void ForceValidate();
    public void ForceNotify();
    public ReactiveProperty<T> ToReactiveProperty();     // bridge to native R3 (read/write)
}
```

INDEI extensions in `Reactive.Bindings.R3.Extensions` (work on **any** `INotifyDataErrorInfo`,
including R3 `BindableReactiveProperty<T>`):

```csharp
public static class INotifyDataErrorInfoExtensions
{
    public static Observable<DataErrorsChangedEventArgs> ErrorsChangedAsObservable<T>(this T subject)
        where T : INotifyDataErrorInfo;

    public static Observable<TProperty> ObserveErrorInfo<TSubject, TProperty>(
        this TSubject subject, Expression<Func<TSubject, TProperty>> propertySelector)
        where TSubject : INotifyDataErrorInfo;

    // Convenience for R3 BindableReactiveProperty<T> (which lacks an observable HasErrors)
    public static Observable<bool> ObserveHasErrors(this INotifyDataErrorInfo subject);
}
```

- DataAnnotations validation reproduces ReactiveProperty's context (`MemberName = "Value"`,
  `DisplayName` from `[Display]`).
- Initial-validation suppression: a simple `bool ignoreInitialValidationError` constructor flag
  (default `false` → validate the initial value, matching ReactiveProperty's default behavior).

### 4.4 `ReadOnlyReactiveCollection<T>` + `CollectionChanged<T>` — `Reactive.Bindings.R3`

Gap: build a projected, auto-disposing read-only collection from a **BCL** `ObservableCollection<T>`
or from an R3 change stream. OC.R3 cannot wrap a BCL collection, build from a raw change stream, or
auto-dispose elements.

```csharp
public readonly struct CollectionChanged<T>   // mirrors RP's CollectionChanged<T>
{
    public NotifyCollectionChangedAction Action { get; }
    public int    Index { get; }
    public T?     Value { get; }
    public IReadOnlyList<T>? Values { get; }
    public int    OldIndex { get; }
    // static factories: Add / Remove / Replace / Move / Reset / ResetWithSource
}

public sealed class ReadOnlyReactiveCollection<T> : ReadOnlyObservableCollection<T>, IDisposable
{
    // constructors take the source/stream + disposeElement + SynchronizationContext? raiseEventContext = null
    public void Dispose();   // disposes subscription; disposes elements when disposeElement = true
}
```

Factory extensions in `Reactive.Bindings.R3`:

```csharp
public static class ReadOnlyReactiveCollectionExtensions
{
    // from an R3 change stream
    public static ReadOnlyReactiveCollection<T> ToReadOnlyReactiveCollection<T>(
        this Observable<CollectionChanged<T>> self, bool disposeElement = true,
        SynchronizationContext? raiseEventContext = null);

    // from an R3 add-stream (+ optional reset)
    public static ReadOnlyReactiveCollection<T> ToReadOnlyReactiveCollection<T>(
        this Observable<T> self, Observable<Unit>? onReset = null, bool disposeElement = true,
        SynchronizationContext? raiseEventContext = null);

    // from a BCL ObservableCollection<T> / ReadOnlyObservableCollection<T>, with converter
    public static ReadOnlyReactiveCollection<T> ToReadOnlyReactiveCollection<T>(
        this ObservableCollection<T> self, bool disposeElement = true,
        SynchronizationContext? raiseEventContext = null);
    public static ReadOnlyReactiveCollection<TResult> ToReadOnlyReactiveCollection<TSource, TResult>(
        this ObservableCollection<TSource> self, Func<TSource, TResult> converter, bool disposeElement = true,
        SynchronizationContext? raiseEventContext = null);
    public static ReadOnlyReactiveCollection<T> ToReadOnlyReactiveCollection<T>(
        this ReadOnlyObservableCollection<T> self, bool disposeElement = true,
        SynchronizationContext? raiseEventContext = null);
    public static ReadOnlyReactiveCollection<TResult> ToReadOnlyReactiveCollection<TSource, TResult>(
        this ReadOnlyObservableCollection<TSource> self, Func<TSource, TResult> converter, bool disposeElement = true,
        SynchronizationContext? raiseEventContext = null);

    // raw change stream from a BCL collection
    public static Observable<CollectionChanged<T>> ToCollectionChanged<T>(this INotifyCollectionChanged self);
    public static Observable<CollectionChanged<T>> ToCollectionChanged<T>(this ObservableCollection<T> self);
    public static Observable<CollectionChanged<T>> ToCollectionChanged<T>(this ReadOnlyObservableCollection<T> self);
}
```

- **UI dispatch (✅ confirmed — Option B):** ReactiveProperty took an `IScheduler` (default = UI
  dispatcher). R3 has no `IScheduler`; the R3-native equivalent for thread affinity is
  `SynchronizationContext`. Every factory/constructor takes `SynchronizationContext? raiseEventContext
  = null`: when non-null, collection-changed callbacks are `Post`ed to it (so a background-thread
  source raises `CollectionChanged` on the UI thread); when `null`, events are raised synchronously on
  the producing thread. The skill maps `UIDispatcherScheduler.Default` /
  `ReactivePropertyScheduler.Default` → `SynchronizationContext.Current` (captured at the call site)
  and any other custom `IScheduler` → `manualReview`.
- `disposeElement = true` disposes `IDisposable` elements on Remove/Replace/Reset/Dispose (parity).

### 4.5 Filtered read-only collection — `Reactive.Bindings.R3`

```csharp
public interface IFilteredReadOnlyObservableCollection<out TElement> : IReadOnlyList<TElement>, INotifyCollectionChanged, INotifyPropertyChanged, IDisposable
    where TElement : class, INotifyPropertyChanged
{
    void Refresh(Func<TElement, bool> filter);
}

public static class FilteredReadOnlyObservableCollectionExtensions
{
    public static IFilteredReadOnlyObservableCollection<TElement> ToFilteredReadOnlyObservableCollection<TCollection, TElement>(
        this TCollection source, Func<TElement, bool> filter)
        where TCollection : INotifyCollectionChanged, IList<TElement>
        where TElement : class, INotifyPropertyChanged;
}
```

Port of RP's `FilteredReadOnlyObservableCollection`. Element-property change hooks reuse the
`ObserveElementProperty`/`ObserveElementPropertyChanged` machinery from §4.1.1, so the filter can
re-evaluate when a watched element property changes (full parity with RP).

### 4.6 Notifiers — `Reactive.Bindings.R3.Notifiers`

Each extends R3 `Observable<T>` (so `notifier.Subscribe(...)` is mechanical) and implements
`INotifyPropertyChanged`. Backed internally by an R3 `ReactiveProperty`/`Subject`.

```csharp
public class BooleanNotifier : Observable<bool>, INotifyPropertyChanged
{
    public BooleanNotifier(bool initialValue = false);
    public bool Value { get; set; }
    public void TurnOn();
    public void TurnOff();
    public void SwitchValue();
}

public enum CountChangedStatus { Increment, Decrement, Empty, Max }

public class CountNotifier : Observable<CountChangedStatus>, INotifyPropertyChanged
{
    public CountNotifier(int max = int.MaxValue);
    public int Max { get; }
    public int Count { get; }
    public IDisposable Increment(int incrementCount = 1);  // auto-decrements on Dispose
    public void Decrement(int decrementCount = 1);
}

public class BusyNotifier : Observable<bool>, INotifyPropertyChanged
{
    public bool IsBusy { get; }
    public IDisposable ProcessStart();   // thread-safe reference count; OnNext(IsBusy) on subscribe
}

public class ScheduledNotifier<T> : Observable<T>, IProgress<T>
{
    public ScheduledNotifier(TimeProvider? timeProvider = null);
    public void Report(T value);
    public IDisposable Report(T value, TimeSpan dueTime);
    public IDisposable Report(T value, DateTimeOffset dueTime);
}
```

- `ScheduledNotifier<T>` replaces `IScheduler` with `TimeProvider` (default
  `ObservableSystem.DefaultTimeProvider` / `TimeProvider.System`). `Report(value)` is immediate.
- All counter/flag mutations preserve RP's thread-safety (`lock`) and `INotifyPropertyChanged`
  raises.

### 4.7 `ReactiveTimer` — `Reactive.Bindings.R3`

```csharp
public class ReactiveTimer : Observable<long>, INotifyPropertyChanged, IDisposable
{
    public ReactiveTimer(TimeSpan interval, TimeProvider? timeProvider = null);
    public TimeSpan Interval { get; set; }
    public bool IsEnabled { get; }
    public void Start();
    public void Start(TimeSpan dueTime);
    public void Stop();
    public void Reset();   // stop + reset count
}
```

- Hot, stoppable/restartable; emits an incrementing `long`. Backed by `TimeProvider.CreateTimer`
  (or R3 `Observable.Timer` with the supplied `TimeProvider`) + an R3 `Subject<long>`.
- **Interval-change parity gap (ADR 0002):** The `period` of the underlying `ITimer` is
  captured once at `Start()`. Setting `Interval` while the timer is running raises
  `PropertyChanged` but the new period only takes effect on the next `Start()`. This differs
  from the legacy implementation, which read `Interval` on every tick. The trade-off was
  made in favour of deterministic behaviour under `FakeTimeProvider`; callers that need the
  new interval applied immediately should call `Stop()` then `Start()`.

### 4.8 `MessageBroker` / `AsyncMessageBroker` — `Reactive.Bindings.R3.Notifiers`

Direct, self-contained port of RP's broker (no R3 needed except `ToObservable` returns R3
`Observable<T>`):

```csharp
public interface IMessagePublisher  { void Publish<T>(T message); }
public interface IMessageSubscriber { IDisposable Subscribe<T>(Action<T> action); }
public interface IMessageBroker : IMessagePublisher, IMessageSubscriber { }

public interface IAsyncMessagePublisher  { Task PublishAsync<T>(T message); }
public interface IAsyncMessageSubscriber { IDisposable Subscribe<T>(Func<T, Task> asyncAction); }
public interface IAsyncMessageBroker : IAsyncMessagePublisher, IAsyncMessageSubscriber { }

public class MessageBroker : IMessageBroker, IDisposable      { public static readonly IMessageBroker Default; }
public class AsyncMessageBroker : IAsyncMessageBroker, IDisposable { public static readonly IAsyncMessageBroker Default; }

public static class MessageBrokerExtensions
{
    public static Observable<T> ToObservable<T>(this IMessageSubscriber subscriber);
}
```

### 4.9 Commands — `Reactive.Bindings.R3` + extensions

Gap: R3's async command toggles via `AwaitOperation`, not a CanExecute flag, and has no shared
CanExecute or fluent `WithSubscribe`. Port RP's `AsyncReactiveCommand` semantics over R3 types.

```csharp
public class AsyncReactiveCommand<T> : ICommand, IDisposable
{
    public AsyncReactiveCommand();                                                   // CanExecute auto false while running
    public AsyncReactiveCommand(Observable<bool> canExecuteSource);
    public AsyncReactiveCommand(Observable<bool> canExecuteSource, ReactiveProperty<bool> sharedCanExecute);
    public AsyncReactiveCommand(ReactiveProperty<bool> sharedCanExecute);            // shared across commands

    public bool CanExecute();
    public Task ExecuteAsync(T parameter);
    public void Execute(T parameter);
    public IDisposable Subscribe(Func<T, Task> asyncAction);
    public event EventHandler? CanExecuteChanged;
}

public class AsyncReactiveCommand : AsyncReactiveCommand<object?>
{
    public void Execute();
    public Task ExecuteAsync();
    public IDisposable Subscribe(Func<Task> asyncAction);
}

public static class AsyncReactiveCommandExtensions
{
    public static AsyncReactiveCommand    ToAsyncReactiveCommand(this Observable<bool> canExecuteSource);
    public static AsyncReactiveCommand    ToAsyncReactiveCommand(this Observable<bool> canExecuteSource, ReactiveProperty<bool> sharedCanExecute);
    public static AsyncReactiveCommand<T> ToAsyncReactiveCommand<T>(this Observable<bool> canExecuteSource);
    public static AsyncReactiveCommand<T> ToAsyncReactiveCommand<T>(this Observable<bool> canExecuteSource, ReactiveProperty<bool> sharedCanExecute);
    public static AsyncReactiveCommand    ToAsyncReactiveCommand(this ReactiveProperty<bool> sharedCanExecute);
    public static AsyncReactiveCommand<T> ToAsyncReactiveCommand<T>(this ReactiveProperty<bool> sharedCanExecute);

    public static AsyncReactiveCommand    WithSubscribe(this AsyncReactiveCommand self, Func<Task> asyncAction, Action<IDisposable>? postProcess = null);
    public static AsyncReactiveCommand<T> WithSubscribe<T>(this AsyncReactiveCommand<T> self, Func<T, Task> asyncAction, Action<IDisposable>? postProcess = null);
}
```

Fluent `WithSubscribe` for **R3's own** `ReactiveCommand` (the partial gap that lets sync commands
migrate without restructuring into constructor injection):

```csharp
public static class ReactiveCommandExtensions
{
    public static ReactiveCommand<T> WithSubscribe<T>(this ReactiveCommand<T> self, Action<T> action, Action<IDisposable>? postProcess = null);
}
```

- Behavioral contract (mirrors RP): on `ExecuteAsync`, set `canExecute.Value = false`, run all
  subscribed async actions (`Task.WhenAll` when >1), restore `true` in `finally`; `Dispose` locks
  CanExecute to `false`. Shared `ReactiveProperty<bool>` makes all sharing commands disable together.

### 4.10 Two-way sync — `ToReactivePropertyAsSynchronized` — `Reactive.Bindings.R3.Extensions`

Gap: R3's `ToBindableReactiveProperty` is one-way. Port the two-way helper, producing an R3
`BindableReactiveProperty<T>` two-way bound to a POCO `INotifyPropertyChanged` property.

Follows R3's own convention: property selectors are `Func<,>` + `CallerArgumentExpression` (no
expression trees / `Expression.Compile`; `netstandard2.0`-safe). **Nested paths** reuse R3's chained
multi-selector form — pass one single-level selector per hop (R3 `ObservePropertyChanged` supports up
to 3 hops), so e.g. `vm.ToReactivePropertyAsSynchronized(x => x.Child, c => c.Name)` replaces RP's
`x => x.Child.Name`. Write-back resolves the parent via the leading selectors and sets the leaf
property by name via reflection.

```csharp
public static class INotifyPropertyChangedExtensions
{
    // single-level
    public static BindableReactiveProperty<TProperty> ToReactivePropertyAsSynchronized<TSubject, TProperty>(
        this TSubject subject,
        Func<TSubject, TProperty> propertySelector,
        bool pushCurrentValueOnSubscribe = true,
        [CallerArgumentExpression(nameof(propertySelector))] string? propertyName = null)
        where TSubject : INotifyPropertyChanged;

    public static BindableReactiveProperty<TResult> ToReactivePropertyAsSynchronized<TSubject, TProperty, TResult>(
        this TSubject subject,
        Func<TSubject, TProperty> propertySelector,
        Func<TProperty, TResult> convert,
        Func<TResult, TProperty> convertBack,
        bool pushCurrentValueOnSubscribe = true,
        [CallerArgumentExpression(nameof(propertySelector))] string? propertyName = null)
        where TSubject : INotifyPropertyChanged;

    public static BindableReactiveProperty<TResult> ToReactivePropertyAsSynchronized<TSubject, TProperty, TResult>(
        this TSubject subject,
        Func<TSubject, TProperty> propertySelector,
        Func<Observable<TProperty>, Observable<TResult>> convert,
        Func<Observable<TResult>, Observable<TProperty>> convertBack,
        bool pushCurrentValueOnSubscribe = true,
        [CallerArgumentExpression(nameof(propertySelector))] string? propertyName = null)
        where TSubject : INotifyPropertyChanged;

    // nested (2 hops) — mirrors R3's chained ObservePropertyChanged overload
    public static BindableReactiveProperty<TProperty2> ToReactivePropertyAsSynchronized<TSubject, TProperty1, TProperty2>(
        this TSubject subject,
        Func<TSubject, TProperty1> propertySelector1,
        Func<TProperty1, TProperty2> propertySelector2,
        bool pushCurrentValueOnSubscribe = true,
        [CallerArgumentExpression(nameof(propertySelector1))] string? propertyName1 = null,
        [CallerArgumentExpression(nameof(propertySelector2))] string? propertyName2 = null)
        where TSubject : INotifyPropertyChanged
        where TProperty1 : INotifyPropertyChanged;
    // (3-hop overload follows the same shape, matching R3's 3-selector ObservePropertyChanged)
}
```

- Read side delegates to R3 `ObservePropertyChanged` (single- or chained-selector). Write side pushes
  the `BindableReactiveProperty` value back: single-level sets `subject.<prop>`; nested resolves the
  parent (`subject` → … → `TPropertyN-1`) then sets the leaf by name. If an intermediate is null the
  write is skipped (parity with RP's null-tolerant path).
- `IScheduler` raise-scheduler overloads and `ReactivePropertyMode`/`ignoreValidationErrorValue`
  parameters are dropped (R3 has no scheduler arg; distinctness is R3's default).

---

## 5. Migration skill (`skills/migrating-reactiveproperty-to-r3`)

The skill is **execution-centric**: it drives code rewrites from a mapping table, then surfaces only
the spots that need manual review.

### 5.1 `SKILL.md` flow

1. Ensure the project references `R3` and `ReactiveProperty.R3`, and drops `ReactiveProperty` /
   `ReactiveProperty.Core` / platform packages.
2. For each ReactiveProperty symbol, apply its rule:
   - `r3-direct` → rewrite to the native R3 API (rename + `using R3;`).
   - `reactiveproperty-r3` → rewrite to the `ReactiveProperty.R3` API (the gap bridge).
   - `manualReview` → leave a concise, located note; do not guess.
3. Fix `using` directives (`Reactive.Bindings*` → `R3` and/or `Reactive.Bindings.R3*`).
4. Build & run the project's existing tests; report build/test status and the manual-review list.

### 5.2 `references/rules.json` schema

```json
{
  "schemaVersion": 2,
  "rules": [
    {
      "ruleId": "RP-NOTIFY-BUSY",
      "symbol": "Reactive.Bindings.Notifiers.BusyNotifier",
      "matchKind": "TypeReference",
      "target": "reactiveproperty-r3",
      "replacement": "Reactive.Bindings.R3.Notifiers.BusyNotifier",
      "usingAdd": ["Reactive.Bindings.R3.Notifiers"],
      "usingRemove": ["Reactive.Bindings.Notifiers"],
      "manualReview": null,
      "notes": "Same API; subscribe yields R3 Observable<bool>."
    }
  ]
}
```

Fields: `ruleId`, `symbol`, `matchKind` (`TypeReference` | `MethodInvocation` | `ObjectCreation`),
`target` (`r3-direct` | `reactiveproperty-r3` | `manualReview`), `replacement`, `usingAdd`,
`usingRemove`, `manualReview` (string or null), `notes`.

The rule set covers every row in §2 (both the `r3-direct` renames and the `reactiveproperty-r3`
gaps), including element-property observation and nested property paths (both fully supported —
§4.1.1, §4.10). The remaining `manualReview` cases are narrow: custom `IScheduler` arguments that are
neither the UI dispatcher nor the default (collection dispatch, §4.4) and any RP API not enumerated
in §4.

---

## 6. Implementation policy (実装方針)

- **TDD, Red → Green → Refactor** (mandatory; MSTest). Each §4 feature starts with a failing test.
  Not done until `dotnet test ReactiveProperty.slnx` passes on Windows.
- **Test project:** `Test/ReactiveProperty.R3.Tests` (`net10.0`), MSTest + bundled
  `ChainingAssertion` (`.Is(...)`). Time-based features use `FakeTimeProvider`
  (`Microsoft.Extensions.TimeProvider.Testing`, added to `Directory.Packages.props`). Use R3's
  testing helpers where natural.
- **Language:** C# 14 (`LangVersion=14.0`, inherited centrally). Use C# 14 syntax freely; for
  features needing newer BCL support, follow the per-TFM guidance in the `dotnet10-features` skill.
- **Validation type:** implement `ValidatableReactiveProperty<T>` as a **standalone** type backed by
  R3 (a wrapper, not a subclass of `BindableReactiveProperty<T>`) to avoid sealed-type and
  reentrancy issues.
- **Time:** `TimeProvider` everywhere a scheduler was used; default to
  `ObservableSystem.DefaultTimeProvider`.
- **Threading & disposal:** preserve ReactiveProperty's `lock`-based thread-safety (notifiers,
  broker, commands) and disposal semantics (element auto-dispose, `Dispose` idempotency, post-dispose
  no-ops).
- **Framework floor:** keep code `netstandard2.0`/`net472`-compatible (guard modern-only APIs; e.g.
  `TimeProvider` is available via the `Microsoft.Bcl.TimeProvider` polyfill if R3 doesn't already
  surface it on netstandard2.0 — verify during implementation).
- **Signing/docs:** strong-name sign with the existing key; XML doc comments on all public APIs.

### Behavior-template test matrix (minimum)

1. Initial-value validation suppressed when `ignoreInitialValidationError`.
2. `HasErrors` transitions with async/stream validation.
3. Multiple errors aggregated with stream validation.
4. `CanExecute` false while running, true on completion.
5. Two commands disable together via shared CanExecute.
6. `Execute` is a no-op after `Dispose`.
7. `CanExecuteChanged` fires on `canExecuteSource` updates.
8. Collection Add/Remove/Replace/Move/Reset projection diff matches.
9. Elements disposed when `disposeElement = true`.
10. `CollectionChangedAsObservable` & `Observe*Changed` emit on BCL `ObservableCollection<T>` ops.
11. `BusyNotifier` reference counting; `IsBusy` observable replays current on subscribe.
12. `CountNotifier` Increment/Decrement/Max/Empty status sequence and `Increment` auto-decrement.
13. `ReactiveTimer` Start/Stop/Reset with `FakeTimeProvider`.
14. `ScheduledNotifier.Report(value, dueTime)` fires after the delay with `FakeTimeProvider`.
15. `MessageBroker`/`AsyncMessageBroker` publish/subscribe/unsubscribe and `ToObservable`.
16. `ToReactivePropertyAsSynchronized` writes back to the POCO and reflects POCO changes.
17. `ObserveElementProperty`/`ObserveElementPropertyChanged` track per-element property changes across
    add/remove, and the filtered collection re-evaluates on a watched element-property change.
18. Nested `ToReactivePropertyAsSynchronized` (chained selectors) reads/writes a 2-hop path and tolerates
    a null intermediate.

---

## 7. ADR

Recorded as [ADR 0003 — *Ship a permanent minimal `ReactiveProperty.R3` migration bridge and a
migration skill*](../adr/0003-reactiveproperty-r3-migration-bridge.md) (`Accepted`). It
cross-references this design document and captures the namespace decision (§3.1), the repo-wide
move to .NET 10 + C# 14 (§3.0), and the collection-dispatch decision (§4.4) under *Decision*.

## 8. Acceptance criteria

- `dotnet build ReactiveProperty.slnx -c Release` succeeds with `ReactiveProperty.R3` (+ tests) in the
  solution, all projects compiling as C# 14.
- `dotnet test ReactiveProperty.slnx` is green, covering the §6 matrix.
- The skill's `rules.json` covers every §2 row.
- One representative end-to-end migration (a small ViewModel exercising notifiers, validation,
  async commands and collections) builds and passes via `ReactiveProperty.R3`.

## 9. Decisions

1. **Namespace** — ✅ **Confirmed `Reactive.Bindings.R3`** (§3.1).
2. **Build target** — ✅ **Confirmed .NET 10 + C# 14.** `LangVersion` raised to `14.0` centrally
   (explicit, so it applies on `netstandard2.0`/`net472` too); shipped packages already target
   `net10.0` (§3.0).
3. **Collection UI dispatch** — ✅ **Confirmed Option B.** Every `ReadOnlyReactiveCollection` factory
   and constructor takes an optional `SynchronizationContext? raiseEventContext = null`; non-null
   `Post`s collection-changed events to it, `null` raises synchronously (§4.4).
4. **Element-property observation** — ✅ **In scope.** `ObserveElementProperty` /
   `ObserveElementObservableProperty` / `ObserveElementPropertyChanged` are implemented on R3's
   `ObservePropertyChanged` and also power the filtered collection (§4.1.1, §4.5).
5. **Nested property paths** — ✅ Implemented via R3's **chained multi-selector** convention
   (one single-level selector per hop, up to 3), not expression-tree paths (§4.10). No `manualReview`.
