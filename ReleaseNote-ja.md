# リリースノート

## v2.1.0

### 追加

- ReadOnlyReactiveCollectionをReadOnlyObservableCollectionから作成できるようにしました
	- readOnlyObservableCollectionInstance.ToReadOnlyReactiveCollection(x => CreateViewModel(x))

### 変更

- ReadOnlyReactiveCollectionのDisposeメソッドを呼び出したときに、コレクション内部のインスタンスに対してもDisposeを呼ぶようにしました

## v2.0.1

### 変更

- BooleanNotifierクラスにINotifyPropertyChangedを実装しました。

## v2.0.0

### 破壊的変更

- 名前空間が Codeplex.Reactive から Reactive.Bindings に変わりました。
- ReactiveProperty#ObserveHasError メソッドをObserveHasErrorsに変更してください。

### 非推奨

- EventToReactiveを非推奨にしました。EventToReactivePropertyかEventToReactiveCommandを使ってください。

### 変更点

- ReadOnlyReactivePropertyが要素を削除するときにDisposeメソッドを呼ぶようになりました。
- CountNotifierクラスにINotifyPropertyChangedを実装しました。

### 追加

- Xamarin.Android用の機能を追加しました
    - ViewクラスにSetBinding拡張メソッドを追加しました。
    - IObservable<T>にSetCommand拡張メソッドを追加しました。
