using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Class;
using CvnetClient.Models;
using CvnetClient.Utils;

namespace CvnetClient.ViewModels
{
    public partial class SubDlgSelPointViewModel : BaseViewModel
    {
        [ObservableProperty]
        ListFlexData listFlexData = new ListFlexData();

        [ObservableProperty]
        SelPointSearchOpt? m_SearchOpt;

        [ObservableProperty]
        string title = "ポイント選択画面";

        private BizArray para; 
        public string v_mst2;
        private string wrk_point;

        [ObservableProperty]
        SelValueModel? selectedValue; /* Return value for CvnetBtListView */
        public BizArray SelPointResult0;
        public string SelPointResult1;
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
                flag = 4
            };
            SearchOpt = new SelPointSearchOpt();

            #region Set ComboList
            Cond01List = new Dictionary<int, string>
            {
                { 1, "0 OR" },
                { 2, "1 AND" }
            };
            SearchOpt.SelCond01 = 1;
            Cond02List = new Dictionary<int, string>
            {
                { 1, "0 完全一致" },
                { 2, "1 部分一致" },
                { 3, "2 前方一致" }
            };
            SearchOpt.SelCond02 = 2;
            PointDivList = new Dictionary<int, string>
            {
                { 1, "0 全店" },
                { 2, "1 店別" },
                { 3, "2 全て" }
            };
            SearchOpt.SelPointDiv = 1; 
            if (para[0] == "1" || para[0] == "5")
            {
                /* キャンペーンの場合 */
                PriorityDivList = new Dictionary<int, string>
                {
                    { 1, "0 全店" },
                    { 2, "1 店別" },
                    { 3, "2 商品全店" },
                    { 4, "3 商品店別" },
                    { 5, "4 全て"}
                };
                SearchOpt.SelPriorityDiv = 5;

                PointDivList = new Dictionary<int, string>
                {
                    { 1, "0 全店" },
                    { 2, "1 店別" },
                    { 3, "2 全て" }
                };
                SearchOpt.SelPointDiv = 1;
                wrk_point = "1";
            }
            else if (para[0] == "2")
            {
                /* ボーナス */
                PriorityDivList = new Dictionary<int, string>
                {
                    { 1, "0 全店" },
                    { 2, "1 店別" },
                    { 3, "2 商品全店" },
                    { 4, "3 商品店別" },
                    { 5, "4 全て"}
                };
                SearchOpt.SelPriorityDiv = 5;
                SearchOpt.PrioDivEnable = 1;

                PointDivList = new Dictionary<int, string>
                {
                    { 1, "0 全店" },
                    { 2, "1 店別" },
                    { 3, "2 全て" }
                };
                SearchOpt.SelPointDiv = 3;
                SearchOpt.PointDivEnable = 1;
                wrk_point = "2";
            }
            else if (para[0] == "3")
            {
                /* 店舗別キャンペーン設定 */
                PriorityDivList = new Dictionary<int, string>
                {
                    { 1, "0 全店" },
                    { 2, "1 店別" },
                    { 3, "2 商品全店" },
                    { 4, "3 商品店別" },
                    { 5, "4 全て"}
                };
                SearchOpt.SelPriorityDiv = 2;
                SearchOpt.PrioDivEnable = 1;
                PointDivList = new Dictionary<int, string>
                {
                    { 1, "0 全店" },
                    { 2, "1 店別" },
                    { 3, "2 全て" }
                };
                SearchOpt.SelPointDiv = 2;
                SearchOpt.PointDivEnable = 1;
                wrk_point = "3";
            }
            else if (para[0] == "4")
            {
                /* 商品店舗別 */
                PriorityDivList = new Dictionary<int, string>
                {
                    { 1, "0 商品全店" },
                    { 2, "1 商品店別" },
                    { 5, "2 全て" }
                };
                SearchOpt.SelPriorityDiv = 5;
                PointDivList = new Dictionary<int, string>
                {
                    { 1, "0 全店" },
                    { 2, "1 店別" },
                    { 3, "2 全て" }
                };
                SearchOpt.SelPointDiv = 1;
                wrk_point = "4";
            }
            #endregion
            /* 範囲初期値 v_mst保存 */
            if (init_para != null && init_para.Length > 0)
                SearchOpt.StartPointCD = init_para[0];
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
        /// 優先区分 
        /// </summary>
        [ObservableProperty]
        public Dictionary<int, string>? m_PriorityDivList;

