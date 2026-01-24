@echo off
cd /d "%~dp0"
set "NPM=C:\Program Files\nodejs\npm.cmd"
if not exist "%NPM%" set "NPM=npm"
echo Installing dependencies...
call "%NPM%" install
if errorlevel 1 (echo npm install failed. & pause & exit /b 1)
echo.
echo Starting dev server...
call "%NPM%" run dev
pause
