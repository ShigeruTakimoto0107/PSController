param(
    [string]$Browser = "Edge"
)
if ([string]::IsNullOrWhiteSpace($Browser)) {
    $Browser = "Edge"
}
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
# --- デバッグモードで起動 ---
Start-Process $exePath -ArgumentList "--remote-debugging-port=9222", "--user-data-dir=$userData"
# --- デバッグポート9222が応答するまで待機 ---
$timeout = 30
$elapsed = 0
while ($elapsed -lt $timeout) {
    try {
        $response = Invoke-RestMethod "http://127.0.0.1:9222/json/version" -ErrorAction Stop
        break
    } catch {
        Start-Sleep -Milliseconds 500
        $elapsed++
    }
}
if ($elapsed -ge $timeout) {
    Write-Host "Error: Browser debug port did not respond within $timeout seconds."
    exit
}
Write-Host "$Browser started in debug mode on port 9222."