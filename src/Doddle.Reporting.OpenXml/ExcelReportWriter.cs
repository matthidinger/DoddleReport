using System;
using System.Globalization;
using System.IO;
using System.Linq;
using ClosedXML.Excel;

namespace Doddle.Reporting.OpenXml
{
    public class ExcelReportWriter : IReportWriter
    {
        public const string SheetName = "SheetName";
        public const string TitleStyle = "TitleStyle";
        public const string SubTitleStyle = "SubTitleStyle";
        public const string HeaderStyle = "HeaderStyle";
        public const string FooterStyle = "FooterStyle";

 
        /// <summary>
        /// Writes the report.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <param name="destination">The destination.</param>
        public void WriteReport(Report report, Stream destination)
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(report.RenderHints[SheetName] as string ?? "Data");
            worksheet.SetShowRowColHeaders(true);

            // Render the header
            var fieldsCount = report.DataFields.Where(f => !f.Hidden).Count();
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

            // Adjust the width of all the columns
            foreach (var column in worksheet.Columns())
            {
                column.AdjustToContents();
            }

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
            foreach (var s in itemText.Split(new[] { Environment.NewLine }, StringSplitOptions.None))
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
                    if (field.DataType == typeof(bool))
                    {
                        cell.SetDataType(XLCellValues.Boolean);
                    }
                    else if (field.DataType.IsNumericType())
                    {
                        cell.SetDataType(XLCellValues.Number);
                    }
                    else if (field.DataType == typeof(DateTime) || field.DataType == typeof(DateTime?))
                    {
                        cell.SetDataType(XLCellValues.DateTime);
                    }

                    cell.Value = reportRow.GetFormattedValue(field);
                    field.DataStyle.CopyToXlStyle(cell.Style);
                }
                else if (reportRow.RowType == ReportRowType.FooterRow)
                {
                    if (field.ShowTotals)
                    {
                        var cell = dataRow.Cell(colCount);
                        cell.FormulaA1 = string.Format(CultureInfo.InvariantCulture, "=SUM({0}{1}:{0}{2})", cell.Address.ColumnLetter, 2, rowCount - 1);
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
    }
}
