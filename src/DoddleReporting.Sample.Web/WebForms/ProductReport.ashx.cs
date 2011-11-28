using Doddle.Reporting;
using Doddle.Reporting.Web;
using DoddleReporting.Sample.Web.Models;

namespace DoddleReporting.Sample.Web.WebForms
{
    public class ProductReport : WebReport
    {
        public override string FileName
        {
            get { return "ProductsReport"; }
        }

        public override void CustomizeReport(Report report)
        {
        }

        public override IReportSource ReportSource
        {
            get { return DoddleProductRepository.GetAll().ToReportSource(); }
        }
    }
}