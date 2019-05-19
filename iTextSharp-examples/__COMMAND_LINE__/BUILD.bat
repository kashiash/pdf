REM change to directory where batch file located so relative path works
cd /d %~dp0

REM delete existing source code files
rmdir /s /q  ..\app_code\iTextInAction2Ed

REM create subdirectories
mkdir ..\app_code\iTextInAction2Ed

REM copy .dlls
xcopy /y ..\..\..\bin\System.Data.SQLite.dll
xcopy /y ..\..\..\bin\Ionic.Zip.Reduced.dll
xcopy /y ..\..\..\bin\itextsharp.dll
xcopy /y ..\..\..\bin\itextsharp.xtra.dll
xcopy /y ..\..\..\bin\iTextAsian.dll
xcopy /y /s ..\..\..\app_code\CSCode\iTextInAction2Ed ..\app_code\iTextInAction2Ed

REM create .dll for .exe file
REM '/platform:x86' option is required, or .exe will die on a 64-bit system
%WINDIR%\Microsoft.NET\Framework\v3.5\csc.exe /target:library /r:itextsharp.dll /r:itextsharp.xtra.dll /r:iTextAsian.dll /r:System.Data.SQLite.dll /r:Ionic.Zip.Reduced.dll /out:iTextInAction2Ed.dll /platform:x86 /recurse:..\app_code\iTextInAction2Ed\*.cs

REM create .dll for .exe file
%WINDIR%\Microsoft.NET\Framework\v3.5\csc.exe /r:iTextInAction2Ed.dll /platform:x86 CommandLine.cs
