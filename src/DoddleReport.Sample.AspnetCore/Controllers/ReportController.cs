using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using DoddleReport.Sample.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace DoddleReport.Sample.AspnetCore.Controllers
{
    public class ReportController : Controller
    {
        public IActionResult Index()
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
            report.RenderHints.FreezeRows = 9;
            report.RenderHints.FreezeColumns = 2;

            // Some writers (like PDF) support Orientation and page sizing
            //report.RenderHints.Orientation = ReportOrientation.Landscape;
            //report.RenderHints.PageSize = new SizeF(8.5f * 72f, 14f * 72f); //US Legal paper size
            //report.RenderHints.PageSize = new SizeF(595.28f, 841.89f); //A4 paper size
            //report.RenderHints.PageSize = new SizeF(842f, 1191f); //A3 paper size


            // Customize the data fields
            report.DataFields["Id"].Hidden = true;
            report.DataFields["Price"].DataFormatString = "{0:c}";
            report.DataFields["Price"].ShowTotals = true;
            report.DataFields["LastPurchase"].DataFormatString = "{0:d}";

            // Assign a delegate to generate a string that will be used as the href attribute 
            // for a link in the given field, specific to the dataitem for that row.
            // Some literal or constant is necessary for the root url if you want Excel
            // navigable links in any writer that doesn't render in the context of the
            // given web project (e.g. excel or pdf)
            report.DataFields["Name"].Url<Product>(p => Url.Action("Index", "Products", new { p.Id }, "http"));

            // Advanced customized on a row-by-row basis
            report.RenderingRow += report_RenderingRow;


            return new ReportResult(report);
        }

        void report_RenderingRow(object sender, ReportRowEventArgs e)
        {
            switch (e.Row.RowType)
            {
                case ReportRowType.HeaderRow:
                    e.Row.Fields["LastPurchase"].HeaderStyle.TextRotation = -90;
                    e.Row.Fields["UnitsInStock"].HeaderStyle.TextRotation = -90;
                    e.Row.Fields["LowStock"].HeaderStyle.TextRotation = -90;
                    break;
                case ReportRowType.DataRow:
                {
                    var unitsInStock = (int)e.Row["UnitsInStock"];
                    if (unitsInStock < 100)
                    {
                        e.Row.Fields["UnitsInStock"].DataStyle.Bold = true;
                        e.Row.Fields["UnitsInStock"].DataStyle.ForeColor = Color.Maroon;
                    }
                }
                    break;
            }
        }

    }
}