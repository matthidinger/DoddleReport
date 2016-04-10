@echo off


@rd /S /Q packages
@mkdir packages

nuget restore ..\src\DoddleReport.sln
nuget pack ..\src\DoddleReport\DoddleReport.csproj -prop Configuration=Release -Build -Symbols -OutputDirectory .\packages
nuget pack ..\src\DoddleReport.AbcPdf\DoddleReport.AbcPdf.csproj -prop Configuration=Release -Build -Symbols -OutputDirectory .\packages
nuget pack ..\src\DoddleReport.iTextSharp\DoddleReport.iTextSharp.csproj -prop Configuration=Release -Build -Symbols -OutputDirectory .\packages
nuget pack ..\src\DoddleReport.OpenXml\DoddleReport.OpenXml.csproj -prop Configuration=Release -Build -Symbols -OutputDirectory .\packages
nuget pack ..\src\DoddleReport.Web\DoddleReport.Web.csproj -prop Configuration=Release -Build -Symbols -OutputDirectory .\packages

nuget pack ..\src\DoddleReport.Sample.Web\DoddleReport.Sample.Mvc.nuspec -OutputDirectory .\packages

pause