# リリースノート

## v2.0

### 破壊的変更

- 名前空間が Codeplex.Reactive から Reactive.Bindings に変わりました。

### 非推奨

- ReactiveProperty#ObserveHasError メソッドを非推奨にしました。ReactiveProperty#ObserveHasErrorsを使ってください。
- EventToReactiveを非推奨にしました。EventToReactivePropertyかEventToReactiveCommandを使ってください。

### 変更点

- ReadOnlyReactivePropertyが要素を削除するときにDisposeメソッドを呼ぶようになりました。
- CountNotifierクラスにINotifyPropertyChangedを実装しました。

### 追加

- Xamarin.Android用の機能を追加しました
    - ViewクラスにSetBinding拡張メソッドを追加しました。
    - IObservable<T>にSetCommand拡張メソッドを追加しました。
