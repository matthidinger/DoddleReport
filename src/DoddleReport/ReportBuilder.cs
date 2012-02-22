using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using DoddleReport.Configuration;
using DoddleReport.ReportSources;

namespace DoddleReport
{
    public static class ReportBuilder
    {
        public static IReportSource ToReportSource<T>(this IEnumerable<T> source)
        {
            var typeCheck = source.GetType().GetInterfaces()[0].GetGenericArguments()[0];

            return typeCheck == typeof(ExpandoObject) || typeCheck == typeof(object)
                ? (IReportSource)new DynamicReportSource(source.Cast<dynamic>())
                : new EnumerableReportSource(source);
        }

        public static IReportSource ToReportSource(this DataTable table)
        {
            return new DataTableReportSource(table);
        }

        public static Report ToReport(this IReportSource source, string format)
        {
            var writer = Config.Report.Writers.GetWriterByName(format);
            return new Report(source, writer);
        }

        public static Report ToReport(this IEnumerable source, string format)
        {
            IReportSource reportSource = new EnumerableReportSource(source);
            return reportSource.ToReport(format);
        }
    }
}