using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Models;
using CvnetClient.Utils;

namespace CvnetClient.ViewModels
{
    public partial class SubDlgSyoKenSakuViewModel : BaseViewModel
    {
        [ObservableProperty]
        private BtListHelper product1;
        [ObservableProperty]
        private BtListHelper product2;
        [ObservableProperty]
        private BtListHelper product3;
        [ObservableProperty]
        private BtListHelper product4;
        [ObservableProperty]
        private BtListHelper product5;
        public string[] Result { get; private set; }
        public SubDlgSyoKenSakuViewModel() 
        { 

        }

        [RelayCommand]
        public void SelProduct1(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null)
            {
                Product1 = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }

        [RelayCommand]
        public void SelProduct2(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null)
            {
                Product2 = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }

        [RelayCommand]
        public void SelProduct3(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null)
            {
                Product3 = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }

        [RelayCommand]
        public void SelProduct4(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null)
            {
                Product4 = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }

        [RelayCommand]
        public void SelProduct5(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null)
            {
                Product5 = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }

        [RelayCommand]
        public void SetChoice() 
        {
            Result = new string[5];
            Result[0] = Product1?.Code ?? string.Empty;
            Result[1] = Product2?.Code ?? string.Empty;
            Result[2] = Product3?.Code ?? string.Empty;
            Result[3] = Product4?.Code ?? string.Empty;
            Result[4] = Product5?.Code ?? string.Empty;

            Close();
        }
    }
}
