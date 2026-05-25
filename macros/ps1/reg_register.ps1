Write-Host "Start registration" -ForegroundColor Cyan
$exePath = [Environment]::GetEnvironmentVariable("PSC_EXE_PATH", "User")
if (-not $exePath) {
    $exePath = "$PSScriptRoot\..\bin\PowerShellController.exe"
    if (-not (Test-Path $exePath)) {
        Write-Host "Error: Path not found" -ForegroundColor Red
        return
    }
}
try {
    $appKey = "HKCU:\SOFTWARE\Classes\Applications\PowerShellController.exe"
    New-Item -Path "$appKey\shell\open\command" -Force | Out-Null
    Set-ItemProperty -Path $appKey -Name "(Default)" -Value "PowerShellController" -Force
    Set-ItemProperty -Path "$appKey\shell\open\command" -Name "(Default)" -Value """$exePath"" ""%1""" -Force
    $macroKey = "HKCU:\SOFTWARE\Classes\PowerShellControllerMacro"
    New-Item -Path "$macroKey\shell\open\command" -Force | Out-Null
    Set-ItemProperty -Path $macroKey -Name "(Default)" -Value "PowerShellController Macro" -Force
    if (Test-Path "$PSScriptRoot\..\ico\PSC.ico") {
        New-Item -Path "$macroKey\DefaultIcon" -Force | Out-Null
        Set-ItemProperty -Path "$macroKey\DefaultIcon" -Name "(Default)" -Value "$PSScriptRoot\..\ico\PSC.ico" -Force
    }
    Set-ItemProperty -Path "$macroKey\shell\open\command" -Name "(Default)" -Value """$exePath"" ""%1""" -Force
    $ext = ".pscm"
    New-Item -Path "HKCU:\SOFTWARE\Classes\$ext" -Force | Out-Null
    Set-ItemProperty -Path "HKCU:\SOFTWARE\Classes\$ext" -Name "(Default)" -Value "PowerShellControllerMacro" -Force
    New-Item -Path "HKCU:\SOFTWARE\Classes\$ext\OpenWithProgids" -Force | Out-Null
    Set-ItemProperty -Path "HKCU:\SOFTWARE\Classes\$ext\OpenWithProgids" -Name "PowerShellControllerMacro" -Value "" -Force
    Write-Host "Registration completed" -ForegroundColor Green
}
catch {
    Write-Host "Registration error" -ForegroundColor Red
}