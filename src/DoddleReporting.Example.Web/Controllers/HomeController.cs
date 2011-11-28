﻿using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Doddle.Reporting;
using Doddle.Reporting.AbcPdf;
using Doddle.Reporting.Web;
using DoddleReporting.Example.Web.Models;
using Doddle.Reporting.Writers;
using System.IO;
using System.Drawing;

namespace DoddleReporting.Example.Web.Controllers
{
    //[HandleError]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewData["Message"] = "Welcome to ASP.NET MVC!";

            return View();
        }


        public ReportResult ProductReport()
        {
            // Get the data for the report (any IEnumerable will work)
            var query = ProductRepository.GetAll();
            var totalProducts = query.Count;
            var totalOrders = query.Sum(p => p.OrderCount);


            // Create the report and turn our query into a ReportSource
            var report = new Report(query.ToReportSource());


            // Customize the Text Fields
            report.TextFields.Title = "Products Report";
            report.TextFields.SubTitle = "This is a sample report showing how Doddle Report works";
            report.TextFields.Footer = "Copyright 2011 &copy; The Doddle Project";
            report.TextFields.Header = string.Format(@"
                Report Generated: {0}
                Total Products: {1}
                Total Orders: {2}
                Total Sales: {3:c}", DateTime.Now, totalProducts, totalOrders, totalProducts * totalOrders);


            // Render hints allow you to pass additional hints to the reports as they are being rendered
            report.RenderHints.BooleanCheckboxes = true;


            // Customize the data fields
            report.DataFields["Id"].Hidden = true;
            report.DataFields["Price"].DataFormatString = "{0:c}";
            report.DataFields["LastPurchase"].DataFormatString = "{0:d}";

            report.RenderingRow += report_RenderingRow;


            // Return the ReportResult
            // the type of report that is rendered will be determined by the extension in the URL (.pdf, .xls, .html, etc)
            return new ReportResult(report);
        }


        void report_RenderingRow(object sender, ReportRowEventArgs e)
        {
            if (e.Row.RowType == ReportRowType.DataRow)
            {
                var unitsInStock = (int)e.Row["UnitsInStock"];
                if (unitsInStock < 100)
                {
                    e.Row.Fields["UnitsInStock"].DataStyle.Bold = true;
                    e.Row.Fields["UnitsInStock"].DataStyle.ForeColor = Color.Maroon;
                }
            }
        }
    }
}
