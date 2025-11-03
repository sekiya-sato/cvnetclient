using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Models;
using CvnetClient.Utils;
using CvnetClient.Views;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using static CvnetClient.ViewModels.SubDlg09Upkeihi2ViewModel;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg01ShojanViewModel : BaseViewModel
    {
        [ObservableProperty]
        ObservableCollection<MasterShohin>? listProduct;

		[ObservableProperty]
        MasterShohin? editProduct;

        //[ObservableProperty]
        //ObservableCollection<DeptItem>? deptList;

        [ObservableProperty]
        FlagOpt? selectedItem;

        [ObservableProperty]
        public Dictionary<string, string>? chushi;

        [ObservableProperty]
        public Dictionary<string, string>? jidohaibun;

        [ObservableProperty]
        string? selectedColorText;

        [ObservableProperty]
        string? selectedSizeText;

        [ObservableProperty]
        MasterShohin? selectedProduct;

        [ObservableProperty]
        string? selProductCd;

        [ObservableProperty]
        int? pageNow;

        [ObservableProperty]
        int? pageTotal;

        private BizArray col_list;

        public void OnInit()
        {
            EditProduct = new MasterShohin();

            SelectedItem = new FlagOpt();
            Chushi = new Dictionary<string, string>
            {
                { "0", "0 正規" },
                { "1", "1 中止" },
            };
            SelectedItem.chushiFLG = Chushi.FirstOrDefault().Key;

            Jidohaibun = new Dictionary<string, string>
            {
                { "0", "0 しない" },
                { "1", "1 売上基準" },
                { "9", "9 商品マスタ依存" }
            };
            SelectedItem.jidohaibunFLG = Jidohaibun.FirstOrDefault().Key;
        }

        string sql_col_list = """
                              商品CD,色CD,サイズCD,JANコード1,JANコード2,JANコード3,メモ,使用FLG,生産予定数,裁断数,下札枚数,
                              自動配分FLG,上代,仕入価格,外貨仕入価格,原価
                            """;



//        public void SelDspUpdate()
//        {
//            /* ダイアログ検索条件を追加 */
//            if (AppData.ClassCvnet.MstDialog.ContainsKey("商品") && AppData.ClassCvnet.ComboListFLg == 1)
//            {
//                var ar = new string[] { "1" };
//                var vm = AppData.DlgService.GetSelSho(AppData.ClassCvnet.MstDialog["商品"].v_mstname, null, ar);
//                if (vm != null)
//                {
//                    //vm.SelShoResult0 
////                  PageView(vm.SelShoResult1.Item2, 0, vm.SelShoResult1.Item1);
//                }

//                return;
//            }

//            var v_para = new string[] { SelProductCd };
//        }

        [RelayCommand]
        public void SelProduct(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && EditProduct != null)
            {
                EditProduct.ProductCD = get_sel00.Code;
                EditProduct.ProductName = get_sel00.Name;
            }
        }

        [RelayCommand]
        public void SelColor(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null)
            {
                // Put the selected color string into the bindable property
                SelectedColorText = get_sel00.Name;
            }
        }

        [RelayCommand]
        public void SelSize(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null)
            {
                // Put the selected color string into the bindable property
                SelectedSizeText = get_sel00.Name;
            }
        }

        [RelayCommand]
        public void SelJan(object value)
        {
            
        }

        // for 更新 button
        [RelayCommand]
        void DoUpdate()
        {

        }

        // for 削除 button
        [RelayCommand]
        void DoDelete()
        {

        }

        // for 追加 button
        [RelayCommand]
        void DoInsert()
        {

        }
        public class FlagOpt
        {
            public string? chushiFLG { get; set; }   // 中止
            public string? jidohaibunFLG { get; set; }   // 自動配分
        }
    }
}