using System;

namespace DoddleReport
{
    public class ReportField
    {
        /// <summary>
        /// Gets the name of the field
        /// </summary>
        public string Name { get; private set; }

        public Type DataType { get; set; }

        public bool Hidden { get; set; }

        public string HeaderText { get; set; }
        public string FooterText { get; set; }

        public bool ShowTotals { get; set; }

        public string DataFormatString { get; set; }

        public ReportStyle DataStyle { get; private set; }
        public ReportStyle HeaderStyle { get; private set;  }
        public ReportStyle FooterStyle { get; private set; }
    
        public ReportField(string columnName) 
            : this(columnName, typeof(object)) {}

        public ReportField(string columnName, Type dataType)
        {
            Name = columnName;
            DataType = dataType;
            Hidden = false;
            HeaderText = Name.SplitUpperCaseToString();
            ShowTotals = false;
            DataFormatString = "{0}";

            DataStyle = new ReportStyle(ReportRowType.DataRow);
            HeaderStyle = new ReportStyle(ReportRowType.HeaderRow);
            FooterStyle = new ReportStyle(ReportRowType.FooterRow);
           
        }

        public override string ToString()
        {
            return string.Format("{0}", Name);
        }

        public override bool Equals(object obj)
        {
            ReportField field = obj as ReportField;
            if (field == null)
                return false;

            return field.ToString().Equals(this.ToString());
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
    }
}