using System.IO;

namespace DoddleReport
{
    public interface IReportWriter
    {
        void WriteReport(Report report, Stream destination);
        void AppendReport(Report source, Report destination);
    }
}