using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Models;
using CvnetClient.Utils;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;

namespace CvnetClient.ViewModels
{
    public partial class SubDlgSelclsz0ViewModel : BaseViewModel
    {
        #region Screen Bind Variables
        [ObservableProperty]
        ObservableCollection<Selclsz0Item>? m_ListSelclsz0;

        [ObservableProperty]
        Selclsz0Item? selectedSelclsz0;

        /// <summary>
        /// Current Dialog return value
        /// </summary>
        public BizArray ret_para;
         
        [ObservableProperty]
        public string? m_Dsp1;

        [ObservableProperty]
        public string? m_Dsp2;
        [ObservableProperty]
        public Visibility? m_Dsp2Visible = Visibility.Collapsed;

        [ObservableProperty]
        public string? m_Dsp3;
        [ObservableProperty]
        public Visibility? m_Dsp3Visible = Visibility.Collapsed;
        #endregion

        #region Private Variables
        [ObservableProperty]
        public string? m_Title = "色サイズ選択画面";
        private BizArray para;
        #endregion

        public void OnInit(string[] init_para = null, string[] wrk_para = null)
        {
            if (init_para != null) para = new BizArray(init_para);
            else para = new BizArray();

            if (para.Count > 0)
            {
                Dsp1 = para[0];
            }

            /* 色サイズ取得 */
            var sql_sub = ",GET_COLORNAME(A.色CD) 色名"
                    + ",GET_SIZENAME(A.商品CD,A.サイズCD) サイズ名";
            if (AppData.ClassCvnet.config.ColSizMei == 1) { sql_sub = ",A.色名 色名,A.サイズ名 サイズ名"; }

            /* 上代取得 */
            var v_jd1 = ",get_jodai(A.商品CD,A.色CD,A.サイズCD) 上代";
            if (wrk_para != null)
            { 
                v_jd1 = ",get_jodai(A.商品CD,A.色CD,A.サイズCD,'" + wrk_para[0] + "','" + wrk_para[1] + "') 上代";
            }

            var sql_str = "Select A.色CD,A.サイズCD" + v_jd1;
            sql_str += ",B.上代 商品上代,A.商品CD";
            sql_str += sql_sub;
            sql_str += ",GET_JODAI(A.商品CD,A.色CD,A.サイズCD) マスタ上代";
            sql_str += ",B.営業原価";
            sql_str += ",A.JANコード1 JANコード";
            sql_str += ",GET_GENKA(A.商品CD,0,A.最終出庫日,A.色CD,A.サイズCD) 原価";
            sql_str += " From HC$MASTER_SHOHIN_JAN A";
            sql_str += " join HC$master_SHOHIN B on (B.商品CD=A.商品CD)";
            sql_str += " where A.商品CD=:1";
            if (para.Count > 1)
            {   /* 色CDあり */
                sql_str += " and A.色CD=:2";
                Dsp2Visible = Visibility.Visible;
                Dsp3Visible = Visibility.Visible;
            }

            var ret_csv = AppData.Http?.AspxSqlQuery(sql_str, init_para); 
            var list = (from DataRow dr in ret_csv.Rows
                        select new Selclsz0Item
                        {
                            ColorCD = dr["色CD"].ToString() ?? string.Empty,
                            SizeCD = dr["サイズCD"].ToString() ?? string.Empty,
                            Retail = Convert.ToInt32(dr["上代"]),
                            ProdRetail = Convert.ToInt32(dr["商品上代"]),
                            ProdCD = dr["商品CD"].ToString() ?? string.Empty,
                            ColorName = dr["色名"].ToString() ?? string.Empty,
                            SizeName = dr["サイズ名"].ToString() ?? string.Empty,
                            MasterRetail = Convert.ToInt32(dr["マスタ上代"]),
                            OperateCost = Convert.ToInt32(dr["営業原価"]),
                            JanCode = dr["JANコード"].ToString() ?? string.Empty,
                            Cost = Convert.ToInt32(dr["原価"])
                        }).ToList();
            ListSelclsz0 = new ObservableCollection<Selclsz0Item>(list);
        }

        #region Events 
        partial void OnSelectedSelclsz0Changed(Selclsz0Item? value)
        {
            if (SelectedSelclsz0 != null)
            {
                Dsp2 = SelectedSelclsz0.ColorCD;
                Dsp3 = SelectedSelclsz0.ColorName;
            }
        }

        [RelayCommand]
        void DoExecute()
        {
            if (ListSelclsz0 == null || ListSelclsz0.Count == 0 ||
                SelectedSelclsz0 == null) return; 
            ret_para = new BizArray();
            /* 色CD,サイズCD,色名,サイズ名,上代 */
            ret_para[0] = SelectedSelclsz0.ColorCD;
            ret_para[1] = SelectedSelclsz0.SizeCD;
            ret_para[2] = SelectedSelclsz0.ColorName;
            ret_para[3] = SelectedSelclsz0.SizeName;
            ret_para[4] = SelectedSelclsz0.Retail.ToString();
            ret_para[5] = SelectedSelclsz0.MasterRetail.ToString();
            ret_para[6] = SelectedSelclsz0.OperateCost.ToString();
            ret_para[7] = SelectedSelclsz0.JanCode;
            ret_para[8] = SelectedSelclsz0.Cost.ToString();
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

    public partial class Selclsz0Item : ObservableObject
    {
        /// <summary>
        /// 色CD
        /// </summary>
        [ObservableProperty]
        string? m_ColorCD;

        /// <summary>
        /// サイズCD
        /// </summary>
        [ObservableProperty]
        string? m_SizeCD;

        /// <summary>
        /// 上代
        /// </summary>
        [ObservableProperty]
        int? m_Retail;

        /// <summary>
        /// 商品上代
        /// </summary>
        [ObservableProperty]
        int? m_ProdRetail;

        /// <summary>
        /// 商品CD
        /// </summary>
        [ObservableProperty]
        string? m_ProdCD;

        /// <summary>
        /// 色名
        /// </summary>
        [ObservableProperty]
        string? m_ColorName;

        /// <summary>
        /// サイズ名
        /// </summary>
        [ObservableProperty]
        string? m_SizeName;

        /// <summary>
        /// マスタ上代
        /// </summary>
        [ObservableProperty]
        int? m_MasterRetail;

        /// <summary>
        /// 営業原価
        /// </summary>
        [ObservableProperty]
        int? m_OperateCost;

        /// <summary>
        /// JANコード
        /// </summary>
        [ObservableProperty]
        string? m_JanCode;

        /// <summary>
        /// 原価
        /// </summary>
        [ObservableProperty]
        int? m_Cost;
    }
}
