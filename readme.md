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

````text
Root
│  build.bat
└─macros
        test_all.pscm       (全コマンド確認用マクロ)
        include_test.pscm   (include機能テスト用マクロ)
````

### 2. ビルドの実行
ルートディレクトリにある「build.bat」を実行します。Windowsに標準搭載されているC#コンパイラ(csc.exe)が自動探索され、その場でネイティブな実行バイナリ(PowerShellController.exe)が「bin」フォルダ内に生成されます。

### 3. 自動実行テストの選択
ビルドが成功すると、コンソール上にテスト実行の確認メッセージが表示されます。

````text
[SUCCESS] PowerShellController.exe has been built.
--------------------------------------------------
[QUESTION] 全コマンド確認マクロを実行しますか？ [Y/N] (Default:Y):
````

- "Y" を入力（またはそのままEnter）すると、画面が切り替わり「test_all.pscm」が自動追従で走り出します。全コマンドがノンストップで自動検証され、実行ログが「logs」フォルダに安全に出力されます。
- "N" を入力すると、テストを実行せずにビルドのみで安全に終了します。


## ディレクトリ構成

````text
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
    │      SetPromptCommand.cs  (setprompt 動的プロンプトターゲット切り替え)
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
````


## 機能概要（マクロコマンド一覧）

- PowerShellの起動と制御：安全な外部プロセス分離
- マクロファイル (**`.pscm`**) の実行：上から順に逐次高速インタープリタ実行
- sendln：PowerShellへのコマンド送信
- wait / waitto：出力およびターゲットプロンプトの厳密な待機
- setvar / getvar：マクロ内での変数操作
- if / elseif / else / endif：柔軟な条件分岐
- loop / endloop / break：繰り返し処理
- goto / ラベル：指定ラベルへのジャンプ
- call / return：サブルーチン呼び出し
- include：外部マクロファイルの静的展開（共通処理の共通化）
- echo on / echo off：エコーバック（コマンド非表示）制御、行番号付きのデバッグ追跡
- print：ログ視認性を高めるカラーメッセージ出力
- pause：オペレーター確認のための安全な一時待機
- setprompt：SSH接続先や特権環境に追従する動的プロンプト切り替え
- admin：Windows管理者権限（UAC）への自動昇格・再起動
- .logopen / .logclose：オペレーション証跡（トランスクリプト）の完全記録
- ver：バージョン情報の表示
- exit：安全なプロセス切断とPSCの正常終了


## マクロ構文の具体例

### 基本的な対話自動化とFail Fast

````text
; サーバーの初期設定自動化サンプル
wait >
print cyan サーバー情報取得開始...
sendln Get-ComputerInfo
wait >

; 特権が必要な処理への移行
admin
````

### 動的プロンプト切り替え（Linux等へのSSHリモート接続）
Windows標準環境から他セグメントのサーバーやLinuxへ接続し、プロンプトが変わる運用もシームレスに追従します。

````text
wait >
sendln ssh user@192.168.1.1
setprompt $
wait $
sendln ls -la
wait $
sendln exit
setprompt >
wait >
print green Linuxサーバー内操作および安全なログアウトを完了
````


## 分岐・ループ・サブルーチンの高度な制御

### goto（ジャンプ）による例外ハンドリング

````text
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
````

### call / return（サブルーチン）

````text
wait >
call GREET
print cyan メイン処理続行

:GREET
print green [Subroutine] 初期化処理を実行中...
return
````
注意：ネストした call（サブルーチン内からのさらに別サブルーチン呼び出し）はサポートされていません。


### include による共通部品化

````text
; main.pscm
wait >
include common.pscm
print green メイン処理実行

; common.pscm
setvar APP_NAME PowerShellController
print cyan %APP_NAME% 共通ライブラリをロードしました
````
- 相対パスは親ファイルのディレクトリを基準とします。
- 循環参照（AがBを呼び、BがAを呼ぶ）は内部パーサーで自動検出し、実行前に厳格にブロックします。


### echo on/off による機密情報の保護とデバッグ
パスワード入力時やログを汚したくない場合は echo off で出力を一時的にマスクできます。
また、echo有効時は内部の行番号が合わせて出力されるため、大規模なマクロでもデバッグが容易です。

````text
wait >
echo off
sendln $password = "Secret123"
wait >
echo on
sendln Write-Output "パスワード設定完了"
wait >
````


## 本ツールの安全性と基本仕様（アーキテクチャ）

1. 冷徹な「Fail Fast（事前条件違反の即時停止）」
不整合を検知したままマクロが暴走し、環境を破壊することを絶対に防ぎます。期待するプロンプトが指定時間内に返ってこない場合や、事前条件への違反を検知した瞬間、ツールは処理を即時強制停止（Terminate）します。
エラー発生時には、マクロのどの部分で問題が起きたかが瞬時に特定できるよう、対象の「行番号」がエラーログに明示的に出力されます。

2. 未登録コマンドのネイティブ透過送信
マクロ言語にない高度なPowerShellコマンドや独自の関数は、コントローラを素通りしてそのままPowerShellプロセスに透過送信されます。マクロのシンプルさと、PowerShellの強力な表現力を100%両立します。

3. タスクマネージャーの可読性
バックグラウンド実行時や監査ログ監視時、不審なプロセスとして誤認されやすい「3文字の謎のEXE」ではなく、「PowerShellController.exe」という規律正しいプロセス名で明示的に動作します。


## 免責事項 (Disclaimer)

本ソフトウェアの使用（実行、複製、改変、配布等の一切の行為）に伴い、万が一ユーザーの環境、システム、データ、ネットワーク、ハードウェア等に損害、損失、障害、データの破損または漏洩、その他いかなる不利益が生じた場合であっても、開発者および著作権者はその理由の如何を問わず、一切の責任（直接損害、間接損害、派生損害、特別損害、逸失利益を含むがこれらに限定されない）を負いません。

本ソフトウェアは「現状のまま（As Is）」提供され、明示または黙示を問わず、その特定の目的への適合性、機能性、正確性、信頼性、または瑕疵の不在について、開発者は一切の保証をいたしません。マクロの記述および実行にあたっては、ユーザー自身の責任において、事前に十分なテスト環境での検証を行ってください。


## ライセンス

本プロジェクトは MIT License のもとで公開されています。商用利用、改変、再配布が自由にいただけます。詳細は LICENSE ファイルを参照してください。