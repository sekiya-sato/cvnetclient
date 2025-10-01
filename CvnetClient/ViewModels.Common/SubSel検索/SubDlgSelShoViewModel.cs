using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input; 
using CvnetClient.Class;
using CvnetClient.Models;
using CvnetClient.Utils;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace CvnetClient.ViewModels
{
    public partial class SubDlgSelShoViewModel : BaseViewModel
    {
        [ObservableProperty]
        ListFlexData listFlexData = new ListFlexData();

        [ObservableProperty]
        SelShoSearchOpt? m_SearchOpt;
            
        private BizArray para;
        private BizArray sv_cat;
        private BizArray wrk_jodai; /* セールマスタ参照配列 */
        private BizArray sv_init_para; /* init_para退避配列 */

        [ObservableProperty]
        SelValueModel? selectedValue; /* Return value for CvnetBtListView */
        public BizArray SelShoResult0;
        public Tuple<string, BizArray> SelShoResult1; /* Default return result method, datatype depends on DoExecute() */

        public void OnInit(string v_mstname, string[] init_para = null, string[] v_para2 = null)
        {
            OnInit(v_mstname, init_para, v_para2, null, null, 0);
        }

        public void OnInit(string v_mst, string[] init_para = null, string[] wrk_para = null, List<CsvItem> def = null, string[] wrk_para2 = null, int v_kt = 0)
        {
            ListFlexData.ListConfig = new ListFlexConfig()
            {
                init_csv = def ?? new List<CsvItem>(),
                flag = 0
            };
            SearchOpt = new SelShoSearchOpt(); 
            sv_cat = new BizArray();
            if (wrk_para != null) para = new BizArray(wrk_para);
            else para = new BizArray();

            #region Set ComboList
            Cond01List = new Dictionary<int, string>
            {
                { 1, "0 OR" },
                { 2, "1 AND" },
                { 3, "2 OFF" }
            };
            SearchOpt.SelCond01 = Cond01List.FirstOrDefault().Key;
            Cond02List = new Dictionary<int, string>
            {
                { 1, "0 完全一致" },
                { 2, "1 部分一致" },
                { 3, "2 前方一致" }
            };
            SearchOpt.SelCond02 = Cond02List.FirstOrDefault().Key;
            #endregion

            SearchOpt.SelInvChk = false;
            if (wrk_para != null)
            {
                if (wrk_para[0] == "3" || wrk_para[0] == "4" || wrk_para[0] == "5")
                    SearchOpt.SelInvChk = true;
            }  


            if (wrk_para2 != null) wrk_jodai = new BizArray(wrk_para2);
            else wrk_jodai = new BizArray();

            if (init_para != null) sv_init_para = new BizArray(init_para);
            else sv_init_para = new BizArray();

            if (AppData.ClassCvnet.config.oroshi != 0)
            {
                // Set LaunchDate design InVisible
                SearchOpt.LaunchDateIsVisible = 1;
            }

            if (v_kt == 1)
            {
                // Set ListFlexView design InVisible
                SearchOpt.FlexIsVisible = 1;
            }
        } 

        #region Combobox List 
        /// <summary>
        /// Query Condition (OR, AND, OFF)
        /// </summary>
        [ObservableProperty]
        public Dictionary<int, string>? m_Cond01List;

        /// <summary>
        /// 品名検索方法 (完全一致, 部分一致, 前方一致)
        /// </summary>
        [ObservableProperty]
        public Dictionary<int, string>? m_Cond02List;
        #endregion

        #region Functions
        /// <summary>
        /// Generate query from current form
        /// </summary>
        /// <returns>Item 1: Query, Item 2: Parameters </returns>
        Tuple<string, BizArray> OnQueryString()
        {
            if (SearchOpt  == null) return Tuple.Create<string, BizArray>("", null);

            string sel_string = "商品CD";
            switch (SearchOpt?.ModeProdCD)
            {
                case 1:
                    if (SearchOpt.Smtflg == 1) sel_string = "メーカー品番";
                    else sel_string = "旧コード";
                    break; 
            }

            var v_para = new BizArray();
            string sql_query = "select z.商品CD from hc$master_shohin z where ";
            sql_query += "z." + sel_string + " between :1 and :2 and z.店頭投入日 >= :3";

            v_para[0] = SearchOpt.StartProdCD;
            v_para[1] = SearchOpt.EndProdCD;
            v_para[2] = SearchOpt.SelInitLaunchDate.ToString("yyyyMMdd");
              
            int cnt = 4;
            var pre_csv = new BizCsvDocument();
            //ListConfig.conn_flg = SearchOpt.SelJoinCond; 
            sql_query +=  ListFlexData.GetQueryStr(v_para, cnt, SearchOpt.SelJoinCond);

            if (!string.IsNullOrEmpty(SearchOpt.SelProductName))
            {
                var qs_tmp = SearchOpt.SelProductName;
                var qs_tmp_ar = new BizArray();
                if (SearchOpt.SelCond01 == 2) {
                    qs_tmp_ar[0] = qs_tmp;
                } else {
                    qs_tmp = qs_tmp.Replace("　", " ");
                    qs_tmp_ar = new BizArray(qs_tmp.Split(" "));
                }
                string qs_str_tmp = string.Empty;
                for (var j = 0; j < qs_tmp_ar.Count; j++)
                {
                    if (j != 0)
                    {
                        if (SearchOpt.SelCond01 == 0) {
                            qs_str_tmp += " or ";
                        } else {
                            qs_str_tmp += " and ";
                        }
                    }
                    if (SearchOpt.SelBSChk)
                    {
                        if (SearchOpt.SelCond02 == 1)
                        {
                            qs_str_tmp += " (";
                            for (int i = 0; i < AppData.ClassEtc.SearchShohinStr.Length; i++) {
                                qs_str_tmp += ((i == 0) ? "" : " or ") + " z." + AppData.ClassEtc.SearchShohinStr[i] + " = '" + qs_tmp_ar[j] + "' ";
                            }
                            qs_str_tmp += ") ";
                        }
                        else if (SearchOpt.SelCond02 == 2)
                        {
                            qs_str_tmp += " (";
                            for (var i = 0; i < AppData.ClassEtc.SearchShohinStr.Length; i++) {
                                qs_str_tmp += ((i == 0) ? "" : " or ") + " z." + AppData.ClassEtc.SearchShohinStr[i] + " like '%" + qs_tmp_ar[j] + "%' ";
                            }
                            qs_str_tmp += ") ";
                        }
                        else
                        {
                            qs_str_tmp += " (";
                            for (var i = 0; i < AppData.ClassEtc.SearchShohinStr.Length; i++) {
                                qs_str_tmp += ((i == 0) ? "" : " or ") + " z." + AppData.ClassEtc.SearchShohinStr[i] + " like '" + qs_tmp_ar[j] + "%' ";
                            }
                            qs_str_tmp += ") ";
                        }
                    }
                    else 
                    {
                        if (SearchOpt.SelCond02 == 1)
                        {
                            qs_str_tmp += " (";
                            for (var i = 0; i < AppData.ClassEtc.SearchShohinStr.Length; i++)
                            {
                                qs_str_tmp += ((i == 0) ? "" : " or ") + " upper(z." + AppData.ClassEtc.SearchShohinStr[i] + ") = upper('" + qs_tmp_ar[j] + "') ";
                            }
                            qs_str_tmp += ") ";
                        }
                        else if (SearchOpt.SelCond02 == 2)
                        {
                            qs_str_tmp += " (";
                            for (var i = 0; i < AppData.ClassEtc.SearchShohinStr.Length; i++)
                            {
                                qs_str_tmp += ((i == 0) ? "" : " or ") + " upper(z." + AppData.ClassEtc.SearchShohinStr[i] + ") like upper('%" + qs_tmp_ar[j] + "%') ";
                            }
                            qs_str_tmp += ") ";
                        }
                        else
                        {
                            qs_str_tmp += " (";
                            for (var i = 0; i < AppData.ClassEtc.SearchShohinStr.Length; i++)
                            {
                                qs_str_tmp += ((i == 0) ? "" : " or ") + " upper(z." + AppData.ClassEtc.SearchShohinStr[i] + ") like upper('" + qs_tmp_ar[j] + "%') ";
                            }
                            qs_str_tmp += ") ";
                        }
                    }
                }
                sql_query += " and (" + qs_str_tmp + ")";
            }
            sv_cat.Clear();
            if (SearchOpt.SelInvChk)
            {
                string zais = "";
                string zaitbl = "";
                if (AppData.ClassCvnet.config.Real_Zaiko == 0)
                {
                    zais = "当月在庫数";
                    zaitbl = "HC$MANAGE_ZAIKO";
                }
                else if (AppData.ClassCvnet.config.Real_Zaiko == 1)
                {   /* 最新在庫ファイル参照ルート */
                    zais = "在庫数";
                    zaitbl = "HC$MANAGE_REAL_ZAIKO";
                }
                var souko = "";
                if (para.Count > 1 && para[1] != null && para[1] != "") { 
                    souko = " and zz.倉庫CD = '" + para[1] + "'";
                }
                string sql_query2 = "select z.商品CD from (select zs.商品CD, sum(NVL(zz." + zais + ", 0)) 在庫数sum from " + zaitbl + " zz, (" + sql_query + ") zs "
                + "where zz.商品CD = zs.商品CD " + souko + " group by zs.商品CD) z where z.在庫数sum between " + SearchOpt.StartInvNum + " and " + SearchOpt.EndInvNum;
                sql_query = sql_query2;
            } 
            return Tuple.Create<string, BizArray>(sql_query, v_para);
        }
        string GetConvKubun(string kubun)
        {
            string cd_name = "";
            if (kubun == "ITM")
            {
                cd_name = "アイテムCD";
            }
            else if (kubun == "BRD")
            {
                cd_name = "ブランドCD";
            }
            else if (kubun == "DZN")
            {
                cd_name = "デザイナーCD";
            }
            else if (kubun == "SZN")
            {
                cd_name = "シーズンCD";
            }
            else if (kubun == "TNJ")
            {
                cd_name = "展示会CD";
            }
            else if (kubun == "SZI")
            {
                cd_name = "素材CD";
            }
            else if (kubun == "MKR")
            {
                cd_name = "メーカーCD";
            }
            else if (kubun == "GEN")
            {
                cd_name = "原産国CD";
            }
            else
            {
                if (Regex.IsMatch(kubun, @"B[0-9]{2}"))
                {
                    cd_name = "名称CD" + kubun.Substring(1, 2);
                }
            }
            return cd_name;
        }
        #endregion

        #region Events
        [RelayCommand]
        void DoShohinMode()
        {
            switch (SearchOpt?.ModeProdCD)
            {
                case 0:
                    SearchOpt.ModeProdCD = 1; 
                    break;
                case 1:
                    SearchOpt.ModeProdCD = 0;
                    break;
                default: 
                    break;
            }
        }
        [RelayCommand]
        void DoExecute()
        {
            var ret = OnQueryString();
            if (para.Count == 0)
            {
                para[0] = "0";
            }
            if (para[0] == "1")
            {
                //return result and exit from current form
                SelShoResult1 = ret; 
                ClientLib.ExitDialogResult(this, true);
            }
            else if (para[0] == "0")
            {
                /* 在庫問い合わせ等 */
                SubDlgSel2ViewModel vm_result = null;
                if (wrk_jodai.Count > 0) 
                    vm_result = AppData.DlgService.GetSel2(new string[] { "1" }, ret.Item2.ToArray(), ret.Item1, wrk_jodai.ToArray()); 
                else 
                    vm_result = AppData.DlgService.GetSel2(new string[] { "1" }, ret.Item2.ToArray(), ret.Item1); 
                if (vm_result != null)
                {
                    SelectedValue = new SelValueModel()
                    {
                        Code = (vm_result.SelectedSel2 != null) ? vm_result.SelectedSel2.ProductCD : "",
                        Name = (vm_result.SelectedSel2 != null) ? vm_result.SelectedSel2.ProductName : "",
                    }; 
                    SelShoResult0 = vm_result.ret_para; 
                    ClientLib.ExitDialogResult(this, true);
                }
            }
        }
        [RelayCommand]
        void DoExit()
        {
            ClientLib.ExitDialogResult(this, true);
        }
        #endregion
    }

    /// <summary>
    /// SelShoSearchOpt - Select Shohin Search Option
    /// </summary>
    public partial class SelShoSearchOpt : ObservableObject
    {
        /// <summary>
        /// Mode to identify to search Shohin by 旧コード or 商品CD
        /// </summary>
        [ObservableProperty]
        public int m_ModeProdCD;

        /// <summary>
        /// flag from CvnetClass
        /// </summary>
        [ObservableProperty]
        public int m_Smtflg;

        /// <summary>
        /// 商品CD - Start Search Condition
        /// </summary>
        [ObservableProperty]
        public string m_StartProdCD;

        /// <summary>
        /// 商品CD -  End Search Condition
        /// </summary>
        [ObservableProperty]
        public string m_EndProdCD;

        /// <summary>
        /// B/S Checkbox Select
        /// </summary>
        [ObservableProperty]
        public bool m_SelBSChk;

        /// <summary>
        /// Search Condition (1 OR,2 AND,3 OFF)
        /// </summary>
        [ObservableProperty]
        public int m_SelCond01;

        /// <summary>
        /// Search Condition (1 完全一致,2 部分一致,3 前方一致)
        /// </summary>
        [ObservableProperty]
        public int m_SelCond02;

        /// <summary>
        /// 商品名 Search Condition
        /// </summary>
        [ObservableProperty]
        public string m_SelProductName;

        /// <summary>
        /// 店頭投入日
        /// </summary>
        [ObservableProperty]
        public DateTime m_SelInitLaunchDate;

        /// <summary>
        /// 店頭投入日 Design Visible Status (0 Visible, 1 Not Visible)
        /// </summary>
        [ObservableProperty]
        public int m_LaunchDateIsVisible;

        /// <summary>
        /// 在庫 Checkbox Select
        /// </summary>
        [ObservableProperty]
        public bool m_SelInvChk;

        /// <summary>
        /// 在庫数 - Start Search Condition
        /// </summary>     
        [ObservableProperty]
        public int m_StartInvNum;

        /// <summary>
        /// 在庫数 - End Search Condition
        /// </summary>
        [ObservableProperty]
        public int m_EndInvNum;

        /// <summary>
        /// 条件結合 (0 AND, 1 OR)
        /// </summary>
        [ObservableProperty]
        public int m_SelJoinCond;

        /// <summary>
        /// ListFlexView Design Visible Status (0 Visible, 1 Not Visible)
        /// </summary>
        [ObservableProperty]
        public int m_FlexIsVisible;

        public SelShoSearchOpt()
        {
            ModeProdCD = 0;
            Smtflg = AppData.ClassCvnet.config.smtflg;
            StartProdCD = string.Empty; 
            EndProdCD = "ZZZZZZZZZZZZZZZZ";
            SelBSChk = false;
            SelProductName = string.Empty;
            SelInitLaunchDate = new DateTime(1901, 1, 1);
            LaunchDateIsVisible = 0;
            SelInvChk = false;
            StartInvNum = 1;
            EndInvNum = 9999;
            SelJoinCond = 0;
            FlexIsVisible = 0;
        }
    } 
}
