# reg_check.ps1
# DEBUG行を削除し、結果のみを標準出力する
if (Test-Path "HKCU:\Software\Classes\.pscm") {
    Write-Output "Registered"
} else {
    Write-Output "NotRegistered"
}