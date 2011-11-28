using System;
using System.IO;
using System.Text;

namespace Doddle.Reporting.Writers
{
    public class HtmlReportWriter : IReportWriter
    {
        public const string HtmlStyle = "HtmlStyle";
        public const string HtmlLogo = "HtmlLogo";

        public StringBuilder Html { get; private set; }
        protected virtual bool WrapHeadAndBody { get; set; }


        public HtmlReportWriter()
        {
            Html = new StringBuilder();
            WrapHeadAndBody = true;
        }

        public HtmlReportWriter(bool wrapHeadAndBody)
            : this()
        {
            WrapHeadAndBody = wrapHeadAndBody;
        }

        protected virtual string InternalStyling()
        {
            return string.Empty;
        }

        protected virtual string DefaultStyle
        {
            get
            {
                return @"
                    .htmlReport { font: 10px Verdana; }
                    .htmlReport td { font-size: 10px; }
                    .htmlReport th { font-size: 10px; font-weight: bold; text-decoration: underline; text-align: left; }
                    .htmlReport h1 { font-size: 14pt; margin-bottom: 10px; }";
            }
        }

        protected void AppendStyling(RenderHintsCollection hints)
        {
            Html.AppendLine(@"<style type='text/css'>");

            Html.AppendLine(DefaultStyle);

            Html.AppendFormat("{0}", hints[HtmlStyle]);
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
                Html.AppendFormat("<center><h4 style='margin-bottom: 1px;'>{0}</h4></center>", textFields.Title.FormatHtml());
            }

            if (!string.IsNullOrEmpty(textFields.SubTitle))
            {
                Html.AppendFormat("<center><h5 style='margin-bottom: 3px; margin-top: 1px'>{0}</h5></center>", textFields.SubTitle.FormatHtml());
            }

            if (!string.IsNullOrEmpty(textFields[HtmlLogo]))
            {
                Html.AppendFormat(textFields[HtmlLogo].FormatHtml());
            }

            if (!string.IsNullOrEmpty(textFields.Header))
            {
                Html.AppendFormat("<b>{0}</b><hr />", textFields.Header.FormatHtml());
            }

            Html.AppendLine("<table border='0' cellpadding='2' cellspacing='0' width='100%'>");
        }

        private bool GetBooleanValue(object input)
        {
            if (input == null)
                return false;

            try
            {
                return Convert.ToBoolean(input);
            }
            catch
            {
                if (input.ToString() == "Yes")
                    return true;

                else
                    return false;
            }
        }

        protected virtual void RenderRow(ReportRow row, RenderHintsCollection hints)
        {

            Html.AppendLine("<tr>");

            foreach (RowField field in row.Fields)
            {
                if (row.RowType == ReportRowType.HeaderRow)
                {
                    Html.AppendFormat("<th style='{1}' align='left'>{0}</th>", field.HeaderText, GetCellStyle(row, field));
                }
                else if (row.RowType == ReportRowType.DataRow)
                {
                    if (hints.BooleanCheckboxes)
                    {
                        if (field.DataType == typeof(bool) || field.DataType == typeof(bool?))
                        {
                            string html = "<input type='checkbox'";

                            if (GetBooleanValue(row[field.Name]) == true)
                            {
                                html += " checked='checked'";
                            }

                            html += " />";
                            row[field] = html;
                        }
                    }

                    Html.AppendFormat("<td style='{1}'>{0}</td>", row.GetFormattedValue(field), GetCellStyle(row, field));
                }
                else if (row.RowType == ReportRowType.FooterRow)
                {
                    Html.AppendFormat("<td style='{1}'>{0}</td>", row.GetFormattedValue(field), GetCellStyle(row, field));
                }
            }

            Html.AppendLine("</tr>");


        }

        protected virtual string GetCellStyle(ReportRow row, RowField field)
        {
            if (row.RowType == ReportRowType.HeaderRow)
            {
                return field.HeaderStyle.GetHtml();
            }
            else if (row.RowType == ReportRowType.DataRow)
            {
                return field.DataStyle.GetHtml();
            }
            else
            {
                return field.FooterStyle.GetHtml();
            }
        }

        protected virtual void RenderFooter(ReportTextFieldCollection textFields, RenderHintsCollection hints)
        {
            Html.AppendLine("</table>");

            ReportStyle footerStyle = new ReportStyle { Italic = true };
            Html.AppendFormat("<p style='{1}'>{0}</p>", textFields.Footer, footerStyle.GetHtml());

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

            using (var sw = new StreamWriter(destination))
            {
                sw.Write(Html.ToString());
            }
        }

        public virtual void AppendReport(Report source, Report destination)
        {

        }
    }
}