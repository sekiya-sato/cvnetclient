using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Models;
using CvnetClient.Views;
using static CvnetClient.ViewModels.SubDlg08PrnHhtlist06ViewModel;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg08PrnHhtlist06ViewModel : BaseViewModel
    {
        public enum PrintType { 通常発行, 再発行}

        [ObservableProperty]
        SearchCondition? condition;        
        public void OnInit() 
        {
            Condition = new SearchCondition();
            //Condition.ShipmentFrom = "0";
            //Condition.ShipmentTo = "99999999";
            //Condition.ReceiptFrom = "0";
            Condition.ReceiptTo = "99999999";
            Condition.DateFrom = DateTime.Now;
            Condition.DateTo = DateTime.Now;
            Condition.NumberFrom = 0;
            Condition.NumberTo = 9999999999;
        }

        [RelayCommand]
        public void SelShipment1(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Condition != null)
            {
                Condition.ShipmentFrom = get_sel00.Code;
                Condition.ShipmentFromName = get_sel00.Name;
            }
        }
        [RelayCommand]
        public void SelShipment2(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Condition != null)
            {
                Condition.ShipmentTo = get_sel00.Code;
                Condition.ShipmentToName = get_sel00.Name;
            }
        }
        [RelayCommand]
        public void SelReceipt1(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Condition != null)
            {
                Condition.ReceiptFrom = get_sel00.Code;
                Condition.ReceiptFromName = get_sel00.Name;
            }
        }
        [RelayCommand]
        public void SelReceipt2(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Condition != null)
            {
                Condition.ReceiptTo = get_sel00.Code;
                Condition.ReceiptToName = get_sel00.Name;
            }
        }

        [RelayCommand]
        async Task DoPrintAsync() 
        {
            var qs = "_ido_meisaisho_sokuji";
            string[] wrk_para = new string[11];
            wrk_para[0] = Condition.ShipmentFrom ?? "";       /* 出庫倉庫 */
            wrk_para[1] = Condition.ShipmentTo ?? "";
            wrk_para[2] = Condition.ReceiptFrom ?? "";     /* 入庫先 */
            wrk_para[3] = Condition.ReceiptTo ?? "";
            wrk_para[4] = Condition.DateFrom?.ToString("yyyyMMdd") ?? "";       /* 移動日 */
            wrk_para[5] = Condition.DateTo?.ToString("yyyyMMdd") ?? "";
            wrk_para[6] = Condition.NumberFrom.ToString();      /* 伝票NO */
            wrk_para[7] = Condition.NumberTo.ToString();

            if (Condition.SelectedPrint.ToString() == "通常発行")
            {
                wrk_para[8] = "0";
            }
            else {
                wrk_para[8] = "1";
            }
            wrk_para[9] = "1";
            wrk_para[10] = "0";

            var qfm_file = "cvnet60prn02.qfm";
            /* 2015.07.07 #2467対応（横レイアウト追加） */
            var chk_sql = "select count(*) from hc$master_meisho m where m.名称区分='YOK' and m.名称CD='003'";
            var chk_csv = AppData.Http!.AspxSqlQuery(chk_sql, null);
            if (chk_csv.Rows[0][0].ToString() == "1") qfm_file = "cvnet60prn02_yk.qfm";
            var ret_csv = AppData.Http!.AspxSqlQuery(qs, wrk_para, qfm_file, 0);
            if (ret_csv.Rows.Count == 0) {
                ClientLib.MessageBoxError(this, "PDFデータがありません");
                return;
            }
            //var lines = ret_csv.Split('\n');

            //if (lines.Length < 2 || lines[1] == "0")
            //{
            //    ClientLib.MessageBoxError(this, "PDFデータがありません");
            //    return;
            //}

            string pdfPath = ret_csv.Rows[0][0].ToString() ?? "";
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
    }

    public partial class SearchCondition : ObservableObject 
    {
        [ObservableProperty]
        private PrintType selectedPrint = PrintType.通常発行;
        [ObservableProperty]
        private string? shipmentFrom;
        [ObservableProperty]
        private string? shipmentTo;
        [ObservableProperty]
        private string? shipmentFromName;
        [ObservableProperty]
        private string? shipmentToName;
        [ObservableProperty]
        private string? receiptFrom;
        [ObservableProperty]
        private string? receiptTo;
        [ObservableProperty]
        private string? receiptFromName;
        [ObservableProperty]
        private string? receiptToName;
        [ObservableProperty]
        private DateTime? dateFrom;
        [ObservableProperty]
        private DateTime? dateTo;
        [ObservableProperty]
        private long? numberFrom;
        [ObservableProperty]
        private long? numberTo;
    }
}
