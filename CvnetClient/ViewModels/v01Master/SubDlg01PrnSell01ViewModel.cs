using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Class;
using CvnetClient.Models;
using CvnetClient.Utils;
using CvnetClient.Views;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Windows;
using System.Windows.Media;
using static CvnetClient.ViewModels.SubDlg01SetjanViewModel;
using static CvnetClient.ViewModels.SubDlg02PrnSiiredtyViewModel;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg01PrnSell01ViewModel : BaseViewModel
    {
        [ObservableProperty]
        ListFlexData listFlexData = new ListFlexData();

        [ObservableProperty]
        BtListHelper? selectedShop;

        [ObservableProperty]
        BtListHelper? selectedProduct1;

        [ObservableProperty]
        BtListHelper? selectedProduct2;

        [ObservableProperty]
        BtListHelper? selectedSales1;

        [ObservableProperty]
        BtListHelper? selectedSales2;

        [ObservableProperty]
        public DateTime changeDate1;

        [ObservableProperty]
        public DateTime changeDate2;

        public enum OutputCategory
        { All = 0, ProperWithSalesMaster = 1, ProperNoSalesMaster = 2, Sales = 3 }

        [ObservableProperty]
        private OutputCategory selectedOutput = OutputCategory.All;

        public enum OutputType
        { CSV = 0, Spool = 1 }

        [ObservableProperty]
        private OutputType selectedType = OutputType.Spool;

        [ObservableProperty]
        private bool isSalesEnabled = true;

        [ObservableProperty]
        private bool isShopEnabled = true;

        public void OnInit(object? init_para = null, string? init_flg = null)
        {
            OnInitBase(init_para, init_flg);

            AppData.ClassCvnet.AspxSqlQueryImp();
            if (AppData.ClassCvnet.SysImp.Rows[1][1] == "") { }
            else {
                
            }

            List<CsvItem> def = new List<CsvItem>()
            {
                new CsvItem { col01 = "ﾌﾞﾗﾝﾄﾞ" },
                new CsvItem { col01 = "大分類" },
            };

            if (AppData.ClassCvnet.config.smtflg==1) 
            {
                def = new List<CsvItem>()
                {
                    new CsvItem { col01 = "仕入先" },
                    new CsvItem { col01 = "ｱｲﾃﾑ" },
                };
            }


            ListFlexData.ListConfig = new ListFlexConfig()
            {
                init_csv = def ?? new List<CsvItem>(),
                flag = 0
            };

            if (AppData.ClassCvnet.config.SellMFlg == 0)
            {
                IsSalesEnabled = false;
            }

            if (para.Count > 0)
            {
                if (para[0] == "1")
                {
                    if (AppData.ClassSatoo.SHAIN_Tenpo != "" && AppData.ClassSatoo.SHAIN_Tenpo != ".")
                    {
                        var sql_str = "select t.得意先cd,t.得意先名 from hc$master_tokui t where t.得意先CD=:1";
                        var v_para = new BizArray();
                        v_para[0] = new string(AppData.ClassSatoo.SHAIN_Tenpo);
                        var ret_Csv = AppData.Http?.AspxSqlQuery(sql_str, v_para.ToArray());
                        if (ret_Csv != null || ret_Csv.Rows.Count > 0)
                        {
                            SelectedShop = new BtListHelper(AppData.ClassSatoo.SHAIN_Tenpo, ret_Csv.Rows[0][1].ToString());
                        }
                        IsShopEnabled = false;
                    }
                }
            }

            para = new BizArray();
            SelectedProduct1 = new BtListHelper(".", "");
            SelectedProduct2 = new BtListHelper("ZZZZZZZZZZZZZZZZZZZZ", "ZZZZZZZZZZZZZZZZZZZZ");
            SelectedSales1 = new BtListHelper(".", "");
            SelectedSales2 = new BtListHelper("ZZZZZZZZ", "ZZZZZZZZ");
            ChangeDate1 = DateTime.Now.AddDays(-7);
            ChangeDate2 = DateTime.Now;
        }

        [RelayCommand]
        async Task DoPrintAsync()
        {
            if (!ClientLib.MessageBox(this, "印刷しますか？")) return;
            ClientLib.CursorToWait();
            if (SelectedShop == null || string.IsNullOrEmpty(SelectedShop.Code))
            {
                ClientLib.MessageBoxError(this, "店舗CDを入力してください");
                return;
            }

            int paraCnt = 6;
            var v_para = new BizArray();
            v_para[0] = SelectedShop.Code;
            v_para[1] = ChangeDate1.ToString("yyyyMMdd");
            v_para[2] = ChangeDate2.ToString("yyyyMMdd");
            v_para[3] = SelectedProduct1 == null ? SelectedProduct1.Code : ".";
            v_para[4] = SelectedProduct2 == null ? SelectedProduct2.Code : "ZZZZZZZZZZZZZZZZZZZZ";
            if (AppData.ClassCvnet.config.SellMFlg == 1) 
            {
                paraCnt = 8;
                v_para[5] = SelectedSales1 == null ? SelectedSales1.Code : ".";
                v_para[6] = SelectedSales2 == null ? SelectedSales2.Code : "ZZZZZZZZ";
            }

            string flex_sql = ListFlexData.GetQueryStr(v_para, paraCnt, 0,"s.");

            var ret_csv = OnQuery(v_para, flex_sql);
            
            var lines = ret_csv.Split('\n');
            if (lines.Length < 2 || lines[1] == "0")
            {
                ClientLib.MessageBoxError(this, "印刷データがありませんでした");
                return;
            }
            string pdfPath = lines[0];

            if (SelectedType == OutputType.Spool)
            {

                await Task.Delay(1500); // PDF生成待ち

                string url = AppData.Http.URLroot + pdfPath + "/data.pdf";

                bool ready = await Utils.GlobalFunc.WaitForPdfAsync(url, TimeSpan.FromSeconds(1500));
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
                bool ready = await Utils.GlobalFunc.WaitForPdfAsync(datapath, TimeSpan.FromSeconds(1500));
                if (!ready)
                {
                    ClientLib.MessageBoxError(this, "Data生成に時間がかかりすぎています。\n 条件を絞ってください。");
                    return;
                }

                ready = await Utils.GlobalFunc.WaitForPdfAsync(headpath, TimeSpan.FromSeconds(1500));
                if (!ready)
                {
                    ClientLib.MessageBoxError(this, "時間がかかりすぎです。\n 条件を絞ってください。");
                    return;
                }
                await csv_para.LoadFromUrlAsync(datapath, headpath);
                try
                {
                    csv_para.SaveCsv(DateTime.Now.ToString("yyyyMMdd") + "-売価確認表.csv");
                }
                catch (Exception ex) { }
            }

        }

        string OnQuery(BizArray wrk_para, string flex_sql) 
        {
            string zaiko_tbl = "HC$MANAGE_ZAIKO";
            string zaiko_s = "当月在庫数";
            if (AppData.ClassCvnet.config.Real_Zaiko == 1)
            {
                zaiko_tbl = "HC$MANAGE_REAL_ZAIKO";
                zaiko_s = "在庫数";
            }
            else if (AppData.ClassCvnet.config.Rk_Zaiko == 1)
            {
                zaiko_tbl = "HC$VIEW_ZAIKORK";
            }

            string kbn_title = ",'全て' title";
            string kbn_sql = "";
            string kbn_sql2 = "";
            switch (SelectedOutput)
            {
                case OutputCategory.ProperNoSalesMaster :
                    kbn_title = ",'ﾌﾟﾛﾊﾟｰ（ｾｰﾙﾏｽﾀ無）' title";
                    break;
                case OutputCategory.ProperWithSalesMaster :
                    kbn_title = ",'ﾌﾟﾛﾊﾟｰ（ｾｰﾙﾏｽﾀ有）' title";
                    kbn_sql = " and sh.セール区分=0";
                    kbn_sql2 = " and sp.セール区分=0";
                    break;
                case OutputCategory.Sales:
                    kbn_title = ",'ｾｰﾙ' title";
                    kbn_sql = " and sh.セール区分=1";
                    kbn_sql2 = " and sp.セール区分=1";
                    break;
                case OutputCategory.All:
                default:
                    break;
            }

            string sql_str = "";

            if (AppData.ClassCvnet.config.SellMFlg == 1)
            {

                sql_str = "select '" + wrk_para[0] + "' 店舗CD,'" + SelectedShop.Name + "' 店舗名,'" + wrk_para[1] + "' 開始日,'" + wrk_para[2] + "' 終了日,'" + wrk_para[3] + "' 開始品番,'" + wrk_para[4] + "' 終了品番";
                sql_str += ",s.ブランドCD,NVL((select 名称 from HC$master_meisho where 名称区分='BRD' and 名称CD=s.ブランドCD),'') ブランド";
                sql_str += ",s.名称CD01 大分類,NVL((select 名称 from HC$master_meisho where 名称区分='B01' and 名称CD=s.名称CD01),'') 大分類名";
                sql_str += ",sp.商品CD,decode(s.略称,'.',s.商品名,s.略称) 商品名,sp.サイズCD,sp.サイズCD||' '||decode(sp.サイズCD,'.','',get_sizename(sp.商品CD,sp.サイズCD)) サイズ";
                sql_str += ",sp.色CD,sp.色CD||' '||decode(sp.色CD,'.','',NVL((select 名称 from HC$master_meisho where 名称区分='COL' and 名称CD=sp.色CD),'')) 色,s.JANコード1,s.シーズンCD,s.上代";
                sql_str += ",decode(st.店舗CD,'.','全店','店舗') 店くくり,to_date(st.期間S) 期間S,decode(sh.セール区分,0,'ﾌﾟﾛﾊﾟｰ',1,'ｾｰﾙ','') セール区分,sp.新掛率,sp.新上代,sh.入力社員CD";
                sql_str += ",NVL((select 名称 from HC$master_meisho where 名称区分='SZN' and 名称CD=s.シーズンCD),'') シーズン";

                sql_str += ",s.\"メーカーCD\",NVL((select 名称 from HC$master_meisho where 名称区分='MKR' and 名称CD=s.メーカーCD),'') 仕入先";
                sql_str += ",s.アイテムCD,NVL((select 名称 from HC$master_meisho where 名称区分='ITM' and 名称CD=s.アイテムCD),'') アイテム";
                sql_str += ",s.\"メーカー品番\",to_date(st.期間E) 期間E";
                sql_str += kbn_title;

                sql_str += " from HC$master_price1 sp,HC$master_price2 st,HC$master_price0 sh, HC$master_shohin s";
                sql_str += " where sp.POS区分=0 and st.POS区分=0 ";
                sql_str += " and sp.商品CD=s.商品CD and st.店舗CD in (:1,'.') and st.期間S between :2 and :3 and sp.商品CD between :4 and :5";
                sql_str += " and sh.名称CD01 between :6 and :7 and sp.ヘッダno=st.ヘッダno and sp.ヘッダno=sh.seq_no ";
                sql_str += " and exists (select 'X' from HC$master_shohin ss where ss.商品CD=sp.商品CD " + flex_sql + ")";
                sql_str += kbn_sql;

                if (AppData.ClassCvnet.config.MultiCoop > 0) sql_str += " and exists (select 'X' from hc$master_tokui t where t.得意先CD=st.店舗CD and (t.法人CD='" + AppData.ClassCvnet.config.MultiCoop.ToString() + "' or t.法人CD='.'))";

                if (SelectedOutput == OutputCategory.ProperWithSalesMaster || SelectedOutput == OutputCategory.Sales) sql_str += " order by :1,s.ブランドCD,s.名称CD01,sp.商品CD,sp.色CD,sp.サイズCD,st.期間S desc";

                if (SelectedOutput == OutputCategory.All || SelectedOutput == OutputCategory.ProperNoSalesMaster)
                {   /* 印刷区分「全て」or「プロパー」時 */

                    var wrk_para2 = new BizArray();
                    for (var i = 0; i < 7; i++)
                    {
                        wrk_para2[i] = wrk_para[i];
                    }

                    /* 印刷区分「プロパー」時はパラメータ初期化 */
                    if (SelectedOutput == OutputCategory.ProperNoSalesMaster) wrk_para = new BizArray();

                    /* パラメータ調整 */
                    var sql_str2 = " and st.店舗CD in (:" + (wrk_para.Count + 1).ToString() + ",'.') and st.期間S between :" + (wrk_para.Count + 2).ToString() + " and :" + (wrk_para.Count + 3).ToString() + " and sp.商品CD between :" + (wrk_para.Count + 4).ToString() + " and :" + (wrk_para.Count + 5).ToString() + " and sh.名称CD01 between :" + (wrk_para.Count + 6).ToString() + " and :" + (wrk_para.Count + 7).ToString() + " ";
                    wrk_para[wrk_para.Count] = wrk_para2[0];
                    wrk_para[wrk_para.Count] = wrk_para2[1];
                    wrk_para[wrk_para.Count] = wrk_para2[2];
                    wrk_para[wrk_para.Count] = wrk_para2[3];
                    wrk_para[wrk_para.Count] = wrk_para2[4];
                    wrk_para[wrk_para.Count] = wrk_para2[5];
                    wrk_para[wrk_para.Count] = wrk_para2[6];
                    var flex_sql2 = ListFlexData.GetQueryStr(wrk_para, wrk_para.Count + 1, 0, "s.");

                    var sql_str3 = " and st.店舗CD in (:" + (wrk_para.Count + 1).ToString() + ",'.') and st.期間S between :" + (wrk_para.Count + 2).ToString() + " and :" + (wrk_para.Count + 3).ToString() + " and sp.商品CD between :" + (wrk_para.Count + 4).ToString() + " and :" + (wrk_para.Count + 5).ToString() + " and sh.名称CD01 between :" + (wrk_para.Count + 6).ToString() + " and :" + (wrk_para.Count + 7).ToString() + " ";
                    wrk_para[wrk_para.Count] = wrk_para2[0];
                    wrk_para[wrk_para.Count] = wrk_para2[1];
                    wrk_para[wrk_para.Count] = wrk_para2[2];
                    wrk_para[wrk_para.Count] = wrk_para2[3];
                    wrk_para[wrk_para.Count] = wrk_para2[4];
                    wrk_para[wrk_para.Count] = wrk_para2[5];
                    wrk_para[wrk_para.Count] = wrk_para2[6];
                    var flex_sql3 = ListFlexData.GetQueryStr(wrk_para, wrk_para.Count + 1, 0, "s.");

                    var sql_str4 = " and st.店舗CD in (:" + (wrk_para.Count + 1).ToString() + ",'.') and st.期間S between :" + (wrk_para.Count + 2).ToString() + " and :" + (wrk_para.Count + 3).ToString() + " and sp.商品CD between :" + (wrk_para.Count + 4).ToString() + " and :" + (wrk_para.Count + 5).ToString() + " and sh.名称CD01 between :" + (wrk_para.Count + 6).ToString() + " and :" + (wrk_para.Count + 7).ToString() + " ";
                    wrk_para[wrk_para.Count] = wrk_para2[0];
                    wrk_para[wrk_para.Count] = wrk_para2[1];
                    wrk_para[wrk_para.Count] = wrk_para2[2];
                    wrk_para[wrk_para.Count] = wrk_para2[3];
                    wrk_para[wrk_para.Count] = wrk_para2[4];
                    wrk_para[wrk_para.Count] = wrk_para2[5];
                    wrk_para[wrk_para.Count] = wrk_para2[6];
                    var flex_sql4 = ListFlexData.GetQueryStr(wrk_para, wrk_para.Count + 1, 0, "s.");

                    var sql_str5 = " and st.店舗CD in (:" + (wrk_para.Count + 1).ToString() + ",'.') and st.期間S between :" + (wrk_para.Count + 2).ToString() + " and :" + (wrk_para.Count + 3).ToString() + " and sp.商品CD between :" + (wrk_para.Count + 4).ToString() + " and :" + (wrk_para.Count + 5).ToString() + " and sh.名称CD01 between :" + (wrk_para.Count + 6).ToString() + " and :" + (wrk_para.Count + 7).ToString() + " ";
                    wrk_para[wrk_para.Count] = wrk_para2[0];
                    wrk_para[wrk_para.Count] = wrk_para2[1];
                    wrk_para[wrk_para.Count] = wrk_para2[2];
                    wrk_para[wrk_para.Count] = wrk_para2[3];
                    wrk_para[wrk_para.Count] = wrk_para2[4];
                    wrk_para[wrk_para.Count] = wrk_para2[5];
                    wrk_para[wrk_para.Count] = wrk_para2[6];
                    var flex_sql5 = ListFlexData.GetQueryStr(wrk_para, wrk_para.Count + 1, 0, "s.");

                    var sql_str6 = " and s.商品CD between :" + (wrk_para.Count + 1).ToString() + " and :" + (wrk_para.Count + 2).ToString();
                    wrk_para[wrk_para.Count] = wrk_para2[3];
                    wrk_para[wrk_para.Count] = wrk_para2[4];

                    sql_str += " union all ";
                    sql_str += "select ";

                    /* 印刷区分「プロパー(ｾｰﾙﾏｽﾀ無)」時はSQL文も作り直し */
                    if (SelectedOutput == OutputCategory.ProperNoSalesMaster) sql_str = "select ";

                    sql_str += "'" + wrk_para[0] + "' 店舗CD,'" + SelectedShop.Name + "' 店舗名,'" + wrk_para[1] + "' 開始日,'" + wrk_para[2] + "' 終了日,'" + wrk_para[3] + "' 開始品番,'" + wrk_para[4] + "' 終了品番";
                    sql_str += ",s.ブランドCD,NVL((select 名称 from HC$master_meisho where 名称区分='BRD' and 名称CD=s.ブランドCD),'') ブランド";
                    sql_str += ",s.名称CD01 大分類,NVL((select 名称 from HC$master_meisho where 名称区分='B01' and 名称CD=s.名称CD01),'') 大分類名";
                    sql_str += ",z.商品CD,decode(s.略称,'.',s.商品名,s.略称) 商品名,z.サイズCD,z.サイズCD||' '||decode(z.サイズCD,'.','',get_sizename(z.商品CD,z.サイズCD)) サイズ";
                    sql_str += ",z.色CD,z.色CD||' '||decode(z.色CD,'.','',NVL((select 名称 from HC$master_meisho where 名称区分='COL' and 名称CD=z.色CD),'')) 色,s.JANコード1,s.シーズンCD,s.上代";
                    sql_str += ",'' 店くくり,to_date('') 期間S,'ﾌﾟﾛﾊﾟｰ' セール区分,0 新掛率,0 新上代,'' 入力社員CD";
                    sql_str += ",NVL((select 名称 from HC$master_meisho where 名称区分='SZN' and 名称CD=s.シーズンCD),'') シーズン";
                    sql_str += ",s.\"メーカーCD\",NVL((select 名称 from HC$master_meisho where 名称区分='MKR' and 名称CD=s.メーカーCD),'') 仕入先";
                    sql_str += ",s.アイテムCD,NVL((select 名称 from HC$master_meisho where 名称区分='ITM' and 名称CD=s.アイテムCD),'') アイテム";
                    sql_str += ",s.\"メーカー品番\",to_date('') 期間E";
                    sql_str += kbn_title;
                    sql_str += " FROM ";
                    sql_str += "(SELECT ";
                    sql_str += "z.商品CD,z.色CD,z.サイズCD ,sum(z." + zaiko_s + ")";
                    sql_str += ",nvl((SELECT count(*) from (SELECT sp.商品CD,sp.色CD,sp.サイズCD,st.店舗CD,st.期間S,st.期間E FROM HC$master_price1 sp,HC$master_price2 st,HC$master_price0 sh,HC$master_shohin s WHERE sp.POS区分=0 AND st.POS区分=0 AND sp.商品CD=s.商品CD " + sql_str2 + " AND sp.ヘッダno=st.ヘッダno AND sp.ヘッダno=sh.seq_no AND exists (select 'X' from HC$master_shohin ss where ss.商品CD=sp.商品CD " + flex_sql2 + ")" + kbn_sql + " GROUP BY sp.商品CD,sp.色CD,sp.サイズCD,st.店舗CD,st.期間S,st.期間E) a WHERE a.商品CD=z.商品CD AND a.色CD=z.色CD AND a.サイズCD=z.サイズCD AND a.店舗CD=z.倉庫CD),0) cnt1";
                    sql_str += ",nvl((SELECT count(*) from (SELECT sp.商品CD,sp.色CD,sp.サイズCD,st.店舗CD,st.期間S,st.期間E FROM HC$master_price1 sp,HC$master_price2 st,HC$master_price0 sh,HC$master_shohin s WHERE sp.POS区分=0 AND st.POS区分=0 AND sp.商品CD=s.商品CD " + sql_str3 + " AND sp.ヘッダno=st.ヘッダno AND sp.ヘッダno=sh.seq_no AND exists (select 'X' from HC$master_shohin ss where ss.商品CD=sp.商品CD " + flex_sql3 + ")" + kbn_sql + " GROUP BY sp.商品CD,sp.色CD,sp.サイズCD,st.店舗CD,st.期間S,st.期間E) a WHERE a.商品CD=z.商品CD AND a.色CD=z.色CD AND a.サイズCD=z.サイズCD AND a.店舗CD='.'),0) cnt2";
                    sql_str += ",nvl((SELECT count(*) from (SELECT sp.商品CD,sp.色CD,sp.サイズCD,st.店舗CD,st.期間S,st.期間E FROM HC$master_price1 sp,HC$master_price2 st,HC$master_price0 sh,HC$master_shohin s WHERE sp.POS区分=0 AND st.POS区分=0 AND sp.商品CD=s.商品CD " + sql_str4 + " AND sp.ヘッダno=st.ヘッダno AND sp.ヘッダno=sh.seq_no AND exists (select 'X' from HC$master_shohin ss where ss.商品CD=sp.商品CD " + flex_sql4 + ")" + kbn_sql + " GROUP BY sp.商品CD,sp.色CD,sp.サイズCD,st.店舗CD,st.期間S,st.期間E) a WHERE a.商品CD=z.商品CD AND a.色CD='.' AND a.サイズCD='.' AND a.店舗CD=z.倉庫CD),0) cnt3";
                    sql_str += ",nvl((SELECT count(*) from (SELECT sp.商品CD,sp.色CD,sp.サイズCD,st.店舗CD,st.期間S,st.期間E FROM HC$master_price1 sp,HC$master_price2 st,HC$master_price0 sh,HC$master_shohin s WHERE sp.POS区分=0 AND st.POS区分=0 AND sp.商品CD=s.商品CD " + sql_str5 + " AND sp.ヘッダno=st.ヘッダno AND sp.ヘッダno=sh.seq_no AND exists (select 'X' from HC$master_shohin ss where ss.商品CD=sp.商品CD " + flex_sql5 + ")" + kbn_sql + " GROUP BY sp.商品CD,sp.色CD,sp.サイズCD,st.店舗CD,st.期間S,st.期間E) a WHERE a.商品CD=z.商品CD AND a.色CD='.' AND a.サイズCD='.' AND a.店舗CD='.'),0) cnt4";
                    sql_str += " FROM " + zaiko_tbl + " z,HC$master_shohin s";
                    sql_str += "	WHERE z.商品CD=s.商品CD and z.倉庫CD='" + wrk_para[0] + "'" + sql_str6;

                    if (AppData.ClassCvnet.config.MultiCoop > 0) sql_str += " and exists (select 'X' from hc$master_tokui t where t.得意先CD=z.倉庫CD and (t.法人CD='" + AppData.ClassCvnet.config.MultiCoop.ToString() + "' or t.法人CD='.'))";
                    sql_str += "	GROUP BY z.倉庫CD,z.商品CD,z.色CD,z.サイズCD";
                    sql_str += "	) z,HC$master_shohin s";
                    sql_str += "	WHERE z.商品CD=s.商品CD AND	z.cnt1=0 AND z.cnt2=0 AND z.cnt3=0 AND z.cnt4=0";

                    sql_str = "select * from (" + sql_str + ") order by '" + wrk_para[0] + "',ブランドCD,大分類,商品CD,色CD,サイズCD,期間S desc";
                }

            }
            else
            {
                sql_str = "select '" + wrk_para[0] + "' 店舗CD,'" + SelectedShop.Name + "' 店舗名,'" + wrk_para[1] + "' 開始日,'" + wrk_para[2] + "' 終了日,'" + wrk_para[3] + "' 開始品番,'" + wrk_para[4] + "' 終了品番";
                sql_str += ",s.ブランドCD,NVL((select 名称 from HC$master_meisho where 名称区分='BRD' and 名称CD=s.ブランドCD),'') ブランド";
                sql_str += ",s.名称CD01 大分類,NVL((select 名称 from HC$master_meisho where 名称区分='B01' and 名称CD=s.名称CD01),'') 大分類名";
                sql_str += ",sp.商品CD,decode(s.略称,'.',s.商品名,s.略称) 商品名,sp.サイズCD,sp.サイズCD||' '||decode(sp.サイズCD,'.','',get_sizename(sp.商品CD,sp.サイズCD)) サイズ";
                sql_str += ",sp.色CD,sp.色CD||' '||decode(sp.色CD,'.','',NVL((select 名称 from HC$master_meisho where 名称区分='COL' and 名称CD=sp.色CD),'')) 色,s.JANコード1,s.シーズンCD,s.上代";
                sql_str += ",decode(sp.店舗CD,'.','全店','店舗') 店くくり,to_date(sp.期間S) 期間S,decode(sp.セール区分,0,'ﾌﾟﾛﾊﾟｰ',1,'ｾｰﾙ','') セール区分,sp.新掛率,sp.新上代,sp.入力社員CD";
                sql_str += ",NVL((select 名称 from HC$master_meisho where 名称区分='SZN' and 名称CD=s.シーズンCD),'') シーズン";

                sql_str += ",s.\"メーカーCD\",NVL((select 名称 from HC$master_meisho where 名称区分='MKR' and 名称CD=s.メーカーCD),'') 仕入先";
                sql_str += ",s.アイテムCD,NVL((select 名称 from HC$master_meisho where 名称区分='ITM' and 名称CD=s.アイテムCD),'') アイテム";
                sql_str += ",s.\"メーカー品番\"";
                sql_str += ",'' dammy" + kbn_title;

                sql_str += " from HC$master_sho_price sp, HC$master_shohin s";
                sql_str += " where sp.POS区分=0";
                sql_str += " and sp.商品CD=s.商品CD and sp.店舗CD in (:1,'.') and sp.期間S between :2 and :3 and sp.商品CD between :4 and :5";
                sql_str += " and exists (select 'X' from HC$master_shohin ss where ss.商品CD=sp.商品CD " + flex_sql + ")";
                sql_str += kbn_sql2;

                if (AppData.ClassCvnet.config.MultiCoop > 0) sql_str += " and exists (select 'X' from hc$master_tokui t where t.得意先CD=sp.店舗CD and (t.法人CD='" + AppData.ClassCvnet.config.MultiCoop.ToString() + "' or t.法人CD='.'))";

                if (SelectedOutput == OutputCategory.ProperWithSalesMaster || SelectedOutput == OutputCategory.Sales) sql_str += " order by '" + wrk_para[0] + "',s.ブランドCD,s.名称CD01,sp.商品CD,sp.色CD,sp.サイズCD,sp.期間S desc";

                if (SelectedOutput == OutputCategory.All || SelectedOutput == OutputCategory.ProperNoSalesMaster)
                {   /* 印刷区分「全て」or「プロパー」時 */

                    var wrk_para2 = new BizArray();
                    for (var i = 0; i < 5; i++)
                    {
                        wrk_para2[i] = wrk_para[i];
                    }

                    /* 印刷区分「プロパー」時はパラメータ初期化 */
                    if (SelectedOutput == OutputCategory.ProperNoSalesMaster) wrk_para = new BizArray();

                    /* パラメータ調整 */
                    var sql_str2 = " and sp.店舗CD in (:" + (wrk_para.Count + 1).ToString() + ",'.') and sp.期間S between :" + (wrk_para.Count + 2).ToString() + " and :" + (wrk_para.Count + 3).ToString() + " and sp.商品CD between :" + (wrk_para.Count + 4).ToString() + " and :" + (wrk_para.Count + 5).ToString();
                    wrk_para[wrk_para.Count] = wrk_para2[0];
                    wrk_para[wrk_para.Count] = wrk_para2[1];
                    wrk_para[wrk_para.Count] = wrk_para2[2];
                    wrk_para[wrk_para.Count] = wrk_para2[3];
                    wrk_para[wrk_para.Count] = wrk_para2[4];
                    var flex_sql2 = ListFlexData.GetQueryStr(wrk_para, wrk_para.Count + 1, 0, "s.");

                    var sql_str3 = " and sp.店舗CD in (:" + (wrk_para.Count + 1).ToString() + ",'.') and sp.期間S between :" + (wrk_para.Count + 2).ToString() + " and :" + (wrk_para.Count + 3).ToString() + " and sp.商品CD between :" + (wrk_para.Count + 4).ToString() + " and :" + (wrk_para.Count + 5).ToString();
                    wrk_para[wrk_para.Count] = wrk_para2[0];
                    wrk_para[wrk_para.Count] = wrk_para2[1];
                    wrk_para[wrk_para.Count] = wrk_para2[2];
                    wrk_para[wrk_para.Count] = wrk_para2[3];
                    wrk_para[wrk_para.Count] = wrk_para2[4];
                    var flex_sql3 = ListFlexData.GetQueryStr(wrk_para, wrk_para.Count + 1, 0, "s.");

                    var sql_str4 = " and sp.店舗CD in (:" + (wrk_para.Count + 1).ToString() + ",'.') and sp.期間S between :" + (wrk_para.Count + 2).ToString() + " and :" + (wrk_para.Count + 3).ToString() + " and sp.商品CD between :" + (wrk_para.Count + 4).ToString() + " and :" + (wrk_para.Count + 5).ToString();
                    wrk_para[wrk_para.Count] = wrk_para2[0];
                    wrk_para[wrk_para.Count] = wrk_para2[1];
                    wrk_para[wrk_para.Count] = wrk_para2[2];
                    wrk_para[wrk_para.Count] = wrk_para2[3];
                    wrk_para[wrk_para.Count] = wrk_para2[4];
                    var flex_sql4 = ListFlexData.GetQueryStr(wrk_para, wrk_para.Count + 1, 0, "s.");

                    var sql_str5 = " and sp.店舗CD in (:" + (wrk_para.Count + 1).ToString() + ",'.') and sp.期間S between :" + (wrk_para.Count + 2).ToString() + " and :" + (wrk_para.Count + 3).ToString() + " and sp.商品CD between :" + (wrk_para.Count + 4).ToString() + " and :" + (wrk_para.Count + 5).ToString();
                    wrk_para[wrk_para.Count] = wrk_para2[0];
                    wrk_para[wrk_para.Count] = wrk_para2[1];
                    wrk_para[wrk_para.Count] = wrk_para2[2];
                    wrk_para[wrk_para.Count] = wrk_para2[3];
                    wrk_para[wrk_para.Count] = wrk_para2[4];
                    var flex_sql5 = ListFlexData.GetQueryStr(wrk_para, wrk_para.Count + 1, 0, "s.");

                    var sql_str6 = " and s.商品CD between :" + (wrk_para.Count + 1).ToString() + " and :" + (wrk_para.Count + 2).ToString();
                    wrk_para[wrk_para.Count] = wrk_para2[3];
                    wrk_para[wrk_para.Count] = wrk_para2[4];

                    sql_str += " union all ";
                    sql_str += "select ";

                    /* 印刷区分「プロパー(ｾｰﾙﾏｽﾀ無)」時はSQL文も作り直し */
                    if (SelectedOutput == OutputCategory.ProperNoSalesMaster) sql_str = "select ";

                    sql_str += "'" + wrk_para[0] + "' 店舗CD,'" + SelectedShop.Name + "' 店舗名,'" + wrk_para[1] + "' 開始日,'" + wrk_para[2] + "' 終了日,'" + wrk_para[3] + "' 開始品番,'" + wrk_para[4] + "' 終了品番";
                    sql_str += ",s.ブランドCD,NVL((select 名称 from HC$master_meisho where 名称区分='BRD' and 名称CD=s.ブランドCD),'') ブランド";
                    sql_str += ",s.名称CD01 大分類,NVL((select 名称 from HC$master_meisho where 名称区分='B01' and 名称CD=s.名称CD01),'') 大分類名";
                    sql_str += ",z.商品CD,decode(s.略称,'.',s.商品名,s.略称) 商品名,z.サイズCD,z.サイズCD||' '||decode(z.サイズCD,'.','',get_sizename(z.商品CD,z.サイズCD)) サイズ";
                    sql_str += ",z.色CD,z.色CD||' '||decode(z.色CD,'.','',NVL((select 名称 from HC$master_meisho where 名称区分='COL' and 名称CD=z.色CD),'')) 色,s.JANコード1,s.シーズンCD,s.上代";
                    sql_str += ",'' 店くくり,to_date('') 期間S,'ﾌﾟﾛﾊﾟｰ' セール区分,0 新掛率,0 新上代,'' 入力社員CD";
                    sql_str += ",NVL((select 名称 from HC$master_meisho where 名称区分='SZN' and 名称CD=s.シーズンCD),'') シーズン";
                    sql_str += ",s.\"メーカーCD\",NVL((select 名称 from HC$master_meisho where 名称区分='MKR' and 名称CD=s.メーカーCD),'') 仕入先";
                    sql_str += ",s.アイテムCD,NVL((select 名称 from HC$master_meisho where 名称区分='ITM' and 名称CD=s.アイテムCD),'') アイテム";
                    sql_str += ",s.\"メーカー品番\"";
                    sql_str += ",'' dammy" + kbn_title;
                    sql_str += " FROM ";
                    sql_str += "(SELECT ";
                    sql_str += "z.商品CD,z.色CD,z.サイズCD ,sum(z." + zaiko_s + ")";
                    sql_str += ",nvl((SELECT count(*) from (SELECT sp.商品CD,sp.色CD,sp.サイズCD,sp.店舗CD,sp.期間S FROM HC$master_sho_price sp,HC$master_shohin s WHERE sp.POS区分=0 AND sp.商品CD=s.商品CD " + sql_str2 + " AND exists (select 'X' from HC$master_shohin ss where ss.商品CD=sp.商品CD " + flex_sql2 + ")" + kbn_sql2 + " GROUP BY sp.商品CD,sp.色CD,sp.サイズCD,sp.店舗CD,sp.期間S) a WHERE a.商品CD=z.商品CD AND a.色CD=z.色CD AND a.サイズCD=z.サイズCD AND a.店舗CD=z.倉庫CD),0) cnt1";
                    sql_str += ",nvl((SELECT count(*) from (SELECT sp.商品CD,sp.色CD,sp.サイズCD,sp.店舗CD,sp.期間S FROM HC$master_sho_price sp,HC$master_shohin s WHERE sp.POS区分=0 AND sp.商品CD=s.商品CD " + sql_str3 + " AND exists (select 'X' from HC$master_shohin ss where ss.商品CD=sp.商品CD " + flex_sql3 + ")" + kbn_sql2 + " GROUP BY sp.商品CD,sp.色CD,sp.サイズCD,sp.店舗CD,sp.期間S) a WHERE a.商品CD=z.商品CD AND a.色CD=z.色CD AND a.サイズCD=z.サイズCD AND a.店舗CD='.'),0) cnt2";
                    sql_str += ",nvl((SELECT count(*) from (SELECT sp.商品CD,sp.色CD,sp.サイズCD,sp.店舗CD,sp.期間S FROM HC$master_sho_price sp,HC$master_shohin s WHERE sp.POS区分=0 AND sp.商品CD=s.商品CD " + sql_str4 + " AND exists (select 'X' from HC$master_shohin ss where ss.商品CD=sp.商品CD " + flex_sql4 + ")" + kbn_sql2 + " GROUP BY sp.商品CD,sp.色CD,sp.サイズCD,sp.店舗CD,sp.期間S) a WHERE a.商品CD=z.商品CD AND a.色CD='.' AND a.サイズCD='.' AND a.店舗CD=z.倉庫CD),0) cnt3";
                    sql_str += ",nvl((SELECT count(*) from (SELECT sp.商品CD,sp.色CD,sp.サイズCD,sp.店舗CD,sp.期間S FROM HC$master_sho_price sp,HC$master_shohin s WHERE sp.POS区分=0 AND sp.商品CD=s.商品CD " + sql_str5 + " AND exists (select 'X' from HC$master_shohin ss where ss.商品CD=sp.商品CD " + flex_sql5 + ")" + kbn_sql2 + " GROUP BY sp.商品CD,sp.色CD,sp.サイズCD,sp.店舗CD,sp.期間S) a WHERE a.商品CD=z.商品CD AND a.色CD='.' AND a.サイズCD='.' AND a.店舗CD='.'),0) cnt4";
                    sql_str += " FROM " + zaiko_tbl + " z,HC$master_shohin s";
                    sql_str += "	WHERE z.商品CD=s.商品CD and z.倉庫CD='" + wrk_para[0] + "'" + sql_str6;

                    if (AppData.ClassCvnet.config.MultiCoop > 0) sql_str += " and exists (select 'X' from hc$master_tokui t where t.得意先CD=z.倉庫CD and (t.法人CD='" + AppData.ClassCvnet.config.MultiCoop.ToString() + "' or t.法人CD='.'))";

                    sql_str += "	GROUP BY z.倉庫CD,z.商品CD,z.色CD,z.サイズCD";
                    sql_str += "	) z,HC$master_shohin s";
                    sql_str += "	WHERE z.商品CD=s.商品CD AND	z.cnt1=0 AND z.cnt2=0 AND z.cnt3=0 AND z.cnt4=0";

                    sql_str = "select * from (" + sql_str + ") order by '" + wrk_para[0] + "',ブランドCD,大分類,商品CD,色CD,サイズCD,期間S desc";
                }
            }

            if(SelectedOutput == OutputCategory.ProperNoSalesMaster){
                sql_str = ""
                + "select "
                + " a.ブランドCD, "
                + " a.ブランド, "
                + " a.大分類, "
                + " a.大分類名, "
                + " a.商品CD, "
                + " a.商品名, "
                + " a.JANコード1 年度 , "
                + " a.シーズンCD, "
                + " a.シーズン, "
                + " substrb(a.色, 1, instr(a.色, ' ')-1) 色CD, "
                + " substrb(a.色, instr(a.色, ' ') + 1) 色名, "
                + " substrb(a.サイズ, 1, instr(a.サイズ, ' ')-1) サイズCD, "
                + " substrb(a.サイズ, instr(a.サイズ, ' ') + 1) サイズ, "
                + "  a.上代, "
                + " a.店くくり 店舗 , "
                + " a.期間S 変更日時開始 , "
                + " a.期間E 変更日時終了 , "
                + " a.セール区分 セール , "
                + " a.新掛率 新割引率 , "
                + " a.新上代 新販売価格 , "
                + " a.入力社員CD 入力担当者  "
                + "  from "
                + "(" + sql_str + ") a";
            }

            string qfm_file = "cvnet01prn03sell.qfm";

            if (AppData.ClassCvnet.config.smtflg == 1)
            {
                qfm_file = "cvnet01prn03sellr.qfm";
            }

            return AppData.Http!.AspxSqlQueryCsv(sql_str, wrk_para.ToArray(), qfm_file);


        }

        #region ComboButtonCommand

        [RelayCommand]
        public void SelShop(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null)
            {
                SelectedShop = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }

        [RelayCommand]
        public void SelProduct1(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null)
            {
                SelectedProduct1 = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }

        [RelayCommand]
        public void SelProduct2(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null)
            {
                SelectedProduct2 = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }

        [RelayCommand]
        public void SelSales1(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null)
            {
                SelectedSales1 = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }

        [RelayCommand]
        public void SelSales2(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null)
            {
                SelectedSales2 = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }

        #endregion
    }
}
