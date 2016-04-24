@echo off


call CreatePackages.bat

pause

forfiles /p packages /m *.nupkg /c "cmd /c nuget push @path"
