using System;
using System.Text;
using System.IO;

namespace Doddle.Reporting.Writers
{
    // TODO: Need to properly encode characters like double quote
    public class DelimitedTextReportWriter : IReportWriter
    {
        public const string DefaultDelimiter = TabDelimiter;
        public const string TabDelimiter = "\t";
        public const string CommaDelimiter = ",";

        public const string DelimiterHint = "Delimiter";
        public const string IncludeHeaderHint = "IncludeHeader";
        public const string EncloseInQuotes = "EncloseInQuotes";

        public void WriteReport(Report report, Stream destination)
        {
            bool includeHeader = report.RenderHints[IncludeHeaderHint] as bool? ?? true;
            string delimiter = report.RenderHints[DelimiterHint] as string ?? DefaultDelimiter;


            var builder = new StringBuilder();

            foreach (ReportRow row in report.GetRows())
            {
                foreach (RowField field in row.Fields)
                {
                    if (row.RowType == ReportRowType.HeaderRow)
                    {
                        if (includeHeader)
                        {
                            builder.AppendFormat("{0}{1}", FormattedHeaderText(field), delimiter);
                        }
                    }
                    else if (row.RowType == ReportRowType.DataRow)
                    {
                        builder.AppendFormat("{0}{1}", GetRowDataFormatted(row, field, report.RenderHints), delimiter);
                    }
                }

                builder.Remove(builder.Length - 1, 1);

                builder.Append(Environment.NewLine);
            }

            var sw = new StreamWriter(destination);
            sw.Write(builder);
            sw.Flush();
        }

        private static string GetRowDataFormatted(ReportRow row, RowField field, RenderHintsCollection hints)
        {
            bool encloseInQuotes = hints[EncloseInQuotes] as bool? ?? true;

            if (encloseInQuotes)
            {
                return string.Format("\"{0}\"", row.GetFormattedValue(field));
            }

            return row.GetFormattedValue(field);
        }

        protected virtual string FormattedHeaderText(RowField field)
        {
            return field.HeaderText.Replace(" ", "").ToUpper();
        }

        public void AppendReport(Report source, Report destination)
        {

        }
    }
}
