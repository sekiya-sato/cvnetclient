using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Models;
using CvnetClient.Utils;
using System.Collections.ObjectModel;
using System.Data;

namespace CvnetClient.ViewModels
{
    public partial class SubDlgSel002ViewModel : BaseViewModel
    {
        private string mstname = "";
        private string sql_first = "";
        private string para_first = "";

        private BizArray para;
        private BizArray para2; 

        private int nullflg = 0;

        [ObservableProperty]
        string? title = "選択画面";

        [ObservableProperty]
        string? searchText;

        List<SubDlgSel002Model>? ListSel002Ori;

        [ObservableProperty]
        ObservableCollection<SubDlgSel002Model>? listSel002;

        [ObservableProperty]
        SubDlgSel002Model? selectedSel002;

        [ObservableProperty]
        SelValueModel? selectedValue;

        /// <summary>
        /// Current Dialog return value
        /// </summary>
        public BizArray ret_para;

        public void OnInit(string v_mstname, string sql_query = "", object v_para2 = null)
        {  
            if (v_para2 != null && v_para2 is string[] init_para) para = new BizArray(init_para);
            else para = new BizArray();

            mstname = v_mstname;
            sql_first = sql_query;
            if (v_para2 != null)
            {
                if (v_para2 is string[] para_array)
                {
                    para2 = new BizArray(para_array);
                    para_first = para_array[0];
                }
                else {
                    para2 = new BizArray();
                    nullflg = 1;
                    para_first = "";
                }
            }
            OnInit2(sql_query);

            //Store First Init2 listSel002 Data
            if (ListSel002 != null && ListSel002.Count > 0) 
                ListSel002Ori = new List<SubDlgSel002Model>(ListSel002.ToList());
        }

        void OnInit2(string sql_query, string qs = "", string p_sort = "")
        {
            var ret_csv = new DataTable();

            if (para2 != null && para2.Count > 0) ListSel002.Clear();

            string v_hugo = ">=";
            if (p_sort != "") v_hugo = "<=";

            /* 2010.09.22　法人CD対応 */
            string houjin_str = AppData.ClassCvnet.GetQueryStrHoujin();

            if (mstname.StartsWith("移動倉庫S", StringComparison.Ordinal)) houjin_str = "";

            /* 仕入先 */
            if (mstname.StartsWith("仕入", StringComparison.Ordinal) ||
                mstname.StartsWith("支払", StringComparison.Ordinal) ||
                mstname.StartsWith("メーカー", StringComparison.Ordinal) ||
                mstname.StartsWith("siire", StringComparison.Ordinal))
            {
                sql_query = "select 仕入先CD,仕入先名 from HC$master_siire s where exists (" + sql_query + " and s.仕入先CD=z.仕入先CD)"
                    + ((string.IsNullOrEmpty(qs)) ? "" : " and 仕入先CD" + v_hugo + "'" + qs + "'")
                    + ((AppData.ClassCvnet.config.smtflg == 0) ? "" : " and 発注停止FLG=0 ")
                    + houjin_str
                    + " order by 仕入先CD " + p_sort;
            }
            /* 社員 */
            else if (mstname.StartsWith("担当", StringComparison.Ordinal) ||
                     mstname.StartsWith("営業担当", StringComparison.Ordinal) ||
                     mstname.StartsWith("店舗担当", StringComparison.Ordinal))
            {
                sql_query = "select 社員CD,名前 from HC$master_shain s where exists (" + sql_query + " and s.社員CD=z.社員CD)"
                    + ((string.IsNullOrEmpty(qs)) ? "" : " and 社員CD" + v_hugo + "'" + qs + "'") + " order by 社員CD " + p_sort;
            }
            /* 得意先 */
            else
            {
                sql_query = "select 得意先CD,得意先名 from HC$master_tokui t where exists (" + sql_query + " and t.得意先CD=z.得意先CD)"
                    + ((string.IsNullOrEmpty(qs)) ? "" : " and 得意先CD" + v_hugo + "'" + qs + "'")
                    + ((AppData.ClassCvnet.config.smtflg == 0) ? "" : " and 出荷停止FLG=0 ")
                    + houjin_str
                    + " order by 得意先CD " + p_sort;
            }
            sql_query = AppData.ClassCvnet.GetSqlDisp(sql_query);
            ret_csv = AppData.Http?.AspxSqlQuery(sql_query, para2.ToArray());
            ListSel002 = new ObservableCollection<SubDlgSel002Model>();
            foreach (DataRow row in ret_csv.Rows)
            {
                var model = new SubDlgSel002Model()
                {
                    Code = row[0] != DBNull.Value ? row[0].ToString() : string.Empty,
                    Name = row[1] != DBNull.Value ? row[1].ToString() : string.Empty
                };
                ListSel002.Add(model);
            }

            if (ListSel002.Count > 0 && p_sort != "")
                ListSel002 = new ObservableCollection<SubDlgSel002Model>(ListSel002.OrderBy(x => x.Code)); 

            Title = mstname + "を選択してください";
        }

        #region Events
        [RelayCommand]
        void PrevList()
        {
            if (ListSel002 == null || ListSel002.Count == 0) return;
            OnInit2(sql_first, ListSel002.FirstOrDefault().Code, "desc");
        } 
        [RelayCommand]
        void NextList()
        {
            if (ListSel002 == null || ListSel002.Count == 0) return;
            OnInit2(sql_first, ListSel002.LastOrDefault().Code);
        } 
        [RelayCommand]
        void TopList()
        {
            para2[0] = para_first;
            OnInit2(sql_first);
        }
        [RelayCommand]
        void NameSearch()
        { 
            if (string.IsNullOrEmpty(SearchText))
                ListSel002 = new ObservableCollection<SubDlgSel002Model>(ListSel002Ori);
            else ListSel002 = new ObservableCollection<SubDlgSel002Model>(ListSel002.Where(x => x.Name.Contains(SearchText)));
        }
        [RelayCommand]
        void DoSearch()
        {
            if (SelectedSel002 == null) return;

            SelectedValue = new SelValueModel()
            {
                Code = SelectedSel002.Code,
                Name = SelectedSel002.Name
            }; 
            ret_para = new BizArray();
            ret_para[0] = SelectedSel002.Code;
            ret_para[1] = SelectedSel002.Name;

            //confirm and close window
            ClientLib.ExitDialogResult(this, true);
        }
        [RelayCommand]
        void DoExit()
        {
            //confirm and close window
            ClientLib.ExitDialogResult(this, true);
        }
        #endregion
    }
}
