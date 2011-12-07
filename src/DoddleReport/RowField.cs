using System;

namespace DoddleReport
{
    /// <summary>
    /// Represents a field within a ReportRow
    /// </summary>
    public class RowField
    {
        public ReportRow Row { get; private set; }

        /// <summary>
        /// The ReportField that this row field is based on
        /// </summary>
        internal ReportField ReportField { get; set; }

        public Report Report
        {
            get { return Row.Report; }
        }

        /// <summary>
        /// Gets the name of the field
        /// </summary>
        public string Name
        {
            get { return ReportField.Name; }
        }

        /// <summary>
        /// It's possible that the DataType could be unique per row if the data is changed in the RenderingRow event
        /// </summary>
        public Type DataType { get; set; }

        public string DataFormatString
        {
            get { return ReportField.DataFormatString; }
        }

        public string HeaderText
        {
            get { return ReportField.HeaderText; }
        }

        public bool Hidden
        {
            get { return ReportField.Hidden; }
        }

        public ReportStyle DataStyle
        {
            get { return ReportField.DataStyle.Copy(); }
        }

        public ReportStyle HeaderStyle
        {
            get { return ReportField.HeaderStyle.Copy(); }
        }

        public ReportStyle FooterStyle
        {
            get { return ReportField.FooterStyle.Copy(); }
        }

        public bool ShowTotals
        {
            get { return ReportField.ShowTotals; }
        }

        public RowField(ReportRow reportRow, ReportField reportField)
        {
            Row = reportRow;
            ReportField = reportField;
            DataType = reportField.DataType;
        }

        public override string ToString()
        {
            return string.Format("{0}", Name);
        }

        public override bool Equals(object obj)
        {
            var field = obj as RowField;
            if (field == null)
                return false;

            return field.ToString().Equals(ToString());
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
}