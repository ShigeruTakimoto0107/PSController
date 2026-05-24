# ================================
# OS 情報：基本バージョン情報
# ================================
try {
    Write-Host "=== OS Version Information ==="
    $os = Get-WmiObject Win32_OperatingSystem
    Write-Output ("Caption        : {0}" -f $os.Caption)
    Write-Output ("Version        : {0}" -f $os.Version)
    Write-Output ("BuildNumber    : {0}" -f $os.BuildNumber)
    Write-Output ("OSArchitecture : {0}" -f $os.OSArchitecture)
}
catch {
    Write-Host "エラー:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Yellow
}

# ================================
# OS 情報：起動時間
# ================================
try {
    Write-Host "`n=== OS Boot Time ==="
    $os = Get-WmiObject Win32_OperatingSystem
    $boot = $os.LastBootUpTime
    $bootTime = [System.Management.ManagementDateTimeConverter]::ToDateTime($boot)
    Write-Output ("Last Boot Time : {0}" -f $bootTime)
}
catch {
    Write-Host "エラー:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Yellow
}

# ================================
# OS 情報：インストール日
# ================================
try {
    Write-Host "`n=== OS Install Date ==="
    $os = Get-WmiObject Win32_OperatingSystem
    $inst = $os.InstallDate
    $instTime = [System.Management.ManagementDateTimeConverter]::ToDateTime($inst)
    Write-Output ("Install Date : {0}" -f $instTime)
}
catch {
    Write-Host "エラー:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Yellow
}

# ================================
# OS 情報：ホスト名・ドメイン
# ================================
try {
    Write-Host "`n=== Host & Domain Info ==="
    $cs = Get-WmiObject Win32_ComputerSystem
    Write-Output ("Computer Name : {0}" -f $cs.Name)
    Write-Output ("Domain        : {0}" -f $cs.Domain)
    Write-Output ("Model         : {0}" -f $cs.Model)
    Write-Output ("Manufacturer  : {0}" -f $cs.Manufacturer)
}
catch {
    Write-Host "エラー:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Yellow
}

# ================================
# OS 情報：メモリ情報
# ================================
try {
    Write-Host "`n=== Memory Info ==="
    $os = Get-WmiObject Win32_OperatingSystem
    $total = [math]::Round($os.TotalVisibleMemorySize / 1024, 2)
    $free  = [math]::Round($os.FreePhysicalMemory / 1024, 2)
    Write-Output ("Total Memory (MB) : {0}" -f $total)
    Write-Output ("Free  Memory (MB) : {0}" -f $free)
}
catch {
    Write-Host "エラー:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Yellow
}

# ================================
# OS 情報：カーネル/システムファイル
# ================================
try {
    Write-Host "`n=== System Files ==="
    $sys32 = $env:SystemRoot + "\System32"
    Write-Output ("System32 Path : {0}" -f $sys32)
    Write-Output ("Kernel32.dll : {0}" -f (Test-Path ($sys32 + "\kernel32.dll")))
    Write-Output ("Ntdll.dll    : {0}" -f (Test-Path ($sys32 + "\ntdll.dll")))
}
catch {
    Write-Host "エラー:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Yellow
}