        /// <summary>
        /// ﾎﾟｲﾝﾄ区分 (全店, 店別, 全て)
        /// </summary>
        [ObservableProperty]
        public Dictionary<int, string>? m_PointDivList;
        #endregion

        #region Functions
        private string OnSqlStr()
        { 
            string SqlWhere1;
            string SqlWhere2;
            string SqlWhere3;
            string PointCdF = SearchOpt.StartPointCD;
            string PoinstCdT = SearchOpt.EndPointCD;
            string Date1 = SearchOpt.StartGrantPd.ToString("yyyyMMdd");
            string Date2 = SearchOpt.EndGrantPd.ToString("yyyyMMdd");
            if (string.IsNullOrEmpty(PointCdF)) PointCdF = ".";
            string pointKbn = string.Empty;
            string yusenKbn = string.Empty;

            if (wrk_point == "1")
            {
                /* キャンペーン*/
                pointKbn = "1";
                if (SearchOpt.SelPriorityDiv == 1) yusenKbn = "0";
                if (SearchOpt.SelPriorityDiv == 2) yusenKbn = "1";
                if (SearchOpt.SelPriorityDiv == 3) yusenKbn = "2";
                if (SearchOpt.SelPriorityDiv == 4) yusenKbn = "3";
            }
            else if (wrk_point == "2")
            {
                /* ボーナス */
                pointKbn = "2";
            }
            else if (wrk_point == "3")
            {
                /* 店別キャンペーン */
                pointKbn = "3";
                yusenKbn = "1";
            }
            else if (wrk_point == "4")
            {
                /* 商品店別ルート*/
                pointKbn = "4";
                if (SearchOpt.SelPriorityDiv == 1)
                {
                    /* 商品店別 */
                    yusenKbn = "2";
                }
                if (SearchOpt.SelPriorityDiv == 2)
                {
                    /* 商品全店 */
                    yusenKbn = "3";
                }
            }

            string sql_query = "";
            sql_query = " SELECT b.ポイントCD ";
            sql_query = sql_query + " FROM HC$MASTER_POINT b WHERE ";
            sql_query = sql_query + " b.ポイントCD between '" + PointCdF + "' and '" + PoinstCdT + "' ";
            sql_query = sql_query + " and ((b.付与開始日 between '" + Date1 + "' and  '" + Date2 + "') ";
            sql_query = sql_query + "  OR ( b.付与終了日 between '" + Date1 + "' and  '" + Date2 + "')) ";

            if (wrk_point == "3" ||  wrk_point == "4")
                sql_query = sql_query + " AND b.ポイント区分 = '1' ";
            else
                sql_query = sql_query + " AND b.ポイント区分 = '" + pointKbn + "' ";

            if (pointKbn == "1")
            {
                /* キャンペーンマスタ*/
                if (SearchOpt.SelPriorityDiv == 5) 
                    sql_query = sql_query + " AND b.優先区分 IN ('0','1','2','3') "; 
                else 
                    sql_query = sql_query + " AND b.優先区分 = '" + yusenKbn + "' "; 
            }
            else if (pointKbn == "3")
            {
                /* 店別キャンペーン */
                sql_query = sql_query + " AND b.優先区分 = '" + yusenKbn + "' ";
                sql_query = sql_query + " AND b.送信FLG = 0 ";
            }
            else if (pointKbn == "4")
            {
                /* 商品店別 */ 
                if (SearchOpt.SelPriorityDiv == 3)
                {
                    sql_query = sql_query + " AND b.優先区分 IN ('2','3') ";
                    sql_query = sql_query + " AND b.送信FLG = 0 ";
                }
                else
                {
                    sql_query = sql_query + " AND b.優先区分 = '" + yusenKbn + "' ";
                    sql_query = sql_query + " AND b.送信FLG = 0 ";
                }
            }

            if (!string.IsNullOrEmpty(SearchOpt.SelPointName))
            { 
                if (SearchOpt.SelCond01 == 1) sql_query = sql_query + " OR  ";
                else sql_query = sql_query + " and ";

                if (SearchOpt.SelCond02 == 1) 
                    sql_query = sql_query + "  b.名称 = '" + SearchOpt.SelPointName + "'";
                else if (SearchOpt.SelCond02 == 2)
                    sql_query = sql_query + "  b.名称  like '%" + SearchOpt.SelPointName + "%' ";
                else if (SearchOpt.SelCond02 == 3)
                    sql_query = sql_query + "  b.名称  like '%" + SearchOpt.SelPointName + "' ";
            }
            return sql_query;
        }
        #endregion

