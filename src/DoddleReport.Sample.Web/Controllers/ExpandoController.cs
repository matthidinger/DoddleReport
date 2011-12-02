using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Web.Mvc;
using DoddleReport.Web;

namespace DoddleReport.Sample.Web.Controllers
{
    public class ExpandoController : Controller
    {

        //
        // Sample URLs:
        //  http://localhost:X/Expando/Expando.html
        //  http://localhost:X/Expando/Expando.xls
        //  http://localhost:X/Expando/Expando.txt
        //  http://localhost:X/Expando/Expando.pdf (REQUIRES ABCPDF INSTALLED)
        //  

        public ReportResult Expando()
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

            var report = new Report(DynamicReportSourceExtensions.ToReportSource(all));

            // Customize the Text Fields
            report.TextFields.Title = "Expando Report";
            report.TextFields.SubTitle = "This is a sample report showing how Doddle Report works";
            report.TextFields.Footer = "Copyright 2011 &copy; The Doddle Project";
            report.TextFields.Header = string.Format(@"Report Generated: {0}", DateTime.Now);

            report.DataFields["SchoolId"].HeaderText = "School";

            // Render hints allow you to pass additional hints to the reports as they are being rendered
            report.RenderHints.BooleanCheckboxes = true;

            return new ReportResult(report);
        }
    }
}
