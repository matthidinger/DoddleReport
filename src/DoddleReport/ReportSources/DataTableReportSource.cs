using System.Data;

namespace DoddleReport.ReportSources
{
    public class DataTableReportSource : IReportSource
    {
        private readonly DataTable _table;

        public DataTableReportSource(DataTable table)
        {
            _table = table;
        }

        public ReportFieldCollection GetFields()
        {
            var fields = new ReportFieldCollection();
            foreach (DataColumn column in _table.Columns)
            {
                fields.Add(column.ColumnName, column.DataType);
            }
            return fields;
        }

        public System.Collections.IEnumerable GetItems()
        {
            return _table.Rows;
        }

        public object GetFieldValue(object dataItem, string fieldName)
        {
            var row = dataItem as DataRow;
            if (row == null)
                return null;

            return row[fieldName];
        }
    }
}
