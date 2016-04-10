namespace DoddleReport
{
    public class ReportRow
    {
        private readonly RowFieldDataDictionary _rowFieldData = new RowFieldDataDictionary();

        public RowFieldCollection Fields { get; private set; }
        public Report Report { get; private set; }
        public ReportRowType RowType { get; private set; }
        public object DataItem { get; private set; }

        internal ReportRow(Report report, ReportRowType rowType, object dataItem)
        {
            Report = report;
            RowType = rowType;
            DataItem = dataItem;

            Fields = new RowFieldCollection(this);
            foreach (var field in report.DataFields)
            {
                var rowField = new RowField(this, field);
                var value = report.Source.GetFieldValue(dataItem, field.Name) ?? string.Empty;
                _rowFieldData[rowField] = value;
            }
        }


        public object this[string fieldName]
        {
            get
            {
                return _rowFieldData[Fields[fieldName]];
            }
            set { _rowFieldData[Fields[fieldName]] = value; }
        }

        public object this[RowField field]
        {
            get
            {
                return _rowFieldData[field];
            }
            set { _rowFieldData[field] = value; }
        }

        public string GetFormattedValue(RowField field)
        {

            return _rowFieldData.GetFormattedString(field);
        }

		public string GetUrlString(RowField field)
		{
			return _rowFieldData.GetUrlString(field);
		}
    }
}