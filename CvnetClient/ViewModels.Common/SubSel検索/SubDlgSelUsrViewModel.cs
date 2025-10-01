using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Class;
using CvnetClient.Models;
using CvnetClient.Utils;

namespace CvnetClient.ViewModels
{
    public partial class SubDlgSelUsrViewModel : BaseViewModel
    {
        [ObservableProperty]
        ListFlexData listFlexData = new ListFlexData();

        [ObservableProperty]
        SelUsrSearchOpt? m_SearchOpt;

        private BizArray para;
        private BizArray sv_cat;
        private BizArray wrk_jodai; /* セールマスタ参照配列 */

        private string Mst_sql = ""; /* バインド無し */
        private string Mst_sql2 = ""; /* バインド付き para.length>0の場合 */
        private string v_mst2 = "";

        [ObservableProperty]
        SelValueModel? selectedValue; /* Return value for CvnetBtListView */
        public BizArray SelUsrResult0;
        public Tuple<string, BizArray> SelUsrResult1; /* Default return result method, datatype depends on DoExecute() */
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
                cnt_start = 3,
                flag = 3
            };
            SearchOpt = new SelUsrSearchOpt();  

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
            #endregion

            /* 付加SQL */
            Mst_sql = string.Empty; Mst_sql2 = string.Empty;
            if (AppData.ClassCvnet.MstDialog.ContainsKey(v_mst))
            {
                MstItem add = AppData.ClassCvnet.MstDialog[v_mst]; 
                if (add.v_para.Length > 0)
                    Mst_sql = add.v_para[0] + " and ";
                if (add.v_para.Length > 1 && wrk_para != null)
                    Mst_sql2 = add.v_para[1] + " and ";
            }

            /* 範囲初期値 v_mst保存 */
            if (init_para != null && init_para.Length > 0) SearchOpt.StartShainCD = init_para[0];
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
        #endregion

