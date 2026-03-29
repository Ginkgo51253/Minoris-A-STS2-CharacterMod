@echo off
setlocal enabledelayedexpansion

cd /d "%~dp0"

set "CONFIG=Debug"
if /i "%~1"=="release" set "CONFIG=Release"
if /i "%~1"=="debug" set "CONFIG=Debug"

set "PACK=0"
set "PACKDEBUG=0"

set "PROPS="
if defined GODOT_PATH set "PROPS=!PROPS! /p:GodotPath=""%GODOT_PATH%"""
if defined STS2_PATH set "PROPS=!PROPS! /p:Sts2Path=""%STS2_PATH%"""
if defined STS2_DATA_DIR set "PROPS=!PROPS! /p:Sts2DataDir=""%STS2_DATA_DIR%"""
if defined STS2_MODS_DIR set "PROPS=!PROPS! /p:ModsPath=""%STS2_MODS_DIR%"""
for %%A in (%*) do (
  if /i "%%~A"=="nocopy" set "PROPS=!PROPS! /p:ModsPath=""%CD%\__no_copy__\\"""
  if /i "%%~A"=="pack" set "PACK=1"
  if /i "%%~A"=="packdebug" set "PACKDEBUG=1"
)

if "%PACKDEBUG%"=="1" (
  set "PACK=1"
  set "CONFIG=ExportDebug"
) else if "%PACK%"=="1" (
  set "CONFIG=ExportRelease"
)

dotnet --version >nul 2>&1
if errorlevel 1 (
  echo dotnet not found. Please install .NET SDK.
  exit /b 1
)

echo Building Minoris ^(%CONFIG%^)...
dotnet build ".\Minoris.csproj" -c %CONFIG% %PROPS%
if errorlevel 1 exit /b %errorlevel%

if "%PACK%"=="1" (
  set "DLL_BUILD=%CD%\.godot\mono\temp\bin\%CONFIG%\Minoris.dll"
  set "DLL_OUT=%CD%\Minoris.dll"
  set "PCK_OUT=%CD%\Minoris.pck"

  if not exist "!DLL_BUILD!" (
    echo Built DLL not found: "!DLL_BUILD!"
    exit /b 1
  )

  copy /Y "!DLL_BUILD!" "!DLL_OUT!" >nul
  if errorlevel 1 exit /b %errorlevel%

  if defined GODOT_PATH (
    set "GODOT_EXE=%GODOT_PATH%"
  ) else (
    set "GODOT_EXE=%CD%\..\..\AgentTheSpire\AgentTheSpire-main\godot\Godot_v4.5.1-stable_mono_win64\Godot_v4.5.1-stable_mono_win64_console.exe"
  )

  if not exist "!GODOT_EXE!" (
    echo Godot executable not found. Set GODOT_PATH env var to your Godot 4 mono console exe.
    echo Tried: "!GODOT_EXE!"
    exit /b 1
  )

  echo Exporting PCK...
  "!GODOT_EXE!" --headless --path "%CD%" --export-pack "BasicExport" "!PCK_OUT!"
  if errorlevel 1 exit /b %errorlevel%
)

echo Build succeeded.
