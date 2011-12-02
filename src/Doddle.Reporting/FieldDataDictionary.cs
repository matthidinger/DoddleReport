using System.Collections.Generic;

namespace Doddle.Reporting
{
    internal class FieldDataDictionary
    {
        private readonly Dictionary<RowField, object> _internalData = new Dictionary<RowField, object>();

        internal string GetFormattedString(RowField field)
        {
            object internalValue = _internalData[field];

            if (internalValue == null)
                return string.Empty;

            if (internalValue is string && string.IsNullOrEmpty((string)internalValue))
                return string.Empty;

            return string.Format(field.DataFormatString, internalValue);
        }

        public object this[RowField field]
        {
            get
            {
                return _internalData[field];
            }
            set
            {
                if (value != null)
                {
                    field.DataType = value.GetType();
                    _internalData[field] = value;
                }
            }
        }
    }
}