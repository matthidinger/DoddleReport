@echo off

CreatePackages.bat

forfiles /p packages /m *.nupkg /c "cmd /c nuget push @path"