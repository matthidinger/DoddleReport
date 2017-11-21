using System;
using System.IO;
using System.Text;

namespace DoddleReport.Writers
{
    public class HtmlReportWriter : IReportWriter
    {
        public const string HtmlStyle = "HtmlStyle";
        public const string HtmlLogo = "HtmlLogo";

        protected StringBuilder Html { get; private set; }
        protected virtual bool WrapHeadAndBody { get; set; }


        public HtmlReportWriter()
            : this(true)
        {
        }

        public HtmlReportWriter(bool wrapHeadAndBody)
        {
            WrapHeadAndBody = wrapHeadAndBody;
            Html = new StringBuilder();
        }

        protected virtual string InternalStyling()
        {
            return string.Empty;
        }

        /// <summary>
        /// This CSS style will be applied to the top of every report. You may override this property to customize the default CSS that gets rendered on all HTML reports
        /// </summary>
        public static string DefaultStyle
        {
            get
            {
                var style = @"
                    .htmlReport { font: 12px Verdana; }
                    .htmlReport h1 { font-size: 12pt; margin-bottom: 10px; }
                    .htmlReport .title { margin-bottom: 1px; }
                    .htmlReport .subTitle { margin-bottom: 3px; margin-top: 1px; }
                    .htmlReport .header { padding-bottom: 8px; border-bottom: solid 1px #ccc; } \r\n";

                style += ".htmlReport td { " + ReportStyle.HeaderRowStyle.ToCss() + "}\r\n";
                style += ".htmlReport th { " + ReportStyle.DataRowStyle.ToCss() + "}\r\n";

                return style;
            }
        }

        protected void AppendStyling(RenderHintsCollection hints)
        {
            Html.AppendLine(@"<meta http-equiv=""content-type"" content=""text/html;charset=utf-8"" />");
            Html.AppendLine(@"<style type='text/css'>");

            // Add the default styles
            Html.AppendLine(DefaultStyle);

            // Add any custom CSS passed into RenderHints
            Html.AppendFormat("{0}", hints[HtmlStyle]);

            // Add any internal styles, such as the ExcelReportWriter CSS
            Html.AppendLine(InternalStyling());

            Html.AppendLine("</style>");

        }

        protected virtual void RenderHeader(ReportTextFieldCollection textFields, RenderHintsCollection hints)
        {
            if (WrapHeadAndBody)
            {
                Html.AppendLine("<html><head>");
            }

            AppendStyling(hints);

            if (WrapHeadAndBody)
            {
                Html.AppendLine("</head><body>");
            }


            Html.AppendLine("<div class='htmlReport'>");

            if (!string.IsNullOrEmpty(textFields.Title))
            {
                Html.AppendFormat("<h4 class='title'>{0}</h4>", textFields.Title.FormatHtml());
            }

            if (!string.IsNullOrEmpty(textFields.SubTitle))
            {
                Html.AppendFormat("<h5 class='subTitle'>{0}</h5>", textFields.SubTitle.FormatHtml());
            }

            if (!string.IsNullOrEmpty(textFields[HtmlLogo]))
            {
                Html.AppendFormat(textFields[HtmlLogo].FormatHtml());
            }

            if (!string.IsNullOrEmpty(textFields.Header))
            {
                Html.AppendFormat("<p class='header'>{0}</p>", textFields.Header.FormatHtml());
            }

            Html.AppendLine("<table border='0' cellpadding='2' cellspacing='0' width='100%'>");
        }

        private static bool GetBooleanValue(object input)
        {
            if (input == null)
                return false;

            try
            {
                return Convert.ToBoolean(input);
            }
            catch
            {
                return input.ToString().Equals("yes", StringComparison.OrdinalIgnoreCase);
            }
        }

        protected virtual void RenderRow(ReportRow row, RenderHintsCollection hints)
        {
            Html.AppendLine("<tr>");

            foreach (var field in row.Fields)
            {
                if (row.RowType == ReportRowType.HeaderRow)
                {
                    Html.AppendFormat("<th class='headerCell' style='{1}'>{0}</th>", field.HeaderText, GetCellStyle(row, field));
                }
                else if (row.RowType == ReportRowType.DataRow)
                {
                    if (hints.BooleanCheckboxes)
                    {
                        if (field.DataType == typeof(bool) || field.DataType == typeof(bool?))
                        {
                            string checkbox = "<input type='checkbox' disabled='disabled'";

                            if (GetBooleanValue(row[field.Name]))
                            {
                                checkbox += " checked='checked'";
                            }

                            checkbox += " />";
                            row[field] = checkbox;
                        }
                    }

					var formattedValue = row.GetFormattedValue(field);
					var url = row.GetUrlString(field);

					if (url != null)
					{
						formattedValue = string.Format("<a href='{1}'>{0}</a>", formattedValue, url);
					}

					Html.AppendFormat("<td style='{1}'>{0}</td>", formattedValue, GetCellStyle(row, field));
				}
                else if (row.RowType == ReportRowType.FooterRow)
                {
                    Html.AppendFormat("<td class='footerCell' style='{1}'>{0}</td>", row.GetFormattedValue(field), GetCellStyle(row, field));
                }
            }

            Html.AppendLine("</tr>");


        }

        protected virtual string GetCellStyle(ReportRow row, RowField field)
        {
            switch (row.RowType)
            {
                case ReportRowType.HeaderRow:
                    return field.HeaderStyle.ToCss();
                case ReportRowType.DataRow:
                    return field.DataStyle.ToCss();
                default:
                    return field.FooterStyle.ToCss();
            }
        }

        protected virtual void RenderFooter(ReportTextFieldCollection textFields, RenderHintsCollection hints)
        {
            Html.AppendLine("</table>");

            var footerStyle = new ReportStyle { Italic = true };
            Html.AppendFormat("<p style='{1}'>{0}</p>", textFields.Footer, footerStyle.ToCss());

            Html.AppendLine("</div>");

            if (WrapHeadAndBody)
            {
                Html.AppendLine("</body>");
                Html.AppendLine("</html>");
            }
        }


        protected virtual void BuildReportHtml(ReportTextFieldCollection textFields, RenderHintsCollection hints, ReportRowCollection rows)
        {
            RenderHeader(textFields, hints);
            bool isFirstDataRow = true;

            foreach (ReportRow row in rows)
            {

                if (row.RowType == ReportRowType.HeaderRow)
                {
                    Html.AppendLine("<thead>");
                }
                else if (row.RowType == ReportRowType.FooterRow)
                {
                    Html.AppendLine("<tfoot>");
                }
                else if (row.RowType == ReportRowType.DataRow && isFirstDataRow)
                {
                    Html.AppendLine("<tbody>");
                    isFirstDataRow = false;
                }



                RenderRow(row, hints);


                if (row.RowType == ReportRowType.HeaderRow)
                {
                    Html.AppendLine("</thead>");
                }
                else if (row.RowType == ReportRowType.FooterRow)
                {
                    Html.AppendLine("</tfoot>");
                }
            }


            Html.AppendLine("</tbody>");

            RenderFooter(textFields, hints);

        }


        public virtual void WriteReport(Report report, Stream destination)
        {
            BuildReportHtml(report.TextFields, report.RenderHints, report.GetRows());

            var sw = new StreamWriter(destination);
            sw.Write(Html.ToString());
            sw.Flush();
        }

        public virtual void AppendReport(Report source, Report destination)
        {

        }
    }
}