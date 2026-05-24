## マクロコマンド詳細リファレンス

### 入出力制御

---

#### `wait パターン`
PowerShellの出力に指定パターンが現れるまで待機します。最も多用する同期コマンドです。

```text
wait >
```

**注意事項：**
- `wait >` が必要なのは、PowerShellにコマンドを送信した後、次の内部コマンドを実行する前です。
- `print`・`setvar`・`if`・`pause`・`echo` などの内部コマンドの後には不要です。
- `getvar` の直前には書いてはいけません（`getvar` 自身がプロンプトを待つため、二重消費になりフリーズします）。
- `sendln` の直後に `getvar` が続く場合も同様です。

```text
; 正しい例：Unknownコマンドの後はwait >
../macros/ps1/some.ps1
wait >
print cyan "次の処理..."

; 正しい例：getvarの直前にwait >は不要
../macros/ps1/some.ps1
getvar myvar

; 誤った例：getvarの直前にwait >を書いてはいけない
../macros/ps1/some.ps1
wait >          ← NG：getvarがタイムアウトしてフリーズする
getvar myvar
```

---

#### `waitto 秒数 パターン`
指定した秒数以内に指定パターンが出現するまで待機します。タイムアウト時は `LastWaitResult` に `false` がセットされ、`if` で分岐できます。

```text
waitto 10 completed
if %LastWaitResult% == false
    print red "タイムアウトしました"
endif
```

---

#### `sendln コマンド文字列`
PowerShellに任意のコマンドを送信します。変数は自動展開されます。

```text
sendln Get-Date
sendln ../macros/ps1/some.ps1 %ARG%
```

**注意事項：**
- 実行前にプロンプトが確認済み（`wait >` 済み）でなければ `[ERROR]` で即時停止します。
- 引数をダブルクォートで囲まないでください。クォートごとPowerShellに送信されます。
- `sendln` の直後に `getvar` を続ける場合は `wait >` を挟まないでください。

```text
; 正しい例
sendln ../macros/ps1/Burauza_Navigate.ps1 %URL%

; 誤った例（クォートごと送信される）
sendln "../macros/ps1/Burauza_Navigate.ps1 %URL%"
```

---

#### `getvar 変数名`
直前にPowerShellへ送信したコマンドの出力（最終行）を変数に格納します。

```text
../macros/ps1/find_exe.ps1
getvar my_exe
print green "EXEパス: %my_exe%"
```

**注意事項：**
- `getvar` 自身がプロンプトを待機するため、直前に `wait >` を書いてはいけません。
- 直前に必ずUnknownコマンドまたは `sendln` でPowerShellにコマンドを送信してください。
- タイムアウトは5000ms です。5秒以内にプロンプトが返らない場合は空文字が格納されます。

```text
; 正しい例
../macros/ps1/get_status.ps1
getvar status

; 誤った例
../macros/ps1/get_status.ps1
wait >          ← NG
getvar status   ← タイムアウトして空文字になる
```

---

### 変数操作

---

#### `setvar 変数名 値`
変数を定義または上書きします。

```text
setvar URL https://www.google.com/
setvar RETRY 3
```

---

#### 変数の展開
マクロ内では `%変数名%` の形式で変数を展開できます。`sendln`・`print`・`if` など多くのコマンドで使用できます。

```text
setvar NAME PowerShellController
print cyan "ようこそ %NAME% へ"
sendln Write-Output %NAME%
```

---

### 表示・ログ

---

#### `print 色 メッセージ`
指定色でコンソールにメッセージを出力します。PowerShellには何も送信しません。

使用可能な色：`red` / `green` / `yellow` / `blue` / `magenta` / `cyan` / `white`

```text
print cyan "処理を開始します..."
print green "完了しました"
print red "エラーが発生しました"
```

**注意事項：**
- `print` の後に `wait >` は不要です。PowerShellにコマンドを送信しないためプロンプトは返りません。

---

#### `echo on` / `echo off`
PowerShellへ送信されるコマンドのエコーバック表示を制御します。

```text
echo off
sendln $password = "Secret123"
wait >
echo on
```

**注意事項：**
- `echo off` 時はPowerShellに送信したコマンド文字列がコンソールに表示されなくなります。パスワードなど機密情報の入力時に使用してください。
- `echo on`（デフォルト）時は行番号付きでコマンドがコンソールに表示され、デバッグが容易になります。

---

#### `ver`
PSCのバージョン情報を表示します。引数はありません。

```text
ver
```

---

#### `.logopen ログファイルパス`
指定したパスへのトランスクリプト（監査証跡）記録を開始します。内部でPowerShellの `Start-Transcript` を生成・送信するメタコマンドです。

```text
.logopen C:\logs\operation.log
```

---

#### `.logclose`
トランスクリプトの記録を終了します。引数はありません。内部でPowerShellの `Stop-Transcript` を生成・送信するメタコマンドです。

```text
.logclose
```

---

### フロー制御

---

#### `if 条件` / `elseif 条件` / `else` / `endif`
条件分岐を行います。条件は `==` または `!=` で比較します。

```text
if "%STATUS%" == "OK"
    print green "正常です"
elseif "%STATUS%" == "WARN"
    print yellow "警告があります"
else
    print red "異常です"
endif
```

---

#### `loop 回数` / `endloop`
指定回数繰り返します。回数を省略または0を指定すると無限ループになります。

```text
loop 3
    print cyan "処理中..."
    sendln Get-Date
    wait >
endloop
```

---

