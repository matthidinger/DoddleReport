using System;
using System.Linq;
using System.Windows.Forms;
using DoddleReport.Sample.Web.Models;
using System.IO;
using System.Diagnostics;

namespace DoddleReport.Sample.WinForms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, System.EventArgs e)
        {

        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            var query = DoddleProductRepository.GetAll();
            var totalProducts = query.Count;
            var totalOrders = query.Sum(p => p.OrderCount);

            // Create the report and turn our query into a ReportSource
            var report = new Report(query.ToReportSource(), new DoddleReport.OpenXml.ExcelReportWriter());

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

            using (var stream = File.Create("Report.xlsx"))
            {
                report.WriteReport(stream);
            }

            Process.Start("Report.xlsx");
        }
    }
}
