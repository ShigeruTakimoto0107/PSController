param(
    [string]$TargetDir = "."
)

if (-not (Test-Path $TargetDir)) {
    Write-Host "Error: フォルダが見つかりません: $TargetDir"
    exit
}

$files = Get-ChildItem -Path $TargetDir -Filter "*.cs" -Recurse
$totalLines = 0
$totalFiles = 0

foreach ($file in $files) {
    $lines = (Get-Content $file.FullName | Measure-Object -Line).Lines
    Write-Host ("[{0,6}] {1}" -f $lines, $file.FullName)
    $totalLines += $lines
    $totalFiles++
}

Write-Host "----------------------------------------"
Write-Host ("合計ファイル数 : {0}" -f $totalFiles)
Write-Host ("合計ステップ数 : {0}" -f $totalLines)
