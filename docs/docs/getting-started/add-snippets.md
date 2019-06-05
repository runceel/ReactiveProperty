# Add code snippets

We provides snippet files [here](https://github.com/runceel/ReactiveProperty/tree/master/Snippet).
If you want to use it, then please install the snippets manually.

Following document is steps to add a snippet to Visual Studio.

- [Add a code snippet to Visual Studio](https://docs.microsoft.com/en-us/visualstudio/ide/walkthrough-creating-a-code-snippet?view=vs-2019#add-a-code-snippet-to-visual-studio)

## Provide snippets

- `rprop`
  ```cs
  public ReactiveProperty<T> PropertyName { get; }
  ```
- `rrprop`
  ```cs
  public ReadOnlyReactiveProperty<T> PropertyName { get; }
  ```
- `rcom`
  ```cs
  public ReactiveCommand CommandName { get; }
  ```
- `rcomg`
  ```cs
  public ReactiveCommand<T> CommandName { get; }
  ```
- `arcom`
  ```cs
  public AsyncReactiveCommand CommandName { get; }
  ```
- `arcomg`
  ```cs
  public AsyncReactiveCommand<T> CommandName { get; }
  ```
- `rcoll`
  ```cs
  public ReactiveCollection<T> CollectionName { get; }
  ```
- `rrcoll`
  ```cs
  public ReadOnlyReactiveCollection<T> CollectionName { get; }
  ```
