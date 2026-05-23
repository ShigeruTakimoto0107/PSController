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
$ws = New-Object System.Net.WebSockets.ClientWebSocket
$uri = New-Object System.Uri($wsUrl)
$ws.ConnectAsync($uri, [Threading.CancellationToken]::None).Wait()
# --- Page.enable を送信（イベント受信を有効化）---
$enableObj = [PSCustomObject]@{ id = 1; method = 'Page.enable'; params = @{} }
$enableJson = $enableObj | ConvertTo-Json -Depth 3
$enableBytes = [System.Text.Encoding]::UTF8.GetBytes($enableJson)
$enableBuffer = New-Object System.ArraySegment[byte] -ArgumentList (,$enableBytes)
$ws.SendAsync($enableBuffer, [System.Net.WebSockets.WebSocketMessageType]::Text, $true, [Threading.CancellationToken]::None).Wait()
# --- Page.enable のレスポンスを読み捨て ---
$recvBuf = New-Object System.ArraySegment[byte] -ArgumentList (,[byte[]]::new(65536))
$result = New-Object System.Text.StringBuilder
do {
    $r = $ws.ReceiveAsync($recvBuf, [Threading.CancellationToken]::None).Result
    $chunk = [System.Text.Encoding]::UTF8.GetString($recvBuf.Array, 0, $r.Count)
    $null = $result.Append($chunk)
} while (-not $r.EndOfMessage)
# --- Page.navigate を送信 ---
$cdpObj = [PSCustomObject]@{
    id     = 2
    method = 'Page.navigate'
    params = @{ url = $TargetUrl }
}
$cdpJson = $cdpObj | ConvertTo-Json -Depth 3
$bytes = [System.Text.Encoding]::UTF8.GetBytes($cdpJson)
$buffer = New-Object System.ArraySegment[byte] -ArgumentList (,$bytes)
$ws.SendAsync($buffer, [System.Net.WebSockets.WebSocketMessageType]::Text, $true, [Threading.CancellationToken]::None).Wait()
# --- Page.loadEventFired を待機（時間ベース、他のメッセージは読み飛ばす）---
$recvBuf = New-Object System.ArraySegment[byte] -ArgumentList (,[byte[]]::new(65536))
$loaded = $false
$deadline = [DateTime]::Now.AddSeconds(30)
while ([DateTime]::Now -lt $deadline) {
    $result = New-Object System.Text.StringBuilder
    do {
        $r = $ws.ReceiveAsync($recvBuf, [Threading.CancellationToken]::None).Result
        $chunk = [System.Text.Encoding]::UTF8.GetString($recvBuf.Array, 0, $r.Count)
        $null = $result.Append($chunk)
    } while (-not $r.EndOfMessage)
    $msg = $result.ToString()
    if ($msg -match '"Page\.loadEventFired"') {
        $loaded = $true
        break
    }
}
if (-not $loaded) {
    Write-Host "Warning: ページ読み込み完了を確認できませんでした。"
} else {
    Write-Host "Navigated to $TargetUrl"
}