try {
    Write-Host "=== CD/DVD トレイをオープンします（Win32 API 方式） ==="

    $signature = @"
[DllImport("winmm.dll", EntryPoint="mciSendStringA", CharSet=CharSet.Ansi)]
public static extern int mciSendString(string command, System.Text.StringBuilder buffer, int bufferSize, IntPtr hwndCallback);
"@

    Add-Type -MemberDefinition $signature -Name "MciApi" -Namespace Win32

    # ドライブレター一覧を取得
    $drives = Get-WmiObject Win32_CDROMDrive

    foreach ($d in $drives) {
        Write-Host ("対象ドライブ: {0}" -f $d.Drive)

        $cmd = "open {0} type CDAudio alias drive{1}" -f $d.Drive, $d.Drive.Substring(0,1)
        [Win32.MciApi]::mciSendString($cmd, $null, 0, [IntPtr]::Zero) | Out-Null

        $cmd = "set drive{0} door open" -f $d.Drive.Substring(0,1)
        [Win32.MciApi]::mciSendString($cmd, $null, 0, [IntPtr]::Zero) | Out-Null

        Write-Host "→ トレイをオープンしました。" -ForegroundColor Green
    }
}
catch {
    Write-Host "エラーが発生しました:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Yellow
}
