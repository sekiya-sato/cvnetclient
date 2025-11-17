using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Models;
using CvnetClient.Utils;
using CvnetClient.Views;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CvnetClient.ViewModels
{
    public partial class SubDlgInp13ViewModel : BaseViewModel
    {
        #region Declare
        public enum OutPutType { List, Detail }
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
        private OutPutType selectedOutPut = OutPutType.List;
        [ObservableProperty]
        OrderList? selectedOrder;
        [ObservableProperty]
        OrderHeader? selectedOrderHeader;
        [ObservableProperty]
        ObservableCollection<OrderDetail>? selectedOrderDetail;
        [ObservableProperty]
        ObservableCollection<OrderDetail>? selectedOrderDetailRef;
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
        public Visibility renkeishow;
        public Visibility sokoshow;
        [ObservableProperty]
        private BtListHelper? product;
        [ObservableProperty]
        public string csvname;
        private int CSV_flg;
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
        string sql_collist2 = "明細取引区分,商品CD,色CD,サイズCD,明細名称,数量,単価,金額,内税消費税,外税消費税," +
                                "上代単価,上代金額,下代単価,下代金額,明細メモ,消費税計算方法" +
                                ",商品シリアル,関連伝票NO,関連伝票行NO,JANCODE,原価FLG,完了FLG";
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
            Renkeishow = Visibility.Collapsed;
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
                    sokoshow = Visibility.Collapsed;
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
                Renkeishow = Visibility.Collapsed;

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
            if (item == null || item.GetType().Name == "DefaultBindableSampleDataObject") return;
            
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
        [RelayCommand]
        public void ProductExpand() 
        {
            if (SelectedOrderHeader.Supplier.Code == null || SelectedOrderHeader.Supplier.Code == "") 
            { ClientLib.MessageBoxError(this, "先に仕入先を入力してください"); 
                return; 
            }
            if (Product.Code == null || Product.Code == "") return;
            OnCheckMst();
            

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
        private void AddNewRow()
        {
            if (SelectedOrderDetail == null)
                SelectedOrderDetail = new ObservableCollection<OrderDetail>();

            var newRow = new OrderDetail
            {
                ProductCD = "",
                ProductName = "",
                Maker = "",
                Size = "",
                SizeName = "",
                Color = "",
                ColorName = "",
                JodaiKingaku = 0,
                JodaiTanka = 0,
                GedaiKingaku = 0,
                GedaiTanka = 0,
                Weight = 0,
                Kubun = "",
                Kanryo = "",
                Abstracts = ""
            };

            SelectedOrderDetail.Add(newRow);
        }
        [RelayCommand]
        private void Refresh() 
        {
            if (SelectedOrderDetailRef == null) return;
            SelectedOrderDetail = new ObservableCollection<OrderDetail>(SelectedOrderDetailRef);
            UpdateSum(SelectedOrderDetail);
        }
        [RelayCommand]
        private void RowRefresh(OrderDetail rowItem) 
        {
            if (SelectedOrderDetailRef == null) return;

            int index = SelectedOrderDetail.IndexOf(rowItem);
            if (index < 0 || index >= SelectedOrderDetailRef.Count) return;

            var source = SelectedOrderDetailRef[index];
            var target = SelectedOrderDetail[index];    

            target.ProductCD = source.ProductCD;
            target.ProductName = source.ProductName;
            target.Kubun = source.Kubun;
            target.JodaiKingaku = source.JodaiKingaku;
            target.GedaiKingaku = source.GedaiKingaku;
            target.JodaiTanka = source.JodaiTanka;
            target.GedaiTanka = source.GedaiTanka;
            target.Abstracts = source.Abstracts;
            target.Color = source.Color;
            target.ColorName = source.ColorName;
            target.Size = source.Size;
            target.SizeName = source.SizeName;
            target.Weight = source.Weight;
        }
        [RelayCommand]
        private void OnCheckMst() 
        {
            return;
        }

        [RelayCommand]
        public void SelSupplier1(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && EditSearch != null)
            {
                EditSearch.SupplierFrom = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        [RelayCommand]
        public void SelSupplier2(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && EditSearch != null)
            {
                EditSearch.SupplierTo = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }

        [RelayCommand]
        public void SelWare1(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && EditSearch != null)
            {
                EditSearch.WareFrom = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        [RelayCommand]
        public void SelWare2(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && EditSearch != null)
            {
                EditSearch.WareTo = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }

        [RelayCommand]
        public void SelProduct1(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && EditSearch != null)
            {
                EditSearch.ProductFrom = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        [RelayCommand]
        public void SelProduct2(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && EditSearch != null)
            {
                EditSearch.ProductFrom = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }

        [RelayCommand]
        public void SelUser1(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && EditSearch != null)
            {
                EditSearch.UserFrom = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        [RelayCommand]
        public void SelUser2(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && EditSearch != null)
            {
                EditSearch.UserTo = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }


        [RelayCommand]
        public void SelSupplierDetail(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && SelectedOrderHeader != null)
            {
                SelectedOrderHeader.Supplier = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        [RelayCommand]
        public void SelWareDetail(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && SelectedOrderHeader != null)
            {
                SelectedOrderHeader.Ware = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }

        [RelayCommand]
        public void SelUserDetail(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && SelectedOrderHeader != null)
            {
                SelectedOrderHeader.User = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        [RelayCommand]
        public void SelProductDetail(object value)
        {
            var get_sel00 = (SelValueModel)value;
            Product = new BtListHelper(get_sel00.Code, get_sel00.Name);
            
        }
        [RelayCommand]
        async Task DoPrintAsync()
        {
            if (!ClientLib.MessageBox(this, "印刷しますか？")) return;

            CSV_flg = 0;

            CreatePara();
            var ret_csv = "";
            if (SelectedOutPut == OutPutType.List)
            {
                /* 一覧 */
                ret_csv = OnQueryPrint(param1, param2);
            }
            else
            {
                /* 明細 */
                ret_csv = OnQueryDetailPrint(param1, "1", param2);
            }
            var lines = ret_csv.Split('\n');

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
        public void DOCSV() 
        {

            /* 2016.09.27 #30512 */
            CSV_flg = 1;

            /* パラメータ生成 */
            CreatePara();



            //var f_name = ^.OptionButton1.OptionItem1[^.OptionButton1.Value].Title;
            //var ret_csv = new SatooCSVDocument;
            //if (SelectedOutPut == OutPutType.List)
            //{
                
            //    OnQueryPrint(param1, param2);
            //}
            //else
            //{
                
            //    OnQueryDetailPrint(param1, "1", param2);
            //}

            //var total_cnt = ret_csv.GetCell(1, 0);
            //if (total_cnt <= 0)
            //{
            //    Form1.OnMess2("出力データがありませんでした");
            //    pp.popupClose();
            //    return;
            //}

            ///* 2021.04.27 商品名称CDのラベル付け */
            //var ret_name = ClassSatoo.AspxSqlQuery("select m.名称 from HC$MASTER_MEISHO m where m.名称区分='IDX' and m.名称CD between 'B01' and 'B10' order by m.名称CD");
            //var name_cnt = 0;
            //var name_flg = 0;

            //var get_csv = new SatooCSVDocument;
            //get_csv.get(ClassSatoo.AspxPath + ret_csv.getcell(0, 0) + "/data.txt");
            //var get_hedder = new SatooCSVDocument;
            //get_hedder.get(ClassSatoo.AspxPath + ret_csv.getcell(0, 0) + "/d_sql.txt");
            //if (get_csv.getcell(0, 0) == "H") get_csv.deleteRow(0);
            //get_csv.insertrow(0);
            //for (var i = 0; i < get_csv.columns; i++)
            //{
            //    /* 2021.04.27 商品名称CDのラベル付け */
            //    if (^.OptionButton1.Value == 1 && mid(get_hedder.getcell(0, i), 0, 4) == "名称CD" && ret_name.rows == 10)
            //    {
            //        if (name_flg == 0)
            //        {
            //            get_csv.setcell(0, i, str(ret_name.getcell(name_cnt, 0)) + "CD");
            //            name_flg = 1;
            //        }
            //        else
            //        {
            //            get_csv.setcell(0, i, str(ret_name.getcell(name_cnt, 0)) + "名");
            //            name_cnt++;
            //            name_flg = 0;
            //        }
            //    }
            //    else
            //    {
            //        get_csv.setcell(0, i, get_hedder.getcell(0, i));
            //    }
            //}

            ///* 2021.02.18 #56430対応修正  */
            //if (cvnet.Config.ExcelOutFlg == 1)
            //{

            //    var dtCSVFlNm = "";
            //    var ExcelFlNm = "";

            //    try
            //    {

            //        if (str(f_name) == "一覧")
            //        {
            //            print("一覧");
            //            dtCSVFlNm = "DataHatchuIchiran.csv";
            //            ExcelFlNm = "HatchuIchiran_macro.xlsm";
            //        }
            //        else if (str(f_name) == "明細")
            //        {
            //            print("明細");
            //            dtCSVFlNm = "DataHatchuMeisai.csv";
            //            ExcelFlNm = "HatchuMeisai_macro.xlsm";
            //        }

            //        /* マクロファイル保存 */
            //        var ExcelDrNm = "/Data/Excel";
            //        var ses = ClassSatoo.AspxFindHTTPSession();
            //        var res = ses.get(ClassSatoo.AspxPath + ExcelDrNm + "/" + ExcelFlNm);
            //        DebugMessage("ClassSatoo.AspxPath + ExcelDrNm + / + ExcelFlNm = ", ClassSatoo.AspxPath + ExcelDrNm + "/" + ExcelFlNm, "\n");
            //        try
            //        {
            //            var fs = new FileSystem(FileSystem.PUBLIC_ROOT);
            //            var f = fs.open("/" + ExcelFlNm, FileSystem.OPEN_WRITE);
            //            f.write(res);
            //            f.close();
            //        }
            //        catch (ex)
            //        {
            //            pp.popupClose();
            //            DebugMessage(ex.message);
            //        }

            //        var fs = new FileSystem(FileSystem.PUBLIC_ROOT);
            //        var f = fs.open("/" + dtCSVFlNm, FileSystem.OPEN_WRITE);
            //        get_csv.save(f);
            //        f.close();

            //        try
            //        {
            //            var rt = new Runtime;
            //            rt.ShellOpen(str(ExcelFlNm));
            //        }
            //        catch (ex)
            //        {
            //            pp.popupClose();
            //            DebugMessage(ex.message);
            //        }

            //        Form1.OnMess2("データを保存しました");
            //    }
            //    catch (e)
            //    {
            //        Form1.OnMess2("保存を中止しました");
            //    }

            //}
            //else
            //{

            //    var v_title = "発注伝票";
            //    if (MenuFlg == "1") v_title = "入荷予定伝票";
            //    try
            //    {
            //        var fs = new FileSystem;
            //        var f = fs.SaveDialog("CSVデータ保存", "CSVファイル(*.CSV)=*.CSV", "csv", v_title + str(f_name) + ".csv");
            //        get_csv.save(f);
            //        f.close();
            //        Form1.OnMess2("データを保存しました");
            //    }
            //    catch (e)
            //    {
            //        Form1.OnMess2("保存を中止しました");
            //    }

            //}
        }
        public string OnQueryPrint(string[] wrk_para1, string[] para1) 
        {
            string[] wrk_para;
            if (FlgSho == 0)
            {
                wrk_para = new string[18];
            }
            else 
            {
                wrk_para = new string[20];
            }
            for (var i = 0; i < wrk_para1.Length; i++)
            {
                    wrk_para[i] = wrk_para1[i];
            }

            var v_title = "発注";
            if (MenuFlg == "1") v_title = "入荷予定";

            var sql_query = "select A.SEQ_NO";
            sql_query += ",SUBSTR(GET_VDATE(a.VDATE_CREATE),0,8)||SUBSTR(GET_VDATE(a.VDATE_CREATE),10,6) 作成日時";
            sql_query += ",SUBSTR(GET_VDATE(a.VDATE_UPDATE),0,8)||SUBSTR(GET_VDATE(a.VDATE_UPDATE),10,6) 更新日時,";
            sql_query += "'" + v_title + "伝票一覧',";          
            sql_query += sql_collist;           
            sql_query += " B.名前 担当名, C.仕入先名 仕入先名, D.得意先名 得意先名, C.消費税CD, C.消費税計算方法, C.消費税端数 ";
            sql_query += ", A.SYSFLG, A.送信FLG ";
            sql_query += "," + AppData.ClassCvnet.comboItem00.GetCaseStr(tori_kbn, "A.取引区分") + " 取引区分名";
            sql_query += ",decode( A.伝票処理区分, 33, '仕入返品指示', '" + v_title + "' ) タイトル";
            sql_query += "," + AppData.ClassCvnet.comboItem00.GetCaseStr("する", "A.SYSFLG2") + " 連携";
            sql_query += " from HC$Tran_TORI0 A, HC$MASTER_SHAIN B, HC$Master_SIIRE C, HC$MASTER_TOKUI D ";
            sql_query += " where (A.入力社員CD=B.社員CD(+)) and (A.取引先CD1=C.仕入先CD(+)) and (A.取引先CD2=D.得意先CD(+)) ";
            sql_query += "and A.SEQ_NO between :1 and :2 and A." + DateName + " between :3 and :4 and A.取引区分 between :5 and :6 ";
            sql_query += "and A.関連伝票NO between :7 and :8 and A.関連伝票NO2 between :9 and :10 and A.手入力伝票NO between :11 and :12 and A.取引先CD1 between :13 and :14 and A.取引先CD2 between :15 and :16";
            sql_query += "and A.入力社員CD between :17 and :18";
            sql_query += " and A.伝票処理区分=" + v_denkbn;
            if (FlgSho == 1)
            {
                wrk_para[18] = EditSearch.ProductFrom.Code ?? string.Empty;
                wrk_para[19] = EditSearch.ProductTo.Code ?? string.Empty;
                sql_query = "SELECT A.* FROM (" + sql_query
                    + ") A WHERE EXISTS (SELECT /*+ INDEX(E 	HC$_NK_TORI23) */ 'X' FROM HC$TRAN_TORI1 E WHERE A.SEQ_NO=E.ヘッダNO AND E.商品CD BETWEEN :" + (wrk_para.Length - 1).ToString() + " AND :" + (wrk_para.Length).ToString() + ")";
            }
            sql_query += " order by A.SEQ_NO";
            if (CSV_flg == 1)
            {
                sql_query = ""
                    + "select "
                    + " a.SEQ_NO 伝票No , "
                    + " a.在庫計上日 発注日 , "
                    + " a.納品日 , "
                    + " DECODE(a.掛計上日, 19010101, '', a.掛計上日) 掛計上日 , "
                    + " a.伝票処理区分 伝票区分 , "
                    + " substrb(a.取引区分名, 1, instr(a.取引区分名, ' ')-1) 取引区分CD , "
                    + " substrb(a.取引区分名, instr(a.取引区分名, ' ')+1) 取引区分名 , "
                    + " a.掛率1 掛率 , "
                    + " a.SYSFLG , "
                    + " a.送信FLG , "
                    + " substrb(a.連携, 1, instr(a.連携, ' ')-1) 連携FLG , "
                    + " substrb(a.連携, instr(a.連携, ' ')+1) 連携 , "
                    + " a.数量合計 数量計 , "
                    + " a.明細金額合計 金額計 , "
                    + " a.上代合計 , "
                    + " a.取引先CD1 仕入先CD , "
                    + " a.仕入先名 , "
                    + " a.取引先CD2 入庫先CD , "
                    + " a.得意先名 入庫先名 , "
                    + " a.入力社員CD 入力者CD , "
                    + " a.担当名 入力者名 , "
                    + " a.外税消費税 消費税計 , "
                    + " a.下代合計 , "
                    + " a.手入力伝票NO 手入力No , "
                    + " a.関連伝票NO 関連No1 , "
                    + " a.関連伝票NO2 関連No2 , "
                    + " a.メモ "
                    + "  from "
                    + "(" + sql_query + ") a";
            }
            return AppData.Http!.AspxSqlQueryCsv(sql_query, wrk_para, "cvnet13prn_header.qfm");
        }
        public string OnQueryDetailPrint(string[] wrk_para1, string hdFlg, string[] v_sho) 
        {
            string[] wrk_para;
            if (FlgSho == 0)
            {
                wrk_para = new string[18];
            }
            else {
                wrk_para = new string[20];
            }
            for (var i = 0; i < wrk_para1.Length; i++)
            {
                 wrk_para[i] = wrk_para1[i];
            }
            var sql_sub = "";
            sql_sub = ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='COL' and H.名称CD=T1.色CD),'.') COL名"
            + ",GET_SIZENAME(T1.商品CD,T1.サイズCD) サイズ名";
            if (AppData.ClassCvnet.config.ColSizMei == 1) sql_sub = ",J.色名 COL名,J.サイズ名";
            var sql_query = "select A.SEQ_NO";
            sql_query += ",SUBSTR(GET_VDATE(a.VDATE_CREATE),0,8)||SUBSTR(GET_VDATE(a.VDATE_CREATE),10,6) 作成日時";
            sql_query += ",SUBSTR(GET_VDATE(a.VDATE_UPDATE),0,8)||SUBSTR(GET_VDATE(a.VDATE_UPDATE),10,6) 更新日時";
            if (MenuFlg == "1")
            {
                sql_query += ",'入荷予定伝票明細'";
            }
            else
            {
                sql_query += ",'発注伝票明細'";
            }

            sql_query += sql_collist;
            
            sql_query += ",B.名前 担当名, C.仕入先名 仕入先名, D.得意先名 得意先名, C.消費税CD, C.消費税計算方法, C.消費税端数 ";
            sql_query += ",A.SYSFLG, A.送信FLG";
            string[] col_list2 = sql_collist2.Split(',');
            for (int i = 0; i < col_list2.Length; i++)
            {
                sql_query += ",T1." + col_list2[i] + " tt" + col_list2[i];
            }
            sql_query += ",A.関連伝票NO2 関連伝票NO2b" + sql_sub + "," + AppData.ClassCvnet.comboItem00.GetCaseStr(tori_kbn, "A.取引区分") + " 取引区分名";
            sql_query += ",T1.行NO";
            sql_query += ",decode(T1.完了FLG,1,'完了','未完') 完了FLG名";
            sql_query += ",NVL((select H.メーカー品番 from HC$master_shohin H where H.商品CD=t1.商品CD),'.') MKR品番";
            sql_query += ",NVL((select H.仕入区分||' '||decode(H.仕入区分,1,'買取',2,'委託',3,'消化','') from HC$master_shohin H where H.商品CD=t1.商品CD),'.') 仕入区分";


            if (MenuFlg == "1")
            {
                sql_query += ",decode( A.伝票処理区分, 33, '仕入返品指示', '入荷予定' ) タイトル";
            }
            else
            {
                sql_query += ",decode( A.伝票処理区分, 33, '仕入返品指示', '発注' ) タイトル";
            }

            sql_query += "," + AppData.ClassCvnet.comboItem00.GetCaseStr("する", "A.SYSFLG2") + " 連携";

            sql_query += " from HC$MASTER_SHOHIN_JAN J,HC$Tran_TORI0 A, HC$MASTER_SHAIN B, HC$Master_SIIRE C, HC$MASTER_TOKUI D ";
            sql_query += " ,HC$tran_TORI1 T1";
            sql_query += " where J.商品CD(+)=T1.商品CD AND J.色CD(+)=T1.色CD AND J.サイズCD(+)=T1.サイズCD AND (A.入力社員CD=B.社員CD(+)) and (A.取引先CD1=C.仕入先CD(+)) and (A.取引先CD2=D.得意先CD(+))";
            sql_query += "and (A.SEQ_NO=T1.ヘッダNO) ";
            sql_query += " and A.伝票処理区分=" + v_denkbn;
            if (hdFlg == "1")
            {
                sql_query += " and A.SEQ_NO between :1 and :2 and A." + DateName + " between :3 and :4 and A.取引区分 between :5 and :6 ";
                sql_query += " and A.関連伝票NO between :7 and :8 and A.関連伝票NO2 between :9 and :10 and A.手入力伝票NO between :11 and :12 and A.取引先CD1 between :13 and :14 and A.取引先CD2 between :15 and :16";
                sql_query += " and A.入力社員CD between :17 and :18";
                if (FlgSho == 1)
                {
                    wrk_para[18] = EditSearch.ProductFrom.Code ?? string.Empty;
                    wrk_para[19] = EditSearch.ProductTo.Code ?? string.Empty;
                    sql_query += " AND EXISTS (SELECT /*+ INDEX(E 	HC$_NK_TORI23) */ 'X' FROM HC$TRAN_TORI1 E WHERE A.SEQ_NO=E.ヘッダNO AND E.商品CD BETWEEN :" + (wrk_para.Length - 1).ToString() + " AND :" + (wrk_para.Length).ToString() + ")";
                }
            }
            else
            {
                sql_query += " and T1.ヘッダNO=:1 and T1.明細取引区分=:2";
            }
            sql_query += " order by T1.ヘッダNO, T1.行NO";

            if (CSV_flg == 1)
            {
                sql_query = ""
                    + "select "
                        + " a.SEQ_NO 伝票No , "
                        + " a.在庫計上日 発注日 , "
                        + " a.納品日 , "
                        + " DECODE(a.掛計上日, 19010101, '', a.掛計上日) 掛計上日 , "
                        + " a.伝票処理区分 伝票区分 , "
                        + " substrb(a.取引区分名, 1, instr(a.取引区分名, ' ')-1) 取引区分CD , "
                        + " substrb(a.取引区分名, instr(a.取引区分名, ' ')+1) 取引区分名 , "
                        + " a.掛率1 掛率 , "
                        + " a.SYSFLG ,"
                        + " a.送信FLG , "
                        + " substrb(a.連携, 1, instr(a.連携, ' ')-1) 連携FLG , "
                        + " substrb(a.連携, instr(a.連携, ' ')+1) 連携 , "
                        + " a.数量合計 数量計 , "
                        + " a.明細金額合計 金額計 , "
                        + " a.上代合計 , "
                        + " a.下代合計 , "
                        + " a.取引先CD1 仕入先CD , "
                        + " a.仕入先名 , "
                        + " a.取引先CD2 入庫先CD , "
                        + " a.得意先名 入庫先名 , "
                        + " a.入力社員CD 入力者CD , "
                        + " a.担当名 入力者名 , "
                        + " a.外税消費税 消費税計 , "
                        + " a.手入力伝票NO 手入力No , "
                        + " a.関連伝票NO 関連No1 , "
                        + " a.関連伝票NO2 関連No2 ,"
                        + " a.メモ , "
                        + " a.行NO 行No , "
                        + " a.tt商品CD 商品CD , "
                        + " a.tt明細名称 商品名 , "
                        + " a.tt色CD 色CD , "
                        + " a.COL名 色名 , "
                        + " a.ttサイズCD サイズCD , "
                        + " a.サイズ名  , "
                        + " a.tt完了FLG 完了FLG , "
                        + " a.完了FLG名 完了 , "
                        + " a.tt数量 数量 , "
                        + " a.tt単価 単価 , "
                        + " a.tt上代単価 上代単価 , "
                        + " a.tt下代単価 下代単価 , "
                        + " a.tt原価FLG 原価FLG , "
                        + " a.tt明細メモ 摘要 , "
                        + " a.MKR品番 メーカー品番 , "
                        + " substrb(a.仕入区分, 1, instr(a.仕入区分, ' ')-1) 仕入区分CD , "
                        + " substrb(a.仕入区分, instr(a.仕入区分, ' ')+1) 仕入区分名 , "
                        + " a.tt外税消費税 消費税 , "
                        + " a.tt金額 金額 , "
                        + " a.tt上代金額 上代金額 , "
                        + " a.tt下代金額 下代金額 , "
                        + " nvl(s.上代,0) マスタ上代 , "
                        + " nvl(s.元上代,0) 元上代 , "
                        + " nvl(s.仕入価格,0) 仕入価格 , "
                        + " GET_GENKA(a.tt商品CD,0,a.在庫計上日) マスタ原価 , "
                        + " nvl(s.営業原価,0) 営業原価 , "
                        + " nvl(s.旧コード,'') 旧コード , "
                        + " nvl(s.略称,'') 略称 , "
                        + " nvl(s.展示会CD,'') 展示会CD , "
                        + " nvl((select m.名称 from HC$MASTER_MEISHO m where m.名称区分='TNJ' and m.名称CD=s.展示会CD),'') 展示会名 , "
                        + " nvl(s.ブランドCD,'') ブランドCD , "
                        + " nvl((select m.名称 from HC$MASTER_MEISHO m where m.名称区分='BRD' and m.名称CD=s.ブランドCD),'') ブランド名 , "
                        + " nvl(s.アイテムCD,'') アイテムCD , "
                        + " nvl((select m.名称 from HC$MASTER_MEISHO m where m.名称区分='ITM' and m.名称CD=s.アイテムCD),'') アイテム名 , "
                        + " nvl(s.シーズンCD,'') シーズンCD , "
                        + " nvl((select m.名称 from HC$MASTER_MEISHO m where m.名称区分='SZN' and m.名称CD=s.シーズンCD),'') シーズン名 , "
                        + " nvl(s.素材CD,'') 素材CD , "
                        + " nvl((select m.名称 from HC$MASTER_MEISHO m where m.名称区分='SZI' and m.名称CD=s.素材CD),'') 素材名 , "
                        + " nvl(s.デザイナーCD,'') デザイナーCD , "
                        + " nvl((select m.名称 from HC$MASTER_MEISHO m where m.名称区分='DZN' and m.名称CD=s.デザイナーCD),'') デザイナー名 , "
                        + " nvl(s.メーカーCD,'') メーカーCD , "
                        + " nvl((select m.名称 from HC$MASTER_MEISHO m where m.名称区分='MKR' and m.名称CD=s.メーカーCD),'') メーカー名 , "
                        + " nvl(s.原産国CD,'') 原産国CD , "
                        + " nvl((select m.名称 from HC$MASTER_MEISHO m where m.名称区分='GEN' and m.名称CD=s.原産国CD),'') 原産国名 , "
                        + " nvl(s.名称CD01,'') 名称CD01 , "
                        + " nvl((select m.名称 from HC$MASTER_MEISHO m where m.名称区分='B01' and m.名称CD=s.名称CD01),'') 名称CD01名 , "
                        + " nvl(s.名称CD02,'') 名称CD02 , "
                        + " nvl((select m.名称 from HC$MASTER_MEISHO m where m.名称区分='B02' and m.名称CD=s.名称CD02),'') 名称CD02名 , "
                        + " nvl(s.名称CD03,'') 名称CD03 , "
                        + " nvl((select m.名称 from HC$MASTER_MEISHO m where m.名称区分='B03' and m.名称CD=s.名称CD03),'') 名称CD03名 , "
                        + " nvl(s.名称CD04,'') 名称CD04 , "
                        + " nvl((select m.名称 from HC$MASTER_MEISHO m where m.名称区分='B04' and m.名称CD=s.名称CD04),'') 名称CD04名 , "
                        + " nvl(s.名称CD05,'') 名称CD05 , "
                        + " nvl((select m.名称 from HC$MASTER_MEISHO m where m.名称区分='B05' and m.名称CD=s.名称CD05),'') 名称CD05名 , "
                        + " nvl(s.名称CD06,'') 名称CD06 , "
                        + " nvl((select m.名称 from HC$MASTER_MEISHO m where m.名称区分='B06' and m.名称CD=s.名称CD06),'') 名称CD06名 , "
                        + " nvl(s.名称CD07,'') 名称CD07 , "
                        + " nvl((select m.名称 from HC$MASTER_MEISHO m where m.名称区分='B07' and m.名称CD=s.名称CD07),'') 名称CD07名 , "
                        + " nvl(s.名称CD08,'') 名称CD08 , "
                        + " nvl((select m.名称 from HC$MASTER_MEISHO m where m.名称区分='B08' and m.名称CD=s.名称CD08),'') 名称CD08名 , "
                        + " nvl(s.名称CD09,'') 名称CD09 , "
                        + " nvl((select m.名称 from HC$MASTER_MEISHO m where m.名称区分='B09' and m.名称CD=s.名称CD09),'') 名称CD09名 , "
                        + " nvl(s.名称CD10,'') 名称CD10 , "
                        + " nvl((select m.名称 from HC$MASTER_MEISHO m where m.名称区分='B10' and m.名称CD=s.名称CD10),'') 名称CD10名 , "
                        + " nvl(j.JANコード1,'') JANコード1 , "
                        + " nvl(j.JANコード2,'') JANコード2 , "
                        + " nvl(j.JANコード3,'') JANコード3 "
                    + " from "
                        + " (" + sql_query + ") a , "
                        + " HC$MASTER_SHOHIN s , "
                        + " HC$MASTER_SHOHIN_JAN j "
                    + " where "
                        + " a.tt商品CD=s.商品CD(+) "
                        + " and a.tt商品CD=j.商品CD(+) "
                        + " and a.tt色CD=j.色CD(+) "
                        + " and a.ttサイズCD=j.サイズCD(+) "
                    + " order by "
                        + " a.SEQ_NO , "
                        + " a.行NO "
                    + "";
            }
            return AppData.Http!.AspxSqlQueryCsv(sql_query, wrk_para, "cvnet13prn_detail.qfm");
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