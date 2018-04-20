using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using ClosedXML.Excel;

namespace DoddleReport.OpenXml
{
    public class ExcelReportWriter : IReportWriter
    {
        public const string SheetName = "SheetName";
        public const string TitleStyle = "TitleStyle";
        public const string SubTitleStyle = "SubTitleStyle";
        public const string HeaderStyle = "HeaderStyle";
        public const string FooterStyle = "FooterStyle";
        public const string PaperSize = "PaperSize";
        public const string AdjustColumnWidthToContents = "AdjustColumnWidthToContents";

        /// <summary>
        /// This Dictionary maps standard .NET format strings (like {0:c}) to OpenXML Format strings like "$ #,##0.00"
        /// A list of examples can be found here http://closedxml.codeplex.com/wikipage?title=NumberFormatId%20Lookup%20Table&referringTitle=Styles%20-%20NumberFormat
        /// </summary>
        public static Dictionary<string, string> OpenXmlDataFormatStringMap =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { "{0:c}", "$ #,##0.00" }
                };

        /// <summary>
        /// Gets the reports to append.
        /// </summary>
        internal IDictionary<Report, IList<Report>> ReportsToAppend { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelReportWriter"/> class.
        /// </summary>
        public ExcelReportWriter()
        {
            ReportsToAppend = new Dictionary<Report, IList<Report>>();
        }

        private static string GetOpenXmlDataFormatString(string dataFormatString)
        {
            if (OpenXmlDataFormatStringMap.ContainsKey(dataFormatString))
                return OpenXmlDataFormatStringMap[dataFormatString];

            return dataFormatString;
        }

        /// <summary>
        /// Writes the report.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <param name="destination">The destination.</param>
        public void WriteReport(Report report, Stream destination)
        {
            var workbook = new XLWorkbook();
            WriteReport(report, workbook);

            using (var ms = new MemoryStream())
            {
                workbook.SaveAs(ms);
                ms.Position = 0;
                ms.CopyTo(destination);
            }
        }

        /// <summary>
        /// Appends the report.
        /// </summary>
        /// <param name="source">The report source.</param>
        /// <param name="destination">The report destination.</param>
        public void AppendReport(Report source, Report destination)
        {
            if (source == null || destination == null)
            {
                throw new ArgumentNullException(source == null ? "source" : "destination");
            }

            var sourceWriter = source.Writer as ExcelReportWriter;
            if (sourceWriter == null)
            {
                throw new InvalidOperationException("Unable to append report, the source Writer is not a ExcelReportWriter");
            }

            if (!ReportsToAppend.ContainsKey(source))
            {
                ReportsToAppend.Add(source, new List<Report>());
            }

            ReportsToAppend[source].Add(destination);
        }

        /// <summary>
        /// Renders the header.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="fieldsCount">The fields count.</param>
        /// <param name="textFields">The report text field collection.</param>
        /// <param name="renderHints">The render hints collection.</param>
        /// <returns>The last row the header was written on.</returns>
        protected virtual int RenderHeader(IXLWorksheet worksheet, int fieldsCount, ReportTextFieldCollection textFields, RenderHintsCollection renderHints)
        {
            int rowCount = 0;
            if (!string.IsNullOrEmpty(textFields.Title))
            {
                rowCount = RenderTextItem(worksheet, fieldsCount, textFields.Title, rowCount, renderHints[TitleStyle] as ReportStyle ?? GetDefaultTitleStyle());
            }

            if (!string.IsNullOrEmpty(textFields.SubTitle))
            {
                rowCount = RenderTextItem(worksheet, fieldsCount, textFields.SubTitle, rowCount, renderHints[SubTitleStyle] as ReportStyle ?? GetDefaultSubTitleStyle());
            }

            if (!string.IsNullOrEmpty(textFields.Header))
            {
                rowCount = RenderTextItem(worksheet, fieldsCount, textFields.Header, rowCount, renderHints[HeaderStyle] as ReportStyle ?? GetDefaultHeaderStyle());
            }

            if (rowCount > 0)
            {
                rowCount++;
                worksheet.Range(rowCount, 1, rowCount, fieldsCount).Merge();
            }

            return rowCount;
        }

        /// <summary>
        /// Renders the text item.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="fieldsCount">The fields count.</param>
        /// <param name="itemText">The item text.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="reportStyle">The report style.</param>
        /// <returns>The row it last wrote on.</returns>
        private static int RenderTextItem(IXLWorksheet worksheet, int fieldsCount, string itemText, int currentRow, ReportStyle reportStyle)
        {
            foreach (var s in itemText.Split(new[] { "\r\n"}, StringSplitOptions.None))
            {
                currentRow++;
                var row = worksheet.Row(currentRow);
                var cell = row.Cell(1);
                cell.Value = s;
                reportStyle.CopyToXlStyle(cell.Style);
                worksheet.Range(currentRow, 1, currentRow, fieldsCount).Merge();
            }

            return currentRow;
        }

        /// <summary>
        /// Renders the row.
        /// </summary>
        /// <param name="rowCount">The row count.</param>
        /// <param name="reportRow">The row.</param>
        /// <param name="dataRow">The data row.</param>
        protected virtual void RenderRow(int rowCount, ReportRow reportRow, IXLRow dataRow)
        {
            int colCount = 0;
            foreach (var field in reportRow.Fields.Where(f => !f.Hidden))
            {
                colCount++;

                if (reportRow.RowType == ReportRowType.HeaderRow)
                {
                    var cell = dataRow.Cell(colCount);
                    cell.Value = field.HeaderText;
                    field.HeaderStyle.CopyToXlStyle(cell.Style);
                }
                else if (reportRow.RowType == ReportRowType.DataRow)
                {
                    var cell = dataRow.Cell(colCount);
                    field.DataStyle.CopyToXlStyle(cell.Style);
                    if (field.DataType == typeof(bool))
                    {
                        cell.SetDataType(XLDataType.Boolean);
                        cell.Value = reportRow[field];
                    }
                    else if (field.DataType.IsNumericType())
                    {
                        cell.SetDataType(XLDataType.Number);
                        if (!string.Equals("{0}", field.DataFormatString))
                        {
                            cell.Style.NumberFormat.Format = GetOpenXmlDataFormatString(field.DataFormatString);
                            cell.Value = reportRow[field];
                        }
                        else
                        {
                            cell.Value = reportRow.GetFormattedValue(field);
                        }
                    }
                    else if (field.DataType == typeof(DateTime) || field.DataType == typeof(DateTime?))
                    {
                        cell.SetDataType(XLDataType.DateTime);
                        cell.Value = reportRow.GetFormattedValue(field);
                    }
                    else
                    {
                        cell.Value = reportRow.GetFormattedValue(field);
                    }

					var url = reportRow.GetUrlString(field);
					if (url != null)
					{
						cell.Hyperlink = new XLHyperlink(url);
					}
                }
                else if (reportRow.RowType == ReportRowType.FooterRow)
                {
                    if (field.ShowTotals)
                    {
                        var cell = dataRow.Cell(colCount);
                        cell.SetDataType(XLDataType.Number);
                        cell.FormulaA1 = string.Format(CultureInfo.InvariantCulture, "=SUM({0}{1}:{0}{2})", cell.Address.ColumnLetter, 2, rowCount - 1);
                        if (!string.Equals("{0}", field.DataFormatString))
                        {
                            cell.Style.NumberFormat.Format = GetOpenXmlDataFormatString(field.DataFormatString);
                        }

                        field.FooterStyle.CopyToXlStyle(cell.Style);
                    }
                }
            }
        }

        /// <summary>
        /// Renders the footer.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="fieldsCount">The fields count.</param>
        /// <param name="textFields">The text fields.</param>
        /// <param name="renderHintsCollection">The render hints collection.</param>
        /// <param name="currentRow">The current row.</param>
        /// <returns>The last row it wrote on.</returns>
        protected virtual int RenderFooter(IXLWorksheet worksheet, int fieldsCount, ReportTextFieldCollection textFields, RenderHintsCollection renderHintsCollection, int currentRow)
        {
            if (!string.IsNullOrEmpty(textFields.Footer))
            {
                currentRow = RenderTextItem(worksheet, fieldsCount, textFields.Footer, currentRow, renderHintsCollection[FooterStyle] as ReportStyle ?? GetDefaultFooterStyle());
            }

            return currentRow;
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
        /// Writes the report.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <param name="workbook">The workbook.</param>
        private void WriteReport(Report report, XLWorkbook workbook)
        {
            var sheetName = report.RenderHints[SheetName] as string ?? "Data";
            IXLWorksheet worksheet;
            int duplicateNameCount = 0;
            var originalSheetName = sheetName;
            while (workbook.Worksheets.TryGetWorksheet(sheetName, out worksheet))
            {
                sheetName = originalSheetName + ++duplicateNameCount;
            }

            worksheet = workbook.Worksheets.Add(sheetName);
            worksheet.SetShowRowColHeaders(true);
            var orientation = report.RenderHints.Orientation == ReportOrientation.Portrait ? XLPageOrientation.Portrait : XLPageOrientation.Landscape;
            worksheet.PageSetup.PageOrientation = orientation;

            // Set the paper size to what the render hint is set to
            if (report.RenderHints[PaperSize] != null)
            {
                worksheet.PageSetup.PaperSize = (XLPaperSize)Enum.Parse(typeof(XLPaperSize), report.RenderHints[PaperSize].ToString());
            }

            if (report.RenderHints.FreezePanes)
                worksheet.SheetView.Freeze(report.RenderHints.FreezeRows, report.RenderHints.FreezeColumns);

            // Render the header
            var fieldsCount = report.DataFields.Count(f => !f.Hidden);
            int rowCount = RenderHeader(worksheet, fieldsCount, report.TextFields, report.RenderHints);

            // Render all the rows
            foreach (var row in report.GetRows())
            {
                rowCount++;
                var dataRow = worksheet.Row(rowCount);
                RenderRow(rowCount, row, dataRow);
            }

            // Render the footer
            RenderFooter(worksheet, fieldsCount, report.TextFields, report.RenderHints, rowCount);


            // TODO: AdjustToContents renders horribly when deployed to an Azure Website, need to determine why

            // Adjust the width of all the columns
            for (int i = 0; i < fieldsCount; i++)
            {
                var reportField = report.DataFields.Where(f => !f.Hidden).Skip(i).Take(1).Single();
                var width = new int[] { reportField.DataStyle.Width, reportField.FooterStyle.Width, reportField.HeaderStyle.Width }.Max();
                var adjustToContents = report.RenderHints[AdjustColumnWidthToContents] as bool? ?? true;

                if (adjustToContents || width > 0)
                {
                    var column = worksheet.Column(i + 1);
                    if (adjustToContents && width > 0)
                    {
                        column.AdjustToContents(width.PixelsToUnits(column.Style.Font), double.MaxValue);
                    }
                    else if (adjustToContents)
                    {
                        column.AdjustToContents(1, 50, 5.0, 100.0);
                    }
                    else
                    {
                        column.Width = width.PixelsToUnits(column.Style.Font);
                    }
                }
            }

            worksheet.Columns().AdjustToContents();

            // Check if the current writer needs to append another report to the report we just generated
            if (ReportsToAppend.ContainsKey(report))
            {
                foreach (var reportToAppend in ReportsToAppend[report])
                {
                    WriteReport(reportToAppend, workbook);
                }
            }
        }
    }
}
