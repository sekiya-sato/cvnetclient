using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Models;
using CvnetClient.Views;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Controls;

namespace CvnetClient.ViewModels
{
    public enum BarcodeType { CODE39, NW7 }
    public partial class SubDlg00UsrlistViewModel : BaseViewModel
    {
        [ObservableProperty] MasterWorker? workerFrom = new();
        [ObservableProperty] MasterWorker? workerTo = new();
        [ObservableProperty] MasterShop? shopFrom = new();
        [ObservableProperty] MasterShop? shopTo = new();

        [ObservableProperty]
        private BarcodeType selectedBarcode = BarcodeType.CODE39;

        [ObservableProperty]
        private string fileName = string.Empty;

        [RelayCommand]
        private void SelWorker(string? target)
        {
            var get_sel00 = AppData.DlgService.GetSel00("担当");
            if (get_sel00 == null) return;

            var cd = get_sel00.SelectSel00?.Code;
            var name = get_sel00.SelectSel00?.Name;

            switch (target)
            {
                case "From":
                    if (WorkerFrom != null)
                    {
                        WorkerFrom.WorkerCD = cd;
                        WorkerFrom.Name = name;
                    }
                    break;

                case "To":
                    if (WorkerTo != null)
                    {
                        WorkerTo.WorkerCD = cd;
                        WorkerTo.Name = name;
                    }
                    break;
            }

            OnPropertyChanged(nameof(WorkerFrom));
            OnPropertyChanged(nameof(WorkerTo));
        }

        [RelayCommand]
        private void SelShop(string? target)
        {
            var get_sel00 = AppData.DlgService.GetSel00("店舗出荷倉庫");
            if (get_sel00 == null) return;

            var cd = get_sel00.SelectSel00?.Code;
            var name = get_sel00.SelectSel00?.Name;

            switch (target)
            {
                case "From":
                    if (ShopFrom != null)
                    {
                        ShopFrom.TradingCD = cd;
                        ShopFrom.TradingName = name;
                    }
                    break;

                case "To":
                    if (ShopTo != null)
                    {
                        ShopTo.TradingCD = cd;
                        ShopTo.TradingName = name;
                    }
                    break;
            }

            OnPropertyChanged(nameof(ShopFrom));
            OnPropertyChanged(nameof(ShopTo));
        }

        [RelayCommand]
        private void RadioChanged(string? barcode)
        {
            if (Enum.TryParse(barcode, out BarcodeType parsed))
            {
                SelectedBarcode = parsed;
            }
        }

        [RelayCommand]
        async Task DoPrintAsync()
        {
            string[] wrk_para = new string[1];
            wrk_para[0] = "Data/img";
            
            var ret_csv = AppData.Http!.AspxSqlQuery2("get_img_path",wrk_para,"",-1);
            var ret_csv1 = ret_csv.Split('\n');
            string image_path = ret_csv1[0].ToString() + "\\";
            string printsql = "select A.社員CD,A.名前,'" + image_path + "'||nvl(A.携帯TEL,'.') 画像,A.店舗CD," +
            "NVL((select H.得意先名 from HC$master_tokui H where H.得意先CD=A.店舗CD),'.') 店舗名," +
            "NVL((Select 自社名 From HC$MASTER_SYSKANRI where rownum<2),'.') 自社名,decode(A.携帯TEL,'.',0,1) 画像表示判定用 " +
            "from HC$master_shain A where A.社員CD between :1 and :2 and A.店舗CD between :3 and :4 order by A.社員CD";

            if (!ClientLib.MessageBox(this, "印刷しますか？")) return;
            ClientLib.CursorToWait();
            var param = new string[4];
            param[0] = workerFrom.WorkerCD?.ToString();
            if (param[0] == null) {
                param[0] = "0";
            }
            param[1] = workerTo.WorkerCD?.ToString();
            if (param[1] == null)
            {
                param[1] = "99999999";
            }
            param[2] = shopFrom.TradingCD?.ToString();
            if (param[2] == null)
            {
                param[2] = "0";
            }
            param[3] = shopTo.TradingCD?.ToString();
            if (param[3] == null)
            {
                param[3] = "99999999";
            }
            if (SelectedBarcode == BarcodeType.CODE39) {
                FileName = "cvnetfelica39_v2.qfm";
            } else if (SelectedBarcode == BarcodeType.NW7) {
                FileName = "cvnetfelica_v2.qfm";
            }
                var ret = AppData.Http!.AspxSqlQueryCsv(string.Format(printsql), param, FileName);
            if (ret.Split('\n').Length < 2)
            {
                ClientLib.MessageBoxError(this, "PDFデータがありません");
                return;
            }
            var ret1 = ret.Split('\n');
            var url = AppData.Http.URLroot + ret1[0] + "/data.pdf";
            await Task.Delay(1500); // PDF生成待ち
            var win = new WebpdfView();
            var vm = win.DataContext as WebpdfViewModel;
            if (vm == null) return;
            vm.Pdfdata = url;
            ClientLib.CursorToNormal();
            ClientLib.ShowDialogView(win, this);
        }
    }
}
