using System.Collections;
using System.Collections.Generic;

namespace Doddle.Reporting.SharePoint
{
    public class SPListReportSource : IReportSource
    {
        public SPList List { get; set; }
        public SPQuery Query { get; set; }
        protected List<SPField> CustomFields { get; set; }

        public SPListReportSource(string listName)
        {
            List = SharePointUtility.CurrentWeb.Lists[listName];
            CustomFields = ListUtility.GetCustomFields(List);
        }

        public ReportFieldCollection GetFields()
        {
            var fields = new ReportFieldCollection();
            foreach (SPField field in CustomFields)
            {
                fields.Add(field.Title, field.FieldValueType);
            }
            return fields;
        }

        public IEnumerable GetItems()
        {
            SPListItemCollection items;

            if (Query != null)
            {
                items = List.GetItems(Query);
            }
            else
            {
                items = List.Items;
            }

            return items;
        }

        public object GetFieldValue(object dataItem, string fieldName)
        {
            if (dataItem == null)
                return string.Empty;

            SPListItem listItem = dataItem as SPListItem;

            if ((listItem == null) || (!listItem.Fields.ContainsField(fieldName)))
                return string.Empty;

            return listItem.GetFormattedValue(fieldName);
        }
    }
}
