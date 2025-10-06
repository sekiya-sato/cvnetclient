using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Class;
using CvnetClient.Models;
using CvnetClient.Utils;

namespace CvnetClient.ViewModels
{
    public partial class SubDlgSelSirViewModel : BaseViewModel
    {
        [ObservableProperty]
        ListFlexData listFlexData = new ListFlexData();

        [ObservableProperty]
        SelSirSearchOpt? m_SearchOpt;

        [ObservableProperty]
        string title = "仕入先選択画面";

        private BizArray para;
        private BizArray sv_cat;
        private BizArray wrk_jodai; /* セールマスタ参照配列 */

        string Mst_sql = string.Empty; /* バインド無し */
        string Mst_sql2 = string.Empty; /* バインド付き para.length>0の場合 */
        string v_mst2 = string.Empty;

        [ObservableProperty]
        SelValueModel? selectedValue; /* Return value for CvnetBtListView */
        public BizArray SelSirResult0;
        public Tuple<string, BizArray> SelSirResult1; /* Default return result method, datatype depends on DoExecute() */


        public void OnInit(string v_mstname, string[] init_para = null, string[] v_para2 = null)
        {
            OnInit(v_mstname, init_para, v_para2, null, null);
        }

        public void OnInit(string v_mst, string[] init_para = null, string[] wrk_para = null, List<CsvItem> def = null, string[] wrk_para2 = null)
        {
            if (wrk_para != null) para = new BizArray(wrk_para);
            else para = new BizArray();

            ListFlexData.ListConfig = new ListFlexConfig()
            {
                init_csv = def ?? new List<CsvItem>(),
                flag = 2
            };
            SearchOpt = new SelSirSearchOpt();

            if (wrk_para2 != null) wrk_jodai = new BizArray(wrk_para2);
            else wrk_jodai = new BizArray();
            if (sv_cat == null) sv_cat = new BizArray();

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
            SearchOpt.SelCond02 = 2;

            var comboItem = AppData.ClassCvnet.comboItem00;

            CloseDateList = comboItem.ComboItem_00<string>("締日");
            SearchOpt.StartCloseDate = "01";
            SearchOpt.EndCloseDate = "99";
            #endregion

            /* 付加SQL */
            Mst_sql = string.Empty; Mst_sql2 = string.Empty;
            if (AppData.ClassCvnet.MstDialog.ContainsKey(v_mst))
            {
                MstItem add = AppData.ClassCvnet.MstDialog[v_mst];
                if (add.v_para != null)
                {
                    if (add.v_para.Length > 0)
                        Mst_sql = add.v_para[0] + " and ";
                    if (add.v_para.Length > 1 && wrk_para != null)
                        Mst_sql2 = add.v_para[1] + " and ";
                }
            }

            /* 範囲初期値 v_mst保存 */
            if (init_para !=  null && init_para.Length > 0) 
                SearchOpt.StartShiireCD = init_para[0];
            v_mst2 = v_mst;
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

        /// <summary>
        /// 締日
        /// </summary>
        [ObservableProperty]
        public Dictionary<string, string>? m_CloseDateList;
        #endregion

        #region Functions
        /// <summary>
        /// Generate query from current form
        /// </summary>
        /// <returns>Item 1: Query, Item 2: Parameters </returns>
        Tuple<string, BizArray> OnQueryString(BizArray param = null)
        {
            var v_para = new BizArray();
            string sql_query = "select 仕入先CD from hc$master_siire z where ";

            #region Add or Remove 発注停止FLG query (Based on 発注停止FLG CheckBox)
            int idx = -1; //Search Index
            while ((idx = Mst_sql.IndexOf("発注停止FLG", idx + 1, StringComparison.Ordinal)) >= 0)
            {
                if (SearchOpt?.SelOrderChk != true)
                {
                    Mst_sql = Mst_sql.Remove(idx, 16).Insert(idx, new string(' ', 16));
                }
            }
            // Once loop ends, if checkbox is selected:
            if (SearchOpt?.SelOrderChk == true) sql_query += "発注停止FLG=0 and ";
            #endregion

            sql_query += Mst_sql;
            int cnt = 1;
            /* 「>>」押下の際に、v_para2が入っているとき対応（請求など）。数字のみ有効 */
            if (param != null && param.Count > 0)
            {
                decimal param_1 = decimal.TryParse(param[0], out var para1) ? para1 : 0;
                if (param_1 < 9999999999999999999999999999m && param_1 != 0)
                {
                    for (int i = 0; i < param.Count; i++)
                    {
                        /* 2017.04.14 締日2、締日3用にパラメータを追加 */
                        v_para[v_para.Count] = param[i];
                        cnt++;
                        sql_query += Mst_sql2;
                    }
                }
            } 
            sql_query += " 仕入先CD between :" + cnt.ToString() + " and :" + (cnt + 1).ToString() + " and 締日 between :" + (cnt + 2).ToString() + " and :" + (cnt + 3).ToString() + "";

            v_para[v_para.Count] = SearchOpt.StartShiireCD;
            v_para[v_para.Count] = SearchOpt.EndShiireCD;
            v_para[v_para.Count] = SearchOpt.StartCloseDate;
            v_para[v_para.Count] = SearchOpt.EndCloseDate;
            cnt = cnt + 4;

            sql_query += ListFlexData.GetQueryStr(v_para, cnt, SearchOpt.SelJoinCond);
            if (!string.IsNullOrEmpty(SearchOpt.SelShiireName))
            {
                string qs_tmp = SearchOpt.SelShiireName;
                var qs_tmp_ar = new BizArray();
                if (SearchOpt.SelCond01 == 2)
                {
                    qs_tmp_ar[0] = qs_tmp;
                }
                else
                {
                    qs_tmp = qs_tmp.Replace("　", " ");
                    qs_tmp_ar = new BizArray(qs_tmp.Split(" "));
                }
                string qs_str_tmp = string.Empty;
                for (int j = 0; j < qs_tmp_ar.Count; j++)
                {
                    if (j != 0)
                    {
                        if (SearchOpt.SelCond01 == 0)
                            qs_str_tmp += " or ";
                        else
                            qs_str_tmp += " and ";
                    }
                    if (SearchOpt.SelBSChk)
                    {
                        if (SearchOpt.SelCond02 == 1)
                        {
                            qs_str_tmp += " (";
                            for (int i = 0; i < AppData.ClassEtc.SearchSiireStr.Length; i++)
                            {
                                qs_str_tmp += ((i == 0) ? "" : " or ") + " z." + AppData.ClassEtc.SearchSiireStr[i] + " = '" + qs_tmp_ar[j] + "' ";
                            }
                            qs_str_tmp += ") ";
                        }
                        else if (SearchOpt.SelCond02 == 2)
                        {
                            qs_str_tmp += " (";
                            for (var i = 0; i < AppData.ClassEtc.SearchSiireStr.Length; i++)
                            {
                                qs_str_tmp += ((i == 0) ? "" : " or ") + " z." + AppData.ClassEtc.SearchSiireStr[i] + " like '%" + qs_tmp_ar[j] + "%' ";
                            }
                            qs_str_tmp += ") ";
                        }
                        else
                        {
                            qs_str_tmp += " (";
                            for (var i = 0; i < AppData.ClassEtc.SearchSiireStr.Length; i++)
                            {
                                qs_str_tmp += ((i == 0) ? "" : " or ") + " z." + AppData.ClassEtc.SearchSiireStr[i] + " like '" + qs_tmp_ar[j] + "%' ";
                            }
                            qs_str_tmp += ") ";
                        }
                    }
                    else
                    {
                        if (SearchOpt.SelCond02 == 1)
                        {
                            qs_str_tmp += " (";
                            for (var i = 0; i < AppData.ClassEtc.SearchSiireStr.Length; i++)
                            {
                                qs_str_tmp += ((i == 0) ? "" : " or ") + " upper(z." + AppData.ClassEtc.SearchSiireStr[i] + ") = upper('" + qs_tmp_ar[j] + "') ";
                            }
                            qs_str_tmp += ") ";
                        }
                        else if (SearchOpt.SelCond02 == 2)
                        {
                            qs_str_tmp += " (";
                            for (var i = 0; i < AppData.ClassEtc.SearchSiireStr.Length; i++)
                            {
                                qs_str_tmp += ((i == 0) ? "" : " or ") + " upper(z." + AppData.ClassEtc.SearchSiireStr[i] + ") like upper('%" + qs_tmp_ar[j] + "%') ";
                            }
                            qs_str_tmp += ") ";
                        }
                        else
                        {
                            qs_str_tmp += " (";
                            for (var i = 0; i < AppData.ClassEtc.SearchSiireStr.Length; i++)
                            {
                                qs_str_tmp += ((i == 0) ? "" : " or ") + " upper(z." + AppData.ClassEtc.SearchSiireStr[i] + ") like upper('" + qs_tmp_ar[j] + "%') ";
                            }
                            qs_str_tmp += ") ";
                        }
                    }
                }
                sql_query += " and (" + qs_str_tmp + ")";
            }
            sv_cat.Clear();
            return Tuple.Create<string, BizArray>(sql_query, v_para);
        }
        #endregion

        #region Events
        [RelayCommand]
        void DoExecute()
        {
            var ret = OnQueryString(para);
            /* とりあえず空対応 */
            /* para.length、今のところ0のみ対応 */
            /* 特殊ダイアログの際は、文字で！ 現状意味無し */
            if (para.Count == 0)
            {
                para[0] = "0";
            }
            if (para[0] == "A")
            {
                /* マスタ関係 */
                SelSirResult1 = ret;
                ClientLib.ExitDialogResult(this, true);
            }
            else if (para[0] == "0")
            {
                SubDlgSel002ViewModel vm_result = null;
                vm_result = AppData.DlgService.GetSel002(v_mst2, ret.Item1, ret.Item2);
                if (vm_result != null)
                {
                    SelectedValue = new SelValueModel()
                    {
                        Code = (vm_result.SelectedSel002 != null) ? vm_result.SelectedSel002.Code : "",
                        Name = (vm_result.SelectedSel002 != null) ? vm_result.SelectedSel002.Name : ""
                    };
                    SelSirResult0 = vm_result.ret_para;
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

    public partial class SelSirSearchOpt : ObservableObject
    {
        /// <summary>
        /// 仕入先CD - Start Search Condition
        /// </summary>
        [ObservableProperty]
        public string m_StartShiireCD;

        /// <summary>
        /// 仕入先CD - End Search Condition 
        /// </summary>
        [ObservableProperty]
        public string m_EndShiireCD;

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
        /// 仕入先名 - Search Condition
        /// </summary>
        [ObservableProperty]
        public string m_SelShiireName;

        /// <summary>
        /// 締日 - Start Search Condition
        /// </summary>
        [ObservableProperty]
        public string m_StartCloseDate;

        /// <summary>
        /// 締日 - End Search Condition
        /// </summary>
        [ObservableProperty]
        public string m_EndCloseDate;

        /// <summary>
        /// 条件結合 (0 AND, 1 OR)
        /// </summary>
        [ObservableProperty]
        public int m_SelJoinCond;

        /// <summary>
        /// 発注停止 Checkbox Select
        /// </summary>
        [ObservableProperty]
        public bool m_SelOrderChk;

        public SelSirSearchOpt()
        {
            StartShiireCD = string.Empty;
            EndShiireCD = "ZZZZZZZZZZZZZZZZ";
            SelBSChk = false;
            SelCond01 = 1;
            SelCond02 = 2;
            SelShiireName = string.Empty;
            StartCloseDate = string.Empty;
            EndCloseDate = string.Empty;
            SelJoinCond = 0;
            SelOrderChk = true;
        }
    }
}
