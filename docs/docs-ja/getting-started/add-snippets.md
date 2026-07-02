# コード スニペットの追加

スニペット ファイルは [こちら](https://github.com/runceel/ReactiveProperty/tree/master/Snippet) で提供しています。
使用する場合は、スニペットを手動でインストールしてください。

次のドキュメントに、Visual Studio にスニペットを追加する手順が記載されています。

- [Visual Studio にコード スニペットを追加する](https://docs.microsoft.com/en-us/visualstudio/ide/walkthrough-creating-a-code-snippet?view=vs-2019#add-a-code-snippet-to-visual-studio)

## 提供されているスニペット

- `rprop`
  ```csharp
  public ReactiveProperty<T> PropertyName { get; }
  ```
- `rrprop`
  ```csharp
  public ReadOnlyReactiveProperty<T> PropertyName { get; }
  ```
- `rcom`
  ```csharp
  public ReactiveCommand CommandName { get; }
  ```
- `rcomg`
  ```csharp
  public ReactiveCommand<T> CommandName { get; }
  ```
- `arcom`
  ```csharp
  public AsyncReactiveCommand CommandName { get; }
  ```
- `arcomg`
  ```csharp
  public AsyncReactiveCommand<T> CommandName { get; }
  ```
- `rcoll`
  ```csharp
  public ReactiveCollection<T> CollectionName { get; }
  ```
- `rrcoll`
  ```csharp
  public ReadOnlyReactiveCollection<T> CollectionName { get; }
  ```
