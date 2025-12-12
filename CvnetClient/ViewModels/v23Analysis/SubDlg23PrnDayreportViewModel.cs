using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Models;
using CvnetClient.Utils;
using CvnetClient.Views;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg23PrnDayreportViewModel : BaseViewModel
    {
        #region Declare
        public enum PrintType { Spool, CSV}
        [ObservableProperty]
        SearchCondition? condition;
        #endregion
        #region Initialize
        public void OnInit(object? init_para = null, string? init_flg = null) 
        {
            OnInitBase(init_para, init_flg);
            Condition = new SearchCondition();
            Condition.ShopFrom = "0";
            Condition.ShopTo = "99999999";
            Condition.DateFrom = DateTime.Now;
            Condition.DateTo = DateTime.Now;
        }
        #endregion
        #region Function
        [RelayCommand]
        public void SelShop1(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Condition != null)
            {
                Condition.ShopFrom = get_sel00.Code;
                Condition.ShopFromName = get_sel00.Name;
            }
        }
        [RelayCommand]
        public void SelShop2(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Condition != null)
            {
                Condition.ShopTo = get_sel00.Code;
                Condition.ShopToName = get_sel00.Name;
            }
        }
        [RelayCommand]
        async Task DoPrintAsync() 
        {
            if (!ClientLib.MessageBox(this, "印刷しますか？")) return;
            string[] wrk_para = new string[4];
            wrk_para[0] = Condition.DateFrom?.ToString("yyyyMMdd") ?? string.Empty;
            wrk_para[1] = Condition.DateTo?.ToString("yyyyMMdd") ?? string.Empty; ;
            wrk_para[2] = Condition.ShopFrom?.ToString() ?? string.Empty;
            wrk_para[3] = Condition.ShopTo?.ToString() ?? string.Empty;

            var v_k1 = "";
            var v_k2 = "";
            if (AppData.ClassCvnet.config.oroshi == 2)
            {
                v_k1 = " 単品NO";
                v_k2 = " 相手商品CD";
            }

            var v_j1 = "";
            if (AppData.ClassCvnet.config.urikakeflg == 1)
            {
                v_j1 = " and B.掛計上FLG=1 ";
            }

            var v_kingaku = "下代単価,下代金額n";
            var v_hiduke = "掛計上日";
            if (AppData.ClassCvnet.config.oroshi != 0)
            {
                v_kingaku = "単価,金額n";
                v_hiduke = "在庫計上日";
            }
            var v_sokbn1 = "A.商品CD=S.商品CD(+)";
            if (AppData.ClassCvnet.config.shokbnflg == 1)
            {
                v_sokbn1 = " A.商品CD=S.商品CD AND s.商品区分FLG=0 ";
            }

            var sql_str = "select 取引先CD1,得意先名," + v_hiduke + ",曜日,明細取引区分,商品CD,色CD" + v_k1 + ",サイズCD" + v_k2;
            sql_str += ",数量n," + v_kingaku + ",品名,上代単価,上代金額n,色名,サイズ名,伝票NO,金額日計,数量日計";
            sql_str += ", get_vDate(MAX(vdate1) over(partition by 取引先CD1," + v_hiduke + "), 'YYYY/MM/DD HH24:MI:SS') vdate1";
            sql_str += ", sum(数量n) over (partition by 取引先CD1," + v_hiduke + ") 数量計";
            sql_str += "," + AppData.ClassCvnet.comboItem00.GetCaseStr("共通売上区分", "明細取引区分") + " 取引区分名";
            sql_str += ",decode(店種区分,1,'1 卸',3,'3 売仕','') 店種区分名,掛率1,明細NO,関連伝票NO2,単位 from ( ";
            sql_str += "select b.取引先CD1,d.得意先名";
            sql_str += ",b." + v_hiduke + ",TO_CHAR(to_date(b." + v_hiduke + ",'YYYYMMDD'),'DAY') 曜日";
            sql_str += ",a.明細取引区分,a.商品CD,a.色CD,a.サイズCD,DECODE(trunc(A.明細取引区分/10),1,1,2,-1,3,-1,4,1,1)*a.数量 数量n";
            sql_str += ",DECODE(trunc(A.明細取引区分/10),1,1,2,-1,3,-1,4,1,1)*a.金額 金額n";
            sql_str += ",DECODE(trunc(A.明細取引区分/10),1,1,2,-1,3,-1,4,1,1)*a.下代金額 下代金額n";
            sql_str += ",DECODE(trunc(A.明細取引区分/10),1,1,2,-1,3,-1,4,1,1)*a.上代金額 上代金額n";
            sql_str += ",NVL((select c.商品名 from HC$master_shohin c where c.商品CD=a.商品CD),'.') 品名";
            sql_str += ",A.上代単価,A.下代単価,A.単価";
            sql_str += ",NVL((select c.名称 from HC$master_meisho c where c.名称区分='COL' and c.名称CD=a.色CD),'.') 色名";
            sql_str += ",GET_SIZENAME(A.商品CD,A.サイズCD) サイズ名";
            sql_str += ",a.ヘッダNO 伝票NO";
            sql_str += ",SUM(DECODE(trunc(A.明細取引区分/10),1,1,2,-1,3,-1,4,1,1)*a.金額) over (partition by b.取引先CD1,b." + v_hiduke + ") 金額日計";
            sql_str += ",SUM(DECODE(trunc(A.明細取引区分/10),1,1,2,-1,3,-1,4,1,1)*a.数量) over (partition by b.取引先CD1,b." + v_hiduke + ") 数量日計";
            sql_str += ",a.行NO 明細NO, a.vdate_create vdate1,d.店種区分,decode(A.上代単価,0,0,trunc(A.下代単価/A.上代単価*100,1)) 掛率1 ";
            sql_str += ",b.関連伝票NO2,s.単位";
            sql_str += " from HC$tran_tori1 a, HC$tran_tori0 b, HC$master_tokui d,HC$MASTER_SHOHIN S";
            sql_str += " where " + v_sokbn1 + v_j1;
            sql_str += " AND a.ヘッダNO=b.SEQ_NO and b.取引先CD1=d.得意先CD and d.店種区分 between 1 and 3 and d.出荷停止FLG=0";
            sql_str += " and b.伝票処理区分 in (0,1) and b." + v_hiduke + " between :1 and :2 and b.取引先CD1 between :3 and :4";
            sql_str += ") order by 取引先CD1," + v_hiduke + ",伝票NO,明細NO";

            if (Condition.SelectedPrint.ToString() == "CSV") 
            {
                sql_str = ""
                    + "select "
                    + " a.取引先CD1 得意先CD , "
                    + " a.得意先名 得意先 , "
                    + " a.掛計上日 日付 , "
                    + " a.曜日 , "
                    + " substrb(a.店種区分名, 1, instr(a.店種区分名, ' ')-1) 店種CD , "
                    + " substrb(a.店種区分名, instr(a.店種区分名, ' ')+1) 店種 , "
                    + " a.VDATE1 現在 , "
                    + " a.伝票NO , "
                    + " a.関連伝票NO2 , "
                    + " a.明細NO 行NO , "
                    + " substrb(a.取引区分名, 1, instr(a.取引区分名, ' ')-1) 取引区分CD , "
                    + " substrb(a.取引区分名, instr(a.取引区分名, ' ')+1) 取引区分名 , "
                    + " a.商品CD , "
                    + " a.品名 商品 , "
                    + " a.色名 色 , "
                    + " a.サイズ名 サイズ , "
                    + " a.掛率1 掛率 , "
                    + " a.数量N 数量 , "
                    + " a.上代単価 , "
                    + " a.上代金額N 上代金額 , "
                    + " a.下代単価 , "
                    + " a.下代金額N 下代金額 "
                    + "  from "
                    + "(" + sql_str + ") a";
            }
            var qfm_file = "cvnettok101.qfm";

            /* 卸対応　10.04.27 */
            if (AppData.ClassCvnet.config.oroshi == 1) qfm_file = "cvnettok101w.qfm";
            if (AppData.ClassCvnet.config.oroshi == 2) qfm_file = "cvnettok101w2.qfm";

            var ret_csv = AppData.Http!.AspxSqlQueryCsv(sql_str, wrk_para, qfm_file);
            var lines = ret_csv.Split('\n');

            if (lines.Length < 2 || lines[1] == "0")
            {
                ClientLib.MessageBoxError(this, "PDFデータがありません");
                return;
            }

            string pdfPath = lines[0];

            if (Condition.SelectedPrint == PrintType.Spool)
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
            else 
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
                await csv_para.LoadFromUrlAsync(datapath,headpath);
                try
                {
                    csv_para.SaveCsv(DateTime.Now.ToString("yyyyMMdd") +"_得意先別売上日報");
                }
                catch(Exception ex) { }
            }
        }
        #endregion
        public partial class SearchCondition : ObservableObject 
        {
            [ObservableProperty]
            private string? shopFrom;
            [ObservableProperty]
            private string? shopTo;
            [ObservableProperty]
            private string? shopFromName;
            [ObservableProperty]
            private string? shopToName;
            [ObservableProperty]
            private PrintType selectedPrint = PrintType.Spool;
            [ObservableProperty]
            private DateTime? dateFrom;
            [ObservableProperty]
            private DateTime? dateTo;
        }
    }
}
