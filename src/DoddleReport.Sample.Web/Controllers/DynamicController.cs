using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Web.Mvc;
using DoddleReport.Sample.Web.Models;
using DoddleReport.Web;

namespace DoddleReport.Sample.Web.Controllers
{
    public class DynamicController : Controller
    {
        //
        // Sample URLs:
        //  http://localhost:X/Home/Index/Reporting/Dynamic/Dynamic.html
        //  http://localhost:X/Home/Index/Reporting/Dynamic/Dynamic.xls
        //  http://localhost:X/Home/Index/Reporting/Dynamic/Dynamic.txt
        //  http://localhost:X/Home/Index/Reporting/Dynamic/Dynamic.pdf (REQUIRES ABCPDF INSTALLED)
        //  

        public ReportResult Dynamic()
        {
            var all = new List<ExpandoObject>();
            dynamic d = new ExpandoObject();
            d.DistrictNumber = 1566;
            d.SchoolId = 1;
            d.FullName = "Matt Hidinger";

            dynamic d2 = new ExpandoObject();
            d2.DistrictNumber = 13566;
            d2.SchoolId = 1;
            d2.FullName = "Bob Jones";

            all.Add(d);
            all.Add(d2);

            var report = new Report(all.ToReportSource());

            // Customize the Text Fields
            report.TextFields.Title = "Dynamic Report";
            report.TextFields.SubTitle = "This is a sample report showing how Doddle Report works";
            report.TextFields.Footer = "Copyright 2011-2012 (c) The Doddle Project";
            report.TextFields.Header = string.Format(@"Report Generated: {0}", DateTime.Now);

            report.DataFields["SchoolId"].HeaderText = "School";

            // Render hints allow you to pass additional hints to the reports as they are being rendered
            report.RenderHints.BooleanCheckboxes = true;

            return new ReportResult(report);
        }

        public ReportResult DynamicNulls()
        {
            var all = new List<dynamic>();

            var product = new Product
            {
                Id = 1,
                Name = "Hammer",
                Description = "Unit Tester Widget",
                Price = 42,
                OrderCount = 10,
                UnitsInStock = 4
            };

            dynamic item = new ExpandoObject();
            item.Id = product.Id;
            item.Name = product.Name;
            item.Description = product.Description;
            item.Price = product.Price;
            item.OrderCount = product.OrderCount;
            item.LastPuchase = product.LastPurchase;
            item.UnitsInStock = product.UnitsInStock;
            item.LowStock = product.LowStock;

            all.Add(item);

            product = new Product
                    {
                        Id = 2,
                        Name = "Secret",
                        Price = 84.42,
                        OrderCount = 1,
                        UnitsInStock = 2
                    };

            item = new ExpandoObject();
            item.Id = product.Id;
            item.Name = product.Name;
            item.Description = product.Description;
            item.Price = product.Price;
            item.OrderCount = product.OrderCount;
            item.LastPuchase = product.LastPurchase;
            item.UnitsInStock = product.UnitsInStock;
            item.LowStock = product.LowStock;

            all.Add(item);

            var report = new Report(all.ToReportSource());

            // Customize the Text Fields
            report.TextFields.Title = "Dynamic Report with NULL values present";
            report.TextFields.SubTitle = "This is a sample report showing how Doddle Report works";
            report.TextFields.Footer = "Copyright 2011-2012 (c) The Doddle Project";
            report.TextFields.Header = string.Format(@"Report Generated: {0}", DateTime.Now);

            report.DataFields["OrderCount"].HeaderText = "Order Count";
            report.DataFields["UnitsInStock"].HeaderText = "Units in Stock";
            report.DataFields["LowStock"].HeaderText = "Stock Low";

            // Render hints allow you to pass additional hints to the reports as they are being rendered
            report.RenderHints.BooleanCheckboxes = true;

            return new ReportResult(report);
        }

        public ReportResult ProductReport()
        {
            IEnumerable<dynamic> query = DoddleProductRepository.GetAllExpando();
            var report = new Report(query.ToReportSource());
            return new ReportResult(report);
        }

        public ReportResult ProductReportObject()
        {
            IEnumerable<object> query = DoddleProductRepository.GetAll();
            var report = new Report(query.ToReportSource());
            return new ReportResult(report);
        }

        public ReportResult ProductReportObjectExpando()
        {
            IEnumerable<object> query = DoddleProductRepository.GetAllExpando();
            var report = new Report(query.ToReportSource());
            return new ReportResult(report);
        }
    }
}
