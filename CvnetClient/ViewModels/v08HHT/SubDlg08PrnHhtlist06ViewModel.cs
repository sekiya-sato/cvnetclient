using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Models;
using CvnetClient.Utils;
using CvnetClient.Views;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg08PrnHhtlist06ViewModel : BaseViewModel
    {
        #region Declare
        public enum PrintType { Regular, Reissue}
        [ObservableProperty]
        SearchCondition? condition;
        #endregion
        #region Initialize
        public void OnInit(object? init_para = null, string? init_flg = null) 
        {
            OnInitBase(init_para, init_flg);
            Condition = new SearchCondition();
            Condition.ReceiptTo = new BtListHelper("99999999", "");
            Condition.DateFrom = DateTime.Now;
            Condition.DateTo = DateTime.Now;
            Condition.NumberFrom = 0;
            Condition.NumberTo = 9999999999;
        }
        #endregion
        #region Function
        [RelayCommand]
        public void SelShipment1(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Condition != null)
            {
                Condition.ShipmentFrom = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        [RelayCommand]
        public void SelShipment2(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Condition != null)
            {
                Condition.ShipmentTo = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        [RelayCommand]
        public void SelReceipt1(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Condition != null)
            {
                Condition.ReceiptFrom = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        [RelayCommand]
        public void SelReceipt2(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Condition != null)
            {
                Condition.ReceiptTo = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        [RelayCommand]
        async Task DoPrintAsync() 
        {
            if (Condition == null) { return; }
            var qs = "_ido_meisaisho_sokuji";
            string[] wrk_para = new string[11];
            wrk_para[0] = Condition.ShipmentFrom?.Code ?? string.Empty;       /* 出庫倉庫 */
            wrk_para[1] = Condition.ShipmentTo?.Code ?? string.Empty;
            wrk_para[2] = Condition.ReceiptFrom?.Code ?? string.Empty;     /* 入庫先 */
            wrk_para[3] = Condition.ReceiptTo?.Code ?? string.Empty;
            wrk_para[4] = Condition.DateFrom?.ToString("yyyyMMdd") ?? "";       /* 移動日 */
            wrk_para[5] = Condition.DateTo?.ToString("yyyyMMdd") ?? "";
            wrk_para[6] = Condition.NumberFrom?.ToString() ?? string.Empty;      /* 伝票NO */
            wrk_para[7] = Condition.NumberTo?.ToString() ?? string.Empty;

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
            
            var chk_sql = "select count(*) from hc$master_meisho m where m.名称区分='YOK' and m.名称CD='003'";
            var chk_csv = AppData.Http!.AspxSqlQuery(chk_sql, new string[] { });
            if (chk_csv.Rows[0][0].ToString() == "1") qfm_file = "cvnet60prn02_yk.qfm";
            var ret_csv = AppData.Http!.AspxSqlQueryCsv(qs, wrk_para, qfm_file, 0);

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
        #endregion
        public partial class SearchCondition : ObservableObject
        {
            [ObservableProperty]
            private PrintType selectedPrint = PrintType.Regular;
            [ObservableProperty]
            private BtListHelper? shipmentFrom;
            [ObservableProperty]
            private BtListHelper? shipmentTo;
            [ObservableProperty]
            private BtListHelper? receiptFrom;
            [ObservableProperty]
            private BtListHelper? receiptTo;           
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
}
