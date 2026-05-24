# PowerShellController (PSController)

> [!WARNING]
> **【開発・実行テスト中のステータス告知】**
> 本プロジェクトは現在、精力的に開発および実行テストを行っている段階（Beta / WIP）です。一部の機能や内部構造、マクロの仕様などは予告なくブラッシュアップ・変更される可能性があります。実験的な導入、またはテスト環境での検証を中心にご利用ください。

**Windows標準環境で駆動する、超軽量・手続き型自動化コントローラ。**

PowerShellControllerは、標準のPowerShellをTeraTermマクロのようなシンプルかつ懐かしい「手続き型スクリプト」で操るための自動化エンジンです。外部プロセスとしてPowerShellをリアルタイム監視し、独自のマクロファイル（`.pscm`）によって対話的な業務フローを劇的に効率化します。

## このツールの特長

* **Zero Dependency / Native:** 外部ライブラリは一切不要。Windows標準のC#コンパイラ（csc.exe）とPowerShellのみで完結します。
* **Minimal Codebase:** 全ソースコードは約1,300行。軽量かつ透明性が高く、セキュリティが厳しい環境下でもコード全体を即座に精査・監査可能です。
* **High Performance:** 外部プロセスの入出力をミリ秒単位で制御。あなたのコーディング次第で、高価なRPAツールに匹敵する堅牢な自動化環境を構築できます。
* **MIT License:** 全ソースを公開。改変・利用の制限なく、あなたの業務環境に合わせて自由にカスタマイズしてください。

## なぜこれが「拡張性」に優れているのか

本ツールは単なるコマンド送出機ではありません。PowerShellの強力なエコシステムと直結することで、その可能性は無限に広がります。

* **Web Automaton:** Selenium/Playwright等を活用したブラウザ操作の抽象化
* **SaaS Integration:** Microsoft Teams, SharePoint等の業務API連携
* **Enterprise Control:** Office製品やレガシーシステムを含む、社内業務のシームレスな統合

「シンプルに書き、深く動かす。」
ロジックを組み上げる美しさと、業務を確実に遂行するパワーを兼ね備えた、あなたのための自動化基盤です。

---
## ビルドと実行テスト

大掛かりな開発環境（Visual Studio等）のインストールは一切必要ありません。依存関係ゼロで、ダウンロード後すぐにビルドと全コマンドの自動検証を行うことができます。

### 1. 事前準備
リポジトリのソースコード一式をローカルに配置し、あらかじめ検証用のマクロファイルを「macros」フォルダ内に配置しておきます。

```text
Root
│  build.bat
└─macros
    └─ビルド時の設定
            │ コマンドTEST.pscm           (全コマンド確認用マクロ)
            │ └include_test.pscm         (include機能テスト用マクロ)
            └─Register_Association.pscm  (プログラムとの関連付け用マクロ)
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

# PowerShellController

## ドキュメント
- [設計仕様書](docs/design/architecture.md)

[サンプルマクロ集]
- [Google自動検索](docs/sample001/001.md)

## 関連リンク
- [マクロの書き方解説](docs/guide/macros.md)

## ディレクトリ構成

```text
Root
│  build.bat          (Visual Studio不要の超軽量コンパイルスクリプト)
│  LICENSE            (MITライセンス公開ファイル)
│  readme.md          (本ドキュメント)
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
