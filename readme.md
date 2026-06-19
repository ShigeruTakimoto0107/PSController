# PSController

PowerShell Automation Engine for Windows Standard Environments

PSController は、PowerShell を Tera Term Macro 感覚で自動化するための軽量マクロエンジンです。
追加ソフトウェアのインストールを前提とせず、Windows 標準環境に搭載されている PowerShell を活用して、自動化・運用業務を実現します。

## なぜ PSController なのか

多くの企業環境では、セキュリティポリシーにより外部ツールの導入が制限されています。
PSController は Windows 標準搭載の PowerShell を利用することで、以下の環境制約下においても自動化を実現します。

- 定型作業の自動化 / サーバ運用の効率化 / Windows サービス監視 / ブラウザ自動化

## 特徴

- 単一 EXE 配布 / C#5 で実装 / 外部ライブラリ不要 / Windows 標準環境重視 / Fail Fast 設計
- PowerShell 透過実行 / Tera Term Macro ライクな操作感 / マクロによる自動化

## 主な機能

- PowerShell 自動化 / Windows サービス監視 / Windows サービス自動復旧 /ブラウザ自動化 (Microsoft Edge デバッグ機能連携)

# ビルド手順

Windows環境において、以下の手順でビルドを完了できます。

1. プロジェクトルートにある build.bat をダブルクリックして実行します。
2. ビルド処理が完了すると、コンソール上で以下の2点について確認メッセージが表示されます。
   - テスト用マクロの実行確認
   - プログラム（PowerShellController.exe）とマクロファイル（.pscm）の関連付け設定
3. それぞれの確認に対して Y を入力することで、環境構築および関連付けが完了します。

- [ビルドマニュアルはこちら](docs/Build/Build.md)

## 実行方法

build.bat を実行すると、bin フォルダ内に PowerShellController.exe が生成され、マクロファイル (.pscm) と関連付けられます。

マクロファイル (.pscm) をダブルクリックすることで、関連付けられた PowerShellController.exe を通じてマクロが実行されます。

## ディレクトリ構成
```text
PSController          システムROOT
├─bin               ビルド済み実行ファイル (PowerShellController.exe)
├─docs              設計書・コマンドマニュアル
├─ico               アイコンファイル
├─logs              マクロ実行時のLOG
├─macros            各種マクロ (.pscm)
│  ├─Automation      自動化用マクロ
│  └─Build           ビルド用マクロ
└─src              ソースファイル
```

## 主なコマンド

## コマンド一覧

| カテゴリ | コマンド | 説明 |
| :--- | :--- | :--- |
| **IO** | `sendln [文字列]` | PowerShellにコマンド送信 |
|  | `wait <パターン>` | パターンが出力されるまで待機 |
|  | `waitto <秒> <パターン>` | タイムアウト付きパターン待機 |
| **出力** | `print <色> <文字列>` | 色付きメッセージ出力 |
|  | `echo <on\|off>` | Printエコー制御 |
|  | `ver` | バージョン表示 |
|  | `setvar <名前> [値]` | 変数セット |
|  | `getvar <名前>` | 変数取得 |
| **フロー制御** | `if <左辺> <演算子> <右辺>` | 条件分岐 |
|  | `elseif <左辺> <演算子> <右辺>` | 条件分岐（else if） |
|  | `else` | 条件分岐（else） |
|  | `endif` | 条件分岐終了 |
|  | `loop <回数>` | 回数指定ループ開始 |
|  | `endloop` | 回数指定ループ終了 |
|  | `while <左辺> <演算子> <右辺>` | 条件ループ開始 |
|  | `endwhile` | 条件ループ終了 |
|  | `break` | ループ脱出 |
|  | `goto <ラベル>` | ジャンプ |
|  | `call <ラベル>` | サブルーチン呼び出し |
|  | `return` | サブルーチン戻り |
| **システム** | `admin` | 管理者権限で再起動 |
|  | `exec <コマンド>` | 外部プロセス実行 |
|  | `exit` | PSController終了 |
|  | `killps` | PowerShellプロセス終了 |
|  | `pause <秒>` | 指定秒数停止 |
|  | `setprompt <正規表現>` | プロンプトパターン変更 |
| **ログ** | `.logopen <ファイル>` | ログ開始 |
|  | `.logclose` | ログ終了 |

詳細はドキュメントを参照してください。
- [コマンドマニュアルはこちら](docs/guide/macros.md)

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
macros/Automation/Burauza_Start.pscm
macros/Automation/Burauza_AutomaticSearch.pscm
```
- [サンプルマニュアルはこちら](docs/sample001/001.md)
---

### Service Monitoring

Windows サービスの状態を監視し、停止した Windows サービスを自動で再起動するサンプルを収録しています。

主な内容：

- サービス状態確認
- 停止検知
- ログ出力
- サービス停止検知
- 自動再起動
- 復旧確認

サンプル：

```text
macros/Automation/Service_Monitoring.pscm
macros/Automation/Service_Outage.pscm
```
- [サンプルマニュアルはこちら](docs/sample002/002.md)
----

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


---
## Security Notice

PSController は PowerShell コマンドおよびユーザー定義マクロを実行します。
第三者が作成したマクロを実行する場合は、内容を十分確認してください。
本番環境で利用する前に、必ず検証環境で十分な動作確認を行ってください。

## Support for Development

本ツールは無償で公開しておりますが、開発継続のための支援を受け付けております。

- GitHub Sponsors: [ご自身のGitHub SponsorsプロフィールURLを記載]
- Buy Me a Coffee: [設定済みのBuy Me a CoffeeのページURLを記載]

支援によって特別な権限や機能が提供されるわけではありません。ご理解いただけますと幸いです。

## 免責事項

本ソフトウェアは現状のまま（AS IS）提供されます。
本ソフトウェアの利用により発生した損害、データ損失、業務停止、その他いかなる不利益についても、開発者は責任を負いません。利用者の判断と責任において使用してください。
