using System.Data;
using System.Reflection;

namespace CvnetClient.Utils
{
    public class GlobalFunc
    {
        /// <summary>
        /// Convert DataTable to Class (property names like "Col01", "Col02", ...)
        /// </summary> 
        public static List<T> ConvertDataTableToList<T>(DataTable dt) where T : new()
        {
            if (dt == null || dt.Rows.Count == 0) return new List<T>();
            var result = new List<T>();
            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            foreach (DataRow row in dt.Rows)
            {
                var item = new T();
                int colCount = Math.Min(dt.Columns.Count, props.Length);
                for (int i = 0; i < colCount; i++)
                {
                    // Expect property names like "Col01", "Col02", ...
                    string propName = $"Col{(i + 1).ToString("00")}";
                    var prop = props.FirstOrDefault(p => p.Name.Equals(propName, StringComparison.OrdinalIgnoreCase));

                    if (prop != null && prop.CanWrite)
                    {
                        var value = row[i]?.ToString();
                        prop.SetValue(item, value);
                    }
                }
                result.Add(item);
            }
            return result;
        }


        public static List<T> ConvertDataTableToListV2<T>(DataTable table) where T : new()
        {
            var list = new List<T>();

            foreach (DataRow row in table.Rows)
            {
                T obj = new T();
                foreach (DataColumn col in table.Columns)
                {
                    var prop = typeof(T).GetProperty(col.ColumnName);

                    if (prop == null || row[col] == DBNull.Value)
                        continue; 

                    try
                    {
                        var value = row[col];

                        // Handle numeric conversions
                        if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(int?))
                        {
                            if (int.TryParse(value.ToString(), out var intVal))
                                prop.SetValue(obj, intVal);
                            continue;
                        }

                        if (prop.PropertyType == typeof(double) || prop.PropertyType == typeof(double?))
                        {
                            if (double.TryParse(value.ToString(), out var dblVal))
                                prop.SetValue(obj, dblVal);
                            continue;
                        }

                        if (prop.PropertyType == typeof(decimal) || prop.PropertyType == typeof(decimal?))
                        {
                            if (decimal.TryParse(value.ToString(), out var decVal))
                                prop.SetValue(obj, decVal);
                            continue;
                        }

                        if (prop.PropertyType == typeof(bool) || prop.PropertyType == typeof(bool?))
                        {
                            if (bool.TryParse(value.ToString(), out var boolVal))
                                prop.SetValue(obj, boolVal);
                            continue;
                        }

                        if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?))
                        {
                            if (DateTime.TryParse(value.ToString(), out var dtVal))
                                prop.SetValue(obj, dtVal);
                            continue;
                        }

                        // For string or other types
                        prop.SetValue(obj, Convert.ChangeType(value, prop.PropertyType));
                    }
                    catch
                    {
                        // Ignore conversion errors
                        continue;
                    }
                }
                list.Add(obj);
            }

            return list;
        }
    }
}
