try {
    Write-Host "=== 現在の実行権限 ==="

    $identity = [Security.Principal.WindowsIdentity]::GetCurrent()
    $principal = New-Object Security.Principal.WindowsPrincipal($identity)

    $isAdmin = $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)

    if ($isAdmin) {
        Write-Output "実行権限 : 管理者 (Administrator)"
    }
    else {
        Write-Output "実行権限 : 一般ユーザー (Standard User)"
    }

    Write-Output ("ユーザー名 : {0}" -f $identity.Name)
}
catch {
    Write-Host "エラーが発生しました:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Yellow
}
