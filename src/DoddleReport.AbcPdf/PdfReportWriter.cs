using System;
using System.Linq;
using System.IO;
using WebSupergoo.ABCpdf10;

namespace DoddleReport.AbcPdf
{
    public class PdfReportWriter : IReportWriter
    {
        public void WriteReport(Report report, Stream destination)
        {
            if (Document == null)
                BuildDocument(report.TextFields, report.RenderHints, report.GetRows());

            Document.Write(destination);
        }

        public PdfDocument Document { get; set; }

        public PdfDocument BuildDocument(ReportTextFieldCollection textFields, RenderHintsCollection hints, ReportRowCollection rows)
        {
            Document = hints.Orientation == ReportOrientation.Portrait ? new PdfDocument(ReportOrientation.Portrait) : new PdfDocument();


            Document.Font = Document.AddFont("Helvetica");
            Document.FontSize = 6;



            XRect docCoords = Document.Rect;
            double height = Document.Rect.Height;
            double width = Document.Rect.Width;

            Document.Rect.Inset(10, 40);
            Document.Rect.Height = Document.Rect.Height - 50;

            int fieldCount = 0;
            if (rows[0] != null)
            {
                fieldCount = rows[0].Fields.Count(f => f.Hidden == false);
            }

            var table = new PDFTable(Document, fieldCount)
                            {
                                CellPadding = 5, 
                                RepeatHeader = true
                            };


            if (hints.ContainsKey("PdfWidths"))
            {
                double[] widths = hints["PdfWidths"] as double[];
                table.SetColumnWidths(widths);
            }


            foreach (ReportRow row in rows)
            {
                table.NextRow();

                int cellIndex = 0;

                foreach (RowField field in row.Fields)
                {
                    table.NextCell();

                    string cellHtml;
                    if (row.RowType == ReportRowType.HeaderRow)
                    {
                        cellHtml = string.Format("<StyleRun FontSize=7><B><U>{0}</U></B></StyleRun>", field.HeaderText);
                    }
                    else
                    {
                        // TODO: Support DataRow styling, etc
                        cellHtml = row.GetFormattedValue(field);
                    }

                    table.AddHtml(cellHtml);
                    cellIndex++;
                }

                if (hints.ContainsKey("PdfUnderline"))
                {
                    if (hints["PdfUnderline"] as bool? == true)
                    {
                        table.UnderlineRow("800 800 800", table.Row);
                    }
                }
            }


            // HEADER
            Document.Rect.Position(10, height - 90);
            Document.Rect.Height = 80;
            Document.Rect.Width = width;

            for (int i = 1; i <= Document.PageCount; i++)
            {
                Document.PageNumber = i;

                Document.AddHtml(string.Format("<h3 align='center'>{0}</h3>", textFields.Title.FormatHtml()));
                Document.AddHtml(string.Format("<h5 align='center'>{0}</h5>", textFields.SubTitle.FormatHtml()));
                Document.AddHtml(string.Format("<p><b>{0}</b></p>", textFields.Header.FormatHtml()));
            }



            // TODO: Support for sub-totals on each page, for columns that ask for totals
            // TODO: Need to add support for the Report Footer text, currently only supports page numbers

            // FOOTER
            if (hints.IncludePageNumbers)
            {
                Document.Rect.Position(10, Document.MediaBox.Bottom);
                Document.Rect.Height = 30;
                Document.Rect.Width = width;
                Document.HPos = 0.5;
                Document.VPos = 0.5;

                for (int i = 1; i <= Document.PageCount; i++)
                {
                    //Document.AddBookmark("Page " + i.ToString(), true);
                    Document.PageNumber = i;
                    Document.AddText(string.Format("Page {0} of {1}", i, Document.PageCount));
                }
            }

            for (int i = 1; i <= Document.PageCount; i++)
            {
                Document.PageNumber = i;
                Document.Flatten();
            }

            return Document;
        }


        public void AppendReport(Report source, Report destination)
        {
            var sourceWriter = source.Writer as PdfReportWriter;
            if (sourceWriter == null)
                throw new InvalidOperationException("Unable to append report, the source Writer is not a PdfReportWriter");

            var destinationWriter = destination.Writer as PdfReportWriter;
            if (destinationWriter == null)
                throw new InvalidOperationException("Unable to append report, the destination Writer is not a PdfReportWriter");


            var supplement = destinationWriter.BuildDocument(destination.TextFields, destination.RenderHints, destination.GetRows());

            sourceWriter.BuildDocument(source.TextFields, source.RenderHints, source.GetRows());
            sourceWriter.Document.Append(supplement);

        }
    }
}
