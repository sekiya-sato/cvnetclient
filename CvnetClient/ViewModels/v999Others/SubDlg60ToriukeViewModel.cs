using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Class;
using CvnetClient.Models;
using CvnetClient.Utils;
using CvnetClient.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg60ToriukeViewModel : BaseViewModel
    {
        public enum List1Type { And,Or}
        public enum List2Type { And,Or}
        public enum OutputType { Spool,CSV}
        public enum SituationType { Enabled, Cancel, All }
        [ObservableProperty]
        private List1Type selectedList1;
        [ObservableProperty]
        private List2Type selectedList2;
        [ObservableProperty]
        private OutputType selectedOutput ;
        [ObservableProperty]
        private SituationType selectedSituation;
        [ObservableProperty]
        ListFlexData listFlexData1 = new ListFlexData();
        [ObservableProperty]
        ListFlexData listFlexData2 = new ListFlexData();
        [ObservableProperty]
        SearchCondition? condition;
        [ObservableProperty]
        Visibility showOrNot;
        [ObservableProperty]
        bool canOrNot;

        public void OnInit(object? init_para = null) 
        {
            OnInitBase(init_para);
            Condition = new SearchCondition();
            AppData.ClassCvnet.AspxSqlQueryImp();
            Condition.DateEdit1 = DateTime.Now;
            Condition.DateEdit2 = DateTime.Now;
            Condition.CodeTen2 = new BtListHelper("99999999", "");
            Condition.CodeTanto2 = new BtListHelper("99999999", "");
            SelectedList1 = List1Type.And;
            SelectedList2 = List2Type.And;
            SelectedOutput = OutputType.Spool;
            SelectedSituation = SituationType.Enabled;
            if (para[0] == "1")
            {
                var tenpo = GetTenpo(AppData.ClassCvnet.SysImp.Rows[1][1].ToString());
                Condition.CodeTen1 = new BtListHelper(tenpo[0], tenpo[1]);
                Condition.CodeTen2 = Condition.CodeTen1;
                ShowOrNot = Visibility.Hidden;
                CanOrNot = false;
            }
            else
            {
                ShowOrNot = Visibility.Visible;
                CanOrNot = true;
            }
            List<CsvItem> def1 = AppData.ClassEtc.Bunrui_List1;
            List<CsvItem> def2 = AppData.ClassEtc.Bunrui_List0;
            ListFlexData1.ListConfig = new ListFlexConfig()
            {
                init_csv = def1 ?? new List<CsvItem>(),
                flag = 1
            };
            ListFlexData2.ListConfig = new ListFlexConfig()
            {
                init_csv = def2 ?? new List<CsvItem>(),
                flag = 0
            };
        }
        [RelayCommand]
        public void SelTenpo1(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Condition != null)
            {
                Condition.CodeTen1 = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        [RelayCommand]
        public void SelTenpo2(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Condition != null)
            {
                Condition.CodeTen2 = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }

        [RelayCommand]
        public void SelTanto1(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Condition != null)
            {
                Condition.CodeTanto1 = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        [RelayCommand]
        public void SelTanto2(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Condition != null)
            {
                Condition.CodeTanto2 = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }

        [RelayCommand]
        public void SelKokyaku(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Condition != null)
            {
                Condition.CodeKokyaku1 = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }

        [RelayCommand]
        async Task DoPrintAsync() 
        {
            var wrk_para = new BizArray();
            int list1 = 0;
            int list2 = 0;
            if (SelectedList1 == List1Type.Or) {
                list1 = 1;
            }
            if (SelectedList2 == List2Type.Or)
            {
                list2 = 1;
            }
            var sql_bunrui = ListFlexData1.GetQueryStr2(wrk_para, ListFlexData1.ListConfig.cnt_start, list1, "MT.");
            var sql_bunrui2 = ListFlexData2.GetQueryStr2(wrk_para, ListFlexData2.ListConfig.cnt_start, list2, "MSH.");

            var date_from = Condition.DateEdit1.ToString("yyyyMMdd");
            var date_to = Condition.DateEdit2.ToString("yyyyMMdd");
            var v_para = new string[9];
            var v_opt = new string[2];

            v_para[0] = new string(date_from);
            v_para[1] = new string(date_to);
            v_para[2] = chkValue(Condition.CodeTen1?.Code);
            v_para[3] = chkValue(Condition.CodeTen2?.Code);
            v_para[4] = chkValue(Condition.CodeTanto1?.Code);
            v_para[5] = chkValue(Condition.CodeTanto2?.Code);
            v_para[6] = chkValue(Condition.CodeKokyaku1?.Code);

            v_para[7] = sql_bunrui;
            v_para[8] = sql_bunrui2;
            if (SelectedSituation == SituationType.Enabled) {
                v_opt[0] = "0";
            } else if (SelectedSituation == SituationType.Cancel) {
                v_opt[0] = "1";
            }
            else {
                v_opt[0] = "2";
            }
            if (SelectedOutput == OutputType.Spool)
            {
                v_opt[1] = "0";
            }
            else
            {
                v_opt[1] = "2";
            }
            var ret_csv = OnQuery(v_para, v_opt);
            var lines = ret_csv.Split('\n');

            if (lines.Length < 2 || lines[1] == "0")
            {
                ClientLib.MessageBoxError(this, "PDFデータがありません");
                return;
            }

            string pdfPath = lines[0];

            if (SelectedOutput == OutputType.Spool)
            {
                string url = AppData.Http!.URLroot + pdfPath + "/data.pdf";

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
                string datapath = AppData.Http!.URLroot + pdfPath + "/data.txt";
                string headpath = AppData.Http!.URLroot + pdfPath + "/d_sql.txt";
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
                    csv_para.SaveCsv(DateTime.Now.ToString("yyyyMMdd") + "_得意先別売上日報");
                }
                catch (Exception ex) { }
            }
        }
        string OnQuery(string[] v_para, string[] v_opt)
        {

            var sql_opt = new string[7];
            sql_opt[0] = v_para[0];
            sql_opt[1] = v_para[1];
            sql_opt[2] = v_para[2];
            sql_opt[3] = v_para[3];
            sql_opt[4] = v_para[4];
            sql_opt[5] = v_para[5];
            sql_opt[6] = v_para[6];

            var sql_query = "select :1 日付FROM,";
            sql_query += " :2 日付TO,";
            sql_query += " :3 店舗FROM,";
            sql_query += " :4 店舗TO,";
            sql_query += " :5 担当者FROM,";
            sql_query += " :6 担当者TO,";
            sql_query += " :7 顧客CDFROM, ";
            sql_query += " ' ' 予備１,";
            if (v_opt[0] == "0")
            {
                sql_query += "'有効' 状態,";
            }
            else
            {
                if (v_opt[0] == "1")
                {
                    sql_query += "'解除' 状態,";
                }
                else
                {
                    sql_query += "'全て' 状態,";
                }
            }
            sql_query += "TH.倉庫CD || ' ' || DECODE(MT.略称,'.',MT.得意先名,MT.略称) 店舗,";
            sql_query += "case TH.完了FLG ";
            sql_query += "	WHEN 0 THEN '有効' ";
            sql_query += "	WHEN 1 THEN '解除'";
            sql_query += "	ELSE ''";
            sql_query += "end 完了FLG,";
            sql_query += "TH.配分指示日 伝票日付,";
            sql_query += "SUBSTR(GET_VDATE(TH.VDATE_CREATE),10,6) 伝票時,";
            sql_query += "TH.伝票NO || '-' || TH.行NO 伝票番号,";
            sql_query += "TH.商品CD 品番,";
            if (SelectedOutput == OutputType.Spool)
            {
                sql_query += "TH.色CD 色,";
                sql_query += "TH.サイズCD サイズ,";
                sql_query += "DECODE(TH.完了FLG,1,-1 * TH.予定数量,TH.予定数量) 数量,";
                sql_query += "DECODE(TH.メモ4, '.', ' ', 	TH.メモ4 || ' ' || nvl((select MK.顧客名 from HC$MASTER_KOKYAKU MK where TH.メモ4 = MK.顧客CD), ' ')) 顧客,";
                sql_query += "DECODE(TH.入力社員CD,'.',' ',TH.入力社員CD || ' ' || nvl((select MS.名前 from HC$MASTER_SHAIN MS where TH.入力社員CD = MS.社員CD),'')) 担当, ";
                sql_query += "TH.メモ1,TH.メモ2 ";
            }
            else
            {
                sql_query += "TH.色CD 色CD,Get_COLORNAME(TH.商品CD,TH.色CD) 色,";
                sql_query += "TH.サイズCD サイズ,";
                sql_query += "DECODE(TH.完了FLG,1,-1 * TH.予定数量,TH.予定数量) 数量,";
                sql_query += "DECODE(TH.メモ4, '.', ' ', 	TH.メモ4) 顧客CD,";
                sql_query += "DECODE(TH.メモ4, '.', ' ', 	nvl((select MK.顧客名 from HC$MASTER_KOKYAKU MK where TH.メモ4 = MK.顧客CD), ' ')) 顧客,";
                sql_query += "DECODE(TH.入力社員CD,'.',' ',TH.入力社員CD) 担当CD, ";
                sql_query += "DECODE(TH.入力社員CD,'.',' ',nvl((select MS.名前 from HC$MASTER_SHAIN MS where TH.入力社員CD = MS.社員CD),'')) 担当, ";
                sql_query += "TH.メモ1,TH.メモ2 ";

            }
            sql_query += "from  HC$TRAN_HIBN1 TH,HC$MASTER_KOKYAKU MK,HC$MASTER_SHAIN MS,HC$MASTER_TOKUI MT,HC$MASTER_SHOHIN_JAN MSJ,HC$MASTER_SHOHIN MSH ";
            sql_query += "where TH.倉庫CD = MT.得意先CD ";
            sql_query += "and TH.入力社員CD = MS.社員CD(+) ";
            sql_query += "and TH.メモ4 = MK.顧客CD(+) ";
            sql_query += "and TH.商品CD= MSH.商品CD ";
            sql_query += "and TH.商品CD= MSJ.商品CD ";
            sql_query += "and TH.色CD= MSJ.色CD ";
            sql_query += "and TH.サイズCD= MSJ.サイズCD ";
            sql_query += ((v_para[7] == "") ? "" : v_para[7]);
            sql_query += ((v_para[8] == "") ? "" : v_para[8]);
            if (int.Parse(v_opt[0].ToString()) == 0)
            {
                sql_query += "and TH.完了FLG = '0' ";
            }
            else
            {
                if (int.Parse(v_opt[0].ToString()) == 1)
                {
                    sql_query += "and TH.完了FLG = '1' ";
                }
            }
            if (v_para[6] != " ")
            {
                sql_query += "and TH.メモ4 = '" + v_para[6] + "' ";
            }
            sql_query += "and TH.入力社員CD between :5 and :6 ";
            sql_query += "and TH.倉庫CD between :3 and :4 ";
            sql_query += "and TH.配分指示日 between :1 and :2 ";
            sql_query += "and TH.区分2 = 60 ";
            sql_query += "order by TH.倉庫CD,TH.配分指示日 DESC,TH.伝票NO DESC,TH.行NO　";

            var qfm_name = "cvnet_toriuke.qfm";

            return AppData.Http!.AspxSqlQueryCsv(sql_query, sql_opt, qfm_name);            
        }
        string chkValue(string? c_val)
        {
            var retVal = "";
            if (c_val == "" || c_val == null)
            {
                retVal = " ";
            }
            else
            {
                retVal = c_val;
            }
            return retVal;
        }

        string[] GetTenpo(string? par_code)
        {
            var str_sql = "";
            var tokuisaki = new string[2];
            str_sql = "SELECT 得意先CD || ',' || 得意先名 FROM HC$MASTER_TOKUI WHERE 得意先CD = '" + par_code + "'";
            var ret_csv = AppData.Http!.AspxSqlQuery(str_sql);
            if (ret_csv.Rows.Count > 0)
            {
                tokuisaki[0] = ret_csv.Rows[0][0].ToString();
                tokuisaki[1] = ret_csv.Rows[0][1].ToString();
            }
            else
            {
                tokuisaki[0] = par_code.Split(" ")[0].ToString();
                tokuisaki[1] = par_code.Split(" ")[1].ToString();
            }
            return tokuisaki;
        }

        public partial class SearchCondition : ObservableObject
        {
            [ObservableProperty]
            private BtListHelper? codeTen1;
            [ObservableProperty]
            private BtListHelper? codeTen2;
            [ObservableProperty]
            private DateTime dateEdit1;
            [ObservableProperty]
            private DateTime dateEdit2;
            [ObservableProperty]
            private BtListHelper? codeTanto1;
            [ObservableProperty]
            private BtListHelper? codeTanto2;
            [ObservableProperty]
            private BtListHelper? codeKokyaku1;
        }
    }
}
