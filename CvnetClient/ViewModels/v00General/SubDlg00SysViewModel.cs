using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Models;
using CvnetClient.Utils;
using CvnetClient.Views;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Drawing.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg00SysViewModel : BaseViewModel
    {
        [ObservableProperty] public Dictionary<int, string>? comboCompanyTaxDeadline;
        [ObservableProperty] public Dictionary<int, string>? comboWeekDivision;
        [ObservableProperty] public Dictionary<int, string>? comboSalesFractionDivision;
        [ObservableProperty] public Dictionary<int, string>? comboPurchaseFractionClassification;
        [ObservableProperty] public Dictionary<int, string>? comboMonthlyAdjustmentFLG;
        [ObservableProperty] public List<long> comboStandardConsumptionTaxCD;
        [ObservableProperty] public MasterSysKanri selectedKanri;
        [ObservableProperty] public MasterSysKanri sysKanri;
        [ObservableProperty] private DateTime startdate;
        [ObservableProperty] private DateTime processingStartDate;
        [ObservableProperty] private DateTime taxStartDate;

        public ObservableCollection<MasterSysTax> SysTaxList { get; set; } = new ObservableCollection<MasterSysTax>();

        private Dictionary<int, string> GenerateDeadlineDaysDict()
        {
            var d = new Dictionary<int, string>();
            for (int i = 1; i <= 28; i++) d[i] = $"{i:D2}";
            d[99] = "99";
            return d;
        }
        public void OnInit(object? init_para = null, string? init_flg = null)
        {
            SysKanri = new MasterSysKanri();

            AppData.ClassCvnet.AspxSqlQuerySysMst();
            OnInitBase(init_para, init_flg);
            ComboCompanyTaxDeadline = GenerateDeadlineDaysDict();
            ComboWeekDivision = new Dictionary<int, string>
            {
                {  0, "0 月-日" },
                {  1, "1 日-土" }
            };
            ComboSalesFractionDivision = new Dictionary<int, string>
            {
                {  0, "1 卸先" },
                {  1, "3 売仕店" },
                {  2, "6 直営店" }
            };
            ComboPurchaseFractionClassification = new Dictionary<int, string>
            {
                {  0, "1 卸先" },
                {  1, "3 売仕店" },
                {  2, "6 直営店" }
            };
            ComboMonthlyAdjustmentFLG = new Dictionary<int, string>
            {
                {  0, "0 ノーチェック" },
                {  1, "1 月を越えての修正不可" }
            };
            sysKanri.MonthlyAdjustmentFLG = ComboMonthlyAdjustmentFLG.FirstOrDefault().Key;
            ComboStandardConsumptionTaxCD = new List<long> { 1, 2, 3 };
            LoadSysMst();
            LoadSysTax();
        }
        private void LoadSysMst()
        {
            try
            {                
                var table = AppData.ClassCvnet.SysMst._data;
                if (table.Rows.Count == 0) return;
                var row = table.Rows[0];
                
                SysKanri.SeqNo = Convert.ToInt64(row["SEQ_NO"]);
                SysKanri.VdateCreate = Convert.ToDecimal(row["VDATE_CREATE"]);
                SysKanri.VdateUpdate = Convert.ToDecimal(row["VDATE_UPDATE"]);

                SysKanri.CompanyName = row["自社名"].ToString();
                SysKanri.PostalCode = row["郵便番号"].ToString();
                SysKanri.Address1 = row["住所1"].ToString();
                SysKanri.Address2 = row["住所2"].ToString();
                SysKanri.Address3 = row["住所3"].ToString();

                SysKanri.Tel = row["TEL"].ToString();
                SysKanri.Fax = row["FAX"].ToString();
                SysKanri.CompanyMail = row["管理者MAIL"].ToString();
                SysKanri.HomePage = row["ホームページ"].ToString();
                SysKanri.MailServer = row["SMTPSERVER"].ToString();

                SysKanri.ProductImageDirectory = row["商品画像DIR"].ToString();
                SysKanri.SYSImageDirectory = row["SYS画像DIR"].ToString();
                string date = (string)row["期首年月日"];
                Startdate = VDateHelper.ConvToDate(date);
                SysKanri.CompanyClosingDate = Convert.ToInt32(row["自社締日"]);

                SysKanri.AmendmentPeriod = Convert.ToInt32(row["修正有効日数"]);
                SysKanri.AdvancePeriod = Convert.ToInt32(row["先付有効日数"]);

                string date2 = (string)row["処理開始日"];
                ProcessingStartDate = VDateHelper.ConvToDate(date2);
                SysKanri.WeekDivision = Convert.ToInt32(row["週区分"]);
                SysKanri.SalesFractionDivision = Convert.ToInt32(row["売上端数区分"]);
                SysKanri.PurchaseFractionClassification = Convert.ToInt32(row["仕入端数区分"]);

                SysKanri.StandardWarehouseCD = row["標準倉庫CD"].ToString();
                SysKanri.StandardConsumptionTaxCD = Convert.ToInt64(row["標準消費税CD"]);
                SysKanri.ConsumptionTaxProductCD = row["消費税商品CD"].ToString();

                SysKanri.BankTransferDestination1 = row["振込先1"].ToString();
                SysKanri.BankTransferDestination2 = row["振込先2"].ToString();
                SysKanri.BankTransferDestination3 = row["振込先3"].ToString();

                SysKanri.RegistrationNumber = row["登録番号"].ToString();
            }
            catch
            {

            }
        }
        partial void OnSelectedKanriChanged(MasterSysKanri? value)
        {
            if (value != null)
            {
                SysKanri = CvnetBaseCore.Common.CloneObject(value);
            }
            else
                SysKanri = null;
        }
        private void LoadSysTax()
        {
            SysTaxList.Clear();

            foreach (DataRow row in AppData.ClassCvnet.SysTax.Rows)
            {
                SysTaxList.Add(new MasterSysTax
                {
                    SeqNo = Convert.ToInt64(row["SEQ_NO"]),
                    VdateUpdate = Convert.ToDecimal(row["VDATE_UPDATE"]),
                    TaxCd = Convert.ToInt32(row["消費税CD"]),
                    NewTaxStartDate = VDateHelper.ConvToDate(row["新消費税開始日"].ToString()),
                    TaxRate = Convert.ToDecimal(row["消費税率"]),
                    NewTaxRate = Convert.ToDecimal(row["新消費税率"])
                });
            }
        }
        [RelayCommand]
        void DoUpdate()
        {
            if (!ClientLib.MessageBox(this, "修正しますか？")) return;
            if (SelectedKanri != null)
            {
                UpdateKanri();
            }
            else
            {
                UpdateSysTax();
            }
        }
        private void UpdateKanri()
        {
            var item = Common.CloneObject(SysKanri);
            Common.ConvertDotStringAdd(item);
            if (item == null) return;
            var ret = AppData.Http!.AspxSqlExe(DBDef.DB_DML.UPDATE, "Master_SYSKANRI", item.SeqNo, item.VdateUpdate.ToString(),
                new string[] { "自社名", "郵便番号", "住所1", "住所2", "住所3", "TEL", "FAX", "管理者MAIL", "ホームページ", "SMTPSERVER", "商品画像DIR", "SYS画像DIR", "自社締日", "修正有効日数", "先付有効日数", "週区分", "売上端数区分", "仕入端数区分", "標準倉庫CD", "標準消費税CD", "消費税商品CD", "振込先1", "振込先2", "振込先3", "登録番号" },
                new string[] { item.CompanyName!, item.PostalCode!, item.Address1!, item.Address2!, item.Address3!, item.Tel!, item.Fax!, item.CompanyMail!, item.HomePage!, item.MailServer!, item.ProductImageDirectory!, item.SYSImageDirectory!, item.CompanyClosingDate?.ToString(), item.AmendmentPeriod?.ToString(), item.AdvancePeriod?.ToString(), item.WeekDivision?.ToString(), item.SalesFractionDivision?.ToString(), item.PurchaseFractionClassification?.ToString(), item.StandardWarehouseCD!, item.StandardConsumptionTaxCD?.ToString(), item.BankTransferDestination1!, item.BankTransferDestination2!, item.BankTransferDestination3!, item.RegistrationNumber! });
            if (ret.Code == 0)
            {
                Common.ConvertDotStringDel(item);

                    SelectedKanri.VdateUpdate = VDateHelper.ToVDate(DateTime.Now);
                    SelectedKanri.CompanyName = item.CompanyName;
                    SelectedKanri.PostalCode = item.PostalCode;
                    SelectedKanri.Address1 = item.Address1;
                    SelectedKanri.Address2 = item.Address2;
                    SelectedKanri.Address3 = item.Address3;
                    SelectedKanri.Tel = item.Tel;
                    SelectedKanri.Fax = item.Fax;
                    SelectedKanri.CompanyMail = item.CompanyMail;
                    SelectedKanri.HomePage = item.HomePage;
                    SelectedKanri.MailServer = item.MailServer;
                    SelectedKanri.ProductImageDirectory = item.ProductImageDirectory;
                    SelectedKanri.SYSImageDirectory = item.SYSImageDirectory;
                    SelectedKanri.CompanyClosingDate = item.CompanyClosingDate;
                    SelectedKanri.AmendmentPeriod = item.AmendmentPeriod;
                    SelectedKanri.AdvancePeriod = item.AdvancePeriod;
                    SelectedKanri.WeekDivision = item.WeekDivision;
                    SelectedKanri.SalesFractionDivision = item.SalesFractionDivision;
                    SelectedKanri.PurchaseFractionClassification = item.PurchaseFractionClassification;
                    SelectedKanri.StandardWarehouseCD = item.StandardWarehouseCD;
                    SelectedKanri.StandardConsumptionTaxCD = item.StandardConsumptionTaxCD;
                    SelectedKanri.BankTransferDestination1 = item.BankTransferDestination1;
                    SelectedKanri.BankTransferDestination2 = item.BankTransferDestination2;
                    SelectedKanri.BankTransferDestination3 = item.BankTransferDestination3;
                    SelectedKanri.RegistrationNumber = item.RegistrationNumber;
                    SysKanri = Common.CloneObject(SelectedKanri);
                }
        }
        private void UpdateSysTax()
        {
            foreach (var item in SysTaxList)
            {
                var clone = Common.CloneObject(item);
                Common.ConvertDotStringAdd(clone);

                var ret = AppData.Http!.AspxSqlExe(
                    DBDef.DB_DML.UPDATE,
                    "Master_SYSTAX",
                    clone.SeqNo,
                    clone.VdateUpdate.ToString(),
                    new[] { "消費税CD", "消費税率", "新消費税開始日", "新消費税率" },
                    new[] { clone.TaxCd.ToString(), clone.TaxRate.ToString(), clone.NewTaxStartDate.ToString(), clone.NewTaxRate.ToString()
                    });

                if (ret.Code != 0)
                {
                    ClientLib.MessageBoxError(this, "データが修正出来ませんでした");
                    return;
                }

                Common.ConvertDotStringDel(clone);
            }
        }

        [RelayCommand]
        async Task DoPrintAsync()
        {
            if (!ClientLib.MessageBox(this, "印刷しますか？")) return;
            ClientLib.CursorToWait();
            if (SysTaxList == null || SysKanri == null) return;

            var resp = AppData.Http!.AspxSqlQueryCsv("_syskanriprint", null, "cvnet_doc_syskanri.qfm");
            var lines = resp.Split('\n');

            if (lines.Length < 2 || lines[1] == "0")
            {
                ClientLib.MessageBoxError(this, "PDFデータがありません。");
                ClientLib.CursorToNormal();
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

            // PDF表示
            var win = new WebpdfView();
            if (win.DataContext is WebpdfViewModel vm)
                vm.Pdfdata = url;

            ClientLib.CursorToNormal();
            ClientLib.ShowDialogView(win, this);
        }
    }
}
