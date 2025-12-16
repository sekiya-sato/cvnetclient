using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Models;
using CvnetClient.Utils;
using System;
using System.Data;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace CvnetClient.ViewModels
{
    public partial class SubDlgSel06ViewModel : BaseViewModel
    {
        [ObservableProperty] private int outputType = 0;
        [ObservableProperty] private DateTime deleteBeforeDate = DateTime.Now.AddDays(-90);
        [ObservableProperty] private string? messageText;
        [ObservableProperty] private bool chkUserOnly;
        [ObservableProperty] private bool chkNotTN;
        [ObservableProperty] private bool chkNotDaTe;
        private BizArray para = new();

        public void OnInit(string v_mstname, string sql_query = "", object v_para2 = null)
        {
            if (v_para2 is string[] init_para)
                para = new BizArray(init_para);
            else
                para = new BizArray();

            // interpret para[] into boolean flags:
            ChkUserOnly = para[0] == "1";
            ChkNotTN = para[1] == "1";
            ChkNotDaTe = para[2] == "1";
        }

        [RelayCommand]
        public void DoExit()
        {
            ClientLib.ExitDialogResult(this, true);
        }

        [RelayCommand]
        public async Task DoOutputAsync()
        {
            try
            {
                ClientLib.CursorToWait();

                if (OutputType == 0)
                    await ExportCsvAsync();
                else
                    await ExportXmlAsync();

                ClientLib.CursorToNormal();
            }
            catch (Exception ex)
            {
                ClientLib.CursorToNormal();
                ClientLib.MessageBoxError(this, $"ファイル出力に失敗しました:\n{ex.Message}");
            }
        }

        [RelayCommand]
        public async Task ExportCsvAsync()
        {
            if (!ClientLib.MessageBox(this, "CSVを出力しますか？"))
                return;

            ClientLib.CursorToWait();
            try
            {
                //// 1. Build SQL
                //string sql = BuildCsvSql();

                //// 2. Execute SQL → get folder & count
                //var ret = AppData.Http.AspxSqlQuery(sql, null, "dummy");
                //string folder = ret.GetCell(0, 0)?.ToString() ?? "";
                //int count = Convert.ToInt32(ret.GetCell(1, 0));

                //if (count <= 0)
                //{
                //    ClientLib.MessageBoxError(this, "処理件数 = 0 です");
                //    return;
                //}

                //// 3. Build URLs for server CSV files
                //string baseUrl = AppData.Http.URLroot + folder;
                //string dataUrl = baseUrl + "/data.txt";
                //string headerUrl = baseUrl + "/d_sql.txt";

                //// 4. Load the CSV document (same as SatooCSVDocument.get)
                //var csv = new BizCsvDocument();
                //await csv.LoadFromUrlAsync(dataUrl, headerUrl);

                //// 5. Save as CSV (same as savedialog + save())
                //csv.SaveCsv("TRAN_RIREKI");

                //MessageText = "CSV 出力が完了しました！";
            }
            finally
            {
                ClientLib.CursorToNormal();
            }
        }

        [RelayCommand]
        public async Task ExportXmlAsync()
        {
            if (!ClientLib.MessageBox(this, "XMLを出力しますか？"))
                return;

            ClientLib.CursorToWait();
            try
            {

            }
            finally
            {
                ClientLib.CursorToNormal();
            }
        }



        //[RelayCommand]
        //public void DoDelete()
        //{
        //    var confirm = MessageBox.Show(
        //        $"{DeleteBeforeDate:yyyy/MM/dd} 以前の履歴を削除します。\nよろしいですか？",
        //        "確認", MessageBoxButton.YesNo);

        //    if (confirm != MessageBoxResult.Yes)
        //        return;

        //    string[] args =
        //    {
        //        "TRAN_RIREKI",
        //        $"get_vdate(LOGIN時間) <= '{DeleteBeforeDate:yyyy/MM/dd} 235959'"
        //    };

        //    var ret = AppData.Http?.AspxSqlQuery2("mi_del", args);

        //    if (ret?.Rows.Count > 0 && Convert.ToInt32(ret.Rows[0][0]) >= 0)
        //        MessageBox.Show($"{ret.Rows[1][0]} 件削除しました");
        //    else
        //        MessageBox.Show("削除に失敗しました");
        //}

        private string BuildCsvSql()
        {
            string sql =
                " select " +
                "GET_VDATE(A.LOGIN時間) LOGIN時間," +
                "A.社員CD," +
                "DECODE(NVL(GET_KUBUN(A.USER_ID), -1), 0, NVL((SELECT B.名前 from HC$master_shain B where A.社員CD = B.社員CD), ' '), 1, NVL(D.仕入先名, ' ')) 社員名," +
                "case when (A.REMOTE_ADDR like '192.168.%') then '社内' else A.REMOTE_ADDR end REMOTE_ADDR," +
                "GET_VDATE(A.最終ACCESS時間) 最終ACCESS時間," +
                "substrb(A.MESS, instrb(A.MESS, '___', 1, 5)+3, instrb(A.MESS, '___', 1, 6)-instrb(A.MESS,'___',1,5)-3) CRS," +
                "NVL(B.店舗CD,'') ||' '|| NVL((select t.得意先名 from HC$MASTER_TOKUI t where t.得意先CD = B.店舗CD),'') 店," +
                "substrb(A.MESS, instrb(A.MESS,'___',1,4)+3, instrb(A.MESS,'___',1,5)-instrb(A.MESS,'___',1,4)-3) BizVer," +
                "substrb(A.MESS, 0, instrb(A.MESS,'___',1,1)-1) WINユーザー名," +
                "substrb(A.MESS, instrb(A.MESS,'___',1,1)+3, instrb(A.MESS,'___',1,2)-instrb(A.MESS,'___',1,1)-3) WINマシン名," +
                "substrb(A.MESS, instrb(A.MESS,'___',1,2)+3, instrb(A.MESS,'___',1,3)-instrb(A.MESS,'___',1,2)-3) CPU," +
                "substrb(A.MESS, instrb(A.MESS,'___',1,3)+3, instrb(A.MESS,'___',1,4)-instrb(A.MESS,'___',1,3)-3) OS" +
                " from HC$tran_rireki A" +
                " left outer join HC$master_shain B on (A.社員CD = B.社員CD)" +
                " left outer join HC$master_tokui C on (C.得意先CD = B.店舗CD)" +
                " left outer join HC$master_siire D on (D.仕入先CD = A.社員CD)" +
                " where NOT(A.MESS LIKE '%DEVELOP0%') and NOT(A.MESS LIKE '%DTP150%')";

            if (ChkUserOnly)
                sql += " and A.USER_ID >= 0 and A.社員CD != '.'";

            if (ChkNotTN)
                sql += " and NVL(substr(A.MESS, instr(A.MESS,'___',1,5)+3,2),'.') NOT IN ('TN')";

            if (ChkNotDaTe)
                sql += " and NVL(substr(A.MESS, instr(A.MESS,'___',1,5)+3,2),'.') NOT IN ('Da','.','te')";
            
                sql += " order by A.SEQ_NO desc";

            return sql;
        }
    }
}
