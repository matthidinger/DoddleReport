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

            if (style.FontSize != 8)
            {
                html.AppendFormat("font-size: {0}pt;", style.FontSize);
            }

            
            html.AppendFormat("text-align: {0};", style.HorizontalAlignment);
            
            html.AppendFormat("vertical-align: {0};", style.VerticalAlignment);

            if (style.Width != 0)
            {
                html.AppendFormat("width: {0}px", style.Width);
            }

            if (style.TextRotation != 0)
            {
                var degrees = style.TextRotation*-1;

                html.AppendFormat("-webkit-transform: rotate({0}deg);", degrees);
                html.AppendFormat("-moz-transform: rotate({0}deg);", degrees);
                html.AppendFormat("-ms-transform: rotate({0}deg);", degrees);
                html.AppendFormat("-o-transform: rotate({0}deg);", degrees);
                html.AppendFormat("transform: rotate({0}deg);", degrees);

                var rad = degrees*Math.PI/180;
                var costheta = Math.Cos(rad);
                var sintheta = Math.Sin(rad);

                html.AppendFormat("filter: progid:DXImageTransform.Microsoft.Matrix(/* IE6–IE9 */ M11={0}, M12={1}, M21={2}, M22={3}, sizingMethod='auto expand');zoom: 1;", costheta, -sintheta, sintheta, costheta);
            }

            return html.ToString();

        }

        public static string FormatHtml(this string source)
        {
            return source.Replace(Environment.NewLine, "<br />");
        }
    }
}
