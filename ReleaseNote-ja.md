# リリースノート

## v2.2

### 破壊的変更

- ObserveElementReactiveProperty 拡張メソッドを削除しました。

### Add

- ObserveElementObservableProperty 拡張メソッドを追加しました。

## v2.1.8

### 追加

- Helpers名前空間にリアルタイムにコレクションの要素をフィルタリングするコレクション FilteredReadOnlyObservableCollection<T> を追加しました。

## v2.1.7

### バグフィックス

- ReadOnlyReactiveProperty が BehaviorSubject<T> をソースに作った時に BehaviorSubject<T> から最初に発行される値を初期値として使っていなかった不具合を修正しました。

## v2.1.6

### 追加

- ObservableCollection と ReadOnlyObservableCollection の要素の ReactiveProperty の変更を監視する ObserveElementReactiveProperty 拡張メソッドを実装しました。
- ObservableCollection と ReadOnlyObservableCollection の要素の PropertyChanged イベントを監視する ObserveElementPropertyChanged 拡張メソッドを実装しました。
- 読み取り専用の ReactiveProperty である ReadOnlyReactiveProperty<T> を実装しました。IObservable<T> から ToReadOnlyReactiveProperty 拡張メソッドで生成できます。

### 破壊的変更

- INotifyCollectionChanged の拡張メソッドとして提供されていた ObserveElementProperty メソッドのアクセシビリティを internal にしました。

## v2.1.5

### 追加

- IEnumerableからReadOnlyReactiveCollectionを作成する拡張メソッドを追加しました。

## v2.1.4

- 欠番

## v2.1.3

### 破壊的変更

- ObserveElementProperty で値に変更があったインスタンスも通知できるようにしました。

## v2.1.2

### 追加

- ObservableCollection と ReadOnlyObservableCollection の要素のプロパティ変更を監視する ObserveElementProperty 拡張メソッドを実装しました。
- INotifyCollectionChanged に対して ObserveXxxChanged 拡張メソッドを追加しました。

### 変更

- ToReadOnlyReactiveCollection 拡張メソッドから参照型制約を除去しました。

## v2.1.1

### 変更

- SynchronizationContext.Current が null の場合に UIDispatcherScheduler を初期化すると発生する例外を変更しました。 

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
