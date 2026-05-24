try {
    Write-Host "=== HDD / SSD 情報 ==="

    $disks = Get-WmiObject Win32_DiskDrive

    foreach ($d in $disks) {
        $sizeGB = [math]::Round($d.Size / 1GB, 2)
        Write-Output ("Model : {0}" -f $d.Model)
        Write-Output ("Size  : {0} GB" -f $sizeGB)
        Write-Output ("Interface : {0}" -f $d.InterfaceType)
        Write-Output ("----------------------------------------")
    }
}
catch {
    Write-Host "エラー (DiskDrive):" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Yellow
}

try {
    Write-Host "`n=== 論理ドライブ（マウント）情報 ==="

    $vols = Get-WmiObject Win32_LogicalDisk

    foreach ($v in $vols) {
        $sizeGB = ""
        $freeGB = ""

        if ($v.Size -ne $null) {
            $sizeGB = [math]::Round($v.Size / 1GB, 2)
        }
        if ($v.FreeSpace -ne $null) {
            $freeGB = [math]::Round($v.FreeSpace / 1GB, 2)
        }

        Write-Output ("Drive       : {0}" -f $v.DeviceID)
        Write-Output ("Type        : {0}" -f $v.DriveType)
        Write-Output ("FileSystem  : {0}" -f $v.FileSystem)
        Write-Output ("Size (GB)   : {0}" -f $sizeGB)
        Write-Output ("Free (GB)   : {0}" -f $freeGB)
        Write-Output ("----------------------------------------")
    }
}
catch {
    Write-Host "エラー (LogicalDisk):" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Yellow
}

try {
    Write-Host "`n=== USB デバイス情報 ==="

    $usb = Get-WmiObject Win32_USBControllerDevice

    $count = 0
    foreach ($u in $usb) {
        $count++
    }

    Write-Output ("USB デバイス数 : {0}" -f $count)
    Write-Output ("----------------------------------------")
}
catch {
    Write-Host "エラー (USB):" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Yellow
}

try {
    Write-Host "`n=== モニタ情報 ==="

    # モニタ名（PnP モニタ）
    $mons = Get-WmiObject Win32_DesktopMonitor

    foreach ($m in $mons) {
        Write-Output ("Name        : {0}" -f $m.Name)
        Write-Output ("DeviceID    : {0}" -f $m.DeviceID)
        Write-Output ("ScreenWidth : {0}" -f $m.ScreenWidth)
        Write-Output ("ScreenHeight: {0}" -f $m.ScreenHeight)
        Write-Output ("----------------------------------------")
    }
}
catch {
    Write-Host "エラー (Monitor):" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Yellow
}

try {
    Write-Host "`n=== マウス（ポインティングデバイス）情報 ==="

    $mice = Get-WmiObject Win32_PointingDevice

    foreach ($m in $mice) {
        Write-Output ("Name        : {0}" -f $m.Name)
        Write-Output ("DeviceID    : {0}" -f $m.DeviceID)
        Write-Output ("Interface   : {0}" -f $m.Interface)
        Write-Output ("Manufacturer: {0}" -f $m.Manufacturer)
        Write-Output ("----------------------------------------")
    }
}
catch {
    Write-Host "エラー (Mouse):" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Yellow
}

try {
    Write-Host "`n=== キーボード情報 ==="

    $keys = Get-WmiObject Win32_Keyboard

    foreach ($k in $keys) {
        Write-Output ("Name        : {0}" -f $k.Name)
        Write-Output ("DeviceID    : {0}" -f $k.DeviceID)
        Write-Output ("Layout      : {0}" -f $k.Layout)
        Write-Output ("Description : {0}" -f $k.Description)
        Write-Output ("----------------------------------------")
    }
}
catch {
    Write-Host "エラー (Keyboard):" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Yellow
}
