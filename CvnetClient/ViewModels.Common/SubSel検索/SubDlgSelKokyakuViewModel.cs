using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Models;
using CvnetClient.Utils;
using System.Collections.ObjectModel;

namespace CvnetClient.ViewModels
{
    public partial class SubDlgSelKokyakuViewModel : BaseViewModel
    {
        [ObservableProperty]
        ObservableCollection<KokyakuFlexItem>? m_CvnetFlexList;

        [ObservableProperty]
        SelCustSearchOpt? m_SearchOpt;

        [ObservableProperty]
        string title = "顧客選択画面";

        private BizArray para;

        SelValueModel? SelectedValue; /* Return value for CvnetBtListView */
        public BizArray SelKokyakuResult0;
        public BizArray SelKokyakuResult1; 
        public void OnInit(string v_mstname, string[] init_para = null, string[] v_para2 = null)
        {
            OnInit(v_mstname, init_para);
        }

        public void OnInit(string v_mstname, string[] init_para = null)
        {
            if(init_para != null) para = new BizArray(init_para);
            else para = new BizArray();

            CvnetFlexList = new ObservableCollection<KokyakuFlexItem>();
            SearchOpt = new SelCustSearchOpt();

            #region Set ComboList
            SearchList = new Dictionary<string, string>
            {
                { "郵便番号", "郵便番号" },
                { "住所1", "住所1" },
                { "住所2", "住所2" },
                { "住所3", "住所3" },
                { "顧客区分", "顧客区分" },
                { "性別", "性別" },
                { "顧客ランク", "顧客ランク" },
                { "ポイントランク", "ポイントランク" },
                { "誕生日", "誕生日" },
                { "EMAIL", "EMAIL" },
                { "名称CD01", "名称CD01" },
                { "名称CD02", "名称CD02" },
                { "名称CD03", "名称CD03" },
                { "名称CD04", "名称CD04" },
                { "名称CD05", "名称CD05" },
                { "名称CD06", "名称CD06" },
                { "名称CD07", "名称CD07" },
                { "名称CD08", "名称CD08" },
                { "名称CD09", "名称CD09" },
                { "名称CD10", "名称CD10" }
            };
            CondList = new Dictionary<string, string>
            {
                { "完全一致", "完全一致" },
                { "前方一致", "前方一致" },
                { "複数指定", "複数指定" },
                { "範囲指定", "範囲指定" }
            };
            #endregion

            if (para.Count > 1)
            {
                if (para[1] == "1" && para[2] != "")
                {
                    SearchOpt.StartStoreCD = para[2];
                    SearchOpt.EndStoreCD = para[2];
                    SearchOpt.IsStoreVisible = 1;
                }
            }
        }

        #region Combobox List
        /// <summary>
        /// 検索項目
        /// </summary>
        [ObservableProperty]
        public Dictionary<string, string>? m_SearchList;

        /// <summary>
        /// 検索条件
        /// </summary>
        [ObservableProperty]
        public Dictionary<string, string>? m_CondList;
        #endregion

