using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Models;
using CvnetClient.Utils;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;

namespace CvnetClient.ViewModels
{
    public partial class SubDlgSel2ViewModel : BaseViewModel
    {
        private BizArray para;
        private BizArray para2;
        private string sv_sql;
        private BizArray wrk_jodai; 
        private string Imp99;

        [ObservableProperty]
        Sel2Disp? dspItem;

        [ObservableProperty]
        ObservableCollection<SubDlgSel2Model> listSel2;

        [ObservableProperty]
        SubDlgSel2Model? selectedSel2;
         
        /// <summary>
        /// Current Dialog return value
        /// </summary>
        public BizArray ret_para;

        #region Functions
        public void OnInit(string[] init_para = null, string[] init_para2 = null, string qs = "", string[] init_para3 = null)
        {
            DspItem = new Sel2Disp();
            ListSel2 = new ObservableCollection<SubDlgSel2Model>();

            if (init_para != null) para = new BizArray(init_para);
            else para = new BizArray();
            if (init_para2.Length > 0) Imp99 = init_para2[0]; 
            if (init_para2.Length > 0) para2 = new BizArray(init_para2);
            else para2 = new BizArray();
            if (!string.IsNullOrEmpty(qs)) sv_sql = qs;
            if (init_para3 != null) wrk_jodai = new BizArray(init_para3);
            else wrk_jodai = new BizArray();

            OnInit2();
        }

