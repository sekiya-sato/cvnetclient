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
        OrderHeader? selectedOrderHeaderRef;
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
        [ObservableProperty]
        public Visibility sokoshow;
        [ObservableProperty]
        private BtListHelper? product;
        [ObservableProperty]
        public string csvname;
        private int CSV_flg;        
        private string[] param1 = new string[18];
        private string[] param2 = new string[2];
        string sql_collist1 = "手入力伝票NO,在庫計上日,納品日,取引区分,入力社員CD,取引先CD2," +
                            "取引先CD1,掛率1,外税対象金額,数量合計,明細金額合計," +
                            "内税消費税,外税消費税,上代合計,下代合計,メモ,掛計上FLG,伝票処理区分,MOD_SEQ,倉庫CD,関連伝票NO" +
                            ",掛計上日,SYSFLG2,関連伝票NO2";
        string sql_collist2 = "明細取引区分,商品CD,色CD,サイズCD,明細名称,数量,単価,金額,内税消費税,外税消費税," +
                                "上代単価,上代金額,下代単価,下代金額,明細メモ,消費税計算方法" +
                                ",商品シリアル,関連伝票NO,関連伝票行NO,JANCODE,原価FLG,完了FLG";
        string[] col_list1;
        string[] col_list2;
        #endregion
        #region Initialize
        public void OnInit(object? init_para = null,string? init_flg = null) 
        {
            OnInitBase(init_para, init_flg);
            col_list1 = sql_collist1.Split(',');
            col_list2 = sql_collist2.Split(',');
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
            SelectedOrderHeader = new OrderHeader{ CreateDate = 0, UpdateDate = 0,Supplier = new BtListHelper("",""),Ware=new BtListHelper("","") };
            SelectedOrderHeaderRef = new OrderHeader { CreateDate = 0, UpdateDate = 0, Supplier = new BtListHelper("", ""), Ware = new BtListHelper("", "") };
            Product = new BtListHelper("","");
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
                    Sokoshow = Visibility.Collapsed;
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
        #region FirstTab
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

            var sql_query = "select A.SEQ_NO,A.VDATE_CREATE,A.VDATE_UPDATE";
            for (int i = 0; i < col_list1.Length; i++)
            {
                sql_query += ",A." + col_list1[i];
            }
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
            var sql_sub = "";

            sql_sub = ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='COL' and H.名称CD=A.色CD),'.') COL名"
            + ",GET_SIZENAME(A.商品CD,A.サイズCD) サイズ名";

            if (AppData.ClassCvnet.config.ColSizMei == 1) sql_sub = ",J.色名 COL名,J.サイズ名 サイズ名";

            var sql_query = "select A.SEQ_NO,A.VDATE_CREATE,A.VDATE_UPDATE";

            for (int i = 0; i < col_list2.Length; i++)
            {
                sql_query += ",A." + col_list2[i];
            }

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
        [RelayCommand]
        private void DoDelete() 
        {
            if (SelectedOrder == null) return;
            var sir_day =DateTime.Parse(SelectedOrder.Nohinbi);
            if (AppData.ClassCvnet.CheckImpDate(sir_day) < 0)
            {
                ClientLib.MessageBoxError(this,"修正可能な伝票ではありません。");
                return;
            }

            if (AppData.ClassCvnet.config.MultiCoop != 0)
            {
                if(OnCheckErrorhj() < 0) { return; }
            }

            if (!ClientLib.MessageBox(this, "削除しますか？")) return;            
            var ret = AppData.Http!.AspxSqlExe(DBDef.DB_DML.DELETE, "Tran_TORI0", long.Parse(SelectedOrder.DenpyoNo!), SelectedOrder.VdateUpdate.ToString(),
                new string[0], new string[0]);
            if (ret.Code == 0)
            {
                if (SelectedOrder != null)
                {
                    ListOrder!.Remove(SelectedOrder);
                    var item = ListOrder.Where(c => c.DenpyoNo == ListOrder.Min(c => c.DenpyoNo)).FirstOrDefault();
                    SelectedOrder = item;
                }
            }
            else
            {
                ClientLib.MessageBoxError(this, ret.Code.ToString());
            }
        }
        #endregion
        #region SupportFunction
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
        private int OnCheckMst()
        {/* 得意先チェック */
            if (SelectedOrderHeader.Ware.Code != "" && SelectedOrderHeaderRef.Ware.Code != SelectedOrderHeader.Ware.Code)
            {
                
                var v_sqlstr = "select 得意先CD,得意先名 from HC$master_TOKUI where 出荷停止FLG = 0 and 得意先CD=:1 and (店種区分 = 0 or (店種区分 between 3 and 8) or (店種区分=1 and 在庫管理FLG=1)) " + AppData.ClassCvnet.GetQueryStrHoujin();
                var v_array = new string[1];
                v_array[0] = new string(SelectedOrderHeader.Ware.Code);
                var wrk_csv = AppData.Http!.AspxSqlQuery(v_sqlstr, v_array);
                if (wrk_csv.Rows.Count > 0)
                {
                    SelectedOrderHeader.Ware = new BtListHelper(wrk_csv.Rows[0][0].ToString(),wrk_csv.Rows[0][1].ToString());
                    SelectedOrderHeaderRef.Ware = new BtListHelper(wrk_csv.Rows[0][0].ToString(),wrk_csv.Rows[0][1].ToString());
                }
                else
                {
                    SelectedOrderHeader.Ware = new BtListHelper();
                    ClientLib.MessageBoxError(this, "得意先CDがマスタに存在しません｡");
                    return -1;
                }
            }
            /* 仕入先チェック */
            if (SelectedOrderHeader.Supplier.Code != "" && SelectedOrderHeaderRef.Supplier.Code != SelectedOrderHeader.Supplier.Code)
            {
                var v_sqlstr = "select 仕入先CD,仕入先名,掛率,掛率2,消費税CD,消費税計算方法,消費税端数 from HC$master_siire where 仕入先CD=:1" + AppData.ClassCvnet.GetQueryStrHoujin(); /* 2010.09.22　法人CD対応 */
                var v_array = new string[1];
                v_array[0] = new string(SelectedOrderHeader.Supplier.Code);
                var wrk_csv = AppData.Http!.AspxSqlQuery(v_sqlstr, v_array);
                if (wrk_csv.Rows.Count > 0)
                {
                    SelectedOrderHeader.Supplier = new BtListHelper(wrk_csv.Rows[0][0].ToString(),wrk_csv.Rows[0][1].ToString());
                    SelectedOrderHeaderRef.Supplier = new BtListHelper(wrk_csv.Rows[0][0].ToString(),wrk_csv.Rows[0][1].ToString());
                }
                else
                {
                    SelectedOrderHeader.Supplier = new BtListHelper();
                    ClientLib.MessageBoxError(this, "仕入先CDがマスタに存在しません｡");
                    return -1;
                }
            }
            /* 入力者チェック 21.10.13 */
            if (SelectedOrderHeader.User.Code != "" && SelectedOrderHeader.User.Code != "." && SelectedOrderHeaderRef.User.Code != SelectedOrderHeader.User.Code)
            {
                var v_sqlstr = "select 社員CD,名前 from HC$MASTER_SHAIN where 社員CD=:1";
                var v_array = new string[1];
                v_array[0] = new string(SelectedOrderHeader.User.Code);
                var wrk_csv = AppData.Http!.AspxSqlQuery(v_sqlstr, v_array);
                if (wrk_csv.Rows.Count > 0)
                {
                    SelectedOrderHeader.User = new BtListHelper(wrk_csv.Rows[0][0].ToString(),wrk_csv.Rows[0][1].ToString());
                    SelectedOrderHeaderRef.User = new BtListHelper(wrk_csv.Rows[0][0].ToString(), wrk_csv.Rows[0][1].ToString());
                }
                else
                {
                    SelectedOrderHeader.User = new BtListHelper();
                    ClientLib.MessageBoxError(this,"入力者CDがマスタに存在しません｡");
                    return -1;
                }
            }
            return 0;
        }
        private int OnCheckErrorhj()
        {
            var v_sqlstr = "select 送信FLG from HC$TRAN_TORI0 WHERE SEQ_NO=:1 ";
            var v_array = new string[1];
            v_array[0] = SelectedOrder.DenpyoNo;
            var wrk_csv = AppData.Http!.AspxSqlQuery(v_sqlstr, v_array);
            if (wrk_csv.Rows.Count > 0)
            {
                if (wrk_csv.Rows[0][0].ToString() == "1")
                {
                    ClientLib.MessageBoxError(this, "法人連携済みです！");
                    return -1;
                }
            }
            return 0;
        }
        private int OnCheckError()
        {
            if (SelectedOrderDetail == null)
            {
                ClientLib.MessageBoxError(this,"明細レコードがありません！");
                return -1;
            }
            if (SelectedOrderHeader.Supplier.Code == "")
            {
                ClientLib.MessageBoxError(this, "仕入先を入力して下さい！");
                return -1;
            }
            if (SelectedOrderHeader.Ware.Code == "")
            {
                ClientLib.MessageBoxError(this, "入庫先を入力して下さい！");
                return -1;
            }
            for (var j = 0; j < SelectedOrderDetail.Count; j++) {
                if (SelectedOrderDetail[j].ProductCD == "" || SelectedOrderDetail[j].ProductCD == null)
                {
                    ClientLib.MessageBoxError(this, "商品CDを入力して下さい！");
                    return -1;
                }
                if ((SelectedOrderDetail[j].Color == "" || SelectedOrderDetail[j].Color == null) && AppData.ClassCvnet.config.SKUFlg != 1)
                {
                    ClientLib.MessageBoxError(this, "色CDを入力して下さい！");
                    return -1;
                }
                if ((SelectedOrderDetail[j].Size == "" || SelectedOrderDetail[j].Size == null) && AppData.ClassCvnet.config.SKUFlg != 1)
                {
                    ClientLib.MessageBoxError(this, "サイズCDを入力して下さい！");
                    return -1;
                }
                if (SelectedOrderDetail[j].Weight == 0)
                {
                    ClientLib.MessageBoxError(this, "数量を入力して下さい！");
                    return -1;
                }
            }

            if (AppData.ClassCvnet.config.MakerOnly == 0)
            {
                var v_skbn = "";
                var v_sqlstr12 = "";
                for (var i = 0; i < SelectedOrderDetail.Count; i++)
                {
                    if (i != 0) v_sqlstr12 += ",";
                    v_sqlstr12 += "'" + SelectedOrderDetail[i].ProductCD + "'";
                }
                var v_sqlstr22 = "select 商品CD,\"メーカーCD\" from HC$master_shohin where 商品CD in (" + v_sqlstr12 + ")";
                var wrk_csv12 = AppData.Http!.AspxSqlQuery(v_sqlstr22);
                if (wrk_csv12.Rows.Count > 0)
                {
                    for (var i = 0; i < wrk_csv12.Rows.Count; i++)
                    {
                       if (wrk_csv12.Rows[i][1] != SelectedOrderHeader.Supplier.Code)
                       {
                           ClientLib.MessageBoxError(this, "仕入先の商品CD以外が存在します！");
                           return -1;
                       }
                    }
                }
            }

            var v_sqlstr1 = "";
            for (var i = 0; i < SelectedOrderDetail.Count; i++)
            {
                if (i != 0) v_sqlstr1 += ",";
                v_sqlstr1 += "'" + SelectedOrderDetail[i].ProductCD + "'";
            }
            var v_sqlstr = "select 商品CD,仕入区分 from HC$master_shohin where 商品CD in (" + v_sqlstr1 + ")";
            var wrk_csv = AppData.Http!.AspxSqlQuery(v_sqlstr);

            var v_skbn1 = "";
            var v_skbn2 = "";
            var v_skbn3 = "";
            for (var i = 0; i < wrk_csv.Rows.Count; i++)
            {
                if (wrk_csv.Rows[i][1] == "1") v_skbn1 = "1";
                if (wrk_csv.Rows[i][1] == "2") v_skbn2 = "1";
                if (wrk_csv.Rows[i][1] == "3") v_skbn3 = "1";
            }
            if (v_skbn3 == "1" && (v_skbn1 == "1" || v_skbn2 == "1"))
            {
                ClientLib.MessageBoxError(this, "仕入条件”消化”とそれ以外の混在は登録できません！");
                return -1;
            }
            for (var l = 0; l < SelectedOrderDetail.Count; l++) {
                var v_key = SelectedOrderDetail[l].ProductCD + SelectedOrderDetail[l].GenkaFlg + SelectedOrderDetail[l].Color + SelectedOrderDetail[l].Size;
                for(var k = 0; k < SelectedOrderDetail.Count; k++)
                {
                    var v_key1 = SelectedOrderDetail[l].ProductCD + SelectedOrderDetail[l].GenkaFlg + SelectedOrderDetail[l].Color + SelectedOrderDetail[l].Size;
                    if (k != l && v_key == v_key1)
                    {
                        ClientLib.MessageBoxError(this, "同一SKUをは１行にまとめてください");
                        return -1;
                    }
                }
            }       
            return 0;
        }
        #endregion
        #region SecondTab
        [RelayCommand]
        public void ProductExpand() 
        {
            if (SelectedOrderHeader.Supplier.Code == null || SelectedOrderHeader.Supplier.Code == "") 
            { ClientLib.MessageBoxError(this, "先に仕入先を入力してください"); 
                return; 
            }
            if (Product.Code == null || Product.Code == "") 
            {
                ClientLib.MessageBoxError(this, "先に商品CDを入力してください");
                return;
            }
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
            }
            string[] para = new string[1];
            para[0] = Product.Code;
            //var vm = new SubDlgSKU01ViewModel(para);
            //var window = new SubDlgSKU01View { DataContext = vm };
            //window.ShowDialog();
        }
        [RelayCommand]
        public void InpBarcode() 
        {
            if (SelectedOrderHeader.Ware.Code == "")
            {
                ClientLib.MessageBoxError(this, "先に入庫先を入力してください");
                return;
            }
            /* マスタチェック */
            if (OnCheckMst() < 0) return;

            var wrk_para = new string[6];
            wrk_para[0] = v_denkbn.ToString();
            wrk_para[1] = SelectedOrderHeader.Ware.Code;
            wrk_para[2] = SelectedOrderHeader.Hachubi?.ToString("yyyyMMdd");
            wrk_para[3] = "0";
            wrk_para[4] = "0";
            wrk_para[5] = SelectedOrderHeader.ToriKubun.ToString();
            //var vm = new SubDlgBcd01ViewModel(wrk_para);
            //var window = new SubDlgBcd01View { DataContext = vm };
            //window.ShowDialog();
            
        }
        [RelayCommand]
        public void ProductSearch() 
        {
            var vm = new SubDlgSyoKenSakuViewModel();
            var window = new SubDlgSyoKenSakuView { DataContext = vm };
            window.ShowDialog();
            var result = vm.Result;

            if (result != null)
            {
                for (int i = 0; i < SelectedOrderDetail.Count; i++)
                {
                    var item = SelectedOrderDetail[i];

                    item.RowColor = "Transparent";
                    
                    if (item.ProductCD == result[0])
                    {
                        var brush = (SolidColorBrush)Application.Current.Resources["SearchColor1"];
                        item.RowColor = brush.Color.ToString();
                    }                       
                    else if (item.ProductCD == result[1])
                    {
                        var brush = (SolidColorBrush)Application.Current.Resources["SearchColor2"];
                        item.RowColor = brush.Color.ToString();
                    }
                    else if (item.ProductCD == result[2])
                    {
                        var brush = (SolidColorBrush)Application.Current.Resources["SearchColor3"];
                        item.RowColor = brush.Color.ToString();
                    }
                    else if (item.ProductCD == result[3])
                    {
                        var brush = (SolidColorBrush)Application.Current.Resources["SearchColor4"];
                        item.RowColor = brush.Color.ToString();
                    }
                    else if (item.ProductCD == result[4])
                    {
                        var brush = (SolidColorBrush)Application.Current.Resources["SearchColor5"];
                        item.RowColor = brush.Color.ToString();
                    }
                }
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
        private void DoInsert()
        {
            if (OnCheckError() < 0) return;
            if (OnCheckMst() < 0) return;
            var sir_day = SelectedOrderHeader.Hachubi;
            if (AppData.ClassCvnet.CheckImpDate(sir_day) < 0)
            {
                ClientLib.MessageBoxError(this,"登録可能な日付ではありません。");
                return;
            }
            
            var col01 = new BizArray();
            var col02 = new BizArray();
            col01[0] = "SESS_ID";
            col01[1] = "MOD_SEQ";
            col02[0] = AppData.ClassSatoo.AspxGetSESS_ID().ToString();
            col02[1] = "-1";
            var ret_val = AppData.Http!.AspxSqlExe(DBDef.DB_DML.INSERT, "WORK_TORIWRK0", 0, "0", col01.ToArray(), col02.ToArray());
            if (ret_val.Code < 0)
            {
                ClientLib.MessageBoxError(this, "NO取得エラー!");
                return;
            }
            var v_start = DateTime.Now;
            var new_seq = ret_val.NewSeq;
            var v_para = new string[2];
            v_para[0] = "WORK_TORIWRK1";
            //var ret_val1 = AppData.ClassCvnet.AspxParaGetDetail(^.Spread1, col_list2, new_seq, v_denkbn, SelectedOrderHeader.Hachubi?.ToString("yyyyMMdd"), err_wrk);
            var ret_csv2 = AppData.Http!.AspxSqlQuery2("mi", v_para);
            if (int.Parse(ret_csv2.Split(",")[0].ToString()) < 0)
            {
                ClientLib.MessageBoxError(this,"明細登録エラー!");
                return;
            }
            //AppData.ClassCvnet.AspxParaGet(^.^.TabForm2, col_list1.Length + 3, col02);
            col02[0] = "";
            col02[1] = SelectedOrderHeader.Hachubi?.ToString("yyyyMMdd");
            col02[2] = SelectedOrderHeader.Nohinbi?.ToString("yyyyMMdd");
            col02[3] = SelectedOrderHeader.ToriKubun.ToString();
            col02[4] = SelectedOrderHeader.User.Code;
            col02[5] = SelectedOrderHeader.Ware.Code ;
            col02[6] = SelectedOrderHeader.Supplier.Code;
            col02[7] = "";
            col02[8] = "";
            col02[9] = Sum.ToString();
            col02[10] = "";
            col02[11] = "";
            col02[12] = "";
            col02[13] = TotalJodai.ToString();
            col02[14] = TotalGedai.ToString();
            col02[15] = SelectedOrderHeader.Biko;
            col02[16] = "1";
            col02[17] = "13";
            col02[18] = "";
            col02[19] = "";
            col02[20] = SelectedOrderHeader.Kanren1;            
            if (AppData.ClassCvnet.config.smtflg != 1) col02[21] = "19010101";
            col02[22] = "0";
            col02[23] = SelectedOrderHeader.Kanren2;
            ret_val = AppData.Http!.AspxSqlExe(DBDef.DB_DML.INSERT, "TRAN_TORI0", 0, "0", col_list1, col02.ToArray());

            if (ret_val.Code == 0)
            {
                ClientLib.MessageBoxOk(this, "データ追加しました");
            }
            else if (ret_val.Code == -1)
            {
                ClientLib.MessageBoxError(this,"ロックエラーです");
            }
            else if (ret_val.Code == -2)
            {
                ClientLib.MessageBoxError(this, "他で更新されていますので、登録されていません");
            }
            else
            {
                ClientLib.MessageBoxError(this, "データ追加できませんでした");
            }
        }
        [RelayCommand]
        private void DoUpdate()
        {
            if (OnCheckError() < 0) return;
            if (OnCheckMst() < 0) return;

            /* 法人連携 10.10.05 */
            if (AppData.ClassCvnet.config.MultiCoop != 0)
            {
                if (OnCheckErrorhj() < 0) return;
            }

            if (SelectedOrderHeader.DenpyoNo == "") return;
            if (ListOrder == null) return;
            var sir_day = SelectedOrderHeaderRef.Hachubi;
            if (AppData.ClassCvnet.CheckImpDate(sir_day) < 0)
            {
                ClientLib.MessageBoxError(this, "修正可能な伝票ではありません。");
                return;
            }
            sir_day = SelectedOrderHeader.Hachubi;
            if (AppData.ClassCvnet.CheckImpDate(sir_day) < 0)
            {
                ClientLib.MessageBoxError(this, "修正可能な日付ではありません。");
                return;
            }
            
            var now_mod_seq = SelectedOrderHeader.DenpyoNo;
            var now_mod_vdate = SelectedOrderHeader.UpdateDate;
            var col01 = new BizArray();
            var col02 = new BizArray();
            col01[0] = "MOD_SEQ";
            col02[0] = "10";
            var ret_val = AppData.Http!.AspxSqlExe(DBDef.DB_DML.LOCK, "TRAN_TORI0", long.Parse(now_mod_seq), now_mod_vdate.ToString(), col01.ToArray(), col02.ToArray());
            if (ret_val.Code == -1)
            {
                ClientLib.MessageBoxError(this, "ロックエラーです");
                return;
            }
            else if (ret_val.Code == -2)
            {
                ClientLib.MessageBoxError(this, "他で更新されていますので、登録されていません");
                return;
            }
            else if (ret_val.Code < 0)
            {
                ClientLib.MessageBoxError(this, "ロックエラー !");
                return;
            }
            var new_seq = ret_val.NewSeq;
            var v_start = DateTime.Now;

            var v_para = new string[2];
            v_para[0] = "WORK_TORIWRK1";
            //var ret_val1 = AppData.ClassCvnet.AspxParaGetDetail(^.Spread1, col_list2, new_seq, v_denkbn, SelectedOrderHeader.Hachubi?.ToString("yyyyMMdd"), err_wrk);
            var ret_csv2 = AppData.Http!.AspxSqlQuery2("mi", v_para);

            if (int.Parse(ret_csv2.Split(",")[0].ToString()) < 0)
            {
                AppData.Http!.AspxSqlExe(DBDef.DB_DML.UNLOCK, "TRAN_TORI0", long.Parse(now_mod_seq), now_mod_vdate.ToString(), null, null);
                ClientLib.MessageBoxError(this, "明細登録エラー ! 赤い項目の[一覧]から選択して下さい｡");
                return;
            }

            //cvnet.AspxParaGet(^.^.TabForm2, SubDialog.col_list.length + 3, col02);

            col02[0] = SelectedOrderHeader.DenpyoNo;
            col02[1] = SelectedOrderHeader.Hachubi?.ToString("yyyyMMdd");
            col02[2] = SelectedOrderHeader.Nohinbi?.ToString("yyyyMMdd");
            col02[3] = SelectedOrderHeader.ToriKubun.ToString();
            col02[4] = SelectedOrderHeader.User.Code;
            col02[5] = SelectedOrderHeader.Ware.Code;
            col02[6] = SelectedOrderHeader.Supplier.Code;
            col02[7] = "";
            col02[8] = "";
            col02[9] = Sum.ToString();
            col02[10] = "";
            col02[11] = "";
            col02[12] = "";
            col02[13] = TotalJodai.ToString();
            col02[14] = TotalGedai.ToString();
            col02[15] = SelectedOrderHeader.Biko;
            col02[16] = "1";
            col02[17] = "13";
            col02[18] = "";
            col02[19] = "";
            col02[20] = SelectedOrderHeader.Kanren1;
            if (AppData.ClassCvnet.config.smtflg != 1) col02[21] = "19010101";
            col02[22] = "0";
            col02[23] = SelectedOrderHeader.Kanren2;

            if (AppData.ClassCvnet.config.smtflg != 1) col02[21] = "19010101";
            ret_val = AppData.Http!.AspxSqlExe(DBDef.DB_DML.UPDATE, "TRAN_TORI0", long.Parse(now_mod_seq), now_mod_vdate.ToString(), col_list1, col02.ToArray());
            if (ret_val.Code == 0)
            {                              
                TimeSpan elapsed = DateTime.Now - v_start;
                ClientLib.MessageBox(this,"データ修正しました (" + elapsed.ToString() + ")");
            }
            else if (ret_val.Code == -1)
            {
                ClientLib.MessageBoxError(this, "ロックエラーです");
            }
            else if (ret_val.Code == -2)
            {
                ClientLib.MessageBoxError(this, "他で更新されていますので、登録されていません");
            }
            else
            {
                AppData.Http!.AspxSqlExe(DBDef.DB_DML.UNLOCK, "TRAN_TORI0", long.Parse(now_mod_seq), now_mod_vdate.ToString(), null, null);
                ClientLib.MessageBoxError(this, "データ修正できませんでした");
            }
        }
        
        #endregion        
        #region BtListFunction
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
        #endregion
        #region Print&DocumentDownload
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
        [RelayCommand]
        async Task DoCSVAsync() 
        {
            if (!ClientLib.MessageBox(this, "CSVファイルを作成しますか？")) return;
            CSV_flg = 1;
            CreatePara();

            var f_name = SelectedOutPut.ToString();
            var ret_csv = "";
            if (SelectedOutPut == OutPutType.List)
            {

                ret_csv = OnQueryPrint(param1, param2);
            }
            else
            {
                ret_csv = OnQueryDetailPrint(param1, "1", param2);
            }
            var lines = ret_csv.Split('\n');

            if (lines.Length < 2 || lines[1] == "0")
            {
                ClientLib.MessageBoxError(this, "PDFデータがありません");
                return;
            }

            string pdfPath = lines[0];
            string url = AppData.Http!.URLroot + pdfPath + "/data.pdf";

            var ret_name = AppData.Http!.AspxSqlQuery("select m.名称 from HC$MASTER_MEISHO m where m.名称区分='IDX' and m.名称CD between 'B01' and 'B10' order by m.名称CD");
            var name_cnt = 0;
            var name_flg = 0;

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

            var header_csv = new BizCsvDocument();
            await header_csv.LoadHeaderFromUrl(headpath);
            var get_csv = new BizCsvDocument();
            await get_csv.LoadFromUrlAsync(datapath, headpath);
            var dt = get_csv.GetTable();
            var dt_header = header_csv.GetTable();
            dt.Rows.InsertAt(dt.NewRow(),0);

            for (var i = 0; i < dt.Columns.Count; i++)
            {
                /* 2021.04.27 商品名称CDのラベル付け */
                if (SelectedOutPut == OutPutType.Detail && dt_header.Rows[0][i].ToString().Substring(0, 4) == "名称CD" && ret_name.Rows.Count == 10)
                {
                    if (name_flg == 0)
                    {
                        dt.Rows[0][i] = ret_name.Rows[name_cnt][0].ToString() + "CD";
                        name_flg = 1;
                    }
                    else
                    {
                        dt.Rows[0][i] = ret_name.Rows[name_cnt][0].ToString() + "名";
                        name_cnt++;
                        name_flg = 0;
                    }
                }
                else
                {
                    dt.Rows[0][i] = dt_header.Rows[0][i];
                }
            }            
            get_csv = new BizCsvDocument(dt);
            var str = get_csv.SaveStr(1);
            get_csv = new BizCsvDocument(str,1);

            if (AppData.ClassCvnet.config.ExcelOutFlg == 1)
            {

                var dtCSVFlNm = "";
                var ExcelFlNm = "";

                try
                {

                    if (f_name == "一覧")
                    {
                        dtCSVFlNm = "DataHatchuIchiran.csv";
                        ExcelFlNm = "HatchuIchiran_macro.xlsm";
                    }
                    else if (f_name == "明細")
                    {
                        dtCSVFlNm = "DataHatchuMeisai.csv";
                        ExcelFlNm = "HatchuMeisai_macro.xlsm";
                    }

                    get_csv.SaveCsv(dtCSVFlNm);
                    
                }
                catch (Exception ex)
                {
                    ClientLib.MessageBoxError(this, "保存を中止しました");
                }

            }
            else
            {

                var v_title = "発注伝票";
                if (MenuFlg == "1") v_title = "入荷予定伝票";
                try
                {
                    get_csv.SaveCsv(v_title + f_name);
                    
                }
                catch (Exception ex)
                {
                    ClientLib.MessageBoxError(this, "保存を中止しました");
                }

            }
        }
        [RelayCommand]
        async Task DoPrint1Async()
        {
            if (SelectedOrderDetail == null || SelectedOrderHeader == null) return;

            CSV_flg = 0;

            var wrk_para = new string[2];
            wrk_para[0] = SelectedOrderHeader.DenpyoNo;        /* 伝票NO */
            wrk_para[1] = SelectedOrderHeader.ToriKubun.ToString();      /* 取引区分 */

            var ret_csv = OnQueryDetailPrint(wrk_para);       /*返されたパラメータをret_csvに代入*/
            var lines = ret_csv.Split('\n');

            if (lines.Length < 2 || lines[1] == "0")
            {
                ClientLib.MessageBoxError(this, "PDFデータがありません");
                return;
            }

            string pdfPath = lines[0];
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
            sql_query += "'" + v_title + "伝票一覧'";
            for (int i = 0; i < col_list1.Length; i++)
            {
                sql_query += ",A." + col_list1[i];
            }
            sql_query += " ,B.名前 担当名, C.仕入先名 仕入先名, D.得意先名 得意先名, C.消費税CD, C.消費税計算方法, C.消費税端数 ";
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
        public string OnQueryDetailPrint(string[] wrk_para1, string? hdFlg = "0", string[]? v_sho = null) 
        {
            string[] wrk_para;
            if (FlgSho == 1 && v_sho != null)
            {
                wrk_para = new string[20];
            }
            else {
                wrk_para = new string[18];
            }
            if (hdFlg != "1") 
            {
                wrk_para = new string[2];
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

            for (int i = 0; i < col_list1.Length; i++)
            {
                sql_query += ",A." + col_list1[i];
            }

            sql_query += ",B.名前 担当名, C.仕入先名 仕入先名, D.得意先名 得意先名, C.消費税CD, C.消費税計算方法, C.消費税端数 ";
            sql_query += ",A.SYSFLG, A.送信FLG";
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
                if (FlgSho == 1 && v_sho != null)
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
        #region Class
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
            private decimal? createDate;
            [ObservableProperty]
            private decimal? updateDate;
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
        #endregion
    }
}