        #region Functions
        private string OnSqlStr()
        {
            string Sqlstr = string.Empty;

            string SqlWhere1 = string.Empty;
            string SqlWhere2 = string.Empty;
            string SqlWhere3 = string.Empty;
            string SqlWhereA = string.Empty;
            string SqlWhereB = string.Empty;
            //string Date1 = AppData.ClassSatoo.GetVdateValue(SearchOpt.StartRegDate.ToString()).ToString();
            //string Date2 = AppData.ClassSatoo.GetVdateValue(SearchOpt.EndRegDate.ToString()).ToString();
            string Tenpo1 = (string.IsNullOrEmpty(SearchOpt.StartStoreCD)) ? "." : SearchOpt.StartStoreCD;
            string Tenpo2 = (string.IsNullOrEmpty(SearchOpt.EndStoreCD)) ? "." : SearchOpt.EndStoreCD;

            if (CvnetFlexList != null)
            {
                foreach (var item in CvnetFlexList)
                {
                    if (string.IsNullOrEmpty(item.FromValue) && string.IsNullOrEmpty(item.ToValue))
                        item.WhereClause = string.Empty;
                    else
                    {
                        if (item.SelCond == "完全一致")
                        {
                            item.WhereClause = " ( " + item.SelSearch + " = '" + item.FromValue + "' ) AND ";
                            item.IsToEnable = 1;
                        }
                        if (item.SelCond == "前方一致")
                        {
                            item.WhereClause = " ( " + item.SelSearch + " LIKE '" + item.FromValue + "%' ) AND ";
                            item.IsToEnable = 1;
                        }
                        if (item.SelCond == "複数指定")
                        {
                            string t2 = string.Empty;
                            if (!string.IsNullOrEmpty(item.ToValue))
                                t2 = ",'" + item.ToValue + "'";
                            item.WhereClause = " ( " + item.SelSearch + " IN ('" + item.FromValue + "'" + t2 + ") ) AND ";
                            item.IsToEnable = 0;
                        }
                        if (item.SelCond == "範囲指定")
                        {
                            item.WhereClause = " ( " + item.SelSearch + " between '" + item.FromValue + "' and '" + item.ToValue + "' ) AND ";
                            item.IsToEnable = 0;
                        }
                    }
                    if (!string.IsNullOrEmpty(item.WhereClause)) {
                        SqlWhereA += item.WhereClause;
                    }
                }
                if(string.IsNullOrEmpty(SearchOpt.SelCustCD)) 
                    SqlWhere1 = string.Empty;
                else if (SearchOpt.SrchNameCond.Trim() == "3")
                    SqlWhere1 = " 顧客CD like '%" + SearchOpt.SelCustCD.Trim() + "' AND ";

                if(string.IsNullOrEmpty (SearchOpt.SelCustName))
                    SqlWhere1 = string.Empty;
                else 
                    SqlWhere2 = "(顧客名 LIKE '%" + SearchOpt.SelCustName.Trim() + "%' OR カナ LIKE '%" + SearchOpt.SelCustName.Trim() + "%') AND ";

                if (string.IsNullOrEmpty(SearchOpt.Tel)) 
                    SqlWhere3 = string.Empty;
                else 
                    SqlWhere3 = " translate(TEL1,'0123456789-()','0123456789') LIKE '%" + SearchOpt.Tel + "%' AND ";

                if (SearchOpt.SrchNameCond == "完全一致" && SearchOpt.SelCustCD != "") 
                    SqlWhereB = " 顧客CD = '" + SearchOpt.SelCustCD.Trim() + "' AND "; 
                else if (SearchOpt.SrchNameCond == "部分一致" && SearchOpt.SelCustCD != "") 
                    SqlWhereB = " 顧客CD LIKE '%" + SearchOpt.SelCustCD.Trim() + "%' AND ";
                else if (SearchOpt.SrchNameCond == "前方一致" && SearchOpt.SelCustCD != "") 
                    SqlWhereB = " 顧客CD LIKE '" + SearchOpt.SelCustCD.Trim() + "%' AND "; 
                else if (SearchOpt.SrchNameCond == "後方一致" && SearchOpt.SelCustCD != "") 
                    SqlWhereB = " 顧客CD like '%" + SearchOpt.SelCustCD.Trim() + "' AND ";

                Sqlstr = "SELECT 顧客CD FROM HC$MASTER_KOKYAKU K WHERE ";
                if (SearchOpt.IsMemberChk) Sqlstr += "退会FLG=0 and "; 
                Sqlstr += SqlWhere1 + SqlWhere2 + SqlWhere3 + SqlWhereA;
                Sqlstr += SqlWhereB;
                /* 2013/07/03 kosugi #6109 顧客累計更新バッチ、及び顧客累計値再更新画面が正しく動作しない MOD START */
                Sqlstr += " (会員登録日 BETWEEN '" + SearchOpt.StartRegDate + "' AND '" + SearchOpt.EndRegDate + "'";
                if (SearchOpt.StartRegDate.ToString("yyyyMMdd") == "19010101") Sqlstr += " or (会員登録日 = '.')";
                Sqlstr += ") AND (最終更新日 BETWEEN '" + SearchOpt.StartUpDate + "' AND '" + SearchOpt.EndUpDate + "'";
                if (SearchOpt.StartUpDate.ToString("yyyyMMdd") == "19010101") Sqlstr += " or (最終更新日 = '.')";
                /* 2013/07/03 kosugi #6109 顧客累計更新バッチ、及び顧客累計値再更新画面が正しく動作しない MOD END */
                Sqlstr += ") AND (店舗CD BETWEEN '" + Tenpo1 + "' AND '" + Tenpo2 + "' )";
            }
            return Sqlstr;
        }
        #endregion

        #region Events
        [RelayCommand]
        void DoStartStore(object value)
        {
            if (value == null || SearchOpt == null) return;
            var get_value = (SelValueModel)value;
            SearchOpt.StartStoreCD = get_value.Code;
            SearchOpt.EndStoreCD = get_value.Code;
        }
        
        [RelayCommand]
        void DoEndStore(object value)
        {
            if (value == null || SearchOpt == null) return;
            var get_value = (SelValueModel)value;
            SearchOpt.EndStoreCD = get_value.Code;
        }

