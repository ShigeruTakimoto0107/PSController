# PowerShellController

PowerShellController は、PowerShell を外部プロセスとして起動し、  
**マクロファイル (.macro / .psm)** を用いて PowerShell を自動操作するための軽量コントローラです。

PowerShell の標準出力をリアルタイムで監視し、  
マクロコマンドを逐次実行することで、  
**対話的な PowerShell 操作を完全自動化**できます。

---

# 機能概要

- PowerShell の起動と制御（管理者権限起動も可能）
- マクロファイル (.macro / .psm) の実行
- sendln による PowerShell へのコマンド送信
- wait / waitto による出力待機（誤一致防止ロジック搭載）
- setvar による変数展開
- if / elseif / else / endif による条件分岐
- loop / endloop, while / endwhile による繰り返し
- break / continue によるループ制御
- include / call / return によるサブルーチン
- goto / label によるジャンプ
- rawtext による生文字列送信
- print によるメッセージ出力
- logopen / logclose によるログ出力

---

# ディレクトリ構成（最新版）

```
src/
  CommandRegistry.cs
  ExecutionContext.*.cs
  MacroLine.cs
  MacroLoader.*.cs
  PowerShellHost.*.cs
  Program.cs
  VersionInfo.cs
```

---

# ソースファイル一覧（説明付き）

| # | ファイル名 | 説明 |
|---|------------|------|
| 1 | CommandRegistry.cs | コマンド名 → 実行ハンドラを登録する中心レジストリ |
| 2 | ExecutionContext.Condition.cs | IF/ELSEIF 条件式の評価ロジック |
| 3 | ExecutionContext.Control.cs | IF/ELSE/ENDIF、LOOP、WHILE の制御状態管理 |
| 4 | ExecutionContext.Core.cs | 行番号・CallStack・LoopStack などコア状態 |
| 5 | ExecutionContext.Expression.cs | `%VAR%` 展開や式評価（==, !=, <, > など） |
| 6 | ExecutionContext.Skip.cs | IF/LOOP のスキップ状態（CurrentSkip）管理 |
| 7 | ExecutionContext.Variables.cs | setvar / 変数テーブルの管理 |
| 8 | MacroLine.cs | 1 行のマクロを構造化（コマンド名・引数・種別） |
| 9 | MacroLoader.Core.cs | マクロファイル読み込みの中心処理 |
| 10 | MacroLoader.Parser.cs | 1 行を MacroLine にパースするロジック |
| 11 | MacroLoader.Utils.cs | コメント除去・空行処理などの補助関数 |
| 12 | PowerShellHost.CommandKey.cs | コマンド名の正規化（大文字小文字無視など） |
| 13 | PowerShellHost.Commands.Basic.cs | sendln / print / pause / rawtext / logopen など基本コマンド |
| 14 | PowerShellHost.Commands.BreakContinue.cs | break / continue の実装 |
| 15 | PowerShellHost.Commands.BuildRegistry.cs | 全コマンドを CommandRegistry に登録する集約処理 |
| 16 | PowerShellHost.Commands.Control.cs | IF/ELSE/ENDIF の構文チェック |
| 17 | PowerShellHost.Commands.If.cs | if / elseif / else / endif の実行ハンドラ |
| 18 | PowerShellHost.Commands.IncludeCall.cs | include / call / return の実装 |
| 19 | PowerShellHost.Commands.LabelGoto.cs | label / goto の実装 |
| 20 | PowerShellHost.Commands.Loop.cs | loop / endloop の実装 |
| 21 | PowerShellHost.Commands.Wait.cs | wait / waitto の実装（WaitForText / Timeout） |
| 22 | PowerShellHost.Commands.While.cs | while / endwhile の実装 |
| 23 | PowerShellHost.Core.Wait.cs | PowerShell 出力監視・waitto の内部状態管理 |
| 24 | PowerShellHost.Input.cs | PowerShell プロセスへの入力処理 |
| 25 | PowerShellHost.Runner.cs | マクロ実行ループ・IF/LOOP の構文チェック |
| 26 | PowerShellHost.StreamReader.cs | 非同期で PowerShell 出力を読み取る仕組み |
| 27 | Program.cs | エントリーポイント・管理者昇格・ファイル読み込み |
| 28 | VersionInfo.cs | バージョン番号・ビルド情報 |

---

# マクロファイルの書式

マクロファイルは **1 行 1 コマンド**で構成されます。

例:

```
setvar A 123
sendln echo %A%
wait 123
```

---

# 使用可能なコマンド一覧

### sendln \<文字列\>  
PowerShell に 1 行送信します。

### wait \<文字列\>  
指定した文字列が出力に現れるまで待機します。

### waitto \<秒数\> \<文字列\>  
指定秒数以内に文字列が出力されなければ TIMEOUT。

### pause \<秒数\>  
指定秒数だけ停止します。

### print \<文字列\>  
コンソールに文字列を出力します。

### setvar \<名前\> \<値\>  
変数を設定します。

### label \<名前\>  
