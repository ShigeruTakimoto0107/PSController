# PSController

**PowerShell Automation Engine for Windows Standard Environments**

PSController は、PowerShell を Tera Term Macro 感覚で自動化するための軽量マクロエンジンです。

追加ソフトウェアのインストールを前提とせず、Windows 標準環境に搭載されている PowerShell を活用して、自動化・運用業務・ブラウザ操作を実現します。

---

## なぜ PSController なのか

多くの企業環境では、セキュリティポリシーや運用ルールにより、

- Python の導入ができない
- AutoIt の導入ができない
- Selenium の導入ができない
- フリーソフトウェアの持ち込みができない

といった制約があります。

しかし、PowerShell は Windows に標準搭載されています。

PSController は PowerShell を利用して、

- 定型作業の自動化
- サーバ運用の効率化
- Windows サービス監視
- ブラウザ自動化

を実現するために開発されました。

---

## 特徴

- 単一 EXE 配布
- C#5 で実装
- 外部ライブラリ不要
- Windows 標準環境重視
- Fail Fast 設計
- PowerShell 透過実行
- Tera Term Macro ライクな操作感
- マクロによる自動化

---

## 主な機能

### PowerShell 自動化

PowerShell の対話実行をマクロから制御できます。

### Windows サービス監視

サービス状態を定期的に確認し、停止を検知できます。

### Windows サービス自動復旧

停止したサービスを自動的に再起動できます。

### ブラウザ自動化

Microsoft Edge のデバッグ機能を利用し、

- URL ナビゲート
- 検索実行
- DOM 情報取得

などを自動化できます。

---

## 主なコマンド

## コマンド一覧

| カテゴリ | コマンド | 説明 |
| :--- | :--- | :--- |
| **IO** | `sendln [文字列]` | PowerShellにコマンド送信 |
| **IO** | `wait <パターン>` | パターンが出力されるまで待機 |
| **IO** | `waitto <秒> <パターン>` | タイムアウト付きパターン待機 |
| **出力** | `print <色> <文字列>` | 色付きメッセージ出力 |
| **出力** | `echo <on\|off>` | Printエコー制御 |
| **出力** | `ver` | バージョン表示 |
| **変数** | `setvar <名前> [値]` | 変数セット |
| **変数** | `getvar <名前>` | 変数取得 |
| **フロー制御** | `if <左辺> <演算子> <右辺>` | 条件分岐 |
| **フロー制御** | `elseif <左辺> <演算子> <右辺>` | 条件分岐（else if） |
| **フロー制御** | `else` | 条件分岐（else） |
| **フロー制御** | `endif` | 条件分岐終了 |
| **フロー制御** | `loop <回数>` | 回数指定ループ開始 |
| **フロー制御** | `endloop` | 回数指定ループ終了 |
| **フロー制御** | `while <左辺> <演算子> <右辺>` | 条件ループ開始 |
| **フロー制御** | `endwhile` | 条件ループ終了 |
| **フロー制御** | `break` | ループ脱出 |
| **フロー制御** | `goto <ラベル>` | ジャンプ |
| **フロー制御** | `call <ラベル>` | サブルーチン呼び出し |
| **フロー制御** | `return` | サブルーチン戻り |
| **システム** | `admin` | 管理者権限で再起動 |
| **システム** | `exec <コマンド>` | 外部プロセス実行 |
| **システム** | `exit` | PSController終了 |
| **システム** | `killps` | PowerShellプロセス終了 |
| **システム** | `pause <秒>` | 指定秒数停止 |
| **システム** | `setprompt <正規表現>` | プロンプトパターン変更 |
| **ログ** | `.logopen <ファイル>` | ログ開始 |
| **ログ** | `.logclose` | ログ終了 |

詳細はドキュメントを参照してください。

---

## サンプル

### Hello World

```text
wait >
print Hello World.
```

---

### Browser Automation

Microsoft Edge を自動操作するサンプルを収録しています。

主な内容：

- Edge 起動
- Google 検索
- URL 取得
- ページ遷移

サンプル：

```text
macros/自動実行/自動検索.pscm
```

---

### Service Monitoring

Windows サービスの状態を監視するサンプルを収録しています。

主な内容：

- サービス状態確認
- 停止検知
- ログ出力

---

### Service Auto Recovery

停止した Windows サービスを自動で再起動するサンプルを収録しています。

主な内容：

- サービス停止検知
- 自動再起動
- 復旧確認

---

## ビルド方法

ビルド環境を準備した後、以下を実行してください。

```cmd
build.bat
```

ビルド完了後、テストマクロが自動実行されます。

詳細は Build ドキュメントを参照してください。

---

## 実行方法

```cmd
PSController.exe sample.pscm
```

---

## ディレクトリ構成

```text

PSController          システムROOT
├─bin               ビルドするとこのフォルダにPowerShellController.exeが出来上がります
├─docs              設計書・コマンドマニュアル
├─ico               アイコンファイル
├─logs              マクロ実行時のLOG
├─macros            各種マクロ
│  ├─Automation      自動化用マクロ
│  │  └─include       インクルード用マクロ
│  ├─Build           ビルド時のマクロ
│  │  └─Include       インクルード用マクロ
│  └─ps1          　呼び出し用パワーシェルスクリプト
└─src              ソースファイル

```
---

## ドキュメント

### コマンドリファレンス

- [コマンドマニュアルはこちら](docs/guide/macros.md)

詳細は docs フォルダを参照してください。

---

## 想定用途

- PowerShell 自動化
- サーバ運用自動化
- Windows サービス監視
- Windows サービス自動復旧
- ログ取得
- 定型業務自動化
- ブラウザ自動化
- 社内 Web システム操作

---

## Security Notice

PSController は PowerShell コマンドおよびユーザー定義マクロを実行します。

第三者が作成したマクロを実行する場合は、内容を十分確認してください。

本番環境で利用する前に、必ず検証環境で十分な動作確認を行ってください。

---

## 免責事項

本ソフトウェアは現状のまま（AS IS）提供されます。

本ソフトウェアの利用により発生した損害、データ損失、業務停止、その他いかなる不利益についても、開発者および著作権者は責任を負いません。

PowerShell コマンドおよびマクロの実行は利用者自身の責任で行ってください。

---

## ライセンス

本プロジェクトは MIT License のもとで公開されています。

MIT License の条件に従い、商用利用、改変、再配布が可能です。

詳細は LICENSE ファイルを参照してください。

---

## サポート

### 不具合報告・要望

GitHub Issues をご利用ください。

### お問い合わせ

pscontroller.project@gmail.com

---

## 開発方針

PSController は以下の方針を重視しています。

- Windows 標準環境を活用する
- 外部依存を最小限にする
- 単一 EXE で配布する
- C#5 互換性を維持する
- 保守性を重視する
- Fail Fast 思想を維持する

---

PSController の目標は、

**「PowerShell を Tera Term Macro 感覚で自動化できること」**

そして、

**「制約の厳しい企業環境でも利用できる実用的な自動化ツールであること」**

です。