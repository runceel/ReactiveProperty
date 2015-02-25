# Release note

## v2.1.0

### Add

- Add ToReadONlyReactiveCollection extension method to ReadOnlyObservableCollection.
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
