# reg_check.ps1
if (Test-Path "HKCU:\SOFTWARE\Classes\.pscm") {
    Write-Output "Registered"
} else {
    Write-Output "NotRegistered"
}