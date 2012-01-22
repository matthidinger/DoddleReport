using System.Web.Mvc;
using DoddleReport.AbcPdf;
using DoddleReport.Sample.Web.Helpers;
using DoddleReport.Web;

namespace DoddleReport.Sample.Web.Controllers
{
    public class AbcPdfController : Controller
    {
        public ReportResult ProductReport()
        {
            var report = ProductReportHelper.GetProductReport();
            return new ReportResult(report, new PdfReportWriter());
        }
    }
}