        [RelayCommand]
        void DoAddRow()
        {
            if (CvnetFlexList == null) return;
            int nextNo = CvnetFlexList.Count + 1;
            CvnetFlexList.Add(new KokyakuFlexItem
            {
                No = nextNo,
                SelSearch = string.Empty,
                SelCond = string.Empty,
                FromValue = string.Empty,
                ToValue = string.Empty
            }); 
        }

        [RelayCommand]
        void DoDeleteRow(KokyakuFlexItem? value)
        {
            if(value != null && CvnetFlexList != null) 
                CvnetFlexList.Remove(value); 
        }

        [RelayCommand]
        void DoExecute()
        {
            string sql = OnSqlStr();
            if (para.Count > 0 && para[0] == "1")
            {
                var ret_para = new BizArray();
                ret_para[0] = sql;
                SelKokyakuResult1 = ret_para;
                ClientLib.ExitDialogResult(this, true);
            }
            else 
            { 
                SubDlgSel10ViewModel vm_result = null;
                vm_result = AppData.DlgService.GetSel10(sql, "k");
                if (vm_result != null)
                {
                    SelectedValue = new SelValueModel()
                    {
                        Code = (vm_result.SelectedSel10 != null) ? vm_result.SelectedSel10.Col01 : "",
                        Name = (vm_result.SelectedSel10 != null) ? vm_result.SelectedSel10.Col02 : ""
                    };
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

    public partial class KokyakuFlexItem : ObservableObject
    {
        /// <summary>
        /// 行
        /// </summary>
        [ObservableProperty]
        public int m_No;

        /// <summary>
        /// 検索項目
        /// </summary>
        [ObservableProperty]
        public string m_SelSearch;

        /// <summary>
        /// 検索条件
        /// </summary>
        [ObservableProperty]
        public string m_SelCond;

        /// <summary>
        /// 検索文字列 (FROM)
        /// </summary>
        [ObservableProperty]
        public string m_FromValue;

        /// <summary>
        /// 検索文字列 (TO)
        /// </summary>
        [ObservableProperty]
        public string m_ToValue;

        /// <summary>
        /// 検索文字列 (TO) Enable Status (0 Enable, 1 Not Enable)
        /// </summary>
        [ObservableProperty]
        public int m_IsToEnable;

        /// <summary>
        /// Query Where Condition
        /// </summary>
        [ObservableProperty]
        public string m_WhereClause;
    }

    public partial class SelCustSearchOpt : ObservableObject
    {
        /// <summary>
        /// 顧客CD
        /// </summary>
        [ObservableProperty]
        public string m_SelCustCD;

        /// <summary>
        /// 顧客名
        /// </summary>
        [ObservableProperty]
        public string m_SelCustName;

        /// <summary>
        /// Search 顧客名 Condition (0 完全一致, 1 部分一致, 2 前方一致, 3 後方一致)
        /// </summary>
        [ObservableProperty]
        public string m_SrchNameCond;

        /// <summary>
        /// 電話番号
        /// </summary>
        [ObservableProperty]
        public string m_Tel;

        /// <summary>
        /// 登録日 - Start Search Condition
        /// </summary> 
        [ObservableProperty]
        public DateTime m_StartRegDate;

        /// <summary>
        /// 登録日 - End Search Condition
        /// </summary>
        [ObservableProperty]
        public DateTime m_EndRegDate;

        /// <summary>
        /// 更新日 - Start Search Condition
        /// </summary>
        [ObservableProperty]
        public DateTime m_StartUpDate;

        /// <summary>
        /// 更新日 - End Search Condition
        /// </summary>
        [ObservableProperty]
        public DateTime m_EndUpDate;

        /// <summary>
        /// 店舗CD -  Start Search Condition
        /// </summary>
        [ObservableProperty]
        public string m_StartStoreCD;

        /// <summary>
        /// ログイン店舗を表示 (0 店舗CD Visible, 1 店舗CD Not Visible)
        /// </summary>
        [ObservableProperty]
        public int m_IsStoreVisible;

        /// <summary>
        /// 店舗CD - End Search Condition
        /// </summary>
        [ObservableProperty]
        public string m_EndStoreCD;

        /// <summary>
        /// 入会中
        /// </summary>
        [ObservableProperty]
        public bool m_IsMemberChk;

        public SelCustSearchOpt()
        { 
            SelCustCD = string.Empty;
            SrchNameCond = string.Empty;
            SelCustName = string.Empty;
            Tel = string.Empty;
            StartRegDate = new DateTime(1901, 1, 1);
            EndRegDate = DateTime.Now;
            StartUpDate = new DateTime(1901, 1, 1);
            EndUpDate = DateTime.Now;
            StartStoreCD = string.Empty;
            EndStoreCD  = "ZZZZZZZZZZ";
            IsMemberChk = true;
        }
    }
}
