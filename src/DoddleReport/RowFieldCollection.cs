using System.Collections.Generic;
using System.Linq;

namespace DoddleReport
{
    /// <summary>
    /// Represents the fields that will be generated onto the report. 
    /// The text to render in the field can be overriden by using the indexer provided.
    /// </summary>
    public class RowFieldCollection : IEnumerable<RowField>
    {
        private readonly Dictionary<string, RowField> _internalFields;

        public RowFieldCollection(ReportRow row)
        {
            _internalFields = new Dictionary<string, RowField>();

            foreach (var reportField in row.Report.DataFields)
            {
                _internalFields.Add(reportField.Name, new RowField(row, reportField));
                //if (!reportField.Hidden)
                //{
                //    _internalFields.Add(reportField.Name, new RowField(row, reportField));
                //}
            }
        }

        public RowField this[string name]
        {
            get
            {
                return _internalFields[name];
            }
        }

        public RowField this[int index]
        {
            get
            {
                return _internalFields.Values.ElementAt(index);
            }
        }

        public bool Contains(string name)
        {
            return _internalFields.ContainsKey(name);
        }


        public IEnumerator<RowField> GetEnumerator()
        {
            return _internalFields.Values.Where(f => f.Hidden == false).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
