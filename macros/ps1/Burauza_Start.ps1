$processName = "msedge"
$exePath = "C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe"
$userData = "C:\PSController\macros\Automation\EdgeProfile"

if (!(Test-Path $exePath)) {
    Write-Host "Error: Edge‚ªŒ©‚Â‚©‚è‚Ü‚¹‚ñ: $exePath"
    exit
}

if (Get-Process $processName -ErrorAction SilentlyContinue) {
    Stop-Process -Name $processName -Force -ErrorAction SilentlyContinue
    Start-Sleep -Seconds 1
}

Start-Process $exePath -ArgumentList "--remote-debugging-port=9222", "--user-data-dir=$userData"
Write-Host "Edge started."