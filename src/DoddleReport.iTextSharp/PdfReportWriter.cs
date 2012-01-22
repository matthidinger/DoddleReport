using System.IO;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace DoddleReport.iTextSharp
{
    public class PdfReportWriter : IReportWriter
    {
        public const string TitleStyle = "TitleStyle";
        public const string SubTitleStyle = "SubTitleStyle";
        public const string HeaderStyle = "HeaderStyle";
        public const string FooterStyle = "FooterStyle";
        public const string FontFamily = "FontFamily";

        /// <summary>
        /// Writes the report.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <param name="destination">The destination.</param>
        public void WriteReport(Report report, Stream destination)
        {
            Rectangle pageSize = new Rectangle(report.RenderHints.PageSize.Width, report.RenderHints.PageSize.Height);
            if (report.RenderHints.Orientation == ReportOrientation.Landscape)
                pageSize = pageSize.Rotate();

            var margins = report.RenderHints.Margins;
            var doc = new Document(pageSize, margins.Width, margins.Width, margins.Height, margins.Height);
            using (PdfWriter.GetInstance(doc, destination))
            {
                doc.Open();

                var globalTable = new PdfPTable(1)
                                      {
                                          HeaderRows = 1, 
                                          WidthPercentage = 100
                                      };

                // Render the header
                RenderHeader(globalTable, report.TextFields, report.RenderHints);

                // Render all the rows
                int fieldsCount = report.DataFields.Where(f => !f.Hidden).Count();
                var table = new PdfPTable(fieldsCount)
                                {
                                    HeaderRows = 1, 
                                    WidthPercentage = 100
                                };

                globalTable.AddCell(new PdfPCell(table) {Border = 0});

                foreach (ReportRow row in report.GetRows())
                {
                    foreach (RowField field in row.Fields.Where(f => !f.Hidden))
                    {
                        PdfPCell cell;
                        if (row.RowType == ReportRowType.HeaderRow)
                        {
                            cell = CreateTextCell(field.HeaderStyle, report.RenderHints[FontFamily] as string,
                                                  field.HeaderText);
                        }
                        else
                        {
                            cell = CreateTextCell(field.DataStyle, report.RenderHints[FontFamily] as string,
                                                  row.GetFormattedValue(field));
                        }

                        cell.Border = 0;
                        cell.Padding = 5;
                        table.AddCell(cell);
                    }
                }

                // Render the footer
                RenderFooter(globalTable, report.TextFields, report.RenderHints);

                doc.Add(globalTable);
                doc.Close();

            }
        }

        /// <summary>
        /// Appends the report.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        public void AppendReport(Report source, Report destination)
        {
        }

        /// <summary>
        /// Renders the header.
        /// </summary>
        /// <param name="globalTable">The global table.</param>
        /// <param name="textFields">The text fields.</param>
        /// <param name="renderHints">The render hints.</param>
        protected virtual void RenderHeader(PdfPTable globalTable, ReportTextFieldCollection textFields,
                                            RenderHintsCollection renderHints)
        {
            int rowCount = 0;
            if (!string.IsNullOrEmpty(textFields.Title))
            {
                ReportStyle reportStyle = renderHints[TitleStyle] as ReportStyle ?? GetDefaultTitleStyle();
                globalTable.AddCell(CreateTextCell(reportStyle, renderHints[FontFamily] as string, textFields.Title));
                rowCount++;
            }

            if (!string.IsNullOrEmpty(textFields.SubTitle))
            {
                ReportStyle reportStyle = renderHints[SubTitleStyle] as ReportStyle ?? GetDefaultSubTitleStyle();
                globalTable.AddCell(CreateTextCell(reportStyle, renderHints[FontFamily] as string, textFields.SubTitle));
                rowCount++;
            }

            if (!string.IsNullOrEmpty(textFields.Header))
            {
                ReportStyle reportStyle = renderHints[HeaderStyle] as ReportStyle ?? GetDefaultHeaderStyle();
                globalTable.AddCell(CreateTextCell(reportStyle, renderHints[FontFamily] as string, textFields.Header));
                rowCount++;
            }

            if (rowCount > 0)
            {
                PdfPCell cell = CreateTextCell(new ReportStyle(), renderHints[FontFamily] as string, string.Empty);
                cell.PaddingBottom = 10;
                globalTable.AddCell(cell);
                rowCount++;
            }

            globalTable.HeaderRows = rowCount;
        }

        /// <summary>
        /// Renders the footer.
        /// </summary>
        /// <param name="globalTable">The global table.</param>
        /// <param name="textFields">The text fields.</param>
        /// <param name="renderHints">The render hints.</param>
        protected virtual void RenderFooter(PdfPTable globalTable, ReportTextFieldCollection textFields,
                                            RenderHintsCollection renderHints)
        {
            if (!string.IsNullOrEmpty(textFields.Footer))
            {
                ReportStyle reportStyle = renderHints[FooterStyle] as ReportStyle ?? GetDefaultFooterStyle();
                globalTable.AddCell(CreateTextCell(reportStyle, renderHints[FontFamily] as string, textFields.Footer));
            }
        }


        /// <summary>
        /// Gets the default title style.
        /// </summary>
        /// <returns>The default title style.</returns>
        private static ReportStyle GetDefaultTitleStyle()
        {
            return new ReportStyle
                       {
                           Bold = true,
                           FontSize = 18,
                           HorizontalAlignment = HorizontalAlignment.Center
                       };
        }

        /// <summary>
        /// Gets the default sub title style.
        /// </summary>
        /// <returns>The default sub title style.</returns>
        private static ReportStyle GetDefaultSubTitleStyle()
        {
            return new ReportStyle
                       {
                           Bold = true,
                           FontSize = 14,
                           HorizontalAlignment = HorizontalAlignment.Center
                       };
        }

        /// <summary>
        /// Gets the default header style.
        /// </summary>
        /// <returns>The default header style.</returns>
        private static ReportStyle GetDefaultHeaderStyle()
        {
            return new ReportStyle
                       {
                           Bold = true,
                           FontSize = 11,
                           HorizontalAlignment = HorizontalAlignment.Left
                       };
        }

        /// <summary>
        /// Gets the default footer style.
        /// </summary>
        /// <returns>The default footer style.</returns>
        private static ReportStyle GetDefaultFooterStyle()
        {
            return new ReportStyle
                       {
                           HorizontalAlignment = HorizontalAlignment.Left
                       };
        }

        /// <summary>
        /// Creates the text cell.
        /// </summary>
        /// <param name="reportStyle">The report style.</param>
        /// <param name="fontName">Name of the font.</param>
        /// <param name="text">The text.</param>
        /// <returns>
        /// The newly created cell.
        /// </returns>
        private static PdfPCell CreateTextCell(ReportStyle reportStyle, string fontName, string text)
        {
            var par = new Paragraph(text, ConvertStyleToFont(reportStyle, fontName));
            var cell = new PdfPCell(par) {Border = 0};
            CopyStyleToCell(reportStyle, cell);
            return cell;
        }

        /// <summary>
        /// Converts a report style to a new font definition.
        /// </summary>
        /// <param name="reportStyle">The report style.</param>
        /// <param name="fontFamily">Name of the font.</param>
        /// <returns>The font.</returns>
        public static Font ConvertStyleToFont(ReportStyle reportStyle, string fontFamily)
        {
            var font = new Font();
            font.SetFamily(fontFamily);

            if (reportStyle.Underline)
            {
                font.SetStyle(Font.UNDERLINE);
            }
            else if (reportStyle.Bold || reportStyle.Italic)
            {
                if (reportStyle.Bold && reportStyle.Italic)
                {
                    font.SetStyle(Font.BOLDITALIC);
                }
                else if (reportStyle.Bold)
                {
                    font.SetStyle(Font.BOLD);
                }
                else
                {
                    font.SetStyle(Font.ITALIC);
                }
            }

            font.Size = reportStyle.FontSize;
            font.SetColor(reportStyle.ForeColor.R, reportStyle.ForeColor.G, reportStyle.ForeColor.B);
            return font;
        }

        /// <summary>
        /// Copies the report style to the cell style.
        /// </summary>
        /// <param name="reportStyle">The report style.</param>
        /// <param name="cell">The cell.</param>
        public static void CopyStyleToCell(ReportStyle reportStyle, PdfPCell cell)
        {
            cell.BackgroundColor = new BaseColor(reportStyle.BackColor);
            
            switch (reportStyle.HorizontalAlignment)
            {
                case HorizontalAlignment.Center:
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    break;
                case HorizontalAlignment.Left:
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    break;
                case HorizontalAlignment.Right:
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    break;
            }

            switch (reportStyle.VerticalAlignment)
            {
                case VerticalAlignment.Bottom:
                    cell.VerticalAlignment = Element.ALIGN_BOTTOM;
                    break;
                case VerticalAlignment.Middle:
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    break;
                case VerticalAlignment.Top:
                    cell.VerticalAlignment = Element.ALIGN_TOP;
                    break;
            }
        }
    }
}