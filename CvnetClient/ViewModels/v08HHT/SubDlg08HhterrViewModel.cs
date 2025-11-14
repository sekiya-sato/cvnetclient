using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Models;
using CvnetClient.Utils;
using CvnetClient.Views;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg08HhterrViewModel : BaseViewModel
    {
        #region Declare
        public enum PrintType {Spool,CSV }
        [ObservableProperty]
        SearchCondtion? condition;
        [ObservableProperty]
        public Dictionary<string, string> comboListProcess;
        #endregion
        #region Initialize
        public void OnInit(object? init_para = null, string? init_flg = null) 
        {
            OnInitBase(init_para, init_flg);
            Condition = new SearchCondtion();

            ComboListProcess = new Dictionary<string, string>
            {
                {"-1", "-1 全て"},
                {"1", "1 仕入" },
                {"2", "2 売上" },
                {"3", "3 客数" },
                {"4", "4 移動" },
                {"6", "6 棚卸" },
                {"7", "7 受注" },
                {"0", "0 発注" },
                {"8", "8 配分" },
            };
            Condition.Process = ComboListProcess.FirstOrDefault().Key;
            Condition.DateFrom = DateTime.Now.AddDays(-10);
            Condition.DateTo = DateTime.Now;
            Condition.WareFrom = "0";
            Condition.WareTo = "99999999";
        }
        #endregion
        #region Function
        [RelayCommand]
        public void SelShop1(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Condition != null)
            {
                Condition.WareFrom = get_sel00.Code;
                Condition.WareFromName = get_sel00.Name;
            }
        }
        [RelayCommand]
        public void SelShop2(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Condition != null)
            {
                Condition.WareTo = get_sel00.Code;
                Condition.WareToName = get_sel00.Name;
            }
        }
        [RelayCommand]
        async Task DoPrintAsync()
        {
            var wrk_para = new string[4];
            wrk_para[0] = Condition.DateFrom?.ToString("yyyyMMdd");
            wrk_para[1] = Condition.DateTo?.ToString("yyyyMMdd");
            wrk_para[2] = Condition.WareFrom.ToString();
            wrk_para[3] = Condition.WareTo.ToString();

            var v_col = " A.カラー||'-'||nvl((select distinct M.名称 from hc$master_shohin_jan T, hc$master_meisho M where A.品番 = T.商品CD(+) and A.カラー = T.色CD(+) and A.サイズ = T.サイズCD(+) and M.名称区分='COL' and A.カラー = M.名称CD(+) ),'ﾏｽﾀ無') 色名, ";
            if (AppData.ClassCvnet.config.oroshi != 0)
            {
                v_col = " A.カラー||nvl( (select '' from hc$master_shohin_jan T where A.品番 = T.商品CD(+) and A.カラー = T.色CD(+) ),'-ﾏｽﾀ無') 色名, ";
            }

            var sql_str = " select * from (select '" + wrk_para[0] + "' 範囲0,'" + wrk_para[1] + "' 範囲1,'" + wrk_para[2] + "' 範囲2,'" + wrk_para[3] + "' 範囲3, ";
            sql_str += " A.店舗,A.日付,";
            sql_str += " A.処理区分||'-'||nvl((select distinct T.名称 from hc$master_meisho T where T.名称区分(+)='HHT' and T.名称CD(+)=A.処理区分),'ﾏｽﾀ無') 処理, ";
            sql_str += " A.伝票NO,A.担当者,";
            sql_str += " CASE WHEN A.処理区分='08' OR A.処理区分='10' OR A.処理区分='15' ";
            sql_str += " THEN A.取引先||' '||nvl((select T.仕入先名 from hc$master_SIIRE T where A.取引先 = T.仕入先CD(+)),'ﾏｽﾀ無') ";
            sql_str += " ELSE A.取引先||' '||nvl((select T.得意先名 from hc$master_tokui T where A.取引先 = T.得意先CD(+)),'ﾏｽﾀ無') END 取引, ";
            sql_str += " A.品番||'-'||nvl((select T.商品名 from hc$master_shohin T where A.品番 = T.商品CD(+)),'ﾏｽﾀ無') 商品, ";
            sql_str += v_col;
            sql_str += " A.サイズ||'-'||nvl((select GET_SIZENAME(T.商品CD,T.サイズCD)  from hc$master_shohin_jan T where A.品番 = T.商品CD and A.カラー = T.色CD and A.サイズ = T.サイズCD ),'ﾏｽﾀ無') サイズ名, ";
            sql_str += " A.数量,A.上代,A.下代,A.HHT日付,A.パックNO,A.シリアルNO,A.顧客CD,A.納品日,A.棚番,A.関連伝票NO,A.チェック ";
            sql_str += ",A.予備01";
            sql_str += ",A.店舗||' '||nvl((select T.得意先名 from hc$master_tokui T where A.店舗 = T.得意先CD(+)),'ﾏｽﾀ無') 店舗名 ";
            sql_str += ",A.担当者||' '||nvl((select T.名前 from hc$master_SHAIN T where A.担当者 = T.社員CD(+)),'') 担当者名";
            sql_str += " from HC$Tran_HHTDATA A ";
            sql_str += " where A.日付 between :1 and :2 ";
            sql_str += " and  A.店舗 between :3 and :4 ";

            if (Condition.Process.ToString() != "-1")
            {
                sql_str += " and trunc(to_number(A.処理区分)/10)='"+ Condition.Process.ToString() + "'";
            }
            sql_str += " and A.取込FLG<>'1'";
            sql_str += " order by A.店舗,A.日付 ) ";

            var qfm_file = "cvnethhtdata001_3.qfm";
            var ret_csv = AppData.Http!.AspxSqlQueryCsv(sql_str, wrk_para, qfm_file);
            var lines = ret_csv.Split('\n');

            if (lines.Length < 2 || lines[1] == "0")
            {
                ClientLib.MessageBoxError(this, "印刷データがありません");
                return;
            }

            string pdfPath = lines[0];

            if (Condition.SelectedPrint.ToString() == "スプール")
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
            else if (Condition.SelectedPrint.ToString() == "CSV")
            {
                var csv_para = new BizCsvDocument();
                string datapath = AppData.Http.URLroot + pdfPath + "/data.txt";
                string headpath = AppData.Http.URLroot + pdfPath + "/d_sql.txt";
                bool ready = await Utils.GlobalFunc.WaitForPdfAsync(datapath, TimeSpan.FromSeconds(30));
                if (!ready)
                {
                    ClientLib.MessageBoxError(this, "データ生成に時間がかかりすぎています。\n 条件を絞ってください。");
                    return;
                }

                ready = await Utils.GlobalFunc.WaitForPdfAsync(headpath, TimeSpan.FromSeconds(30));
                if (!ready)
                {
                    ClientLib.MessageBoxError(this, "ヘッダー生成に時間がかかりすぎています。\n 条件を絞ってください。");
                    return;
                }
                await csv_para.LoadFromUrlAsync(datapath, headpath);
                try
                {
                    csv_para.SaveCsv(DateTime.Now.ToString("yyyyMMdd") + "_HHT未更新データ");
                }
                catch (Exception ex) { }
            }
        }
        #endregion
        public partial class SearchCondtion : ObservableObject 
        {
            [ObservableProperty]
            private string? process;
            [ObservableProperty]
            private DateTime? dateFrom;
            [ObservableProperty]
            private DateTime? dateTo;
            [ObservableProperty]
            private string? wareFrom;
            [ObservableProperty]
            private string? wareTo;
            [ObservableProperty]
            private string? wareFromName;
            [ObservableProperty]
            private string? wareToName;
            [ObservableProperty]
            private PrintType selectedPrint = PrintType.Spool;
        }
    }
}
