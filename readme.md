# PowerShellController (PSController)

> [!WARNING]
> **【開発・実行テスト中のステータス告知】**
> 本プロジェクトは現在、精力的に開発および実行テストを行っている段階（Beta / WIP）です。一部の機能や内部構造、マクロの仕様などは予告なくブラッシュアップ・変更される可能性があります。実験的な導入、またはテスト環境での検証を中心にご利用ください。

Windows標準のPowerShellを、TeraTermマクロやJCL、あるいはAccessマクロのような直感的で懐かしい「手続き型スクリプト」の感覚で完全掌握する軽量自動化コントローラ。

外部プロセスとして分離されたPowerShellの入出力をミリ秒単位でリアルタイム監視し、独自のマクロファイル (**`.pscm`**) によって対話的なオペレーションをシンプルかつ的確に自動化します。


## 拡張性について

本ツールは単なるコマンド送出機ではありません。操作対象となるPowerShellの環境を充実させることで、その自動化領域は爆発的に広がります。

- ブラウザ自動化 (SeleniumやPlaywrightとの連携)
- Microsoft TeamsやSharePointなどの各種SaaS自動化
- Office製品や各種社内業務システムのシームレスな制御

マクロによる極めてシンプルな記述でありながら、その実態は完全なプログラム。ロジックの組み方次第で、Power Automateや高価なRPAツールをも凌駕する、美しく強力な自動化環境をあなた自身のコードで組み上げることができます。


## ビルドと実行テスト

大掛かりな開発環境（Visual Studio等）のインストールは一切必要ありません。依存関係ゼロで、ダウンロード後すぐにビルドと全コマンドの自動検証を行うことができます。

### 1. 事前準備
リポジトリのソースコード一式をローカルに配置し、あらかじめ検証用のマクロファイルを「macros」フォルダ内に配置しておきます。

```text
Root
│  build.bat
└─macros
        test_all.pscm       (全コマンド確認用マクロ)
        include_test.pscm   (include機能テスト用マクロ)
```

### 2. ビルドの実行
ルートディレクトリにある「build.bat」を実行します。Windowsに標準搭載されているC#コンパイラ(csc.exe)が自動探索され、その場でネイティブな実行バイナリ(PowerShellController.exe)が「bin」フォルダ内に生成されます。

### 3. 自動実行テストの選択
ビルドが成功すると、コンソール上にテスト実行の確認メッセージが表示されます。

```text
[SUCCESS] PowerShellController.exe has been built.
--------------------------------------------------
[QUESTION] 全コマンド確認マクロを実行しますか？ [Y/N] (Default:Y):
```

- "Y" を入力（またはそのままEnter）すると、画面が切り替わり「test_all.pscm」が自動追従で走り出します。全コマンドがノンストップで自動検証され、実行ログが「logs」フォルダに安全に出力されます。
- "N" を入力すると、テストを実行せずにビルドのみで安全に終了します。


## ディレクトリ構成

