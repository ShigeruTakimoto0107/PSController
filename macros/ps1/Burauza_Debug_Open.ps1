param(
    [string]$Browser = "Edge"   # 省略時は Edge
)

# ★ 空文字・空白なら Edge 扱いにする（重要）
if ([string]::IsNullOrWhiteSpace($Browser)) {
    $Browser = "Edge"
}

# --- ブラウザごとの設定 ---
switch ($Browser.ToLower()) {
    "edge" {
        $processName = "msedge"
        $exePath = "C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe"
        $userData = "$env:TEMP\edge-debug"
    }
    "chrome" {
        $processName = "chrome"
        $exePath = "C:\Program Files\Google\Chrome\Application\chrome.exe"
        $userData = "$env:TEMP\chrome-debug"
    }
    default {
        Write-Host "Error: Browser must be 'Edge' or 'Chrome'."
        exit
    }
}

# --- 既存プロセスを強制終了 ---
if (Get-Process $processName -ErrorAction SilentlyContinue) {
    Stop-Process -Name $processName -Force -ErrorAction SilentlyContinue
    Start-Sleep -Seconds 1
}

# --- 実行ファイルチェック ---
if (!(Test-Path $exePath)) {
    Write-Host "Error: Browser executable not found: $exePath"
    exit
}

# --- デバッグモードで起動（ナビゲートなし） ---
Start-Process $exePath -ArgumentList "--remote-debugging-port=9222", "--user-data-dir=$userData"

# --- プロセスが出るまで待機 ---
while (!(Get-Process $processName -ErrorAction SilentlyContinue)) {
    Start-Sleep -Milliseconds 500
}

Write-Host "$Browser started in debug mode on port 9222."
