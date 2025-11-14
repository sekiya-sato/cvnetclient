using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Models;
using CvnetClient.Utils;
using CvnetClient.Views;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg23PrnTokumonthViewModel : BaseViewModel
    {
        #region Declare
        public enum PrintType { Spool, CSV }
        public enum ProcessType { Shipment, Store, All }
        public enum TantoType { Sales, Budget }
        [ObservableProperty]
        SearchCondition? condition;
        [ObservableProperty]
        public int flg;
        [ObservableProperty]
        public int flg2;
        #endregion
        #region Initialize
        public void OnInit(object? init_para = null, string? init_flg = null) 
        {
            OnInitBase(init_para, init_flg);
            Condition = new SearchCondition();
            Condition.Tokuiname = "得意先";
            Condition.Date = DateTime.Now;
            Condition.TantoFrom = "0";
            Condition.TantoTo = "99999999";
            Condition.BrdFrom = "0";
            Condition.BrdTo = "99999999";
            Condition.ShopFrom = "0";
            Condition.ShopTo = "99999999";
            Condition.ShowOrNot = "Hidden";
            Condition.FirstOkay = "False";
            Condition.SecondOkay = "False";
            Flg = 0;
            Flg2 = 0;
        }
        #endregion
        #region Function
        [RelayCommand]
        public void SelTanto1(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Condition != null)
            {
                Condition.TantoFrom = get_sel00.Code;
                Condition.TantoFromName = get_sel00.Name;
            }
        }
        [RelayCommand]
        public void SelTanto2(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Condition != null)
            {
                Condition.TantoTo = get_sel00.Code;
                Condition.TantoToName = get_sel00.Name;
            }
        }
        [RelayCommand]
        public void SelBrd1(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Condition != null)
            {
                Condition.BrdFrom = get_sel00.Code;
                Condition.BrdFromName = get_sel00.Name;
            }
        }
        [RelayCommand]
        public void SelBrd2(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Condition != null)
            {
                Condition.ShopTo = get_sel00.Code;
                Condition.ShopToName = get_sel00.Name;
            }
        }
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
        public void ActiveOrNot()
        {
            if (Flg == 0) {
                Condition.FirstOkay = "True";
                Condition.SecondOkay = "True";
                Condition.ShowOrNot = "Visible";
                Flg = 1;
            } else if (Flg == 1) 
            {
                Condition.FirstOkay = "False";
                Condition.SecondOkay = "False";
                Condition.ShowOrNot = "Hidden";
                Flg = 0;
            }
        }
        [RelayCommand]
        public void ChangeTokui() 
        {
            if (Flg2 == 0) {
                Condition.Tokuiname = "請求先";
                Flg2 = 1;
            } else if (Flg2 == 1) {
                Condition.Tokuiname = "得意先";
                Flg2 = 0;
            }
        }        
        [RelayCommand]
        async Task DoPrintAsync()
        {
            BizArray wrk_para = new BizArray();
            //string[] wrk_para = new string[4];
            wrk_para[0] = Condition.DateFrom?.ToString("yyyyMMdd") ?? string.Empty;
            wrk_para[1] = Condition.DateTo?.ToString("yyyyMMdd") ?? string.Empty;
            wrk_para[2] = Condition.ShopFrom.ToString();
            wrk_para[3] = Condition.ShopTo.ToString();
            wrk_para[4] = Condition.TantoFrom.ToString();
            wrk_para[5] = Condition.TantoTo.ToString();
            /* 2008/06/26 ブランド縛り追加 */
            string[] v_brd = null;
            if (Condition.ShowOrNot == "Visible")
            {
                v_brd = new string[2];
                v_brd[0] = Condition.BrdFrom.ToString() ?? string.Empty;
                v_brd[1] = Condition.BrdTo.ToString() ?? string.Empty;
            }

            var v_j1 = "";
            if (AppData.ClassCvnet.config.urikakeflg == 1)
            {
                v_j1 = " and b.掛計上FLG=1 ";
            }

            var v_tanto1 = "(CASE WHEN b.担当者CD = '.' THEN D.営業担当CD ELSE b.担当者CD END)"; /*#54959*/
            var v_tanto2 = "営業担当CD";
            var v_kingaku = "下代金額";
            var v_hiduke = "掛計上日";

            var v_sokbn1 = "";
            if (AppData.ClassCvnet.config.shokbnflg == 1)
            {
                v_sokbn1 = " AND c.商品区分FLG=0 ";
            }

            var sql_str = "select '" + wrk_para[0] + "' 開始日,'" + wrk_para[1] + "' 終了日";
            sql_str += ",a.営業担当CD,NVL((select H.名前 from HC$master_shain h where h.社員CD = a.営業担当CD),'（担当設定無し）') 担当名";
            sql_str += ",a.取引先CD1,a.得意先名,a.数量計,a.金額計,a.粗利計,a.売上数量計,a.売上金額計,a.返品数量計,a.返品金額計,a.値引計";
            sql_str += "," + ((v_brd == null) ? ("'' ブランドCD, '' ブランド名") : ("A.ブランドCD, NVL((SELECT M.名称 FROM HC$MASTER_MEISHO M WHERE M.名称区分='BRD' AND M.名称CD=A.ブランドCD),'') ブランド名"));
            sql_str += ",decode(a.金額計,0,0,round(a.粗利計/a.金額計*100,1)) 粗利率";
            sql_str += "," + ((Condition.Tokuiname == "請求先") ? "'請求先'" : "'得意先'") + " 出力単位";
            sql_str += " from (";
            sql_str += "select " + ((Condition.SelectedTanto.ToString() == "営業担当別予算マスタ") ? "NVL(E.営業担当CD,'.') 営業担当CD" : v_tanto1 + v_tanto2);
            sql_str += "," + ((Condition.Tokuiname == "請求先") ? "d.請求先CD" : "b.取引先CD1") + " 取引先CD1";
            sql_str += "," + ((Condition.Tokuiname == "請求先") ? "nvl((select t.得意先名 from hc$master_tokui t where t.得意先CD=d.請求先CD),'')" : "min(D.得意先名)") + " 得意先名";
            sql_str += " ,sum(decode(trunc(取引区分/10),1,1,2,-1,4,1,0)*a.数量) 数量計";

            sql_str += ",sum(DECODE(TRUNC(取引区分/10),1,1,2,-1,3,-1,4,1,0) * a." + v_kingaku + ") 金額計";

            if (AppData.ClassCvnet.config.torikin == 1)
            {
                sql_str += ",sum(DECODE(TRUNC(取引区分/10),1,1,2,-1,3,-1,4,1,0) * (a." + v_kingaku + "-DECODE(trunc(A.明細取引区分/10),1,1,2,1,3,0,4,1,0) * a.原価金額 )) 粗利計";
            }
            else
            {
                 sql_str += ",sum(DECODE(TRUNC(取引区分/10),1,1,2,-1,3,-1,4,1,0) * (a." + v_kingaku + "-DECODE(trunc(A.明細取引区分/10),1,1,2,1,3,0,4,1,0) * NVL(get_genka(a.商品CD,a.原価FLG,a.在庫計上日,a.色CD,a.サイズCD),0) * a.数量 )) 粗利計";
            }
            sql_str += ",sum(DECODE(TRUNC(取引区分/10),1,1,4,1,0) * a.数量) 売上数量計";
            sql_str += ",sum(DECODE(TRUNC(取引区分/10),1,1,4,1,0) * a." + v_kingaku + ") 売上金額計";
            sql_str += ",sum(DECODE(TRUNC(取引区分/10),2,-1,0) * a.数量) 返品数量計";
            sql_str += ",sum(DECODE(TRUNC(取引区分/10),2,-1,0) * a." + v_kingaku + ") 返品金額計";
            sql_str += ",sum(DECODE(TRUNC(取引区分/10),3,-1,0) * a." + v_kingaku + ") 値引計";

            sql_str += "," + ((v_brd == null) ? "'' ブランドCD" : "C.ブランドCD");

            sql_str += " from HC$tran_tori1 a JOIN HC$tran_tori0 b ON (a.ヘッダNO=b.SEQ_NO " + v_j1 + ")";
            sql_str += " LEFT OUTER JOIN HC$master_shohin C ON (a.商品CD=c.商品CD" + v_sokbn1 + ") JOIN HC$master_tokui D ON (b.取引先CD1=d.得意先CD)";
            sql_str += ((Condition.SelectedTanto.ToString() == "営業担当別予算マスタ") ? " LEFT OUTER JOIN (select E.営業担当CD,E.得意先CD,E.ブランドCD from HC$MASTER_YO_EIGYO E where E.日付 between '" + wrk_para[0] + "' and '" + wrk_para[1] + "') E ON (E.得意先CD=B.取引先CD1 and E.ブランドCD=C.ブランドCD)" : "");

            sql_str += " where B.取引区分 <= 40";

            if (Condition.SelectedProcess.ToString() == "出荷売上")
            {   
                sql_str += " and B.伝票処理区分 = 0 and d.店種区分 in (1)";
            }
            else
            if (Condition.SelectedProcess.ToString() == "店頭売上")
            {   
                sql_str += " and B.伝票処理区分 = 1 and d.店種区分 in (3)";
            }
            else
            if (Condition.SelectedProcess.ToString() == "全て")
            {   
                sql_str += " and ( (B.伝票処理区分 in (0) and d.店種区分 in (1)) or (B.伝票処理区分 in(1) and d.店種区分 in(3)) )";
            }
            sql_str += " and b." + v_hiduke + " between :1 and :2 and " + ((Condition.Tokuiname == "請求先") ? "d.請求先CD" : "b.取引先CD1") + " between :3 and :4";
            sql_str += " and " + ((Condition.SelectedTanto.ToString() == "営業担当別予算マスタ") ? "NVL(E.営業担当CD,'.') " : v_tanto1) + " between :5 and :6";
            /* sql_str += " and " +((Form1.OptionButton3.Value == 1) ? "NVL(E.営業担当CD,'.') " : "D.営業担当CD") + " between :5 and :6"; */
            if (v_brd != null)
            {
                wrk_para[6] = v_brd[0].ToString();
                wrk_para[7] = v_brd[1].ToString();
                sql_str += " and C.ブランドCD between :7 and :8";
            }
            /* sql_str += " group by "+((Form1.OptionButton3.Value == 1) ? "NVL(E.営業担当CD,'.')," : "D.営業担当CD,")+( (v_brd==null)?"":"C.ブランドCD," ) + "b.取引先CD1"; */
            sql_str += " group by " + ((Condition.SelectedTanto.ToString() == "営業担当別予算マスタ") ? "E.営業担当CD," : v_tanto1 + ",") + ((v_brd == null) ? "" : "C.ブランドCD,") + ((Condition.Tokuiname == "請求先") ? "d.請求先CD" : "b.取引先CD1");
            /* sql_str += " group by "+((Form1.OptionButton3.Value == 1) ? "E.営業担当CD," : "D.営業担当CD,")+( (v_brd==null)?"":"C.ブランドCD," ) + "b.取引先CD1"; */
            sql_str += ") a order by a.営業担当CD," + ((v_brd == null) ? "" : "A.ブランドCD,") + "a.取引先CD1";


            /*20160810 #29179*/
            if(Condition.SelectedPrint.ToString() == "CSV"){
                switch (Condition.ShowOrNot.ToString())
                {
                    /* 何基準？ */
                    case "Visible":
                        sql_str = ""
                         + "select "
                         + " concat(concat(SUBSTRB(a.終了日, 1, 4), '年'), concat(substr(a.終了日, 5,2), '月度' ) ) 年月 , "
                        + " a.開始日 開始年月 , "
                        + " a.終了日 終了年月 , "
                        + " a.営業担当CD  , "
                        + " a.担当名 , "
                        + " a.取引先CD1 得意先CD , "
                        + " a.得意先名 得意先 , "
                        + " a.売上数量計 売上数 , "
                        + " a.売上金額計 売上金額 , "
                        + " a.返品数量計 返品数 , "
                        + " a.返品金額計 返品金額 , "
                        + " a.値引計 値引金額 , "
                        + " a.数量計 純売数 , "
                        + " a.金額計 純売金額 , "
                        + " a.粗利計 粗利額 , "
                        + " a.粗利率  "
                         + "  from "
                         + "(" + sql_str + ") a";
                        break;

                    case "Hidden":
                        sql_str = ""
                    + "select"
                    + " concat(concat(SUBSTRB(a.終了日, 1, 4), '年'), concat(substr(a.終了日, 5,2), '月度' ) ) 年月 , "
                    + " a.開始日 開始年月 , "
                    + " a.終了日 終了年月 , "
                    + " a.営業担当CD  , "
                    + " a.担当名 , "
                    + " a.ブランドCD , "
                    + " a.ブランド名 , "
                    + " a.取引先CD1 得意先CD , "
                    + " a.得意先名 得意先 , "
                    + " a.売上数量計 売上数 , "
                    + " a.売上金額計 売上金額 , "
                    + " a.返品数量計 返品数 , "
                    + " a.返品金額計 返品金額 , "
                    + " a.値引計 値引金額 , "
                    + " a.数量計 純売数 , "
                    + " a.金額計 純売金額 , "
                    + " a.粗利計 粗利額 , "
                    + " a.粗利率  "
                     + "  from "
                     + "(" + sql_str + ") a";
                        break;
                }
            }

            var qfm = "cvnetrep201.qfm";
            if (v_brd != null) qfm = "cvnetrep201_brd.qfm";

            var chk_sql = "select count(*) from hc$master_meisho m where m.名称区分='YOK' and m.名称CD='008'";
            var chk_csv = AppData.Http!.AspxSqlQuery(chk_sql, null);
            if (chk_csv.Rows[0][0].ToString() != "0")
            {
                qfm = "cvnetrep201_yk.qfm";
                if (v_brd != null) qfm = "cvnetrep201_brd_yk.qfm";
            }


            var ret_csv = AppData.Http!.AspxSqlQueryCsv(sql_str, wrk_para.ToArray(), qfm);
            var lines = ret_csv.Split('\n');

            if (lines.Length < 2 || lines[1] == "0")
            {
                ClientLib.MessageBoxError(this, "PDFデータがありません");
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
            else if(Condition.SelectedPrint.ToString() == "CSV")
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
                    csv_para.SaveCsv(DateTime.Now.ToString("yyyyMMdd") + "_得意先別売上月報");
                }
                catch (Exception ex) { }
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
            private string? brdFrom;
            [ObservableProperty]
            private string? brdTo;
            [ObservableProperty]
            private string? brdFromName;
            [ObservableProperty]
            private string? brdToName;
            [ObservableProperty]
            private string? tantoFrom;
            [ObservableProperty]
            private string? tantoTo;
            [ObservableProperty]
            private string? tantoFromName;
            [ObservableProperty]
            private string? tantoToName;
            [ObservableProperty]
            private PrintType selectedPrint = PrintType.Spool;
            [ObservableProperty]
            private ProcessType selectedProcess = ProcessType.Shipment;
            [ObservableProperty]
            private TantoType selectedTanto = TantoType.Sales;
            [ObservableProperty]
            private DateTime? dateFrom;
            [ObservableProperty]
            private DateTime? dateTo;
            [ObservableProperty]
            private string? tokuiname;
            [ObservableProperty]
            private string? firstOkay;
            [ObservableProperty]
            private string? secondOkay;
            [ObservableProperty]
            private DateTime? date;
            [ObservableProperty]
            private string? showOrNot;

            partial void OnDateChanged(DateTime? value)
            {
                if (value == null) return;

                var firstDay = new DateTime(value.Value.Year, value.Value.Month, 1);
                var lastDay = firstDay.AddMonths(1).AddDays(-1);

                DateFrom = firstDay;
                DateTo = lastDay;
            }
        }
    }
}
