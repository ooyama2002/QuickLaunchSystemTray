set path=%path%;C:\Windows\Microsoft.NET\Framework\v4.0.30319
cd %~dp0

csc /target:winexe  /win32icon:icon-128.ico QuickLaunchSystemTray.cs

pause
