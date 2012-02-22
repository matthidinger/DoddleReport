using System.Web.Mvc;

namespace DoddleReport.Web
{
    public static class ControllerExtensions
    {
        public static ReportResult ReportResult(this Controller controller, Report report)
        {
            return new ReportResult(report);
        }
    }
}