        private void OnInit2(string p_sort = "")
        {
            DataTable ret_csv;
            if (para.Count > 0) 
            {
                string v_sort = " asc ";
                if (!string.IsNullOrEmpty(p_sort)) v_sort = p_sort;
                string v_hugo = ">=";
                if (!string.IsNullOrEmpty(p_sort)) v_hugo = "<=";

                BizArray v_para = new BizArray();

                /* 上代取得 */
                string v_jd1 = ",A.上代";
                if (wrk_jodai.Count > 0)
                {
                    if (wrk_jodai.Count > 2)
                    {
                        v_jd1 = ",get_jodai(A.商品CD,'" + wrk_jodai[0] + "','" + wrk_jodai[1] + "','" + wrk_jodai[2] + "','" + wrk_jodai[3] + "') 上代";
                        /* 卸対応 */
                        if (AppData.ClassCvnet.config.oroshi == 1) v_jd1 = ",get_jodai(A.商品CD,'" + wrk_jodai[0] + "','" + wrk_jodai[1] + "','" + wrk_jodai[2] + "','" + wrk_jodai[5] + "',0) 上代";
                    }
                    else
                    {
                        v_jd1 = ",get_jodai(A.商品CD,'" + wrk_jodai[0] + "','" + wrk_jodai[1] + "') 上代";
                    }
                }

                /* 原価取得 */
                var v_gk1 = ",A.原価";
                if (para[3] != null)
                {
                    v_gk1 = ",get_genka(A.商品CD,'" + para[3] + "','" + para[4] + "') 原価";
                }

                /* 卸対応（単品) */
                if (AppData.ClassCvnet.config.oroshi == 2)
                {
                    if (para[3] != null) {
                        v_gk1 = ",get_genka(A.商品CD,'" + para[3] + "','" + para[4] + "','" + wrk_jodai[0] + "','" + wrk_jodai[1] + "') 原価";
                    }
                }

                /* 色サイズ原価 */
                if (AppData.ClassCvnet.config.GenkaSKU == 1)
                {
                    if (para[3] != null)
                    {
                        v_gk1 = ",get_genka(A.商品CD,'" + para[3] + "','" + para[4] + "','" + wrk_jodai[0] + "','" + wrk_jodai[1] + "') 原価";
                    }
                }

                /* 名称取得 */
                var v_ms1 = ",A.ブランドCD,A.アイテムCD,A.デリバリー日,B.名称 展示会名,C.名称 ブランド名,D.名称 アイテム名";
                if (AppData.ClassCvnet.config.smtflg == 1)
                {
                    v_ms1 = ",A.メーカーCD,A.アイテムCD,A.デリバリー日,B.名称 展示会名,C.名称 メーカー名,D.名称 アイテム名";
                }

                string sql_query = "select A.商品CD,A.メーカー品番,A.商品名" + v_jd1;
                sql_query += ",A.絵型名,A.展示会CD" + v_ms1 + v_gk1;
                sql_query += ",A.消費税計算方法";
                sql_query += ",A.上代 マスタ上代";
                sql_query += ",to_date(decode(A.店頭投入日,'.','19010101',A.店頭投入日)) 店頭投入日 ";
                sql_query += ",A.営業原価";
                sql_query += ",A.仕入価格";
                sql_query += ",A.納品日";
                sql_query += ",A.仕入区分||' '||decode(A.仕入区分,1,'買取',2,'委託',3,'消化','') 仕入区分";
                sql_query += ((AppData.ClassCvnet.config.tanpin == 1) ? ",A.商品管理FLG" : ",0 商品管理FLG");
                sql_query += ((AppData.ClassCvnet.config.oroshi >= 1) ? ",A.単位" : ",'' 単位");
                sql_query += ((AppData.ClassCvnet.config.oroshi == 1) ? ",get_jodai(A.商品CD,'" + wrk_jodai[0] + "','" + wrk_jodai[1] + "','" + wrk_jodai[2] + "','" + wrk_jodai[5] + "',1) 売単価" : ",0 売単価");
                sql_query += ",A.名称CD01 副名CD1";
                sql_query += ",E.名称 副名1";
                sql_query += ",A.名称CD02 副名CD2";
                sql_query += ",F.名称 副名2";
                sql_query += ",A.名称CD03 副名CD3";
                sql_query += ",G.名称 副名3";
                sql_query += ",A.商品サイズ区分";
                sql_query += ",A.在庫管理FLG";
                sql_query += " from HC$MASTER_SHOHIN A";
                sql_query += " ,(select 名称CD,名称 from HC$Master_MEISHO where 名称区分='TNJ') B";
                sql_query += " ,(select 名称CD,名称 from HC$Master_MEISHO where 名称区分='BRD') C";
                sql_query += " ,(select 名称CD,名称 from HC$Master_MEISHO where 名称区分='ITM') D";
                sql_query += " ,(select 名称CD,名称 from HC$Master_MEISHO where 名称区分='B01') E";
                sql_query += " ,(select 名称CD,名称 from HC$Master_MEISHO where 名称区分='B02') F";
                sql_query += " ,(select 名称CD,名称 from HC$Master_MEISHO where 名称区分='B03') G";

                /* 汎用検索でのSQL文字列対応 */
                if (sv_sql != "")
                { 
                    sql_query += " where exists (" + sv_sql + " and A.商品CD=z.商品CD group by z.商品CD)";
                    for (var i = 0; i < para2.Count; i++)
                    {
                        v_para[i] = para2[i];
                    }
                }
                else
                {
                    sql_query += " where A.商品CD" + v_hugo + ":1";
                }

                if (para[0] != null && para[0] != "")
                {
                    v_para[0] = para[0];
                }
                else if (para2[0].Length == 0)
                {
                    /* para2[0]がnullの場合は"."を入れる */
                    v_para[0] = ".";
                }
                else if (v_para[0].Length == 0)
                {
                    /* v_para[0]がnullの場合は"."を入れる */
                    v_para[0] = ".";
                }

                sql_query += "  and (A.展示会CD=B.名称CD(+) and A.ブランドCD=C.名称CD(+) and A.アイテムCD=D.名称CD(+) and A.名称CD01=E.名称CD(+) and A.名称CD02=F.名称CD(+) and A.名称CD03=G.名称CD(+) )";
                if (para.Count > 1)
                {  
                    if (para[1] == "0")
                    { /* 仕入先限定 */
                        v_para[v_para.Count] = para[2];
                        sql_query += " and A.メーカーCD=:2"; 
                    }
                }

                sql_query += "  order by 商品CD" + v_sort;
                sql_query = AppData.ClassCvnet.GetSqlDisp(sql_query);

                /* 2008.11.25 在庫データ取得方法変更対応 */
                var zaiko_tbl = "hc$manage_zaiko";
                var nengetsu = "";
                if (AppData.ClassCvnet.config.Rk_Zaiko == 1) zaiko_tbl = "HC$VIEW_ZAIKORK"; 
                if (AppData.ClassCvnet.config.Rk_Zaiko == 1) nengetsu = " and z.年月='" + AppData.ClassSatoo.GetDateVal(DateTime.Now, 0, "00") + "'";

                /* 在庫数を追加 */
                if (wrk_jodai[3] != null) {
                    sql_query = "select a.*,nvl((select sum(当月在庫数) from " + zaiko_tbl + " z where z.商品CD=a.商品CD and z.倉庫CD='" + wrk_jodai[3] + "'" + nengetsu + "),0) 在庫数 from ("
                    + sql_query + ") a";
                    //Form1.Spread1.SetLabel(17,"倉庫在庫");
                }
                else {
                    sql_query = "select a.*,nvl((select sum(当月在庫数) from " + zaiko_tbl + " z where z.商品CD=a.商品CD " + nengetsu + "),0) 在庫数 from ("
                    + sql_query + ") a";
                    //Form1.Spread1.SetLabel(17,"全在庫");
                }

                sql_query = AppData.ClassCvnet.GetSqlDisp(sql_query);
                /* 13.08.07 upd start okamoto */
                ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para.ToArray());

                if (ret_csv?.Rows.Count > 0 && !string.IsNullOrEmpty(p_sort)) {
                    ret_csv = ret_csv.AsEnumerable()
                                     .OrderBy(r => r[0])
                                     .CopyToDataTable();
                }

                var list = (from DataRow dr in ret_csv.Rows
                            select new SubDlgSel2Model
                            {
                                ProductCD = dr["商品CD"].ToString() ?? string.Empty,
                                MakerNo = dr["メーカー品番"].ToString() ?? string.Empty,
                                ProductName = dr["商品名"].ToString() ?? string.Empty,
                                Price = long.TryParse(dr["上代"].ToString(), out var _price) ? _price : 0,
                                ImgName = dr["絵型名"].ToString() ?? string.Empty,
                                ExhibitCD = dr["展示会CD"].ToString() ?? string.Empty,
                                BrandCD = dr["ブランドCD"].ToString() ?? string.Empty,
                                ItemCD = dr["アイテムCD"].ToString() ?? string.Empty,
                                CustDeliDate = dr["デリバリー日"].ToString() ?? string.Empty,
                                ExhibitName = dr["展示会名"].ToString() ?? string.Empty,
                                BrandName = dr["ブランド名"].ToString() ?? string.Empty,
                                ItemName = dr["アイテム名"].ToString() ?? string.Empty,
                                Cost = long.TryParse(dr["原価"].ToString(), out var _cost) ? _cost : 0,
                                TaxCalcMethod = int.TryParse(dr["消費税計算方法"].ToString(), out var _taxCalc) ? _taxCalc : 0,
                                MasterPrice = long.TryParse(dr["マスタ上代"].ToString(), out var _mstPrice) ? _mstPrice : 0,
                                InitLaunchDate = dr["店頭投入日"].ToString() ?? string.Empty,
                                OpCostPrice = long.TryParse(dr["営業原価"].ToString(), out var _opcost) ? _opcost : 0,
                                PurchasePrice = long.TryParse(dr["仕入価格"].ToString(), out var _poprice) ? _poprice : 0,
                                DeliveryDate = dr["納品日"].ToString() ?? string.Empty,
                                PurchaseCate = int.TryParse(dr["仕入区分"].ToString(), out var _cate) ? _cate : 0,
                                PrdMngmentFLG = int.TryParse(dr["商品管理FLG"].ToString(), out var _flg) ? _flg : 0,
                                Unit = dr["単位"].ToString() ?? string.Empty,
                                SalesUnitPrice = int.TryParse(dr["売単価"].ToString(), out var _salesUnit) ? _salesUnit : 0,
                                SubnameCD1 = dr["副名CD1"].ToString() ?? string.Empty,
                                Subname1 = dr["副名1"].ToString() ?? string.Empty,
                                SubnameCD2 = dr["副名CD2"].ToString() ?? string.Empty,
                                Subname2 = dr["副名2"].ToString() ?? string.Empty,
                                SubnameCD3 = dr["副名CD3"].ToString() ?? string.Empty,
                                Subname3 = dr["副名3"].ToString() ?? string.Empty,
                                ProdSizeCate = dr["商品サイズ区分"].ToString() ?? string.Empty,
                                InvMngmentFLG = int.TryParse(dr["在庫管理FLG"].ToString(), out var _flg2) ? _flg2 : 0,
                                InvQuantity = long.TryParse(dr["在庫数"].ToString(), out var _qnt) ? _qnt : 0 
                            }).OrderBy(c => c.ProductCD).ToList(); 
                ListSel2 = new ObservableCollection<SubDlgSel2Model>(list);
            }
        }
        #endregion

