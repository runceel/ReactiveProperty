# ReactiveProperty.R3 で R3 に移行する

ReactiveProperty では新規アプリケーションに [R3](https://github.com/Cysharp/R3) を推奨するようになりましたが、既存アプリの多くは R3 が標準では提供しない機能にまだ依存しています。`ReactiveProperty.R3` は、そのようなケースのための永続的なブリッジ パッケージです。使い慣れた ReactiveProperty の体験を保ちながら、R3 と相性のよい Observable を公開し、`System.Reactive` のスケジューラーではなく `TimeProvider` / `SynchronizationContext` を使用します。

## 使用する場面

プロジェクトを `Reactive.Bindings` から R3 へ移行していて、R3 が意図的に含めていない上位レベルの MVVM ヘルパーがまだ必要な場合は、`ReactiveProperty.R3` を使用します。

このブリッジは、移行中によく現れる次の不足を補います:

- `BooleanNotifier`、`BusyNotifier`、`CountNotifier`、`ScheduledNotifier<T>` などの notifier とメッセージ ブローカー
- `ReactiveTimer`
- `AsyncReactiveCommand`
- `ReadOnlyReactiveCollection<T>` とコレクション ヘルパー
- `ValidatableReactiveProperty<T>` と検証ヘルパー
- `ToReactivePropertyAsSynchronized`

## パッケージをインストールする

> **ブリッジの提供状況:** `R3` と `ObservableCollections.R3` はすでに NuGet で利用できます。
> `ReactiveProperty.R3` と `ReactiveProperty.R3.WPF` のブリッジ パッケージは **まだ公開されていません**。
> 公開されるまでは、このリポジトリの `Source/ReactiveProperty.R3` と
> `Source/ReactiveProperty.R3.WPF` プロジェクトに `ProjectReference` を追加して、ソースからブリッジを参照してください。
> 以下の `dotnet add package` コマンドは、ブリッジのリリース後に想定している利用手順を示しています。

.NET CLI または Visual Studio NuGet Package Manager を使用して、プロジェクトにパッケージを追加します。

.NET CLI を使用している場合は、次を実行します:

```powershell
dotnet add package R3
dotnet add package ReactiveProperty.R3
```

Visual Studio を使用している場合は、プロジェクトを右クリックし、Manage NuGet Packages を選択して `R3` と `ReactiveProperty.R3` を検索し、そこからインストールします。

プロジェクトで WPF のトリガー アクションまたはコンバーターを使用している場合は、WPF ブリッジ パッケージも追加します:

```powershell
dotnet add package ReactiveProperty.R3.WPF
```

Visual Studio ユーザーは、同じ方法で NuGet Package Manager から `ReactiveProperty.R3.WPF` をインストールできます。

## 移行の進め方

1. 古い ReactiveProperty パッケージを `R3` と `ReactiveProperty.R3` に置き換えます。
2. R3 がすでに直接提供している部分は、ネイティブの R3 API に書き換えます。
3. ブリッジ型は、上記の本当の不足部分にだけ残します。
4. Observable を合成するファイルでは `using R3;` を追加します。ブリッジ型は R3 Observable を返すためです。

### 名前空間の変更

このブリッジは専用の名前空間を使用するため、段階的な移行中に元の ReactiveProperty パッケージと共存できます:

- `Reactive.Bindings` -> 直接変換では `R3`
- `Reactive.Bindings.Extensions` -> `R3` と `Reactive.Bindings.R3.Extensions`
- `Reactive.Bindings.Notifiers` -> `Reactive.Bindings.R3.Notifiers`

次の表は、もっとも一般的な移行を示しています:

| ReactiveProperty API | R3 / ブリッジでの置き換え |
|---|---|
| `ReactivePropertySlim<T>` | `R3.ReactiveProperty<T>` |
| `AsyncReactiveCommand` | `Reactive.Bindings.R3.AsyncReactiveCommand` |
| `BusyNotifier` | `Reactive.Bindings.R3.Notifiers.BusyNotifier` |
| `ReadOnlyReactiveCollection<T>` | `Reactive.Bindings.R3.ReadOnlyReactiveCollection<T>` |
| `ValidatableReactiveProperty<T>` | `Reactive.Bindings.R3.ValidatableReactiveProperty<T>` |
| `ToReactivePropertyAsSynchronized` | `Reactive.Bindings.R3.Extensions.ToReactivePropertyAsSynchronized` |

典型的な移行後のコードは次のようになります:

```csharp
using R3;
using Reactive.Bindings.R3.Notifiers;

public sealed class ViewModel
{
    public ReactiveProperty<int> Count { get; } = new(0);
    public BusyNotifier IsBusy { get; } = new();
}
```

## 実行可能なサンプル

このリポジトリには、完全な移行を文脈付きで並べて確認できる WPF アプリの **移行前/移行後ペア** が含まれています:

- **移行前** — `Samples/ReactivePropertySamples.WPF`（`Samples/ReactivePropertySamples.Shared` を含む）:
  `Reactive.Bindings`（ReactiveProperty）を基にした元のアプリです。
- **移行後** — `Samples/ReactivePropertySamples.R3.WPF`（`Samples/ReactivePropertySamples.R3.Shared` を含む）:
  同じアプリを R3 + `ReactiveProperty.R3` に移行したものです。

ViewModel は `.Shared` プロジェクトにあるため、2 つの `.Shared` フォルダーを比較すると、どのシンボルがどのように変わったかを正確に確認できます。移行後の ViewModel は `Samples/ReactivePropertySamples.R3.Tests` でもカバーされており、以下で説明する動作を固定しています。

## GitHub Copilot CLI で移行する

このリポジトリには、`skills/migrating-reactiveproperty-to-r3/` に移行用の **skill** が含まれています。これは上記の手順を、繰り返し実行できるエージェント駆動のフローにします。パッケージ参照を変更し、マッピング テーブルに基づいてすべての ReactiveProperty シンボルを書き換え、推測ではなく人の判断が必要な少数のケースを報告します。以下のフローは、ViewModel、DataAnnotations 検証、notifier、`ReadOnlyReactiveCollection<T>`、XAML の `EventToReactiveCommand` を使用するサンプル WPF アプリでエンドツーエンドに検証済みです。

### 1. skill をインストールする

**推奨 — CLI が自動的に読み込み、エージェントが skill のマッピング テーブルをスムーズに読めるように、あなたのプロジェクトにインストールしてください。** skill フォルダーをプロジェクトの `.agents/skills/` ディレクトリにコピーします（これはリポジトリの `skills/README.md` が最初に推奨している方法です）:

```powershell
# このリポジトリのクローンから、あなたのアプリのリポジトリ ルートで実行します
Copy-Item -Recurse <path-to-clone>/skills/migrating-reactiveproperty-to-r3 `
  ".agents/skills/migrating-reactiveproperty-to-r3"
```

skill がプロジェクト ツリーの **内側** にあるため、エージェントは同梱の `references/rules.json` を構造化ファイル リーダーで直接読み取れます — 読み取りスコープの回避策は不要です。

**代替 — 個人用 Copilot skills ディレクトリに一度だけインストールし、すべてのプロジェクトで使用します**:

```powershell
Copy-Item -Recurse skills/migrating-reactiveproperty-to-r3 `
  "$HOME/.copilot/skills/migrating-reactiveproperty-to-r3"
```

macOS/Linux での個人用のコピー先は `~/.copilot/skills/migrating-reactiveproperty-to-r3` です。複数のプロジェクトを移行するときに便利ですが、その場合 skill フォルダーはプロジェクトの *外側* に置かれるため、このガイドの最後にある読み取りスコープに関する注意に該当します。どちらの方法でも、コピー後にプロジェクト内で `copilot` を起動します。skill は自動的に読み込まれ、R3 への移行を依頼したときに有効になります。（ローカル プラグインや marketplace としてインストールする方法も使えますが、フォルダーをコピーするのがもっとも簡単です。）

### 2. コードを変更する前に計画を依頼する

プロジェクトで CLI を開き、まず棚卸しと計画を依頼します。スコープを絞った現実的なプロンプトが最適です — エージェントに「すべてを移行して」と一度に依頼しないでください:

```text
この WPF アプリを ReactiveProperty から R3 に移行したいです。コードを変更する前に、
プロジェクト内のすべての ReactiveProperty シンボルを棚卸しし、それぞれについて、
一致するルール、対象 (r3-direct / reactiveproperty-r3 / manualReview)、R3 での置き換えを
教えてください。manual-review 項目は別に一覧化してください。
```

エージェントはマッピング テーブルを読み取り、シンボルごとの計画を作成します。そこには、直接の R3 置き換え（`ReactivePropertySlim<T>` → `R3.ReactiveProperty<T>`、`ReactiveProperty<T>` → `R3.BindableReactiveProperty<T>`、`ObserveProperty` → `ObservePropertyChanged`、…）、本当の不足部分に対するブリッジ置き換え（`AsyncReactiveCommand`、`BusyNotifier`、`ReadOnlyReactiveCollection<T>`、`ValidatableReactiveProperty<T>`、`ToReactivePropertyAsSynchronized`）、短い manual-review リストが含まれます。

### 3. 段階的に移行する

小さくレビューしやすい単位で移行し、同じセッションを `--continue` で継続します:

```text
CounterViewModel と PeopleViewModel を今 R3 に移行してください。NuGet の R3 と、
不足する型には ReactiveProperty.R3 ブリッジを使用してください。
```

```text
次に残りの ViewModel を移行し、パッケージ参照と using ディレクティブを修正し、
XAML の EventToReactiveCommand xmlns を R3.WPF ブリッジに差し替えてください。
```

XAML トリガー アクションの場合、書き換えはほぼ xmlns の差し替えです:

```xml
<!-- 変更前 -->
xmlns:i="clr-namespace:Reactive.Bindings.Interactivity;assembly=ReactiveProperty.WPF"
<!-- 変更後 -->
xmlns:i="clr-namespace:Reactive.Bindings.R3.Interactivity;assembly=ReactiveProperty.R3.WPF"
```

### 4. ビルド、テスト、修正する

エージェントに既存のテストのビルドと実行を依頼し、失敗があれば具体的なプロンプトで対応します:

```text
ソリューションをビルドしてテストを実行し、ビルド状態、テスト状態、manual-review
リストを報告してください。
```

```text
ビルドが次のエラーで失敗します: <エラーを貼り付ける>。修正してください。
```

ブリッジの不足部分を補う型は **R3** Observable を公開するため、購読や演算子チェーン（`Subscribe`、`Where`、`Select`、…）を使うファイルには `using R3;` が存在することを確認してください。

> **検証タイミング:** 単一プロパティの DataAnnotations は R3 の
> `BindableReactiveProperty<T>.EnableValidation()` に対応しますが、R3 の検証は **遅延実行** です — バインディングまたは購読者がアタッチされるまで初期値をエラーとして扱いません。ヘッドレス テスト（またはロジック）が元の即時検証を期待している場合は、代わりに `ReactiveProperty.R3` の `ValidatableReactiveProperty<T>` を使用してください。これは即時に検証します。

### 5. manual-review 項目を解決する

skill は機械的に書き換えられないケースを推測しません。ファイル、行、メモ付きでフラグを立てます。通常は次のような短いリストになります:

- **`ReactivePropertyMode` 引数**（たとえば `ToReactivePropertyAsSynchronized` や `ReactiveProperty<T>` コンストラクター上のもの）。R3 に対応するものはないため、エージェントはそれを削除し、意図を再現する方法を説明します。`DistinctUntilChanged` は R3 の既定の重複除外動作に対応し、`RaiseLatestValueOnSubscribe` はプロパティが現在値を再生するかどうかに対応します。実際に依存していた動作を確認してください。
- **カスタム `IScheduler` 引数。** R3 は時間に `TimeProvider`、ディスパッチに `SynchronizationContext` を使用するため、既定以外のスケジューラーは自動変換できません。エージェントはそれを報告し、あなたがその呼び出し箇所に適した provider を選択します。
- **`System.Reactive` に残す必要がある `System.IObservable<T>` 境界**（公開 API、サードパーティの Rx など）。宣言を書き換えるのではなく、R3 の `ToObservable()` / `AsSystemObservable()` でブリッジしてください。
- **即時検証のままにする必要がある DataAnnotations 検証。** skill は単一プロパティの DataAnnotations を `EnableValidation()` に書き換えますが、これは遅延検証です — 上記の **検証タイミング** の注を参照してください。即時検証に依存していた場合（たとえばヘッドレス テストなど）は、そのプロパティを `ValidatableReactiveProperty<T>` に切り替えてください。

このフロー全体は、「このアプリを R3 に移行して」「ReactiveProperty を R3 に置き換えて」「この ViewModel を ReactiveProperty から移行して」のような簡単な依頼で開始できます。

> **注 (個人用 `~/.copilot/skills/` インストールを使用した場合のみ):** skill は、同梱のマッピング テーブル `migrating-reactiveproperty-to-r3/references/rules.json` によって動作します。CLI の構造化ファイル リーダーはプロジェクト ディレクトリにスコープされるため、skill がプロジェクトの外側にある `~/.copilot/skills/` 配下にある場合、エージェントがそのファイルを「読めない」と報告することがあります — コンテンツ/ポリシー ブロックだと誤って表示することさえあります。これは読み取りスコープ上の癖であり、実際のブロックではありません。エージェントは引き続き（たとえばシェル コマンドで）読み取ることができ、そうでない場合も skill 自体に含まれるガイダンスにフォールバックします。**step 1 で推奨しているように、skill をプロジェクトの `.agents/skills/` にインストールすれば、この問題は完全に避けられます**。マッピング テーブルが作業ツリーの内側に置かれるためです。

## 大規模な移行に関するメモ

このブリッジは一時的な shim ではなく、長期的な移行パスとして意図されています。既存の MVVM パターンやすでに依存している動作を保ちながら、段階的に R3 へ移行したいプロジェクトに適しています。
