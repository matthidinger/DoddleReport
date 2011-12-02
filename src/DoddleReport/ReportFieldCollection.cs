using System;
using System.Collections.Generic;
using System.Linq;

namespace DoddleReport
{
    /// <summary>
    /// Represents the fields that will be generated onto the report. 
    /// The text to render in the field can be overriden by using the indexer provided.
    /// </summary>
    public class ReportFieldCollection : IEnumerable<ReportField>
    {
        private readonly Dictionary<string, ReportField> _internalFields;

        public ReportFieldCollection()
        {
            _internalFields = new Dictionary<string, ReportField>();
        }

        public void Add(string fieldName, Type dataType)
        {
            Add(new ReportField(fieldName, dataType));
        }

        public void Add(ReportField field)
        {
            _internalFields.Add(field.Name, field);
        }

        // TODO: Support adding fields that do not exist in data source
        //public void Add(string fieldName, Func<ReportField, object> callback)
        //{

        //}

        public ReportField this[string name]
        {
            get
            {
                if (!_internalFields.ContainsKey(name))
                {
                    return null;
                }

                return _internalFields[name];
            }
        }

        public ReportField this[int index]
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

        public int Count
        {
            get { return _internalFields.Count; }
        }


        #region IEnumerable<ReportField> Members

        public IEnumerator<ReportField> GetEnumerator()
        {
            return _internalFields.Values.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _internalFields.Values.GetEnumerator();
        }

        #endregion
    }
}
