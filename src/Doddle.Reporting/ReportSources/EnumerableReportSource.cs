using System;
using System.Collections.Generic;
using System.Reflection;
using System.Collections;

namespace Doddle.Reporting
{
    /// <summary>
    /// Generate a Report from any IEnumerable
    /// </summary>
    public class EnumerableReportSource : IReportSource
    {
        public IEnumerable Source { get; set; }

        private IEnumerable<PropertyInfo> GetPropertes()
        {
            var itemType = GetItemType();
            if (itemType == null)
                return null;

            return itemType.GetProperties();
        }

        private Type GetItemType()
        {
            foreach (object i in Source)
            {
                return i.GetType();
            }

            return null;
        }

        public EnumerableReportSource(IEnumerable source)
        {
            Source = source;
        }

        public ReportFieldCollection GetFields()
        {
            var fields = new ReportFieldCollection();
            var properties = GetPropertes();

            if (properties == null)
                return fields;

            foreach (var propInfo in properties)
            {
                fields.Add(propInfo.Name, propInfo.PropertyType);
            }

            return fields;
        }

        public IEnumerable GetItems()
        {
            return Source;
        }

        public object GetFieldValue(object dataItem, string fieldName)
        {
            if (dataItem == null)
                return string.Empty;

            var value = dataItem.GetProperty<object>(fieldName);

            return value;
        }
    }
}
