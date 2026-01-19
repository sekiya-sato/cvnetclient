using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Class;
using CvnetClient.Models;
using CvnetClient.Utils;
using CvnetClient.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Windows;
using Xceed.Wpf.Toolkit.PropertyGrid.Implementation.Converters;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg01SetjanViewModel : BaseViewModel
    {

        [ObservableProperty]
        ListFlexData listFlexData = new ListFlexData();

        private BizArray wrk_jodai;

        public enum Settings
        { Type49 = 49,Type45 = 45 }

        public enum First
        { None = 0, JANコード1 = 1, JANコード2 = 2, JANコード3 = 3 }

        public enum Overwrite
        { shinai = 0, suru = 1 }

        public enum Body
        { JAN49 = 0, JishaCD = 1 }

        public enum Second
        { None = 0,JANコード1 = 1, JANコード2 = 2, JANコード3 = 3 }

        public enum Third
        { None = 0, JANコード1 = 1, JANコード2 = 2, JANコード3 = 3 }

        [ObservableProperty]
        private Settings selectedType = Settings.Type45;

        [ObservableProperty]
        private First selectedFirst = First.JANコード3;

        [ObservableProperty]
        private Body selectedBody = Body.JAN49;

        [ObservableProperty]
        private Overwrite selectedOverwrite = Overwrite.shinai;

        [ObservableProperty]
        private Second selectedSecond = Second.None;

        [ObservableProperty]
        private Third selectedThird = Third.None;

        [ObservableProperty]
        private bool isSettingsEnabled = true;

        [ObservableProperty]
        private bool isBodyEnabled = true;

        [ObservableProperty]
        private bool isOverwriteEnabled = true;

        [ObservableProperty]
        private Visibility settingsVisibility = Visibility.Visible;



        public void OnInit(object? init_para = null, string? init_flg = null)
        {
            OnInitBase(init_para, init_flg);

            List<CsvItem> def = null;

            ListFlexData.ListConfig = new ListFlexConfig()
            {
                init_csv = def ?? new List<CsvItem>(),
                flag = 0
            };

            if (para.Count > 0) {
                if (para[0] == "1")
                {
                    IsSettingsEnabled = false;
                    IsBodyEnabled = false;
                    IsOverwriteEnabled = false;
                }
            }

            para = new BizArray();
        }

        [RelayCommand]
        async Task CheckDuplicate() 
        {
            var wrk_para2 = new BizArray();
            var wrk_para = new BizArray();

            int total_cnt = 0;
            int flg = 0;

            wrk_para2[0] = ((int)selectedFirst).ToString();
            wrk_para2[1] = ((int)selectedSecond).ToString();
            wrk_para2[2] = ((int)selectedThird).ToString();

            if (selectedFirst != First.None)   { flg = 1; }
            if (selectedSecond != Second.None) { flg = 1; }
            if (selectedThird != Third.None)   { flg = 1; }

            if (flg == 0)
            {
                ClientLib.MessageBox(this, "段が選ばれていません");
                return;
            }

            string whereStr = "";
            string whereStr2 = "";
            string subSqlStr = "";
            string whereStrIn = "";
            string orderStr = "";

            for (int i=0; i<wrk_para2.Count; i++) {
                if (wrk_para2[i] != "0")
                {
                    whereStr += ((whereStr == "") ? "" : " OR ") + "J.\"JANコード" + wrk_para2[i] + "\"='.'";
                    whereStr2 += ((whereStr2 == "") ? "" : " AND ") + "J.\"JANコード" + wrk_para2[i] + "\"<>'.'";
                    subSqlStr += ((subSqlStr == "") ? "" : ",") + "J.\"JANコード" + wrk_para2[i] + "\" A" + i.ToString();
                    whereStrIn += ((whereStrIn == "") ? "" : ",") + "J.\"JANコード" + wrk_para2[i] + "\"";
                }
                else {
                    subSqlStr += ((subSqlStr == "") ? "" : ",") + "'' A" + i.ToString(); 
                }
                orderStr += ((orderStr == "") ? "" : ",") + "A" + i.ToString();
            }

            string sql_prod = ListFlexData.GetQueryStr(wrk_para, 1);
            //sql_prod = "";
            string exists_prod = "";
            if (sql_prod != "") exists_prod = " AND EXISTS (SELECT 'X' FROM HC$MASTER_SHOHIN S WHERE S.商品CD=J.商品CD " + sql_prod + ")";

            string sql_str = "SELECT J.* FROM ("
                    + "SELECT '（JAN設定無し）' A0, '' A1, '' A2,J.商品CD,J.色CD,J.サイズCD,1 順 FROM HC$MASTER_SHOHIN_JAN J"
                    + " WHERE J.商品CD<>'.' AND " + whereStr + "" + exists_prod
                    + " UNION"
                    + " SELECT " + subSqlStr + ",J.商品CD,J.色CD,J.サイズCD,2 順 FROM HC$MASTER_SHOHIN_JAN J"
                    + " WHERE J.商品CD<>'.' AND  (" + whereStrIn + ") IN"
                    + " (SELECT " + whereStrIn + " FROM HC$MASTER_SHOHIN_JAN J"
                    + " WHERE " + whereStr2 + "" + exists_prod
                    + " HAVING COUNT(*)>1"
                    + " GROUP BY " + whereStrIn + ")"
                    + ") J ORDER BY J.順," + orderStr + ",J.商品CD,J.色CD,J.サイズCD";
            //sql_str = "SELECT J.* FROM (SELECT '（JAN設定無し）' A0, '' A1, '' A2,J.商品CD,J.色CD,J.サイズCD,1 順 \r\nFROM HC$MASTER_SHOHIN_JAN J \r\nWHERE J.商品CD<>'.'\r\nAND J.\"JANコード3\"='.' \r\nUNION SELECT J.\"JANコード3\" A0,'' A1,'' A2,J.商品CD,J.色CD,J.サイズCD,2 順 \r\nFROM HC$MASTER_SHOHIN_JAN J WHERE J.商品CD<>'.' AND  (J.\"JANコード3\") IN (SELECT J.\"JANコード3\" FROM HC$MASTER_SHOHIN_JAN J WHERE J.\"JANコード3\"<>'.' HAVING COUNT(*)>1 GROUP BY J.\"JANコード3\")) J ORDER BY J.順,A0,A1,A2,J.商品CD,J.色CD,J.サイズCD";
            if (!ClientLib.MessageBox(this, "印刷しますか？")) return;
            ClientLib.CursorToWait();
            var param = new string[4];

            var ret = AppData.Http!.AspxSqlQueryCsv(sql_str, wrk_para.ToArray() , "cvnet01prn_chkjan.qfm");
            //var ret = AppData.Http!.AspxSqlQueryCsv(sql_str, null, "cvnet01prn_chkjan.qfm");
            await Task.Delay(1500); // PDF生成待ち
            var lines = ret.Split('\n');

            if (lines.Length < 2 || lines[1] == "0")
            {
                ClientLib.MessageBoxError(this, "PDFデータがありません");
                return;
            }

            string pdfPath = lines[0];
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

        [RelayCommand]
        void DoList()
        {
            var wrk_para = new BizArray();
            var wrk_para2 = new BizArray();
            wrk_para2 = ListFlexData.GetSearchPara(wrk_para);
            var v_para = new BizArray();
            var tmp_str = "";
            for (int i = 0; i < wrk_para2.Count; i++)
            {
                tmp_str += ((i == 0) ? "" : "@") + wrk_para2[i];
            }
            v_para[0] = new string(tmp_str);
            tmp_str = "";
            for (var i = 0; i < wrk_para.Count; i++)
            {
                tmp_str += ((i == 0) ? "" : "@") + ((wrk_para[i] == "") ? "." : wrk_para[i]);
            }
            v_para[1] = new string(tmp_str);
            v_para[2] = new string(((int)selectedBody).ToString());
            if (selectedType == Settings.Type49) {
                v_para[3] = new string("49");
            }
            else v_para[3] = new string("45");

            tmp_str = "";
            int cnt = 0;
            if (SelectedFirst != 0)
            {
                tmp_str += ((cnt == 0) ? "" : "@") + selectedFirst;
                cnt++;
            }
            if (selectedSecond > 0)
            {
                tmp_str += ((cnt == 0) ? "" : "@") + selectedSecond;
                cnt++;
            }
            if (selectedThird > 0)
            {
                tmp_str += ((cnt == 0) ? "" : "@") + selectedThird;
                cnt++;
            }
            if (cnt == 0)
            {
                ClientLib.MessageBoxError(this,"更新列が選択されていません");
                return;
            }
            v_para[4] = new string(tmp_str);
            v_para[5] = new string(((int)selectedOverwrite).ToString());

            var ret_csv = AppData.Http!.AspxSqlQuery2("SetShohinJan", v_para.ToArray());
            if (!string.IsNullOrEmpty(ret_csv)) {
                var lines = ret_csv.Split('\n');
                if (Int32.Parse(lines[0]) != 0)
                {
                    string err_str = "更新エラー";
                    if (lines.Count() > 1) err_str += "\n" + lines[1];
                    ClientLib.MessageBoxError(this, err_str);
                }
                else
                {
                    ClientLib.MessageBox(this,lines[0] + "件 正常更新終了");
                }
            } else ClientLib.MessageBoxError(this, "サーバーエラー");



        }
    }
}
