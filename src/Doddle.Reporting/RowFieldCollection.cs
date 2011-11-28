using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Doddle.Reporting
{
    /// <summary>
    /// Represents the fields that will be generated onto the report. 
    /// The text to render in the field can be overriden by using the indexer provided.
    /// </summary>
    public class RowFieldCollection : IEnumerable<RowField>
    {
        private Dictionary<string, RowField> _internalFields;

        public RowFieldCollection(ReportFieldCollection reportFields)
        {
            _internalFields = new Dictionary<string, RowField>();

            foreach (ReportField field in reportFields)
            {
                _internalFields.Add(field.Name, new RowField(field));
                //if (field.Hidden == false)
                //{
                //    _internalFields.Add(field.Name, new RowField(field));
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


        #region IEnumerable<ReportField> Members

        public IEnumerator<RowField> GetEnumerator()
        {
            return _internalFields.Values.Where(f => f.Hidden == false).GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
