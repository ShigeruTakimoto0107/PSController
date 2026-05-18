# PowerShellController (PSController)

PowerShellController は、PowerShell を外部プロセスとして起動し、  
**マクロファイル (.macro)** を用いて PowerShell を自動操作するための軽量コントローラです。

PowerShell の標準出力をリアルタイムで監視し、  
マクロコマンドを逐次実行することで、  
**対話的な PowerShell 操作を完全自動化**できます。

---

## 機能概要

- PowerShell の起動と制御
- マクロファイル (.macro) の実行
- `sendln` による PowerShell へのコマンド送信
- `wait` / `waitto` による出力待機
- `setvar` による変数展開（`%VAR%` 形式）
- `if` / `elseif` / `else` / `endif` による条件分岐
- `print` によるカラーメッセージ出力
- `ver` によるバージョン情報表示
- 起動直後のプロンプト誤検出防止ロジック搭載
- マクロ終了後のインタラクティブ入力への自動移行

---

## ディレクトリ構成

```
PSController/
├── macros/          # マクロファイル置き場
├── src/
│   ├── Commands/    # 各コマンドの実装
│   ├── Core/        # エントリーポイント・PowerShellホスト・実行コンテキスト
│   ├── Parser/      # マクロファイルのパース処理
│   └── Registry/    # コマンド登録・ビルド処理
├── Build.bat        # ビルドスクリプト
└── readme.md
```

---

## ソースファイル一覧

### src/Core/

| ファイル名 | 説明 |
|---|---|
| `Program.cs` | エントリーポイント。PowerShellプロセス起動・出力監視・インタラクティブループ |
| `PowerShellHost.cs` | WAITバッファ管理・出力待機・カラー出力などのホスト機能 |
| `ExecutionContext.cs` | 変数ストア・条件制御フラグ・`%VAR%`展開 |
| `VersionInfo.cs` | バージョン番号・ビルド情報 |

### src/Commands/

| ファイル名 | 説明 |
|---|---|
| `ICommand.cs` | コマンドインターフェース定義 |
| `PrintCommand.cs` | `print` コマンドの実装（カラー出力対応） |
| `SetVarCommand.cs` | `setvar` コマンドの実装 |
| `VerCommand.cs` | `ver` コマンドの実装（バージョン表示） |

### src/Parser/

| ファイル名 | 説明 |
|---|---|
| （パーサー関連ファイル） | マクロファイルの読み込み・1行パース処理 |

### src/Registry/

| ファイル名 | 説明 |
|---|---|
| `CommandRegistry.cs` | コマンド名 → 実行ハンドラを登録する中心レジストリ |
| `CommandRegistryBuilder.cs` | 全コマンドを CommandRegistry に登録する集約処理 |

---

## マクロファイルの書式

マクロファイルは **1行1コマンド**で構成されます。  
`#` または `;` で始まる行はコメントとして無視されます。

```
; これはコメント
wait >
print cyan 接続完了
setvar USER admin
sendln echo %USER%
waitto 5 >
print green 完了
```

---

## 使用可能なコマンド一覧

| コマンド | 引数 | 説明 |
|---|---|---|
| `wait` | `<文字列>` | 指定文字列が出力に現れるまで無制限に待機 |
| `waitto` | `<秒数> <文字列>` | 指定秒数以内に文字列が現れなければタイムアウト |
| `sendln` | `<文字列>` | PowerShell に1行送信 |
| `rawtext` | `<文字列>` | 改行なしで PowerShell に生文字列を送信 |
| `print` | `<色> <文字列>` | 指定色でコンソールにメッセージを出力 |
| `setvar` | `<名前> <値>` | 変数を設定（`%名前%` で展開） |
| `if` | `<左辺> <演算子> <右辺>` | 条件分岐の開始（`==`, `!=` に対応） |
| `elseif` | `<左辺> <演算子> <右辺>` | 追加条件分岐 |
| `else` | なし | 条件不一致時の処理 |
| `endif` | なし | 条件分岐の終了 |
| `ver` | なし | バージョン情報を表示 |

### print で使用できる色

`red` / `green` / `yellow` / `blue` / `cyan` / `magenta` / `white` / `gray`

### if コマンドの特殊変数

| 変数名 | 説明 |
|---|---|
| `lastwait` | 直前の `waitto` の結果（`ok` または `ng`）|

使用例：
```
waitto 5 >
if lastwait == ng
    print red タイムアウトしました
endif
```

---

## ビルド方法

```bat
Build.bat
```

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
| 1.0.0 | 初版リリース。基本コマンド・WAIT制御・プロンプト制御バグ修正 |
