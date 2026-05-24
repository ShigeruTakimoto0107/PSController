try {
    Write-Host "=== 現在起動しているサービス一覧 ==="

    $services = Get-Service

    foreach ($svc in $services) {
        Write-Output ("{0,-40} {1,-10} {2}" -f $svc.DisplayName, $svc.Status, $svc.ServiceName)
    }
}
catch {
    Write-Host "エラーが発生しました:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Yellow
}
