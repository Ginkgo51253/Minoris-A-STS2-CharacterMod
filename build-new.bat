@echo off
setlocal enabledelayedexpansion

cd /d "%~dp0"

:: ============================================================
::  CONFIGURABLE PATHS - Edit these to match your setup
:: ============================================================

set "DEFAULT_GODOT_PATH=%CD%\..\Godot_v4.5.1-stable_mono_win64\Godot_v4.5.1-stable_mono_win64_console.exe"
set "DEFAULT_STS2_PATH=%CD%\..\..\STS2"
set "DEFAULT_STS2_DATA_DIR=%DEFAULT_STS2_PATH%\data_sts2_windows_x86_64"
set "PROJECT_NAME=Minoris"

:: ============================================================
::  Build Configuration
:: ============================================================

set "BUILD_CONFIG=Debug"
set "EXPORT_CONFIG=Debug"

if /i "%~1"=="release" (
    set "BUILD_CONFIG=Release"
    set "EXPORT_CONFIG=Release"
)
if /i "%~1"=="debug" (
    set "BUILD_CONFIG=Debug"
    set "EXPORT_CONFIG=Debug"
)

set "PACK=0"
for %%A in (%*) do (
    if /i "%%~A"=="pack" set "PACK=1"
    if /i "%%~A"=="packdebug" (
        set "PACK=1"
        set "EXPORT_CONFIG=Debug"
    )
    if /i "%%~A"=="packrelease" (
        set "PACK=1"
        set "EXPORT_CONFIG=Release"
    )
)

:: ============================================================
::  Path Resolution
:: ============================================================

if defined GODOT_PATH (
    set "GODOT_EXE=%GODOT_PATH%"
) else (
    set "GODOT_EXE=%DEFAULT_GODOT_PATH%"
)

if defined STS2_PATH (
    set "STS2_PATH_VAL=%STS2_PATH%"
) else (
    set "STS2_PATH_VAL=%DEFAULT_STS2_PATH%"
)

if defined STS2_DATA_DIR (
    set "STS2_DATA=%STS2_DATA_DIR%"
) else (
    set "STS2_DATA=%STS2_PATH_VAL%\data_sts2_windows_x86_64"
)

:: ============================================================
::  MSBuild Properties
:: ============================================================

set "PROPS=/p:GodotPath="
set "PROPS=%PROPS%\"%GODOT_EXE%\""
set "PROPS=%PROPS% /p:Sts2Path="
set "PROPS=%PROPS%\"%STS2_PATH_VAL%\""
set "PROPS=%PROPS% /p:Sts2DataDir="
set "PROPS=%PROPS%\"%STS2_DATA%\""

:: ============================================================
::  Validate Environment
:: ============================================================

dotnet --version >nul 2>&1
if errorlevel 1 (
    echo [ERROR] dotnet SDK not found. Please install .NET 9.0 or later.
    exit /b 1
)

echo.
echo ============================================================
echo  Minoris Build Script
echo ============================================================
echo  Build:      %BUILD_CONFIG% (Export: %EXPORT_CONFIG%)
echo  Godot:      %GODOT_EXE%
echo  STS2:       %STS2_PATH_VAL%
echo  Data:       %STS2_DATA%
echo ============================================================

if not exist "%GODOT_EXE%" (
    echo [ERROR] Godot not found: %GODOT_EXE%
    echo Please check DEFAULT_GODOT_PATH in script header.
    exit /b 1
)

if not exist "%STS2_DATA%\sts2.dll" (
    echo [ERROR] STS2 not found: %STS2_DATA%\sts2.dll
    echo Please check DEFAULT_STS2_PATH in script header.
    exit /b 1
)

:: ============================================================
::  Build
:: ============================================================

echo.
echo Building %PROJECT_NAME% (%BUILD_CONFIG%)...
dotnet build "%PROJECT_NAME%.csproj" -c %BUILD_CONFIG% %PROPS%
if errorlevel 1 (
    echo [ERROR] Build failed.
    exit /b 1
)

echo Build succeeded.

:: ============================================================
::  Copy DLL to project root
:: ============================================================

set "DLL_BUILD=%CD%\.godot\mono\temp\bin\%BUILD_CONFIG%\%PROJECT_NAME%.dll"
set "DLL_OUT=%CD%\%PROJECT_NAME%.dll"

if not exist "%DLL_BUILD%" (
    echo [ERROR] DLL not found: %DLL_BUILD%
    exit /b 1
)

copy /Y "%DLL_BUILD%" "%DLL_OUT%" >nul
if errorlevel 1 (
    echo [ERROR] Failed to copy DLL to %DLL_OUT%
    exit /b 1
)
echo DLL copied: %DLL_OUT%

:: ============================================================
::  PCK Export
:: ============================================================

if "%PACK%"=="1" (
    echo.
    echo Exporting PCK...

    set "PCK_OUT=%CD%\%PROJECT_NAME%.pck"

    "%GODOT_EXE%" --headless --path "%CD%" --export-pack "BasicExport" "%PCK_OUT%"
    if errorlevel 1 (
        echo [ERROR] PCK export failed.
        exit /b 1
    )

    echo PCK exported: %PCK_OUT%
)

echo.
echo ============================================================
echo  Build completed!
echo ============================================================
exit /b 0