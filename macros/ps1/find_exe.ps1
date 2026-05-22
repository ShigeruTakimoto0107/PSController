# カレントディレクトリ(bin)から見た相対パスを指定
$target = "..\bin\PowerShellController.exe"

# 相対パスを絶対パスに変換して存在確認
if (Test-Path $target) {
    # .FullName でフルパスを取得し、それを標準出力に出す
    (Get-Item $target).FullName
} else {
    # エラー時は空文字ではなく明示的にエラーメッセージを出す
    Write-Error "File not found: $target"
}