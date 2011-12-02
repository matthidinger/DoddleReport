nuget pack ..\src\DoddleReport\DoddleReport.csproj -Build -Prop Configuration=Release
nuget pack ..\src\DoddleReport.Web\DoddleReport.Web.csproj -Build -Prop Configuration=Release
nuget pack ..\src\DoddleReport.AbcPdf\DoddleReport.AbcPdf.csproj -Build -Prop Configuration=Release
nuget pack ..\src\DoddleReport.OpenXml\DoddleReport.OpenXml.csproj -Build -Prop Configuration=Release

nuget pack DoddleReport.Dynamic.nuspec
nuget pack DoddleReport.Sample.Mvc.nuspec

pause