# MainForm タブ機能 設計方針

この資料は、`MainForm` のトレース検索タブを複数タブ対応するための設計方針です。
Git 経由で作業メンバーに共有し、実装時の認識をそろえることを目的とします。

## 前提

Windows Forms の UI コントロールは、各タブで別インスタンスにする必要があります。

`TextBox`、`Button`、`DataGridView` などの `Control` は、同時に複数の `TabPage` に配置できません。
1 つのコントロールが持てる親コンテナは 1 つだけです。
そのため、同じ UI コントロールのインスタンスを全タブで共有する設計にはできません。

## 基本方針

UI コントロール自体はタブごとに別インスタンスにします。
一方で、検索、表示、クリア、CSV 出力などの処理ロジックは共通化します。

共通化する対象は、UI コントロールのインスタンスではなく、各タブのコントロールを束ねて扱う仕組みです。

方針は以下です。

- 各タブに検索条件入力欄、ボタン、表示用グリッドを個別に持たせる
- タブごとの UI 部品を `TraceTabContext` のようなクラスにまとめる
- 検索、表示、クリア、出力処理は `TraceTabContext` を引数にして共通化する
- タブ番号ごとの検索結果や描画情報は既存の辞書構造を活かす

## TraceTabContext の例

実装時は、以下のようなタブ単位のコンテキストクラスを用意します。

```csharp
private sealed class TraceTabContext
{
    public int TabNo { get; set; }
    public TabPage TabPage { get; set; }

    public TextBox ProductionOrderNumberTextBox { get; set; }
    public TextBox ItemNameTextBox { get; set; }
    public TextBox ItemCodeTextBox { get; set; }
    public TextBox LotNumberTextBox { get; set; }

    public CheckBox UseFromCheckBox { get; set; }
    public DateTimePicker FromDatePicker { get; set; }

    public RadioButton ForwardRadioButton { get; set; }
    public RadioButton BackwardRadioButton { get; set; }

    public Button SearchButton { get; set; }
    public Button ClearButton { get; set; }
    public Button CsvOutputButton { get; set; }

    public Panel StartHeaderPanel { get; set; }
    public Panel MiddleHeaderPanel { get; set; }
    public Panel EndHeaderPanel { get; set; }

    public DataGridView StartGrid { get; set; }
    public DataGridView MiddleGrid { get; set; }
    public DataGridView EndGrid { get; set; }
}
```

`MainForm` 側では、タブ番号ごとに保持します。

```csharp
private readonly Dictionary<int, TraceTabContext> _traceTabs =
    new Dictionary<int, TraceTabContext>();
```

## 共通化する処理の形

現在の処理は、1 タブ目のコントロール名を直接参照している箇所があります。
複数タブ対応では、直接参照をやめて `TraceTabContext` 経由にします。

例:

```csharp
private TraceSearchParameters CollectSearchParametersFromControls(
    TraceTabContext tab)
```

```csharp
private void DoTrace(
    TraceTabContext tab,
    TraceSearchParameters parameters)
```

```csharp
private void ClearTraceTab(
    TraceTabContext tab)
```

```csharp
private void ExportCsv(
    TraceTabContext tab)
```

この形にすると、各タブの UI は別々でも、処理ロジックは 1 本化できます。

## 現状コードで見直すポイント

`MainForm` の現状では、以下の箇所が 1 タブ目固定になっています。
複数タブ対応時に見直しが必要です。

- `CollectSearchParametersFromControls` が 1 タブ目の `TextBox`、`CheckBox`、`RadioButton` を直接参照している
- `DoTrace` が `dataGridStart`、`dataGridMiddle`、`dataGridEnd` に直接表示している
- 検索結果保存時の `tabNo` が `1` 固定になっている
- 現在タブ取得処理が `Tag = 1` 固定になっている
- 2 タブ目以降の検索、クリア、CSV 出力ボタンに共通イベントが割り当たっていない

一方で、以下の既存構造は活かせます。

- `_tabTraceResults`
- `_tabDisplayResults`
- `_tabDrawContexts`
- タブ番号ごとの検索結果管理
- タブ番号ごとの描画情報管理

## 実装時の注意

2 タブ目以降のコントロール名は、Designer 上では `textBox1`、`button3`、`dataGridView1` のような連番名になっています。
このまま処理内で直接参照すると保守しづらくなります。

実装時は、初期化時に各タブのコントロールを `TraceTabContext` に登録し、その後の処理ではコンテキスト経由で参照する方針にします。

より大きく整理できる場合は、タブ内の UI 一式を `UserControl` 化し、各 `TabPage` に同じ `UserControl` を 1 つずつ配置する案もあります。
ただし、その場合でも `UserControl` のインスタンスはタブごとに別々に作成します。

## まとめ

`MainForm` のタブ機能は、以下の考え方で進めます。

```text
UI コントロールはタブごとに別インスタンス。
処理ロジックは TraceTabContext 経由で共通化。
タブごとの結果と描画情報はタブ番号で管理。
```

この方針により、各タブで独立して検索、表示、出力を行いつつ、処理の重複を抑えられます。
