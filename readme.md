# PowerShellController (PSController)

PowerShellController は、PowerShell を外部プロセスとして起動し、
**マクロファイル (.psm)** を用いて PowerShell を自動操作する軽量コントローラです。

PowerShell の標準出力をリアルタイムで監視し、
マクロコマンドを逐次実行することで、
**対話的な PowerShell 操作を完全自動化**できます。

---

## 機能概要

- PowerShell の起動と制御
- マクロファイル (.psm) の実行
- `sendln` による PowerShell へのコマンド送信
- `wait` / `waitto` による出力待機
- `setvar` / `getvar` による変数操作
- `if` / `elseif` / `else` / `endif` による条件分岐
- `loop` / `endloop` / `break` による繰り返し処理
- `goto` / ラベル によるジャンプ
- `call` / `return` によるサブルーチン呼び出し
- `include` によるマクロファイルの展開
- `echo on` / `echo off` によるエコーバック制御
- `print` によるカラーメッセージ出力
- `pause` による待機
- `setprompt` による動的プロンプト切り替え
- `admin` による管理者権限での自動再起動
- `.logopen` / `.logclose` によるトランスクリプト記録
- `ver` によるバージョン情報表示
- `exit` による PSC 終了
- 未登録コマンドはそのまま PowerShell に送信
- Fail Fast：事前条件違反は即エラー停止

---

## ディレクトリ構成
PSController/
├── bin/       # ビルド成果物
├── ico/       # アイコン
├── logs/      # ログ置き場
├── macros/    # マクロファイル置き場
├── ps1/       # スクリプト置き場
├── src/
│   ├── Commands/
│   ├── Core/
│   ├── Flow/
│   ├── IO/
│   ├── Meta/
│   ├── Parser/
│   ├── Registry/
│   └── System/
├── Build.bat
└── readme.md

---

## ソースファイル一覧

### src/Core/

| ファイル名 | 説明 |
|---|---|
| `Program.cs` | エントリーポイント |
| `PowerShellProcess.cs` | プロセス起動・出力監視 |
| `MacroRunner.cs` | マクロ実行ループ |
| `PowerShellHost.cs` | WAITバッファ・出力待機・カラー出力 |
| `ExecutionContext.cs` | 変数ストア・制御フラグ・`%VAR%`展開 |
| `MacroAbortException.cs` | マクロ強制停止用例外 |
| `VersionInfo.cs` | バージョン番号・ビルド情報 |

### src/Commands/

| ファイル名 | 説明 |
|---|---|
| `ICommand.cs` | コマンドインターフェース |
| `PrintCommand.cs` | `print` コマンド |
| `SetVarCommand.cs` | `setvar` コマンド |
| `GetVarCommand.cs` | `getvar` コマンド |
| `SetPromptCommand.cs` | `setprompt` コマンド |
| `PauseCommand.cs` | `pause` コマンド |
| `EchoCommand.cs` | `echo` コマンド |
| `VerCommand.cs` | `ver` コマンド |

### src/Flow/

| ファイル名 | 説明 |
|---|---|
| `IfCommand.cs` | `if` コマンド |
| `ElseIfCommand.cs` | `elseif` コマンド |
| `ElseCommand.cs` | `else` コマンド |
| `EndIfCommand.cs` | `endif` コマンド |
| `LoopCommand.cs` | `loop` コマンド |
| `EndLoopCommand.cs` | `endloop` コマンド |
| `BreakCommand.cs` | `break` コマンド |
| `GotoCommand.cs` | `goto` コマンド |
| `CallCommand.cs` | `call` コマンド |
| `ReturnCommand.cs` | `return` コマンド |

### src/IO/

| ファイル名 | 説明 |
|---|---|
| `WaitCommand.cs` | `wait` コマンド |
| `WaitToCommand.cs` | `waitto` コマンド |
| `SendLnCommand.cs` | `sendln` コマンド |

### src/Meta/

| ファイル名 | 説明 |
|---|---|
| `TranscriptOpenCommand.cs` | `.logopen` メタコマンド |
| `TranscriptCloseCommand.cs` | `.logclose` メタコマンド |

### src/System/

| ファイル名 | 説明 |
|---|---|
| `ExitCommand.cs` | `exit` コマンド |
| `AdminCommand.cs` | `admin` コマンド |

### src/Parser/

| ファイル名 | 説明 |
|---|---|
| `MacroLoader.cs` | マクロファイル読み込み・include展開 |

### src/Registry/

| ファイル名 | 説明 |
|---|---|
| `CommandRegistry.cs` | コマンド名→ハンドラ登録 |
| `CommandRegistryBuilder.cs` | 全コマンド登録の集約 |

---

## マクロファイルの書式

マクロファイルは **1行1コマンド**で構成されます。
`#` `;` `//` で始まる行はコメントとして無視されます。
インデントは自由です（空白・タブは自動除去）。
; これはコメント
wait >
print cyan 接続完了
setvar USER admin
sendln echo %USER%
waitto 5 >
if lastwait == ok
print green 完了
else
print red タイムアウト
endif

