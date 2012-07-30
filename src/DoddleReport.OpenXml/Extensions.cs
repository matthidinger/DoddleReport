using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ClosedXML.Excel;

namespace DoddleReport.OpenXml
{
    internal static class Extensions
    {
        #region Declarations

        private static readonly string[] Digits = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

        #endregion

        #region Methods

        public static void CopyTo(this Stream input, Stream output)
        {
            var buffer = new byte[32768];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
            }
        }

        /// <summary>
        /// Copies the report style to an XL Style.
        /// </summary>
        /// <param name="reportStyle">The report style.</param>
        /// <param name="xlStyle">The xl style.</param>
        public static void CopyToXlStyle(this ReportStyle reportStyle, IXLStyle xlStyle)
        {
            if (reportStyle.BackColor != Color.White)
            {
                xlStyle.Fill.BackgroundColor = XLColor.FromColor(reportStyle.BackColor);
            }

            if (reportStyle.ForeColor != Color.Black)
            {
                xlStyle.Font.SetFontColor(XLColor.FromColor(reportStyle.ForeColor));
            }

            xlStyle.Font.SetBold(reportStyle.Bold);
            xlStyle.Font.SetFontSize(reportStyle.FontSize);
            xlStyle.Font.SetItalic(reportStyle.Italic);
            xlStyle.Font.SetUnderline(reportStyle.Underline ? XLFontUnderlineValues.Single : XLFontUnderlineValues.None);
            xlStyle.Alignment.Horizontal =
                reportStyle.HorizontalAlignment == HorizontalAlignment.Center ? XLAlignmentHorizontalValues.Center :
                reportStyle.HorizontalAlignment == HorizontalAlignment.Left ? XLAlignmentHorizontalValues.Left :
                XLAlignmentHorizontalValues.Right;
            xlStyle.Alignment.Vertical =
                reportStyle.VerticalAlignment == VerticalAlignment.Bottom ? XLAlignmentVerticalValues.Bottom :
                reportStyle.VerticalAlignment == VerticalAlignment.Middle ? XLAlignmentVerticalValues.Center :
                XLAlignmentVerticalValues.Top;
            xlStyle.Alignment.TextRotation = reportStyle.TextRotation;
        }

        /// <summary>
        /// Pixels to point.
        /// </summary>
        /// <param name="pixels">The pixels.</param>
        /// <param name="xlFont">The font.</param>
        /// <returns>
        /// The value in points.
        /// </returns>
        /// <remarks>
        /// The formula is documented there: http://msdn.microsoft.com/en-us/library/documentformat.openxml.spreadsheet.column.aspx
        /// </remarks>
        public static double PixelsToUnits(this int pixels, IXLFont xlFont)
        {
            if (pixels < 5)
            {
                pixels = 5;
            }

            var fontSize = (float)xlFont.FontSize;
            var font = new Font(xlFont.FontName, fontSize, FontStyle.Regular);
            int underscoreWidth = TextRenderer.MeasureText("__", font).Width;
            double maxDigitWidth = Digits.Select(d => TextRenderer.MeasureText("_" + d + "_", font).Width - underscoreWidth).Max();
            return Math.Truncate((pixels - 5) / maxDigitWidth * 100 + 0.5) / 100;
        }

        #endregion
    }
}
