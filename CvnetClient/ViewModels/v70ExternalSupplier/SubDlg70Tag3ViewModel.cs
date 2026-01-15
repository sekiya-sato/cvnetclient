using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Models;
using CvnetClient.Utils;
using System.Data;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg70Tag3ViewModel : BaseViewModel {
        #region Variables
        [ObservableProperty]
        string? title = "外部連携 : 下札発行用CSVデータ作成";

        [ObservableProperty]
        Tag3SearchOpt? tagSearchOpt;

        private int col_max = 90;
        private string txt_no = string.Empty;
        private string pos_no = string.Empty;
        private int num_su = 0;
        #endregion

        #region Combobox List 
        /// <summary>
        /// 札種 List
        /// </summary>
        [ObservableProperty]
        public Dictionary<string, string>? m_BillList;

        [ObservableProperty]
        public Dictionary<string, string>? m_SlipCatList;
        #endregion 

        public void OnInit(object? init_para = null, string? init_flg = null)
        {
            OnInitBase(init_para);

            /* xmlパターンファイル取得/読込追加 */
            GetServerPaturn();

            TagSearchOpt = new Tag3SearchOpt();
        }

        #region Dialog Search
        [RelayCommand]
        public void SelProd01()
        {
            if (TagSearchOpt == null) return;
            /* 自社社員 全表示 */
            if (AppData.ClassSatoo.LoginKubun == 0)
            {
                if (AppData.ClassCvnet.MstDialog.ContainsKey("商品"))
                {
                    SelValueModel sel_value = new SelValueModel();
                    sel_value.Code = TagSearchOpt.SelProdStart.Code;
                    sel_value.Name = TagSearchOpt.SelProdStart.Name;
                    var vm = AppData.DlgService.GetSelSho(AppData.ClassCvnet.MstDialog["商品"].v_mstname, null,
                                                          new string[] { "0" }, null, null, 0, sel_value);
                    if (vm != null)
                    {
                        TagSearchOpt.SelProdStart = new BtListHelper(vm.SelectedValue.Code, vm.SelectedValue.Name);
                        TagSearchOpt.SelProdEnd = new BtListHelper(vm.SelectedValue.Code, vm.SelectedValue.Name);
                    }
                }
            }
            else
            {
                /* 自社社員以外(仕入先) 仕入先縛り有 */
            }
        }
        [RelayCommand]
        public void SelProd02()
        {
            if (TagSearchOpt == null) return;
            /* 自社社員 全表示 */
            if (AppData.ClassSatoo.LoginKubun == 0)
            {
                if (AppData.ClassCvnet.MstDialog.ContainsKey("商品"))
                {
                    SelValueModel sel_value = new SelValueModel();
                    sel_value.Code = TagSearchOpt.SelProdStart.Code;
                    sel_value.Name = TagSearchOpt.SelProdStart.Name;
                    var vm = AppData.DlgService.GetSelSho(AppData.ClassCvnet.MstDialog["商品"].v_mstname, null,
                                                          new string[] { "0" }, null, null, 0, sel_value);
                    if (vm != null)
                    {
                        TagSearchOpt.SelProdEnd = new BtListHelper(vm.SelectedValue.Code, vm.SelectedValue.Name);
                    }
                }
            }
            else
            {
                /* 自社社員以外(仕入先) 仕入先縛り有 */
            }
        }
        [RelayCommand]
        public void SelCust01(object value)
        {
            if (TagSearchOpt == null) return;
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null)
            {
                TagSearchOpt.SelCustStart = new BtListHelper(get_sel00.Code, get_sel00.Name);
                TagSearchOpt.SelCustEnd = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        [RelayCommand]
        public void SelCust02(object value)
        {
            if (TagSearchOpt == null) return;
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null)
            {
                TagSearchOpt.SelCustEnd = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        [RelayCommand]
        public void SelStore01(object value)
        {
            if (TagSearchOpt == null) return;
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null)
            {
                TagSearchOpt.SelStoreStart = new BtListHelper(get_sel00.Code, get_sel00.Name);
                TagSearchOpt.SelStoreEnd = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        [RelayCommand]
        public void SelStore02(object value)
        {
            if (TagSearchOpt == null) return;
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null)
            {
                TagSearchOpt.SelStoreEnd = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// 品番検索実行 Button
        /// </summary>
        [RelayCommand]
        void DoSearch()
        {

        }
        /// <summary>
        /// 割合加算 Button
        /// </summary>
        [RelayCommand]
        void DoPercentAdd()
        {

        }
        /// <summary>
        /// 枚数加算 Button
        /// </summary>
        [RelayCommand]
        void DoSheetNum()
        {

        }
        /// <summary>
        /// データ作成 Button
        /// </summary>
        [RelayCommand]
        void DoExecute()
        {

        }
        /// <summary>
        /// 戻る Button
        /// </summary>
        [RelayCommand]
        void DoCancel()
        {

        }
        #endregion

        #region Function
        void GetServerPaturn()
        {
            var data = AppData.Http?.GetXmlFile("Kyakusuu_Paturn.xml");
        }
        #endregion
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
        string? m_SelSlipCat;

        /// <summary>
        /// 商品CD - Start
        /// </summary>
        [ObservableProperty]
        BtListHelper? m_SelProdStart;

        /// <summary>
        /// 商品CD - End
        /// </summary>
        [ObservableProperty]
        BtListHelper? m_SelProdEnd;

        /// <summary>
        /// 得意先 - Start
        /// </summary>
        [ObservableProperty]
        BtListHelper? m_SelCustStart;

        /// <summary>
        /// 得意先 - End
        /// </summary>
        [ObservableProperty]
        BtListHelper? m_SelCustEnd;

        /// <summary>
        /// 得意先 Mstname
        /// </summary>
        [ObservableProperty]
        string? m_SelMstCust;

        /// <summary>
        /// 倉庫 - Start
        /// </summary>
        [ObservableProperty]
        BtListHelper? m_SelStoreStart;

        /// <summary>
        /// 倉庫 - End
        /// </summary>
        [ObservableProperty]
        BtListHelper? m_SelStoreEnd;

        /// <summary>
        /// 倉庫 Mstname
        /// </summary>
        [ObservableProperty]
        string? m_SelMstStore;

        /// <summary>
        /// 伝票NO - Start
        /// </summary>
        [ObservableProperty]
        string? m_SelSlipStartNo;

        /// <summary>
        /// 伝票NO - End
        /// </summary>
        [ObservableProperty]
        string? m_SelSlipEndNo;

        /// <summary>
        /// 在庫計上日 - Start
        /// </summary>
        [ObservableProperty]
        DateTime? m_SelInvStartDate;

        /// <summary>
        /// 在庫計上日 - End
        /// </summary>
        [ObservableProperty]
        DateTime? m_SelInvEndDate;

        /// <summary>
        /// 行 Option (0: 追加, 1: 新規)
        /// </summary>
        [ObservableProperty]
        int m_SelLineCond;

        /// <summary>
        /// 変更タイトル
        /// </summary>
        [ObservableProperty]
        string? m_ChgTitle;

        /// <summary>
        /// 変更内容(文字列用)
        /// </summary>
        [ObservableProperty]
        string? m_ChgWordCnt;

        /// <summary>
        /// 変更内容(数値用)
        /// </summary>
        [ObservableProperty]
        int m_ChgNumCnt;

        /// <summary>
        /// 割合加算
        /// </summary>
        [ObservableProperty]
        int m_PercentAdd;

        /// <summary>
        /// 枚数加算
        /// </summary>
        [ObservableProperty]
        int m_SheetAdd;

        /// <summary>
        /// 出力先
        /// </summary>
        [ObservableProperty]
        string? m_OutputDest;

        /// <summary>
        /// 出力先 Message
        /// </summary>
        [ObservableProperty]
        string? m_OutputMess;

        public Tag3SearchOpt()
        {
            SelBill = string.Empty;
            SelSlipCat = string.Empty;
            SelProdStart = new();
            SelProdEnd = new();
            SelCustStart = new();    
            SelCustEnd = new();
            SelMstCust = string.Empty;
            SelStoreStart = new();
            SelStoreEnd = new();
            SelMstStore = string.Empty;
            SelSlipStartNo = string.Empty;
            SelSlipEndNo = string.Empty;
            SelInvStartDate = DateTime.Now;
            SelInvEndDate =  DateTime.Now;
            SelLineCond = 0;
            ChgTitle = string.Empty;
            ChgWordCnt = string.Empty;
            ChgNumCnt = 0;
            PercentAdd = 0;
            SheetAdd = 0;
            OutputDest = string.Empty;
            OutputMess = string.Empty;
        }
    }
}
