/*--------------------------------------------------------------------------
 * ReactiveProperty
 * ver 0.3.2.0 (Nov. 25th, 2011)
 *
 * created and maintained by neuecc <ils@neue.cc - @neuecc/Twitter>
 * licensed under Microsoft Public License(Ms-PL)
 * http://reactiveproperty.codeplex.com/
 *--------------------------------------------------------------------------*/

---Description---

ReactiveProperty is MVVM and Asynchronous Extensions for Reactive Extensions.

* ReactiveProperty - Two-way bindable IObservable, from V to VM and VM to V
* ReactiveCommand - Convert observable condition sequence to ICommand
* Easy to use asynchronous extension for WebClient/WebRequest/WebResponse/Stream
* Typesafe convert INotifyPropertyChanged to ReactiveProperty
* Event to ReactiveProperty Blend behavior
* There means V -> VM -> M -> VM -> V completely connected in Reactive.

Note:
ReactiveProperty is not replace existing MVVM Framework.
ReactiveProperty no provides ViewModelBase, Messenger, etc.
I recommend that use MVVM Framework together.

---Bin/Assembly---

Rx_Stable/Rx_Experimental
|-NET40 - .NET 4.0 Client Profile
|-SL4 - Silverlight 4
|-SL5 - Silverlight 5
|-WP7.Rx-Main - Windows Phone 7.1

WP7
|- Windows Phone 7.1(no requires Rx-Main. This depends Microsoft.Phone.Reactive)

---Dependency---

Currently target Rx version is
Build 1.0.10621(Stable, Rx-Main on NuGet),
Build 1.1.11111(Experimental, Rx_Experimental-Main on NuGet)
or WP7(Microsoft.Phone.Reactive)
Codeplex.Reactive.Interactivity namespace and under classes,
depends System.Windows.Interactivity(Blend SDK)

---NuGet Installation---

Install-Package ReactiveProperty
-> NET40, SL4, SL5, WP7.Rx-Main | dependencies Rx-Main

Install-Package ReactiveProperty-Experimental
-> NET40, SL4, SL5, WP7.Rx-Main | dependencies Rx_Experimental-Main

Install-Package ReactiveProperty-WP7
-> WP7(Microsoft.Phone.Reactive)

---Snippet---

rprop ReactiveProperty.snippet
-> ReactiveProperty<T> PropertyName { get; private set; }

rcomm ReactiveCommand.snippet
-> ReactiveCommand CommandName { get; private set; }

rcommg ReactiveCommandGeneric.snippet
-> ReactiveCommand<T> CommandName { get; private set; }

rcoll ReactiveCollection.snippet
-> ReactiveCollection<T> CollectionName { get; private set; }

---Namespace and Description---

Codeplex.Reactive
-> ReacitveProperty - two-way bindable IObservable
-> ReactiveCommand - declaratively notify CanExecute from inside
-> ReactiveCollection - IObservable as ObservableCollection(and operate on IScheduler)
-> ReactiveTimer - Schedulable and hot(stoppable/continuable) timer.
-> UIDispatcherScheduler - Schedule on UIDispatcher, if access same thread schedule immediately.

Codeplex.Reactive.Asynchronous
-> Extension Methods for asynchronous operation(WebClient, WebRequest, WebResponse, Stream)

Codeplex.Reactive.Extensions
-> Extension Methods for interface(IObservble<T>, INotifyPropertyChanged, etc...)

Codeplex.Reactive.Interactivity
-> EventToReactive - Trigger that converts UIEvent to ReactiveProperty.

Codeplex.Reactive.Notifiers
-> ScheduledNotifier - Notify value on scheduler(use with asynchronous progress report).
-> CountNotifier - Notify event of count signals(this is thraed safe).
-> BooleanNotifier - Notify boolean flag.

Codeplex.Reactive.Helpers
-> SerializeHelper - Pack and unpack ReactiveProperty values.

Standard usage and all API references details, see project home.

---Source Info---

Solution file is for Visual Studio 2010.
Version control under Mercurial.
External library reference under NuGet.
Using Code Contracts(except WP7) binary rewrite.
Unit test using MSTest and mock library is Moles.
Auto generate unit test using Pex.
Assert helper using ChainingAssertion http://chainingassertion.codeplex.com/

---Special Thanks---

Icon design by @ocazuco.

---History---

2011-11-25 ver 0.3.2.0
    Add
        EventToReactive add IgnoreEventArgs property
        ReactiveProperty.FromObject add converter overload
    Fix
        Fix OnErrorRetry no work on OnCompleted
        Fix Stream.ReadLineAsync no work infinity stream
    Change
        Pairwise improved performance

2011-11-21 ver 0.3.1.0
    Add
        EventToReactive Convert(Func<object, object>) DependencyProperty
    Fix
        EventToReactive sample add Convert example
        Remove CombineLatest overloads in Rx-Experimental
    Remove
        ReactiveCollection.GetEnumerableOnScheduler

2011-11-20 ver 0.3.0.0
    Add
        ReactiveProperty.FromObject - Create OneWayToSource synchronized ReactiveProperty
        ToReactivePropertyAsSynchronized - Create TwoWay synchronized ReactiveProperty
        ReactiveTimer - Schedulable and hot(stoppable/continuable) timer
        BooleanNotifier - Notify boolean flag
        ForceValidate - Call ReactiveProperty's IDataErrorInfo validation
        GetOnScheduler, GetEnumerableOnScheduler - ReactiveCollection method
        CatchIgnore - IObservable<T> Extension
        Pairwise - IObservable<T> Extension
        CombineLatestValuesAreAllTrue - IEnumerable<IObservable<bool>> Extension
    Fix
        Fix bugs, WebRequestExtensions.UploadValues - values no concatenate "&"
        No crash in silverlight design view
        No throw exception when call dispose multiple in ReactiveProperty and ReactiveCommand
    Change
        ReactivePropertyMode's default changes to DistinctUntilChanged|RaiseLatestValueOnSubscribe
        (Changed:ver0.2 default behavior is DistinctUntilChanged only)
        Namespace changed - Codeplex.Reactive.Notifier -> Codeplex.Reactive.Notifiers
        Namespace changed - Codeplex.Reactive.Serialization -> Codeplex.Reactive.Helpers
        Name changed - IValue -> IReactiveValue
        Remove ReactiveProperty's parentRaisePropertyChanged overloads
        Remove CombineLatest overloads in Rx-Experimental

2011-10-17 ver 0.2.0.0
    Add
        Support Rx-Experimental
        INotifyPropertyChangedExtensions.ObserveProperty isPushCurrentValueAtFirst overload
        (Changed:default behavior is true, ver.0.1 was false)
        INotifyPropertyChangingExtensions(WPF/WP7)
        ReactiveCommand.Execute() overload
        ReactiveCollection add AddOnScheduler, ClearOnScheduler, etc...
    Change
        ReactiveCommand<T>.Dispose() send OnCompleted to subscribers
        ReactiveCollection remove notify on scheduler

2011-10-06 ver 0.1.0.0
    Initial Release