#### `break`
現在のループを即時脱出します。

```text
loop
    waitto 5 completed
    if %LastWaitResult% == true
        break
    endif
endloop
```

---

#### `goto ラベル名`
指定ラベルへジャンプします。

```text
goto ERROR

:SUCCESS
print green "成功"
goto END

:ERROR
print red "失敗"

:END
print cyan "終了"
```

---

#### `call ラベル名` / `return`
サブルーチンを呼び出します。`return` で呼び出し元の次の行に戻ります。

```text
call INIT
print cyan "メイン処理続行"

:INIT
print green "初期化中..."
return
```

**注意事項：**
- ネストした `call`（サブルーチン内からさらに別サブルーチンを呼び出す）はサポートされていません。

---

#### `include ファイルパス`
外部マクロファイルを静的に展開します。相対パスは親ファイルのディレクトリを基準とします。

```text
include common.pscm
```

**注意事項：**
- 循環参照（AがBを呼び、BがAを呼ぶ）は内部パーサーで自動検出し、実行前に厳格にブロックします。

---

### システム制御

---

#### `admin`
管理者権限への昇格を制御します。

- すでに管理者権限で動作している場合：何もせず次の行へ進みます。
- 管理者権限で動作していない場合：UACダイアログを表示し、管理者権限で自分自身を再起動します。
- ローカル管理者グループに属していない、またはUACをキャンセルした場合：`[ERROR]` で即時停止します。

```text
admin
wait >
; ここから管理者権限で実行される
```

**注意事項：**
- `admin` の直後に `wait >` が必要です。PowerShellの起動時プロンプトをここで待ちます。
- `admin` コマンド自身はPowerShellに何も送信しません。

---

#### `killps`
自分自身（PSCプロセス）および管理下のPowerShellプロセスを除く、他のすべてのPowerShellプロセスを強制終了します。セッション開始時にゾンビプロセスを一掃する用途に使います。

```text
killps
wait >
```

**注意事項：**
- PSCが管理しているPowerShellプロセスは停止しません。
- `killps` 後に `wait >` は不要です（PowerShellには何も送信しません）。

---

#### `pause` / `pause 秒数`
- 引数なし：「続行するには何かキーを押してください...」と表示し、キー入力を待ちます。
- 引数あり：指定した秒数だけ待機します。

```text
pause           ; キー入力待ち
pause 3         ; 3秒待機
```

---

#### `exit`
PSCを安全に終了します。引数はありません。

```text
exit
```

---

### wait > が必要な場面・不要な場面（まとめ）

**必要な場面：** PowerShellにコマンドを送信した後、次の内部コマンドを実行する前。

```text
../macros/ps1/some.ps1
wait >
print cyan "次の処理..."

sendln some_command
wait >
print cyan "次の処理..."
```

**不要な場面：**

1. 内部コマンドの後（`print`・`setvar`・`if`・`endif`・`pause`・`echo` など）
2. Unknownコマンドまたは `sendln` の直後に `getvar` が続く場合（`getvar` 自身がプロンプトを待つため、`wait >` を挟むとフリーズします）

```text
; 正しい例
../macros/ps1/some.ps1
getvar myvar

sendln some_command
getvar myvar

; 誤った例
../macros/ps1/some.ps1
wait >          ← NG：getvarがタイムアウトしてフリーズする
getvar myvar
```


## マクロ構文の具体例

### 基本的な対話自動化とFail Fast

```text
; サーバーの初期設定自動化サンプル
wait >
print cyan サーバー情報取得開始...
sendln Get-ComputerInfo
wait >

; 特権が必要な処理への移行
admin
wait >
```

### 動的プロンプト切り替え（Linux等へのSSHリモート接続）
Windows標準環境から他セグメントのサーバーやLinuxへ接続し、プロンプトが変わる運用もシームレスに追従します。

```text
wait >
sendln ssh user@192.168.1.1
waitto 10 $
wait $
sendln ls -la
wait $
sendln exit
wait >
print green Linuxサーバー内操作および安全なログアウトを完了
```

### goto（ジャンプ）による例外ハンドリング

```text
wait >
setvar FLAG ng
if %FLAG% == ok
    goto SUCCESS
endif
goto ERROR

:SUCCESS
print green 処理成功
goto END

:ERROR
print red 事前チェックに失敗したため、安全に分岐します

:END
print cyan スクリプト終了
```

### call / return（サブルーチン）

```text
wait >
call GREET
print cyan メイン処理続行

:GREET
print green [Subroutine] 初期化処理を実行中...
return
```

注意：ネストした call（サブルーチン内からのさらに別サブルーチン呼び出し）はサポートされていません。

### include による共通部品化

```text
; main.pscm
wait >
include common.pscm
print green メイン処理実行

; common.pscm
setvar APP_NAME PowerShellController
print cyan %APP_NAME% 共通ライブラリをロードしました
```

- 相対パスは親ファイルのディレクトリを基準とします。
- 循環参照（AがBを呼び、BがAを呼ぶ）は内部パーサーで自動検出し、実行前に厳格にブロックします。

### echo on/off による機密情報の保護とデバッグ

```text
wait >
echo off
sendln $password = "Secret123"
wait >
echo on
sendln Write-Output "パスワード設定完了"
wait >
```

### ゾンビプロセス一掃と変数キャプチャの組み合わせ

```text
killps
wait >
print cyan EXEパスを特定中...
../macros/ps1/find_exe.ps1
getvar my_exe
print green "EXEパス: %my_exe%"
```