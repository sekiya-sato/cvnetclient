using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Models;
using CvnetClient.Utils;
using CvnetClient.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg02SirHenpinViewModel : BaseViewModel
    {
        public enum PrintType { 通常発行,再発行}
        [ObservableProperty]
        SearchCondition? condition;
        [ObservableProperty]
        public Dictionary<string, string>? comboList1;
        [ObservableProperty]
        public Dictionary<string,string>? comboList2;
        [ObservableProperty]
        public string? insatsu_flg = "0";
        public void OnInit() 
        { 
            Condition = new SearchCondition();

            ComboList1 = new Dictionary<string, string>
            {
                {  "2", "2 生地付属仕入" },
                {  "3", "3 商品仕入" }
            };
            Condition.SelectedCombo1 = ComboList1.FirstOrDefault().Key;

            ComboList2 = new Dictionary<string, string>
            {
                {  "10", "10 仕入" },
                {  "15", "15 消化仕入" },
                {  "20", "20 仕入返品" },
                {  "25", "25 消化仕入返品" },
                {  "30", "30 値引" },
                {  "99", "99 消費税" }
            };
            Condition.SelectedCombo2 = ComboList2.FirstOrDefault().Key;

            Condition.SupplyTo = new BtListHelper("9999999999", ""); 
            Condition.WareTo = new BtListHelper("00106", "木村洋服店");
            Condition.WareFrom = new BtListHelper("00106", "木村洋服店");

            Condition.SlipNoFrom = 0;
            Condition.SlipNoTo = 9999999999;

            Condition.DateFrom = DateTime.Now;
            Condition.DateTo = DateTime.Now.AddDays(1);

            Condition.SelectedCombo1 = "3";
            Condition.SelectedCombo2 = "20";
        }

        [RelayCommand]
        public void SelSupplier1(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Condition != null)
            {
                Condition.SupplyFrom = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }

        [RelayCommand]
        public void SelSupplier2(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Condition != null)
            {
                Condition.SupplyTo = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }

        [RelayCommand]
        public void SelWare1(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Condition != null)
            {
                Condition.WareFrom = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }

        [RelayCommand]
        public void SelWare2(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Condition != null)
            {
                Condition.WareTo = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }

        [RelayCommand]
        async Task DoPrintAsync()
        {
            if (Condition.SelectedCombo1 == "" && Condition.SelectedCombo2 == "")
            {
                ClientLib.MessageBoxError(this, "伝票処理区分/取引区分を選択してください");
                return;
            }
            else if (Condition.SelectedCombo1 == "")
            {
                ClientLib.MessageBoxError(this, "伝票処理区分を選択してください");
                return;
            }
            
            else if (Condition.SelectedCombo2 == "")
            {
                ClientLib.MessageBoxError(this, "取引区分を選択してください");
                return;
            }           
            
            var wrk_para = new string[14];
            wrk_para[0] = Condition.SelectedCombo1;                   
            wrk_para[1] = Condition.DateFrom?.ToString("yyyyMMdd") ?? string.Empty;     
            wrk_para[2] = Condition.DateTo?.ToString("yyyyMMdd") ?? string.Empty;
            wrk_para[3] = Condition.SupplyFrom?.Code ?? string.Empty;             
            wrk_para[4] = Condition.SupplyTo.Code;
            wrk_para[5] = Condition.WareFrom.Code;              
            wrk_para[6] = Condition.WareTo.Code;
            wrk_para[7] = Condition.SlipNoFrom.ToString();                              
            wrk_para[8] = Condition.SlipNoTo.ToString();
            wrk_para[9] = Condition.SelectedCombo2;
            if (Condition.SelectedPrint.ToString() == "通常発行")
            {
                wrk_para[10] = "0";
            }
            else 
            {
                wrk_para[10] = "1";
            }

            wrk_para[11] = Condition.ManualInpFrom.ToString();
            wrk_para[12] = Condition.ManualInpFrom.ToString();
            wrk_para[13] = Insatsu_flg;


            var qfm_file = "cvnet02prn_sirhenpin.qfm";
            
            if (AppData.ClassCvnet.config.usegenka == 0) qfm_file = "cvnet02prn_sirhenpin_g.qfm";
            var ret_csv = AppData.Http!.AspxSqlQueryCsv("_p_sirhenpin01", wrk_para, qfm_file, 00);
            var lines = ret_csv.Split('\n');

            if (lines.Length < 2 || lines[1] == "0")
            {
                ClientLib.MessageBoxError(this, "PDFデータがありません");
                return;
            }

            string pdfPath = lines[0];
            string url = AppData.Http.URLroot + pdfPath + "/data.pdf";

            bool ready = await Utils.GlobalFunc.WaitForPdfAsync(url, TimeSpan.FromSeconds(30));
            if (!ready)
            {
                ClientLib.MessageBoxError(this, "PDF生成に時間がかかりすぎています。\n 条件を絞ってください。");
                return;
            }

            var win = new WebpdfView();
            if (win.DataContext is WebpdfViewModel vm)
            {
                vm.Pdfdata = url;
            }
            ClientLib.CursorToNormal();
            ClientLib.ShowDialogView(win, this);
        }
        public partial class SearchCondition : ObservableObject 
        {
            [ObservableProperty]
            private PrintType selectedPrint = PrintType.通常発行;
            [ObservableProperty]
            private string? selectedCombo1;
            [ObservableProperty]
            private DateTime? dateFrom;
            [ObservableProperty] 
            private DateTime? dateTo;
            [ObservableProperty]
            BtListHelper? supplyFrom;
            [ObservableProperty]
            BtListHelper? supplyTo;
            [ObservableProperty]
            BtListHelper? wareFrom;
            [ObservableProperty]
            BtListHelper? wareTo;
            [ObservableProperty]
            private long? slipNoFrom;
            [ObservableProperty]
            private long? slipNoTo;
            [ObservableProperty]
            private long? manualInpFrom;
            [ObservableProperty]
            private long? manualInpTo;
            [ObservableProperty]
            private string? selectedCombo2;
        }
    }
}
