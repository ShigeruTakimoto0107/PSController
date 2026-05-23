param([string]$Query)

if (-not $Query) {
    Write-Host "Error: 検索ワードが指定されていません。"
    exit 1
}

# --- 9222 のタブ一覧を取得 ---
$tabs = Invoke-RestMethod "http://127.0.0.1:9222/json/list"
$wsUrl = $tabs[0].webSocketDebuggerUrl

# --- WebSocket 接続（Add-Type 不要） ---
$ws = New-Object System.Net.WebSockets.ClientWebSocket
$ws.ConnectAsync([Uri]$wsUrl, [Threading.CancellationToken]::None).Wait()

# --- ★ Tab キーを送って検索ボックスにフォーカスを当てる ---
$cmdTab = @{
    id = 1
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
    id = 2
    method = "Input.insertText"
    params = @{ text = $Query }
} | ConvertTo-Json

$bytes = [System.Text.Encoding]::UTF8.GetBytes($cmdInput)
$buffer = New-Object System.ArraySegment[byte] -ArgumentList (,$bytes)
$ws.SendAsync($buffer, [System.Net.WebSockets.WebSocketMessageType]::Text, $true, [Threading.CancellationToken]::None).Wait()

Start-Sleep -Milliseconds 200

# --- Enter キー送信 ---
$cmdEnter = @{
    id = 3
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

Write-Host "Google で検索しました: $Query"
