@echo off
setlocal enabledelayedexpansion

:: ============================================================
::  Windowsディレクトリを取得
:: ============================================================
set "WIN_DIR=%SystemRoot%"

:: csc.exe を探す（新しい順にソートされるようにパスを探索）
set "CSC_PATH="
for /f "delims=" %%i in ('dir /b /s /a-d "%WIN_DIR%\Microsoft.NET\Framework64\csc.exe" 2^>nul ^| sort /r') do (
    if not defined CSC_PATH (
        set "CSC_PATH=%%i"
    )
)

:: もし64bit版が見つからなければ32bit版を探す
if not defined CSC_PATH (
    for /f "delims=" %%i in ('dir /b /s /a-d "%WIN_DIR%\Microsoft.NET\Framework\csc.exe" 2^>nul ^| sort /r') do (
        if not defined CSC_PATH (
            set "CSC_PATH=%%i"
        )
    )
)

:: 結果確認（見つからなかった場合のガード）
if not defined CSC_PATH (
    echo [Error] csc.exe が見つかりませんでした。
    exit /b 1
)

set TARGET_EXE=PowerShellController.exe
set TEST_MACRO=test_all.pcm
set REG_MACRO=register_association.pcm

echo Using: %CSC_PATH%
echo [BUILD] Compiling PowerShellController Project...

if not exist "%CSC_PATH%" (
    echo Error: csc.exe not found.
    pause
    exit /b
)

:: ============================================================
::  bin / logs フォルダ作成
:: ============================================================
if not exist ".\bin" (
    echo [INFO] Creating bin directory...
    mkdir ".\bin"
)

if not exist ".\logs" (
    echo [INFO] Creating log directory...
    mkdir ".\logs"
)

:: ============================================================
::  Git 情報取得（Git が無い場合は NO_GIT）
:: ============================================================
set GIT_VER=
for /f "delims=" %%i in ('git rev-parse --short HEAD 2^>nul') do set GIT_VER=%%i
if not defined GIT_VER set GIT_VER=NO_GIT

:: ============================================================
::  日付取得（YYYY-MM-DD）
:: ============================================================
for /f "tokens=1-3 delims=/ " %%a in ("%date%") do (
    set YYYY=%%a
    set MM=%%b
    set DD=%%c
)
set BUILD_DATE=%YYYY%-%MM%-%DD%

:: ============================================================
::  VersionInfo.cs 生成（Core フォルダへ）
:: ============================================================
(
echo namespace PowerShellController {
echo       public static class VersionInfo {
echo           public const string ProgramName = "PowerShellController";
echo           public const string Version     = "1.0.0";
echo           public const string Copyright   = "(C) 2026 Kolog898";
echo           public const string BuildDate   = "%BUILD_DATE%";
echo           public const string GitVersion  = "%GIT_VER%";
echo       }
echo }
) > .\src\Core\VersionInfo.cs

:: ============================================================
::  コンパイル（フォルダ構成に対応）
:: ============================================================
"%CSC_PATH%" ^
 /out:.\bin\%TARGET_EXE% ^
 /nologo ^
 /optimize ^
 /codepage:65001 ^
 /r:System.dll ^
 /win32icon:.\ico\psc.ico ^
 .\src\Core\*.cs ^
 .\src\Parser\*.cs ^
 .\src\Registry\*.cs ^
 .\src\Commands\*.cs ^
 .\src\Flow\*.cs ^
 .\src\IO\*.cs ^
 .\src\Meta\*.cs ^
 .\src\System\*.cs

if %ERRORLEVEL% equ 0 (
    echo [SUCCESS] %TARGET_EXE% has been built.
    echo --------------------------------------------------
    
    :: 1. テスト実行の確認プロンプト（デフォルトY）
    set /p CHOICE="[QUESTION] 全コマンド確認マクロを実行しますか？ [Y/N] (Default:Y): "
    if /i "!CHOICE!"=="N" (
        echo [INFO] テスト実行をスキップします。
    ) else (
        echo [INFO] 全コマンド確認自動テストを実行します...
        cls
        .\bin\%TARGET_EXE% .\bin\%TEST_MACRO%
    )
    
    echo --------------------------------------------------
    
    :: 2. 関連付けマクロの確認プロンプト（デフォルトY）
    set /p CHOICE_REG="[QUESTION] .pcm拡張子のシステム関連付け登録を実行しますか？ [Y/N] (Default:Y): "
    if /i "!CHOICE_REG!"=="N" (
        echo [INFO] 関連付け登録をスキップします。
    ) else (
        if not exist ".\bin\%REG_MACRO%" (
            echo [ERROR] 関連付けマクロファイル（.\bin\%REG_MACRO%）が見つかりません。
            goto END_PROCESS
        )
        echo [INFO] 関連付け登録マクロを実行します...
        cls
        .\bin\%TARGET_EXE% .\bin\%REG_MACRO%
    )

) else (
    echo [FAILED] Compilation error.
)

:END_PROCESS
echo --------------------------------------------------
echo [INFO] Build and Test process finished.
pause