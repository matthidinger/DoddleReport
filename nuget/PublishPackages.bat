@echo off

rem CreatePackages.bat

forfiles /p packages /m *.nupkg /c "cmd /c nuget push @path"