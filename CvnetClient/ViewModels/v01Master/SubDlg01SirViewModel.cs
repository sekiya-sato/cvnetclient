using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Models;
using CvnetClient.Utils;
using CvnetClient.Views;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg01SirViewModel : BaseViewModel
    {
        [ObservableProperty] ObservableCollection<MasterSupplier>? listSupplier;
        [ObservableProperty] MasterSupplier? selectedSupplier;
        [ObservableProperty] MasterSupplier? editSupplier;
        [ObservableProperty] string? startCode;
        [ObservableProperty] private bool isAutoOrderVisible;

        // All combos are public and properly initialized
        [ObservableProperty] public Dictionary<int, string>? comboOrderFlag;
        [ObservableProperty] public Dictionary<string, string>? comboDepartment;
        [ObservableProperty] public Dictionary<int, string>? comboOrderStopFlag;
        [ObservableProperty] public Dictionary<int, string>? comboPurchaseType;
        [ObservableProperty] public Dictionary<string, string>? comboCorporationCd;
        [ObservableProperty] public Dictionary<string, string>? comboLinkedCorporationCd;
        [ObservableProperty] public Dictionary<string, string>? comboPaymentDestinationCd;
        [ObservableProperty] public Dictionary<int, string>? comboPaymentPrintFlag;
        [ObservableProperty] public Dictionary<string, string>? comboPaymentMethod;
        [ObservableProperty] public Dictionary<int, string>? comboClosingDay;
        [ObservableProperty] public Dictionary<int, string>? comboClosingDay2;
        [ObservableProperty] public Dictionary<int, string>? comboClosingDay3;
        [ObservableProperty] public Dictionary<int, string>? comboScheduledPaymentMonth;
        [ObservableProperty] public Dictionary<int, string>? comboScheduledPaymentMonth2;
        [ObservableProperty] public Dictionary<int, string>? comboScheduledPaymentMonth3;
        [ObservableProperty] public Dictionary<int, string>? comboScheduledPaymentDay;
        [ObservableProperty] public Dictionary<int, string>? comboScheduledPaymentDay2;
        [ObservableProperty] public Dictionary<int, string>? comboScheduledPaymentDay3;
        [ObservableProperty] public Dictionary<int, string>? comboTaxCd;                 
        [ObservableProperty] public Dictionary<int, string>? comboTaxRounding;
        [ObservableProperty] public Dictionary<int, string>? comboTaxCalculationMethod;
        [ObservableProperty] public Dictionary<int, string>? comboSlipIssueType;       
        [ObservableProperty] public Dictionary<string, string>? comboCurrencyType;
        [ObservableProperty] public Dictionary<int, string>? comboCurrencyRoundingDigit;
        [ObservableProperty] public Dictionary<int, string>? comboCurrencyFractionType;
        [ObservableProperty] public Dictionary<string, string>? comboNameCd01;
        [ObservableProperty] public Dictionary<string, string>? comboNameCd02;
        [ObservableProperty] public Dictionary<string, string>? comboNameCd03;
        [ObservableProperty] public Dictionary<string, string>? comboNameCd04;
        [ObservableProperty] public Dictionary<string, string>? comboNameCd05;
        [ObservableProperty] public Dictionary<string, string>? comboNameCd06;
        [ObservableProperty] public Dictionary<string, string>? comboNameCd07;
        [ObservableProperty] public Dictionary<string, string>? comboNameCd08;
        [ObservableProperty] public Dictionary<string, string>? comboNameCd09;
        [ObservableProperty] public Dictionary<string, string>? comboNameCd10;

        string sql_list = @"
            SELECT * FROM (
                SELECT
                    A.*,
                    (A.入力社員CD || ' ' || NVL(B.名前, '')) AS 最終修正者
                FROM HC$MASTER_SIIRE A
                LEFT JOIN HC$MASTER_SHAIN B ON B.社員CD = A.入力社員CD
                WHERE 仕入先CD {0} :1
                ORDER BY 仕入先CD {1}
            ) WHERE ROWNUM <= {2}";

        private Dictionary<int, string> GenerateScheduledPaymentDaysDict()
        {
            var d = new Dictionary<int, string>();
            for (int i = 1; i <= 28; i++) d[i] = $"{i:D2}";
            d[99] = "99 末日";
            return d;
        }

        private Dictionary<int, string> GenerateScheduledPaymentMonths => new()
        {
            { 0, "0 当月" }, { 1, "1 翌月" }, { 2, "2 翌々月" }, { 3, "3 翌々々月" },
            { 4, "4 4ヶ月後" }, { 5, "5 5ヶ月後" }, { 6, "6 6ヶ月後" }
        };

        [RelayCommand]
        void OnInit()
        {
            EditSupplier = new MasterSupplier();

            ComboOrderFlag = new() {
                { 0, "000 自動発注しない" }, { 1, "001 週1回:日" }, { 9, "009 週2回:日水" },
                { 41, "041 週3回:日水金" }, { 127, "127 発注(毎日)" }
            };

            ComboOrderStopFlag = new() { { 0, "0 しない" }, { 1, "1 する" } };
            EditSupplier.OrderStopFlag = 0;

            // 部門
            ComboDepartment = LoadMeishoCombo("BMN");
            EditSupplier.Department = ComboDepartment.FirstOrDefault().Key;

            ComboPurchaseType = new() { { 0, "0 無" }, { 1, "1 買取" }, { 2, "2 委託" }, { 3, "3 消化" } };
            EditSupplier.PurchaseType = 0;

            ComboPaymentPrintFlag = new() { { 0, "0 しない" }, { 1, "1 する" } };
            EditSupplier.PaymentPrintFlag = 0;

            ComboPaymentMethod = new() {
                { "0", "80 現金" }, { "1", "81 小切手" }, { "2", "82 振込" }, { "3", "85 手形" }
            };
            EditSupplier.PaymentMethod = "0";

            // 締日 & 支払予定日
            ComboClosingDay = ComboClosingDay2 = ComboClosingDay3 = GenerateScheduledPaymentDaysDict();
            EditSupplier.ClosingDay = EditSupplier.ClosingDay2 = EditSupplier.ClosingDay3 = 99;

            ComboScheduledPaymentMonth = ComboScheduledPaymentMonth2 = ComboScheduledPaymentMonth3 = GenerateScheduledPaymentMonths;
            EditSupplier.ScheduledPaymentMonth = EditSupplier.ScheduledPaymentMonth2 = EditSupplier.ScheduledPaymentMonth3 = 0;

            ComboScheduledPaymentDay = ComboScheduledPaymentDay2 = ComboScheduledPaymentDay3 = GenerateScheduledPaymentDaysDict();
            EditSupplier.ScheduledPaymentDay = EditSupplier.ScheduledPaymentDay2 = EditSupplier.ScheduledPaymentDay3 = 99;

            // 支払先
            ComboPaymentDestinationCd = LoadSupplierCombo();
            EditSupplier.PaymentDestinationCd = ComboPaymentDestinationCd.FirstOrDefault().Key;

            // 消費税関連
            ComboTaxCd = new() { { 0, "0 非課税" }, { 1, "1 課税" } };
            EditSupplier.TaxCd = 1;

            ComboTaxRounding = new() { { 0, "0 四捨五入" }, { 1, "1 切り上げ" }, { 2, "2 切り捨て" } };
            EditSupplier.TaxRounding = 0;

            ComboTaxCalculationMethod = new() { { 0, "0 支払単位" }, { 1, "1 伝票単位" } };
            EditSupplier.TaxCalculationMethod = 0;

            // 伝票発行区分 ← WAS MISSING!
            ComboSlipIssueType = new() { { 0, "0 発行しない" }, { 1, "1 発行する" } };
            EditSupplier.SlipIssueType = 0;

            // 為替
            ComboCurrencyType = new() { { "0", "00 円" }, { "1", "01 ドル" }, { "2", "02 ユーロ" }, { "3", "03 人民元" } };
            EditSupplier.CurrencyType = "0";

            ComboCurrencyRoundingDigit = new() { { 0, "0 支払単位" }, { 1, "1 切り上げ" }, { 2, "2 切り捨て" } };
            EditSupplier.CurrencyRoundingDigit = 0;

            ComboCurrencyFractionType = new() { { 0, "0 支払単位" }, { 1, "1 切り上げ" }, { 2, "2 切り捨て" } };
            EditSupplier.CurrencyFractionType = 0;

            // 法人CD & 連携先法人CD ← WAS MISSING!
            var corpDict = LoadMeishoCombo("HJN");
            ComboCorporationCd = ComboLinkedCorporationCd = corpDict;
            EditSupplier.CorporationCd = EditSupplier.LinkedCorporationCd = corpDict.FirstOrDefault().Key;

            // 名称CD01～10 ← WAS MISSING!
            for (int i = 1; i <= 10; i++)
            {
                var dict = LoadMeishoCombo($"D{i:D2}");
                var prop = GetType().GetProperty($"ComboNameCd{i:D2}");
                prop?.SetValue(this, dict);
                var editProp = typeof(MasterSupplier).GetProperty($"NameCd{i:D2}");
                editProp?.SetValue(EditSupplier, dict.FirstOrDefault().Key);
            }
        }
        private Dictionary<string, string> LoadMeishoCombo(string kubun)
        {
            var dict = new Dictionary<string, string>();
            var dt = AppData.Http?.AspxSqlQuery(
                $"SELECT 名称CD, 名称 FROM HC$MASTER_MEISHO WHERE 名称区分 = '{kubun}' ORDER BY 名称CD", null);
            if (dt != null)
                foreach (DataRow r in dt.Rows)
                    dict[r[0].ToString()!] = $"{r[0]} {r[1]}";
            if (!dict.Any()) dict[""] = "";
            return dict;
        }

        private Dictionary<string, string> LoadSupplierCombo()
        {
            var dict = new Dictionary<string, string>();
            var dt = AppData.Http?.AspxSqlQuery(
                "SELECT 仕入先CD, 仕入先名 FROM HC$MASTER_SIIRE WHERE 発注停止FLG = 0 ORDER BY 仕入先CD", null);
            if (dt != null)
                foreach (DataRow r in dt.Rows)
                    dict[r[0].ToString()!] = $"{r[0]} {r[1]}";
            if (!dict.Any()) dict[""] = "";
            return dict;
        }
        [RelayCommand]
        public void SelSupplier(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && EditSupplier != null)
            {
                EditSupplier.SupplierCD = get_sel00.Code;
                EditSupplier.SupplierName = get_sel00.Name;
            }
        }

        partial void OnSelectedSupplierChanged(MasterSupplier? value)
            {
                if (value != null)
                    EditSupplier = Common.CloneObject(value);
                else
                    EditSupplier = null;
            }

        [RelayCommand]
        void BackList()
        {
            var startcd = string.IsNullOrEmpty(StartCode) ? "." : StartCode;
            if (ListSupplier != null && ListSupplier.Count > 0)
            {
                startcd = ListSupplier.Min(c => c.SupplierCD);
            }
            SubList(startcd!, "<=", "desc", AppData.maxQueryCnt);
            if (ListSupplier == null || ListSupplier.Count == 0)
                ClientLib.MessageBoxOk(this, "データがありません");
        }

        [RelayCommand]
        void NextList()
        {
            var startcd = string.IsNullOrEmpty(StartCode) ? "." : StartCode;
            if (ListSupplier != null && ListSupplier.Count > 0)
            {
                startcd = ListSupplier.Max(c => c.SupplierCD);
            }
            SubList(startcd!, ">=", "asc", AppData.maxQueryCnt);
            if (ListSupplier == null || ListSupplier.Count == 0)
                ClientLib.MessageBoxOk(this, string.Empty);
        }
        [RelayCommand]
        void DoInsert()
        {
            if (!ClientLib.MessageBox(this, "新規登録しますか？")) return;

            var item = Common.CloneObject(EditSupplier)!;
            Common.ConvertDotStringAdd(item);

            var ret = AppData.Http!.AspxSqlExe(DBDef.DB_DML.INSERT, "HC$MASTER_SIIRE", item.SeqNo, item.VdateUpdate.ToString(),
                new string[] {
                    "仕入先CD", "仕入先名", "カナ", "旧コード", "略称",
                    "郵便番号", "住所1", "住所2", "住所3", "TEL", "FAX", "仕入先MAIL",
                    "支払先CD", "支払印刷", "支払方法",
                    "締日", "支払予定月", "支払予定日",
                    "締日2", "支払予定月2", "支払予定日2",
                    "締日3", "支払予定月3", "支払予定日3",
                    "消費税CD", "消費税計算方法", "消費税端数",
                    "伝票発行区分", "部門", "備考",
                    "名称CD01", "名称CD02", "名称CD03", "名称CD04", "名称CD05",
                    "名称CD06", "名称CD07", "名称CD08", "名称CD09", "名称CD10",
                    "発注FLG", "発注停止FLG", "仕入区分",
                    "法人CD", "連携先法人CD",
                    "登録番号", "入力社員CD"
                },
                new string[] {
                    item.SupplierCD!, item.SupplierName!, item.Kana!, item.OldCD!, item.Abbreviation!,
                    item.PostalCode!, item.Address1!, item.Address2!, item.Address3!,
                    item.Tel!, item.Fax!, item.SupplierMail!,
                    item.PaymentDestinationCd!, item.PaymentPrintFlag.ToString()!, item.PaymentMethod!,
                    item.ClosingDay.ToString() !, item.ScheduledPaymentMonth.ToString()!, item.ScheduledPaymentDay.ToString() !,
                    item.ClosingDay2.ToString() !, item.ScheduledPaymentMonth2.ToString() !, item.ScheduledPaymentDay2.ToString()!,
                    item.ClosingDay3.ToString() !, item.ScheduledPaymentMonth3.ToString() !, item.ScheduledPaymentDay3.ToString() !,
                    item.TaxCd.ToString() !, item.TaxCalculationMethod.ToString() !, item.TaxRounding.ToString()!,
                    item.SlipIssueType.ToString() !, item.Department!, item.Remarks!,
                    item.NameCd01!, item.NameCd02!, item.NameCd03!, item.NameCd04!, item.NameCd05!,
                    item.NameCd06!, item.NameCd07!, item.NameCd08!, item.NameCd09!, item.NameCd10!,
                    item.OrderFlag.ToString() !, item.OrderStopFlag.ToString() !, item.PurchaseType.ToString() !,
                    item.CorporationCd!, item.LinkedCorporationCd!,
                    item.RegistrationNumber!, AppData.ClassSatoo.SHAIN_CD?? "."
                });

            if (ret.Code == 0)
            {
                item.SeqNo = ret.NewSeq;
                item.VdateUpdate = decimal.Parse(ret.VDate);
                item.VdateCreate = item.VdateUpdate;
                Common.ConvertDotStringDel(item);
                ListSupplier!.Add(item);
                SelectedSupplier = item;
                ClientLib.MessageBoxOk(this, "登録しました");
            }
            else
            {
                ClientLib.MessageBoxError(this, ret.Code.ToString());
            }
        }
        void Dump(string name, object? value)
        {
            Debug.WriteLine($"{name} = {value ?? "NULL"}");
        }

        [RelayCommand]
        void DoUpdate()
        {
            if (!ClientLib.MessageBox(this, "修正しますか？")) return;

            if (EditSupplier == null)
            {
                ClientLib.MessageBox(this, "No data to update.");
                return;
            }
            var item = Common.CloneObject(EditSupplier);
            Common.ConvertDotStringAdd(item);
            Debug.WriteLine("=========== DEBUG: UPDATE PAYLOAD (BEFORE SEND) ===========");
            Dump("SeqNo", item.SeqNo);
            Dump("VdateUpdate", EditSupplier.VdateUpdate);
            Dump("VdateUpdate", item.VdateUpdate);

            Dump("SupplierCD", item.SupplierCD);
            Dump("OldCD", item.OldCD);
            Dump("Abbreviation", item.Abbreviation);
            Dump("SupplierName", item.SupplierName);
            Dump("Kana", item.Kana);

            Dump("PostalCode", item.PostalCode);
            Dump("Address1", item.Address1);
            Dump("Address2", item.Address2);
            Dump("Address3", item.Address3);
            Dump("Tel", item.Tel);
            Dump("Fax", item.Fax);

            Dump("Rate1", item.Rate1);
            Dump("Rate2", item.Rate2);
            Dump("PaymentDestinationCd", item.PaymentDestinationCd);
            Dump("PaymentPrintFlag", item.PaymentPrintFlag);

            Dump("ClosingDay", item.ClosingDay);
            Dump("ScheduledPaymentMonth", item.ScheduledPaymentMonth);
            Dump("ScheduledPaymentDay", item.ScheduledPaymentDay);
            Dump("PaymentMethod", item.PaymentMethod);

            Dump("TaxCd", item.TaxCd);
            Dump("TaxCalculationMethod", item.TaxCalculationMethod);
            Dump("TaxRounding", item.TaxRounding);
            Dump("PaymentRate", item.PaymentRate);

            Dump("RecipientFlag1", item.RecipientFlag1);
            Dump("RecipientFlag2", item.RecipientFlag2);
            Dump("RecipientFlag3", item.RecipientFlag3);
            Dump("RecipientName1", item.RecipientName1);
            Dump("RecipientName2", item.RecipientName2);

            Dump("SlipIssueType", item.SlipIssueType);

            Dump("SlipPrint1", item.SlipPrint1);
            Dump("SlipPrint2", item.SlipPrint2);
            Dump("SlipPrint3", item.SlipPrint3);
            Dump("SlipPrint4", item.SlipPrint4);

            Dump("Department", item.Department);
            Dump("Remarks", item.Remarks);

            Dump("NameCd01", item.NameCd01);
            Dump("NameCd02", item.NameCd02);
            Dump("NameCd03", item.NameCd03);
            Dump("NameCd04", item.NameCd04);
            Dump("NameCd05", item.NameCd05);
            Dump("NameCd06", item.NameCd06);
            Dump("NameCd07", item.NameCd07);
            Dump("NameCd08", item.NameCd08);
            Dump("NameCd09", item.NameCd09);
            Dump("NameCd10", item.NameCd10);

            Dump("BankName", item.BankName);
            Dump("BranchName", item.BranchName);
            Dump("TransferType", item.TransferType);
            Dump("AccountNumber", item.AccountNumber);
            Dump("Remarks2", item.Remarks2);

            Dump("OrderFlag", item.OrderFlag);
            Dump("ProductionFlag", item.ProductionFlag);

            Dump("CurrencyType", item.CurrencyType);
            Dump("CurrencyRoundingDigit", item.CurrencyRoundingDigit);
            Dump("CurrencyFractionType", item.CurrencyFractionType);

            Dump("PosKbn", item.PosKbn);
            Dump("InputEmployeeCd", item.InputEmployeeCd);

            Dump("OrderStopFlag", item.OrderStopFlag);
            Dump("PurchaseType", item.PurchaseType);

            Dump("CorporationCd", item.CorporationCd);
            Dump("LinkCd", item.LinkCd);
            Dump("LinkedCorporationCd", item.LinkedCorporationCd);

            Dump("SupplierMail", item.SupplierMail);

            Dump("DueDate", item.DueDate);
            Dump("MinimumAmount", item.MinimumAmount);

            Dump("WarehousePostal", item.WarehousePostal);
            Dump("WarehouseAddress1", item.WarehouseAddress1);
            Dump("WarehouseAddress2", item.WarehouseAddress2);
            Dump("WarehouseAddress3", item.WarehouseAddress3);
            Dump("WarehouseTel", item.WarehouseTel);
            Dump("WarehouseFax", item.WarehouseFax);

            Dump("ClosingDay2", item.ClosingDay2);
            Dump("ScheduledPaymentMonth2", item.ScheduledPaymentMonth2);
            Dump("ScheduledPaymentDay2", item.ScheduledPaymentDay2);

            Dump("ClosingDay3", item.ClosingDay3);
            Dump("ScheduledPaymentMonth3", item.ScheduledPaymentMonth3);
            Dump("ScheduledPaymentDay3", item.ScheduledPaymentDay3);

            Dump("RegistrationNumber", item.RegistrationNumber);

            Debug.WriteLine("===========================================================");
            if (item == null) return;
            var ret = AppData.Http!.AspxSqlExe(DBDef.DB_DML.UPDATE, "HC$MASTER_SIIRE",
                item.SeqNo, item.VdateUpdate.ToString(),
                new string[]
                {
                    "仕入先CD","旧コード","略称","仕入先名","カナ",
                    "郵便番号","住所1","住所2","住所3","TEL","FAX",
                    "掛率","掛率2","支払先CD","支払印刷",
                    "締日","支払予定月","支払予定日","支払方法",
                    "消費税CD","消費税計算方法","消費税端数","支払率",
                    "宛名FLG1","宛名FLG2","宛名FLG3","宛名名称1","宛名名称2",
                    "伝票発行区分",
                    "伝票印字1","伝票印字2","伝票印字3","伝票印字4",
                    "部門","備考",
                    "名称CD01","名称CD02","名称CD03","名称CD04","名称CD05",
                    "名称CD06","名称CD07","名称CD08","名称CD09","名称CD10",
                    "振込銀行","振込支店","振込種別","振込口座","備考2",
                    "発注FLG","生産FLG",
                    "為替区分","為替桁切指定","為替端数区分",
                    "入力社員CD",
                    "発注停止FLG","仕入区分",
                    "法人CD","連携CD","連携先法人CD",
                    "仕入先MAIL",
                    "期日","下限額",
                    "締日2","支払予定月2","支払予定日2",
                    "締日3","支払予定月3","支払予定日3",
                },
                new string[]
                {
                    item.SupplierCD!, item.OldCD!, item.Abbreviation!, item.SupplierName!, item.Kana!,
                    item.PostalCode!, item.Address1!, item.Address2!, item.Address3!, item.Tel!, item.Fax!,
                    item.Rate1.ToString()!, item.Rate2.ToString()!, item.PaymentDestinationCd!, item.PaymentPrintFlag.ToString()!,
                    item.ClosingDay.ToString()!, item.ScheduledPaymentMonth.ToString()!, item.ScheduledPaymentDay.ToString()!, item.PaymentMethod ?? "80",
                    item.TaxCd.ToString()!, item.TaxCalculationMethod.ToString()!, item.TaxRounding.ToString()!, item.PaymentRate.ToString()!,
                    item.RecipientFlag1 ?? ".", item.RecipientFlag2 ?? ".", item.RecipientFlag3 ?? ".", item.RecipientName1!, item.RecipientName2!,
                    item.SlipIssueType.ToString()!,
                    item.SlipPrint1!, item.SlipPrint2!, item.SlipPrint3!, item.SlipPrint4!,
                    item.Department!, item.Remarks!,
                    item.NameCd01 ?? ".", item.NameCd02 ?? ".", item.NameCd03 ?? ".", item.NameCd04 ?? ".", item.NameCd05 ?? ".",
                    item.NameCd06 ?? ".", item.NameCd07 ?? ".", item.NameCd08 ?? ".", item.NameCd09 ?? ".", item.NameCd10 ?? ".",
                    item.BankName!, item.BranchName!, item.TransferType!, item.AccountNumber!, item.Remarks2 ?? ".",
                    item.OrderFlag.ToString()!, item.ProductionFlag.ToString()!,
                    item.CurrencyType!, item.CurrencyRoundingDigit.ToString()!, item.CurrencyFractionType.ToString()!,
                    AppData.ClassSatoo.SHAIN_CD ?? ".",
                    item.OrderStopFlag.ToString()!, item.PurchaseType.ToString()!,
                    item.CorporationCd ?? ".", item.LinkCd ?? ".", item.LinkedCorporationCd ?? ".",
                    item.SupplierMail!,
                    item.DueDate.ToString()!, item.MinimumAmount.ToString()!,
                    item.ClosingDay2.ToString()!, item.ScheduledPaymentMonth2.ToString()!, item.ScheduledPaymentDay2.ToString()!,
                    item.ClosingDay3.ToString()!, item.ScheduledPaymentMonth3.ToString()!, item.ScheduledPaymentDay3.ToString()!,
                });

            if (ret.Code == 0)
            {
                Common.ConvertDotStringDel(item);
                if (SelectedSupplier != null)
                {
                    item.VdateUpdate = decimal.Parse(ret.VDate);
                    SelectedSupplier.SupplierCD = item.SupplierCD;
                    SelectedSupplier.SupplierName = item.SupplierName;
                    SelectedSupplier.Kana = item.Kana;
                    SelectedSupplier.OldCD = item.OldCD;
                    SelectedSupplier.Abbreviation = item.Abbreviation;
                    SelectedSupplier.PostalCode = item.PostalCode;
                    SelectedSupplier.Address1 = item.Address1;
                    SelectedSupplier.Address2 = item.Address2;
                    SelectedSupplier.Address3 = item.Address3;
                    SelectedSupplier.Tel = item.Tel;
                    SelectedSupplier.Fax = item.Fax;

                    // Recipient fields
                    SelectedSupplier.RecipientFlag1 = item.RecipientFlag1;
                    SelectedSupplier.RecipientFlag2 = item.RecipientFlag2;
                    SelectedSupplier.RecipientFlag3 = item.RecipientFlag3;
                    SelectedSupplier.RecipientName1 = item.RecipientName1;
                    SelectedSupplier.RecipientName2 = item.RecipientName2;

                    // Rates
                    SelectedSupplier.Rate1 = item.Rate1;
                    SelectedSupplier.Rate2 = item.Rate2;
                    SelectedSupplier.PaymentRate = item.PaymentRate;

                    // Payment destination
                    SelectedSupplier.PaymentDestinationCd = item.PaymentDestinationCd;
                    SelectedSupplier.PaymentPrintFlag = item.PaymentPrintFlag;
                    SelectedSupplier.PaymentMethod = item.PaymentMethod;

                    // Closing & scheduled dates
                    SelectedSupplier.ClosingDay = item.ClosingDay;
                    SelectedSupplier.ScheduledPaymentMonth = item.ScheduledPaymentMonth;
                    SelectedSupplier.ScheduledPaymentDay = item.ScheduledPaymentDay;

                    // Added as required
                    SelectedSupplier.DueDate = item.DueDate;
                    SelectedSupplier.MinimumAmount = item.MinimumAmount;

                    // Closing/scheduled 2 & 3
                    SelectedSupplier.ClosingDay2 = item.ClosingDay2;
                    SelectedSupplier.ScheduledPaymentMonth2 = item.ScheduledPaymentMonth2;
                    SelectedSupplier.ScheduledPaymentDay2 = item.ScheduledPaymentDay2;

                    SelectedSupplier.ClosingDay3 = item.ClosingDay3;
                    SelectedSupplier.ScheduledPaymentMonth3 = item.ScheduledPaymentMonth3;
                    SelectedSupplier.ScheduledPaymentDay3 = item.ScheduledPaymentDay3;

                    // Tax
                    SelectedSupplier.TaxCd = item.TaxCd;
                    SelectedSupplier.TaxCalculationMethod = item.TaxCalculationMethod;
                    SelectedSupplier.TaxRounding = item.TaxRounding;

                    // Names
                    SelectedSupplier.NameCd01 = item.NameCd01;
                    SelectedSupplier.NameCd02 = item.NameCd02;
                    SelectedSupplier.NameCd03 = item.NameCd03;
                    SelectedSupplier.NameCd04 = item.NameCd04;
                    SelectedSupplier.NameCd05 = item.NameCd05;
                    SelectedSupplier.NameCd06 = item.NameCd06;
                    SelectedSupplier.NameCd07 = item.NameCd07;
                    SelectedSupplier.NameCd08 = item.NameCd08;
                    SelectedSupplier.NameCd09 = item.NameCd09;
                    SelectedSupplier.NameCd10 = item.NameCd10;

                    // Slip print fields
                    SelectedSupplier.SlipPrint1 = item.SlipPrint1;
                    SelectedSupplier.SlipPrint2 = item.SlipPrint2;
                    SelectedSupplier.SlipPrint3 = item.SlipPrint3;
                    SelectedSupplier.SlipPrint4 = item.SlipPrint4;

                    // Bank info
                    SelectedSupplier.BankName = item.BankName;
                    SelectedSupplier.BranchName = item.BranchName;
                    SelectedSupplier.TransferType = item.TransferType;
                    SelectedSupplier.AccountNumber = item.AccountNumber;
                    SelectedSupplier.Remarks2 = item.Remarks2;

                    // Flags
                    SelectedSupplier.OrderFlag = item.OrderFlag;
                    SelectedSupplier.OrderStopFlag = item.OrderStopFlag;
                    SelectedSupplier.ProductionFlag = item.ProductionFlag;
                    SelectedSupplier.PurchaseType = item.PurchaseType;

                    SelectedSupplier.PosKbn = item.PosKbn;
                    SelectedSupplier.WarehousePostal = item.WarehousePostal;
                    SelectedSupplier.WarehouseAddress1 = item.WarehouseAddress1;
                    SelectedSupplier.WarehouseAddress2 = item.WarehouseAddress2;
                    SelectedSupplier.WarehouseAddress3 = item.WarehouseAddress3;
                    SelectedSupplier.WarehouseTel = item.WarehouseTel;
                    SelectedSupplier.WarehouseFax = item.WarehouseFax;

                    // Other
                    SelectedSupplier.CurrencyType = item.CurrencyType;
                    SelectedSupplier.CurrencyRoundingDigit = item.CurrencyRoundingDigit;
                    SelectedSupplier.CurrencyFractionType = item.CurrencyFractionType;

                    SelectedSupplier.Department = item.Department;
                    SelectedSupplier.Remarks = item.Remarks;

                    SelectedSupplier.CorporationCd = item.CorporationCd;
                    SelectedSupplier.LinkedCorporationCd = item.LinkedCorporationCd;
                    SelectedSupplier.LinkCd = item.LinkCd;

                    SelectedSupplier.SupplierMail = item.SupplierMail;
                    SelectedSupplier.RegistrationNumber = item.RegistrationNumber;

                    EditSupplier = Common.CloneObject(SelectedSupplier);
                    ClientLib.MessageBoxOk(this, "登録しました");
                }
            }
            else
            {
                ClientLib.MessageBoxError(this, ret.Code.ToString());
            }
        }
        [RelayCommand]
        void DoDelete()
        {
            // Confirm deletion
            if (!ClientLib.MessageBox(this, "削除しますか？")) return;

            if (EditSupplier == null) return;

            // Execute DELETE
            var ret = AppData.Http!.AspxSqlExe(
                DBDef.DB_DML.DELETE,
                "HC$MASTER_SIIRE", // ← supplier master table name
                EditSupplier.SeqNo,
                EditSupplier.VdateUpdate.ToString(),
                new string[0],
                new string[0]
            );

            if (ret.Code == 0)
            {
                // Remove the deleted supplier from the list and update selection
                if (SelectedSupplier != null && ListSupplier != null)
                {
                    ListSupplier.Remove(SelectedSupplier);

                    // Optionally select the first remaining record
                    var item = ListSupplier.OrderBy(c => c.SupplierCD).FirstOrDefault();
                    SelectedSupplier = item;
                    EditSupplier = Common.CloneObject(SelectedSupplier);
                }
            }
            else
            {
                ClientLib.MessageBoxError(this, ret.Code.ToString());
            }
        }
        [RelayCommand]
        void DoList()
        {
            SubList(string.IsNullOrEmpty(StartCode) ? "." : StartCode, ">=", "asc", AppData.maxQueryCnt);
            if (ListSupplier == null || ListSupplier.Count == 0)
                ClientLib.MessageBoxOk(this, "データがありません");
        }

        void SubList(string startCd, string sql_P1, string sql_P2, int sql_P3)
        {
            var sql = string.Format(sql_list, sql_P1, sql_P2, sql_P3);
            var retData = AppData.Http?.AspxSqlQuery(sql, new string[] { startCd });
            if (retData == null || retData.Rows.Count == 0) return;

            var list = (from DataRow dr in retData.Rows
                        select new MasterSupplier
                        {
                            SeqNo = Convert.ToInt64(dr["SEQ_NO"]),
                            VdateCreate = Convert.ToDecimal(dr["VDATE_CREATE"]),
                            VdateUpdate = Convert.ToDecimal(dr["VDATE_UPDATE"]),

                            SupplierCD = dr["仕入先CD"].ToString() ?? string.Empty,
                            SupplierName = dr["仕入先名"].ToString() ?? string.Empty,
                            Kana = dr["カナ"].ToString() ?? string.Empty,
                            OldCD = dr["旧コード"].ToString() ?? string.Empty,
                            Abbreviation = dr["略称"].ToString() ?? string.Empty,
                            PostalCode = dr["郵便番号"].ToString() ?? string.Empty,
                            Address1 = dr["住所1"].ToString() ?? string.Empty,
                            Address2 = dr["住所2"].ToString() ?? string.Empty,
                            Address3 = dr["住所3"].ToString() ?? string.Empty,
                            Tel = dr["TEL"].ToString() ?? string.Empty,
                            Fax = dr["FAX"].ToString() ?? string.Empty,
                            SupplierMail = dr["仕入先MAIL"].ToString() ?? string.Empty,

                            RecipientFlag1 = dr["宛名FLG1"].ToString() ?? string.Empty,
                            RecipientFlag2 = dr["宛名FLG2"].ToString() ?? string.Empty,
                            RecipientFlag3 = dr["宛名FLG3"].ToString() ?? string.Empty,
                            RecipientName1 = dr["宛名名称1"].ToString() ?? string.Empty,
                            RecipientName2 = dr["宛名名称2"].ToString() ?? string.Empty,

                            Rate1 = Convert.ToDecimal(dr["掛率"]),
                            Rate2 = Convert.ToDecimal(dr["掛率2"]),
                            PaymentRate = Convert.ToDecimal(dr["支払率"]),

                            PaymentDestinationCd = dr["支払先CD"].ToString() ?? string.Empty,
                            PaymentPrintFlag = Convert.ToInt32(dr["支払印刷"]),
                            ClosingDay = Convert.ToInt32(dr["締日"]),
                            ScheduledPaymentMonth = Convert.ToInt32(dr["支払予定月"]),
                            ScheduledPaymentDay = Convert.ToInt32(dr["支払予定日"]),
                            PaymentMethod = dr["支払方法"].ToString() ?? string.Empty,

                            TaxCd = Convert.ToInt32(dr["消費税CD"]),
                            TaxCalculationMethod = Convert.ToInt32(dr["消費税計算方法"]),
                            TaxRounding = Convert.ToInt32(dr["消費税端数"]),

                            NameCd01 = dr["名称CD01"].ToString() ?? string.Empty,
                            NameCd02 = dr["名称CD02"].ToString() ?? string.Empty,
                            NameCd03 = dr["名称CD03"].ToString() ?? string.Empty,
                            NameCd04 = dr["名称CD04"].ToString() ?? string.Empty,
                            NameCd05 = dr["名称CD05"].ToString() ?? string.Empty,
                            NameCd06 = dr["名称CD06"].ToString() ?? string.Empty,
                            NameCd07 = dr["名称CD07"].ToString() ?? string.Empty,
                            NameCd08 = dr["名称CD08"].ToString() ?? string.Empty,
                            NameCd09 = dr["名称CD09"].ToString() ?? string.Empty,
                            NameCd10 = dr["名称CD10"].ToString() ?? string.Empty,

                            SlipPrint1 = dr["伝票印字1"].ToString() ?? string.Empty,
                            SlipPrint2 = dr["伝票印字2"].ToString() ?? string.Empty,
                            SlipPrint3 = dr["伝票印字3"].ToString() ?? string.Empty,
                            SlipPrint4 = dr["伝票印字4"].ToString() ?? string.Empty,

                            BankName = dr["振込銀行"].ToString() ?? string.Empty,
                            BranchName = dr["振込支店"].ToString() ?? string.Empty,
                            TransferType = dr["振込種別"].ToString() ?? string.Empty,
                            AccountNumber = dr["振込口座"].ToString() ?? string.Empty,
                            Remarks2 = dr["備考2"].ToString() ?? string.Empty,

                            OrderFlag = Convert.ToInt32(dr["発注FLG"]),
                            ProductionFlag = Convert.ToInt32(dr["生産FLG"]),

                            CurrencyType = dr["為替区分"].ToString() ?? string.Empty,
                            CurrencyRoundingDigit = Convert.ToInt32(dr["為替桁切指定"]),
                            CurrencyFractionType = Convert.ToInt32(dr["為替端数区分"]),

                            Department = dr["部門"].ToString() ?? string.Empty,
                            PosKbn = Convert.ToInt32(dr["POS区分"]),
                            InputEmployeeCd = dr["入力社員CD"].ToString() ?? string.Empty,

                            OrderStopFlag = Convert.ToInt32(dr["発注停止FLG"]),
                            PurchaseType = Convert.ToInt32(dr["仕入区分"]),

                            CorporationCd = dr["法人CD"].ToString() ?? string.Empty,
                            LinkCd = dr["連携CD"].ToString() ?? string.Empty,
                            LinkedCorporationCd = dr["連携先法人CD"].ToString() ?? string.Empty,

                            DueDate = Convert.ToInt32(dr["期日"]),
                            MinimumAmount = Convert.ToDecimal(dr["下限額"]),

                            WarehousePostal = dr["倉庫郵便番号"].ToString() ?? string.Empty,
                            WarehouseAddress1 = dr["倉庫住所1"].ToString() ?? string.Empty,
                            WarehouseAddress2 = dr["倉庫住所2"].ToString() ?? string.Empty,
                            WarehouseAddress3 = dr["倉庫住所3"].ToString() ?? string.Empty,
                            WarehouseTel = dr["倉庫TEL"].ToString() ?? string.Empty,
                            WarehouseFax = dr["倉庫FAX"].ToString() ?? string.Empty,

                            ClosingDay2 = Convert.ToInt32(dr["締日2"]),
                            ScheduledPaymentMonth2 = Convert.ToInt32(dr["支払予定月2"]),
                            ScheduledPaymentDay2 = Convert.ToInt32(dr["支払予定日2"]),

                            ClosingDay3 = Convert.ToInt32(dr["締日3"]),
                            ScheduledPaymentMonth3 = Convert.ToInt32(dr["支払予定月3"]),
                            ScheduledPaymentDay3 = Convert.ToInt32(dr["支払予定日3"]),

                            RegistrationNumber = dr["登録番号"].ToString() ?? string.Empty
                        })
                        .OrderBy(c => c.SupplierCD)
                        .ToList();

            Common.ConvertDotStringDel(list);
            ListSupplier = new ObservableCollection<MasterSupplier>(list);

            if (ListSupplier.Count > 0)
            {
                SelectedSupplier = ListSupplier[0];
            }
        }

        string printsql = "select * from (" +
            "select A.SEQ_NO, " +
            "SUBSTR(GET_VDATE(A.VDATE_CREATE), 0, 8) || SUBSTR(GET_VDATE(A.VDATE_CREATE), 10, 6) 作成日時, " +
            "SUBSTR(GET_VDATE(A.VDATE_UPDATE), 0, 8) || SUBSTR(GET_VDATE(A.VDATE_UPDATE), 10, 6) 更新日時, " +
            "A.仕入先CD, A.仕入先名, A.カナ, A.旧コード, A.略称, A.郵便番号, A.住所1, A.住所2, A.住所3, A.TEL, A.FAX, " +
            "A.掛率, A.掛率2, A.支払先CD, A.支払印刷, A.締日, A.支払予定月, A.支払予定日, A.支払方法, " +
            "A.消費税CD, A.消費税計算方法, A.消費税端数, A.支払率, A.名称CD01, A.備考, A.伝票印字1, A.伝票印字2, A.伝票印字3, A.伝票印字4, " +
            "A.振込銀行, A.振込支店, A.振込種別, A.振込口座, A.備考2, A.発注FLG, A.伝票発行区分, A.部門, A.名称CD02, A.名称CD03, A.名称CD04, " +
            "A.名称CD05, A.名称CD06, A.名称CD07, A.名称CD08, A.名称CD09, A.名称CD10, A.生産FLG, A.為替区分, A.為替桁切指定, A.為替端数区分, " +
            "A.発注停止FLG, A.仕入区分, A.法人CD, A.連携先法人CD, A.連携CD, A.期日, A.下限額, A.締日2, A.支払予定月2, A.支払予定日2, " +
            "A.締日3, A.支払予定月3, A.支払予定日3, A.登録番号, " +
            "NVL((select H.仕入先名 from HC$MASTER_SIIRE H where H.仕入先CD = A.支払先CD), '.') 支払先名, " +
            "NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='D01' and H.名称CD=A.名称CD01),'.') 分類01名, " +
            "NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='D02' and H.名称CD=A.名称CD02),'.') 分類02名, " +
            "NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='D03' and H.名称CD=A.名称CD03),'.') 分類03名, " +
            "NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='D04' and H.名称CD=A.名称CD04),'.') 分類04名, " +
            "NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='D05' and H.名称CD=A.名称CD05),'.') 分類05名, " +
            "NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='D06' and H.名称CD=A.名称CD06),'.') 分類06名, " +
            "NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='D07' and H.名称CD=A.名称CD07),'.') 分類07名, " +
            "NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='D08' and H.名称CD=A.名称CD08),'.') 分類08名, " +
            "NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='D09' and H.名称CD=A.名称CD09),'.') 分類09名, " +
            "NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='D10' and H.名称CD=A.名称CD10),'.') 分類10名, " +
            "(A.入力社員CD || ' ' || (select B.名前 from HC$MASTER_SHAIN B where B.社員CD=A.入力社員CD)) 最終修正者, " +
            "NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='BMN' and H.名称CD=A.部門), '.') 部門名, " +
            "NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='HJN' and H.名称CD=A.法人CD), '.') 法人名, " +
            "NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='HJN' and H.名称CD=A.連携先法人CD), '.') 連携先法人名 " +
            "from HC$MASTER_SIIRE A";

        [RelayCommand]
        async Task DoPrintAsync()
        {
            if (!ClientLib.MessageBox(this, "印刷しますか？")) return;
            ClientLib.CursorToWait();

            if (ListSupplier == null || ListSupplier.Count == 0) return;

            // Build parameter placeholders
            var paramNames = ListSupplier.Select((c, i) => $":p{i}||''").ToList();

            // Build final SQL with placeholders
            var sql = printsql +
                        $" where TO_CHAR(A.仕入先CD) in ({string.Join(",", paramNames)}) order by A.仕入先CD)";

            // Build parameter values
            var parameters = ListSupplier.Select(c => c.SupplierCD).ToArray();

            // Execute and get CSV result
            var ret = AppData.Http!.AspxSqlQueryCsv(sql, parameters, "cvnet_siire.qfm");

            if (ret.Split('\n').Length < 2)
            {
                ClientLib.MessageBoxError(this, "PDFデータがありません");
                return;
            }

            // Extract PDF URL
            var ret1 = ret.Split('\n');
            var url = AppData.Http.URLroot + ret1[0] + "/data.pdf";

            // Wait for PDF generation
            await Task.Delay(1500);

            // Open PDF viewer
            var win = new WebpdfView();
            if (win.DataContext is WebpdfViewModel vm)
            {
                vm.Pdfdata = url;
            }

            ClientLib.CursorToNormal();
            ClientLib.ShowDialogView(win, this);
        }

    }
}
