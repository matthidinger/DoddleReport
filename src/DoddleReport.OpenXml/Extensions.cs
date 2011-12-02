using System.IO;
using System;
using System.Drawing;
using ClosedXML.Excel;

namespace DoddleReport.OpenXml
{
    internal static class Extensions
    {
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
        }

        /// <summary> 
        /// Determines if a type is numeric.  Nullable numeric types are considered numeric. 
        /// </summary> 
        /// <remarks> 
        /// Boolean is not considered numeric. 
        /// </remarks> 
        public static bool IsNumericType(this Type type)
        {
            if (type == null)
            {
                return false;
            }

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                case TypeCode.Object:
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        return IsNumericType(Nullable.GetUnderlyingType(type));
                    }

                    return false;
            }

            return false;
        }
    }
}
