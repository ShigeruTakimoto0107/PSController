# -------------------------------------------------------------------------
# .pscm 拡張子 関連付け情報専用 削除スクリプト（単機能版）
# -------------------------------------------------------------------------
Write-Host "Starting .pscm registry cleanup..." -ForegroundColor Cyan

# 現行リリース版(.pcm / .psm)には一切触れず、新拡張子 .pscm の関連キーのみをループ削除
$targetPaths = @(
    "HKLM:\SOFTWARE\Classes\.pscm",
    "HKCU:\Software\Classes\.pscm",
    "HKCU:\Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.pscm",
    "HKLM:\SOFTWARE\Classes\PowerShellControllerMacro",
    "HKCU:\Software\Classes\PowerShellControllerMacro",
    "HKLM:\SOFTWARE\Classes\Applications\PowerShellController.exe"
)

foreach ($path in $targetPaths) {
    if (Test-Path $path) {
        Write-Host "削除中: $path" -ForegroundColor Red
        # -Recurse を指定することでキー配下もすべて消去
        Remove-Item -Path $path -Recurse -Force -ErrorAction SilentlyContinue
    }
}

Write-Host "==========================================" -ForegroundColor Green
Write-Host ".pscm に関するすべての関連情報を削除しました。" -ForegroundColor Green
Write-Host "==========================================" -ForegroundColor Green