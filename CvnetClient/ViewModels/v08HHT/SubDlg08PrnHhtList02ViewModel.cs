using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Models;
using CvnetClient.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg08PrnHhtList02ViewModel : BaseViewModel
    {
        [ObservableProperty]
        private DetailsPurchase? purchaseProduct;
        [ObservableProperty]
        private HaibunMenu? haibunChoose;

        [ObservableProperty]
        BtListHelper findFromHaibunCd = new();
        [ObservableProperty]
        BtListHelper findFromStockinCd = new();
        public void OnInit(object? init_para = null, string? init_flg = null)
        {
            OnInitBase(init_para, init_flg);

            PurchaseProduct = new DetailsPurchase();
            HaibunChoose = new HaibunMenu();
            findFromHaibunCd = new BtListHelper();
            PurchaseProduct.HaibunKubunFrom = "Hello";
            PurchaseProduct.HaibunKubunTo = "Byebye";
        }

        //[RelayCommand]
        //public void HaibunChoice(object value)
        //{
        //    var get_sel00 = (SelValueModel)value;
        //    if (get_sel00 != null && EditHaibunCD != null)
        //    {
        //        EditHaibunCD = new BtListHelper(get_sel00.Code, get_sel00.Name);
        //        PurchaseProduct.HaibunKubunFrom = EditHaibunCD.Code;
        //        PurchaseProduct.HaibunKubunFrom = EditHaibunCD.Name;
        //    }
        //}
       [RelayCommand]
        public void FindFromHaibun(SelValueModel value)
        {
            if (value == null) return;

            FindFromHaibunCd = new BtListHelper(value.Code, value.Name);

            HaibunChoose.FromShopCd = FindFromHaibunCd.Code;            
            HaibunChoose.FromShopName = FindFromHaibunCd.Name;
        }

        [RelayCommand]
        public void FindFromStockin(SelValueModel value)
        {
            if (value == null) return;

            FindFromStockinCd = new BtListHelper(value.Code, value.Name);

            HaibunChoose.FromStockinCd = FindFromStockinCd.Code;
            HaibunChoose.FromStockinName = FindFromStockinCd.Name;
        }
    }
    public partial class DetailsPurchase : ObservableObject
    {
        [ObservableProperty]
        public string haibunKubunFrom;
        [ObservableProperty]
        public string haibunKubunTo;
        [ObservableProperty]
        public string stockOutFrom;
        [ObservableProperty]
        public string stockOutTo;
        [ObservableProperty]
        public string stockInFrom;
        [ObservableProperty]
        public string stockInTo;
        [ObservableProperty]
        public string dateFrom;
        [ObservableProperty]
        public string dateTo;
        [ObservableProperty]
        public string denpyoFrom;
        [ObservableProperty]
        public string denpyoTo;
    }

    public partial class HaibunMenu : ObservableObject
    {
        [ObservableProperty]
        private string? fromShopCd;
        [ObservableProperty]
        private string? fromShopName;
        [ObservableProperty]
        private string? toShopCd;
        [ObservableProperty]
        private string? toShopName;
        [ObservableProperty]
        private string? fromStockinCd;
        [ObservableProperty]
        private string? fromStockinName;
        [ObservableProperty]
        private string? toStockinCd;
        [ObservableProperty]
        private string? toStockinName;
    }
}