using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Models;
using CvnetClient.Utils;
using CvnetClient.Views;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Windows;
using System.Windows.Media;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg13Nohin2v3150ViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string brand = "";
        [ObservableProperty]
        private string item1 = "";
        [ObservableProperty]
        private string item2 = "";
        [ObservableProperty]
        private string monthYear = DateTime.Now.ToString("yyyy/MM");
        [ObservableProperty]
        private string category1 = "";
        [ObservableProperty]
        private string category2 = "";


        [ObservableProperty]
        private int[] retailPrice = new int[5];
        [ObservableProperty]
        private int retailPriceTotal;

        //[ObservableProperty]
        //private ObservableCollection<int> retailPrice = new() {0, 0, 0, 0, 0 };

        //[ObservableProperty]
        //private int retailPriceTotal;


        [ObservableProperty]
        private int[] amountSheets = new int[5];
        [ObservableProperty]
        private int totalSheets;

        [ObservableProperty]
        private int[] costPrice = new int[5];
        [ObservableProperty]
        private int totalCostPrice;

        [ObservableProperty]
        private string[] weekLabel = new string[5];

        [ObservableProperty]
        private string weekLabel1;
        [ObservableProperty]
        private string weekLabel2;
        [ObservableProperty]
        private string weekLabel3;
        [ObservableProperty]
        private string weekLabel4;
        [ObservableProperty]
        private string weekLabel5;

        [ObservableProperty]
        private int selectedWeek;

        [ObservableProperty]
        private string?[]? day_range = new string?[4];

        [ObservableProperty]
        private bool[] bt_ColSz = new bool[4];
        

        public void OnInit(object? init_para = null, string? init_flg = null)
        {
            OnInitBase(init_para, init_flg);
            if (para[0] != null)
            {
                for (var i = 0; i < 5; i++)
                {
                    weekLabel[i] = "第" + (i + 1).ToString() + "週 (" + (para[i * 2]).Substring(6, 10) + "～" + para[i * 2 + 1].Substring(6, 10) + ")";

                    day_range[0] = para[i * 2];
                    day_range[1] = para[i * 2 + 1];
                    day_range[2] = para[i * 2];
                    day_range[3] = para[i * 2 + 1];
                    /* 最終日を取得 */
                    if (para[i * 2 + 1] != "")
                    {
                        //lst_days[i] = new String(init_para[i * 2 + 1]);
                        //lst_day = init_para[i * 2 + 1];
                        //max = i;
                    }
                    else
                    {
                        bt_ColSz[i] = false;
                    }
                }
            }
            MonthYear = para[15];
            Brand = para[10];
            Item1 = para[11];
            Item2 = para[12];
            Category1 = para[13];
            Category2 = para[14];
            JodaiGenka();
        }

        public void JodaiGenka()
        {
            var v_sql = ""
            + " select sum(上代 * 数量) 上代合計,"
            + " 	sum(数量) 枚数,"
            + " 	sum(原価 * 数量) 原価合計,"
            + " 	開始日,"
            + " 	終了日"
            + " from (select s.商品CD,"
            + " 		decode(s.予備17, '.', s.上代, '0', s.上代, s.予備17) 上代,"
            + "        get_genka(s.商品CD,0,t0.納品日,t1.色CD,t1.サイズCD) 原価,"
            + " 		t1.数量,"
            + " 		NVL((select w1.開始日"
            + " 			from HC$MASTER_WEEK w1"
            + " 			where t0.納品日 between w1.開始日 and w1.終了日), 0) 開始日,"
            + " 		NVL((select w1.終了日"
            + " 			from HC$MASTER_WEEK w1"
            + " 			where t0.納品日 between w1.開始日 and w1.終了日), 0) 終了日"
            + " 	from HC$MASTER_SHOHIN s,"
            + ((para[18] == "1") ? " HC$TRAN_VTORI0 t0," : " HC$TRAN_TORI0 t0,") 
            + ((para[18] == "1") ? " HC$TRAN_VTORI1 t1" : " HC$TRAN_TORI1 t1") 
            + " 	where t0.SEQ_NO = t1.ヘッダNO"
            + ((para[18] == "1") ? " and t0.伝票処理区分 = 3" : " and t0.伝票処理区分 = 13") 
            + " 	and t1.商品CD = s.商品CD"
            + " 	and t0.納品日 between :1 and :2 and s.ブランドCD = :3 and s.アイテムCD"
            + " 		between :4 and :5 and t0.取引区分 between :6 and :7) a"
            + " group by 開始日, 終了日"
            + " order by 開始日, 終了日"
            + "";

            var v_para = new BizArray();
            v_para[0] = para[16];
            v_para[1] = para[17];
            v_para[2] = para[10];
            v_para[3] = para[11];
            v_para[4] = para[12];
            v_para[5] = para[13];
            v_para[6] = para[14];

            var ret_csv = AppData.Http!.AspxSqlQuery(v_sql, v_para.ToArray());

            if (ret_csv.Rows.Count > 0)
            {
                var total_jodai = 0;
                var total_mai = 0;
                var total_gedai = 0;

                var RetailPrice = 0;
                var AmountSheets = 0;
                var CostPrice = 0;

                for (var i = 0; i < 5; i++)
                {
                    var v_start_day = day_range[0];
                    var v_last_day = day_range[1];



                    for (var j = 0; j < ret_csv.Rows.Count; j++)
                    {
                        var r_start_day = ret_csv.Rows[j][3];
                        var r_last_day = ret_csv.Rows[j][4];

                        

                        if (ret_csv.Rows[j][0] != DBNull.Value) { RetailPrice = Convert.ToInt32(ret_csv.Rows[j][0]); }
                        if (ret_csv.Rows[j][1] != DBNull.Value) { AmountSheets = Convert.ToInt32(ret_csv.Rows[j][0]); }
                        if (ret_csv.Rows[j][2] != DBNull.Value) { CostPrice = Convert.ToInt32(ret_csv.Rows[j][0]); }

                        if (r_start_day == v_start_day || r_last_day == v_last_day)
                        {
                            retailPrice[i] = RetailPrice;
                            amountSheets[i] = AmountSheets;
                            costPrice[i] = CostPrice;

                            total_jodai += retailPrice[i];
                            total_mai += amountSheets[i];
                            total_gedai += costPrice[i];
                        }
                    }
                }
                RetailPriceTotal = total_jodai;
                TotalSheets = total_mai;
                TotalCostPrice = total_gedai;
            }
        }

        //[RelayCommand]
        //async Task DownloadCSVAsync()
        //{
        //    var wrk_para2 = new BizArray();
        //    var wrk_para = new BizArray();

        //    int total_cnt = 0;
        //    int flg = 0;

        //    wrk_para2[0] = ((int)selectedFirst).ToString();
        //    wrk_para2[1] = ((int)selectedSecond).ToString();
        //    wrk_para2[2] = ((int)selectedThird).ToString();

        //    string sql_str = " select t0.納品日 展開日,"
        //+ " 	s.アイテムCD 大分類CD,"
        //+ " 	NVL((select m.名称"
        //+ " 		from HC$MASTER_MEISHO m"
        //+ " 		where m.名称区分 = 'ITM'"
        //+ " 		and m.名称CD = s.アイテムCD), '') 大分類名,"
        //+ " 	s.商品CD 商品CD,"
        //+ " 	max(s.商品名) 商品名,"
        //+ " 	t1.色CD 色CD,"
        //+ " 	max(get_colorname(t1.色CD)) 色名,"
        //+ " 	t1.サイズCD サイズCD,"
        //+ " 	max(get_sizename(t1.商品CD, t1.サイズCD)) サイズ名,"
        //+ " 	sum(t1.数量) 発注数,"
        //+ " 	max(s.上代) 上代,"
        //+ " 	max(decode(s.予備17, '.', 0, s.予備17)) \"セール予定金額\","
        //+ " 	t0.納品日 希望納期"
        //+ " from HC$MASTER_SHOHIN s,"
        //+ ((para[18] == "1") ? " HC$TRAN_VTORI0 t0," : " HC$TRAN_TORI0 t0,") 
        //+ ((para[18] == "1") ? " HC$TRAN_VTORI1 t1" : " HC$TRAN_TORI1 t1") 
        //+ " where t0.SEQ_NO = t1.ヘッダNO"
        //+ ((para[18] == "1") ? " and t0.伝票処理区分 = 3" : " and t0.伝票処理区分 = 13") 
        //+ " and t1.商品CD = s.商品CD"
        //+ " and t0.納品日 between :1 and :2 and s.ブランドCD = :3 and s.アイテムCD"
        //+ " 	between :4 and :5 and t0.取引区分 between :6 and :7"
        //+ " group by t0.納品日, s.アイテムCD, s.商品CD, t1.色CD, t1.サイズCD"
        //+ " order by s.アイテムCD, t0.納品日, s.商品CD, t1.色CD, t1.サイズCD";

        //    if (!ClientLib.MessageBox(this, "CSVファイルを作成しますか？")) return;

        //    var f_name = SelectedOutPut.ToString();
        //    var ret_csv = AppData.Http!.AspxSqlQueryCsv(sql_str, wrk_para, "cvnet13prn_header.qfm");

        //    var lines = ret_csv.Split('\n');

        //    if (lines.Length < 2 || lines[1] == "0")
        //    {
        //        ClientLib.MessageBoxError(this, "PDFデータがありません");
        //        return;
        //    }

        //    string pdfPath = lines[0];
        //    string url = AppData.Http!.URLroot + pdfPath + "/data.pdf";

        //    var ret_name = AppData.Http!.AspxSqlQuery("select m.名称 from HC$MASTER_MEISHO m where m.名称区分='IDX' and m.名称CD between 'B01' and 'B10' order by m.名称CD");
        //    var name_cnt = 0;
        //    var name_flg = 0;

        //    string datapath = AppData.Http.URLroot + pdfPath + "/data.txt";
        //    string headpath = AppData.Http.URLroot + pdfPath + "/d_sql.txt";
        //    bool ready = await Utils.GlobalFunc.WaitForPdfAsync(datapath, TimeSpan.FromSeconds(30));
        //    if (!ready)
        //    {
        //        ClientLib.MessageBoxError(this, "Data生成に時間がかかりすぎています。\n 条件を絞ってください。");
        //        return;
        //    }

        //    ready = await Utils.GlobalFunc.WaitForPdfAsync(headpath, TimeSpan.FromSeconds(30));
        //    if (!ready)
        //    {
        //        ClientLib.MessageBoxError(this, "Header生成に時間がかかりすぎています。\n 条件を絞ってください。");
        //        return;
        //    }

        //    var header_csv = new BizCsvDocument();
        //    await header_csv.LoadHeaderFromUrl(headpath);
        //    var get_csv = new BizCsvDocument();
        //    await get_csv.LoadFromUrlAsync(datapath, headpath);
        //    var dt = get_csv.GetTable();
        //    var dt_header = header_csv.GetTable();
        //    dt.Rows.InsertAt(dt.NewRow(), 0);

        //    for (var i = 0; i < dt.Columns.Count; i++)
        //    {
        //        /* 2021.04.27 商品名称CDのラベル付け */
        //        if (SelectedOutPut == OutPutType.Detail && dt_header.Rows[0][i].ToString().Substring(0, 4) == "名称CD" && ret_name.Rows.Count == 10)
        //        {
        //            if (name_flg == 0)
        //            {
        //                dt.Rows[0][i] = ret_name.Rows[name_cnt][0].ToString() + "CD";
        //                name_flg = 1;
        //            }
        //            else
        //            {
        //                dt.Rows[0][i] = ret_name.Rows[name_cnt][0].ToString() + "名";
        //                name_cnt++;
        //                name_flg = 0;
        //            }
        //        }
        //        else
        //        {
        //            dt.Rows[0][i] = dt_header.Rows[0][i];
        //        }
        //    }
        //    get_csv = new BizCsvDocument(dt);
        //    var str = get_csv.SaveStr(1);
        //    get_csv = new BizCsvDocument(str, 1);

        //    if (AppData.ClassCvnet.config.ExcelOutFlg == 1)
        //    {

        //        var dtCSVFlNm = "";
        //        var ExcelFlNm = "";

        //        try
        //        {

        //            if (f_name == "一覧")
        //            {
        //                dtCSVFlNm = "(" + this.day_range[0] + "-" + this.day_range[1] + ")-MDマップ.csv" );
        //                ExcelFlNm = "HatchuIchiran_macro.xlsm";
        //            }
        //            else if (f_name == "明細")
        //            {
        //                dtCSVFlNm = "DataHatchuMeisai.csv";
        //                ExcelFlNm = "HatchuMeisai_macro.xlsm";
        //            }

        //            get_csv.SaveCsv(dtCSVFlNm);

        //        }
        //        catch (Exception ex)
        //        {
        //            ClientLib.MessageBoxError(this, "保存を中止しました");
        //        }

        //    }
        //    else
        //    {

        //        var v_title = "発注伝票";
        //        if (MenuFlg == "1") v_title = "入荷予定伝票";
        //        try
        //        {
        //            get_csv.SaveCsv(v_title + f_name);

        //        }
        //        catch (Exception ex)
        //        {
        //            ClientLib.MessageBoxError(this, "保存を中止しました");
        //        }

        //    }
        //}
    }
}
