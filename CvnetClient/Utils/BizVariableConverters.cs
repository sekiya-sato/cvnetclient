using Newtonsoft.Json.Linq;
using System.Data;
using System.IO; 
using System.Text;
using System.Windows.Documents;
using System.Windows.Forms;

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
        DataTable csv_doc;

        public BizCsvDocument() { 
            csv_doc = new DataTable(); 
        }

        public BizCsvDocument(DataTable csv_doc) {
            this.csv_doc = csv_doc;
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
            var dt = new DataTable();
            this.csv_doc = dt;
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
            this.csv_doc = dt;
        }
         
        public DataTable GetTable()
        {
            return csv_doc;
        }

        public void Clear()
        {
            if (csv_doc != null) csv_doc.Clear();
            else csv_doc = new DataTable();
        }

        /// <summary>
        /// Set each DataTable Column Name by split the string value 
        /// </summary>
        /// <param name="colName">ColName1,ColName2,ColName3</param>
        public void SetColHeader(string colName)
        {
            if (string.IsNullOrEmpty(colName)) return;

            // Clear All Column Name
            clearColHeader();

            var columns = colName
                                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(x => x.Trim())  // Trim spaces like " Col1 " → "Col1"
                                .Where(x => x.Length > 0) // Filter out accidental empty values
                                .ToList();
              
            int i = 0;
            foreach (DataColumn col in csv_doc.Columns)
            {
                col.ColumnName = columns[i++] ?? string.Empty;
            }
        }

        /// <summary>
        /// Set each DataTable Column Name by string array 
        /// </summary>
        public void SetColHeader(string[] colName)
        { 
            if (colName == null || colName.Length == 0) return;

            // Clear All Column Name
            clearColHeader();

            // Set New Column Name
            int i = 0;
            foreach (DataColumn col in csv_doc.Columns)
            {
                col.ColumnName = colName[i++] ?? string.Empty;
            }
        }

        /// <summary>
        /// Set Default Column Name Col0, Col1, Col2..
        /// </summary>
        public void clearColHeader()
        {
            if (csv_doc == null || csv_doc?.Columns?.Count == 0) return;
            // Clear All Column Name
            int i = 0;
            foreach (DataColumn col in csv_doc.Columns)
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
            if (csv_doc == null || csv_doc.Columns.Count == 0) return string.Empty;

            var sb = new StringBuilder();

            // 1. Header row (optional)
            if (v_flg == 0)
            {
                for (int i = 0; i < csv_doc.Columns.Count; i++)
                {
                    sb.Append(csv_doc.Columns[i].ColumnName);
                    if (i < csv_doc.Columns.Count - 1)
                        sb.Append(",");
                }
                sb.AppendLine();
            }

            // 2. Data rows
            foreach (DataRow row in csv_doc.Rows)
            {
                for (int i = 0; i < csv_doc.Columns.Count; i++)
                {
                    var value = row[i]?.ToString() ?? string.Empty; 
                    sb.Append(value);

                    if (i < csv_doc.Columns.Count - 1)
                        sb.Append(",");
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public void SaveCsv(string fileName = "export")
        {
            if (csv_doc == null || csv_doc.Rows.Count == 0)
            {
                System.Windows.MessageBox.Show("No data to export!", "Warning",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return;
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
                            for (int i = 0; i < csv_doc.Columns.Count; i++)
                            {
                                writer.Write(csv_doc.Columns[i].ColumnName);
                                if (i < csv_doc.Columns.Count - 1)
                                    writer.Write(",");
                            }
                            writer.WriteLine();

                            // Write rows
                            foreach (DataRow row in csv_doc.Rows)
                            {
                                for (int i = 0; i < csv_doc.Columns.Count; i++)
                                {
                                    var value = row[i]?.ToString().Replace("\"", "\"\"");
                                    writer.Write($"\"{value}\"");
                                    if (i < csv_doc.Columns.Count - 1)
                                        writer.Write(",");
                                }
                                writer.WriteLine();
                            }
                        }

                        System.Windows.MessageBox.Show("CSV export successful!", "Success",
                            System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                    }
                    catch (Exception ex) {
                        System.Windows.MessageBox.Show("Error saving file: " + ex.Message,
                            "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}
