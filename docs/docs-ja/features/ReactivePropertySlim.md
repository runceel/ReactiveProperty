# ReactivePropertySlim

`ReactivePropertySlim` は `ReactiveProperty` の軽量版です。
`ReactivePropertySlim` は `ReactiveProperty` より 5 倍高速です。

`ReactivePropertySlim` は次の機能を提供します。

- `INotifyPropertyChanged` インターフェイスを実装しています。
- `IObservable<T>` インターフェイスを実装しています。
- `Value` プロパティを提供します。
- `ForceNotify` メソッドを提供します。

`ReactivePropertySlim` は高性能です。
次の表は、`ReactiveProperty` と `ReactivePropertySlim` のベンチマーク結果を示しています。
ReactivePropertySlim は、インスタンス作成で 16 倍、主要なユースケースで 36 倍高速です。

```
|                             メソッド |         平均 |     エラー |    標準偏差 |
|----------------------------------- |-------------:|----------:|----------:|
|     CreateReactivePropertyInstance |    87.146 ns | 0.8331 ns | 0.7385 ns |
| CreateReactivePropertySlimInstance |     5.460 ns | 0.0537 ns | 0.0502 ns |
|           BasicForReactiveProperty | 2,470.957 ns | 9.1934 ns | 8.1497 ns |
|       BasicForReactivePropertySlim |    68.773 ns | 1.3841 ns | 1.8478 ns |
```

このクラスは `ReactiveProperty` のように使用できます。

```csharp
var rp = new ReactivePropertySlim<string>("neuecc");
rp.Select(x => $"{x}-san").Subscribe(x => Console.WriteLine(x));
rp.Value = "xin9le";
rp.Value = "okazuki";
```

出力は次のとおりです。

```
neuecc-san
xin9le-san
okazuki-san
```

`ReactiveProperty` との違いの 1 つは、`ReactivePropertySlim` は `IObservable<T>` から作成できないことです。

```csharp
// 有効なコードではありません。
var rp = Observable.Interval(TimeSpan.FromSeconds(1)).ToReactivePropertySlim();
```

`IObservable<T>` から Slim クラスのインスタンスを作成したい場合は、`ToReadOnlyReactivePropertySlim` 拡張メソッドを使います。

```csharp
var rp = Observable.Interval(TimeSpan.FromSeconds(1)).ToReadOnlyReactivePropertySlim();
```

## UI スレッドへのディスパッチ

`ReactivePropertySlim` クラスは UI スレッドへ自動的にディスパッチしません。
必要な場合は、`ReactiveProperty` を使うか、明示的に UI スレッドへディスパッチしてください。

```csharp
var rp = Observable.Interval(TimeSpan.FromSeconds(1))
    .ObserveOnUIDispatcher() // UI スレッドへディスパッチします
    .ToReadOnlyReactivePropertySlim();
```

## 検証

`ValidatableReactiveProperty<T>` は、検証機能を備えた `IReactiveProperty<T>` の軽量実装です。高性能を維持しながら検証機能を提供するように設計されています。

#### 例

簡単な検証ロジックで `ValidatableReactiveProperty<T>` を使う例を次に示します。

```csharp
var validatableProperty = new ValidatableReactiveProperty<string>(
    initialValue: "",
    validate: value => string.IsNullOrEmpty(value) ? "Value cannot be empty" : null
);

validatableProperty.Value = "valid"; // 検証エラーなし
validatableProperty.Value = ""; // 検証エラー: "Value cannot be empty"
```

より複雑な検証シナリオでは、`ValidatableReactiveProperty<T>` を `DataAnnotations` と一緒に使うこともできます。

```csharp
public class Person
{
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }
}

public class PersonViewModel : IDisposable
{
    private Person _person = new Person();

    public ValidatableReactiveProperty<string> Name { get; }

    public PersonViewModel()
    {
        Name = _person.ToReactivePropertySlimAsSynchronized(x => x.Name)
                      .ToValidatableReactiveProperty(() => Name, disposeSource: true);
    }

    public void Dispose()
    {
        Name.Dispose();
    }
}
```

この例では、`Person` クラスの `Name` プロパティを `DataAnnotations` で検証しています。`PersonViewModel` クラスは `Name` プロパティを `ValidatableReactiveProperty<string>` インスタンスと同期し、検証ルールが適用されるようにします。

#### パフォーマンス

`ValidatableReactiveProperty<T>` は、従来の検証付き `ReactiveProperty<T>` に比べて大幅な性能向上を提供します。次のベンチマーク結果は、その性能上の利点を示しています。

|                                        メソッド |         平均 |       エラー |      標準偏差 |
|---------------------------------------------- |-------------:|------------:|------------:|
|                    ReactivePropertyValidation | 4,954.138 ns |  93.2171 ns | 107.3490 ns |
|         ValidatableReactivePropertyValidation |   704.852 ns |  12.8322 ns |  10.7155 ns |

`ValidatableReactiveProperty<T>` を使うことで、アプリケーション内の堅牢な検証ロジックを維持しながら高性能を実現できます。
