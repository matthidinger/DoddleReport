using System;
using System.Linq;
using System.Web.Mvc;
using DoddleReport.Sample.Web.Models;
using DoddleReport.Builder.Extentions;
using DoddleReport.OpenXml;
using DoddleReport.Web;

namespace DoddleReport.Sample.Web.Controllers
{
    public class BuilderPaternController : Controller
    {
        // GET
        public ActionResult Index()
        {
            var query = DoddleProductRepository.GetAll();
            var totalProducts = query.Count;
            var totalOrders = query.Sum(p => p.OrderCount);

            var report = query.Export()
                .Column("Id", x => x.Id)
                .Column("Description", x => x.Description)
                .Column("Price", x => x.Price)
                .Column("Calculation", x => 0)
                .Column("Formula", x => 0)
                .Column("Text formula", x => string.Empty)
                .ToReport(null);

            report.TextFields.Title = "Products Report";
            report.TextFields.SubTitle = "This is a sample report showing how Doddle Report works";
            report.TextFields.Footer = "Copyright 2011 (c) The Doddle Project";
            report.TextFields.Header = string.Format(@"
                Report Generated: {0}
                Total Products: {1}
                Total Orders: {2}
                Total Sales: {3:c}", DateTime.Now, totalProducts, totalOrders, totalProducts * totalOrders);

            report.RenderHints.FreezeRows = 5;
            report.RenderHints.FreezeColumns = 1;

            report.DataFields["Price"].DataFormatString = "{0:c}";
            report.DataFields["Price"].ShowTotals = true;

            report.DataFields["Calculation"].DataFormatString = "{0:c}";
            report.DataFields["Calculation"].ExcelFormula = "=2*C#ROW#";

            report.DataFields["Formula"].ShowTotals = true;
            report.DataFields["Formula"].DataFormatString = "{0:c}";
            report.DataFields["Formula"].ExcelFormula = "=C#ROW#+D#ROW#";

            report.DataFields["Text formula"].ExcelFormula = "=LEFT(B#ROW#, 30)";



            report.RenderHints[ExcelReportWriter.Password] = "password";

            return new ReportResult(report, new ExcelReportWriter(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }
    }
}