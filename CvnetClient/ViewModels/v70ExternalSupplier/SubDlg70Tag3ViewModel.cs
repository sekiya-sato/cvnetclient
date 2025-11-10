using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg70Tag3ViewModel : BaseViewModel
    {
        [ObservableProperty]
        string? title; 
    }

    public partial class Tag3SearchOpt : ObservableObject
    {
        /// <summary>
        /// 札種
        /// </summary>
        [ObservableProperty]
        string? m_SelBill;

        /// <summary>
        /// 伝票処理区分
        /// </summary>
        [ObservableProperty]
        string? m_SelSlipCategory;

        /// <summary>
        /// 商品CD - Start
        /// </summary>
        [ObservableProperty]
        string? m_SelProdStartCd;

        /// <summary>
        /// 商品CD - End
        /// </summary>
        [ObservableProperty]
        string? m_SelProdEndCd;

        /// <summary>
        /// 得意先 - Start
        /// </summary>
        [ObservableProperty]
        string? m_SelCustStart;

        /// <summary>
        /// 得意先 - End
        /// </summary>
        [ObservableProperty]
        string? m_SelCustEnd;

        /// <summary>
        /// 倉庫 - Start
        /// </summary>
        [ObservableProperty]
        string? m_SelStoreStart;

        /// <summary>
        /// 倉庫 - End
        /// </summary>
        [ObservableProperty]
        string? m_SelStoreEnd;


    }
}
