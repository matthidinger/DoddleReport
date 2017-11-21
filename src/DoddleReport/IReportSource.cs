using System.Collections;

namespace DoddleReport
{
    public interface IReportSource
    {
        /// <summary>
        /// The fields
        /// </summary>
        /// <returns></returns>
        ReportFieldCollection GetFields();

        /// <summary>
        /// The data items that will be rendered to the report as rows
        /// </summary>
        /// <returns></returns>
        IEnumerable GetItems();

        /// <summary>
        /// Extract the field data from a dataItem
        /// </summary>
        /// <param name="dataItem"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        object GetFieldValue(object dataItem, string fieldName);
    }
}
