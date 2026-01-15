using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Class;
using CvnetClient.Models;
using CvnetClient.Utils;
using CvnetClient.ViewModels.v13Hachu;
using CvnetClient.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Windows;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg13Nohin1v3150ViewModel : BaseViewModel
    {


        [ObservableProperty]
        BtListHelper? findBrand = new();
        [ObservableProperty]
        private string? selectedBrand;

        [ObservableProperty]
        BtListHelper? findItem1 = new();
        [ObservableProperty]
        private string? selectedItem1;

        [ObservableProperty]
        BtListHelper? findItem2 = new();
        [ObservableProperty]
        private string? selectedItem2;

        [ObservableProperty]
        private string codeNen0 = DateTime.Now.ToString();
        [ObservableProperty]
        private string codeNen1 = DateTime.Now.ToString();
        [ObservableProperty]
        private string codeNen2 = DateTime.Now.ToString();

        [ObservableProperty]
        private string codeNen01 = DateTime.Now.ToString();
        [ObservableProperty]
        private string codeNen02 = DateTime.Now.ToString();
        [ObservableProperty]
        private string codeNen03 = DateTime.Now.ToString();
        [ObservableProperty]
        private string codeNen04 = DateTime.Now.ToString();
        [ObservableProperty]
        private string codeNen05 = DateTime.Now.ToString();
        [ObservableProperty]
        private string codeNen06 = DateTime.Now.ToString();
        [ObservableProperty]
        private string codeNen07 = DateTime.Now.ToString();
        [ObservableProperty]
        private string codeNen08 = DateTime.Now.ToString();
        [ObservableProperty]
        private string codeNen09 = DateTime.Now.ToString();
        [ObservableProperty]
        private string codeNen10 = DateTime.Now.ToString();

        [ObservableProperty]
        public Dictionary<int, string>? codeTyp1List;
        [ObservableProperty]
        public Dictionary<int, string>? codeTyp2List;

        [ObservableProperty]
        private int selectedCodeTyp1;
        [ObservableProperty]
        private int selectedCodeTyp2;

        [ObservableProperty]
        private bool isSettingsEnabled;

        public enum OptionItem1
        { 発注 = 0, 契約発注 = 1 }

        [ObservableProperty]
        private OptionItem1 optionButton1 = OptionItem1.発注;

        public void OnInit(object? init_para = null, string? init_flg = null)
        {
            OnInitBase(init_para, init_flg);

            CodeNen0 = AppData.ClassSatoo.GetDateVal(DateTime.Now,0);

            var comboItem = AppData.ClassCvnet.comboItem00;
            CodeTyp1List = comboItem.ComboItem_00<int>("商品発注区分");
            CodeTyp2List = comboItem.ComboItem_00<int>("商品発注区分");
            SelectedCodeTyp1 = CodeTyp1List.FirstOrDefault().Key;
            SelectedCodeTyp2 = CodeTyp2List.FirstOrDefault().Key;

            OptionButton1 = OptionItem1.発注;
            isSettingsEnabled = false;

            OnTouchCodeNen0();
        }

        [RelayCommand]
        public void OpenSubDlg13Nohin2()
        {
            // 1️⃣ Create a new parameter list (BizArray in your project)
            BizArray newPara = new BizArray();

            newPara[0] = CodeNen01; 
            newPara[1] = CodeNen02;
            newPara[2] = CodeNen03;
            newPara[3] = CodeNen04;
            newPara[4] = CodeNen05;
            newPara[5] = CodeNen06;
            newPara[6] = CodeNen07;
            newPara[7] = CodeNen08;
            newPara[8] = CodeNen09;
            newPara[9] = CodeNen10;
            newPara[10] = SelectedBrand;
            newPara[11] = SelectedItem1;
            newPara[12] = SelectedItem2;
            newPara[13] = SelectedCodeTyp1.ToString();
            newPara[14] = SelectedCodeTyp2.ToString();
            newPara[15] = codeNen0;
            newPara[16] = codeNen1;
            newPara[17] = codeNen2;
            newPara[18] = OptionButton1.ToString();

            // Add as many as needed
            string newFlg = "MyFlag"; // if needed


            // Create the child ViewModel
            var vm = new SubDlg13Nohin2v3150ViewModel();

            // 3️⃣ Initialize the child VM with the new parameters
            vm.OnInitBase(newPara, newFlg);

            var view = new SubDlg13Nohin2v3150View
            {
                DataContext = vm
            };

            // 5️⃣ Show NON-MODAL (or use ShowDialogView for modal)
            ClientLib.ShowWindowView(
                childWin: view,
                myVm: this,                      // Owner window
                loc: WindowStartupLocation.CenterOwner,
                isShowTaskbar: false
            );
        }

        void OnTouchCodeNen0() {

            if (string.IsNullOrWhiteSpace(CodeNen0))
                return;

            CodeNen1 = AppData.ClassSatoo.GetDateVal(DateTime.Now, 0);
            CodeNen2 = AppData.ClassSatoo.GetDateVal(DateTime.Now, 1);

            DateTime fw1 = DateTime.Now;
            DateTime fw2 = DateTime.Now;
            DateTime fw3 = DateTime.Now;
            DateTime fw4 = DateTime.Now;
            DateTime fw5 = DateTime.Now;

            DateTime lw1 = DateTime.Now;
            DateTime lw2 = DateTime.Now;
            DateTime lw3 = DateTime.Now;
            DateTime lw4 = DateTime.Now;
            DateTime lw5 = DateTime.Now;

            DateTime codeNen0Date = DateTime.Now;

            if (!DateTime.TryParse(CodeNen0, out codeNen0Date))
                return;

            fw1 = AppData.ClassCvnet.GetSysWeek(codeNen0Date);
            lw1 = fw1.AddDays(6);

            fw2= lw1.AddDays(1);
            lw2= fw2.AddDays(6);

            fw3 = lw2.AddDays(1);
            lw3 = fw3.AddDays(6);

            fw4 = lw3.AddDays(1);
            lw4 = fw4.AddDays(6);

            fw5 = lw4.AddDays(1);
            lw5 = fw5.AddDays(6);

            CodeNen01 = AppData.ClassSatoo.GetDateVal(codeNen0Date,0);
            CodeNen02 = lw1.ToString();
            CodeNen03 = fw2.ToString();
            CodeNen04 = lw2.ToString();
            CodeNen05 = fw3.ToString();
            CodeNen06 = lw3.ToString();
            CodeNen07 = fw4.ToString();
            CodeNen08 = lw4.ToString();
            CodeNen09 = fw5.ToString();
            CodeNen10 = AppData.ClassSatoo.GetDateVal(codeNen0Date, 1);
        }

        private bool TryParseYearMonth(string value, out DateTime result)
        {
            // Examples supported:
            // 2025/03
            // 202503
            // 2025-03

            string[] formats =
            {
                "yyyy/MM",
                "yyyyMM",
                "yyyy-MM"
            };

            return DateTime.TryParseExact(
                value,
                formats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out result);
        }

        [RelayCommand]
        public void SelBrand(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null)
            {
                findBrand.Code = get_sel00.Code;
                findBrand.Name = get_sel00.Name;
            }
        }

        [RelayCommand]
        public void SelItem1(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null)
            {
                findItem1.Code = get_sel00.Code;
                findItem1.Name = get_sel00.Name;
            }
        }

        [RelayCommand]
        public void SelItem2(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null)
            {
                findItem2.Code = get_sel00.Code;
                findItem2.Name = get_sel00.Name;
            }
        }


    }
}
