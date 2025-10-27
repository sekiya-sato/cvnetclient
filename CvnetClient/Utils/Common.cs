using System.Data;
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
    }
}
