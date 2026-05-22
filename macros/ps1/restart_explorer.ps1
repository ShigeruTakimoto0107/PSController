# -------------------------------------------------------------------------
# 4. エクスプローラー再起動（環境反映用）
# -------------------------------------------------------------------------
Write-Host "Restarting Explorer to apply changes instantly..." -ForegroundColor Cyan
try {
    Stop-Process -Name "explorer" -Force
    Start-Sleep -Seconds 1
    Start-Process "explorer.exe"
    Write-Host "==========================================" -ForegroundColor Green
    Write-Host "完了しました！.pscmのプロパティを確認してください。" -ForegroundColor Green
    Write-Host "==========================================" -ForegroundColor Green
} catch {
    Write-Host "Warning: エクスプローラーの再起動中に軽微な警告が発生しました。" -ForegroundColor Yellow
}