using System.Data;
using System.IO; 
using System.Text;
using System.Windows.Forms;

namespace CvnetClient.Utils
{
    /// <summary>
    /// BizV Array variable convert to c#
    /// </summary>
    public class BizArray
    { 
        List<string> list;
        public int Count => list.Count;

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
                throw new IndexOutOfRangeException($"Index {index} is out of range. Current size: {list.Count}");
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
