using CommunityToolkit.Mvvm.ComponentModel;
using CvnetBaseCore;
using CvnetClient.Models;
using CvnetClient.Utils;
using System.Data;
using System.Windows;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg01ShoInp1ViewModel : BaseViewModel
    {
        #region Binding Variables
        [ObservableProperty]
        string m_DgTitle = "商品マスタ : 商品発注入力";

        [ObservableProperty]
        ShoInp1SearchOpt m_ShoInp1SearchOpt;
        #endregion

        #region Class Variables
        private DataTable csv_col;
        private DataTable csv_siz;
        private int kake = 0;
        private int ColSizFlg = 0;
        /* 原価 */
        private int genka = 0;
        /* 消費税計算方法 */
        private int shohi = 1;
        private DataTable csv_jodai;
        private DataTable csv_genka;
        private DataTable pre_csv_genka;
        private double vd_create = 0;
        private string syori_no;
        string den_init = "";

        private BizArray col_list; /* 伝票 Insert,Update時の項目名リスト */
        /***** 発注・仕入のときとで、項目が少し違ったので、分けました
	    *****　sql_collist_13　納品日-----sql_collist_3　掛計上日 *****/
        /* 08.10.06 掛計上日の追加 */
        string sql_collist_13 = "在庫計上日,納品日,取引区分,入力社員CD,倉庫CD,取引先CD1"
                              + ",掛率1,掛計上FLG,関連伝票NO,数量合計,上代合計,下代合計,在庫計上FLG,メモ"
	                          + ",伝票処理区分,取引先CD2,MOD_SEQ,外税対象金額,明細金額合計,掛計上日,消費税率";
        string sql_collist_3 = "在庫計上日,掛計上日,取引区分,入力社員CD,倉庫CD,取引先CD1"
                             + ",掛率1,掛計上FLG,関連伝票NO,数量合計,上代合計,下代合計,在庫計上FLG,メモ"
                             + ",伝票処理区分,取引先CD2,MOD_SEQ,外税対象金額,明細金額合計,納品日,消費税率";

        private BizArray col_list2; /* 伝票 明細の項目名リスト */
        string sql_collist2 = "伝票処理区分,在庫計上日,明細取引区分,明細名称,単価,金額,"
                            + "上代金額,下代金額,原価FLG"; 
        public int resp_code;
        #endregion

        #region ComboList Variable
        /// <summary>
        /// 取引区分 List
        /// </summary>
        [ObservableProperty]
        public Dictionary<string, string>? m_TranCatList;

        /// <summary>
        /// 掛計上 List
        /// </summary>
        [ObservableProperty]
        public Dictionary<int, string>? m_AccrualList;

        /// <summary>
        /// 在庫計上 List
        /// </summary>
        [ObservableProperty]
        public Dictionary<int, string>? m_RecInvList;

        /// <summary>
        /// 消費税率 List
        /// </summary>
        [ObservableProperty]
        public Dictionary<int, string>? m_TaxList;
        #endregion

        public int OnInit(string[] init_para, string flg)
        {
            resp_code = 0;
            ShoInp1SearchOpt = new ShoInp1SearchOpt();
            /***** init_para *****
		    *****   [0]：商品CD　[1]：伝票処理区分　[2]：V_SEQ　[3]：V_UPDATE　[4]：伝票NO　[5]：発注？契約発注？*/
            OnInitBase(init_para, flg);
            if (init_para.Length < 6) {
                resp_code = -1;
                ClientLib.ExitDialogResult(this, true);
                return resp_code;
            }
            long p_no = long.TryParse(init_para[2], out long _no) ? _no : 0;
            var ret_aspx = AppData.Http!.AspxSqlExe(DBDef.DB_DML.LOCK, "Master_SHOHIN", p_no, init_para[3], null, null);
            if (ret_aspx.Code < 0)
            {
                resp_code = ret_aspx.Code;
                ClientLib.ExitDialogResult(this, true);
                return resp_code;
            }
            ColSizFlg = AppData.ClassCvnet.config.ColSizFlg;
            ShoInp1SearchOpt.DgCol1Header = (ColSizFlg == 1) ? "色/ｻｲｽﾞ" : "ｻｲｽﾞ/色";
            ShoInp1SearchOpt.Dsp7Label = (ColSizFlg == 1) ? "色" : "ｻｲｽﾞ";
            ShoInp1SearchOpt.Dsp8Label = (ColSizFlg == 1) ? "ｻｲｽﾞﾞ" : "色";

            syori_no = init_para[1];  /* 伝票処理区分を保存　発注：１３　仕入：３　契約発注入力：３のため */
            den_init = "商品発注区分"; /* 伝票処理 */
            var wrk_list = sql_collist_13.Split(",");
            col_list = new BizArray(wrk_list);

            if (init_para[1] == "3")
            {
                ShoInp1SearchOpt.OrderDateTitle = "仕入日";
                ShoInp1SearchOpt.DeliverDateTitle = "掛計上日";
                ShoInp1SearchOpt.RecDestTitle = "倉庫";
                ShoInp1SearchOpt.AccrualTVis = Visibility.Visible; /* 掛計上？ */
                ShoInp1SearchOpt.AccrualDVis = Visibility.Visible;
                ShoInp1SearchOpt.RecInvTVis = Visibility.Visible;
                ShoInp1SearchOpt.RecInvDVis = Visibility.Visible;
                den_init = "商品仕入区分";
                DgTitle = "商品仕入入力";
                col_list = new BizArray(sql_collist_3.Split(","));
            }

            #region Set ComboList
            var comboItem = AppData.ClassCvnet.comboItem00;

            // 取引区分
            var get_tran = comboItem.ComboItem_00<string>(den_init);
            TranCatList = get_tran;
            ShoInp1SearchOpt.TranCate = "10";

            // 掛計上
            var get_accrual = comboItem.ComboItem_00<int>("する");
            AccrualList = get_accrual;
            ShoInp1SearchOpt.AccrualCd = get_accrual.FirstOrDefault().Key;

            // 在庫計上
            var get_rec = comboItem.ComboItem_00<int>("する");
            RecInvList = get_rec;
            ShoInp1SearchOpt.RecInvCd = get_rec.FirstOrDefault().Key;
            #endregion

            ShoInp1SearchOpt.OrderDate = DateTime.Now; /* 初期化処理 */
            ShoInp1SearchOpt.AccrualCd = 1;
            ShoInp1SearchOpt.DeliverDate = DateTime.Now;
            ShoInp1SearchOpt.InputStaffCd = AppData.ClassSatoo.SHAIN_CD;
            ShoInp1SearchOpt.InputStaffName = AppData.ClassSatoo.SHAIN_Name;
            var tmp_csv = AppData.ClassCvnet.AspxSqlQueryImp();

            if (tmp_csv != null)
                ShoInp1SearchOpt.RecDestCd = new BtListHelper(tmp_csv.Rows[0][1].ToString(), tmp_csv.Rows[0][2].ToString());

            var v_para = new BizArray();
            v_para[0] = init_para[0];
            string sql_str = "select j.色CD,j.サイズCD,get_jodai(j.商品CD,j.色CD,j.サイズCD) 上代";
            /* 原価対応 */
            sql_str += ", CASE WHEN (J.仕入価格 IS NULL) OR (J.仕入価格=0) THEN S.仕入価格 ELSE J.仕入価格 END AS 仕入価格";
            /* 原価対応END*/
            sql_str += ",NVL(s.商品名,'') 商品名";
            sql_str += ",NVL(s.消費税計算方法,'') 消費税計算方法";
            sql_str += ",NVL(s.メーカーCD||' '||NVL((select m.名称 from HC$Master_MEISHO m where m.名称区分='MKR' and m.名称CD=s.メーカーCD),''),'') ﾒｰｶｰ名";
            sql_str += ",NVL(s.上代,0) 商品上代";
            sql_str += ",仕入区分";
            sql_str += " from HC$master_shohin_jan j,HC$master_shohin s where j.商品CD=s.商品CD(+) and j.商品CD=:1 order by j.色CD,j.サイズCD";

            var ret_csv = AppData.Http?.AspxSqlQuery(sql_str, v_para.ToArray());
            if (ret_csv != null)
                kake = int.TryParse(ret_csv?.Rows[0][6].ToString(), out int _kake) ? _kake : 0;

            if (ret_csv.Rows.Count == 0)
            {
                MessageBox.Show("商品マスタ、もしくは商品色サイズマスタがありませんでした", "注意", MessageBoxButton.OK);
                resp_code = -1;
                ClientLib.ExitDialogResult(this, true);
                return resp_code;
            }

            ShoInp1SearchOpt.ProdCd = init_para[0];
            ShoInp1SearchOpt.ProdName = ret_csv.Rows[0][4].ToString();
            ShoInp1SearchOpt.SupplierCd = new BtListHelper(ret_csv.Rows[0][6].ToString(),""); /* ﾒｰｶｰ */
            ShoInp1SearchOpt.Retail = int.TryParse(ret_csv.Rows[0][7].ToString(), out int _retail) ? _retail : 0; /* 商品ﾏｽﾀ上代 */
            ShoInp1SearchOpt.Cost = int.TryParse(ret_csv.Rows[0][3].ToString(), out int _cost) ? _cost : 0; /* 原価 */
            ShoInp1SearchOpt.CostPreData = _cost; /* 原価 */
            if (ret_csv.Rows[0][8].ToString() == "3") {
                ShoInp1SearchOpt.AccrualCd = AccrualList.FirstOrDefault().Key;
            }

            shohi = int.TryParse(ret_csv.Rows[0][5].ToString(), out int _shohi) ? _shohi : 0;

            csv_jodai = new DataTable();
            csv_genka = new DataTable();
            pre_csv_genka = new DataTable(); /* 原価対応2023/11/01 */
            var inscsv = new DataTable();
            inscsv.Columns.Add("Flex0", typeof(string));
            csv_jodai.Columns.Add("COL00", typeof(string));
            csv_genka.Columns.Add("COL00", typeof(string));
            pre_csv_genka.Columns.Add("COL00", typeof(string)); /* 原価対応2023/11/01 */

            if (ColSizFlg == 1) {
                sql_str = "select distinct サイズCD,get_sizename(商品CD,サイズCD) サイズ from HC$master_shohin_jan where 商品CD=:1 order by サイズCD";
            }
            else {
                sql_str = "select distinct 色CD,get_colorname(色CD) 色 from HC$master_shohin_jan where 商品CD=:1 order by 色CD";
            }

            var col_csv = AppData.Http?.AspxSqlQuery(sql_str, v_para.ToArray());
            csv_col = col_csv;

            for (int i = 0; i < col_csv.Rows.Count; i++)
            { 
            }


            return resp_code;
        }

        #region Events
        #endregion

        #region Functions
        #endregion
    }

    public partial class ShoInp1SearchOpt : ObservableObject
    {
        /// <summary>
        /// 伝票No (Text1)
        /// </summary>
        [ObservableProperty]
        string m_SlipNo;

        /// <summary>
        /// 入力者 (Text7) Code
        /// </summary>
        [ObservableProperty]
        string m_InputStaffCd;

        /// <summary>
        /// 入力者 (Text7) Name
        /// </summary>
        [ObservableProperty]
        string m_InputStaffName;

        /// <summary>
        /// 商品CD (TextLabel1)
        /// </summary>
        [ObservableProperty]
        string m_ProdCd;

        /// <summary>
        /// 商品名 (TextLabel2)
        /// </summary>
        [ObservableProperty]
        string m_ProdName;

        /// <summary>
        /// 取引区分 (Text6)
        /// </summary>
        [ObservableProperty]
        string m_TranCate;

        /// <summary>
        /// 発注日 (Label8) Title
        /// </summary>
        [ObservableProperty]
        string m_OrderDateTitle;

        /// <summary>
        /// 発注日 (Text4)
        /// </summary>
        [ObservableProperty]
        DateTime m_OrderDate;

        /// <summary>
        /// 納品日 (Text5)
        /// </summary>
        [ObservableProperty]
        DateTime m_DeliverDate;

        /// <summary>
        /// 納品日 (Label7) Title
        /// </summary>
        [ObservableProperty]
        string m_DeliverDateTitle;

        /// <summary>
        /// 掛計上日 (Text18)
        /// </summary>
        [ObservableProperty]
        DateTime m_AccountDate;

        /// <summary>
        /// 仕入先 (Text9)
        /// </summary>
        [ObservableProperty]
        BtListHelper m_SupplierCd;

        /// <summary>
        /// 掛計上 (Label9) Title Visible
        /// </summary>
        [ObservableProperty]
        Visibility m_AccrualTVis;

        /// <summary>
        /// 掛計上 (Text11)
        /// </summary>
        [ObservableProperty]
        int m_AccrualCd;

        /// <summary>
        /// 掛計上 (Text11) Value Visible
        /// </summary>
        [ObservableProperty]
        Visibility m_AccrualDVis;

        /// <summary>
        /// 入庫先 (Text8)
        /// </summary>
        [ObservableProperty]
        BtListHelper m_RecDestCd;

        /// <summary>
        /// 入庫先 (Label2) Title
        /// </summary>
        [ObservableProperty]
        string m_RecDestTitle;

        /// <summary>
        /// 在庫計上 (Label11) Title Visible 
        /// </summary>
        [ObservableProperty]
        Visibility m_RecInvTVis;

        /// <summary>
        /// 在庫計上 (Text16)
        /// </summary>
        [ObservableProperty]
        int m_RecInvCd;

        /// <summary>
        /// 在庫計上 (Text16) Value Visible
        /// </summary>
        [ObservableProperty]
        Visibility m_RecInvDVis;

        /// <summary>
        /// 備考 (Text17)
        /// </summary>
        [ObservableProperty]
        string m_Memo;

        /// <summary>
        /// 上代 (Text28)
        /// </summary>
        [ObservableProperty]
        int m_Retail;

        /// <summary>
        /// 原価 (Text29)
        /// </summary>
        [ObservableProperty]
        int m_Cost;

        /// <summary>
        /// 原価 (Text29) pre_data
        /// </summary>
        [ObservableProperty]
        int m_CostPreData;

        /// <summary>
        /// 原価FLG (Imp1)
        /// </summary>
        [ObservableProperty]
        int m_CostFlg;

        /// <summary>
        /// 消費税率 (Text3_2)
        /// </summary>
        [ObservableProperty]
        int m_TaxRate;

        /// <summary>
        /// 色 / ｻｲｽﾞ Title
        /// </summary>
        [ObservableProperty]
        string m_Dsp7Label;

        /// <summary>
        /// 色 / ｻｲｽﾞ (Dsp7)
        /// </summary>
        [ObservableProperty]
        string m_Dsp7;

        /// <summary>
        /// ｻｲｽﾞ / 色 (Dsp8)
        /// </summary>
        [ObservableProperty]
        string m_Dsp8;

        /// <summary>
        /// ｻｲｽﾞ / 色 Title
        /// </summary>
        [ObservableProperty]
        string m_Dsp8Label;

        /// <summary>
        /// DataGrid Column 1 Title 
        /// </summary>
        [ObservableProperty]
        string m_DgCol1Header;

        /// <summary>
        /// 数量合計 (Text13)
        /// </summary>
        [ObservableProperty]
        int m_TotalQnt;

        /// <summary>
        /// 上代合計 (Text14) 
        /// </summary>
        [ObservableProperty]
        long m_TotalRetail;

        /// <summary>
        /// 下代合計 (Text15) 
        /// </summary>
        [ObservableProperty]
        long m_TotalWholesales;

        /// <summary>
        /// 作成日 (Text2)
        /// </summary>
        [ObservableProperty]
        string m_VDate_Create;

        /// <summary>
        /// 修正日 (Text3)
        /// </summary>
        [ObservableProperty]
        string m_VDate_Update;

        public ShoInp1SearchOpt()
        {
            RecDestTitle = "入庫先";
            DeliverDateTitle = "納品日";
            OrderDateTitle = "発注日";
            AccrualTVis = Visibility.Collapsed;
            AccrualDVis = Visibility.Collapsed;
            RecInvTVis = Visibility.Collapsed;
            RecInvDVis = Visibility.Collapsed;
            Dsp7Label = "ｻｲｽﾞ";
            Dsp8Label = "色";
        }
    }
}
