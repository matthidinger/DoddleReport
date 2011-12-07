nuget pack ..\src\DoddleReport\DoddleReport.csproj -Build -Prop Configuration=Release
nuget pack ..\src\DoddleReport.Web\DoddleReport.Web.csproj -Build -Prop Configuration=Release
nuget pack ..\src\DoddleReport.AbcPdf\DoddleReport.AbcPdf.csproj -Build -Prop Configuration=Release
nuget pack ..\src\DoddleReport.OpenXml\DoddleReport.OpenXml.csproj -Build -Prop Configuration=Release
nuget pack ..\src\DoddleReport.iTextSharp\DoddleReport.iTextSharp.csproj -Build -Prop Configuration=Release

nuget pack DoddleReport.Dynamic.nuspec
nuget pack DoddleReport.Sample.Mvc.nuspec



nuget push DoddleReport.1.2.nupkg
nuget push DoddleReport.Abcpdf.1.2.nupkg
nuget push DoddleReport.Dynamic.1.2.nupkg
nuget push DoddleReport.OpenXml.1.2.nupkg
nuget push DoddleReport.Sample.Mvc.1.2.nupkg
nuget push DoddleReport.Web.1.2.nupkg
nuget push DoddleReport.iTextSharp.1.2.nupkg




pause