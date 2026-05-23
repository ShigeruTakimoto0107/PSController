param([string]$Query)
if (-not $Query) {
    Write-Host "Error: 検索ワードが指定されていません。"
    exit 1
}
# --- 9222 のタブ一覧を取得 ---
$tabs = Invoke-RestMethod "http://127.0.0.1:9222/json/list"
$wsUrl = $tabs[0].webSocketDebuggerUrl
# --- WebSocket 接続 ---
$ws = New-Object System.Net.WebSockets.ClientWebSocket
$ws.ConnectAsync([Uri]$wsUrl, [Threading.CancellationToken]::None).Wait()
# --- Page.enable を送信（イベント受信を有効化）---
$enableObj = [PSCustomObject]@{ id = 1; method = 'Page.enable'; params = @{} }
$enableJson = $enableObj | ConvertTo-Json -Depth 3
$enableBytes = [System.Text.Encoding]::UTF8.GetBytes($enableJson)
$enableBuffer = New-Object System.ArraySegment[byte] -ArgumentList (,$enableBytes)
$ws.SendAsync($enableBuffer, [System.Net.WebSockets.WebSocketMessageType]::Text, $true, [Threading.CancellationToken]::None).Wait()
# --- Tab キーで検索ボックスにフォーカス ---
$cmdTab = @{
    id = 2
    method = "Input.dispatchKeyEvent"
    params = @{
        type = "keyDown"
        key = "Tab"
        windowsVirtualKeyCode = 9
        nativeVirtualKeyCode = 9
    }
} | ConvertTo-Json
$bytes = [System.Text.Encoding]::UTF8.GetBytes($cmdTab)
$buffer = New-Object System.ArraySegment[byte] -ArgumentList (,$bytes)
$ws.SendAsync($buffer, [System.Net.WebSockets.WebSocketMessageType]::Text, $true, [Threading.CancellationToken]::None).Wait()
Start-Sleep -Milliseconds 200
# --- 検索文字列を入力 ---
$cmdInput = @{
    id = 3
    method = "Input.insertText"
    params = @{ text = $Query }
} | ConvertTo-Json
$bytes = [System.Text.Encoding]::UTF8.GetBytes($cmdInput)
$buffer = New-Object System.ArraySegment[byte] -ArgumentList (,$bytes)
$ws.SendAsync($buffer, [System.Net.WebSockets.WebSocketMessageType]::Text, $true, [Threading.CancellationToken]::None).Wait()
Start-Sleep -Milliseconds 200
# --- Enter キー送信 ---
$cmdEnter = @{
    id = 4
    method = "Input.dispatchKeyEvent"
    params = @{
        type = "keyDown"
        key = "Enter"
        windowsVirtualKeyCode = 13
        nativeVirtualKeyCode = 13
    }
} | ConvertTo-Json
$bytes = [System.Text.Encoding]::UTF8.GetBytes($cmdEnter)
$buffer = New-Object System.ArraySegment[byte] -ArgumentList (,$bytes)
$ws.SendAsync($buffer, [System.Net.WebSockets.WebSocketMessageType]::Text, $true, [Threading.CancellationToken]::None).Wait()
# --- Page.loadEventFired を待機 ---
$recvBuf = New-Object System.ArraySegment[byte] -ArgumentList (,[byte[]]::new(65536))
$timeout = 30
$elapsed = 0
while ($elapsed -lt $timeout) {
    $result = New-Object System.Text.StringBuilder
    do {
        $r = $ws.ReceiveAsync($recvBuf, [Threading.CancellationToken]::None).Result
        $chunk = [System.Text.Encoding]::UTF8.GetString($recvBuf.Array, 0, $r.Count)
        $null = $result.Append($chunk)
    } while (-not $r.EndOfMessage)
    $msg = $result.ToString()
    if ($msg -match '"Page.loadEventFired"') {
        break
    }
    $elapsed++
}
if ($elapsed -ge $timeout) {
    Write-Host "Warning: ページ読み込み完了を確認できませんでした。"
} else {
    Write-Host "Google で検索しました: $Query"
}
