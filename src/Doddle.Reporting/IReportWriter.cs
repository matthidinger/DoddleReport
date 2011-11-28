using System.IO;

namespace Doddle.Reporting
{
    public interface IReportWriter
    {
        void WriteReport(Report report, Stream destination);
        void AppendReport(Report source, Report destination);
    }
}