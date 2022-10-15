@ECHO OFF

REM The following directory is for .NET 4.0
set filepath=%~dp0
set DOTNETFX4 = %SystemRoot%\Microsoft.NET\Framework\v4.0.30319
cd /d %DOTNETFX4%

echo Installing Scheduler windows service...
echo ---------------------------------------------------
InstallUtil /i %filepath%\Emaratech.Services.Scheduler.WindowsService.exe
echo ---------------------------------------------------
echo Done.
cd /d %filepath%