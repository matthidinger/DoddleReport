using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DoddleReport
{
    public class Report
    {
        private readonly ReportTextFieldCollection _textFields = new ReportTextFieldCollection();
        private readonly RenderHintsCollection _renderHints = new RenderHintsCollection();
        readonly Dictionary<RowField, decimal> _decTotals = new Dictionary<RowField, decimal>();

        public Report() : this(null, null)
        {
        }

        public Report(IReportSource source) : this(source, null)
        {
            
        }

        public Report(IReportSource source, IReportWriter writer)
        {
            if (source != null)
            {
                _source = source;
                DataFields = source.GetFields() ?? new ReportFieldCollection();
            }

            Writer = writer;
        }

        private IReportSource _source;
        public IReportSource Source
        {
            get
            {
                return _source;
            }
            set
            {
                _source = value;
                DataFields = _source.GetFields();
            }
        }
        public IReportWriter Writer { get; set; }

        public ReportFieldCollection DataFields { get; set; }


        public event EventHandler<ReportRowEventArgs> RenderingRow;


        public ReportTextFieldCollection TextFields
        {
            get { return _textFields; }
        }

        public RenderHintsCollection RenderHints
        {
            get { return _renderHints; }
        }

        protected virtual void OnRowRendering(ReportRowEventArgs e)
        {
            var handler = RenderingRow;
            if (handler != null)
            {
                handler(this, e);
            }
        }


        public virtual ReportRowCollection GetRows()
        {
            var rows = new ReportRowCollection(this);
            rows.RowAdding += RenderingRow;
            
            var headerRow = new ReportRow(this, ReportRowType.HeaderRow, null);
            rows.Add(headerRow);

            foreach (var dataItem in Source.GetItems())
            {
                var row = new ReportRow(this, ReportRowType.DataRow, dataItem);
                AddTotalsIfRowSupports(row);
                rows.Add(row);
            }

            AddFooterRow(rows);

            return rows;
        }

        public void WriteReport(Stream destination)
        {
            if (Source == null)
                throw new InvalidOperationException("You must assign a valid Source before Writing the report");

            if (Writer == null)
                throw new InvalidOperationException("You must assign a valid Writer before Writing the report");

            //AddTotalsIfRowSupports(dataRow, row);

            Writer.WriteReport(this, destination);
        }

        public void AppendReport(Report report)
        {
            report.Writer = Writer;
            report.Writer.AppendReport(this, report);
        }

        private void AddFooterRow(ReportRowCollection rows)
        {
            if (_decTotals.Count == 0) return;

            var footerRow = new ReportRow(this, ReportRowType.FooterRow, null);
            foreach (var total in _decTotals)
            {
                footerRow[total.Key] = string.Format(total.Key.DataFormatString, total.Value);
            }

            foreach (var field in DataFields.Where(field => !string.IsNullOrEmpty(field.FooterText)))
            {
                footerRow[field.Name] = field.FooterText;
            }

            rows.Add(footerRow);
        }

        private void AddTotalsIfRowSupports(ReportRow row)
        {
            foreach (var field in row.Fields)
            {
                if (field.ShowTotals)
                {
                    decimal value = Convert.ToDecimal(row[field]);
                    if (_decTotals.ContainsKey(field))
                        _decTotals[field] += value;
                    else
                        _decTotals[field] = value;
                }

                //// TODO: Fix totals for all numeric types
                //if (field.DataType == typeof(decimal) || field.DataType == typeof(int) || field.DataType == typeof(short))
                //{


                //}
            }
        }

        public object GetFieldValue(object dataItem, string fieldName)
        {
            if(Source == null)
                throw new InvalidOperationException("The Source for this report has not been set");

            return Source.GetFieldValue(dataItem, fieldName);
        }
    }
}