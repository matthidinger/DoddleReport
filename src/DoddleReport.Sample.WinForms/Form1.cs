using System;
using System.Linq;
using System.Windows.Forms;
using DoddleReport.Sample.Web.Models;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using DoddleReport.Sample.WinForms.Properties;
using DoddleReport.Writers;

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
            if (cmbReportType.Text == "Excel (Open XML)")
            {
                RenderExcel();
            }
            else if (cmbReportType.Text == "PDF with title image (ITextSharp)")
            {
                RenderPdfWithImage();
            }
            else if (cmbReportType.Text == "HTML with custom title style & image")
            {
                RenderHtmlWithImage();
            }
        }

        private static void RenderExcel()
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

        private static void RenderPdfWithImage()
        {
            var query = DoddleProductRepository.GetAll();
            var totalProducts = query.Count;
            var totalOrders = query.Sum(p => p.OrderCount);

            // Create the report and turn our query into a ReportSource
            var report = new Report(query.ToReportSource(), new DoddleReport.iTextSharp.PdfReportWriter());

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

            report.RenderHints["TitleStyle"] = new ReportTitleStyle
            {
                Bold = true,
                FontSize = 18,
                HorizontalAlignment = HorizontalAlignment.Left,
                ImageWidthPercentage = 20,  //Image will take up 20% with page width
                Image = new ReportImage
                {
                    ImageData = File.ReadAllBytes(@"./Resources/Logomakr.png"),
                    Width = 75,
                    Height = 75
                }
            };




            // Customize the data fields
            report.DataFields["Id"].Hidden = true;
            report.DataFields["Price"].DataFormatString = "{0:c}";
            report.DataFields["LastPurchase"].DataFormatString = "{0:d}";

            using (var stream = File.Create("Report.pdf"))
            {
                report.WriteReport(stream);
            }

            Process.Start("Report.pdf");
        }


        private static void RenderHtmlWithImage()
        {
            var query = DoddleProductRepository.GetAll();
            var totalProducts = query.Count;
            var totalOrders = query.Sum(p => p.OrderCount);

            // Create the report and turn our query into a ReportSource
            var report = new Report(query.ToReportSource(), new HtmlReportWriter());

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
            
            report.RenderHints["TitleStyle"] = new ReportTitleStyle
            {
                Bold = true,
                Underline = true,
                FontSize = 18,
                HorizontalAlignment = HorizontalAlignment.Left,
                ForeColor = Color.SteelBlue,
                Image = new ReportImage
                {
                    ImageData = File.ReadAllBytes(@"./Resources/Logomakr.png"),
                    Width = 150,
                    Height = 100
                }
            };

            report.RenderHints["SubTitleStyle"] = new ReportStyle
            {
                Bold = true,
                FontSize = 12,
                HorizontalAlignment = HorizontalAlignment.Left,
                ForeColor = Color.SteelBlue
            };

            report.RenderHints["HeaderStyle"] = new ReportStyle
            {
                Italic = true,
                FontSize = 10,
                HorizontalAlignment = HorizontalAlignment.Left,
                ForeColor = Color.Salmon
            };
            
            // Customize the data fields
            report.DataFields["Id"].Hidden = true;
            report.DataFields["Price"].DataFormatString = "{0:c}";
            report.DataFields["LastPurchase"].DataFormatString = "{0:d}";

            using (var stream = File.Create("Report.html"))
            {
                report.WriteReport(stream);
            }

            Process.Start("Report.html");
        }
    }
}
