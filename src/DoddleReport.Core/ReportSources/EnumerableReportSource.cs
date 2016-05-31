using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using System.Reflection;

namespace DoddleReport.ReportSources
{
    /// <summary>
    /// Generate a Report from any IEnumerable
    /// </summary>
    public class EnumerableReportSource : IReportSource
    {
        private const int MaxColumn = 10000;

        public IEnumerable Source { get; set; }


        private Type GetItemType()
        {
            // If the list implements IEnumerable<T> get the type from the generic
            // declaration so that we don't query the a query that could take
            // some time to connect
            var itemType = (from i in Source.GetType().GetInterfaces()
                            where i._IsGenericType() &&
                                  i.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                            select i.GetGenericArguments().Single()).SingleOrDefault();


            // If the itemType is Object, we assume they are using an anonymous type
            // and need to get the type of the first item, instead of the IEnumerable
            if (itemType != null && itemType != typeof(object))
            {
                return itemType;
            }

            var firstItem = Source.OfType<object>().FirstOrDefault();
            return firstItem != null ? firstItem.GetType() : null;
        }

        public EnumerableReportSource(IEnumerable source)
        {
            Source = source;
        }

        public ReportFieldCollection GetFields()
        {
            var fields = new ReportFieldCollection();
            var itemType = GetItemType();
            var properties = itemType.GetProperties();

            if (properties == null)
                return fields;

            var fieldsOrder = new Dictionary<ReportField, int>();
            foreach (var propInfo in properties)
            {
                var reportField = new ReportField(propInfo.Name, propInfo.PropertyType);
                var order = SetReportFieldProperties(itemType, propInfo, reportField);
                fieldsOrder.Add(reportField, order);
            }

            // Add the fields with the requested order
            foreach (var field in fieldsOrder.OrderBy(kv => kv.Value).Select(kv => kv.Key))
            {
                fields.Add(field);
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

        /// <summary>
        /// Sets the report field properties based on the item type attributes (data annotations).
        /// </summary>
        /// <param name="itemType">Type of the item.</param>
        /// <param name="propInfo">The prop info.</param>
        /// <param name="reportField">The report field.</param>
        /// <returns>The order number for the field.</returns>
        private static int SetReportFieldProperties(Type itemType, PropertyInfo propInfo, ReportField reportField)
        {
            // TODO: Light up MetadataTypeAttribute
            //var metadataTypeAttribute = itemType.GetTypeInfo().GetAttribute<MetadataTypeAttribute>();
            MemberInfo memberInfo;
            //if (metadataTypeAttribute != null)
            //{
            //    memberInfo = itemType.GetProperty(propInfo.Name, BindingFlags.Public | BindingFlags.Instance) ??
            //                 (MemberInfo) itemType.GetField(propInfo.Name, BindingFlags.Public | BindingFlags.Instance);
            //}
            //else
            //{
                memberInfo = propInfo;
            //}

            if (memberInfo != null)
            {
                var dataTypeAttribute = memberInfo.GetAttribute<DataTypeAttribute>();
                var displayFormatAttribute = dataTypeAttribute != null ? dataTypeAttribute.DisplayFormat : memberInfo.GetAttribute<DisplayFormatAttribute>();
                if (displayFormatAttribute != null)
                {
                    reportField.FormatAs<object>(o => o != null ? string.Format(displayFormatAttribute.DataFormatString, o) : displayFormatAttribute.NullDisplayText);
                }

                // TODO: Wire up DisplayNameAttribute
                //var displayNameAttribute = memberInfo.GetAttribute<DisplayNameAttribute>();
                //if (displayNameAttribute != null)
                //{
                //    reportField.HeaderText = displayNameAttribute.DisplayName;
                //}

#if !NET35
                var displayAttribute = memberInfo.GetAttribute<DisplayAttribute>();
                if (displayAttribute != null)
                {
                    reportField.HeaderText = displayAttribute.GetShortName();
                    return displayAttribute.GetOrder() ?? MaxColumn;
                }
#endif
            }

            return MaxColumn;
        }
    }
}
