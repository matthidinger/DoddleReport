using System.Collections;
using System.Data;
using DoddleReport.Configuration;
using DoddleReport.ReportSources;

namespace DoddleReport
{
    public static class ReportBuilder
    {
        public static IReportSource ToReportSource(this IEnumerable source)
        {
            return new EnumerableReportSource(source);
        }

        public static Report ToReport(this IReportSource source, string format)
        {
            // TODO: Get a Writer
            //var writer = Config.Report.Writers.GetWriterByName(format);
            return new Report(source, null);
        }

        public static Report ToReport(this IEnumerable source, string format)
        {
            IReportSource reportSource = new EnumerableReportSource(source);
            return reportSource.ToReport(format);
        }
    }
}