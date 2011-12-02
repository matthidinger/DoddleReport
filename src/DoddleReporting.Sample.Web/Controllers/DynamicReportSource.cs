using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace Doddle.Reporting
{
    // By using a generic class we can take advantage 
    // of the fact that .NET will create a new generic type 
    // for each type T. This allows us to avoid creating 
    // a dictionary of Dictionary<string, PropertyInfo> 
    // for each type T. We also avoid the need for the  
    // lock statement with every call to Map. 
    public static class Mapper<T>
        // We can only use reference types 
        where T : class
    {
        private static readonly Dictionary<string, PropertyInfo> _propertyMap;

        static Mapper()
        {
            // At this point we can convert each 
            // property name to lower case so we avoid  
            // creating a new string more than once. 
            _propertyMap =
                typeof(T)
                .GetProperties()
                .ToDictionary(
                    p => p.Name.ToLower(),
                    p => p
                );
        }

        public static void Map(ExpandoObject source, T destination)
        {
            // Might as well take care of null references early. 
            if (source == null)
                throw new ArgumentNullException("source");
            if (destination == null)
                throw new ArgumentNullException("destination");

            // By iterating the KeyValuePair<string, object> of 
            // source we can avoid manually searching the keys of 
            // source as we see in your original code. 
            foreach (var kv in source)
            {
                PropertyInfo p;
                if (_propertyMap.TryGetValue(kv.Key.ToLower(), out p))
                {
                    var propType = p.PropertyType;
                    if (kv.Value == null)
                    {
                        if (!propType.IsByRef && propType.Name != "Nullable`1")
                        {
                            // Throw if type is a value type  
                            // but not Nullable<> 
                            throw new ArgumentException("not nullable");
                        }
                    }
                    else if (kv.Value.GetType() != propType)
                    {
                        // You could make this a bit less strict  
                        // but I don't recommend it. 
                        throw new ArgumentException("type mismatch");
                    }
                    p.SetValue(destination, kv.Value, null);
                }
            }
        }
    }

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
                fields.Add(t.Key, t.Value.GetType());
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