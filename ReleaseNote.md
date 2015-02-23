# Release note

## v2.0

### Breaking change

- change namespace Codeplex.Reactive to Reactive.Bindings

### Obsolate

- ReactiveProperty#ObserveHasError obsolated. Please use ReactiveProperty#ObserveHasErrors.
- EventToReactive obsolated. Please use EventToReactiveProperty or EventToReactiveCommand.

### Add

- Add features for Xamarin.Android
    - SetBinding extension method added to View class.
    - SetCommand  extension method added to IObservable<T>.
