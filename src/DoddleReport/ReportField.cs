using System;

namespace DoddleReport
{
    /// <summary>
    /// Represents a data field within a Report
    /// </summary>
    public class ReportField
    {
        private string _dataFormatString;

        /// <summary>
        /// Gets the name of the field
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The type of data contained within this field
        /// </summary>
        public Type DataType { get; set; }

        /// <summary>
        /// If true, the field will not be displayed on the report
        /// </summary>
        public bool Hidden { get; set; }

        /// <summary>
        /// The header displayed for this field of data
        /// </summary>
        public string HeaderText { get; set; }

        /// <summary>
        /// The footer displayed for this field of data
        /// </summary>
        public string FooterText { get; set; }

        /// <summary>
        /// If true, in some cases (like numeric fields), we can automatically total up the data and render it in the footer of the column
        /// </summary>
        public bool ShowTotals { get; set; }

        /// <summary>
        /// A format string to customize how the data is displayed. For example, use "{0:c}" for currency. This property cannot be used in conjunction with the FormatDataAs method.
        /// </summary>
        public string DataFormatString
        {
            get
            {
                return _dataFormatString;
            }

            set
            {
                _dataFormatString = value;
                FormatAsDelegate = null;
            }
        }

        /// <summary>
        /// Customize how the data for this field is rendered
        /// </summary>
        public ReportStyle DataStyle { get; set; }

        /// <summary>
        /// Customize how the header for this field is rendered
        /// </summary>
        public ReportStyle HeaderStyle { get; private set;  }

        /// <summary>
        /// Customize how the footer for this field is rendered
        /// </summary>
        public ReportStyle FooterStyle { get; private set; }

		internal Delegate FormatAsDelegate { get; private set; }

		internal Delegate UrlDelegate { get; private set; }
        public string ExcelFormula { get; set; }

        public ReportField(string fieldName) 
            : this(fieldName, typeof(object)) {}

        public ReportField(string fieldName, Type dataType)
        {
            Name = fieldName;
            DataType = dataType;
            Hidden = false;
            HeaderText = Name.SplitUpperCaseToString();
            ShowTotals = false;
            DataFormatString = "{0}";

            DataStyle = new ReportStyle(ReportRowType.DataRow);
            HeaderStyle = new ReportStyle(ReportRowType.HeaderRow);
            FooterStyle = new ReportStyle(ReportRowType.FooterRow);
        }

        /// <summary>
        /// Use this method for advanced formatting of this field using a callback. This method cannot be used in conjunction with DataFormatString
        /// </summary>
        /// <typeparam name="T">The type of data contained within this field</typeparam>
        /// <param name="formatAsDelegate">A callback used to take the data item allowing you to specify how to render it as a string</param>
        public void FormatAs<T>(Func<T, string> formatAsDelegate)
        {
            FormatAsDelegate = formatAsDelegate;
        }

		/// <summary>
		/// A delegate to generate a uri/url to be used as the href attribute for a link in the given field.
		/// </summary>
		/// <typeparam name="T">The type of the DataItem of the current row</typeparam>
		/// <param name="hrefLambda">A function that consumes the curent DataItem to create a string representation of a url</param>
		public void Url<T>(Func<T, string> hrefLambda)
		{
			UrlDelegate = hrefLambda;
		}

        public override string ToString()
        {
            return string.Format("{0}", Name);
        }

        public override bool Equals(object obj)
        {
            var field = obj as ReportField;
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