using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Models;
using CvnetClient.Utils;
using System.Collections.ObjectModel;
using System.Data;

namespace CvnetClient.ViewModels
{
    public partial class SubDlgSelybnViewModel : BaseViewModel
    {
        [ObservableProperty]
        ObservableCollection<SelybnFlexItem>? m_SelybnFlexList;

        [ObservableProperty]
        SelybnFlexItem? m_SelectedSelybn;

        [ObservableProperty]
        string? title = "郵便住所選択画面";

        private BizArray para;

        SelybnModel? SelectedValue; /* Return value for CvnetBtListView */

        public void OnInit(object? init_para = null)
        {
            string wrk_para = "";
            if (init_para != null)
            {
                if (init_para is string s)
                {
                    var v_para = new string[] { s };
                    para = new BizArray(v_para);
                    wrk_para = s;
                }
                else if (init_para is string[] arr)
                {
                    para = new BizArray(arr);
                    wrk_para = para[0];
                }

                else {
                    para = new BizArray();
                }
            }
            else
            {
                para = new BizArray();
            }

            if (string.IsNullOrEmpty(wrk_para)) wrk_para = ".";

                OnInit2(wrk_para, "");
        }

        public void OnInit2(string wrk_para = "", string p_sort = "")
        {

            string v_sort = "asc";
            if (p_sort != "") v_sort = " desc ";
            var v_hugo = ">=";
            if (p_sort != "") v_hugo = "<=";

            string yb_schema = "";
            if (AppData.ClassCvnet.config.YBSchema != "" && AppData.ClassCvnet.config.YBSchema != "." && AppData.ClassCvnet.config.YBSchema != null) yb_schema = AppData.ClassCvnet.config.YBSchema + ".";

            BizArray v_para = new BizArray();

            v_para[0] = wrk_para;


            string sql_str = "";
            sql_str = "SELECT 郵便番号,住所1,住所2 FROM " + yb_schema + " HC$MASTER_YBNJYS ";
            sql_str += " WHERE 郵便番号 " + v_hugo + " :1";
            sql_str += " ORDER BY 郵便番号 " + v_sort;
            sql_str = AppData.ClassCvnet.GetSqlDisp(sql_str);
            var ret_csv = AppData.Http?.AspxSqlQuery(sql_str, v_para.ToArray());
            if (ret_csv == null || ret_csv.Rows.Count == 0) return;
            var data_csv = GlobalFunc.ConvertDataTableToList<SelybnFlexItem>(ret_csv);
            SelybnFlexList = new ObservableCollection<SelybnFlexItem>(data_csv.OrderBy(x => x.Col01));

        }

        #region Events
        [RelayCommand]
        void PrevList()
        {
            if (SelybnFlexList == null || SelybnFlexList.Count == 0) return;
            var csv_sv = SelybnFlexList.OrderBy(x => x.Col01).ToList();
            string paraPrev = csv_sv[0].Col01;
            OnInit2(paraPrev,"");
        }

        [RelayCommand]
        void NextList()
        {
            if (SelybnFlexList == null || SelybnFlexList.Count == 0) return;
            var csv_sv = SelybnFlexList.OrderBy(x => x.Col01).ToList();
            string paraNext = csv_sv.LastOrDefault().Col01;
            OnInit2(paraNext, "desc");
        }

        [RelayCommand]
        void DoSearch()
        { 
            if (SelectedSelybn == null) return;
            SelectedValue = new SelybnModel()
            {
                PostalCode = SelectedSelybn.Col01,
                Addres1 = SelectedSelybn.Col02,
                Addres2 = SelectedSelybn.Col03
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

    public partial class SelybnFlexItem : ObservableObject
    {
        [ObservableProperty] string? col01;
        [ObservableProperty] string? col02;
        [ObservableProperty] string? col03;
    }
}
