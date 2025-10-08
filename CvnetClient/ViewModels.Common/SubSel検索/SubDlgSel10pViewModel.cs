using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Models;
using CvnetClient.Utils;
using System.Collections.ObjectModel;
using System.Data;

namespace CvnetClient.ViewModels
{
    public partial class SubDlgSel10pViewModel : BaseViewModel
    {
        [ObservableProperty]
        string title = "選択画面";

        [ObservableProperty]
        ObservableCollection<Sel10pItem>? listSel10p;

        [ObservableProperty]
        Sel10pItem? selectedSel10p;

        [ObservableProperty]
        SelValueModel? selectedValue;

        string sv_qs;
        private BizArray para;
        public BizArray ret_para;

        public void OnInit(string qs = "", string para2 = null, string[] para3 = null, string ken_cd = "", string ken_jyun = "")
        {
            if (para == null) para = new BizArray();

            /* 05.10.05 パラメータセーブ */
            if (string.IsNullOrEmpty(sv_qs)) sv_qs = qs;

            ListSel10p = new ObservableCollection<Sel10pItem>();

            if (!string.IsNullOrEmpty(para2))
            { 
                if (para2 == "p") Title = "ポイントを選択して下さい";
                string sql_query = "select p.ポイントCD,p.名称 from HC$MASTER_POINT p ";
                sql_query += " where EXISTS (" + qs + " and p.ポイントCD = b.ポイントCD  ) ";
                sql_query += " order by ポイントCD ";

                sql_query = AppData.ClassCvnet.GetSqlDisp(sql_query);
                var ret_csv = AppData.Http?.AspxSqlQuery(sql_query);

                var list_csv = new List<Sel10pItem>();
                foreach (DataRow row in ret_csv.Rows)
                {
                    var model = new Sel10pItem
                    {
                        Code = row[0] != DBNull.Value ? row[0].ToString() : string.Empty,
                        Name = row[1] != DBNull.Value ? row[1].ToString() : string.Empty
                    };
                    list_csv.Add(model);
                }
                ListSel10p = new ObservableCollection<Sel10pItem>(list_csv);
            }
        }

        public void OnInit2(string para1 = "")
        {
            if (ListSel10p == null || ListSel10p.Count == 0) return; 
            string sql_query = "select p.ポイントCD,p.名称 from HC$MASTER_POINT p ";
            sql_query += " where EXISTS (" + sv_qs + " and p.ポイントCD = b.ポイントCD  ) and p.ポイントCD >= NVL('" + para1 + "','.')";
            sql_query += " order by ポイントCD ";
            sql_query = AppData.ClassCvnet?.GetSqlDisp(sql_query);
            var ret_csv = AppData.Http?.AspxSqlQuery(sql_query);

            var list_csv = new List<Sel10pItem>();
            foreach (DataRow row in ret_csv.Rows)
            {
                var model = new Sel10pItem
                {
                    Code = row[0] != DBNull.Value ? row[0].ToString() : string.Empty,
                    Name = row[1] != DBNull.Value ? row[1].ToString() : string.Empty
                };
                list_csv.Add(model);
            }
            ListSel10p = new ObservableCollection<Sel10pItem>(list_csv);
        }

        #region Events
        [RelayCommand]
        void NextList()
        {
            if (ListSel10p == null || ListSel10p.Count == 0) return;
            para[1] = ListSel10p.LastOrDefault().Code;
            OnInit2(para[1]);
        }

        [RelayCommand]
        void TopList()
        {
            para[1] = "";
            OnInit2();
        }

        [RelayCommand]
        void DoSearch()
        { 
            if (SelectedSel10p == null) return;
            SelectedValue = new SelValueModel()
            {
                Code = SelectedSel10p.Code,
                Name = SelectedSel10p.Name
            };
            ret_para = new BizArray();
            ret_para[0] = SelectedSel10p.Code;
            ret_para[1] = SelectedSel10p.Name;
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
