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

        //[ObservableProperty]
        //      MasterShohin? editProduct;

        [ObservableProperty]
        MasterShohinJan? editProduct;

        //[ObservableProperty]
        //ObservableCollection<DeptItem>? deptList;

        //[ObservableProperty]
        //FlagOpt? selectedItem;

        [ObservableProperty]
        public Dictionary<int, string>? chushi;

        [ObservableProperty]
        public Dictionary<int, string>? jidohaibun;

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
            EditProduct = new MasterShohinJan();

            Chushi = new Dictionary<int, string>
            {
                { 0, "0 正規" },
                { 1, "1 中止" },
            };
            EditProduct.UseFlag = Chushi.FirstOrDefault().Key;

            Jidohaibun = new Dictionary<int, string>
            {
                { 0, "0 しない" },
                { 1, "1 売上基準" },
                { 9, "9 商品マスタ依存" }
            };
            EditProduct.AutoAllocationFlag = Jidohaibun.FirstOrDefault().Key;

            string sql_query = "select nvl((select 値 from hc$master_config where フラグ名 = 'dispColSizKakaku'),0) flg from dual";

        }
        string sql_col_list = """
                              商品CD,色CD,サイズCD,JANコード1,JANコード2,JANコード3,メモ,使用FLG,生産予定数,裁断数,下札枚数,
                              自動配分FLG,上代,仕入価格,外貨仕入価格,原価
                            """;

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
        //public class FlagOpt
        //{
        //    public string? chushiFLG { get; set; }   // 中止
        //    public string? jidohaibunFLG { get; set; }   // 自動配分
        //}
    }
}