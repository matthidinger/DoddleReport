using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace DoddleReport.Builder
{
    public static class DoddleExporter
    {
        /// <summary>
        ///     Converts an enumerable of dictionaries to a report source
        /// </summary>
        /// <typeparam name="TValue">
        ///     The type of values in the dictionaries
        /// </typeparam>
        /// <param name="elements">
        ///     An enumerable of elements
        /// </param>
        /// <param name="select"></param>
        /// <returns>The report source that was created from the elements</returns>
        public static IReportSource ToReportSource<TValue>(IEnumerable<IDictionary<string, TValue>> elements)
        {
            var elementsArray = elements.ToArray();
            if (!elementsArray.Any())
                throw new ArgumentException("Can't export empty list of elements");
            return ToReportSource(elementsArray, elementsArray.First().Keys.ToArray(),
                (element, key) => element.ContainsKey(key) ? element[key] : default(TValue));
        }

        /// <summary>
        /// Converts an enumerable of XElement to a report source
        /// </summary>
        /// <param name="rootElements">
        ///     The xml root elements that contain the values
        /// </param>
        /// <param name="keys">
        ///     They keys that can be used to fetch values from each root element
        /// </param>
        /// <returns>The report source that was created from the elements</returns>
        public static IReportSource ToReportSource(IEnumerable<XElement> rootElements, string[] keys)
        {
            return ToReportSource(rootElements, keys, delegate (XElement element, string key)
            {
                var value = element.Element(XmlConvert.EncodeLocalName(key));
                return value != null ? value.Value : null;
            });
        }

        /// <summary>
        ///     Converts a list of elements to a report source
        /// </summary>
        /// <param name="elements">
        ///     An enumerable of elements
        /// </param>
        /// <param name="keys">
        ///     They keys with which the values can be fetched from one element
        /// </param>
        /// <param name="valueSelector">
        ///     The function with which one value can be fetched given one key and one element
        /// </param>
        /// <returns>The report source that was created from the elements</returns>
        public static IReportSource ToReportSource<T>(IEnumerable<T> elements, string[] keys,
            Func<T, string, object> valueSelector)
        {
            var expandos = new List<ExpandoObject>();
            foreach (var element in elements)
            {
                var expando = new ExpandoObject();
                var expandoDictionary = (IDictionary<string, object>)expando;
                foreach (var key in keys)
                {
                    var value = valueSelector(element, key);
                    expandoDictionary[key] = value;
                }
                expandos.Add(expando);
            }
            return expandos.ToReportSource();
        }
    }
}
