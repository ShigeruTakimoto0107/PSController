# PowerShellController (PSController)

Windows のエンタープライズ運用管理を効率化する、堅牢かつ軽量な PowerShell 自動化コントローラ。

PowerShell を外部プロセスとして安全に起動・制御し、独自のマクロファイル (.psm) を用いて、これまで自動化が困難だった対話的なプロンプト操作を完全自動化します。

## 設計思想と特徴（大企業・本番環境向け安全設計）

### 1. Windows 標準環境のみで動作（ノン・イントルージョン）
Cygwin や外部ランタイムを持ち込めない統制環境でも導入可能。  
Windows 標準の .NET と PowerShell のみでスタンドアロン動作します。

### 2. Fail Fast（事前条件違反の即時停止）による環境保護
期待するプロンプトが指定時間内に返らない、事前条件に少しでも違反した瞬間に即時停止します。  
安全性が確認できない限り次の処理を実行しません。

### 3. タスクマネージャーの可読性とセキュリティ
不審な短い EXE 名ではなく PowerShellController.exe として明示的に動作し、監査やセキュリティ審査を通過しやすくします。

### 4. 未登録コマンドのネイティブ透過送信
マクロに存在しない PowerShell コマンドはそのまま透過送信され、マクロの簡潔さと PowerShell の表現力を両立します。

## ディレクトリ構成

大企業のデプロイ手順書にそのまま組み込める構造です。

```
PSController/                (リポジトリ ROOT)
  LICENSE                    (MIT ライセンス)
  README.md                  (本ドキュメント)
  PSController.sln           (Visual Studio ソリューション)

  src/                       (C# ソースコード)
    Core/
      Program.cs             (エントリーポイント)
      PowerShellProcess.cs   (プロセス起動・出力監視)
      MacroParser.cs         (マクロ構文解析・Fail Fast 制御)

  macros/                    (運用自動化サンプルマクロ .psm)
  ps1/                       (マクロから呼び出される支援スクリプト .ps1)
```

## 機能概要（マクロコマンド一覧）

- PowerShell の起動と制御（外部プロセス分離）
- マクロファイル (.psm) の逐次インタープリタ実行
- sendln：PowerShell へのコマンド送信
- wait / waitto：出力・プロンプトの厳密待機
- setvar / getvar：変数操作
- if / elseif / else / endif：条件分岐
- loop / endloop / break：繰り返し
- goto / ラベル：ジャンプ
- call / return：サブルーチン
- include：外部マクロの静的展開
- echo on / echo off：エコーバック制御
- print：カラー表示
- pause：安全な一時停止
- setprompt：動的プロンプト切り替え
- admin：UAC 自動昇格
- .logopen / .logclose：操作ログ記録
- ver：バージョン表示
- exit：安全終了

## マクロ構文の具体例

### 基本的な対話自動化と Fail Fast

```text
; サーバーの初期設定自動化サンプル
wait >
print cyan サーバー情報取得開始...
sendln Get-ComputerInfo
wait >

; 特権が必要な処理への移行
admin
```

## 動的プロンプト切り替え（Linux 等への SSH 接続）

```text
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

### goto による例外ハンドリング

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

## call / return（サブルーチン）

```text
wait >
call GREET
print cyan メイン処理続行

:GREET
print green [Subroutine] 初期化処理を実行中...
return
```

注意：ネストした call は Fail Fast 思想により非サポート。

## include による共通部品化

```text
; main.psm
wait >
include common.psm
print green メイン処理実行

; common.psm
setvar APP_NAME PowerShellController
print cyan %APP_NAME% 共通ライブラリをロードしました
```

相対パスは親ファイル基準。  
循環参照はパーサーが自動検出し実行前にブロックします。

## echo on/off による機密情報保護

```text
wait >
echo off
sendln $password = "Secret123"
wait >
echo on
sendln Write-Output "パスワード設定完了"
wait >
```

## 免責事項 (Disclaimer)

本ソフトウェアの使用に伴ういかなる損害についても、開発者および著作権者は責任を負いません。  
本ソフトウェアは「現状のまま（As Is）」提供され、特定目的への適合性や正確性について保証しません。  
マクロ実行前には必ず十分なテスト環境で検証してください。

## ライセンス

本プロジェクトは MIT License のもとで公開されています。  
商用利用・改変・再配布が自由に行えます。詳細は LICENSE を参照してください。
