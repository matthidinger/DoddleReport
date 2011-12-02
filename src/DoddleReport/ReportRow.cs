namespace DoddleReport
{
    public class ReportRow
    {
        private readonly FieldDataDictionary _fieldData = new FieldDataDictionary();

        public RowFieldCollection Fields { get; set; }
        public ReportRowType RowType { get; set; }
        public object DataItem { get; set; }

        internal ReportRow(ReportRowType rowType, ReportFieldCollection fields, IReportSource source, object dataItem)
        {
            RowType = rowType;
            DataItem = dataItem;

            Fields = new RowFieldCollection(fields);
            foreach (var field in fields)
            {
                var rowField = new RowField(field);
                var value = source.GetFieldValue(dataItem, field.Name) ?? string.Empty;
                _fieldData[rowField] = value;
            }
        }


        public object this[string fieldName]
        {
            get
            {
                return _fieldData[Fields[fieldName]];
            }
            set { _fieldData[Fields[fieldName]] = value; }
        }

        public object this[RowField field]
        {
            get
            {
                return _fieldData[field];
            }
            set { _fieldData[field] = value; }
        }

        public string GetFormattedValue(RowField field)
        {
            return _fieldData.GetFormattedString(field);
        }
    }
}