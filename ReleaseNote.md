# Release note

## v2.0

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
