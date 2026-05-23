param(
    [string]$TargetUrl
)

if (-not $TargetUrl) {
    Write-Host "Error: URL が指定されていません。"
    exit 1
}

# --- CDP タブ一覧取得 ---
$debugUrl = "http://127.0.0.1:9222/json/list"

try {
    $tabs = Invoke-RestMethod -Uri $debugUrl -ErrorAction Stop
} catch {
    Write-Host "Error: CDP に接続できません。"
    exit 1
}

$wsUrl = $tabs[0].webSocketDebuggerUrl
if (-not $wsUrl) {
    Write-Host "Error: WebSocket Debugger URL が取得できませんでした。"
    exit 1
}

# --- ★ Add-Type は使わない！ ---
$ws = New-Object System.Net.WebSockets.ClientWebSocket

$uri = New-Object System.Uri($wsUrl)
$ws.ConnectAsync($uri, [Threading.CancellationToken]::None).Wait()

# --- Page.navigate を送信 ---
$cdpObj = [PSCustomObject]@{
    id     = 1
    method = 'Page.navigate'
    params = @{ url = $TargetUrl }
}

$cdpJson = $cdpObj | ConvertTo-Json -Depth 3

$bytes = [System.Text.Encoding]::UTF8.GetBytes($cdpJson)
$buffer = [System.ArraySegment[byte]]::new($bytes)

$ws.SendAsync(
    $buffer,
    [System.Net.WebSockets.WebSocketMessageType]::Text,
    $true,
    [Threading.CancellationToken]::None
).Wait()

Write-Host "Navigated to $TargetUrl"
