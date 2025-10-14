using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Models;
using CvnetClient.Utils;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg01WeekViewModel : BaseViewModel
    {
        [ObservableProperty]
        string? title = "マスタ : 週マスタ";

        [ObservableProperty]
        ObservableCollection<MasterWeekEx>? weekList = null;

        [ObservableProperty]
        Week01SearchOpt? m_SearchOpt;

        [ObservableProperty]
        string? m_Mess2;

        private BizArray para;
        private int Chg_flg = 0;

        [RelayCommand]
        void Init()
        {
            OnInit(null);
        }

        public void OnInit(string[] init_para = null)
        { 
            if (init_para != null) para = new BizArray(init_para);
            else para = new BizArray();

            WeekList = new ObservableCollection<MasterWeekEx>();
            SearchOpt = new Week01SearchOpt();
            BaseWeekList = new Dictionary<int, string>
            {
                { 0, "0 52週" },
                { 1, "1 53週" }
            };
            SearchOpt.SelBaseWeek = BaseWeekList.FirstOrDefault().Key;
        }

        #region Combobox List 
        /// <summary>
        /// 基準週 (0 52週, 1 53週)
        /// </summary>
        [ObservableProperty]
        public Dictionary<int, string>? m_BaseWeekList;
        #endregion

        #region Events
        [RelayCommand]
        void DoSearch()
        {
            if (SearchOpt  == null) return;
            SearchOpt.YearTbCor = 0;
            SearchOpt.WeekNoTbCor = 0;

            var v_para = new BizArray();
            v_para[0] = SearchOpt.SelYear.Trim();

            var wrk_csv2 = OnQuery(v_para.ToArray());
            WeekList?.Clear();

            if (wrk_csv2.Rows.Count == 0)
            {
                v_para = new BizArray();
                v_para[0] = SearchOpt.SelYear.Trim();
                v_para[1] = SearchOpt.SelBaseWeek.ToString();
                wrk_csv2 = OnQuery2(v_para.ToArray());

                SearchOpt.YearTbCor = 1;
                SearchOpt.WeekNoTbCor = 1;
            }

            if (wrk_csv2.Rows.Count > 0)
            {
                var list = (from DataRow dr in wrk_csv2.Rows
                            select new MasterWeekEx
                            {
                                Year = int.TryParse(dr["年"].ToString(), out var _year) ? _year : 0,
                                WeekNo = int.TryParse(dr["週NO"].ToString(), out var _weekNo) ? _weekNo : 0,
                                StartDate = DateTime.TryParseExact(dr["開始日"].ToString(), "yyyyMMdd",
                                            CultureInfo.InvariantCulture, DateTimeStyles.None,
                                            out var _startDate) ? _startDate : new DateTime(1901, 1, 1),
                                StartDayWeek = dr["開始曜日"].ToString() ?? string.Empty,
                                EndDate = DateTime.TryParseExact(dr["終了日"].ToString(), "yyyyMMdd",
                                          CultureInfo.InvariantCulture, DateTimeStyles.None,
                                          out var _endDate) ? _endDate : new DateTime(1901, 1, 1),
                                EndDayWeek = dr["終了曜日"].ToString() ?? string.Empty,
                                Memo = dr["メモ"].ToString() ?? string.Empty
                            });
                WeekList = new ObservableCollection<MasterWeekEx>(list);
            }
        }

        [RelayCommand]
        void DoDelete()
        {
            if (BaseWeekList?.Count == 0) return;

            var wrk_para = new BizArray();
            wrk_para[0] = "MASTER_WEEK";
            wrk_para[1] = "年=" + SearchOpt?.SelYear;
            var ret_csv = AppData.Http?.AspxSqlQuery2("mi_del", wrk_para.ToArray(), "", -1);

            if (!string.IsNullOrEmpty(ret_csv) && ret_csv.Contains("\n"))
            {
                var ret_result = ret_csv?.Split("\n");
                var ret_code = (ret_result?.Length > 1) ? ret_result[0] : "-1";
                if (ret_code == "0") Mess2 = string.Format("削除しました [{0}]", ret_result[1]);
                else Mess2 = string.Format("エラーが起こりました [{0}]", ret_code);
            }
        }

        [RelayCommand]
        void DoExecute()
        {
            if (WeekList?.Count == 0) return;

            string wrk_para1 = "年,週NO,開始日,終了日,メモ";
            foreach (var item in WeekList)
            {
                wrk_para1 += "\n";
                wrk_para1 += string.Format("{0},{1},{2},{3},{4}", 
                                            item.Year, 
                                            item.WeekNo,
                                            item.StartDate.ToString("yyyyMMdd"),
                                            item.EndDate.ToString("yyyyMMdd"),
                                            item.Memo);
            }
            var wrk_para = new BizArray();
            wrk_para[0] = "MASTER_WEEK";
            wrk_para[1] = wrk_para1;
            wrk_para[2] = "2";
            var ret_csv = AppData.Http?.AspxSqlQuery2("mi_csv", wrk_para.ToArray(), "", -1);

            if (!string.IsNullOrEmpty(ret_csv) && ret_csv.Contains("\n"))
            {
                var ret_result = ret_csv?.Split("\n");
                var ret_code = (ret_result?.Length > 1) ? ret_result[0] : "-1";
                if (ret_code == "0") Mess2 = string.Format("更新しました [{0}]", ret_result[1]);
                else Mess2 = string.Format("エラーが起こりました [{0}]", ret_code);
            }
        }

        [RelayCommand]
        void DoExit()
        {
            ClientLib.ExitDialogResult(this, true);
        }
        #endregion

        #region Functions
        private DataTable OnQuery(string[] v_para)
        {
            string sql_query = "SELECT 年,週NO";
            sql_query += ",開始日,SUBSTR(to_char(to_date(開始日),'DAY'),0,1) 開始曜日";
            sql_query += ",終了日,SUBSTR(to_char(to_date(終了日),'DAY'),0,1) 終了曜日";
            sql_query += ",メモ FROM HC$MASTER_WEEK WHERE 年=:1 ORDER BY 年,週NO";
            var ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            return ret_csv;
        }
        private DataTable OnQuery2(string[] v_para)
        {
            string sql_query = string.Empty;

            int week_cnt = 0;
            if (v_para[1] == "0") week_cnt = 52; 
            else week_cnt = 53;

            if (AppData.ClassCvnet.SysMst == null) return new DataTable();

            /* (10.02.18) 【システム管理】週区分 */
            int week_kubun = int.TryParse(AppData.ClassCvnet.SysMst._data.Rows[0][18].ToString(), out var _kubun) ? _kubun : 0;
            int weekbn = week_kubun * -1;

            for (int i = 0; i < week_cnt; i++)
            {
                if (i > 0) sql_query += " UNION ALL ";
                sql_query += "(SELECT ";
                sql_query += v_para[0] + " 年," + (i + 1).ToString() + " 週NO ";
                sql_query += ",trunc( to_date('" + v_para[0] + "-01-01','YYYY-MM-DD'),'IW')+" + (i * 7 + weekbn).ToString() + " 開始日";
                sql_query += ",SUBSTR(to_char(trunc( to_date('" + v_para[0] + "-01-01','YYYY-MM-DD'),'IW')+" + (i * 7 + weekbn).ToString() + ",'DAY'),0,1) 開始曜日";
                sql_query += ",trunc( to_date('" + v_para[0] + "-01-01','YYYY-MM-DD'),'IW')+" + (i * 7 + 6 + weekbn).ToString() + " 終了日";
                sql_query += ",SUBSTR(to_char(trunc( to_date('" + v_para[0] + "-01-01','YYYY-MM-DD'),'IW')+" + (i * 7 + 6 + weekbn).ToString() + ",'DAY'),0,1) 終了曜日";
                sql_query += " FROM dual)";
            }

            var ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            return ret_csv;
        }
        #endregion
    }

    public partial class Week01SearchOpt : ObservableObject
    {
        /// <summary>
        /// 年 - Search Condition
        /// </summary>
        [ObservableProperty]
        public string m_SelYear;

        /// <summary>
        /// 基準週 - Search Condition (0 52週, 1 53週)
        /// </summary>
        [ObservableProperty]
        public int m_SelBaseWeek;

        /// <summary>
        /// 年 CvnetFlexView Column Backgroud Color Setup (0 Default, 1 Red)
        /// </summary>
        [ObservableProperty]
        public int m_YearTbCor;

        /// <summary>
        /// 週NO CvnetFlexView Column Backgroud Color Setup (0 Default, 1 Red)
        /// </summary>
        [ObservableProperty]
        public int m_WeekNoTbCor;
         
        public Week01SearchOpt()
        { 
            SelYear = DateTime.Now.ToString("yyyy");
            SelBaseWeek = 0;
        }
    }

    public partial class MasterWeekEx : MasterWeek
    {
        /// <summary>
        /// 開始曜日
        /// </summary>
        [ObservableProperty]
        public string m_StartDayWeek;

        /// <summary>
        /// 終了曜日
        /// </summary>
        [ObservableProperty]
        public string m_EndDayWeek; 
    }
}
