using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Models;
using CvnetClient.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CvnetClient.ViewModels
{
    public partial class SubDlgInp00ViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string menuFlg;
        [ObservableProperty]
        SearchModel? editSearch;
        [ObservableProperty]
        private int selectedTabIndex;
        [ObservableProperty]
        OrderList? selectedOrder;
        [ObservableProperty]
        OrderHeader? selectedOrderHeader;
        [ObservableProperty]
        OrderHeader? selectedOrderHeaderRef;
        [ObservableProperty]
        ObservableCollection<OrderDetail>? selectedOrderDetail;
        [ObservableProperty]
        ObservableCollection<OrderList>? listOrder;
        [ObservableProperty]
        public int? flgSho;
        [ObservableProperty]
        public string? startCode;
        [ObservableProperty]
        public int sum;
        [ObservableProperty]
        public int totalJodai;
        [ObservableProperty]
        public int totalGedai;
        private string shoriKaishibi;
        private string tori_kbn = "商品発注区分";
        private int v_denkbn = 13;
        [ObservableProperty]
        public string? dateName;
        [ObservableProperty]
        public Visibility renkeishow;
        [ObservableProperty]
        public Visibility sokoshow;
        [ObservableProperty]
        private BtListHelper? product;
        [ObservableProperty]
        ObservableCollection<OrderDetail>? selectedOrderDetailRef;
        private string[] param1 = new string[18];
        private string[] param2 = new string[2];
        [ObservableProperty]
        public string csvname;
        string sql_collist1 = "手入力伝票NO,在庫計上日,掛計上日,取引区分,入力社員CD,倉庫CD," +
                            "取引先CD1,掛率1,外税対象金額,数量合計,明細金額合計," +
                            "内税消費税,外税消費税,上代合計,下代合計,メモ,掛率2,伝票処理区分,MOD_SEQ,関連伝票NO" +
                            ",関連伝票NO2,消費税率,納品先CD,担当者CD";
        string sql_collist2 = "明細取引区分,商品CD,色CD,サイズCD,明細名称,数量,単価,金額,内税消費税,外税消費税," +
                                "上代単価,上代金額,下代単価,下代金額,明細メモ,消費税計算方法" +
                                ",商品シリアル,関連伝票NO,JANCODE,原価FLG,関連商品CD,HHT_SEQ_NO";
        string[] col_list1;
        string[] col_list2;
        public void OnInit(object? init_para = null, string? init_flg = null)
        {
            OnInitBase(init_para, init_flg);
            col_list1 = sql_collist1.Split(',');
            col_list2 = sql_collist2.Split(',');
            EditSearch = new SearchModel();
            EditSearch.DenpyoNo1 = "0";
            EditSearch.DenpyoNo2 = "9999999999";
            EditSearch.SalesbiFrom = DateTime.Now;
            EditSearch.SalesbiTo = DateTime.MaxValue;
            EditSearch.Kanren1From = "0";
            EditSearch.Kanren1To = "9999999999999";
            EditSearch.Kanren2From = "0";
            EditSearch.Kanren2To = "9999999999999";
            EditSearch.SupplierTo = new BtListHelper("99999999", "");
            EditSearch.WareTo = new BtListHelper("99999999", "");
            EditSearch.ProductTo = new BtListHelper("zzzzzzzzzzzzzzzzzzzz", "");
            EditSearch.UserTo = new BtListHelper("99999999", "");
            DateName = "納品日";
            Csvname = "CSV出力";
            /* 初期化処理 */
            //Form1.TabFrame1.TabForm1.Imp2 = cvnet.ImpDateDiff.onGetDay();
            //Form1.TabFrame1.TabForm2.Text4 = sysdate();
            //Form1.TabFrame1.TabForm2.Text19 = 0; ;
            //Form1.TabFrame1.TabForm2.Text5 = sysdate();
            //Form1.TabFrame1.TabForm2.Text6.OnSet("10");
            //Form1.TabFrame1.TabForm2.Text7 = ClassSatoo.SHAIN_CD + " " + ClassSatoo.SHAIN_Name;
            /* 20041111 Text7Length over のエラーを修正 */
            var ret_csv = AppData.ClassCvnet.AspxSqlQueryImp();
            //Form1.TabFrame1.TabForm2.Text8 = ret_csv.Rows[0][1] + " " + ret_csv.Rows[0][2];
            //var wrk_str00 = Form1.TabFrame1.TabForm2.impText19;
            //var wrk_str = Form1.TabFrame1.TabForm2.impText19.ComboItem1;
            //wrk_str.clear();
            for (var i = 0; i < 4; i++)
            {
                //wrk_str.insert();
                //wrk_str[wrk_str.length - 1] = AppData.ClassCvnet.SysMst.GetSaleKake(i).ToString("099");
                //if (wrk_str00 == "")
                //{
                //    wrk_str00.value = wrk_str[wrk_str.length - 1];
                //}
            }
            //Form1.TabFrame1.TabForm2.impText19 = "100";

            /* 名称ラベル代入用 */
            //Form1.TabFrame1.TabForm1.Spread1.SetTitleNames(Form1.TabFrame1.TabForm1.Spread1);
            //Form1.TabFrame1.TabForm2.Spread1.SetTitleNames();

            /* flg対応 11.07.26 */
            if (init_flg != null) MenuFlg = init_flg;

            GetShoriKaishibi();
            /* 2014.05.30 #13270 受払表対応 */
            if (MenuFlg == "uke")
            {
                //DebugMessage("wrk_para=", str(wrk_para), "\n");
                //Form1.TabFrame1.TabForm1.Imp1.Value = wrk_para[0];
                //Form1.TabFrame1.TabForm1.Imp1_2.Value = wrk_para[0];
                //Form1.TabFrame1.TabForm1.Imp2.Value = wrk_para[1];
                //Form1.TabFrame1.TabForm1.Imp2_2.Value = wrk_para[1];
                //Form1.TabFrame1.TabForm1.CvnetButton1.OnTouch();
                //Form1.TabFrame1.TabForm1.Spread1.Value = 0;
                //Form1.TabFrame1.TabForm1.Spread1.OnZoom();
                //Form1.TabFrame1.Value = 1;
                //Form1.TabFrame1.TabForm2.CvnetButton2.OnTouch();
                //Form1.TabFrame1.TabForm2.CvnetButton5.Active = $FALSE;
                //Form1.TabFrame1.TabForm2.CvnetButton5.Visible = $FALSE;
                //Form1.TabFrame1.TabForm2.CvnetButton6.Active = $FALSE;
                //Form1.TabFrame1.TabForm2.CvnetButton6.Visible = $FALSE;
                //Form1.TabFrame1.TabForm2.Bt_Back.Active = $TRUE;
                //Form1.TabFrame1.TabForm2.Bt_Back.Visible = $TRUE;
                //Form1.TabFrame1.TabForm1.Active = $FALSE;
                //Form1.TabFrame1.TabForm1.Visible = $FALSE;

                /* 2020.02.14 #55106対応追加 */
                /* wrk_para[2]="2"は、遷移先各伝票画面の修正不可モード */
                //if (wrk_para[2] == "2")
                //{
                //    //Form1.TabFrame1.TabForm2.CvnetButton3.Active = $FALSE;
                //    //Form1.TabFrame1.TabForm2.CvnetButton3.Visible = $FALSE;
                //}

            }
            else
            {

                /* 指示画面/移動元初期設定 08.03.24 */
                //if (int.Parse(para[0]) == 1)
                //{
                //    //Form1.TabFrame1.TabForm1.Imp7 = cvnet.SysImp.getCell(0, 1) + " " + cvnet.SysImp.getCell(0, 2);
                //    //Form1.TabFrame1.TabForm1.Imp7_2 = cvnet.SysImp.getCell(0, 1) + " " + cvnet.SysImp.getCell(0, 2);
                //}

                ///* 指示画面/店舗初期設定 10.11.17 */
                ///* 社販・店舗メニュー対応版 */
                //if (int.Parse(para[0]) == 3)
                //{
                //    //Form1.TabFrame1.TabForm1.Imp7 = ret_csv.getcell(1, 1) + " " + ret_csv.getcell(1, 2);
                //    //Form1.TabFrame1.TabForm1.Imp7_2 = ret_csv.getcell(1, 1) + " " + ret_csv.getcell(1, 2);
                //    //Form1.TabFrame1.TabForm2.Text8 = ret_csv.getcell(1, 1) + " " + ret_csv.getcell(1, 2);
                //    /* (09.01.14)追加 店舗固定 */
                //    if (cvnet.Config.TenpoFix == 1)
                //    {
                //        //Form1.TabFrame1.TabForm1.Imp7.Active = $FALSE;
                //        //Form1.TabFrame1.TabForm1.Imp7_2.Active = $FALSE;
                //        //Form1.TabFrame1.TabForm1.CvnetButton8.Active = $FALSE;
                //        //Form1.TabFrame1.TabForm1.CvnetButton9.Active = $FALSE;
                //        //Form1.TabFrame1.TabForm2.text8.Active = $FALSE;
                //        //Form1.TabFrame1.TabForm2.CvnetButton8.Active = $FALSE;
                //        //Form1.TabFrame1.TabForm2.Text7.Active = $false;
                //        //Form1.TabFrame1.TabForm2.CvnetButton17.Active = $false;
                //    }
                //}

            }


            /* SKU管理しない場合 */
            if (AppData.ClassCvnet.config.SKUFlg == 1)
            {
                //Form1.TabFrame1.TabForm2.Label99.Visible =$FALSE;
                //Form1.TabFrame1.TabForm2.TextSub01.Visible =$FALSE;
                //Form1.TabFrame1.TabForm2.TextSub01.Active =$FALSE;
                //Form1.TabFrame1.TabForm2.CvnetButton100.Visible =$FALSE;
                //Form1.TabFrame1.TabForm2.CvnetButton100.Active =$FALSE;
                //Form1.TabFrame1.TabForm2.CvnetButton13.Visible =$FALSE;
                //Form1.TabFrame1.TabForm2.CvnetButton13.Active =$FALSE;
                //Form1.TabFrame1.TabForm2.CvnetButton12.Visible =$FALSE;
                //Form1.TabFrame1.TabForm2.CvnetButton12.Active =$FALSE;
                //Form1.TabFrame1.TabForm2.Spread1.FlexRecord1.Set01.Set02.Line4.Width = 300;
                //Form1.TabFrame1.TabForm2.Spread1.FlexRecord1.Set01.Set02.Line5.Width = 0;
                //Form1.TabFrame1.TabForm2.Spread1.FlexRecord1.Set01.Set02.Line5.Editable =$false;
                //Form1.TabFrame1.TabForm2.Spread1.FlexRecord1.Set01.Set02.btn03.Width = 0;
                //Form1.TabFrame1.TabForm2.Spread1.FlexRecord1.Set01.Set02.btn03.Active =$false;
                //Form1.TabFrame1.TabForm2.Spread1.FlexRecord1.Set01.Set02.Line6.Width = 0;
                //Form1.TabFrame1.TabForm2.Spread1.FlexRecord1.Set01.Set02.Line6.Editable =$false;
                //Form1.TabFrame1.TabForm2.Spread1.FlexRecord1.Set01.Set02.btn04.Width = 0;
                //Form1.TabFrame1.TabForm2.Spread1.FlexRecord1.Set01.Set02.btn04.Active =$false;
                //Form1.TabFrame1.TabForm2.Spread1.FlexRecord1.Set01.Set03.Line7.Width = 360;
                //Form1.TabFrame1.TabForm2.Spread1.FlexRecord1.Set01.Set03.Line25.Width = 0;
                //Form1.TabFrame1.TabForm2.Spread1.FlexRecord1.Set01.Set03.Line26.Width = 0;
            }

            /* その他出荷売上対応 09.12.10 */
            if (int.Parse(para[0]) == 2)
            {
                //SubDialog.Title = "その他出荷売上";
                //Form1.v_denkbn = 20;
            }

            /* 消費税率コンボボックス一覧初期セット 2013.11.19 */
            //SubDialog.OnTaxListSet(Form1.TabFrame1.TabForm2.Text5.OnGet());
            //Form1.v_last_date = Form1.TabFrame1.TabForm2.Text5.OnGet();

            /* 2021.02.18 #56430対応追加  */
            if (AppData.ClassCvnet.config.ExcelOutFlg == 1)
            {
                Csvname = "EXCEL出力";
            }

            /* 原価FLG非表示 21.10.14 */
            if (AppData.ClassCvnet.config.usegenka == 0)
            {
                //Form1.TabFrame1.TabForm2.Spread1.FlexRecord1.Set01.Set02.Line4.Width = 160;
                //Form1.TabFrame1.TabForm2.Spread1.FlexRecord1.Set01.Set02.Line22.Width = 0;
                //Form1.TabFrame1.TabForm2.Spread1.FlexRecord1.Set01.Set02.Line22.Editable =$false;
            }
        }

        [RelayCommand]
        public void ClickProd()
        {
            FlgSho = FlgSho == 0 ? 1 : 0;
        }
        [RelayCommand]
        public void ClickDateTitle(string value)
        {
            DateName = DateName == "納品日" ? "在庫計上日" : "納品日";
        }
        void GetShoriKaishibi()
        {
            var sql_query = "select 処理開始日 from HC$MASTER_SYSKANRI";
            var ret_data = AppData.Http!.AspxSqlQuery(sql_query, null);
            if (ret_data.Rows.Count > 0) shoriKaishibi = ret_data.Rows[0][0].ToString();
        }
        [RelayCommand]
        void DoList()
        {
            CreatePara();
            OnQuery(param1, "all", param2, null);
        }
        [RelayCommand]
        void BackList()
        {
            if (ListOrder != null && ListOrder.Count > 0)
            {
                StartCode = ListOrder.Max(c => c.DenpyoNo);
            }
            else
            {
                DoList();
            }
            CreatePara();
            OnQuery(param1, StartCode, param2, null);
            if (ListOrder == null || ListOrder.Count == 0)
                ClientLib.MessageBoxOk(this, "データがありません");
        }

        [RelayCommand]
        void NextList()
        {
            if (ListOrder != null && ListOrder.Count > 0)
            {
                StartCode = ListOrder.Min(c => c.DenpyoNo);
            }
            else
            {
                DoList();
                return;
            }
            CreatePara();
            OnQuery(param1, StartCode, param2, "<=");
            if (ListOrder == null || ListOrder.Count == 0)
                ClientLib.MessageBoxOk(this, "データがありません");
        }
        void OnQuery(string[] param, string flg, string[] param2, string p_sort = null)
        {
            //var v_para = new BizArray();
            //for (var i = 0; i < param1.Length; i++)
            //{
            //    v_para[i] = param1[i];
            //}

            var v_hugo = ">=";
            if (p_sort != null)
            {
                v_hugo = "<=";
            }
            var parameter = new string[18];
            if (FlgSho == 1)
            {
                parameter = new string[20];
            }
            for (var i = 0; i < param.Length; i++)
            {
                parameter[i] = param[i];
            }

            var sql_query = "select A.SEQ_NO,A.VDATE_CREATE,A.VDATE_UPDATE,";
            for (var i = 0; i < col_list1.Length; i++)
            {
                sql_query += "A." + col_list1[i] + ",";
            }
            sql_query += "B.名前 担当名,C.得意先名 得意先名, D.得意先名 倉庫名,C.掛率,C.セール掛率,C.消費税CD,C.消費税計算方法,C.消費税端数,C.下代桁切指定,C.下代端数区分,C.下代計算FLG"
                    + ",NVL((select max(K.請求日) 請求日 from HC$Manage_kakesky K where K.得意先CD=A.取引先CD1),'19010101') 最終締日";
            sql_query += " , A.請求書NO";

            sql_query += ",NVL((SELECT n.納品先名 FROM HC$MASTER_NOHIN n WHERE n.納品先CD = A.納品先CD),'') 納品先名";
            sql_query += ",NVL((SELECT n.名前 FROM HC$MASTER_SHAIN n WHERE n.社員CD = A.担当者CD),'') 担当者名";

            sql_query += " from HC$Tran_TORI0 A,HC$MASTER_SHAIN B,HC$Master_TOKUI C,HC$MASTER_TOKUI D ";
            sql_query += "where (A.入力社員CD=B.社員CD(+)) and (A.取引先CD1=C.得意先CD(+)) and (A.倉庫CD=D.得意先CD(+)) ";
            sql_query += "and A.SEQ_NO between :1 and :2 and A.在庫計上日 between :3 and :4 and A.取引区分 between :5 and :6 ";
            sql_query += "and A.関連伝票NO between :7 and :8 and A.関連伝票NO2 between :9 and :10 and A.取引先CD1 between :11 and :12 and A.倉庫CD between :13 and :14 ";
            sql_query += "and A.請求書NO between :15 and :16 ";
            sql_query += "and A.手入力伝票NO between :17 and :18 ";
            sql_query += "and A.入力社員CD between :19 and :20 ";
            sql_query += " and A.伝票処理区分=" + v_denkbn;

            
            if (flg != "all")
            {
                sql_query += " and A.SEQ_NO " + v_hugo + " '" + flg + "'";
            }

            if (FlgSho == 1)
            {
                parameter[18] = param2[0];
                parameter[19] = param2[1];
                sql_query = "SELECT A.* FROM (" + sql_query
                    + ") A WHERE EXISTS (SELECT /*+ INDEX(E 	HC$_NK_TORI23) */ 'X' FROM HC$TRAN_TORI1 E WHERE A.SEQ_NO=E.ヘッダNO AND E.商品CD BETWEEN :" + (parameter.ToArray().Length - 1).ToString() + " AND :" + (parameter.ToArray().Length).ToString() + ")";
            }

            sql_query += " ORDER BY A.SEQ_NO DESC";

            if (flg != "all")
            {
                sql_query = "select * from (" + sql_query + ") where rownum<=" + AppData.maxQueryCnt;
            }

            var ret_csv = AppData.Http!.AspxSqlQuery(sql_query, parameter);

            if (ret_csv == null || ret_csv.Rows.Count == 0)
            {
                ClientLib.MessageBox(this, "データがありません!");
                return;
            }
            var list = (from DataRow dr in ret_csv.Rows
                        select new OrderList
                        {
                            DenpyoNo = dr["SEQ_NO"].ToString() ?? string.Empty,
                            Nohinbi = (dr["納品日"].ToString().Substring(0, 4) + "/" + dr["納品日"].ToString().Substring(4, 2) + "/" + dr["納品日"].ToString().Substring(6, 2)).ToString() ?? string.Empty,
                            Supplier = dr["取引先CD1"].ToString() ?? string.Empty,
                            SupplierName = dr["仕入先名"].ToString() ?? string.Empty,
                            Ware = dr["取引先CD2"].ToString() ?? string.Empty,
                            WareName = dr["得意先名"].ToString() ?? string.Empty,
                            Torihiki = dr["取引区分"].ToString() ?? string.Empty,
                            WeightSum = dr["数量合計"].ToString() ?? string.Empty,
                            PriceSum = dr["明細金額合計"].ToString() ?? string.Empty,
                            Tanto = dr["入力社員CD"].ToString() ?? string.Empty,
                            TantoName = dr["担当名"].ToString() ?? string.Empty,
                            Kanren1 = dr["関連伝票NO"].ToString() ?? string.Empty,
                            Kanren2 = dr["関連伝票NO2"].ToString() ?? string.Empty,
                            Tenyuryoku = dr["手入力伝票NO"].ToString() ?? string.Empty,
                            Hachubi = dr["掛計上日"].ToString() ?? string.Empty,
                            VdateCreate = Convert.ToDecimal(dr["VDATE_CREATE"]),
                            VdateUpdate = Convert.ToDecimal(dr["VDATE_UPDATE"])

                        }).OrderByDescending(c => c.DenpyoNo).ToList();
            Common.ConvertDotStringDel(list);
            ListOrder = new ObservableCollection<OrderList>(list);
            if (ListOrder.Count > 0)
            {
                SelectedOrder = ListOrder[0];
            }
        }

        void OnTaxListSet() { 
        
        }

        private void RowDoubleClick(OrderList item)
        {
            if (item == null || item.GetType().Name == "DefaultBindableSampleDataObject") return;

            SelectedOrderHeader.Supplier = new BtListHelper(item.Supplier, item.SupplierName);
            SelectedOrderHeader.Nohinbi = DateTime.Parse(item.Nohinbi);
            SelectedOrderHeader.Ware = new BtListHelper(item.Ware, item.WareName);
            SelectedOrderHeader.ToriKubun = int.Parse(item.Torihiki);
            SelectedOrderHeader.DenpyoNo = item.DenpyoNo;
            selectedOrderHeader.Hachubi = DateTime.Parse(item.Hachubi.Substring(0, 4) + "/" + item.Hachubi.Substring(4, 2) + "/" + item.Hachubi.Substring(6, 2));
            SelectedOrderHeader.Tenyuryoku = item.Tenyuryoku;
            SelectedOrderHeader.Kanren1 = item.Kanren1;
            SelectedOrderHeader.Kanren2 = item.Kanren2;
            SelectedOrderHeader.User = new BtListHelper(item.Tanto, item.TantoName);

            SelectedOrderHeaderRef = new OrderHeader();
            SelectedOrderHeaderRef.Supplier = new BtListHelper(item.Supplier, item.SupplierName);
            SelectedOrderHeaderRef.Nohinbi = DateTime.Parse(item.Nohinbi);
            SelectedOrderHeaderRef.Ware = new BtListHelper(item.Ware, item.WareName);
            SelectedOrderHeaderRef.ToriKubun = int.Parse(item.Torihiki);
            SelectedOrderHeaderRef.DenpyoNo = item.DenpyoNo;
            SelectedOrderHeaderRef.Hachubi = DateTime.Parse(item.Hachubi.Substring(0, 4) + "/" + item.Hachubi.Substring(4, 2) + "/" + item.Hachubi.Substring(6, 2));
            SelectedOrderHeaderRef.Tenyuryoku = item.Tenyuryoku;
            SelectedOrderHeaderRef.Kanren1 = item.Kanren1;
            SelectedOrderHeaderRef.Kanren2 = item.Kanren2;
            SelectedOrderHeaderRef.User = new BtListHelper(item.Tanto, item.TantoName);
            var wrk_para = new string[2];
            wrk_para[0] = item.DenpyoNo;
            wrk_para[1] = item.Torihiki;
            OnQueryDetail(wrk_para);
            SelectedTabIndex = 1;

        }

        void OnQueryDetail(string[] param)
        {
            var sql_query = "select A.SEQ_NO,A.VDATE_CREATE,A.VDATE_UPDATE,";
            for (var i = 0; i < col_list2.Length; i++)
            {
                sql_query += "A." + col_list2[i] + ",";
            }
            sql_query += " NVL((select H.名称 from HC$master_meisho H where H.名称区分='COL' and H.名称CD=A.色CD),'.') COL名";
            sql_query += ",GET_SIZENAME(A.商品CD,A.サイズCD) サイズ名";
            sql_query += " , get_jodai(A.商品CD,A.色CD,A.サイズCD) 上代";
            sql_query += " ,get_kakeritu('" + /*Form1.TabFrame1.TabForm2.Text9.OnGet() + "'," + Form1.TabFrame1.TabForm2.Text6.OnGet()*/ ""+ ",A.商品CD) 下代掛率";
            sql_query += " from HC$tran_tori1 A";
            sql_query += " where A.ヘッダNO=:1 and A.明細取引区分=:2 order by A.ヘッダNO,A.行NO";
            var ret_data = AppData.Http!.AspxSqlQuery(sql_query, param);
            if (ret_data == null || ret_data.Rows.Count == 0)
            {
                ClientLib.MessageBox(this, "データがありません!");
                return;
            }
            var list = (from DataRow dr in ret_data.Rows
                        select new OrderDetail
                        {
                            ProductCD = dr["商品CD"].ToString() ?? string.Empty,
                            ProductName = dr["明細名称"].ToString() ?? string.Empty,
                            Maker = dr["MKR品番"].ToString() ?? string.Empty,
                            Size = dr["サイズCD"].ToString() ?? string.Empty,
                            SizeName = dr["サイズ名"].ToString() ?? string.Empty,
                            Color = dr["色CD"].ToString() ?? string.Empty,
                            ColorName = dr["COL名"].ToString() ?? string.Empty,
                            JodaiKingaku = Convert.ToInt32(dr["上代金額"]),
                            JodaiTanka = Convert.ToInt32(dr["上代単価"]),
                            GedaiKingaku = Convert.ToInt32(dr["下代金額"]),
                            GedaiTanka = Convert.ToInt32(dr["下代単価"]),
                            Weight = Convert.ToInt32(dr["数量"]),
                            Kubun = dr["仕入区分"].ToString() ?? string.Empty,
                            Kanryo = dr["完了FLG"].ToString() ?? string.Empty,
                            Abstracts = dr["明細メモ"].ToString() ?? string.Empty,
                            RowColor = "Transparent"

                        }).OrderBy(c => c.ProductCD).ToList();
            Common.ConvertDotStringDel(list);
            SelectedOrderDetail = new ObservableCollection<OrderDetail>(list);
            SelectedOrderDetailRef = new ObservableCollection<OrderDetail>(
                                                                            list.Select(x => new OrderDetail
                                                                            {
                                                                                ProductCD = x.ProductCD,
                                                                                ProductName = x.ProductName,
                                                                                Maker = x.Maker,
                                                                                Size = x.Size,
                                                                                SizeName = x.SizeName,
                                                                                Color = x.Color,
                                                                                ColorName = x.ColorName,
                                                                                JodaiKingaku = x.JodaiKingaku,
                                                                                JodaiTanka = x.JodaiTanka,
                                                                                GedaiKingaku = x.GedaiKingaku,
                                                                                GedaiTanka = x.GedaiTanka,
                                                                                Weight = x.Weight,
                                                                                Kubun = x.Kubun,
                                                                                Kanryo = x.Kanryo,
                                                                                Abstracts = x.Abstracts
                                                                            })
            );
            UpdateSum(SelectedOrderDetail);
        }

        void CreatePara()
        {
            param1[0] = EditSearch.DenpyoNo1;
            param1[1] = EditSearch.DenpyoNo2;

            var date_Str = EditSearch.SalesbiFrom?.ToString("yyyyMMdd");
            if (EditSearch.SalesbiFrom < DateTime.Parse(shoriKaishibi.Substring(0, 4) + "/" + shoriKaishibi.Substring(4, 2) + "/" + shoriKaishibi.Substring(6, 2))) date_Str = shoriKaishibi;
            param1[2] = date_Str;

            param1[3] = EditSearch.SalesbiTo?.ToString("yyyyMMdd");
            param1[4] = EditSearch.ToriKubunFrom?.ToString() ?? string.Empty;
            param1[5] = EditSearch.ToriKubunTo?.ToString() ?? string.Empty;
            param1[6] = EditSearch.Kanren1From?.ToString() ?? string.Empty;
            param1[7] = EditSearch.Kanren1To?.ToString() ?? string.Empty;
            param1[8] = EditSearch.Kanren2From?.ToString() ?? string.Empty;
            param1[9] = EditSearch.Kanren2To?.ToString() ?? string.Empty;
            param1[10] = EditSearch.Tenyuryoku1?.ToString() ?? string.Empty;
            param1[11] = EditSearch.Tenyuryoku2?.ToString() ?? string.Empty;
            param1[12] = EditSearch.SupplierFrom?.Code ?? string.Empty;
            param1[13] = EditSearch.SupplierTo?.Code ?? string.Empty;
            param1[14] = EditSearch.WareFrom?.Code ?? string.Empty;
            param1[15] = EditSearch.WareTo?.Code ?? string.Empty;
            param1[16] = EditSearch.UserFrom?.Code ?? string.Empty;
            param1[17] = EditSearch.UserTo?.Code ?? string.Empty;
            if (FlgSho == 1)
            {
                param2[0] = EditSearch.ProductFrom?.Code ?? string.Empty;
                param2[1] = EditSearch.ProductTo?.Code ?? string.Empty;
            }
        }
        void UpdateSum(ObservableCollection<OrderDetail> list)
        {
            int kei = 0;
            int koukei = 0;
            int koukei1 = 0;
            for (var i = 0; i < list.Count; i++)
            {
                kei += list[i].Weight;
                koukei += list[i].JodaiKingaku;
                koukei1 += list[i].GedaiKingaku;
            }
            Sum = kei;
            TotalJodai = koukei;
            TotalGedai = koukei1;
        }
        public partial class SearchModel : ObservableObject
        {
            [ObservableProperty]
            public string? denpyoNo1;
            [ObservableProperty]
            private string? denpyoNo2;
            [ObservableProperty]
            private DateTime? salesbiFrom;
            [ObservableProperty]
            private DateTime? salesbiTo;
            [ObservableProperty]
            private int? toriKubunFrom;
            [ObservableProperty]
            private int? toriKubunTo;
            [ObservableProperty]
            private string? kanren1From;
            [ObservableProperty]
            private string? kanren1To;
            [ObservableProperty]
            private string? kanren2From;
            [ObservableProperty]
            private string? kanren2To;
            [ObservableProperty]
            private string? tenyuryoku1;
            [ObservableProperty]
            private string? tenyuryoku2;
            [ObservableProperty]
            private BtListHelper? supplierFrom;
            [ObservableProperty]
            private BtListHelper? supplierTo;
            [ObservableProperty]
            private BtListHelper? wareFrom;
            [ObservableProperty]
            private BtListHelper? wareTo;
            [ObservableProperty]
            private BtListHelper? productFrom;
            [ObservableProperty]
            private BtListHelper? productTo;
            [ObservableProperty]
            private BtListHelper? userFrom;
            [ObservableProperty]
            private BtListHelper? userTo;
            [ObservableProperty]
            private int? renkei;
        }
        public partial class OrderList : ObservableObject
        {
            [ObservableProperty]
            private string? denpyoNo;
            [ObservableProperty]
            private string? nohinbi;
            [ObservableProperty]
            private string? supplier;
            [ObservableProperty]
            private string? supplierName;
            [ObservableProperty]
            private string? ware;
            [ObservableProperty]
            private string? wareName;
            [ObservableProperty]
            private string? torihiki;
            [ObservableProperty]
            private string? weightSum;
            [ObservableProperty]
            private string? priceSum;
            [ObservableProperty]
            private string? tanto;
            [ObservableProperty]
            private string? tantoName;
            [ObservableProperty]
            private string? kanren1;
            [ObservableProperty]
            private string? kanren2;
            [ObservableProperty]
            private string? tenyuryoku;
            [ObservableProperty]
            private decimal vdateCreate;
            [ObservableProperty]
            private decimal vdateUpdate;
            [ObservableProperty]
            private string? hachubi;
        }
        public partial class OrderHeader : ObservableObject
        {
            [ObservableProperty]
            private string? denpyoNo;
            [ObservableProperty]
            private DateTime? hachubi;
            [ObservableProperty]
            private DateTime? nohinbi;
            [ObservableProperty]
            private int? toriKubun;
            [ObservableProperty]
            private string? kanren1;
            [ObservableProperty]
            private string? kanren2;
            [ObservableProperty]
            private string? tenyuryoku;
            [ObservableProperty]
            private BtListHelper? supplier;
            [ObservableProperty]
            private BtListHelper? ware;
            [ObservableProperty]
            private BtListHelper? user;
            [ObservableProperty]
            private string? biko;
            [ObservableProperty]
            private decimal? createDate;
            [ObservableProperty]
            private decimal? updateDate;
            [ObservableProperty]
            private decimal? generalTax;
        }
        public partial class OrderDetail : ObservableObject
        {
            [ObservableProperty]
            private string? productCD;
            [ObservableProperty]
            private string? productName;
            [ObservableProperty]
            private string? color;
            [ObservableProperty]
            private string? colorName;
            [ObservableProperty]
            private string? size;
            [ObservableProperty]
            private string? sizeName;
            [ObservableProperty]
            private int weight;
            [ObservableProperty]
            private int jodaiTanka;
            [ObservableProperty]
            private int jodaiKingaku;
            [ObservableProperty]
            private int gedaiTanka;
            [ObservableProperty]
            private int gedaiKingaku;
            [ObservableProperty]
            private string? abstracts;
            [ObservableProperty]
            private string? kanryo;
            [ObservableProperty]
            private string? maker;
            [ObservableProperty]
            private string? kubun;
            [ObservableProperty]
            private string? genkaFlg;
            [ObservableProperty]
            private string? rowColor;
        }
    }
}
