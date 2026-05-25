# -------------------------------------------------------------------------
# .pscm 拡張子 関連付け情報専用 削除スクリプト（単機能版）
# -------------------------------------------------------------------------
Write-Host "Starting .pscm registry cleanup..." -ForegroundColor Cyan
$targetPaths = @(
    "HKCU:\SOFTWARE\Classes\.pscm",
    "HKCU:\Software\Classes\.pscm",
    "HKCU:\Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.pscm",
    "HKCU:\SOFTWARE\Classes\PowerShellControllerMacro",
    "HKCU:\Software\Classes\PowerShellControllerMacro",
    "HKCU:\SOFTWARE\Classes\Applications\PowerShellController.exe"
)
foreach ($path in $targetPaths) {
    if (Test-Path $path) {
        Write-Host "削除中: $path" -ForegroundColor Red
        Remove-Item -Path $path -Recurse -Force -ErrorAction SilentlyContinue
    }
}
Write-Host "==========================================" -ForegroundColor Green
Write-Host ".pscm に関するすべての関連情報を削除しました。" -ForegroundColor Green
Write-Host "==========================================" -ForegroundColor Green