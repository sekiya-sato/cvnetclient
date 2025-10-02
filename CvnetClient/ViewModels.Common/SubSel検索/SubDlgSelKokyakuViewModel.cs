using CommunityToolkit.Mvvm.ComponentModel;

namespace CvnetClient.ViewModels
{
    public partial class SubDlgSelKokyakuViewModel : BaseViewModel
    { 

        public void OnInit(string v_mstname, string[] init_para = null, string[] v_para2 = null)
        {
            OnInit(v_mstname, init_para);
        }

        public void OnInit(string v_mstname, string[] init_para = null)
        { 
            
        } 
    }

    public partial class CvnetFlexItem : ObservableObject
    {
        /// <summary>
        /// 行
        /// </summary>
        [ObservableProperty]
        public int m_No;

        /// <summary>
        /// 検索項目
        /// </summary>
        [ObservableProperty]
        public string m_SelSearch;

        /// <summary>
        /// 検索条件
        /// </summary>
        [ObservableProperty]
        public string m_SelCond;

        /// <summary>
        /// 検索文字列 (FROM)
        /// </summary>
        [ObservableProperty]
        public string m_FromValue;

        /// <summary>
        /// 検索文字列 (TO)
        /// </summary>
        [ObservableProperty]
        public string m_ToValue;
    }

    public partial class SelCustSearchOpt : ObservableObject
    {
        /// <summary>
        /// 顧客CD
        /// </summary>
        [ObservableProperty]
        public string m_SelCustCD;

        /// <summary>
        /// 顧客名
        /// </summary>
        [ObservableProperty]
        public string m_SelCustName;

        /// <summary>
        /// Search 顧客名 Condition (0 完全一致, 1 部分一致, 2 前方一致, 3 後方一致)
        /// </summary>
        [ObservableProperty]
        public string m_SrchNameCond;

        /// <summary>
        /// 電話番号
        /// </summary>
        [ObservableProperty]
        public string m_Tel;

        /// <summary>
        /// 登録日 - Start Search Condition
        /// </summary> 
        [ObservableProperty]
        public DateTime m_StartRegDate;

        /// <summary>
        /// 登録日 - End Search Condition
        /// </summary>
        [ObservableProperty]
        public DateTime m_EndRegDate;

        /// <summary>
        /// 更新日 - Start Search Condition
        /// </summary>
        [ObservableProperty]
        public DateTime m_StartUpDate;

        /// <summary>
        /// 更新日 - End Search Condition
        /// </summary>
        [ObservableProperty]
        public DateTime m_EndUpDate;

        /// <summary>
        /// 店舗CD -  Start Search Condition
        /// </summary>
        [ObservableProperty]
        public string m_StartStoreCD;

        /// <summary>
        /// 店舗CD - End Search Condition
        /// </summary>
        [ObservableProperty]
        public string m_EndStoreCD;

        /// <summary>
        /// 入会中
        /// </summary>
        [ObservableProperty]
        public bool m_IsMemberChk;

        public SelCustSearchOpt()
        { 
            SelCustCD = string.Empty;
            SrchNameCond = string.Empty;
            SelCustName = string.Empty;
            Tel = string.Empty;
            StartRegDate = new DateTime(1901, 1, 1);
            EndRegDate = DateTime.Now;
            StartUpDate = new DateTime(1901, 1, 1);
            EndUpDate = DateTime.Now;
            StartStoreCD = string.Empty;
            EndStoreCD  = "ZZZZZZZZZZ";
            IsMemberChk = true;
        }
    }
}
