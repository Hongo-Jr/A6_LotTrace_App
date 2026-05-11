# MainForm タブ別グリッド表示 整合修正メモ

この資料は、`MainForm` の複数タブ対応を取り込んだ後に行った、グリッド表示まわりの整合修正内容をまとめたものです。

今回の修正は、交点検出機能そのものの実装ではありません。
主な目的は、タブ別グリッド表示の実装に対して、旧来の 1 タブ目固定処理が残っていた箇所を整理することです。

## 背景

`MainForm` には、タブごとの UI 部品を `TraceTabContext` で束ね、`ActivateTabDisplay(tabNo)` で対象タブのグリッドへ表示する流れが追加されていました。

一方で、既存処理には以下のような 1 タブ目固定の参照が残っていました。

- `dataGridStart`
- `dataGridMiddle`
- `dataGridEnd`

そのため、タブ 2 以降で検索しても、表示処理、イベント処理、描画キャッシュ、スクロール同期がタブ 1 側のグリッドを参照する可能性がありました。

## 修正概要

### 1. 全タブのグリッドへイベント登録

変更前は、以下のイベントがタブ 1 のグリッドにだけ登録されていました。

- `Paint`
- `CellFormatting`
- `Scroll`
- `CellMouseEnter`
- `CellMouseLeave`
- `MouseLeave`

変更後は、`GetTabContext(1..10)` で各タブの `GridStart`、`GridMiddle`、`GridEnd` を取得し、全タブのグリッドへ登録します。

理由:

タブ 2 以降のグリッドで `CellFormatting` や `Paint` が動かないと、文字色、罫線、ツールチップ、スクロール同期が反映されないためです。

### 2. `DoTrace` の旧固定グリッド再バインド処理を削除

変更前は、`DoTrace` 内で以下の流れになっていました。

1. `StoreDisplayArtifactsForTab(tabNo, result, displayResult)` でタブ別結果を保持
2. `_tabDisplayTables[tabNo] = displayTable` で表示テーブルを保持
3. `ActivateTabDisplay(tabNo)` で対象タブへ表示
4. その後、`dataGridStart`、`dataGridMiddle`、`dataGridEnd` へ再度 `DataSource` を設定

変更後は、表示処理を `ActivateTabDisplay(tabNo)` に寄せ、後段の固定グリッド再バインドを削除しました。

理由:

タブ 2 以降で検索しても、最後にタブ 1 用グリッドへ再バインドしてしまうためです。
タブ別表示の責務は `ActivateTabDisplay(tabNo)` に集約します。

### 3. 文字色キャッシュを現在タブ対象に変更

変更前は、文字色キャッシュ作成が常に以下の固定グリッド対象でした。

- `dataGridStart`
- `dataGridMiddle`
- `dataGridEnd`

変更後は、`GetCurrentTabContext()` から現在タブの `GridStart`、`GridMiddle`、`GridEnd` を取得してキャッシュを作成します。

理由:

`CellFormatting` は実際に表示されているグリッドで発火します。
キャッシュがタブ 1 固定だと、タブ 2 以降のグリッドから参照できません。

### 4. `CellFormatting` のキャッシュ未作成時ガード追加

変更前は、以下のようにキャッシュを直接参照していました。

```csharp
GridForeColorCache cache = _gridPaintCache.ForeColorCaches[grid];
```

変更後は、`TryGetValue` で確認し、キャッシュがない場合は処理を抜けます。

理由:

DataSource 切替直後、未検索タブ、または初期表示中に `CellFormatting` が先に発火すると、キャッシュ未作成で例外になる可能性があるためです。

### 5. 罫線描画キャッシュを現在タブ対象に変更

変更前は、罫線描画キャッシュもタブ 1 固定のグリッドに対して作成していました。

変更後は、現在タブの `GridStart`、`GridMiddle`、`GridEnd` に対して作成します。

理由:

タブ 2 以降の `Paint` が発火しても、そのグリッド用の罫線キャッシュが存在しなければ描画できないためです。

### 6. スクロール同期を発生元タブ内に限定

変更前は、どのグリッドでスクロールしても、同期先が以下の固定グリッドでした。

- `dataGridStart`
- `dataGridMiddle`
- `dataGridEnd`

変更後は、イベント発生元の `DataGridView` から所属する `TraceTabContext` を逆引きし、そのタブ内の 3 グリッドだけを同期します。

理由:

タブ 2 以降のスクロール操作で、タブ 1 のグリッドを同期対象にしてしまうためです。

### 7. 交点 CSV 出力の旧プロパティ参照を更新

`CrossPointRecord` の仕様変更により、旧プロパティ参照を更新しました。

変更前:

- `ItemCode`
- `TabNumbers`

変更後:

- `CrossPointFlag`
- `ProductionOrderNumber`
- `LotNumber`
- `ItemName`
- `StartDateText`
- `Weight`
- 対象タブごとの `TabN`

理由:

交点検出結果のモデルが、`MaterialPair` / `TabNumbers` 方式から、`NodeIdentityKey` / `TabPresence` 方式に変わったためです。

## 今回の修正で意図していないこと

以下は今回の主目的ではありません。

- 交点グリッドの画面表示完成
- 通常グリッド上の交点セル背景色変更
- タブ機能全体の完成
- レイアウト処理全体の再設計
- `UserControl` 化などの構造変更

今回の修正は、既存の `TraceTabContext` と `ActivateTabDisplay(tabNo)` の方針に合わせ、旧 1 タブ目固定処理を整合させる範囲に留めています。

## 注意点

`MainForm` には、まだ 1 タブ目固定の参照が残っている可能性があります。

特に以下の領域は、今後のタブ対応で継続確認が必要です。

- ヘッダー表示更新
- グリッド幅調整
- CSV 出力
- Excel 出力
- 交点グリッド表示
- 交点セル背景色反映

作業時は、`dataGridStart`、`dataGridMiddle`、`dataGridEnd` を直接参照している箇所が、タブ 1 専用処理なのか、全タブ対応すべき処理なのかを確認してください。

## ビルド確認

以下のコマンドでビルド成功を確認しました。

```powershell
dotnet build LotTraceApp.sln
```

結果:

- エラー: 0
- 警告: 1

警告は既存の未使用 Designer フィールドです。
