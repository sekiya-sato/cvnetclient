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

        /// <summary>
		/// 修正用の一時的なProductオブジェクト
		/// </summary>
		[ObservableProperty]
        MasterShohin? editProduct;

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
        }

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
    }
}