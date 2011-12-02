nuget pack ..\src\Doddle.Reporting\Doddle.Reporting.csproj -Build -Prop Configuration=Release
nuget pack ..\src\Doddle.Reporting.Web\Doddle.Reporting.Web.csproj -Build -Prop Configuration=Release
nuget pack ..\src\Doddle.Reporting.AbcPdf\Doddle.Reporting.AbcPdf.csproj -Build -Prop Configuration=Release
nuget pack ..\src\Doddle.Reporting.OpenXml\Doddle.Reporting.OpenXml.csproj -Build -Prop Configuration=Release

nuget pack DoddleReport.Dynamic.nuspec
nuget pack DoddleReport.Sample.Mvc.nuspec

pause