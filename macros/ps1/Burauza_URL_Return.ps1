# --- CDP タブ一覧取得 ---
$tabs = Invoke-RestMethod "http://127.0.0.1:9222/json/list"
$wsUrl = $tabs[0].webSocketDebuggerUrl

# --- WebSocket 接続 ---
$ws = New-Object System.Net.WebSockets.ClientWebSocket
$ws.ConnectAsync([Uri]$wsUrl, [Threading.CancellationToken]::None).Wait()

# --- JS：最初の外部リンクを拾う ---
$js = @'
(() => {
  const anchors = Array.from(document.querySelectorAll('a[href^="http"]'));

  const isVisible = el => {
    const rect = el.getBoundingClientRect();
    const style = window.getComputedStyle(el);
    if (style.display === 'none' || style.visibility === 'hidden' || style.opacity === '0') return false;
    if (rect.width === 0 || rect.height === 0) return false;
    return true;
  };

  const isExternal = href => {
    if (!href) return false;

    // Google 全ドメイン除外
    if (href.match(/^https?:\/\/([^\/]+\.)?google\./)) return false;

    const bad = [
      'gstatic.com',
      'maps.google.',
      'accounts.google.',
      'policies.google.',
      'support.google.'
    ];
    if (bad.some(x => href.includes(x))) return false;

    return true;
  };

  const target = anchors.find(a => isVisible(a) && isExternal(a.href));
  return target ? target.href : "";
})();
'@

# --- Runtime.evaluate ---
$cmdEval = @{
    id = 1
    method = "Runtime.evaluate"
    params = @{
        expression    = $js
        returnByValue = $true
    }
} | ConvertTo-Json -Depth 5

$bytes  = [System.Text.Encoding]::UTF8.GetBytes($cmdEval)
$buffer = [System.ArraySegment[byte]]::new($bytes)
$ws.SendAsync($buffer, [System.Net.WebSockets.WebSocketMessageType]::Text, $true, [Threading.CancellationToken]::None).Wait()

# --- 応答を受信 ---
$recvBuf = New-Object System.ArraySegment[byte] -ArgumentList (,[byte[]]::new(65536))
$result  = New-Object System.Text.StringBuilder
do {
    $r = $ws.ReceiveAsync($recvBuf, [Threading.CancellationToken]::None).Result
    $chunk = [System.Text.Encoding]::UTF8.GetString($recvBuf.Array, 0, $r.Count)
    $null = $result.Append($chunk)
} while (-not $r.EndOfMessage)

# --- URL 抽出 ---
$json = $result.ToString() | ConvertFrom-Json -ErrorAction SilentlyContinue
$url  = $json.result.result.value

# --- URL を返す（画面にはこれだけ出る） ---
$url