        #region Events
        partial void OnSelectedSel2Changed(SubDlgSel2Model value)
        {
            if (value == null || DspItem == null) return;
            DspItem.SelShohinCD = value.ProductCD;
            DspItem.DspExhibit = string.Format("{0} {1}", value.ExhibitCD, value.ExhibitName);
            DspItem.DspBrand = string.Format("{0} {1}", value.BrandCD, value.BrandName);
            DspItem.DspItem = string.Format("{0} {1}", value.ItemCD, value.ItemName);
            DspItem.DspCustDeliDate = value.CustDeliDate;
            DspItem.DspImage = string.Format("{0}{1}{2}{3}",AppData.Url, "Data/", AppData.DataAddPath, value.ImgName);
        }

        [RelayCommand]
        void PrevList()
        { 
            if (ListSel2.Count == 0) return;

            string v_para0 = para2[0];
            string v_para1 = para2[1];

            DspItem.SelShohinCD = ListSel2.FirstOrDefault().ProductCD;
            para[0] = ".";
            para2[0] = "."; para2[1] = DspItem.SelShohinCD;
            OnInit2(" desc ");

            para2[0] = v_para0;
            para2[1] = v_para1;
        }

        [RelayCommand]
        void NextList()
        {
            if (ListSel2.Count == 0) return;

            string v_para0 = para2[0];
            string v_para1 = para2[1];

            DspItem.SelShohinCD = ListSel2.LastOrDefault().ProductCD;
            para[0] = DspItem.SelShohinCD;
            para2[0] = DspItem.SelShohinCD;
            OnInit2();

            para2[0] = v_para0;
            para2[1] = v_para1;
        }

