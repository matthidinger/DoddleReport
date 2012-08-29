set msbuild=%WINDIR%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe

%msbuild% CreatePackages.build /t:Publish
pause