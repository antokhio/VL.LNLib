@echo off
setlocal

:: Define directories
set SOURCE_DIR=lnlib
set BUILD_DIR=bin
set TARGET_DIR=src\VL.LNLib\runtimes\win-64\native

:: Create build directory if it doesn't exist
if not exist %BUILD_DIR% mkdir %BUILD_DIR%

echo Configuring project with .NET API enabled...
cmake -S %SOURCE_DIR% -B %BUILD_DIR% -DENABLE_DOTNET_API=ON

if %ERRORLEVEL% NEQ 0 (
    echo CMake configuration failed.
    exit /b %ERRORLEVEL%
)

echo Building project (Release)...
cmake --build %BUILD_DIR% --config Release

if %ERRORLEVEL% NEQ 0 (
    echo Build failed.
    exit /b %ERRORLEVEL%
)

echo Copying build artifacts to %TARGET_DIR%...
if not exist "%TARGET_DIR%" mkdir "%TARGET_DIR%"
xcopy "%BUILD_DIR%\Release\*.*" "%TARGET_DIR%\" /Y /E /I

if %ERRORLEVEL% NEQ 0 (
    echo Copy failed.
    exit /b %ERRORLEVEL%
)

echo Build and copy completed successfully.
endlocal