# ============================================================
# Burauza_URL_Return.ps1
# 検索結果ページの最初の外部リンクURLを取得する
#
# 【注意】このスクリプトの直前にpauseを入れてはいけません。
# Googleは検索結果表示後もDOMを動的に書き換え続けるため、
# 時間を置くとリンクが取得できなくなります。
# Burauza_Google_Search.ps1の直後に実行してください。
# ============================================================
# --- CDP タブ一覧取得 ---
$tabs = Invoke-RestMethod "http://127.0.0.1:9222/json/list"
$wsUrl = $tabs[0].webSocketDebuggerUrl
# --- WebSocket 接続 ---
$ws = New-Object System.Net.WebSockets.ClientWebSocket
$ws.ConnectAsync([Uri]$wsUrl, [Threading.CancellationToken]::None).Wait()
# --- 接続直後に少し待ってから送信 ---
Start-Sleep -Milliseconds 300
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
    id = 99
    method = "Runtime.evaluate"
    params = @{
        expression    = $js
        returnByValue = $true
    }
} | ConvertTo-Json -Depth 5
$bytes  = [System.Text.Encoding]::UTF8.GetBytes($cmdEval)
$buffer = [System.ArraySegment[byte]]::new($bytes)
$ws.SendAsync($buffer, [System.Net.WebSockets.WebSocketMessageType]::Text, $true, [Threading.CancellationToken]::None).Wait()
# --- 応答を受信（id=99 の応答が来るまで読み続ける）---
$recvBuf = New-Object System.ArraySegment[byte] -ArgumentList (,[byte[]]::new(65536))
$url = ""
$deadline = [DateTime]::Now.AddSeconds(30)
while ([DateTime]::Now -lt $deadline) {
    $result = New-Object System.Text.StringBuilder
    do {
        $r = $ws.ReceiveAsync($recvBuf, [Threading.CancellationToken]::None).Result
        $chunk = [System.Text.Encoding]::UTF8.GetString($recvBuf.Array, 0, $r.Count)
        $null = $result.Append($chunk)
    } while (-not $r.EndOfMessage)
    $msg = $result.ToString()
    if ($msg -match '"id"\s*:\s*99') {
        $json = $msg | ConvertFrom-Json -ErrorAction SilentlyContinue
        $url = $json.result.result.value
        break
    }
}
# --- URL を返す ---
$url