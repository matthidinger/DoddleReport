using System.Collections.Generic;

namespace DoddleReport
{
    internal class RowFieldDataDictionary
    {
        private readonly Dictionary<RowField, object> _internalData = new Dictionary<RowField, object>();

        internal string GetFormattedString(RowField field)
        {
            object internalValue = _internalData[field];

            if (internalValue == null)
                return string.Empty;

            if (internalValue is string && string.IsNullOrEmpty((string)internalValue))
                return string.Empty;


            if (field.DataType == typeof(bool) || field.DataType == typeof(bool?))
            {
                if (field.Report.RenderHints.BooleansAsYesNo)
                {
                    return internalValue as bool? == true ? "Yes" : "No";
                }
            }

            if (field.FormatAsDelegate != null)
            {
                return (string)field.FormatAsDelegate.DynamicInvoke(internalValue);
            }

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
                //if (value != null)
                //{
                //    field.DataType = value.GetType();
                    _internalData[field] = value;
                //}
            }
        }
    }
}