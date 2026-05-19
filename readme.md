# PowerShellController (PSController)

Windowsのエンタープライズ運用管理を劇的に効率化する、堅牢かつ軽量なPowerShell自動化コントローラ。

PowerShellを外部プロセスとして安全に起動・制御し、独自のマクロファイル (.psm) を用いて、これまで自動化が困難だった「対話的なプロンプト操作」を完全自動化します。


## 設計思想と強力な特徴（大企業・本番環境向け安全設計）

1. Windows標準環境のみでビルドおよび動作（究極のゼロ・ディペンデンシー）
大企業のエンタープライズ・本番環境や、インターネットから隔離された高セキュリティエリアでは、Visual Studio等の巨大な開発ランタイムを持ち込むことはおろか、外部バイナリ（EXE）の搬入すら厳しいセキュリティ審査が課されます。
本ツールは、Windowsに標準で100%同梱されている.NETのC#コンパイラ（csc.exe）を自動探索する「ビルド用バッチファイル(build.bat)」を同梱。リポジトリからソースコードだけを安全にテキスト搬入すれば、Visual Studio不要でその場で1秒で100%ネイティブなバイナリ（EXE）をビルドして実行可能です。

2. 冷徹な「Fail Fast（事前条件違反の即時停止）」による環境保護
不整合を検知したままマクロが暴走し、本番環境を破壊することを絶対に防ぎます。期待するプロンプトが指定時間内に返ってこない場合や、事前条件に1ミリでも違反（Violation）を検知した瞬間、ツールが冷徹に処理をミリ秒単位で即時強制停止（Terminate）します。安全性が確認できない限り、次の一手は絶対に実行しません。

3. タスクマネージャーの可読性とセキュリティ
バックグラウンド実行時や監査ログ監視時、不審なプロセスとして誤認されやすい「3文字の謎のEXE」ではなく、「PowerShellController.exe」という規律正しいプロセス名で明示的に動作。企業のインフラ監査や情シスのセキュリティ審査をスムーズに通過します。

4. 未登録コマンドのネイティブ透過送信
マクロ言語にない高度なPowerShellコマンドや独自の関数は、コントローラを素通りしてそのままPowerShellプロセスに透過送信されます。マクロのシンプルさと、PowerShellの強力な表現力を100%両立します。


## ディレクトリ構成

大企業のデプロイ手順書や構成管理にそのまま組み込める、美しく洗練されたディレクトリ構造を採用しています。
ビルド成果物（binやlogsフォルダ）はソースコード管理（Git）から排除され、常にクリーンな状態を保ちます。
```
PSController/ (リポジトリROOT)
  LICENSE              (MITライセンス)
  README.md            (本ドキュメント)
  build.bat            (Visual Studio不要の超軽量コンパイルスクリプト)
  PSController.sln     (Visual Studio ソリューション)
  
  src/                  (C# ソースコード)
    Core/
      Program.cs          (エントリーポイント)
      PowerShellProcess.cs (プロセス起動・出力監視)
      MacroParser.cs       (マクロ構文解析・Fail Fast制御)
      
  macros/               (運用自動化サンプルマクロ .psm)
  ps1/                  (マクロ内から呼び出される支援スクリプト .ps1)
```

## 機能概要（マクロコマンド一覧）

- PowerShellの起動と制御：安全な外部プロセス分離
- マクロファイル (.psm) の実行：上から順に逐次高速インタープリタ実行
- sendln：PowerShellへのコマンド送信
- wait / waitto：出力およびターゲットプロンプトの厳密な待機
- setvar / getvar：マクロ内での変数操作
- if / elseif / else / endif：柔軟な条件分岐
- loop / endloop / break：繰り返し処理
- goto / ラベル：指定ラベルへのジャンプ
- call / return：サブルーチン呼び出し
- include：外部マクロファイルの静的展開（共通処理の共通化）
- echo on / echo off：エコーバック（コマンド非表示）制御
- print：ログ視認性を高めるカラーメッセージ出力
- pause：オペレーター確認のための安全な一時待機
- setprompt：SSH接続先や特権環境に追従する動的プロンプト切り替え
- admin：Windows管理者権限（UAC）への自動昇格・再起動
- .logopen / .logclose：オペレーション証跡（トランスクリプト）の完全記録
- ver：バージョン情報の表示
- exit：安全なプロセス切断とPSCの正常終了


## マクロ構文の具体例

### 基本的な対話自動化とFail Fast
```
  ; サーバーの初期設定自動化サンプル
  wait >
  print cyan サーバー情報取得開始...
  sendln Get-ComputerInfo
  wait >
```
```
  ; 特権が必要な処理への移行
  admin
```

### 動的プロンプト切り替え（Linux等へのSSHリモート接続）
Windows標準環境から他セグメントのサーバーやLinuxへ接続し、プロンプトが変わる運用もシームレスに追従します。
```
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
```

## 分岐・ループ・サブルーチンの高度な制御

### goto（ジャンプ）による例外ハンドリング
```
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
```
  wait >
  call GREET
  print cyan メイン処理続行

  :GREET
  print green [Subroutine] 初期化処理を実行中...
  return
```
注意：ネストした call（サブルーチン内からのさらに別サブルーチン呼び出し）は Fail Fast 思想に基づきサポートされていません。


### include による共通部品化
```
  ; main.psm
  wait >
  include common.psm
  print green メイン処理実行

  ; common.psm
  setvar APP_NAME PowerShellController
  print cyan %APP_NAME% 共通ライブラリをロードしました
```
- 相対パスは親ファイルのディレクトリを基準とします。
- 循環参照（AがBを呼び、BがAを呼ぶ）は内部パーサーで自動検出し、実行前に厳格にブロックします。


### echo on/off による機密情報の保護
パスワード入力時や、ログを汚したくない場合は echo off でコンソールの出力を一時的にマスクできます。
```
  wait >
  echo off
  sendln $password = "Secret123"
  wait >
  echo on
  sendln Write-Output "パスワード設定完了"
  wait >
```

## 免責事項 (Disclaimer)

本ソフトウェアの使用（実行、複製、改変、配布等の一切の行為）に伴い、万が一ユーザーの本番環境、開発環境、システム、データ、ネットワーク、ハードウェア等に損害、損失、障害、データの破損または漏洩、その他いかなる不利益が生じた場合であっても、開発者および著作権者はその理由の如何を問わず、一切の責任（直接損害、間接損害、派生損害、特別損害、逸失利益を含むがこれらに限定されない）を負いません。

本ソフトウェアは「現状のまま（As Is）」提供され、明示または黙示を問わず、その特定の目的への適合性、機能性、正確性、信頼性、または瑕疵の不在について、開発者は一切の保証をいたしません。マクロの記述および実行にあたっては、ユーザー自身の責任において、事前に十分なテスト環境での検証を行ってください。


## ライセンス

本プロジェクトは MIT License のもとで公開されています。商用利用、改変、再配布が企業内でも自由に行っていただけます。詳細は LICENSE ファイルを参照してください。