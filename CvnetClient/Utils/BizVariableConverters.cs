using System.Data;
using System.Net.Http;
using System.Dynamic;
using System.IO; 
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using CvnetClient.ViewModels;

namespace CvnetClient.Utils
{
    /// <summary>
    /// BizV Array variable convert to c#
    /// </summary>
    public class BizArray
    { 
        List<string> list;
        public int Count => (list != null) ? list.Count : 0;

        // 🔹 Indexer for array-like access
        public string this[int index]
        {
            get => Get(index);
            set => Set(index, value);
        }

        public BizArray() 
        {
            list = new List<string>();
        }

        public BizArray(string[] wrk_para)
        {
            list = new List<string>();
            foreach (string wrk in wrk_para)
            {
                list.Add(wrk);
            }
        }

        public void Add(string value)
        {
            list.Add(value);
        }

        public void Set(int index, string value)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), "Index cannot be negative.");

            while (list.Count <= index)
            {
                list.Add(value);
            }
            list[index] = value;
        }

        public string? Get(int index)
        {
            if (index < 0 || index >= list.Count)
                return null; //throw new IndexOutOfRangeException($"Index {index} is out of range. Current size: {list.Count}");
            return (index < list.Count) ? list[index] : null;
        }

        public string[] ToArray()
        {
            return list.ToArray();
        }

        public void Clear()
        {
            list.Clear();
        }
    }

    public class BizCsvDocument
    {
        public DataTable csv_table { get; set; }

        #region DataTable Features
        public BizCsvDocument() {
            csv_table = new DataTable(); 
        }

        public BizCsvDocument(DataTable csv_doc) {
            this.csv_table = csv_doc;
        }

        /// <summary>
        /// Convert Aspx string table to DataTable Type
        /// </summary>
        /// <param name="csv_text">
        /// Example input string as below:
        /// line0, line1, line2, line3 \n
        /// line0, line1, line2, line3
        /// </param>
        /// <param name="head_flag">0 Without Column Header, 1 With Column Header</param>
        public BizCsvDocument(string csv_text, int head_flag = 0)
        {
            ConvertStrToTable(csv_text, head_flag);
        }

        private void ConvertStrToTable(string csv_text, int head_flag = 0)
        {
            var dt = new DataTable();
            this.csv_table = dt;
            if (string.IsNullOrEmpty(csv_text)) return;

            // Convert escaped sequences to real characters
            string clean = csv_text
                .Replace("\\\"", "\"")   // unescape quotes
                .Replace("\\r\\n", "\r\n"); // unescape newlines
            // Remove wrapping quotes if present (e.g.  "社員マスタ" → 社員マスタ)
            csv_text = clean.Replace("\"", string.Empty);

            // Split by line
            var lines = csv_text
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            if (lines.Length == 0) return;

            // Split first line by comma
            var firstLine = lines[0].Split(',');

            // If has header, use it as column names
            if (head_flag == 1)
            {
                foreach (var colName in firstLine)
                    dt.Columns.Add(colName.Trim());
            }
            else
            {
                // Generate default column names: Col0, Col1, ...
                for (int i = 0; i < firstLine.Length; i++)
                    dt.Columns.Add($"Col{i}");

                // Add first line as data row (since it's not header)
                dt.Rows.Add(firstLine.Select(x => x.Trim()).ToArray());
            }

            // Start from next line if header exists, else from line 1
            int startIndex = (head_flag == 1) ? 1 : 0;
            for (int i = startIndex; i < lines.Length; i++)
            {
                var values = lines[i].Split(',');
                // Pad or trim to match column count
                var rowValues = new object[dt.Columns.Count];
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    rowValues[j] = j < values.Length ? values[j].Trim() : string.Empty;
                }
                dt.Rows.Add(rowValues);
            }
            this.csv_table = dt;
        }
         
        public DataTable GetTable()
        {
            return csv_table;
        }

        public void Clear()
        {
            if (csv_table != null) csv_table.Clear();
            else csv_table = new DataTable();
        }

        /// <summary>
        /// Set each DataTable Column Name by split the string value 
        /// </summary>
        /// <param name="colName">ColName1,ColName2,ColName3</param>
        public bool SetColHeader(string colName)
        {
            bool isUpdate = false;
            if (string.IsNullOrEmpty(colName) || csv_table?.Columns?.Count == 0) return isUpdate;

            // Clear All Column Name
            clearColHeader();

            var columns = colName
                                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(x => x.Trim())  // Trim spaces like " Col1 " → "Col1"
                                .Where(x => x.Length > 0) // Filter out accidental empty values
                                .ToList();
              
            int i = 0;
            foreach (DataColumn col in csv_table.Columns)
            {
                if (i >= columns.Count) break;
                col.ColumnName = columns[i++] ?? string.Empty;
            }
            return isUpdate = true;
        }

        /// <summary>
        /// Set each DataTable Column Name by string array 
        /// </summary>
        public void SetColHeader(string[] colName)
        { 
            if (colName == null || colName.Length == 0 || csv_table?.Columns?.Count == 0) return;

            // Clear All Column Name
            clearColHeader();

            // Set New Column Name
            int i = 0;
            foreach (DataColumn col in csv_table.Columns)
            {
                if (i >= colName.Length) break;
                col.ColumnName = colName[i++] ?? string.Empty;
            }
        }

        /// <summary>
        /// Set Default Column Name Col0, Col1, Col2..
        /// </summary>
        public void clearColHeader()
        {
            if (csv_table == null || csv_table?.Columns?.Count == 0) return;
            // Clear All Column Name
            int i = 0;
            foreach (DataColumn col in csv_table.Columns)
            {
                col.ColumnName = $"Col{i++}"; 
            }
        }

        /// <summary>
        /// Convert DataTable into CSV string format
        /// </summary>
        /// <param name="v_flg">0:With Header,1:Without Header</param>
        public string SaveStr(int v_flg = 0)
        {
            if (csv_table?.Columns?.Count == 0) return string.Empty;

            var sb = new StringBuilder();

            // 1. Header row (optional)
            if (v_flg == 0)
            {
                for (int i = 0; i < csv_table.Columns.Count; i++)
                {
                    sb.Append(csv_table.Columns[i].ColumnName);
                    if (i < csv_table.Columns.Count - 1)
                        sb.Append(",");
                }
                sb.AppendLine();
            }

            // 2. Data rows
            foreach (DataRow row in csv_table.Rows)
            {
                for (int i = 0; i < csv_table.Columns.Count; i++)
                {
                    var value = row[i]?.ToString() ?? string.Empty; 
                    sb.Append(value);

                    if (i < csv_table.Columns.Count - 1)
                        sb.Append(",");
                }
                sb.AppendLine();
            }
            return sb.ToString().TrimEnd('\r', '\n');
        }
        
        /// <summary>
        /// Direct convert DataTable into CSV File
        /// </summary> 
        public string SaveCsv(string fileName = "export")
        {
            string filePath = string.Empty;

            if (csv_table?.Rows?.Count == 0)
            {
                System.Windows.MessageBox.Show("No data to export!", "Warning",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return filePath;
            }

            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "CSV files (*.csv)|*.csv";
                dialog.FileName = fileName + ".csv";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var writer = new StreamWriter(dialog.FileName, false, Encoding.UTF8)) {

                            // Write header
                            for (int i = 0; i < csv_table.Columns.Count; i++)
                            {
                                writer.Write(csv_table.Columns[i].ColumnName);
                                if (i < csv_table.Columns.Count - 1)
                                    writer.Write(",");
                            }
                            writer.WriteLine();

                            // Write rows
                            foreach (DataRow row in csv_table.Rows)
                            {
                                for (int i = 0; i < csv_table.Columns.Count; i++)
                                {
                                    var value = row[i]?.ToString().Replace("\"", "\"\"");
                                    writer.Write($"\"{value}\"");
                                    if (i < csv_table.Columns.Count - 1)
                                        writer.Write(",");
                                }
                                writer.WriteLine();
                            }
                        }
                        filePath = dialog.FileName;

                        System.Windows.MessageBox.Show("CSV export successful!", "Success",
                            System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                    }
                    catch (Exception ex) {
                        System.Windows.MessageBox.Show("Error saving file: " + ex.Message,
                            "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    }
                }
            }

            return filePath;
        }
        /// <summary>
        /// 固定長ファイル出力
        /// </summary> 
        public void Save2(StreamWriter fp, string[] v_format, int? v_flg, string v_ret)
        {
            if (csv_table == null || csv_table.Rows.Count == 0)
                return;

            for (int i = 0; i < csv_table.Rows.Count; i++)
            {
                var v_line = new StringBuilder();

                for (int j = 0; j < csv_table.Columns.Count; j++)
                {
                    string v_col = csv_table.Rows[i][j]?.ToString() ?? string.Empty;

                    // Jika ada format panjang tetap
                    if (v_format != null && j < v_format.Length && !string.IsNullOrEmpty(v_format[j]))
                    {
                        if (int.TryParse(v_format[j], out int width))
                        {
                            if (v_flg == null)
                                v_col = FullStr(v_col, width);
                            else
                                v_col = FullStr(ToHalfWidth(v_col), width);
                        }
                    }

                    v_line.Append(v_col);
                }

                v_line.Append(v_ret ?? "\n");

                fp.Write(v_line.ToString());
            }
        }

        // Pad right to fixed length (truncate if too long)
        private string FullStr(string s, int length)
        {
            if (s == null) s = string.Empty;
            if (s.Length > length)
                return s.Substring(0, length);
            return s.PadRight(length);
        }

        // Dummy converter: full-width → half-width (can enhance later)
        private string ToHalfWidth(string s)
        {
            // 🔸 optional: implement real conversion if needed
            // For now, return as-is
            return s;
        }

        /// <summary>
        /// Convert file from server into CSV file
        /// </summary> 
        public async Task LoadFromUrlAsync(string dataUrl, string headerUrl)
        {
            using var http = new HttpClient();

            // 🟢 Baca sebagai byte dan decode dengan Shift-JIS
            var headerBytes = await http.GetByteArrayAsync(headerUrl);
            var dataBytes = await http.GetByteArrayAsync(dataUrl);

            string headerText = Encoding.GetEncoding("shift_jis").GetString(headerBytes);
            string dataText = Encoding.GetEncoding("shift_jis").GetString(dataBytes);

            string[] headerCells = headerText.Trim().Split(',');
            DataTable table = new DataTable();
            foreach (var col in headerCells)
                table.Columns.Add(col.Trim());

            var dataLines = dataText.Split('\n');
            if (dataLines[0].StartsWith("H"))
                dataLines = dataLines.Skip(1).ToArray();

            foreach (var line in dataLines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                var row = table.NewRow();
                var cells = line.Trim().Split(',');
                for (int i = 0; i < Math.Min(cells.Length, table.Columns.Count); i++)
                    row[i] = cells[i].Trim();
                table.Rows.Add(row);
            }

            csv_table = table;
        }

        /// <summary>
        /// Read text file from server that provided table column name only
        /// </summary>
        /// <param name="headerUrl">For example: d_sql.txt</param>
        /// <returns></returns>
        public async Task LoadHeaderFromUrl(string headerUrl) {
            using var http = new HttpClient();

            // 🟢 Baca sebagai byte dan decode dengan Shift-JIS
            var headerBytes = await http.GetByteArrayAsync(headerUrl);

            string headerText = Encoding.GetEncoding("shift_jis").GetString(headerBytes);

            string[] headerCells = headerText.Trim().Split(',');
            DataTable table = new DataTable();
            foreach (var col in headerCells)
                table.Columns.Add(col.Trim());
            csv_table = table;

            table.Rows.InsertAt(table.NewRow(),0);
            int i = 0;
            foreach (var line in headerCells)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;                
                table.Rows[0][i] = line.Trim();
                i++;
            }
        }

        /// <summary>
        /// Read text file from data.txt
        /// </summary>
        /// <param name="dataUrl"></param>
        public async Task LoadDataFromUrlAsync(string dataUrl)
        {
            try
            {
                using var http = new HttpClient();
                var dataBytes = await http.GetByteArrayAsync(dataUrl);
                string dataText = Encoding.GetEncoding("shift_jis").GetString(dataBytes);

                if (!string.IsNullOrEmpty(dataText))
                {
                    ConvertStrToTable(dataText, 0);
                }
            }
            catch { }
        }
        #endregion

        #region List Features 

        /// <summary>
        /// Convert dynamic list to string format that use for CSV file generate
        /// Mainly to againts unconsistent column avaible format 
        /// </summary>
        /// <param name="data_table"></param>
        /// <param name="includeHeader"></param>
        /// <returns></returns>
        public static string ConvertDynamicListToCsv(List<dynamic> data_table, bool includeHeader = false)
        { 
            if (data_table == null || data_table.Count == 0)  return string.Empty;
            
            var sb = new StringBuilder();
            var firstRow = (IDictionary<string, object>)data_table.First();

            // ✅ Add header if requested
            if (includeHeader)
            {
                sb.AppendLine(string.Join(",", firstRow.Keys));
            }

            // ✅ Add each row
            foreach (var item in data_table)
            {
                var dict = (IDictionary<string, object>)item;
                var values = dict.Values.Select(v => EscapeCsvValue(v?.ToString() ?? ""));
                sb.AppendLine(string.Join(",", values));
            } 
            return sb.ToString();   
        }

        /// <summary>
        /// Escape commas, quotes, and line breaks
        /// </summary> 
        private static string EscapeCsvValue(string value)
        { 
            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
            {
                value = "\"" + value.Replace("\"", "\"\"") + "\"";
            }
            return value;
        }

        public static void SaveStrToCsv(string csvContent, 
                                        string filename = "export",
                                        string dlgTitle = "Save CSV File",
                                        string dlgFiler = "CSV Files")
        { 
            if(string.IsNullOrEmpty(csvContent)) return;

            // Show file dialog
            var dialog = new SaveFileDialog
            {
                Title = dlgTitle,
                Filter = $"{dlgFiler} (*.csv)|*.csv",
                FileName = $"{filename}.csv"
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                { 
                    File.WriteAllText(dialog.FileName, csvContent, Encoding.UTF8); 
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving file:\n" + ex.Message);
                }
            }
        }


        public static List<T> ConvertDataTableToList<T>(DataTable table) where T : new()
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
        #endregion
    }

    public class BtListHelper : BaseViewModel
    {
        private string _code;
        private string _name;
          
        public string Code
        {
            get => _code;
            set
            {
                if (_code != value)
                {
                    _code = value?.Trim();
                    OnPropertyChanged(nameof(Code));
                    OnPropertyChanged(nameof(Display));
                }
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value?.Trim();
                    OnPropertyChanged(nameof(Name));
                    OnPropertyChanged(nameof(Display));
                }
            }
        }

        // ✨ Editable Display property
        public string Display
        {
            get => $"{Code} {Name}".Trim();
            set
            {
                if (value == null) return;

                var parts = value.Trim().Split(' ', 2);
                Code = parts.Length > 0 ? parts[0] : string.Empty;
                Name = parts.Length > 1 ? parts[1] : string.Empty;

                OnPropertyChanged(nameof(Display));
            }
        }

        public BtListHelper() { }

        public BtListHelper(string code, string name)
        {
            Code = code?.Trim();
            Name = name?.Trim();
        }

        public override string ToString() => Display;

        //public string OnGetId()
        //{ 
        //    if (string.IsNullOrEmpty(Display)) return string.Empty;
        //    var _code = Display.Split(' ',2);
        //    return (_code.Length > 1) ? _code[0] : Display.Trim();
        //} 
    }
}
