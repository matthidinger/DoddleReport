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
        public IEnumerable Source { get; set; }

        private IEnumerable<PropertyInfo> GetProperties()
        {
            var itemType = GetItemType();
            if (itemType == null)
                return null;

            return itemType.GetProperties();
        }

        private Type GetItemType()
        {
            // If the list implements IEnumerable<T> get the type from the generic
            // declaration so that we don't query the a query that could take
            // some time to connect
            var itemType = (from i in Source.GetType().GetInterfaces()
                            where i.IsGenericType &&
                                  i.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                            select i.GetGenericArguments().Single()).SingleOrDefault();
            if (itemType != null)
            {
                return itemType;
            }

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
            var properties = GetProperties();
            var itemType = GetItemType();

            if (properties == null)
                return fields;

            foreach (var propInfo in properties)
            {
                var reportField = new ReportField(propInfo.Name, propInfo.PropertyType);
                SetReportFieldProperties(itemType, propInfo, reportField);
                fields.Add(reportField);
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
        private static void SetReportFieldProperties(Type itemType, PropertyInfo propInfo, ReportField reportField)
        {
            var metadataTypeAttribute = itemType.GetCustomAttributes(typeof(MetadataTypeAttribute), false).Cast<MetadataTypeAttribute>().SingleOrDefault();
            MemberInfo memberInfo;
            if (metadataTypeAttribute != null)
            {
                memberInfo = itemType.GetProperty(propInfo.Name, BindingFlags.Public | BindingFlags.Instance) as MemberInfo;
                if (memberInfo == null)
                {
                    memberInfo = itemType.GetField(propInfo.Name, BindingFlags.Public | BindingFlags.Instance);
                }
            }
            else
            {
                memberInfo = propInfo;
            }

            if (memberInfo != null)
            {
                var dataTypeAttribute = memberInfo.GetCustomAttributes(typeof(DataTypeAttribute), false).Cast<DataTypeAttribute>().SingleOrDefault();
                DisplayFormatAttribute displayFormatAttribute;
                if (dataTypeAttribute != null)
                {
                    displayFormatAttribute = dataTypeAttribute.DisplayFormat;
                }

                displayFormatAttribute = memberInfo.GetCustomAttributes(typeof(DisplayFormatAttribute), false).Cast<DisplayFormatAttribute>().SingleOrDefault();
                if (displayFormatAttribute != null)
                {
                    reportField.FormatAs<object>(o => o != null ? string.Format(displayFormatAttribute.DataFormatString, o) : displayFormatAttribute.NullDisplayText);
                }

#if NET35
                var displayNameAttribute = memberInfo.GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().SingleOrDefault();
                if (displayNameAttribute != null)
                {
                    reportField.HeaderText = displayNameAttribute.DisplayName;
                }
#else
                var displayAttribute = memberInfo.GetCustomAttributes(typeof(DisplayAttribute), false).Cast<DisplayAttribute>().SingleOrDefault();
                if (displayAttribute != null)
                {
                    reportField.HeaderText = displayAttribute.GetShortName();
                }
#endif
            }
        }
    }
}
