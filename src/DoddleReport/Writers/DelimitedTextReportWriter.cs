using System;
using System.Text;
using System.IO;

namespace DoddleReport.Writers
{
    // TODO: Need to properly encode characters like double quote
    public class DelimitedTextReportWriter : IReportWriter
    {
        /// <summary>
        /// Override this property to change the default delimiter for all delimited-text reports
        /// </summary>
        public static string DefaultDelimiter = TabDelimiter;

        public const string TabDelimiter = "\t";
        public const string CommaDelimiter = ",";

        public const string DelimiterHint = "Delimiter";
        public const string IncludeHeaderHint = "IncludeHeader";
        public const string EncloseInQuotes = "EncloseInQuotes";

        /// <summary>
        /// Use this delegate to customize the way headers are formatted on the report. The default is to remove spaces and make upper case
        /// </summary>
        public static Func<RowField, string> GetHeaderText = field => field.HeaderText.Replace(" ", "").ToUpper();


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
                            builder.AppendFormat("{0}{1}", GetHeaderText(field), delimiter);
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

        public void AppendReport(Report source, Report destination)
        {

        }
    }
}
