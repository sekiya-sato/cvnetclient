
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Models;
using CvnetClient.Utils;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;

namespace CvnetClient.ViewModels
{
    public partial class SubDlgSel13ViewModel : BaseViewModel
    {
        #region Binding Variable
        [ObservableProperty]
        public string? m_Title = "売上金額履歴";

        [ObservableProperty]
        public Sel13Dsp? m_Sel13Dsp;

        [ObservableProperty]
        public ObservableCollection<Sel13GridItem>? m_Sel13GridItems;

        [ObservableProperty]
        public Sel13GridItem m_Sel13GridItem;

        /// <summary>
        /// Sel13 Dialog Return Value
        /// </summary>
        public BizArray ret_para;
        #endregion

        /************/
        /* 初期処理 */
        /************/
        public void OnInit(string[] init_para)
        {
            OnInitBase(init_para);
            OnInit2(init_para);
        }

        public void OnInit2(string[] init_para)
        {
            Sel13Dsp = new Sel13Dsp();
            Sel13GridItems = new ObservableCollection<Sel13GridItem>();

            Sel13Dsp.SalesDate = DateTime.ParseExact(init_para[2], "yyyyMMdd", CultureInfo.InvariantCulture);
            string sql_query = "select t0.seq_no,t0.在庫計上日,t0.掛計上日,t0.取引区分"
                + ",t1.上代単価,t1.下代単価,trunc(t1.下代単価*100/t1.上代単価) 掛率,t0.関連伝票NO,t0.関連伝票NO2,t1.色CD,t1.サイズCD"
                + " from HC$TRAN_TORI0 t0,HC$TRAN_TORI1 t1"
                + " where t0.SEQ_NO = t1.ヘッダNO and t0.伝票処理区分 = 0"
                + " and t1.商品CD = :1 and t0.取引先CD1 = :2 and t0.在庫計上日 >= :3"
                + " order by t0.SEQ_NO desc, t1.商品CD, t1.色CD, t1.サイズCD";

            var ret_csv = AppData.Http?.AspxSqlQuery(sql_query, init_para);
            if (ret_csv != null && ret_csv.Rows.Count > 0)
            {
                var list = (from DataRow dr in ret_csv.Rows
                            select new Sel13GridItem
                            {
                                SeqNo = dr["SEQ_NO"].ToString() ?? string.Empty,
                                InvRecordDate = dr["在庫計上日"].ToString() ?? string.Empty,
                                AccountDate = dr["掛計上日"].ToString() ?? string.Empty,
                                TranCate = dr["取引区分"].ToString() ?? string.Empty,
                                RetailUnitPrice = int.TryParse(dr["上代単価"].ToString(), out var retail_unit) ? retail_unit : 0,
                                CostUnitPrice = int.TryParse(dr["下代単価"].ToString(), out var cost_unit) ? cost_unit : 0,
                                CallRate = dr["掛率"].ToString() ?? string.Empty,
                                RelateSlipNo = dr["関連伝票NO"].ToString() ?? string.Empty,
                                RelateSlipNo2 = dr["関連伝票NO2"].ToString() ?? string.Empty,
                                ColorCd = dr["色CD"].ToString() ?? string.Empty,
                                SizeCd = dr["サイズCD"].ToString() ?? string.Empty
                            }).ToList();
                foreach (var item in list)
                {
                    if (DateTime.TryParseExact(item.InvRecordDate, "yyyyMMdd",
                                               null,
                                               System.Globalization.DateTimeStyles.None,
                                               out DateTime dt1))
                    {
                        item.InvRecordDate = dt1.ToString("yyyy/MM/dd");
                    }

                    if (DateTime.TryParseExact(item.AccountDate, "yyyyMMdd",
                                               null,
                                               System.Globalization.DateTimeStyles.None,
                                               out DateTime dt2))
                    {
                        item.AccountDate = dt2.ToString("yyyy/MM/dd");
                    }

                    item.TranCate = AppData.ClassCvnet.comboItem00.FindStr("本部売上区分", item.TranCate);
                }

                Sel13GridItems = new ObservableCollection<Sel13GridItem>(list);
            }
        }

        #region Events
        partial void OnSel13GridItemChanged(Sel13GridItem value)
        {
            if (Sel13Dsp == null || value == null) return;
            Sel13Dsp.Color = value.ColorCd;
            Sel13Dsp.Size = value.SizeCd;
        }

        [RelayCommand]
        public void DoSearch() 
        {
            var v_para = new BizArray();
            v_para = para;
            v_para[2] = Sel13Dsp.SalesDate?.ToString("yyyyMMdd");
            OnInit2(v_para.ToArray());
        }

        [RelayCommand]
        public void DoExecute()
        { 
            if (Sel13GridItems == null || Sel13GridItems.Count == 0) return;
            if (Sel13GridItem == null) return;
            ret_para = new BizArray();
            ret_para[0] = Sel13GridItem.CostUnitPrice.ToString();
            ClientLib.ExitDialogResult(this, true);
        }

        [RelayCommand]
        public void DoExit()
        {
            ClientLib.ExitDialogResult(this, true);
        }
        #endregion 
    }

    public partial class Sel13GridItem : ObservableObject
    {
        /// <summary>
        /// SEQ_NO
        /// </summary>
        [ObservableProperty]
        string? m_SeqNo;

        /// <summary>
        /// 在庫計上日
        /// </summary>
        [ObservableProperty]
        string? m_InvRecordDate;

        /// <summary>
        /// 掛計上日
        /// </summary>
        [ObservableProperty]
        string? m_AccountDate;

        /// <summary>
        /// 取引区分
        /// </summary>
        [ObservableProperty]
        string? m_TranCate;

        /// <summary>
        /// 上代単価
        /// </summary>
        [ObservableProperty]
        int m_RetailUnitPrice;

        /// <summary>
        /// 下代単価
        /// </summary>
        [ObservableProperty]
        int m_CostUnitPrice;

        /// <summary>
        /// 掛率
        /// </summary>
        [ObservableProperty]
        string? m_CallRate;

        /// <summary>
        /// 関連伝票NO
        /// </summary>
        [ObservableProperty]
        string? m_RelateSlipNo;

        /// <summary>
        /// 関連伝票NO2
        /// </summary>
        [ObservableProperty]
        string? m_RelateSlipNo2;

        /// <summary>
        /// 色CD
        /// </summary>
        [ObservableProperty]
        string? m_ColorCd;

        /// <summary>
        /// サイズCD
        /// </summary>
        [ObservableProperty]
        string? m_SizeCd;
    }

    public partial class Sel13Dsp : ObservableObject
    {
        /// <summary>
        /// 売上日 [CodeNen1]
        /// </summary>
        [ObservableProperty]
        DateTime? m_SalesDate;

        /// <summary>
        /// 色 [Dsp7]
        /// </summary>
        [ObservableProperty]
        string? m_Color;

        /// <summary>
        /// ｻｲｽﾞ [Dsp8]
        /// </summary>
        [ObservableProperty]
        string? m_Size;

        public Sel13Dsp()
        {
            SalesDate = new DateTime(1901, 1, 1);
            Color = string.Empty;
            Size = string.Empty;
        }
    }
}
