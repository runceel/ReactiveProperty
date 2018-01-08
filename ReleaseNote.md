# Release note

New release notes see below.

[Releases](https://github.com/runceel/ReactiveProperty/releases)

## v4.0.0-pre4
### Add
- Add IsEnabled property to ReactiveTimer class.

## v4.0.0-pre1
### Update
- Update System.Reactive v4.0.0-preview00001

### Change
- Change ReaciveProperty constructor accessibility from internal to public
- Change ReadOnlyReaciveProperty constructor accessibility from internal to public

### Breaking change
- Remove SerializeHelper
- Change .NET Standard version from 1.1 to 1.3
    - Disable .NET Framework 4.5
	- Disable Windows store app

## v3.6.0
### Breaking change
- Rename iOS SetBinding method to SetBindingXXXXX(target class name).

### Add
- Add non argument version AsyncReactiveCommand#Subscribe method.
- Add non argument version ReactiveCommand#Subscribe method.

## v3.5.0
### Breaking change
- Change ObserveEveryValueChanged method namespace Extensions to ObjectExtensions.

### Add
- Add Refresh method to IFilteredReadOnlyObservableCollection.

## v3.4.0
### Update
- Update to Rx 3.1.1

### Add
- Add ObserveDependencyProperty extension method to DependencyObject(UWP and WPF).
- Add ToReadOnlyReactiveProperty extension method to DependencyObject(UWP and WPF)
- Add ToReactiveProperty extension method to DependencyObject(UWP and WPF)

## v3.3.0

- Update to Rx 3.1.0
- Update to UWP Behavior v2.0.0

## v3.2.0

### Add

- More `SetBinding` extension method for iOS.

## v3.1.0
- Support VB PropertySelector expression.

## v3.0.0

### Update
- Update Reactive Extension 3.0.

## v2.9

### Add
- Add AsyncReactiveCommand class.

## v2.8

### Add
- Add disposeElement argument at ToReadOnlyReactiveCollection. Default value is true.

### Change
- Default scheduler create logic.(You can use ReactiveProperty in ConsoleApplication.)

## v2.7.2

### Add
- Add ObserveEveryValueChanged extension method.(WPF only)

## v2.7.1
### Add
- Add IO<bool>#Inverse extension method.

## v2.7
### Add
- Add BusyNotifier.

## v2.6
### Change
- Change default scheduler to ReactivePropertyScheduler from UIDispatcherScheduler.
- Change reference BehaviorSDK to Microsoft.Xaml.Behaviors.Uwp.Managed in UWP.

## v2.5

### Add
- Add ForceNotify/ForceValidate method.

## v2.4.2

### BugFix

- Fixed bug, dosen't work ignoreValidationErrorValue at ToReactivePropertyAsSynchronized.

## v2.4.1

### BugFix

- Fixed bug, doesn't call collection element Dispose method when ReadOnlyReactiveCollection#Dispose called.

## v2.4.0

### Add

- Add extension method to UIView for binding ReactiveProperty/ReactiveCommand.

## v2.3.1

### changes

- Change IReadOnlyReactiveProperty generic parameter to out.

## v2.3

### Performance

- Performance improvement ReactiveProperty's constructor.

### Add

- Add Xamarin.Mac profile.
- Add .NET Framework 4.6 binary.

## v2.2.8

### Changes

- Changed implements interface to ReactiveProeprty and ReadOnlyReactiveProperty. Issue #11.

## v2.2.7

### Changes

- Changed internal implementation in IFilteredReadOnlyObservableCollection.

## v2.2.6

### Add

- Add AddRangeOnScheduler method to ReactiveCollection class.

## v2.2.5

### Update

- Update System.Windows.Interactivity assembly reference.

## v2.2.4

### BugFix

- Fixed bug, FilteredReadOnlyObservableCollection index management were broken when collection item removed.
- Fixed bug, FilteredReadOnlyObservableCollection index management were broken when collection item replaced.

## v2.2.3

### BugFix

- Fixed bug, converter called when ReadOnlyReactiveCollection remove method called.

## v2.2.2

### BugFix

- Fixed bug, have Problem in initialization at FilteredReadOnlyObservableCollection index.

## v2.2.1

### Bug fix

- Fixed bug, call several times Subscribe method in the constructor of ReadOnlyReactiveProperty.

## v2.2

### Breaking changes

- Remove ObserveElementReactiveProperty extension method.

### Add

- Add ObserveElementObservableProperty extension method.

## v2.1.8

### Add

- Added FilteredReadOnlyObservableCollection<T> class at Helpers namespace. It is real time filtered collection.

## v2.1.7

### Bug fix

- Fixed bug. ReadOnlyReactiveProperty<T> did not use the value to be issued as the initial value from BehaviorSubject<T> when made BehaviorSubject<T> to the source.

## v2.1.6

- Implemented ObserveElementReactiveProperty extension method, to observe ReactiveProeprty which ObservableCollection and ReadOnlyObservableCollection elements have changing.
- Implemented ObserveElementPropertyChanged extension method, to observe PropertyChanged event which ObservableCollection and ReadOnlyObservableCollection elements have.
- Implemented ReadOnlyReactiveProperty<T> class. ToReadOnlyReactiveProperty extension method creates its instance from IObservable<T>.  

### Breaking change

- Changed accesibility of ObserveElementProperty method to 'internal' from 'public'. 

## v2.1.5

### Add

- Add create ReadOnlyReactiveCollection method from IEnumerable.

## v2.1.4

- Not released.

## v2.1.3

### Breaking change

- ObserveElementProperty returns instance which has value changed property.

## v2.1.2

### Add

- Implemented ObserveElementProperty extension method, to observe property changes of the ObservableCollection elements and ReadOnlyObservableCollection elements.
- Added ObserveXxxChanged extension methods for INotifyCollectionChanged.

### Change

- Remove class constranit from ToReadOnlyObservableCollection extension method.

## v2.1.1

### Change

- Changed exception which is occured when UIDispatcherScheduler is initialized under SynchronizationContext.Current is null.

## v2.1.0

### Add

- Add ToReadOnlyReactiveCollection extension method to ReadOnlyObservableCollection.
	- readOnlyObservableCollectionInstance.ToReadOnlyReactiveCollection(x => CreateViewModel(x))

### Change

- Dispose method is called in collection when ReadOnlyReactiveCollection called dispose method.

## v2.0.1

### Change

- Implements INotifyPropertyChanged to BooleanNotifier.

## v2.0.0

### Breaking change

- Change namespace Codeplex.Reactive to Reactive.Bindings
- Rename ReactiveProperty#ObserveHasError to ReactiveProperty#ObserveHasErrors.

### Obsolate

- EventToReactive obsolated. Please use EventToReactiveProperty or EventToReactiveCommand.

### Change

- ReadOnlyReactiveProperty call Dispose method when item removed.
- Implements INotifyPropertyChanged to CountNotifier..

### Add

- Add features for Xamarin.Android
    - SetBinding extension method added to View class.
    - SetCommand  extension method added to IObservable<T>.
