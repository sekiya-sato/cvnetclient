using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Models;
using CvnetClient.Utils;
using CvnetClient.Views;
using System;
using System.Collections.ObjectModel;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CvnetClient.ViewModels
{
    public partial class SubDlgInp13ViewModel : BaseViewModel
    {
        #region Declare
        [ObservableProperty]
        SearchModel? editSearch;
        [ObservableProperty]
        public Dictionary<int, string>? comboListToriHikiFrom;
        [ObservableProperty]
        public Dictionary<int, string>? comboListToriHikiTo;
        [ObservableProperty]
        public Dictionary<int, string>? comboListToriHiki;
        [ObservableProperty]
        public Dictionary<int, string>? comboListRenkei;
        [ObservableProperty]
        public Dictionary<string, string>? comboListKanryo;
        [ObservableProperty]
        ObservableCollection<OrderList>? listOrder;
        [ObservableProperty]
        private int selectedTabIndex;
        [ObservableProperty]
        OrderList? selectedOrder;
        [ObservableProperty]
        OrderHeader? selectedOrderHeader;
        [ObservableProperty]
        ObservableCollection<OrderDetail>? selectedOrderDetail;
        [ObservableProperty]
        public int? flgSho;
        [ObservableProperty]
        public string? startCode;
        [ObservableProperty]
        public string? menuFlg;
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
        public bool renkeishow = false;
        public bool sokoshow;
        [ObservableProperty]
        private BtListHelper? product;
        [ObservableProperty]
        public string csvname;
        private string[] param1 = new string[18];
        private string[] param2 = new string[2];
        string sql_collist = "A.手入力伝票NO,A.在庫計上日,A.納品日,A.取引区分,A.入力社員CD,A.取引先CD2," +
                            "A.取引先CD1,A.掛率1,A.外税対象金額,A.数量合計,A.明細金額合計," +
                            "A.内税消費税,A.外税消費税,A.上代合計,A.下代合計,A.メモ,A.掛計上FLG,A.伝票処理区分,A.MOD_SEQ,A.倉庫CD,A.関連伝票NO" +
                            ",A.掛計上日" +
                            ",A.SYSFLG2" +
                            ",A.関連伝票NO2";


        string sql_collist1 = "A.明細取引区分,A.商品CD,A.色CD,A.サイズCD,A.明細名称,A.数量,A.単価,A.金額,A.内税消費税,A.外税消費税," +
                                "A.上代単価,A.上代金額,A.下代単価,A.下代金額,A.明細メモ,A.消費税計算方法" +
                                ",A.商品シリアル,A.関連伝票NO,A.関連伝票行NO,A.JANCODE,A.原価FLG,A.完了FLG";
        #endregion
        #region Initialize
        public void OnInit(object? init_para = null,string? init_flg = null) 
        {
            OnInitBase(init_para, init_flg);

            FlgSho = 0;
            EditSearch = new SearchModel();
            EditSearch.DenpyoNo1 = "0";
            EditSearch.DenpyoNo2 = "9999999999";
            EditSearch.NohinbiFrom = DateTime.Now;
            EditSearch.NohinbiTo = DateTime.MaxValue;      
            EditSearch.Kanren1From = "0";
            EditSearch.Kanren1To = "9999999999999";
            EditSearch.Kanren2From = "0";
            EditSearch.Kanren2To = "9999999999999";
            EditSearch.SupplierTo = new BtListHelper("99999999","");
            EditSearch.WareTo = new BtListHelper("99999999", "");
            EditSearch.ProductTo = new BtListHelper("zzzzzzzzzzzzzzzzzzzz", "");
            EditSearch.UserTo = new BtListHelper("99999999", "");
            DateName = "納品日";
            Csvname = "CSV出力";
            Renkeishow = false;
            ComboListKanryo = new Dictionary<string, string> 
            {
                {"0","0 未完" },
                {"1","1 完了" }
            };
            ComboListToriHikiFrom = new Dictionary<int, string>
            {
                {  00, "00" },
                {  10, "10 発注" },
                {  11, "11 追加発注" },
                {  15, "15 自動発注" }
            };
            EditSearch.ToriKubunFrom = ComboListToriHikiFrom.FirstOrDefault().Key;

            ComboListToriHikiTo = new Dictionary<int, string>
            {
                {  99, "99" },
                {  10, "10 発注" },
                {  11, "11 追加発注" },
                {  15, "15 自動発注" }
            };
            EditSearch.ToriKubunTo = ComboListToriHikiTo.FirstOrDefault().Key;
            SelectedOrderHeader = new OrderHeader();
            
            if (v_flg != null) MenuFlg = v_flg;
            var comboItem = AppData.ClassCvnet.comboItem00;
            ComboListRenkei = comboItem.ComboItem_00<int>("する");
            
            if (int.Parse(para[0]) == 2)
            {               
                ComboListToriHikiFrom = comboItem.ComboItem_00<int>("仕入返品指示区分");
                EditSearch.ToriKubunFrom = ComboListToriHikiFrom.FirstOrDefault().Key;
                ComboListToriHikiTo = comboItem.ComboItem_00<int>("仕入返品指示区分");
                EditSearch.ToriKubunTo = ComboListToriHikiTo.FirstOrDefault().Key;
                ComboListToriHiki = comboItem.ComboItem_00<int>("仕入返品指示区分");
                SelectedOrderHeader.ToriKubun = ComboListToriHikiTo.FirstOrDefault().Key;
                v_denkbn = 33;                
            }
          
            SelectedOrderHeader.Hachubi = DateTime.Now;
            SelectedOrderHeader.Nohinbi = DateTime.Now;
            SelectedOrderHeader.ToriKubun = 10;
            SelectedOrderHeader.User = new BtListHelper(AppData.ClassSatoo.SHAIN_CD,AppData.ClassSatoo.SHAIN_Name);
            
            var ret_csv = AppData.ClassCvnet.AspxSqlQueryImp();
            SelectedOrderHeader.Ware = new BtListHelper(ret_csv.Rows[0][1].ToString(), ret_csv.Rows[0][2].ToString());
              
            if (AppData.ClassCvnet.config.smtflg == 1)
            {               
                //Form1.TabFrame1.TabForm2.Label1.Visible = $TRUE;
                //Form1.TabFrame1.TabForm2.Text24.Visible = $TRUE;
                //Form1.TabFrame1.TabForm2.Text24.Value = sysdate();
            }

            if (int.Parse(para[0].ToString()) == 1)
            {
                EditSearch.WareFrom = new BtListHelper(AppData.ClassCvnet.SysImp.Rows[0][1].ToString(),AppData.ClassCvnet.SysImp.Rows[0][2].ToString());
                EditSearch.WareTo = new BtListHelper(AppData.ClassCvnet.SysImp.Rows[0][1].ToString(),AppData.ClassCvnet.SysImp.Rows[0][2].ToString());
                if (ret_csv.Rows[1][1].ToString() != "")
                {
                    EditSearch.WareFrom = new BtListHelper(ret_csv.Rows[1][1].ToString(),ret_csv.Rows[1][2].ToString());
                    EditSearch.WareTo = new BtListHelper(ret_csv.Rows[1][1].ToString(), ret_csv.Rows[1][2].ToString());
                    SelectedOrderHeader.Ware = new BtListHelper(ret_csv.Rows[1][1].ToString(), ret_csv.Rows[1][2].ToString());
                }
                if (AppData.ClassCvnet.config.TenpoFix == 1)
                {
                    sokoshow = false;
                    //Form1.TabFrame1.TabForm1.CvnetButton8.Active = $FALSE;
                    //Form1.TabFrame1.TabForm1.CvnetButton9.Active = $FALSE;
                    //Form1.TabFrame1.TabForm2.text8.Active = $FALSE;
                    //Form1.TabFrame1.TabForm2.CvnetButton8.Active = $FALSE;
                }
            }

            if (AppData.ClassCvnet.config.SKUFlg == 1)
            {
                //Form1.TabFrame1.TabForm2.Label99.Visible =$FALSE;
                //Form1.TabFrame1.TabForm2.TextSub01.Visible =$FALSE;
                //Form1.TabFrame1.TabForm2.TextSub01.Active =$FALSE;
                //Form1.TabFrame1.TabForm2.CvnetButton100.Visible =$FALSE;
                //Form1.TabFrame1.TabForm2.CvnetButton100.Active =$FALSE;
                //Form1.TabFrame1.TabForm2.CvnetButton12.Visible =$FALSE;
                //Form1.TabFrame1.TabForm2.CvnetButton12.Active =$FALSE;
                //Form1.TabFrame1.TabForm2.CvnetButton10.Visible =$FALSE;
                //Form1.TabFrame1.TabForm2.CvnetButton10.Active =$FALSE;
            }

            if (AppData.ClassCvnet.config.MultiCoop == 0)
            {
                Renkeishow = false;

                //Form1.TabFrame1.TabForm2.Label9.Visible =$false;
                //Form1.TabFrame1.TabForm2.Text25.Active =$false;
                //Form1.TabFrame1.TabForm2.Text25.Visible =$false;
            }
            GetShoriKaishibi();
            if (AppData.ClassCvnet.config.ExcelOutFlg == 1)
            {
                Csvname = "EXCEL出力";
            }
        }
        #endregion
        #region Function
        [RelayCommand]
        public void ClickProd()
        {
            FlgSho = FlgSho == 0 ? 1 : 0;
        }

        [RelayCommand]
        public void ClickDateTitle(string value) 
        {
            if (DateName == "納品日") 
            {
                DateName = "在庫計上日";
            }
            else
            {
                DateName = "納品日";
            }
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
            OnQuery(param1, "all", param2,null);
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
            OnQuery(param1, StartCode, param2,null);
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
            OnQuery(param1, StartCode, param2,"<=");
            if (ListOrder == null || ListOrder.Count == 0)
                ClientLib.MessageBoxOk(this, "データがありません");
        }
        void OnQuery(string[] param, string flg, string[] param2,string p_sort = null) 
        {
            var v_hugo = ">=";
            if (p_sort != null) {
                v_hugo = "<=";
            }
            var parameter = new string[18];
            if (FlgSho == 1) {
                parameter = new string[20];
            }
            for (var i = 0; i < param.Length; i++)
            {
                parameter[i] = param[i];
            }

            var sql_query = "select A.SEQ_NO,A.VDATE_CREATE,A.VDATE_UPDATE,";
            sql_query += sql_collist;
            sql_query += ",B.名前 担当名,C.仕入先名 仕入先名,D.得意先名 得意先名,C.掛率,C.掛率2,C.消費税CD,C.消費税計算方法,C.消費税端数";
            sql_query += ",'' 仕入数";
            sql_query += " from HC$Tran_TORI0 A,HC$MASTER_SHAIN B,HC$Master_SIIRE C,HC$MASTER_TOKUI D ";
            sql_query += "where (A.入力社員CD=B.社員CD(+)) and (A.取引先CD1=C.仕入先CD(+)) and (A.取引先CD2=D.得意先CD(+)) ";
            sql_query += "and A.SEQ_NO between :1 and :2 and A." + ((DateName == "納品日") ? "在庫計上日" : "納品日") + " between :3 and :4 and A.取引区分 between :5 and :6 ";
            sql_query += "and A.関連伝票NO between :7 and :8 and A.関連伝票NO2 between :9 and :10 and A.手入力伝票NO between :11 and :12 and A.取引先CD1 between :13 and :14 and A.取引先CD2 between :15 and :16 ";
            sql_query += "and A.入力社員CD between :17 and :18";
            sql_query += " and A.伝票処理区分=" + v_denkbn.ToString();

            if (EditSearch.Renkei.ToString() != "")
            {
                sql_query += " and a.sysflg2=" + EditSearch.Renkei.ToString();
            }

            if (flg != "all")
            {
                sql_query += " and A.SEQ_NO " + v_hugo + " '" + flg + "'";
            }
            if (FlgSho == 1)
            {
                parameter[18] = param2[0];
                parameter[19] = param2[1];
                sql_query = "SELECT A.* FROM (" + sql_query
                    + ") A WHERE EXISTS (SELECT /*+ INDEX(E 	HC$_NK_TORI23) */ 'X' FROM HC$TRAN_TORI1 E WHERE A.SEQ_NO=E.ヘッダNO AND E.商品CD BETWEEN :" + (parameter.Length - 1).ToString() + " AND :" + (parameter.Length).ToString() + ")";
            }
            
            sql_query += " ORDER BY A.SEQ_NO DESC";
            
            sql_query = "SELECT * FROM (" + sql_query + ") where rownum <= " + AppData.maxQueryCnt;
            

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
                            Nohinbi = (dr["納品日"].ToString().Substring(0,4)+"/"+ dr["納品日"].ToString().Substring(4, 2) + "/" + dr["納品日"].ToString().Substring(6, 2)).ToString() ?? string.Empty,
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
        void CreatePara()
        {
            param1[0] = EditSearch.DenpyoNo1;
            param1[1] = EditSearch.DenpyoNo2;

            var date_Str = EditSearch.NohinbiFrom?.ToString("yyyyMMdd");
            if (EditSearch.NohinbiFrom < DateTime.Parse(shoriKaishibi.Substring(0, 4) + "/" + shoriKaishibi.Substring(4, 2) + "/" + shoriKaishibi.Substring(6, 2))) date_Str = shoriKaishibi;
            param1[2] = date_Str;

            param1[3] = EditSearch.NohinbiTo?.ToString("yyyyMMdd");
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
        [RelayCommand]
        private void RowDoubleClick(OrderList item)
        {
            if (item != null)
            {
                SelectedOrderHeader.Supplier =new BtListHelper(item.Supplier, item.SupplierName);
                SelectedOrderHeader.Nohinbi = DateTime.Parse(item.Nohinbi);
                SelectedOrderHeader.Ware = new BtListHelper(item.Ware, item.WareName);
                SelectedOrderHeader.ToriKubun = int.Parse(item.Torihiki);
                SelectedOrderHeader.DenpyoNo = item.DenpyoNo;
                selectedOrderHeader.Hachubi = DateTime.Parse(item.Hachubi.Substring(0,4) + "/" + item.Hachubi.Substring(4, 2) + "/" + item.Hachubi.Substring(6, 2));
                SelectedOrderHeader.Tenyuryoku = item.Tenyuryoku;
                SelectedOrderHeader.Kanren1 = item.Kanren1;
                SelectedOrderHeader.Kanren2 = item.Kanren2;
                SelectedOrderHeader.User = new BtListHelper(item.Tanto,item.TantoName);

                var wrk_para = new string[2];
                wrk_para[0] = item.DenpyoNo;
                wrk_para[1] = item.Torihiki;
                OnQueryDetail(wrk_para);
                SelectedTabIndex = 1;
            } else return;
        }
        void OnQueryDetail(string[] param)
        {
            var sql_sub = "";

            sql_sub = ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='COL' and H.名称CD=A.色CD),'.') COL名"
            + ",GET_SIZENAME(A.商品CD,A.サイズCD) サイズ名";

            if (AppData.ClassCvnet.config.ColSizMei == 1) sql_sub = ",J.色名 COL名,J.サイズ名 サイズ名";

            var sql_query = "select A.SEQ_NO,A.VDATE_CREATE,A.VDATE_UPDATE,";

            sql_query += sql_collist1;

            sql_query += sql_sub;
            sql_query += ",NVL((select H.メーカー品番 from HC$master_shohin H where H.商品CD=A.商品CD),'.') MKR品番";
            sql_query += ",NVL((select H.仕入区分||' '||decode(H.仕入区分,1,'買取',2,'委託',3,'消化','') from HC$master_shohin H where H.商品CD=A.商品CD),'.') 仕入区分";

            sql_query += " from HC$MASTER_SHOHIN_JAN J,HC$tran_tori1 A";
            sql_query += " where A.商品CD=J.商品CD(+) AND A.色CD=J.色CD(+) AND A.サイズCD=J.サイズCD(+) AND A.ヘッダNO=:1 and A.明細取引区分=:2 order by A.ヘッダNO,A.行NO";

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
                            Abstracts = dr["明細メモ"].ToString() ?? string.Empty

                        }).OrderBy(c => c.ProductCD).ToList();
            Common.ConvertDotStringDel(list);
            SelectedOrderDetail = new ObservableCollection<OrderDetail>(list);
            int kei = 0;
            int koukei = 0;
            int koukei1 = 0;
            for (var i = 0; i < SelectedOrderDetail.Count; i++) {
                kei += SelectedOrderDetail[i].Weight;
                koukei += SelectedOrderDetail[i].JodaiKingaku;
                koukei1 += SelectedOrderDetail[i].GedaiKingaku;
            }
            Sum = kei;
            TotalJodai = koukei;
            TotalGedai = koukei1;
        }
        [RelayCommand]
        public void ProductExpand() 
        {
            if (SelectedOrderHeader.Supplier.Code == null || SelectedOrderHeader.Supplier.Code == "") 
            { ClientLib.MessageBoxError(this, "先に仕入先を入力してください"); 
                return; 
            }
            if (Product.Code == null || Product.Code == "") return;
            if(OnCheckMst() < 0) return;

            if (AppData.ClassCvnet.config.MakerOnly == 0)
            {
                var v_skbn = "";
                var v_sqlstr1 = "";
                var v_sqlstr = "select 商品CD,\"メーカーCD\" from HC$master_shohin where 商品CD = '" + Product.Code + "'";
                var wrk_csv = AppData.Http!.AspxSqlQuery(v_sqlstr);
                if (wrk_csv.Rows.Count > 0)
                {
                    for (var i = 0; i < wrk_csv.Rows.Count; i++)
                    {
                        if (wrk_csv.Rows[i][1].ToString() != SelectedOrderHeader.Supplier.Code)
                        {
                            ClientLib.MessageBoxError(this, "仕入先の商品CD以外が存在します！");
                            return;
                        }
                    }
                }
                var vm = new SubDlgSKU01ViewModel(Product.Code);
                var window = new SubDlgSKU01View { DataContext = vm };
                window.ShowDialog();
            }
        }

        [RelayCommand]
        private static int OnCheckMst() 
        {
            return 0;
        }
        #endregion
        public partial class SearchModel :ObservableObject 
        {
            [ObservableProperty]
            public string? denpyoNo1;
            [ObservableProperty]
            private string? denpyoNo2;
            [ObservableProperty]
            private DateTime? nohinbiFrom;
            [ObservableProperty]
            private DateTime? nohinbiTo;
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
            private string? createDate;
            [ObservableProperty]
            private string? updateDate;
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
        }
    }
}