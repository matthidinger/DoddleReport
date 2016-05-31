using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DoddleReport
{
    /// <summary>
    /// A report consists of the ReportSource, TextFields, RenderHints that are used by an IReportWriter to render the report
    /// </summary>
    public class Report
    {
        private readonly ReportTextFieldCollection _textFields = new ReportTextFieldCollection();
        private readonly RenderHintsCollection _renderHints = new RenderHintsCollection();
        private readonly Dictionary<RowField, object> _totals = new Dictionary<RowField, object>();
        private readonly Dictionary<Type, object> _lambdas = new Dictionary<Type, object>();

        public Report() : this(null, null)
        {
        }

        /// <summary>
        /// Create a new report by using a specific report source
        /// </summary>
        /// <param name="source">The data for the report</param>
        public Report(IReportSource source) : this(source, null)
        {
            
        }

        /// <summary>
        /// Create a new report by using a specific report source and report writer
        /// </summary>
        /// <param name="source">The data for the report</param>
        /// <param name="writer">The type of writer used to render the report</param>
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

        /// <summary>
        /// The data for the report
        /// </summary>
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

        /// <summary>
        /// The writer that should be used to render the report.
        /// </summary>
        public IReportWriter Writer { get; set; }

        /// <summary>
        /// The columns of data returned from the report source. They may be customized by using the indexer of this property.
        /// </summary>
        public ReportFieldCollection DataFields { get; set; }

        /// <summary>
        /// This event is fired before a row is rendered, allowing some customization of the data
        /// </summary>
        public event EventHandler<ReportRowEventArgs> RenderingRow;

        /// <summary>
        /// Text fields are passed to the report writers to render the data as they see fit
        /// </summary>
        public ReportTextFieldCollection TextFields
        {
            get { return _textFields; }
        }

        /// <summary>
        /// Render hints are passed to each report writer to alter their rendering behavior. Not all render hints are supported in every writer
        /// </summary>
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

        /// <summary>
        /// Write the report to a stream using the specified report writer
        /// </summary>
        /// <param name="destination"></param>
        public void WriteReport(Stream destination)
        {
            if (Source == null)
                throw new InvalidOperationException("You must assign a valid Source before Writing the report");

            if (Writer == null)
                throw new InvalidOperationException("You must assign a valid Writer before Writing the report");

            //AddTotalsIfRowSupports(dataRow, row);

            Writer.WriteReport(this, destination);
        }

        /// <summary>
        /// Append a report to another. This only works for certain report writers and both reports must be using the same report writer.
        /// </summary>
        /// <param name="report"></param>
        public void AppendReport(Report report)
        {
            report.Writer = Writer;
            report.Writer.AppendReport(this, report);
        }

        private void AddFooterRow(ReportRowCollection rows)
        {
            if (_totals.Count == 0) return;

            var footerRow = new ReportRow(this, ReportRowType.FooterRow, null);
            foreach (var total in _totals)
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
                if (field.ShowTotals && field.DataType.IsNumericType())
                {
                    var numericType = field.DataType.IsGenericType ? field.DataType.GetGenericArguments().Single() : field.DataType;
                    var addFieldTotalMethod = typeof(Report).GetMethod("AddFieldTotal", BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(numericType);
                    object value = row[field];
                    if (value as string == string.Empty)
                    {
                        value = null;
                    }

                    addFieldTotalMethod.Invoke(this, new object[] { field, value });
                }
            }
        }

        private void AddFieldTotal<T>(RowField field, Nullable<T> value) where T : struct
        {
            var numericValue = value != null ? value.Value : default(T);
            if (_totals.ContainsKey(field))
            {
                Func<T, T, T> lambda;
                if (!_lambdas.ContainsKey(typeof(T)))
                {
                    var firstParam = Expression.Parameter(typeof(T), "x");
                    var secondParam = Expression.Parameter(typeof(T), "y");
                    lambda = Expression.Lambda<Func<T, T, T>>(Expression.Convert(Expression.Add(firstParam, secondParam), typeof(T)), firstParam, secondParam).Compile();
                    _lambdas.Add(typeof(T), lambda);
                }
                else
                {
                    lambda = (Func<T, T, T>)_lambdas[typeof(T)];
                }

                var currentValue = (T)_totals[field];
                object result = (object)lambda(currentValue, numericValue);
                _totals[field] = result;
            }
            else
            {
                _totals[field] = numericValue;
            }
        }
    }
}