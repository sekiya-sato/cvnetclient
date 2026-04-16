using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Models;
using CvnetClient.Utils;
using System.Collections.ObjectModel;
using System.Data;

namespace CvnetClient.ViewModels
{
    public partial class SubDlgSelghnViewModel : BaseViewModel
    {
        #region Binding Variable
        [ObservableProperty]
        string m_DgTitle = "原価変更履歴";

        [ObservableProperty]
        ObservableCollection<SelghnItem>? m_SelghnList;
        #endregion

        public void OnInit(string[] init_para)
        { 
            OnInitBase(init_para);
            SelghnList = new ObservableCollection<SelghnItem>();
            OnInit2();
        }

        public void OnInit2(string[] wrk_para = null)
        {
            var v_para = new BizArray();
            v_para[0] = para[0];

            string sql_str = "SELECT 変更日,元原価,仕入伝票NO ";
            sql_str += " FROM HC$MASTER_SHOHIN_GENKA_HENKOU";
            sql_str += " WHERE 商品CD=:1 ";
            sql_str += " ORDER BY 変更日 DESC,SEQ_NO DESC";

            var ret_csv = AppData.Http?.AspxSqlQuery(sql_str, v_para.ToArray());
            if (ret_csv == null || ret_csv.Rows.Count == 0) return;
            var list = (from DataRow dr in ret_csv.Rows
                        select new SelghnItem
                        {
                            ChangeDate = dr["変更日"].ToString() ?? string.Empty,
                            OriCost = dr["元原価"].ToString() ?? string.Empty,
                            PurchSlipNo = dr["仕入伝票NO"].ToString() ?? string.Empty,
                        });
            SelghnList = new ObservableCollection<SelghnItem>(list);
        }

        #region Button Events
        [RelayCommand]
        private void DoExit()
        {
            ClientLib.ExitDialogResult(this, true);
        }
        #endregion
    }

    public partial class SelghnItem : ObservableObject
    {
        /// <summary>
        /// 変更日
        /// </summary>
        [ObservableProperty]
        string? m_ChangeDate;

        /// <summary>
        /// 元原価
        /// </summary>
        [ObservableProperty]
        string? m_OriCost;

        /// <summary>
        /// 仕入伝票NO
        /// </summary>
        [ObservableProperty]
        string? m_PurchSlipNo;

        public SelghnItem()
        {
            ChangeDate = "";
            OriCost = "";
            PurchSlipNo = "";
        }
    }
}
