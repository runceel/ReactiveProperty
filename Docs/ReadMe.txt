/*--------------------------------------------------------------------------
 * ReactiveProperty
 * ver 0.4.5.0 (May. 07th, 2014)
 *
 * created and maintained by neuecc <ils@neue.cc - @neuecc/Twitter>, xin9le<@xin9le/Twitter>, okazuki<@okazuki/Twitter>
 * licensed under Microsoft Public License(Ms-PL)
 * http://reactiveproperty.codeplex.com/
 *--------------------------------------------------------------------------*/

---Description---

ReactiveProperty is MVVM and Asynchronous Extensions for Reactive Extensions.

* ReactiveProperty - Two-way bindable IObservable, from V to VM and VM to V
* ReactiveCommand - Convert observable condition sequence to ICommand
* Typesafe convert INotifyPropertyChanged to ReactiveProperty
* Event to ReactiveProperty Blend behavior
* There means V -> VM -> M -> VM -> V completely connected in Reactive.

Note:
ReactiveProperty is not replace existing MVVM Framework.
ReactiveProperty no provides ViewModelBase, Messenger, etc.
I recommend that use MVVM Framework together.

---Bin/Assembly---
Rx-Main
|-net4 - .NET 4
|-net45 - .NET 4.5, .NET 4.5.1
|-netcore451 - Windows store app(Windows 8.1)
|-portable-net45+win81 - Portable Class Library .NET 4.5, .NET 4.5.1, Windows store app(Windows 8.1)
|-portable-net45+wp8-win81 - Portable Class Library .NET 4.5, .NET 4.5.1, Windows Phone 8, Windows store app(Windows 8.1)
|-wp8 - Windows Phone 8
|-Xamarin.Android
|-Xamarin.iOS
|-wpa81 - Windows Phone 8.1

---Dependency---

Currently target Rx version is
Build Rx-Main 2.2.0 on NuGet.
ReactiveProperty.Platform.* depends
Unofficial.Blend.Interactivity 1.0.0 on NuGet(WP8, .NET 4.5),
Behaviors SDK (XAML)(Windows store app(Windows 8.1)).

---NuGet Installation---

Install-Package ReactiveProperty
-> NET45, WP8, WinRT8.1 | dependencies Rx-Main, Unofficial.Blend.Interactivity

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

Codeplex.Reactive.Binding
-> Extension Methods for ReactiveProperty<T>. Code based data binding.

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

Solution file is for Visual Studio 2013 Update 2 RC.
Version control under Git.
External library reference under NuGet.
Unit test using MSTest and mock library is Fakes.
Assert helper using ChainingAssertion http://chainingassertion.codeplex.com/

---Special Thanks---

Icon design by @ocazuco.

---History---
2014/05/07 ver 0.4.5.0
	Add
		Default constructor at ReactiveProperty.

2014/05/06 ver 0.4.5-beta1
	Add
		Support platform .NET Framework 4 Client Profile.

2014/05/03 ver 0.4.4.0
	Add
		ReadOnlyReactiveCollection.

2014/04/30 ver 0.4.3.0
	Info
		Universal Windows app support.

2014-04-12 ver 0.4.2.1
    Info
		Xamarin.Android and Xamarin.iOS 4.12.3 or highter. detail.
			http://docs.xamarin.com/releases/android/xamarin.android_4/xamarin.android_4.12/#3
			https://bugzilla.xamarin.com/show_bug.cgi?id=18024

2014-04-11 ver 0.4.2
    no change.

2014-04-11 ver 0.4.2.beta5
	Fix
		target platform WPA81 library.

2014-04-11 ver 0.4.2.beta4
	Add
		Windows Phone Runtime support.
	Change
		Remove System.Runtime.InteropServices.WindowsRuntime.dll from Xamarin.Android(please use alpha channel)
		Remove System.Runtime.InteropServices.WindowsRuntime.dll and System.Dynamic.Runtime.dll from Xamarin.iOS(please use alpha channel)

2014-04-04 ver 0.4.2.beta3
	Add
		Codeplex.Reactive.Binding namespace. Bind to ReactiveProperty to POCO property.
		ReactiveCommand add ToEventHandler method.

2014-04-04 ver 0.4.2.beta2
	Add
		Xamarin Android and iOS support.

2014-04-02 ver 0.4.2.beta
	Change
		ReactiveProperty SetValidationNotifyError methods behavior. ObserveErrorChanges property change IO<object> to IO<IE>.
	Add
		ReactiveProperty add string-specific SetValidationNotifyError methods.
		ReactiveProperty add Task base SetValidationNotifyError methods.

2014-02-02 ver 0.4.1.0
    Change
        ReactiveProperty ObserveErrorChanged property OnNext value type change Exception[] to string at DataAnnotations validation error.
    Add
        ReactiveProperty add AddValidateNotifyError method.
		ReactiveProperty add AddValidateAttribute extesion method.

2014-01-01 ver 0.4.0.1
	Change
		ReactiveProperty not implements IDataErrorInfo.
		UIDispatcherHelper's infrastructure changed Dispatcher to SynchronizationContext.
	Remove
		ReactiveProperty.ForceValidate method.
		ReactiveProperty.Error property.
		ReactiveProperty.SetValidateError method.
		ObservePropertyChanging method.

2013-12-15 ver 0.4.0.rc1 preview
	Change
		Portable Class Library(Supported platform .NET45, WP8, Windows store app(Windows 8.1)).
	Remove
		Codeplex.Reactive.Asynchronous.

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