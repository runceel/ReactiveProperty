/*--------------------------------------------------------------------------
 * ReactiveProperty
 * ver 0.1.0.0 (Oct. 6th, 2011)
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

|-NET40 - .NET 4.0 Client Profile
|-SL4 - Silverlight 4
|-WP7 - Windows Phone 7.1(no requires Rx-Main. This depend Microsoft.Phone.Reactive)
|-WP7.Rx-Main - Windows Phone 7.1

---Dependency---

Currently target Rx version is Build 1.0.10605(Stable, Rx-Main on NuGet) except WP7.
Codeplex.Reactive.Interactivity namespace and under classes,
depend System.Windows.Interactivity(Blend SDK)

---NuGet Installation---

Install-Package ReactiveProperty
-> NET40, SL, WP7.Rx-Main

Install-Package ReactiveProperty-WP7
-> WP7

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
-> ReactiveCollection - IObservable as ObservableCollection(and notify dispatch on IScheduler)
-> UIDispatcherScheduler - Schedule on UIDispatcher, if access same thread schedule immediately.

Codeplex.Reactive.Asynchronous
-> Extension Methods for asynchronous operation(WebClient, WebRequest, WebResponse, Stream)

Codeplex.Reactive.Extensions
-> Extension Methods for interface(IObservble<T>, INotifyPropertyChanged, etc...)

Codeplex.Reactive.Interactivity
-> EventToReactive - Trigger that converts UIEvent to ReactiveProperty.

Codeplex.Reactive.Notifier
-> ScheduledNotifier - Notify value on scheduler(use with asynchronous progress report).
-> SignalNotifier - Notify event of count signals(this is thraed safe).

Codeplex.Reactive.Serialization
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

---History---

2011-10-06 ver 0.1.0.0
    Initial Release