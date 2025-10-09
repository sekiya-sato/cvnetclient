using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Models;
using CvnetClient.Utils;
using System.Collections.ObjectModel;
using System.Data;

namespace CvnetClient.ViewModels
{
    public partial class SubDlgSel10ViewModel : BaseViewModel
    {
        [ObservableProperty]
        ObservableCollection<Sel10FlexItem>? m_Sel01FlexList;

        [ObservableProperty]
        Sel10FlexItem? m_SelectedSel10;

        [ObservableProperty]
        string? title = "選択画面";

        private BizArray para;
        private BizArray para2;

        private string sv_qs;
        private BizArray sv_ret_para;
        private string sv_ret_para2;
        private BizArray sv_ret_para3;

        SelValueModel? SelectedValue; /* Return value for CvnetBtListView */
        public BizArray SelKokyakuResult0;
        public BizArray SelKokyakuResult1;

        public void OnInit(string qs, string para2 = "", string[] para3 = null, string ken_cd = "", string ken_jyun = "")
        {
            /* 05.10.05 パラメータセーブ */
            if (string.IsNullOrEmpty(sv_qs))
            {
                sv_qs = qs;
                if (!string.IsNullOrEmpty(para2)) sv_ret_para2 = para2;
                if (para3 != null) sv_ret_para3 = new BizArray(para3);
                else sv_ret_para3 = new BizArray();

            }

            if (para2.ToLower().Trim() == "t")
            {
                Title = "得意先選択画面";
                var para_d = new BizArray();
                /* 05.12.28変更 住所対応 */
                string sql_query = "select A.得意先CD,A.得意先名,A.TEL,A.郵便番号,A.住所1||A.住所2||A.住所3 住所,A.取引金額,A.取引数 ";
                sql_query += " from HC$MASTER_TOKUI A";
                sql_query += " where A.得意先CD in (" + qs + ")";

                /* 頁切り替え対応　05.10.05 */
                if (!string.IsNullOrEmpty(ken_cd))
                {
                    if (!string.IsNullOrEmpty(ken_jyun))
                        sql_query += " and A.得意先CD <= '" + ken_cd + "'";
                    else
                        sql_query += " and A.得意先CD >= '" + ken_cd + "'";
                }

                sql_query += " order by ";
                if (para3 != null)
                {
                    if (para3[0] == "0") sql_query += "A.取引金額 ";
                    else sql_query += "A.取引数 ";

                    if (!string.IsNullOrEmpty(ken_jyun))
                    {
                        if (para3[1] == "0") sql_query += "asc, ";
                        else sql_query += "desc, ";
                    }
                    else
                    {
                        if (para3[1] == "0") sql_query += "desc, ";
                        else sql_query += "asc, ";
                    }
                }

                if (!string.IsNullOrEmpty(ken_jyun)) sql_query += " A.得意先CD DESC";
                else sql_query += " A.得意先CD";
                sql_query = AppData.ClassCvnet.GetSqlDisp(sql_query);
                var data_csv = AppData.Http?.AspxSqlQuery(sql_query, para_d.ToArray());
                if (data_csv == null || data_csv.Rows.Count == 0) return;
                var ret_csv = GlobalFunc.ConvertDataTableToList<Sel10FlexItem>(data_csv);
                if (para3 != null)
                {
                    if (para3[0] == "0" && para3[1] == "0")
                        ret_csv = ret_csv.OrderByDescending(x => x.col06).ToList();
                    else if (para3[0] == "0" && para3[1] == "1")
                        ret_csv = ret_csv.OrderBy(x => x.col06).ToList();
                    else if (para3[0] == "1" && para3[1] == "0")
                        ret_csv = ret_csv.OrderByDescending(x => x.col07).ToList();
                    else if (para3[0] == "1" && para3[1] == "1")
                        ret_csv = ret_csv.OrderByDescending(x => x.col07).ToList();
                }
                Sel01FlexList = new ObservableCollection<Sel10FlexItem>(ret_csv);
            }

            if (para2.ToLower().Trim() == "s")
            {
                Title = "仕入先選択画面";
                var para_d = new BizArray();
                /* 05.12.28変更 住所対応 */
                string sql_query = "select A.仕入先CD,A.仕入先名,A.TEL,A.郵便番号,A.住所1||A.住所2||A.住所3 住所,A.取引金額,A.取引数  ";
                sql_query += " from HC$MASTER_SIIRE A";
                sql_query += " where  A.仕入先CD in (" + qs + ")";

                /* 頁切り替え対応　05.10.05 */
                if (!string.IsNullOrEmpty(ken_cd))
                {
                    if (!string.IsNullOrEmpty(ken_jyun))
                        sql_query += " and A.仕入先CD <= '" + ken_cd + "'";
                    else
                        sql_query += " and A.仕入先CD >= '" + ken_cd + "'";
                }

                sql_query += " order by ";
                if (para3 != null)
                {
                    if (para3[0] == "0") sql_query += "A.取引金額 ";
                    else sql_query += "A.取引数 ";

                    if (!string.IsNullOrEmpty(ken_jyun))
                    {
                        if (para3[1] == "0") sql_query += "asc, ";
                        else sql_query += "desc, ";
                    }
                    else {
                        if (para3[1] == "0") sql_query += "asc, ";
                        else sql_query += "asc, ";
                    }
                }
                if (!string.IsNullOrEmpty(ken_jyun)) sql_query += "A.仕入先CD DESC";
                else sql_query += "仕入先CD";

                sql_query = AppData.ClassCvnet.GetSqlDisp(sql_query);
                var data_csv = AppData.Http?.AspxSqlQuery(sql_query, para_d.ToArray());
                if (data_csv == null || data_csv.Rows.Count == 0) return;
                var ret_csv = GlobalFunc.ConvertDataTableToList<Sel10FlexItem>(data_csv);
                if (para3 != null)
                {
                    if (para3[0] == "0" && para3[1] == "0")
                        ret_csv = ret_csv.OrderByDescending(x => x.col06).ToList();
                    else if (para3[0] == "0" && para3[1] == "1")
                        ret_csv = ret_csv.OrderBy(x => x.col06).ToList();
                    else if (para3[0] == "1" && para3[1] == "0")
                        ret_csv = ret_csv.OrderByDescending(x => x.col07).ToList();
                    else if (para3[0] == "1" && para3[1] == "1")
                        ret_csv = ret_csv.OrderByDescending(x => x.col07).ToList();
                }
                Sel01FlexList = new ObservableCollection<Sel10FlexItem>(ret_csv);
            }

            if (para2.ToLower().Trim() == "k")
            {
                Title = "顧客選択画面";
                /*shimada 2015.03.10 #15007 顧客CD検索でヒットが違う人になる*/
                qs += "AND A.顧客CD = K.顧客CD";
                var para_d = new BizArray();
                /* 05.12.28変更 住所対応 */
                string sql_query = "select A.顧客CD,A.顧客名,A.TEL1,A.郵便番号,A.住所1||A.住所2||A.住所3 住所 ";
                sql_query += " from HC$MASTER_KOKYAKU A";
                sql_query += " where EXISTS (" + qs + ")";

                /* 頁切り替え対応　05.10.05 */
                if (!string.IsNullOrEmpty(ken_cd))
                {
                    if (!string.IsNullOrEmpty(ken_jyun))
                        sql_query += " and A.顧客CD <= '" + ken_cd + "'";
                    else
                        sql_query += " and A.顧客CD >= '" + ken_cd + "'";
                }
                if (!string.IsNullOrEmpty(ken_jyun))
                    sql_query += " order by A.顧客CD DESC";
                else
                    sql_query += " order by 顧客CD";

                var data_csv = AppData.Http?.AspxSqlQuery(sql_query, para_d.ToArray());
                if (data_csv == null || data_csv.Rows.Count == 0) return;
                var ret_csv = GlobalFunc.ConvertDataTableToList<Sel10FlexItem>(data_csv);
                Sel01FlexList = new ObservableCollection<Sel10FlexItem>(ret_csv.OrderBy(x => x.col01));
            }
        }

