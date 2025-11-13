using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Class;
using CvnetClient.Models;
using CvnetClient.Utils;
using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using static MaterialDesignThemes.Wpf.Theme;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace CvnetClient.ViewModels
{
    public partial class SubDlgInp13ViewModel : BaseViewModel
    {
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
        ObservableCollection<OrderList>? listOrder;
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
        private BizArray para;
        private BizArray v_flg;
        private string shoriKaishibi;
        private string tori_kbn = "商品発注区分";
        private int v_denkbn = 13;
        [ObservableProperty]
        public string? dateName;
        public bool renkeishow;
        public bool sokoshow;
        public string csvname;

        public void OnInit(object? init_para = null,object? init_flg = null) 
        {
            if (init_para != null)
            {
                if (init_para is string s)
                {
                    var v_para = new string[] { s };
                    para = new BizArray(v_para);
                }
                else if (init_para is string[] arr)
                    para = new BizArray(arr);
                else para = new BizArray();
            }
            else para = new BizArray();

            if (init_flg != null)
            {
                if (init_flg is string s)
                {
                    var v_para = new string[] { s };
                    v_flg = new BizArray(v_para);
                }
                else if (init_flg is string[] arr)
                    v_flg = new BizArray(arr);
                else v_flg = new BizArray();
            }
            else v_flg = new BizArray();

            FlgSho = 0;
            EditSearch = new SearchModel();
            EditSearch.DenpyoNo1 = "0";
            EditSearch.DenpyoNo2 = "9999999999";
            EditSearch.NohinbiFrom = DateTime.Now;
            EditSearch.NohinbiTo = new DateTime(2099/12/31);            
            EditSearch.Kanren1From = "0";
            EditSearch.Kanren1To = "9999999999999";
            EditSearch.Kanren2From = "0";
            EditSearch.Kanren2To = "9999999999999";
            EditSearch.SupplierTo = new BtListHelper("99999999","");
            EditSearch.WareTo = new BtListHelper("99999999", "");
            EditSearch.ProductTo = new BtListHelper("zzzzzzzzzzzzzzzzzzzz", "");
            EditSearch.UserTo = new BtListHelper("99999999", "");
            DateName = "納品日";
            csvname = "CSV出力";
            renkeishow = false;
            //ComboListToriHikiFrom = new Dictionary<string, string>
            //{
            //    {  "00", "00" },
            //    {  "10", "10 発注" },
            //    {  "11", "11 追加発注" },
            //    {  "15", "15 自動発注" }
            //};
            //EditSearch.ToriKubunFrom = ComboListToriHikiFrom.FirstOrDefault().Key;
            
            //ComboListToriHikiTo = new Dictionary<string, string>
            //{
            //    {  "99", "99" },
            //    {  "10", "10 発注" },
            //    {  "11", "11 追加発注" },
            //    {  "15", "15 自動発注" }
            //};
            //EditSearch.ToriKubunTo = ComboListToriHikiTo.FirstOrDefault().Key;
            SelectedOrderHeader = new OrderHeader();
            
            if (v_flg[0] != null) MenuFlg = v_flg[0];
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
          
            SelectedOrderHeader.Hachubi = DateTime.Now.ToString("yyyy/MM/dd");
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
                renkeishow = false;

                //Form1.TabFrame1.TabForm2.Label9.Visible =$false;
                //Form1.TabFrame1.TabForm2.Text25.Active =$false;
                //Form1.TabFrame1.TabForm2.Text25.Visible =$false;
            }
            GetShoriKaishibi();
            if (AppData.ClassCvnet.config.ExcelOutFlg == 1)
            {
                csvname = "EXCEL出力";
            }
        }

        [RelayCommand]
        public void ClickProd(string value) 
        {
            if (FlgSho == 0) 
            { 
                FlgSho = 1;
            }
            else
            {
                FlgSho = 0;
            }
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
            var param1 = new string[18];
            var param2 = new string[2];
            param1[0] = EditSearch.DenpyoNo1;
            param1[1] = EditSearch.DenpyoNo2;

            var date_Str = EditSearch.NohinbiFrom?.ToString("yyyyMMdd");
            if (EditSearch.NohinbiFrom < DateTime.Parse(shoriKaishibi.Substring(0,4) + "/" + shoriKaishibi.Substring(4, 2) + "/" + shoriKaishibi.Substring(6, 2))) date_Str = shoriKaishibi;
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
            OnQuery(param1, "all", param2);
            
        }
        string sql_collist  = "A.手入力伝票NO,A.在庫計上日,A.納品日,A.取引区分,A.入力社員CD,A.取引先CD2,"+
	                        "A.取引先CD1,A.掛率1,A.外税対象金額,A.数量合計,A.明細金額合計,"+
	                        "A.内税消費税,A.外税消費税,A.上代合計,A.下代合計,A.メモ,A.掛計上FLG,A.伝票処理区分,A.MOD_SEQ,A.倉庫CD,A.関連伝票NO"+
	                        ",A.掛計上日"+
	                        ",A.SYSFLG2"+
	                        ",A.関連伝票NO2";


        string sql_collist1  = "明細取引区分,商品CD,色CD,サイズCD,明細名称,数量,単価,金額,内税消費税,外税消費税,"+
	                            "上代単価,上代金額,下代単価,下代金額,明細メモ,消費税計算方法"+
	                            ",商品シリアル,関連伝票NO,関連伝票行NO,JANCODE,原価FLG,完了FLG";
        void OnQuery(string[] param, string flg, string[] param2) 
        {
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
                sql_query += " and A.SEQ_NO <= '" + flg + "'";
            }
            if (FlgSho == 1)
            {
                parameter[18] = param2[0];
                parameter[19] = param2[1];
                sql_query = "SELECT A.* FROM (" + sql_query
                    + ") A WHERE EXISTS (SELECT /*+ INDEX(E 	HC$_NK_TORI23) */ 'X' FROM HC$TRAN_TORI1 E WHERE A.SEQ_NO=E.ヘッダNO AND E.商品CD BETWEEN :" + (parameter.Length - 1).ToString() + " AND :" + (parameter.Length).ToString() + ")";
            }
            sql_query += " ORDER BY A.SEQ_NO DESC";

            if (flg != "all")
            {
                sql_query = "SELECT * FROM (" + sql_query + ") where rownum <= " + AppData.maxQueryCnt;
            }

            var ret_csv = AppData.Http!.AspxSqlQuery(sql_query, parameter);
            if (ret_csv == null || ret_csv.Rows.Count == 0) return;
            var list = (from DataRow dr in ret_csv.Rows
                        select new OrderList
                        {
                            DenpyoNo = dr["SEQ_NO"].ToString() ?? string.Empty,
                            Nohinbi = DateTime.ParseExact(dr["納品日"]?.ToString(), "yyyyMMdd", null),
                            Supplier = dr["取引先CD1"].ToString() ?? string.Empty,
                            SupplierName = dr["仕入先名"].ToString() ?? string.Empty,
                            Ware = dr["取引先CD2"].ToString() ?? string.Empty,
                            WareName = dr["得意先名"].ToString() ?? string.Empty,
                            Torihiki = dr["取引区分"].ToString() ?? string.Empty,
                            WeightSum = dr["数量合計"].ToString() ?? string.Empty,
                            PriceSum = dr["明細金額合計"].ToString() ?? string.Empty

                        }).OrderBy(c => c.DenpyoNo).ToList();
            Common.ConvertDotStringDel(list);
        }
        public partial class SearchModel :ObservableObject 
        {
            [ObservableProperty]
            public string? denpyoNo1;
            [ObservableProperty]
            public string? denpyoNo2;
            [ObservableProperty]
            public DateTime? nohinbiFrom;
            [ObservableProperty]
            public DateTime? nohinbiTo;
            [ObservableProperty]
            public int? toriKubunFrom;
            [ObservableProperty]
            public int? toriKubunTo;
            [ObservableProperty]
            public string? kanren1From;
            [ObservableProperty]
            public string? kanren1To;
            [ObservableProperty]
            public string? kanren2From;
            [ObservableProperty]
            public string? kanren2To;
            [ObservableProperty]
            public string? tenyuryoku1;
            [ObservableProperty]
            public string? tenyuryoku2;
            [ObservableProperty]
            public BtListHelper? supplierFrom;
            [ObservableProperty]
            public BtListHelper? supplierTo;
            [ObservableProperty]
            public BtListHelper? wareFrom;
            [ObservableProperty]
            public BtListHelper? wareTo;
            [ObservableProperty]
            public BtListHelper? productFrom;
            [ObservableProperty]
            public BtListHelper? productTo;
            [ObservableProperty]
            public BtListHelper? userFrom;
            [ObservableProperty]
            public BtListHelper? userTo;
            [ObservableProperty]
            private int? renkei;
        }

        public partial class OrderList : ObservableObject 
        {
            [ObservableProperty]
            public string? denpyoNo;
            [ObservableProperty]
            public DateTime? nohinbi;
            [ObservableProperty]
            public string? supplier;
            [ObservableProperty]
            public string? supplierName;
            [ObservableProperty]
            public string? ware;
            [ObservableProperty]
            public string? wareName;
            [ObservableProperty]
            public string? torihiki;
            [ObservableProperty]
            public string? weightSum;
            [ObservableProperty]
            public string? priceSum;
            [ObservableProperty]
            public string? kanren;
            [ObservableProperty]
            private long? seq_no;
        }

        public partial class OrderHeader : ObservableObject
        {
            [ObservableProperty]
            public string? denpyoNo;
            [ObservableProperty]
            public string? hachubi;
            [ObservableProperty]
            public DateTime? nohinbi;
            [ObservableProperty]
            public int? toriKubun;
            [ObservableProperty]
            public string? kanren1;
            [ObservableProperty]
            public string? kanren2;
            [ObservableProperty]
            public string? tenyuryoku;
            [ObservableProperty]
            public BtListHelper? supplier;
            [ObservableProperty]
            public BtListHelper? ware;
            [ObservableProperty]
            public BtListHelper? user;
            [ObservableProperty]
            public string? biko;
            [ObservableProperty]
            public string? createDate;
            [ObservableProperty]
            public string? updateDate;
        }

        public partial class OrderDetail : ObservableObject
        {
            [ObservableProperty]
            public string? productCD;
            [ObservableProperty]
            public string? productName;
            [ObservableProperty]
            public string? color;
            [ObservableProperty]
            public string? colorName;
            [ObservableProperty]
            public string? size;
            [ObservableProperty]
            public string? sizeName;
            [ObservableProperty]
            public string? weight;
            [ObservableProperty]
            public string? jodaiTanka;
            [ObservableProperty]
            public string? jodaiKingaku;
            [ObservableProperty]
            public string? gedaiTanka;
            [ObservableProperty]
            public string? gedaiKingaku;
            [ObservableProperty]
            public string? abstracts;
            [ObservableProperty]
            public string? kanryo;
            [ObservableProperty]
            public string? maker;
            [ObservableProperty]
            public string? kubun;
        }
    }
}