        [RelayCommand]
        void TopList()
        {
            para[0] = Imp99;
            OnInit2();
        }

        [RelayCommand]
        void DoExecute()
        {
            if (SelectedSel2 == null) return;
            ret_para = new BizArray();

            /* 2007.10.11 メーカー品番を追加したので、調整 */ 
            ret_para[0] = SelectedSel2.ProductCD;
            ret_para[1] = SelectedSel2.ProductName;
            ret_para[2] = SelectedSel2.Price.ToString();
            ret_para[3] = SelectedSel2.ImgName;
            ret_para[4] = dspItem.DspExhibit;
            ret_para[5] = dspItem.DspBrand;
            ret_para[6] = dspItem.DspItem;
            ret_para[7] = dspItem.DspCustDeliDate;
            ret_para[8] = SelectedSel2.Cost.ToString();
            ret_para[9] = SelectedSel2.TaxCalcMethod.ToString();

            /* 項目追加 */
            ret_para[10] = SelectedSel2.BrandCD;
            ret_para[11] = SelectedSel2.BrandName;
            ret_para[12] = SelectedSel2.MasterPrice.ToString();

            /* さらに追加・店頭投入日 20060816 */
            ret_para[13] = SelectedSel2.InitLaunchDate; // return format is yyyy/MM/dd
            ret_para[14] = SelectedSel2.OpCostPrice.ToString(); /* 営業原価追加 2007.10.03 */
            ret_para[15] = SelectedSel2.PurchasePrice.ToString();  /* 仕入値追加 2008.06.16 */
            bool isDate = DateTime.TryParseExact(selectedSel2.DeliveryDate, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt16);
            ret_para[16] = (isDate) ? dt16.ToString("yyyyMMdd") : "19010101"; /* 納品日追加 2008.09.26 */

            /* さらに追加・メーカー品番 20081106 */
            ret_para[17] = SelectedSel2.MakerNo;
            /* さらに追加・仕入区分 20091006 */
            ret_para[18] = SelectedSel2.PurchaseCate.ToString();
            /* さらに追加・商品管理FLG 20100405 */
            ret_para[19] = SelectedSel2.PrdMngmentFLG.ToString();
            /* さらに追加・単位 20100428 */
            ret_para[20] = SelectedSel2.Unit.ToString();
            /* さらに追加・売単価 20100706 */
            ret_para[21] = SelectedSel2.SalesUnitPrice.ToString();
            /* さらに追加 副名1・2・3 20100714 */
            ret_para[22] = SelectedSel2.SubnameCD1;
            ret_para[23] = SelectedSel2.Subname1;
            ret_para[24] = SelectedSel2.SubnameCD2;
            ret_para[25] = SelectedSel2.Subname2;
            ret_para[26] = SelectedSel2.SubnameCD3;
            ret_para[27] = SelectedSel2.Subname3;
            /* さらに追加 サイズ区分 20110524 */
            ret_para[28] = SelectedSel2.ProdSizeCate;
            /* さらに追加 在庫管理フラグ */
            ret_para[29] = SelectedSel2.InvMngmentFLG.ToString();

            //confirm and close window
            ClientLib.ExitDialogResult(this, true); 
        }

