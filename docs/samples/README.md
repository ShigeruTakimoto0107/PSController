# サンプルマクロ紹介

## その1：Google自動検索

**マクロファイル：** `macros\自動実行\Google自動検索.pscm`

Edgeをデバッグモードで起動し、Googleで「横浜 ラーメン」を検索して、最初の検索結果のURLにナビゲートするまでを完全自動化するサンプルです。

---

### マクロ全体

```text
echo on
wait >
print cyan ゾンビプロセス削除...
Killps
print cyan Edge起動中...
../macros/ps1/Burauza_Debug_Open.ps1
wait >
setvar URL https://www.google.com/
print cyan Google通常検索にナビゲート...
../macros/ps1/Burauza_Navigate.ps1 %URL%
wait >
print cyan 横浜 ラーメンで検索...
../macros/ps1/Burauza_Google_Search.ps1 "横浜 ラーメン"
wait >
print cyan 最初の１件のURLをパワーシェルに表示...
../macros/ps1/Burauza_URL_Return.ps1
getvar URL
print cyan 取得したURLにナビゲート...
../macros/ps1/Burauza_Navigate.ps1 %URL%
wait >
pause
exit
```

---

### 実行ステップ

#### STEP 1：Edgeを起動

既存のEdgeプロセスを終了し、リモートデバッグポート9222でEdgeをデバッグモード起動します。
(※実行マシンで初めてEdgeを起動する場合は、Microsoft アカウントにナビゲートされます。その場合Edgeが通常利用できるようなセットアップを行ってから、実行してください。）

```text
print cyan Edge起動中...
../macros/ps1/Burauza_Debug_Open.ps1
wait >
```

![STEP1 Edgeを起動](画像1.png)

---

#### STEP 2：Googleにナビゲート

変数 `URL` に `https://www.google.com/` をセットし、Edgeをナビゲートします。ページの読み込み完了を確認してから次のステップへ進みます。

```text
setvar URL https://www.google.com/
print cyan Google通常検索にナビゲート...
../macros/ps1/Burauza_Navigate.ps1 %URL%
wait >
```

![STEP2 Googleにナビゲート](画像2.png)

---

#### STEP 3：「横浜 ラーメン」で検索

Googleの検索ボックスに「横浜 ラーメン」を入力し、Enterキーを送信して検索を実行します。検索結果ページの読み込み完了を確認してから次のステップへ進みます。

```text
print cyan 横浜 ラーメンで検索...
../macros/ps1/Burauza_Google_Search.ps1 "横浜 ラーメン"
wait >
```

![STEP3 横浜 ラーメンで検索](画像3.png)

---

#### STEP 4：最初の1件のURLを取得

検索結果ページのDOMを解析し、最初の外部リンクURLを取得して変数 `URL` に格納します。

```text
print cyan 最初の１件のURLをパワーシェルに表示...
../macros/ps1/Burauza_URL_Return.ps1
getvar URL
```

> **注意：** このステップの直前に `pause` を入れてはいけません。Googleは検索結果表示後もDOMを動的に書き換え続けるため、時間を置くとURLが取得できなくなります。

![STEP4 最初の1件のURLを取得](画像4.png)

---

#### STEP 5：取得したURLにナビゲート

STEP4で取得したURLにEdgeをナビゲートします。ページの読み込み完了を確認してから終了します。

```text
print cyan 取得したURLにナビゲート...
../macros/ps1/Burauza_Navigate.ps1 %URL%
wait >
pause
exit
```

![STEP5 取得したURLにナビゲート](画像5.png)

---

### 使用するps1スクリプト

| ファイル | 役割 |
|---|---|
| `macros\ps1\Burauza_Debug_Open.ps1` | Edgeをデバッグモードで起動 |
| `macros\ps1\Burauza_Navigate.ps1` | 指定URLにナビゲート（読み込み完了まで待機） |
| `macros\ps1\Burauza_Google_Search.ps1` | 検索ボックスにキーワードを入力して検索実行 |
| `macros\ps1\Burauza_URL_Return.ps1` | 検索結果ページの最初の外部リンクURLを取得 |
