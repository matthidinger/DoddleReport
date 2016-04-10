using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using DoddleReport.Web;

namespace DoddleReport.Sample.Web.Controllers
{
    public class DataAnnotationsController : Controller
    {
        public class AnnotatedProduct
        {
            [Display(Name = "Product Name")]
            public string Name { get; set; }

            public decimal Price { get; set; }

            public bool OutOfStock { get; set; }
        }

        public ReportResult Index()
        {
            var list = new List<AnnotatedProduct>();


            list.Add(new AnnotatedProduct { Name = "Product 1", Price = 500 });
            list.Add(new AnnotatedProduct { Name = "Product 2", Price = 600 });
            list.Add(new AnnotatedProduct { Name = "Product 3", Price = 700 });

            var report = new Report(list.ToReportSource());


            // Adavanced data field formatting using a callback delegate
            report.DataFields["OutOfStock"].FormatAs<bool>(value => value ? "Yes" : "No");

            return new ReportResult(report);
        }
    }
}