        #region Events
        [RelayCommand]
        void DoExecute()
        {
            if (para == null) return;
            string sql = OnSqlStr();
            /* とりあえず空対応 */
            /* para.length、今のところ0のみ対応 */
            /* 特殊ダイアログの際は、文字で！ 現状意味無し */
            if (para.Count == 0) para[0] = "0";
            if (para[0] == "1")
            {
                /* キャンペーン */
                SelPointResult1 = sql;
                ClientLib.ExitDialogResult(this, true);
            }
            else  
            {
                SubDlgSel10pViewModel vm_result = null;
                vm_result = AppData.DlgService.GetSel10p(sql, "p");
                if (vm_result != null)
                {
                    SelectedValue = new SelValueModel
                    {
                        Code = (vm_result.SelectedSel10p != null) ? vm_result.SelectedSel10p.Code : "",
                        Name = (vm_result.SelectedSel10p != null) ? vm_result.SelectedSel10p.Name : ""
                    };
                    SelPointResult0 = vm_result.ret_para;
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

    public partial class SelPointSearchOpt : ObservableObject
    {
        /// <summary>
        /// ﾎﾟｲﾝﾄCD - Start Search Condition
        /// </summary>
        [ObservableProperty]
        public string m_StartPointCD;

        /// <summary>
        /// ﾎﾟｲﾝﾄCD - End Search Condition
        /// </summary>
        [ObservableProperty]
        public string m_EndPointCD;

        /// <summary>
        /// B/S Checkbox Select
        /// </summary>
        [ObservableProperty]
        public bool m_SelBSChk;

        /// <summary>
        /// Search Condition (1 OR,2 AND)
        /// </summary>
        [ObservableProperty]
        public int m_SelCond01;

        /// <summary>
        /// Search Condition (1 完全一致,2 部分一致,3 前方一致)
        /// </summary>
        [ObservableProperty]
        public int m_SelCond02;

        /// <summary>
        /// ﾎﾟｲﾝﾄ名 - Search Condition
        /// </summary>
        [ObservableProperty]
        public string m_SelPointName;

        /// <summary>
        /// 付与期間 - Start Search Condition
        /// </summary>
        [ObservableProperty]
        public DateTime m_StartGrantPd;

        /// <summary>
        /// 付与期間 - End Search Condition
        /// </summary>
        [ObservableProperty]
        public DateTime m_EndGrantPd;

        /// <summary>
        /// 優先区分 - Search Condition
        /// </summary>
        [ObservableProperty]
        public int m_SelPriorityDiv;

        /// <summary>
        /// 優先区分 Enable Status (0 Enable, 1 Not Enable)
        /// </summary>
        [ObservableProperty]
        public int? m_PrioDivEnable;

        /// <summary>
        /// ﾎﾟｲﾝﾄ区分 - Search Condition (1 全店,2 店別,3 全て)
        /// </summary>
        [ObservableProperty]
        public int m_SelPointDiv;

        /// <summary>
        /// ﾎﾟｲﾝﾄ区分 Enable Status (0 Enable, 1 Not Enable)
        /// </summary>
        [ObservableProperty]
        public int? m_PointDivEnable;

        public SelPointSearchOpt()
        {
            StartPointCD = string.Empty;
            EndPointCD = "ZZZZZZZZZZZZZZZZ";
            SelBSChk = false;
            SelCond01 = 1;
            SelCond02 = 2;
            SelPointName = string.Empty;
            StartGrantPd = new DateTime(1901, 1, 1);
            EndGrantPd = DateTime.Now;
            SelPriorityDiv = 0;
            PrioDivEnable = 0;
            SelPointDiv = 1;
            PointDivEnable = 0;
        }
    }
}