```text
Root
│  build.bat          (Visual Studio不要の超軽量コンパイルスクリプト)
│  LICENSE            (MITライセンス公開ファイル)
│  readme.md          (本ドキュメント)
│  Reorganize.bat     (ソース配置・整理用スクリプト)
│
├─bin                 (コンパイルされたEXE本体の出力先)
│      PowerShellController.exe
│
├─ico
│      PSC.ico        (生成されるEXEに埋め込まれる専用アイコン)
│
├─macros              (ユーザー用マクロ配置ROOT)
│  │  1_関連付け登録.pscm  (Windowsに.pscm拡張子をクリーン登録するマクロ)
│  │  2_関連付け削除.pscm  (登録したレジストリを安全に全削除するマクロ)
│  │  include_test.pscm
│  │  test_all.pscm
│  │
│  └─ps1              (単機能ごとに細分化された内部PowerShellスクリプト群)
│          find_exe.ps1
│          reg_clean.ps1
│          reg_register.ps1
│          restart_explorer.ps1
│
└─src                 (C# ソースコードROOT)
    ├─Commands        (基本マクロコマンド群)
    │      EchoCommand.cs       (echo on/off 行番号付きエコーバック制御)
    │      GetVarCommand.cs     (getvar 変数取得処理)
    │      ICommand.cs          (全コマンドの基底となる共通インターフェース)
    │      PauseCommand.cs      (pause オペレーター確認の一時待機)
    │      PrintCommand.cs      (print コンソールへのカラーログ出力)
    │      SetVarCommand.cs     (setvar 変数定義・代入処理)
    │      VerCommand.cs        (ver バージョン情報表示)
    │
    ├─Core            (コントローラの心臓部・実行基盤)
    │      ExecutionContext.cs  (変数の状態やコールスタックを保持する実行コンテキスト)
    │      MacroAbortException.cs (Fail Fast時に行番号付きで処理を安全に停止させる例外)
    │      MacroRunner.cs       (解析されたマクロを行単位で高速に逐次実行するコア)
    │      PowerShellHost.cs    (PowerShellプロセスとの直接の対話を抽象化するホスト)
    │      PowerShellProcess.cs (外部プロセスとしてPowerShellを安全にハンドリングする実装)
    │      Program.cs           (アプリケーションのエントリーポイント)
    │      VersionInfo.cs       (Build.batから自動生成されるバージョン・Gitコミット情報)
    │
    ├─Flow            (条件分岐やループ、ジャンプなどの構造化フロー制御)
    │      BreakCommand.cs      (break ループからの即時脱出)
    │      CallCommand.cs       (call サブルーチン呼び出し)
    │      ElseCommand.cs       (else 条件不一致時の処理切り替え)
    │      ElseIfCommand.cs     (elseif 多重条件分岐)
    │      EndIfCommand.cs      (endif 条件分岐の終了ブロック終了判定)
    │      EndLoopCommand.cs    (endloop ループの終端・先頭への巻き戻し)
    │      GotoCommand.cs       (goto 指定ラベルへのジャンプ制御)
    │      IfCommand.cs         (if 条件分岐の開始判定)
    │      LoopCommand.cs       (loop 指定回数または無限繰り返しブロックの開始)
    │      ReturnCommand.cs     (return サブルーチンからの復帰)
    │
    ├─IO              (PowerShellプロセスとの厳密な入出力制御)
    │      SendLnCommand.cs     (sendln コマンド末尾に改行を付与して透過送信)
    │      WaitCommand.cs       (wait 標準プロンプトの出現をミリ秒単位で厳密に待機)
    │      WaitToCommand.cs     (waitto 任意の文字列やパターンの出現を指定時間待機)
    │
    ├─Meta            (証跡管理および運用メタコマンド)
    │      TranscriptCloseCommand.cs (.logclose ログ記録の安全な終了)
    │      TranscriptOpenCommand.cs  (.logopen 監査証跡用トランスクリプトの完全記録開始)
    │
    ├─Parser          (マクロスクリプトの構文解析)
    │      MacroLine.cs         (行番号、コマンド名、引数、コメントを構造化するデータモデル)
    │      MacroLoader.cs       (include展開やコメント除去、ラベル事前解析を行うロード機構)
    │
    ├─Registry        (コマンドの動的マッピング)
    │      CommandRegistry.cs   (文字列とコマンドクラスを高速に紐付ける一元管理オブジェクト)
    │      CommandRegistryBuilder.cs (各フォルダに分散したコマンドを検知し登録するビルダー)
    │
    └─System          (OS・システムレベル制御)
            AdminCommand.cs      (admin 管理者権限への自動UAC昇格とプロセス再起動)
            ExitCommand.cs       (exit コントローラの安全な終了処理)
            KillPsCommand.cs     (killps 自分以外のPowerShellプロセスを強制終了)
```


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


## 本ツールの安全性と基本仕様（アーキテクチャ）

1. **冷徹な「Fail Fast（事前条件違反の即時停止）」**
不整合を検知したままマクロが暴走し、環境を破壊することを絶対に防ぎます。期待するプロンプトが指定時間内に返ってこない場合や、事前条件への違反を検知した瞬間、ツールは処理を即時強制停止（Terminate）します。
エラー発生時には、マクロのどの部分で問題が起きたかが瞬時に特定できるよう、対象の「行番号」がエラーログに明示的に出力されます。

2. **未登録コマンドのネイティブ透過送信**
マクロ言語にない高度なPowerShellコマンドや独自の関数は、コントローラを素通りしてそのままPowerShellプロセスに透過送信されます。マクロのシンプルさと、PowerShellの強力な表現力を100%両立します。

3. **タスクマネージャーの可読性**
バックグラウンド実行時や監査ログ監視時、不審なプロセスとして誤認されやすい「3文字の謎のEXE」ではなく、「PowerShellController.exe」という規律正しいプロセス名で明示的に動作します。

4. **カレントディレクトリの固定**
マクロファイルがどこに配置されていても、PowerShellのカレントディレクトリは常に `bin` フォルダに固定されます。ps1スクリプトへの相対パスが安定して動作します。


## 免責事項 (Disclaimer)

本ソフトウェアの使用（実行、複製、改変、配布等の一切の行為）に伴い、万が一ユーザーの環境、システム、データ、ネットワーク、ハードウェア等に損害、損失、障害、データの破損または漏洩、その他いかなる不利益が生じた場合であっても、開発者および著作権者はその理由の如何を問わず、一切の責任（直接損害、間接損害、派生損害、特別損害、逸失利益を含むがこれらに限定されない）を負いません。

本ソフトウェアは「現状のまま（As Is）」提供され、明示または黙示を問わず、その特定の目的への適合性、機能性、正確性、信頼性、または瑕疵の不在について、開発者は一切の保証をいたしません。マクロの記述および実行にあたっては、ユーザー自身の責任において、事前に十分なテスト環境での検証を行ってください。


## ライセンス

本プロジェクトは MIT License のもとで公開されています。商用利用、改変、再配布が自由にいただけます。詳細は LICENSE ファイルを参照してください。
