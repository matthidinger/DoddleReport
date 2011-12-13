using System.Web.Mvc;
using DoddleReport.Sample.Web.Models;
using DoddleReport.Web;

namespace DoddleReport.Sample.Web.Areas.Reporting.Controllers
{
    // Used for testing DoddleReport in an MVC Area

    public class DoddleAreaController : Controller
    {
        public ReportResult ProductReport()
        {
            var query = DoddleProductRepository.GetAll();
            var report = new Report(query.ToReportSource());
            return new ReportResult(report);
        }
    }
}