---

## コマンド一覧

### 入出力

| コマンド | 引数 | 説明 |
|---|---|---|
| `wait` | `<文字列>` | 文字列が出るまで無制限に待機 |
| `waitto` | `<秒数> <文字列>` | タイムアウト付き待機 |
| `sendln` | `<文字列>` | PowerShell に1行送信 |
| `getvar` | `<変数名>` | 直前のコマンド出力を変数に取り込む |

### 制御フロー

| コマンド | 引数 | 説明 |
|---|---|---|
| `if` | `<左辺> <演算子> <右辺>` | 条件分岐（`==` `!=`） |
| `elseif` | `<左辺> <演算子> <右辺>` | 追加条件分岐 |
| `else` | なし | 条件不一致時の処理 |
| `endif` | なし | 条件分岐の終了 |
| `loop` | `<回数>` | 指定回数繰り返す |
| `endloop` | なし | ループ末尾 |
| `break` | なし | ループを抜ける |
| `goto` | `<ラベル名>` | 指定ラベルにジャンプ |
| `call` | `<ラベル名>` | サブルーチン呼び出し |
| `return` | なし | サブルーチンから戻る |

### PSCコマンド

| コマンド | 引数 | 説明 |
|---|---|---|
| `print` | `<色> <文字列>` | カラーメッセージ出力 |
| `setvar` | `<名前> <値>` | 変数設定（`%名前%` で展開） |
| `getvar` | `<変数名>` | PowerShell出力を変数に取り込む |
| `setprompt` | `<文字列>` | プロンプト検出文字を変更 |
| `pause` | `<秒数>` | 指定秒数待機 |
| `echo` | `on` / `off` | エコーバック制御（デフォルト on） |
| `include` | `<ファイルパス>` | マクロファイルを展開して読み込む |
| `ver` | なし | バージョン情報表示 |

### システム

| コマンド | 引数 | 説明 |
|---|---|---|
| `exit` | なし | PSC を終了 |
| `admin` | なし | 管理者権限で再起動 |

### メタコマンド

| コマンド | 引数 | 説明 |
|---|---|---|
| `.logopen` | `<ファイルパス>` | トランスクリプト開始 |
| `.logclose` | なし | トランスクリプト停止 |

### ラベル

`:` で始まる行はラベルとして扱われます。
`goto` / `call` のジャンプ先として使用します。
:LABEL_NAME
print green ラベルに到達
return

### 未登録コマンド

登録されていないコマンドはそのまま PowerShell に送信されます。
ps1 スクリプトの直接呼び出しに活用できます。
wait >
.\ps1\setup.ps1
wait >

**注意：** 事前に `wait` が必要です。未確認の場合はエラー停止します。

---

## print で使用できる色

`red` / `green` / `yellow` / `blue` / `cyan` / `magenta` / `white`

---

## 特殊変数

| 変数名 | 説明 |
|---|---|
| `lastwait` | 直前の `waitto` の結果（`ok` / `ng`）|

---

## setprompt の使い方（SSH対応）

wait >
sendln ssh user@192.168.1.1
setprompt $
wait $
sendln ls -la
wait $
sendln exit
setprompt >
wait >
print green SSH完了

---

## goto / call / return の使い方

### goto（ジャンプ）
wait >
setvar FLAG ng
if %FLAG% == ok
goto SUCCESS
endif
goto ERROR
:SUCCESS
print green 成功
goto END
:ERROR
print red エラー
:END
print cyan 終了

### call / return（サブルーチン）
wait >
call GREET
print cyan メイン処理続行
:GREET
print green こんにちは
return

**注意：** ネストした `call` はサポートされていません。

---

## include の使い方
; main.psm
wait >
include common.psm
print green メイン処理

; common.psm
setvar APP_NAME PSController
print cyan %APP_NAME% 起動

- 相対パスは親ファイルのディレクトリ基準
- 循環参照は自動検出して警告

---

## echo on/off の使い方
wait >
echo off
sendln echo hello
wait >
echo on
sendln echo world
wait >

`echo off` 時はエコーバックが抑制されクリーンな出力になります。
プロンプトは常に表示されます。

---

## getvar の使い方
wait >
sendln $env:USERNAME
getvar USERNAME
print green ユーザー名: %USERNAME%

直前のコマンド出力の最終行を変数に取り込みます。
出力は画面に表示されません。

---

## ビルド方法
Build.bat

.NET Framework 4.0 以上が必要です。

---

## 動作環境

- Windows 10 / 11
- .NET Framework 4.0 以上
- Windows PowerShell 5.1

---

## バージョン履歴

| バージョン | 内容 |
|---|---|
| 1.0.0 | 初版リリース。全コマンド実装・Fail Fast設計・構成整理 |
