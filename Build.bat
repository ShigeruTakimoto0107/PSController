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
::  VersionInfo.cs 生成
:: ============================================================
(
echo namespace PowerShellController {
echo     public static class VersionInfo {
echo         public const string ProgramName = "PowerShellController";
echo         public const string Version     = "1.0.0";
echo         public const string Copyright   = "(C) 2026 Kolog898";
echo         public const string BuildDate   = "%BUILD_DATE%";
echo         public const string GitVersion  = "%GIT_VER%";
echo     }
echo }
) > .\src\VersionInfo.cs

:: ============================================================
::  コンパイル
:: ============================================================
"%CSC_PATH%" ^
 /out:.\bin\%TARGET_EXE% ^
 /nologo ^
 /optimize ^
 /codepage:65001 ^
 /r:System.dll ^
 /win32icon:.\ico\psc.ico ^
 .\src\*.cs

if %ERRORLEVEL% equ 0 (
    echo [SUCCESS] %TARGET_EXE% has been built.
    echo [INFO] Running %TARGET_EXE%...
    .\bin\%TARGET_EXE%
) else (
    echo [FAILED] Compilation error.
)

pause