        #region Functions
        /// <summary>
        /// Generate query from current form
        /// </summary>
        /// <returns>Item 1: Query, Item 2: Parameters </returns>
        Tuple<string, BizArray> OnQueryString(BizArray param = null)
        {
            var v_para = new BizArray();
            string sql_query = "select 社員CD from hc$master_shain z where ";

            #region Add or Remove 就業FLG query (Based on 就業FLG CheckBox)
            int idx = -1; //Search Index
            while ((idx = Mst_sql.IndexOf("就業FLG", idx + 1, StringComparison.Ordinal)) >= 0)
            {
                if (SearchOpt?.SelEmployChk != true)
                {
                    Mst_sql = Mst_sql.Remove(idx, 14).Insert(idx, new string(' ', 14));
                }
            }
            // Once loop ends, if checkbox is selected:
            if (SearchOpt?.SelEmployChk == true) sql_query += "就業FLG='0' and ";
            #endregion

            sql_query += Mst_sql;
            int cnt = 1;
            /* 「>>」押下の際に、v_para2が入っているとき対応（請求など）。数字のみ有効 */
            if (param != null && param.Count > 0)
            {
                decimal param_1 = decimal.TryParse(param[0], out var para1) ? para1 : 0;
                if (param_1 < 9999999999999999999999999999m && param_1 != 0) {
                    for (int i = 0; i < param.Count; i++)
                    {
                        v_para[v_para.Count] = param[i];
                        cnt++;
                        sql_query += Mst_sql2;
                    } 
                }
            }
            sql_query += " 社員CD between :" + cnt.ToString() + " and :" + (cnt + 1).ToString() + "";
            v_para[v_para.Count] = SearchOpt.StartShainCD;
            v_para[v_para.Count] = SearchOpt.EndShainCD;
            cnt = cnt + 2;

            sql_query +=  ListFlexData.GetQueryStr(v_para, cnt, SearchOpt.SelJoinCond); 
            
            if (!string.IsNullOrEmpty(SearchOpt.SelShainName))
            {
                var qs_tmp = SearchOpt.SelShainName;
                var qs_tmp_ar = new BizArray();
                if (SearchOpt.SelCond01 == 2) {
                    qs_tmp_ar[0] = qs_tmp;
                } else {
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
                            for (int i = 0; i < AppData.ClassEtc.SearchShainStr.Length; i++)
                            {
                                qs_str_tmp += ((i == 0) ? "" : " or ") + " z." + AppData.ClassEtc.SearchShainStr[i] + " = '" + qs_tmp_ar[j] + "' ";
                            }
                            qs_str_tmp += ") ";
                        }
                        else if (SearchOpt.SelCond02 == 2)
                        {
                            qs_str_tmp += " (";
                            for (var i = 0; i < AppData.ClassEtc.SearchShainStr.Length; i++)
                            {
                                qs_str_tmp += ((i == 0) ? "" : " or ") + " z." + AppData.ClassEtc.SearchShainStr[i] + " like '%" + qs_tmp_ar[j] + "%' ";
                            }
                            qs_str_tmp += ") ";
                        }
                        else
                        {
                            qs_str_tmp += " (";
                            for (var i = 0; i < AppData.ClassEtc.SearchShainStr.Length; i++)
                            {
                                qs_str_tmp += ((i == 0) ? "" : " or ") + " z." + AppData.ClassEtc.SearchShainStr[i] + " like '" + qs_tmp_ar[j] + "%' ";
                            }
                            qs_str_tmp += ") ";
                        }
                    }
                    else 
                    {
                        if (SearchOpt.SelCond02 == 1)
                        {
                            qs_str_tmp += " (";
                            for (var i = 0; i < AppData.ClassEtc.SearchShainStr.Length; i++)
                            {
                                qs_str_tmp += ((i == 0) ? "" : " or ") + " upper(z." + AppData.ClassEtc.SearchShainStr[i] + ") = upper('" + qs_tmp_ar[j] + "') ";
                            }
                            qs_str_tmp += ") ";
                        }
                        else if (SearchOpt.SelCond02 == 2)
                        {
                            qs_str_tmp += " (";
                            for (var i = 0; i < AppData.ClassEtc.SearchShainStr.Length; i++)
                            {
                                qs_str_tmp += ((i == 0) ? "" : " or ") + " upper(z." + AppData.ClassEtc.SearchShainStr[i] + ") like upper('%" + qs_tmp_ar[j] + "%') ";
                            }
                            qs_str_tmp += ") ";
                        }
                        else
                        {
                            qs_str_tmp += " (";
                            for (var i = 0; i < AppData.ClassEtc.SearchShainStr.Length; i++)
                            {
                                qs_str_tmp += ((i == 0) ? "" : " or ") + " upper(z." + AppData.ClassEtc.SearchShainStr[i] + ") like upper('" + qs_tmp_ar[j] + "%') ";
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
            if (para.Count == 0)
            {
                para[0] = "0";
            }
            if (para[0] == "A")
            {
                //return result and exit from current form
                SelUsrResult1 = ret;
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
                    SelUsrResult0 = vm_result.ret_para;
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
    /// SelUsrSearchOpt - Select 社員 Search Option
    /// </summary>
    public partial class SelUsrSearchOpt : ObservableObject
    {
        /// <summary>
        /// 社員CD - Start Search Condition
        /// </summary>
        [ObservableProperty]
        public string m_StartShainCD;

        /// <summary>
        /// 社員CD - End Search Condition
        /// </summary>
        [ObservableProperty]
        public string m_EndShainCD;

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
        /// 社員名 Search Condition
        /// </summary>
        [ObservableProperty]
        public string m_SelShainName;

        /// <summary>
        /// 条件結合 (0 AND, 1 OR)
        /// </summary>
        [ObservableProperty]
        public int m_SelJoinCond;

        /// <summary>
        /// 就業 Checkbox Select
        /// </summary>
        [ObservableProperty]
        public bool m_SelEmployChk;

        public SelUsrSearchOpt()
        {
            StartShainCD = string.Empty;
            EndShainCD = "ZZZZZZZZZZZZZZZZ";
            SelBSChk = false;
            SelCond01 = 1;
            SelCond02 = 2;
            SelShainName = string.Empty;
            SelJoinCond = 0;
            SelEmployChk = true;
        }
    }
}
