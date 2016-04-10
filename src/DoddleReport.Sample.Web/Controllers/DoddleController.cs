using System;
using System.Linq;
using System.Web.Mvc;
using System.Drawing;
using DoddleReport.Sample.Web.Models;
using DoddleReport.Web;

namespace DoddleReport.Sample.Web.Controllers
{
	// **********************
	//  
	// Don't forget to edit Global.asax and call the following method at the top of RegisterRoutes()
	//
	//      routes.MapReportingRoute();
	//
	// See http://doddlereport.codeplex.com/wikipage?title=Web%20Reporting for details
	// **********************


	public class DoddleController : Controller
	{
		//
		// Try the following sample URLs:
		//
		//  http://localhost:X/doddle/ProductReport.html
		//  http://localhost:X/doddle/ProductReport.txt
		//  http://localhost:X/doddle/ProductReport.xls
		//  http://localhost:X/doddle/ProductReport.xlsx (Requires DoddleReport.OpenXml)
		//  http://localhost:X/doddle/ProductReport.pdf (Requires DoddleReport.iTextSharp or DoddleReport.AbcPdf)
		//  

		public ReportResult ProductReport()
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
			// ReSharper disable Mvc.ActionNotResolved
			// ReSharper disable Mvc.ControllerNotResolved
			report.DataFields["Name"].Url<Product>(p => Url.Action("Index", "Products", new { p.Id }, "http"));
			// ReSharper restore Mvc.ControllerNotResolved
			// ReSharper restore Mvc.ActionNotResolved

			// Advanced customized on a row-by-row basis
			report.RenderingRow += report_RenderingRow;


			// Return the ReportResult
			// the type of report that is rendered will be determined by the extension in the URL (.pdf, .xls, .html, etc)
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
