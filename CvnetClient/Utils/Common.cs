using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
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

        public static async Task<bool> WaitForPdfAsync(string url, TimeSpan timeout)
        {
            using var httpClient = new HttpClient();
            var start = DateTime.Now;

            while (DateTime.Now - start < timeout)
            {
                try
                {
                    var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
                    if (response.IsSuccessStatusCode)
                        return true; 
                }
                catch
                {
                   
                }

                await Task.Delay(1000);
            }

            return false; 
        }

        public static void ChkDataTableColType(DataTable csv_data)
        {
            if (csv_data == null || csv_data.Columns.Count == 0) return;

            bool hasRow = csv_data.Rows.Count > 0;
            int index = 0;
            foreach (DataColumn column in csv_data.Columns)
            {
                Type t = column.DataType;
                string recommend_type = "";
                if (t.Name == typeof(int).Name)
                    recommend_type = "int";
                else if (t.Name == typeof(short).Name)
                    recommend_type = "short";
                else if (t.Name == typeof(long).Name)
                    recommend_type = "long";
                else if (t.Name == typeof(float).Name)
                    recommend_type = "float";
                else if (t.Name == typeof(double).Name)
                    recommend_type = "double";
                else if (t.Name == typeof(decimal).Name)
                    recommend_type = "decimal";
                else if (t.Name == typeof(string).Name)
                    recommend_type = "string";
                else if (t.Name == typeof(bool).Name)
                    recommend_type = "bool";
                else if (t.Name == typeof(DateTime).Name)
                    recommend_type = "DateTime";
                else if (t.Name == typeof(byte[]).Name)
                    recommend_type = "byte[]";
                else recommend_type = "object";

                object firstRowValue = hasRow ? csv_data.Rows[0][column] : null;
                string firstRowText = (firstRowValue == DBNull.Value || firstRowValue == null) ? "NULL" : firstRowValue.ToString();

                Debug.WriteLine(string.Format("Index: {0} | ColumnName: {1} | DataType: {2} | RecommendType: {3} | FirtRowValue: {4} |", 
                                               index, column.ColumnName, column.DataType, recommend_type, firstRowText));
                index++;
            }
        }

        public static double RoundUp(double value, int decimals = 0)
        {
            double factor = Math.Pow(10, decimals);
            return Math.Ceiling(value * factor) / factor;
        }

        public static double RoundDown(double value, int decimals = 0)
        {
            double factor = Math.Pow(10, decimals);
            if (value >= 0)
                //return Math.Ceiling(value * factor) / factor;   // positive → round up
                return Math.Floor(value * factor) / factor;
            else
                return Math.Floor(value * factor) / factor;     // negative → round down (away from zero)
        }
    }
}
