# ============================================================
# registry_backup.ps1
# レジストリ全体バックアップのサンプル
# 必要なキーだけに絞って改造してください
# ============================================================

$date = Get-Date -Format "yyyyMMdd_HHmmss"
$backupDir = "..\backup"

# バックアップフォルダが存在しない場合は自動作成
if (!(Test-Path $backupDir)) {
    New-Item -ItemType Directory -Path $backupDir | Out-Null
}

# 全体バックアップ（時間がかかります）
Write-Host "HKLM をバックアップ中..."
reg export HKLM "$backupDir\HKLM_$date.reg" /y

Write-Host "HKCU をバックアップ中..."
reg export HKCU "$backupDir\HKCU_$date.reg" /y

Write-Host "バックアップ完了: $backupDir"