try {
    Write-Host "=== CD/DVD トレイを閉じます（Win32 API 方式） ==="

    $signature = @"
[DllImport("winmm.dll", EntryPoint="mciSendStringA", CharSet=CharSet.Ansi)]
public static extern int mciSendString(string command, System.Text.StringBuilder buffer, int bufferSize, IntPtr hwndCallback);
"@

    Add-Type -MemberDefinition $signature -Name "MciApi" -Namespace Win32

    # ドライブレター一覧を取得
    $drives = Get-WmiObject Win32_CDROMDrive

    foreach ($d in $drives) {
        Write-Host ("対象ドライブ: {0}" -f $d.Drive)

        $alias = "drive{0}" -f $d.Drive.Substring(0,1)

        # ドライブを開く（MCI の準備）
        $cmd = "open {0} type CDAudio alias {1}" -f $d.Drive, $alias
        [Win32.MciApi]::mciSendString($cmd, $null, 0, [IntPtr]::Zero) | Out-Null

        # トレイを閉じる
        $cmd = "set {0} door closed" -f $alias
        [Win32.MciApi]::mciSendString($cmd, $null, 0, [IntPtr]::Zero) | Out-Null

        Write-Host "→ トレイを閉じました。" -ForegroundColor Green
    }
}
catch {
    Write-Host "エラーが発生しました:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Yellow
}
