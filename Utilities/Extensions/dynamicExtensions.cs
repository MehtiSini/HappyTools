using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;

namespace HappyTools.Utilities.Extensions
{

    public static class dynamicExtensions
    {
        public static void AddProperty(this ExpandoObject expando, string propertyName, object propertyValue)
        {
            // ExpandoObject supports IDictionary so we can extend it like this
            var expandoDict = expando as IDictionary<string, object>;
            if (expandoDict.ContainsKey(propertyName))
                expandoDict[propertyName] = propertyValue;
            else
                expandoDict.Add(propertyName, propertyValue);
        }
        public static object GetExpandoPropertyValue(this ExpandoObject expando, string propertyName)
        {
            // ExpandoObject supports IDictionary so we can extend it like this
            var expandoDict = expando as IDictionary<string, object>;
            if (expandoDict.ContainsKey(propertyName))
                return expandoDict[propertyName];
            else
                throw new KeyNotFoundException(propertyName);
        }

        /// <summary>
        /// Create a datatable from a list of <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <param name="data">The list can be created from a dictionary with Dictionary.Values.ToList()</param>
        /// <param name="tableName">Name of the data table</param>
        /// <returns></returns>
        public static DataTable ToDataTable(this IEnumerable<dynamic> data, string tableName)
        {
            return data.ToList().ToDataTable(tableName);
        }
        /// <summary>
        /// Create a datatable from a list of ExpandoObjects
        /// </summary>
        /// <param name="list">The list can be created from a dictionary with Dictionary.Values.ToList()</param>
        /// <param name="tableName">Name of the data table</param>
        /// <returns></returns>
        public static DataTable ToDataTable(this List<ExpandoObject> list, string tableName)
        {

            if (list == null || list.Count == 0)
            {
                return null;
            }

            //build columns
            var props = (IDictionary<string, object>)list[0];
            var t = new DataTable(tableName);
            foreach (var prop in props)
            {
                t.Columns.Add(new DataColumn(prop.Key, prop.Value != null ? prop.Value.GetType() : typeof(string)));
            }
            //add rows
            foreach (var row in list)
            {
                var data = t.NewRow();
                foreach (var prop in (IDictionary<string, object>)row)
                {
                    data[prop.Key] = prop.Value;
                }
                t.Rows.Add(data);
            }
            return t;
        }
    }
}
