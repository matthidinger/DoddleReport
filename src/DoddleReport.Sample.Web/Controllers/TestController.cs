using System.Collections.Generic;
using System.Web.Mvc;
using DoddleReport.Sample.Web.Models;
using DoddleReport.Web;
using DoddleReport.Writers;

namespace DoddleReport.Sample.Web.Controllers
{
    public class TestController : Controller
    {
        public ReportResult ListOfObjects()
        {
            var list = new List<object>();


            list.Add(new
                         {
                             FirstName = "Matt",
                             LastName = "Hidinger"
                         });

            var report = new Report(list.ToReportSource());
            return new ReportResult(report);
        }

        public ReportResult WithExtension()
        {
            var query = DoddleProductRepository.GetAll();
            var report = new Report(query.ToReportSource());
            return new ReportResult(report) { FileName = "CustomName.html"};
        }

        public ReportResult CustomHtmlStyle()
        {
            var query = DoddleProductRepository.GetAll();
            var report = new Report(query.ToReportSource());

            //report.RenderHints["HtmlStyle"] = @".htmlReport th { font-size: 40px !important; }";
            foreach (var dataField in report.DataFields)
            {
                dataField.HeaderStyle.FontSize = 13;
            }
            return new ReportResult(report);
        }

        public ReportResult WithExtensionAndWriter()
        {
            var query = DoddleProductRepository.GetAll();
            var report = new Report(query.ToReportSource());
            return new ReportResult(report, new DelimitedTextReportWriter()) { FileName = "CustomName.txt" };
        }

        public ReportResult WithoutExtension()
        {
            var query = DoddleProductRepository.GetAll();
            var report = new Report(query.ToReportSource());
            return new ReportResult(report) { FileName = "CustomName" };
        }
    }
}
