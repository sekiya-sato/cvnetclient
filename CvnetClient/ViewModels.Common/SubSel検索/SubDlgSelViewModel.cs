using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Models;
using CvnetClient.Utils;
using System.Data;

namespace CvnetClient.ViewModels
{
    public partial class SubDlgSelViewModel : BaseViewModel
    {
        #region Binding Variable
        [ObservableProperty]
        private string m_DgTitle;

        [ObservableProperty]
        private DataTable m_Spread1;

        [ObservableProperty]
        private DataRowView m_SelectedRow;
        #endregion

        #region Class Variable
        private BizCsvDocument SpRow;
        public BizArray ret_result;
        #endregion

        public void OnInit(string[] init_para)
        { 
            OnInitBase(init_para);
            OnInit2();
        }

        public void OnInit2()
        {
            if (para.Count > 1)
            {
                if (para[0] == "COL")
                    DgTitle = "カラーを選択して下さい";
                else if (para[0] == "SIZ")
                {
                    DgTitle = "サイズを選択して下さい";
                    if (para.Count > 2)
                    {
                        if (para[2] != "")
                        {
                            para[1] = "";
                            var wrk_pp = new BizArray();
                            wrk_pp[0] = para[2];
                            var wrk_csv = AppData.Http?.AspxSqlQuery("select 商品サイズ区分 from HC$master_shohin where 商品CD=:1", wrk_pp.ToArray());
                            if (wrk_csv?.Rows.Count > 0)
                                para[0] = wrk_csv.Rows[0][0].ToString();
                        }
                    }
                }
                else if (para[0] == "HIN")
                {
                    DgTitle = "品質を選択して下さい";
                    if (para[2] != "")
                    {
                        SpRow = new BizCsvDocument(para[2], 1);
                        Spread1 = SpRow.GetTable();
                        return;
                    }
                }
                else if (para[0] == "TOKUI")
                {
                    DgTitle = "店舗を選択して下さい";
                    if (para[2] != "")
                    {
                        SpRow = new BizCsvDocument(para[2], 1);
                        Spread1 = SpRow.GetTable();
                        return;
                    }
                }
                /* user76 12.05.04 名称無しは表示しない */
                var v_para = new BizArray();
                v_para[0] = para[0];
                v_para[1] = para[1];
                string sql_query = "select 名称CD,名称,略称 from HC$MASTER_MEISHO ";
                sql_query += " where 名称区分=:1 and 名称CD>=:2 ";
                sql_query += " order by 名称CD";
                sql_query = AppData.ClassCvnet.GetSqlDisp(sql_query);
                var ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para.ToArray());
                if (ret_csv != null) Spread1 = ret_csv;
            }
        }

        #region Button Events
        [RelayCommand]
        private void NextList() 
        {
            if (Spread1 != null && Spread1.Rows.Count == 0) return;
            para[1] = Spread1.Rows[Spread1.Rows.Count - 1][0].ToString();
            OnInit2();
        }

        [RelayCommand]
        private void TopList() {
            para[1] = "";
            OnInit2();
        }

        [RelayCommand]
        private void DoSearch() { 
            if (SelectedRow == null) return;
            ret_result = new BizArray();
            ret_result[0] = SelectedRow.Row[0].ToString();
            ret_result[1] = SelectedRow.Row[1].ToString();
            ClientLib.ExitDialogResult(this, true);
        }

        [RelayCommand]
        private void DoExit() {
            ClientLib.ExitDialogResult(this, true);
        }
        #endregion
    }
}
