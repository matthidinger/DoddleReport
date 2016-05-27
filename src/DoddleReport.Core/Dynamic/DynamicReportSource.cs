using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace DoddleReport
{
    public static class DynamicReportBuilder
    {
        public static IReportSource ToReportSource(this IEnumerable<ExpandoObject> source)
        {
            return new DynamicReportSource(source);
        }
    }


    /// <summary>
    /// Generate a Report for a collection of dynamic ExpandoObjects
    /// </summary>
    public class DynamicReportSource : IReportSource
    {
        private readonly IEnumerable<ExpandoObject> _source;

        public DynamicReportSource(IEnumerable<ExpandoObject> source)
        {
            _source = source;
        }

        public ReportFieldCollection GetFields()
        {
            var fields = new ReportFieldCollection();
            var item = _source.FirstOrDefault();

            if (item == null)
                return fields;

            foreach (var t in item)
            {
                fields.Add(t.Key, (t.Value ?? new object()).GetType());
            }

            return fields;
        }

        public IEnumerable GetItems()
        {
            return _source;
        }

        public object GetFieldValue(object dataItem, string fieldName)
        {
            if (dataItem == null)
                return string.Empty;

            var di = (IDictionary<string, object>)dataItem;
            return di[fieldName];
        }
    }
}