        #region Events
        [RelayCommand]
        void PrevList()
        {
            if (Sel01FlexList == null || Sel01FlexList.Count == 0) return;
            var csv_sv = Sel01FlexList.OrderBy(x => x.col01).ToList();
            string ken_cd = csv_sv[0].Col01;
            OnInit(sv_qs, sv_ret_para2, sv_ret_para3.ToArray(), ken_cd, "U");
        }

        [RelayCommand]
        void NextList()
        {
            if (Sel01FlexList == null || Sel01FlexList.Count == 0) return;
            var csv_sv = Sel01FlexList.OrderBy(x => x.col01).ToList();
            string ken_cd = csv_sv.LastOrDefault().Col01;
            OnInit(sv_qs, sv_ret_para2, sv_ret_para3.ToArray(), ken_cd);
        }

        [RelayCommand]
        void DoSearch()
        { 
            if (SelectedSel10 == null) return;
            SelectedValue = new SelValueModel()
            {
                Code = SelectedSel10.Col01,
                Name = SelectedSel10.Col02
            };
            ClientLib.ExitDialogResult(this, true);
        }

        [RelayCommand]
        void DoExit()
        {
            ClientLib.ExitDialogResult(this, true);
        }
        #endregion
    }

    public partial class Sel10FlexItem : ObservableObject
    {
        [ObservableProperty] public string? col01;
        [ObservableProperty] public string? col02;
        [ObservableProperty] public string? col03;
        [ObservableProperty] public string? col04;
        [ObservableProperty] public string? col05;
        [ObservableProperty] public string? col06;
        [ObservableProperty] public string? col07;
        [ObservableProperty] public string? col08;
        [ObservableProperty] public string? col09;
        [ObservableProperty] public string? col10;
        [ObservableProperty] public string? col11;
        [ObservableProperty] public string? col12;
        [ObservableProperty] public string? col13;
        [ObservableProperty] public string? col14;
        [ObservableProperty] public string? col15;
        [ObservableProperty] public string? col16;
    }
}
