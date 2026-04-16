using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Class;
using CvnetClient.Models;
using CvnetClient.Utils;
using CvnetClient.Views;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Windows;
using System.Windows.Media;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg30PrnYosanViewModel : BaseViewModel
    {
        [ObservableProperty] BtListHelper? selectedShop1;

        [ObservableProperty] BtListHelper? selectedShop2;

        [ObservableProperty] BtListHelper? selectedBrand1;

        [ObservableProperty] BtListHelper? selectedBrand2;

        [ObservableProperty] private string selectedDate;

        [ObservableProperty] private string startDate;

        [ObservableProperty] private string endDate;

        [ObservableProperty] private bool showSelectBrandTitle = true;
        [ObservableProperty] private bool showSelectBrand = true;

        [ObservableProperty] private bool isTenpo1Enabled = true;
        [ObservableProperty] private bool isTenpo2Enabled = true;
        [ObservableProperty] private bool isTenpo1BtListEnabled = true;
        [ObservableProperty] private bool isTenpo2BtListEnabled = true;

        [ObservableProperty] private bool isShopsEnabled = true;
        [ObservableProperty] private bool isBrandEnabled = true;
        [ObservableProperty] private bool isExistingNewShopEnabled = true;
        [ObservableProperty] private bool isAllShopsEnabled = true;

        [ObservableProperty] private bool showShops = true;
        [ObservableProperty] private bool showBrand = true;
        [ObservableProperty] private bool showExistingNewShop = true;
        [ObservableProperty] private bool showAllShops = true;

        public enum OutputCategory
        { Shops = 0, Brand = 1, ExistingNewShop = 2, AllShops = 3 }
        [ObservableProperty]
        private OutputCategory selectedOutput = OutputCategory.Shops;  //ischecked

        public enum YearToYearComparison
        { Date = 0, Day = 1 }
        [ObservableProperty]
        private YearToYearComparison selectedYoY = YearToYearComparison.Date;

        public enum OutputType
        { CSV = 0, Spool = 1 }
        [ObservableProperty]
        private OutputType selectedType = OutputType.Spool;


        public void OnInit(object? init_para = null, string? init_flg = null)
        {
            OnInitBase(init_para, init_flg);
            SelectedDate = DateTime.Now.ToString();
            AppData.ClassCvnet.AspxSqlQueryImp();
            if (para.Count > 0)
            {
                if (para[0] == "1")
                {
                    //Form1.OptionButton2.Active = $false;
                    /* FLG参照　09.10.19 */
                    if (AppData.ClassCvnet.config.TenpoFix == 1)
                    {
                        IsTenpo1Enabled = false;
                        IsTenpo2Enabled = false;
                        IsTenpo1BtListEnabled = false;
                        IsTenpo2BtListEnabled = false;
                    }

                    //Form1.CodeNen9 = sysdate();
                    //Form1.CodeNen9.OnTouch();

                    if (AppData.ClassCvnet.SysImp.Rows[1][1] == "")
                    {
                        SelectedShop1 = new BtListHelper(AppData.ClassCvnet.SysImp.Rows[0][1].ToString(), AppData.ClassCvnet.SysImp.Rows[0][2].ToString());
                        SelectedShop2 = new BtListHelper(AppData.ClassCvnet.SysImp.Rows[0][1].ToString(), AppData.ClassCvnet.SysImp.Rows[0][2].ToString());
                    }
                    else
                    {
                        SelectedShop1 = new BtListHelper(AppData.ClassCvnet.SysImp.Rows[1][1].ToString(), AppData.ClassCvnet.SysImp.Rows[1][2].ToString());
                        SelectedShop2 = new BtListHelper(AppData.ClassCvnet.SysImp.Rows[1][1].ToString(), AppData.ClassCvnet.SysImp.Rows[1][2].ToString());
                    }
                }
            }


            if (AppData.ClassCvnet.config.YosanFlg2 == 1)
            {
                ShowSelectBrandTitle = true;
                ShowSelectBrand = true;
            }

            if (AppData.ClassCvnet.config.UserFlg == 17) ShowExistingNewShop = true;

            para = new BizArray();
            StartDate = DateTime.Now.AddDays(-7).ToString("yyyyMMdd");
            EndDate = DateTime.Now.ToString("yyyyMMdd");
        }

        [RelayCommand]
        async Task DoPrintAsync()
        {
            if (!ClientLib.MessageBox(this, "印刷しますか？")) return;
            ClientLib.CursorToWait();

            int paraCnt = 6;
            var v_para = new BizArray();
            v_para[0] = StartDate;
            v_para[1] = EndDate;
            v_para[2] = SelectedShop1?.Code ?? ".";
            v_para[3] = SelectedShop2?.Code ?? "99999999";
            v_para[4] = SelectedBrand1?.Code ?? ".";
            v_para[5] = SelectedBrand2?.Code ?? "ZZZZZZZZZZZZZZZZZZZZ";
            v_para[6] = ((int)SelectedOutput).ToString();
            v_para[7] = ((int)SelectedYoY).ToString();
            v_para[8] = "";

            var ret_csv = OnQuery(v_para);

            var lines = ret_csv.Split('\n');
            var retNo = int.Parse(lines[1]);
            if (lines.Length < 2 || retNo <= 0)
            {
                ClientLib.MessageBoxError(this, "印刷データがありませんでした");
                return;
            }
            string pdfPath = lines[0];

            if (SelectedType == OutputType.Spool)
            {

                await Task.Delay(1500); // PDF生成待ち

                string url = AppData.Http.URLroot + pdfPath + "/data.pdf";

                bool ready = await Utils.GlobalFunc.WaitForPdfAsync(url, TimeSpan.FromSeconds(1500));
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
            else
            {
                var csv_para = new BizCsvDocument();
                string datapath = AppData.Http.URLroot + pdfPath + "/data.txt";
                string headpath = AppData.Http.URLroot + pdfPath + "/d_sql.txt";
                bool ready = await Utils.GlobalFunc.WaitForPdfAsync(datapath, TimeSpan.FromSeconds(1500));
                if (!ready)
                {
                    ClientLib.MessageBoxError(this, "Data生成に時間がかかりすぎています。\n 条件を絞ってください。");
                    return;
                }

                ready = await Utils.GlobalFunc.WaitForPdfAsync(headpath, TimeSpan.FromSeconds(1500));
                if (!ready)
                {
                    ClientLib.MessageBoxError(this, "時間がかかりすぎです。\n 条件を絞ってください。");
                    return;
                }
                await csv_para.LoadFromUrlAsync(datapath, headpath);
                try
                {
                    csv_para.SaveCsv(DateTime.Now.ToString("yyyyMMdd") + "-予算表.csv");
                }
                catch (Exception ex) { }
            }

        }

        string OnQuery(BizArray wrk_para)
        {
            var qfm_file = "cvnet30prn_yosan.qfm";

            return AppData.Http!.AspxSqlQueryCsv("_30_yosan", wrk_para.ToArray(), qfm_file);
        }

        partial void OnSelectedDateChanged(string value)
        {
            if (DateTime.TryParse(value, out DateTime selDate))
            {
                StartDate = AppData.ClassSatoo.GetDateVal(selDate, 0);
                EndDate = AppData.ClassSatoo.GetDateVal(selDate, 1);
            }
            else {
                StartDate = AppData.ClassSatoo.GetDateVal(DateTime.Now, 0);
                EndDate = AppData.ClassSatoo.GetDateVal(DateTime.Now, 1);
            }
        }

        [RelayCommand]
        public void SelTenpo1(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null)
            {
                SelectedShop1 = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }

        [RelayCommand]
        public void SelTenpo2(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null)
            {
                SelectedShop2 = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }

        [RelayCommand]
        public void SelBrand1(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null)
            {
                SelectedBrand1 = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }

        [RelayCommand]
        public void SelBrand2(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null)
            {
                SelectedBrand2 = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
    }
}
