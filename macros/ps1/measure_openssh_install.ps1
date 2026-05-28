$start = Get-Date
Add-WindowsCapability -Online -Name OpenSSH.Server~~~~0.0.1.0
$end = Get-Date
$elapsed = ($end - $start).TotalSeconds
Write-Output "Š—vŠÔ: $elapsed •b"