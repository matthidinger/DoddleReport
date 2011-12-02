using System;
using System.Drawing;
using System.Text;

namespace DoddleReport
{
    public static class HtmlFormatHelper
    {
        public static string GetHtml(this ReportStyle style)
        {
            var html = new StringBuilder();

            html.Append(style.Bold ? "font-weight: bold;" : "font-weight: normal;");
            html.Append(style.Underline ? "text-decoration: underline;" : "text-decoration: none;");
            html.Append(style.Italic ? "font-style: italic;" : "font-style: none;");

            if (style.BackColor != Color.White)
            {
                html.AppendFormat("background-color: {0};", ColorTranslator.ToHtml(style.BackColor));
            }

            if (style.ForeColor != Color.Black)
            {
                html.AppendFormat("color: {0};", ColorTranslator.ToHtml(style.ForeColor));
            }

            if (style.FontSize != 10)
            {
                html.AppendFormat("font-size: {0};", style.FontSize);
            }

            
            html.AppendFormat("text-align: {0};", style.HorizontalAlignment);
            
            html.AppendFormat("vertical-align: {0};", style.VerticalAlignment);

            if (style.Width != 0)
            {
                html.AppendFormat("width: {0}px", style.Width);
            }

            return html.ToString();

        }

        public static string FormatHtml(this string source)
        {
            return source.Replace(Environment.NewLine, "<br />");
        }
    }
}