        [RelayCommand]
        void DoExit()
        {
            //confirm and close window
            //ClientLib.ExitDialogResult(this, true);
            this.Close();
        }
        #endregion
    }

    public partial class Sel2Disp : ObservableObject
    {
        /// <summary>
        /// 商品CD Selected - BizV Imp1
        /// </summary>
        [ObservableProperty]
        string? m_SelShohinCD; 

        /// <summary>
        /// 展示会CD - BizV Dsp1
        /// </summary>
        [ObservableProperty]
        string? m_DspExhibit;

        /// <summary>
        /// ブランドCD - BizV Dsp2
        /// </summary>
        [ObservableProperty]
        string? m_DspBrand;

        /// <summary>
        /// アイテムCD - BizV Dsp3
        /// </summary>
        [ObservableProperty]
        string? m_DspItem;

        /// <summary>
        /// デリバリー日 - BizV Dsp4
        /// </summary>
        [ObservableProperty]
        string? m_DspCustDeliDate;

        /// <summary>
        /// ImageLabel1
        /// </summary>
        [ObservableProperty]
        string? m_DspImage;

        public Sel2Disp()
        {
            SelShohinCD = string.Empty;
            DspExhibit = string.Empty;
            DspBrand = string.Empty; 
            DspItem = string.Empty;
            DspCustDeliDate = string.Empty; 
        }
    }
}
