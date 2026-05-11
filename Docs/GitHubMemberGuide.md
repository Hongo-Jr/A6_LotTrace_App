# GitHub 共有・作業ガイド

この資料は、LotTraceApp を GitHub から取得して Visual Studio で作業するための最小手順です。
慣れるまでは、必ず作業前にこの順番で確認してください。

## 目的

- GitHub から最新版を取得する
- 自分の作業ブランチで修正する
- 変更をコミットして GitHub に送る
- Pull Request でレビューしてから `main` に反映する

## 最初に必要なもの

- Visual Studio 2022 以降
- Git for Windows
- GitHub アカウント
- このリポジトリへのアクセス権限

Visual Studio に GitHub アカウントでログインしていても、Git のコミット用にユーザー名とメールアドレスが必要です。
Visual Studio の Git 設定で以下を登録してください。

- ユーザー名: GitHub のユーザー名、または氏名
- メールアドレス: GitHub のメールアドレス、または GitHub の noreply メール

## 初回取得

1. GitHub のリポジトリ画面を開く
2. `Code` ボタンから HTTPS の URL をコピーする
3. Visual Studio を開く
4. `Git リポジトリのクローン` を選ぶ
5. コピーした URL を貼り付ける
6. 保存先フォルダを選ぶ
7. クローンする
8. `LotTraceApp.sln` を開く
9. `ビルド > ソリューションのビルド` を実行する

クローンとは、GitHub 上のリポジトリを自分の PC に丸ごとコピーする操作です。

## 初回ビルド前の確認

このプロジェクトは .NET Framework 4.8 の Windows Forms アプリです。
clone 直後にビルドできない場合は、先に以下を確認してください。

1. Visual Studio Installer で `.NET デスクトップ開発` ワークロードが入っていること
2. .NET Framework 4.8 Developer Pack が入っていること
3. Visual Studio の `ツール > NuGet パッケージ マネージャー > パッケージ マネージャーの設定` で、NuGet のパッケージ復元が有効になっていること
4. ソリューションを右クリックして `NuGet パッケージの復元` を実行すること
5. 復元後、リビルドすること

このリポジトリでは `packages/` フォルダをGit管理しません。
必要なライブラリは `packages.config` を元に NuGet が復元します。

フォームデザイナーが開けない場合も、まずビルドエラーを解消してください。
Windows Forms デザイナーは、参照ライブラリが不足していたり、プロジェクトがビルドできない状態だと開けないことがあります。

## 通常作業の流れ

直接 `main` に作業せず、作業用ブランチを作ります。

1. `main` を選択する
2. `Pull` して最新版にする
3. 作業用ブランチを作る
4. 修正する
5. ビルドする
6. Git 変更で差分を確認する
7. コミットする
8. Push する
9. Pull Request を作る
10. レビュー後に `main` へマージする

## ブランチ名

ブランチ名は内容が分かる名前にします。

例:

```text
feature/csv-export
feature/ui-tabs
fix/excel-border
fix/backward-line-range
```

慣れるまでは以下のどちらかで十分です。

```text
feature/作業内容
fix/修正内容
```

## コミットの考え方

コミットは「あとから見て何をしたか分かる単位」で作ります。

良い例:

```text
CSV出力処理を追加
Excel罫線色を画面表示に合わせる
BackwardトレースのStart罫線情報を追加
```

避けたい例:

```text
修正
更新
いろいろ
```

## Push の前に確認するもの

Git 変更に以下が出てきた場合は、コミットする前に確認してください。

- `bin/`
- `obj/`
- `.vs/`
- `Logs/`
- `Export/`
- `*.user`
- 一時ファイル
- 個人用のメモやバックアップ

これらは通常、GitHub に上げないファイルです。

## Pull Request

Pull Request は、自分のブランチの変更を `main` に入れてよいか確認するための依頼です。

基本の流れ:

1. 作業ブランチを Push する
2. GitHub で Pull Request を作る
3. base を `main` にする
4. compare を自分の作業ブランチにする
5. 変更内容を書く
6. レビューを依頼する
7. 問題なければマージする

Pull Request には、何を変えたか、どう確認したかを書きます。

例:

```text
変更内容:
- CSV出力処理を追加
- 現在表示中のStart/Middle/EndグリッドをCSV化

確認:
- ソリューションのビルド成功
- CSV出力ファイルをExcelで開いて内容確認
```

## 作業前の Pull

作業を始める前に必ず `main` を Pull してください。

理由:

- 他の人の変更を取り込むため
- 古い状態から作業して競合するのを減らすため

Visual Studio では `Git 変更` または `Git` メニューから `Pull` できます。

## 競合した場合

Pull やマージで競合した場合は、慌ててコミットしないでください。

1. 競合ファイルを確認する
2. 自分の変更と相手の変更を見比べる
3. 残す内容を決める
4. ビルドする
5. 問題なければコミットする

判断に迷う場合は、競合ファイルをそのまま共有して相談してください。

## よくあるエラー

### ユーザー名とメールアドレスが構成されていない

Git のコミット作成者情報が未設定です。
Visual Studio の Git 設定でユーザー名とメールアドレスを登録してください。

### ownership / safe.directory のエラー

別PCや別ユーザーで作ったフォルダを使っている場合に出ることがあります。
自分が信頼しているプロジェクトフォルダであれば、safe.directory に登録します。

例:

```powershell
git config --global --add safe.directory H:/00.Project/TOK/LotTraceApp
```

### Push できない

まず以下を確認してください。

- GitHub にログインしているか
- リポジトリへの権限があるか
- 作業ブランチを Push しようとしているか
- Pull が必要な状態ではないか

## このプロジェクトでの注意

- 基本ブランチは `main`
- 作業は作業用ブランチで行う
- `main` へ直接 Push しない
- 変更後は必ずビルドする
- `LotTraceApp.ini` は共有設定を含むため、変更する場合は内容を確認する
- 出力ファイルやログは GitHub に上げない

## 参考リンク

- [GitHub Docs: Cloning a repository](https://docs.github.com/en/repositories/creating-and-managing-repositories/cloning-a-repository)
- [GitHub Docs: Creating a pull request with GitHub Desktop](https://docs.github.com/desktop/guides/contributing-to-projects/creating-a-pull-request)
- [Microsoft Learn: Use git fetch, pull, push and sync in Visual Studio](https://learn.microsoft.com/en-us/visualstudio/version-control/git-fetch-pull-sync)
- [Microsoft Learn: Create a pull request in Visual Studio](https://learn.microsoft.com/en-us/visualstudio/version-control/git-create-pull-request)

