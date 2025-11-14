using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Models;
using CvnetClient.Utils;
using CvnetClient.Views;
using Microsoft.VisualBasic.FileIO;
using System.Data;
using System.IO;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg00DBTableViewModel : BaseViewModel
    {
        public enum PrintType { EXCEL,PDF}
        [ObservableProperty]
        public PrintType selectedPrint = PrintType.EXCEL;
        [ObservableProperty]
        private List<ItemInfo> comboListTable = new();
        [ObservableProperty]
        public DataTable? wrk_csv;
        [ObservableProperty]
        private ItemInfo? selectedItem;
        [ObservableProperty]
        private ItemInfo? selectedItem2;
        public void OnInit(object? init_para = null, string? init_flg = null) 
        {
            OnInitBase(init_para, init_flg);
            Wrk_csv = new DataTable();

            if (AppData.ClassCvnet.SysCnt == null)
            {
                var wrk_csv1 = AppData.Http!.AspxSqlQuery2("count2", null);
                Wrk_csv = Convert(wrk_csv1);
                AppData.ClassCvnet.SysCnt = Wrk_csv;
            }
            else
            {
                Wrk_csv = AppData.ClassCvnet.SysCnt;
            }
            ComboListTable = Wrk_csv.AsEnumerable()
                .Select(r => new ItemInfo
                {
                    ID = r.Field<string>("ID"),
                    Name = r.Field<string>("名称"),
                    TableName = r.Field<string>("テーブル名")
                })
                .ToList();
            SelectedItem = ComboListTable.FirstOrDefault();
            SelectedItem2 = ComboListTable.FirstOrDefault();
        }

        [RelayCommand]
        async Task DoPrintAsync()
        {           
            var v_para = new string[2];
            v_para[0] = SelectedItem.ID.ToString() ?? string.Empty;
            v_para[1] = SelectedItem2.ID.ToString() ?? string.Empty;

            var dbstr = "";
            for (var i = 0; i < Wrk_csv.Rows.Count; i++)
            {
                if (int.Parse(v_para[0]) <= int.Parse(Wrk_csv.Rows[i][0].ToString()) && int.Parse(Wrk_csv.Rows[i][0].ToString()) <= int.Parse(v_para[1]))
                {
                    dbstr += ((dbstr == "") ? "" : ",") + "'HC$" + Wrk_csv.Rows[i][3].ToString().ToUpper() + "'";
                }
            }
            var qfm_file = "cvnet_DB_table.qfm";

            var sql_query = "select * from (SELECT C.TABLE_NAME テーブル名, B.OWNER OWNER"
                    + ",C.COLUMN_NAME カラム名, C.DATA_TYPE データタイプ"
                    + ",case C.DATA_TYPE when 'VARCHAR2' then to_char(C.DATA_LENGTH) when 'NUMBER' then C.DATA_PRECISION || ',' || C.DATA_SCALE else '.' end DATA_LENGTH"
                    + ",C.DATA_DEFAULT デフォルト,D.COMMENTS 備考, 'フィールド定義' コメント①,'0' コメント②,A.SEARCH_CONDITION"
                    + ",row_number() over (partition by C.TABLE_NAME order by C.TABLE_NAME, B.CONSTRAINT_NAME, C.COLUMN_NAME) 順"
                    + " FROM USER_CONSTRAINTS A,USER_CONS_COLUMNS B,USER_TAB_COLUMNS C,USER_COL_COMMENTS D"
                    + " WHERE A.TABLE_NAME=B.TABLE_NAME and A.CONSTRAINT_NAME=B.CONSTRAINT_NAME"
                    + " and C.TABLE_NAME = B.TABLE_NAME(+) and C.COLUMN_NAME = B.COLUMN_NAME(+)"
                    + " and C.TABLE_NAME = D.TABLE_NAME and C.COLUMN_NAME = D.COLUMN_NAME"
                    + " and substr(B.CONSTRAINT_NAME, 1, 3) = 'SYS' and C.TABLE_NAME in (" + dbstr + ")"
                + " union all"
                    + " SELECT B.TABLE_NAME テーブル名"
                    + ",NVL((select MAX(A.OWNER) from USER_CONSTRAINTS A where A.TABLE_NAME = B.TABLE_NAME), '.') OWNER"
                    + ",B.INDEX_NAME インデックス名"
                    + ",case substr(B.INDEX_NAME, 1, 6) when 'HC$_PK' then 'TRUE' else 'FALSE' end PK"
                    + ",case substr(B.INDEX_NAME, 1, 6) when 'HC$_PK' then 'TRUE' when 'HC$_UK' then 'TRUE' else 'FALSE' end UK"
                    + ",null 型変換エラー,B.COLUMN_NAME カラム,'インデックス定義' コメント①,'1' コメント②,null コメント③"
                    + ",row_number() over (partition by B.TABLE_NAME order by B.TABLE_NAME, C.CONSTRAINT_NAME, B.INDEX_NAME, B.COLUMN_POSITION) 順"
                    + " FROM USER_IND_COLUMNS B,USER_CONS_COLUMNS C"
                    + " where B.TABLE_NAME = C.TABLE_NAME(+) and B.COLUMN_NAME = C.COLUMN_NAME(+) and B.INDEX_NAME = C.CONSTRAINT_NAME(+)"
                    + " and B.TABLE_NAME in (" + dbstr + ")"
                    + " ) A"
                + " order by A.テーブル名,A.コメント②,順";
            sql_query = "select * from (" + sql_query + ")";
            var ret_csv = AppData.Http!.AspxSqlQueryCsv(sql_query, v_para, qfm_file);
            var lines = ret_csv.Split('\n');

            if (lines.Length < 2 || lines[1] == "0")
            {
                ClientLib.MessageBoxError(this, "PDFデータがありません");
                return;
            }

            string pdfPath = lines[0];

            if (SelectedPrint == PrintType.PDF)
            {
                string url = AppData.Http.URLroot + pdfPath + "/data.pdf";

                bool ready = await Utils.GlobalFunc.WaitForPdfAsync(url, TimeSpan.FromSeconds(30));
                if (!ready)
                {
                    ClientLib.MessageBoxError(this, "PDF生成に時間がかかりすぎています。\n 条件を絞ってください。");
                    return;
                }

                var win = new WebpdfView();
                if (win.DataContext is WebpdfViewModel vm)
                {
                    vm.Pdfdata = url;
                }
                ClientLib.CursorToNormal();
                ClientLib.ShowDialogView(win, this);
            }
            else if (SelectedPrint == PrintType.EXCEL)
            {
                var csv_para = new BizCsvDocument();
                string datapath = AppData.Http.URLroot + pdfPath + "/data.txt";
                string headpath = AppData.Http.URLroot + pdfPath + "/d_sql.txt";
                bool ready = await Utils.GlobalFunc.WaitForPdfAsync(datapath, TimeSpan.FromSeconds(30));
                if (!ready)
                {
                    ClientLib.MessageBoxError(this, "Data生成に時間がかかりすぎています。\n 条件を絞ってください。");
                    return;
                }

                ready = await Utils.GlobalFunc.WaitForPdfAsync(headpath, TimeSpan.FromSeconds(30));
                if (!ready)
                {
                    ClientLib.MessageBoxError(this, "Header生成に時間がかかりすぎています。\n 条件を絞ってください。");
                    return;
                }
                await csv_para.LoadFromUrlAsync(datapath, headpath);
                try
                {
                    csv_para.SaveCsv(SelectedItem.TableName + "~" + SelectedItem2.TableName);
                }
                catch (Exception ex) { }
            }
        }
        public DataTable Convert(string csvData)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(string));
            dt.Columns.Add("名称", typeof(string));
            dt.Columns.Add("件数", typeof(int));
            dt.Columns.Add("テーブル名", typeof(string));

            using (StringReader sr = new StringReader(csvData))
            using (TextFieldParser parser = new TextFieldParser(sr))
            {
                parser.SetDelimiters(",");
                parser.HasFieldsEnclosedInQuotes = true;

                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();
                    if (fields.Length < 4) continue; // skip incomplete lines

                    DataRow row = dt.NewRow();
                    row["ID"] = int.TryParse(fields[0], out int id) ? id : 0;
                    row["名称"] = fields[1];
                    row["件数"] = int.TryParse(fields[2], out int cnt) ? cnt : 0;
                    row["テーブル名"] = fields[3];
                    dt.Rows.Add(row);
                }
            }

            return dt;
        }

        public class ItemInfo
        {
            public string ID { get; set; } = "";
            public string Name { get; set; } = "";
            public string TableName { get; set; } = "";
        }
    }
}
