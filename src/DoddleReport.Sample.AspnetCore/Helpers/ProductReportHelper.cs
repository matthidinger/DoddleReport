using System;
using System.Linq;
using DoddleReport.Sample.Web.Models;

namespace DoddleReport.Sample.Web.Helpers
{
    public static class ProductReportHelper
    {
        public static Report GetProductReport()
        {
            // Get the data for the report (any IEnumerable will work)
            var query = DoddleProductRepository.GetAll();
            var totalProducts = query.Count;
            var totalOrders = query.Sum(p => p.OrderCount);


            // Create the report and turn our query into a ReportSource
            var report = new Report(query.ToReportSource());


            // Customize the Text Fields
            report.TextFields.Title = "Products Report";
            report.TextFields.SubTitle = "This is a sample report showing how Doddle Report works";
            report.TextFields.Footer = "Copyright 2011 (c) The Doddle Project";
            report.TextFields.Header = string.Format(@"
                Report Generated: {0}
                Total Products: {1}
                Total Orders: {2}
                Total Sales: {3:c}", DateTime.Now, totalProducts, totalOrders, totalProducts * totalOrders);


            // Render hints allow you to pass additional hints to the reports as they are being rendered
            report.RenderHints.BooleanCheckboxes = true;
            report.RenderHints.BooleansAsYesNo = true;
            //report.RenderHints.Orientation = ReportOrientation.Landscape;


            // Customize the data fields
            report.DataFields["Id"].Hidden = true;
            report.DataFields["Price"].DataFormatString = "{0:c}";
            report.DataFields["Price"].ShowTotals = true;
            report.DataFields["LastPurchase"].DataFormatString = "{0:d}";

            return report;
        }
    }
}