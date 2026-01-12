using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Class;
using CvnetClient.Models;
using CvnetClient.Utils;
using CvnetClient.Views;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg04prnShuk01rNewViewModel : BaseViewModel
    {
        public enum ListType { And, Or }
        public enum PrintType {NoShow,Show}
        public enum OutPutType {Spool,CSV}
        public enum StockType {Actual,Available }
        public enum PriceType {Retail,Unit}
        public enum OutPutContent { WareHouseProduct, Product, ProductWareHouse }
        [ObservableProperty]
        private PrintType selectedPrint = PrintType.Show;
        [ObservableProperty]
        private OutPutType selectedOutPut = OutPutType.Spool;
        [ObservableProperty]
        private PriceType selectedPrice = PriceType.Retail;
        [ObservableProperty]
        private StockType selectedStock = StockType.Actual;
        [ObservableProperty]
        private OutPutContent selectedContent = OutPutContent.WareHouseProduct;
        [ObservableProperty]
        ListFlexData listFlexData1 = new ListFlexData();
        [ObservableProperty]
        ListFlexData listFlexData2 = new ListFlexData();        
        [ObservableProperty]
        private ListType selectedList1;
        [ObservableProperty]
        private ListType selectedList2;
        [ObservableProperty]
        public Dictionary<string, string> comboListSupplier;
        [ObservableProperty]
        public Dictionary<string, string> comboListShukei;
        [ObservableProperty]
        SearchCondition? condition;
        [ObservableProperty]
        private string menuFlg;
        public void OnInit(object? init_para = null,string? init_flg = null) 
        {
            OnInitBase(init_para, init_flg);
            AppData.ClassCvnet.AspxSqlQueryImp();
            if (AppData.ClassCvnet.SysImp.Rows[1][1].ToString() == "")
            {
                Condition.CodeTenpo1 = new BtListHelper(AppData.ClassCvnet.SysImp.Rows[0][1].ToString(),AppData.ClassCvnet.SysImp.Rows[0][2].ToString());
                Condition.CodeTenpo2 = new BtListHelper(AppData.ClassCvnet.SysImp.Rows[0][1].ToString(),AppData.ClassCvnet.SysImp.Rows[0][2].ToString());
            }
            else
            {
                Condition.CodeTenpo1 = new BtListHelper(AppData.ClassCvnet.SysImp.Rows[1][1].ToString(),AppData.ClassCvnet.SysImp.Rows[1][2].ToString());
                Condition.CodeTenpo2 = new BtListHelper(AppData.ClassCvnet.SysImp.Rows[1][1].ToString(),AppData.ClassCvnet.SysImp.Rows[1][2].ToString());
            }

            var sqlstr = "select 値 ";
            sqlstr += " from hc$master_config";
            sqlstr += " where  カテゴリ = '移動区分' and フラグ名 = 'ido_disp'";
            var ret_csv = AppData.Http!.AspxSqlQuery(sqlstr);
            if (ret_csv.Rows.Count > 0)
            {
                //Form1.OptionButton1.Value = ret_csv.Rows[0][0];
            }
            else
            {
                //Form1.OptionButton1.Value = AppData.ClassCvnet.config.ido_disp;
            }
            if (AppData.ClassCvnet.config.sokohyjflg == 1)
            {
                //Form1.OptionButton2.OptionItem1[0].Title = "店舗商品別";
                //Form1.OptionButton2.OptionItem1[2].Title = "商品店舗別";
            }

            if (para[0] == "1")
            {
                if (AppData.ClassSatoo.SHAIN_Tenpo != "" && AppData.ClassSatoo.SHAIN_Tenpo != ".")
                {
                    var sql_str1 = "select t.得意先cd,t.得意先名 from hc$master_tokui t where t.得意先CD=:1";
                    var v_para1 = new string[1];
                    v_para1[0] = AppData.ClassSatoo.SHAIN_Tenpo;
                    var ret_Csv = AppData.Http!.AspxSqlQuery(sql_str1, v_para1);
                    if (ret_csv.Rows.Count > 0)
                    {
                        Condition.CodeTenpo1 = new BtListHelper(AppData.ClassSatoo.SHAIN_Tenpo,ret_csv.Rows[0][1].ToString());
                        Condition.CodeTenpo2 = new BtListHelper(AppData.ClassSatoo.SHAIN_Tenpo,ret_csv.Rows[0][1].ToString());
                    }
                    //Form1.CodeTenpo1.Active =$false;
                    //Form1.CodeTenpo2.Active =$false;
                    //Form1.Button3.Active =$false;
                    //Form1.Button3.Visible =$false;
                    //Form1.Button4.Active =$false;
                    //Form1.Button4.Visible =$false;
                }
            }

            ret_csv = AppData.ClassCvnet.GetMeiList_Shohin();
            if (ret_csv.Rows.Count == 0)
            {
                ret_csv = new DataTable();
                ret_csv.Columns.Add();
            }
            
            //Form1.CodeTotal1.ComboItem1 << ret_csv;
            //if (ret_csv.rows > 1)
            //{
            //    Form1.CodeTotal1.Value = Form1.CodeTotal1.ComboItem1[10].Value;
            //}
            //else
            //{
            //    Form1.CodeTotal1.OnSet("-1");
            //}

            List<CsvItem> def1 = AppData.ClassEtc.Bunrui_List0;
            List<CsvItem> def2 = AppData.ClassEtc.Bunrui_List1;
            ListFlexData1.ListConfig = new ListFlexConfig()
            {
                init_csv = def1 ?? new List<CsvItem>(),
                flag = 0
            };
            ListFlexData2.ListConfig = new ListFlexConfig()
            {
                init_csv = def2 ?? new List<CsvItem>(),
                flag = 1
            };
            if (init_flg != null)
            {
                MenuFlg = init_flg;
                if (init_flg == "99")
                {
                    //Form1.Label12.Visible = $FALSE;
                    //Form1.GroupBox5.Visible = $FALSE;
                    //Form1.OptionButton4.Visible = $FALSE;
                }
            }
            var v_para = new string[1];
            v_para[0] = "汎用在庫表";
            var sql_str = ""
            + "SELECT DISTINCT "
                + "first_value(m.ランク) over (order by m.名称CD) 集計項目1"
            + " FROM "
                + "HC$MASTER_MEISHO m"
            + " WHERE "
                + "m.名称区分 = 'MPK' AND "
                + "m.名称 = :1";
            ret_csv = AppData.Http!.AspxSqlQuery(sql_str, v_para);
            if (ret_csv.Rows.Count > 0)
            {
                //Form1.CodeTotal1.OnSet(ret_csv.getCell(0, 0));
            }

            if (AppData.ClassCvnet.config.usegenka == 0)
            {
                //Form1.CheckBox1.Visible =$false;
                //Form1.CheckBox1.Active =$false;
            }
        }
        [RelayCommand]
        async Task DoPrintAsync()
        {
            var v_tan1 = Condition.CodeTan1.Code ?? string.Empty;
            var v_tan2 = Condition.CodeTan2.Code ?? string.Empty;


            var wrk_para2 = new BizArray();
            /* 2017.06.29  倉庫分類(得意先分類)条件追加のため修正 */
            /* var v_sql = Form1.ListFlexView1.GetQueryStr2( wrk_para2, 1,^.OptionButton5.Value ); */
            var v_sql = ListFlexData1.GetQueryStr2(wrk_para2, 1, /*^.OptionButton7.Value*/0, "B.");
            var wrk_para = new string[20];
            wrk_para[0] = v_sql;
            wrk_para[1] = Condition.CodeTenpo1.Code;
            wrk_para[2] = Condition.CodeTenpo2.Code;
            //if (Form1.CheckBox1.CheckItem1[0].Selected == false)
            //{
            //    wrk_para[3] = "0";
            //}
            //else
            //{
            //    wrk_para[3] = "1";
            //}
            wrk_para[4] = Condition.Text1;
            wrk_para[5] = Condition.Text2;
            //wrk_para[6] = new String(^.OptionButton1.Value);
            //var v_matu = ClassSatoo.getDateVal(^.CodeNen.Value, 1);
            //wrk_para[7] = new String(Mid(v_matu, 0, 4) + Mid(v_matu, 5, 2) + Mid(v_matu, 8, 2));
            wrk_para[8] = Condition.CodeHin1;
            wrk_para[9] = Condition.CodeHin2;
            wrk_para[10] = Condition.CodeTotal1;
            //wrk_para[11] = new String(^.OptionButton3.Value);
            wrk_para[12] = Condition.CodeNen1.ToString("yyyyMMdd");
            wrk_para[13] = Condition.CodeNen.ToString("yyyyMMdd");

            /*2010/06/29 検索条件単品NO追加*/
            wrk_para[14] = new string(v_tan1);
            wrk_para[15] = new string(v_tan2);

            //wrk_para[16] = new String(^.OptionButton6.Value);

            wrk_para[17] = MenuFlg;

            /* 出力区分追加 16.07.08 */
            //wrk_para[18] = new String(^.OptionButton4.Value);

            /* 2017.06.29  倉庫分類(得意先分類)条件追加 */
            var wrk_para3 = new BizArray();
            var v_sql2 = ListFlexData2.GetQueryStr2(wrk_para3, 1, /*^.OptionButton5.Value*/ 0, "t.");
            wrk_para[19] = v_sql2;


            var ret_csv = OnQuery(wrk_para);
            var lines = ret_csv.Split('\n');

            if (lines.Length < 2 || lines[1] == "0")
            {
                ClientLib.MessageBoxError(this, "印刷データがありませんでした");
                return;
            }
            string pdfPath = lines[0];

            /* ｽﾌﾟｰﾙ or CSV */
            if (SelectedOutPut == OutPutType.Spool)
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
               
                var ret_name = AppData.Http!.AspxSqlQuery("select m.名称 from HC$MASTER_MEISHO m where m.名称区分='IDX' and m.名称CD between 'B01' and 'B10' order by m.名称CD");
                var name_cnt = 0;
                var name_flg = 0;

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
                using var http = new HttpClient();
                var headerBytes = await http.GetByteArrayAsync(headpath);
                string headerText = Encoding.GetEncoding("shift_jis").GetString(headerBytes);
                string[] headerCells = headerText.Trim().Split(',');
                DataTable get_hedder = new DataTable();
                foreach (var col in headerCells)
                    get_hedder.Columns.Add(col.Trim());
                
                var get_csv = csv_para.GetTable();
                for (var i = 0; i < get_csv.Columns.Count; i++)
                {

                    if (SelectedPrint == PrintType.Show && get_hedder.Rows[0][i].ToString().Substring(0,4) == "名称CD" && ret_name.Rows.Count == 10)
                    {
                        if (name_flg == 0)
                        {
                            get_csv.Rows[0][i] = ret_name.Rows[name_cnt][0].ToString() + "CD";
                            name_flg = 1;
                        }
                        else
                        {
                            get_csv.Rows[0][i] = ret_name.Rows[name_cnt][0].ToString() + "名";
                            name_cnt++;
                            name_flg = 0;
                        }
                    }
                    else
                    {
                        get_csv.Rows[0][i] = get_hedder.Rows[0][i];
                    }
                }
                try
                {
                    csv_para.SaveCsv(DateTime.Now.ToString("yyyyMMdd") + "-在庫表");
                }
                catch (Exception ex) 
                {
                    ClientLib.MessageBoxError(this, "保存を中止しました" + ex.Message);
                }               
            }
        }
        string OnQuery(string[] wrk_para) 
        {
            var chk_sql = "select count(*) from hc$master_meisho m where m.名称区分='YOK' and m.名称CD='007'";
            var chk_csv = AppData.Http!.AspxSqlQuery(chk_sql, null);

            var sql_str = "_zaiko_101_r_new";
            var qfm_file = "cvnettana101_r.qfm";
            if (SelectedPrint == PrintType.Show) qfm_file = "cvnettana101b_r.qfm";
            if (chk_csv.Rows[0][0].ToString() == "1")
            {
                qfm_file = "cvnettana101_r_yk.qfm";
                if (SelectedPrint == PrintType.Show) qfm_file = "cvnettana101b_r_yk.qfm";
            }

            if (SelectedContent == OutPutContent.Product)
            {
                sql_str = "_zaiko_102_r_new";
                qfm_file = "cvnettana102_r.qfm";
                if (SelectedPrint == PrintType.Show) qfm_file = "cvnettana102b_r.qfm";
                if (chk_csv.Rows[0][0].ToString() == "1")
                {
                    qfm_file = "cvnettana102_r_yk.qfm";
                    if (SelectedPrint == PrintType.Show) qfm_file = "cvnettana102b_r_yk.qfm";
                }
            }

            /* 商品CD倉庫別 */
            if (SelectedContent == OutPutContent.ProductWareHouse)
            {
                sql_str = "_zaiko_103_r_new";
                qfm_file = "cvnettana103_r.qfm";
                if (SelectedPrint == PrintType.Show) qfm_file = "cvnettana103b_r.qfm";
                if (chk_csv.Rows[0][0].ToString() == "1")
                {
                    qfm_file = "cvnettana103_r_yk.qfm";
                    if (SelectedPrint == PrintType.Show) qfm_file = "cvnettana103b_r_yk.qfm";
                }
            }

            return AppData.Http!.AspxSqlQueryCsv(sql_str, wrk_para, qfm_file, 01);
        }
        public partial class SearchCondition : ObservableObject
        {
            [ObservableProperty]
            private BtListHelper codeTenpo1;
            [ObservableProperty]
            private BtListHelper codeTenpo2;
            [ObservableProperty]
            private BtListHelper codeTan1;
            [ObservableProperty]
            private BtListHelper codeTan2;
            [ObservableProperty]
            private DateTime codeNen;
            [ObservableProperty]
            private DateTime codeNen1;
            [ObservableProperty]
            private string codeHin1;
            [ObservableProperty]
            private string codeHin2;
            [ObservableProperty]
            private string codeTotal1;
            [ObservableProperty]
            private string text1;
            [ObservableProperty]
            private string text2;
        }
    }
}
