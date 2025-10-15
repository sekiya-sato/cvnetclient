using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Models;
using CvnetClient.Views;
using System.Collections.ObjectModel;
using System.Data;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg01SirViewModel : BaseViewModel
    {
        [ObservableProperty]
        ObservableCollection<MasterSupplier>? listSupplier;
        [ObservableProperty]
        MasterSupplier? selectedSupplier;
        [ObservableProperty]
        MasterSupplier? editSupplier;
        [ObservableProperty]
        string? startCode;
        [ObservableProperty]
        bool isAutoOrderVisible;

        // 自動発注 (orderFlag)
        [ObservableProperty]
        public Dictionary<int, string>? comboOrderFlag;

        // 部門 (department)
        [ObservableProperty]
        public Dictionary<string, string>? comboDepartment;

        // 発注停止FLG (orderStopFlag)
        [ObservableProperty]
        public Dictionary<int, string>? comboOrderStopFlag;

        // 仕入区分 (purchaseType)
        [ObservableProperty]
        public Dictionary<int, string>? comboPurchaseType;

        // 法人CD (corporationCd)
        [ObservableProperty]
        public Dictionary<string, string>? comboCorporationCd;

        // 連携先法人CD (linkedCorporationCd)
        [ObservableProperty]
        public Dictionary<string, string>? comboLinkedCorporationCd;

        // 支払先CD (paymentDestinationCd)
        [ObservableProperty]
        public Dictionary<string, string>? comboPaymentDestinationCd;

        // 支払印刷 (paymentPrintFlag)
        [ObservableProperty]
        public Dictionary<int, string>? comboPaymentPrintFlag;

        // 支払方法 (paymentMethod)
        [ObservableProperty]
        public Dictionary<string, string>? comboPaymentMethod;

        // 締日 (closingDay) options
        [ObservableProperty]
        public List<int>? comboClosingDay;

        // 締日2 (closingDay2) options
        [ObservableProperty]
        public List<int>? comboClosingDay2;

        // 締日3 (closingDay3) options
        [ObservableProperty]
        public List<int>? comboClosingDay3;

        // 支払予定月 (scheduledPaymentMonth) options
        [ObservableProperty]
        public Dictionary<int, string>? comboScheduledPaymentMonth;

        // 支払予定月2 (scheduledPaymentMonth2) options
        [ObservableProperty]
        public Dictionary<int, string>? comboScheduledPaymentMonth2;

        // 支払予定月3 (scheduledPaymentMonth3) options
        [ObservableProperty]
        public Dictionary<int, string>? comboScheduledPaymentMonth3;

        // 支払予定日 (scheduledPaymentDay) options
        [ObservableProperty]
        public List<int>? comboScheduledPaymentDay;

        // 支払予定日2 (scheduledPaymentDay2) options
        [ObservableProperty]
        public List<int>? comboScheduledPaymentDay2;

        // 支払予定日3 (scheduledPaymentDay3) options
        [ObservableProperty]
        public List<int>? comboScheduledPaymentDay3;

        // 消費税CD (taxCd)
        [ObservableProperty]
        public Dictionary<decimal, string>? comboTaxCd;

        // 消費税端数 (taxRounding)
        [ObservableProperty]
        public Dictionary<int, string>? comboTaxRounding;

        // 消費税計算方法 (taxCalculationMethod)
        [ObservableProperty]
        public Dictionary<int, string>? comboTaxCalculationMethod;

        // 伝票発行区分 (slipIssueType)
        [ObservableProperty]
        public Dictionary<int, string>? comboSlipIssueType;

        // 為替区分 (currencyType)
        [ObservableProperty]
        public Dictionary<string, string>? comboCurrencyType;

        // 為替桁切指定 (currencyRoundingDigit)
        [ObservableProperty]
        public Dictionary<int, string>? comboCurrencyRoundingDigit;

        //為替端数区分 (Exchange fraction division)
        [ObservableProperty]
        public Dictionary<int, string>? comboCurrencyFractionType;

        // 名称CD01〜10 (nameCd01 ~ nameCd10)
        [ObservableProperty]
        public Dictionary<string, string>? comboNameCd01;
        [ObservableProperty]
        public Dictionary<string, string>? comboNameCd02;
        [ObservableProperty]
        public Dictionary<string, string>? comboNameCd03;
        [ObservableProperty]
        public Dictionary<string, string>? comboNameCd04;
        [ObservableProperty]
        public Dictionary<string, string>? comboNameCd05;
        [ObservableProperty]
        public Dictionary<string, string>? comboNameCd06;
        [ObservableProperty]
        public Dictionary<string, string>? comboNameCd07;
        [ObservableProperty]
        public Dictionary<string, string>? comboNameCd08;
        [ObservableProperty]
        public Dictionary<string, string>? comboNameCd09;
        [ObservableProperty]
        public Dictionary<string, string>? comboNameCd10;


        string sql_list = "select * from" +
            "(select A.SEQ_NO, A.VDATE_CREATE, A.VDATE_UPDATE, A.仕入先CD, A.仕入先名, A.カナ, A.旧コード," +
            "A.略称, A.郵便番号, A.住所1, A.住所2, A.住所3, A.TEL, A.FAX, A.宛名FLG1, A.宛名FLG2, A.宛名FLG3, A.宛名名称1, A.宛名名称2, " +
            "A.掛率, A.掛率2, A.支払先CD, A.支払印刷, A.締日, A.支払予定月, A.支払予定日, A.支払方法, A.消費税CD, A.消費税計算方法, A.消費税端数," +
            "A.支払率, A.名称CD01, A.備考, A.伝票印字1, A.伝票印字2, A.伝票印字3, A.伝票印字4, A.振込銀行, A.振込支店, A.振込種別, A.振込口座," +
            "A.備考2, A.発注FLG, A.伝票発行区分, A.部門, A.名称CD02, A.名称CD03, A.名称CD04, A.名称CD05, A.名称CD06, A.名称CD07, A.名称CD08," +
            "A.名称CD09, A.名称CD10, A.生産FLG, A.為替区分, A.為替桁切指定, A.為替端数区分, A.発注停止FLG, A.仕入区分, A.法人CD, A.連携先法人CD," +
            "A.連携CD, A.期日, A.下限額, A.締日2, A.支払予定月2, A.支払予定日2, A.締日3, A.支払予定月3, A.支払予定日3," +
            "NVL((select H.仕入先名 from HC$Master_SIIRE H where H.仕入先CD = A.支払先CD), '.')支払先名" +
            ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分= 'D01' and H.名称CD= A.名称CD01),'.') 分類01名" +
            ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分= 'D02' and H.名称CD= A.名称CD02),'.') 分類02名" +
            ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分= 'D03' and H.名称CD= A.名称CD03),'.') 分類03名" +
            ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分= 'D04' and H.名称CD= A.名称CD04),'.') 分類04名" +
            ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分= 'D05' and H.名称CD= A.名称CD05),'.') 分類05名" +
            ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分= 'D06' and H.名称CD= A.名称CD06),'.') 分類06名" +
            ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分= 'D07' and H.名称CD= A.名称CD07),'.') 分類07名" +
            ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分= 'D08' and H.名称CD= A.名称CD08),'.') 分類08名" +
            ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分= 'D09' and H.名称CD= A.名称CD09),'.') 分類09名" +
            ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分= 'D10' and H.名称CD= A.名称CD10),'.') 分類10名" +
            ",(A.入力社員CD ||' '|| (select B.名前 from HC$MASTER_SHAIN B where B.社員CD=A.入力社員CD)) 最終修正者" +
            ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分= 'BMN' and H.名称CD= A.部門),'.')部門名" +
            ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分= 'HJN' and H.名称CD= A.法人CD),'.') 法人名" +
            ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分= 'HJN' and H.名称CD= A.連携先法人CD),'.')連携先法人名" +
            "from HC$Master_SIIRE A " +
            "where A.仕入先CD in (select 仕入先CD from hc$master_siire z where 発注停止FLG = 0 and 仕入先CD between :1 and :2 and 締日 between :3 and :4)" +
            "and A.仕入先CD >= '00000'  order by A.仕入先CD asc)" +
            " where rownum<={40}";


        // ✅ Generate scheduled payment days list (1–28 + 99)
        private List<int> GenerateScheduledPaymentDays()
        {
            var list = new List<int>();
            for (int i = 1; i <= 28; i++)
            {
                list.Add(i);
            }
            list.Add(99); // 99 = default or end of month
            return list;
        }

        // ✅ Generate scheduled payment month dictionary (0–6)
        private Dictionary<int, string> GenerateScheduledPaymentMonths()
        {
            return new Dictionary<int, string>
            {
                { 0, "0 当月" },     // Current month
                { 1, "1 翌月" },     // Next month
                { 2, "2 翌々月" },   // 2 months later
                { 3, "3 翌々々月" }, // 3 months later
                { 4, "4 4ヶ月後" },  // 4 months later
                { 5, "5 5ヶ月後" },  // 5 months later
                { 6, "6 6ヶ月後" }   // 6 months later
            };
        }

        [RelayCommand]
        void Init()
        {
            EditSupplier = new MasterSupplier();

            IsAutoOrderVisible = AppData.ClassCvnet.config.MultiCoop != 0;

            if (IsAutoOrderVisible)
            {
                ComboOrderFlag = new Dictionary<int, string>
                {
                    { 0,   "000 自動発注しない" },   // Do not auto order
                    { 1,   "001 週1回:日" },         // Once a week: Sunday
                    { 9,   "009 週2回:日水" },       // Twice a week: Sun & Wed
                    { 41,  "041 週3回:日水金" },     // Three times a week: Sun, Wed, Fri
                    { 127, "127 発注(毎日)" }        // Every day
                };
                EditSupplier.OrderFlag = ComboOrderFlag.FirstOrDefault().Key;
            }

            // ✅ 発注停止FLG (Order Stop)
            ComboOrderStopFlag = new Dictionary<int, string>
            {
                { 0, "0 しない" },
                { 1, "1 する" }
            };
            EditSupplier.OrderStopFlag = ComboOrderStopFlag.FirstOrDefault().Key;

            // ✅ 部門 (Department)
            var comboList = new Dictionary<string, string>();
            string sql_query = "select A.名称CD,A.名称 from HC$Master_MEISHO a  where  a.名称区分='BMN' order by a.名称CD";
            var get_combolist = AppData.Http?.AspxSqlQuery(sql_query, null);
            foreach (DataRow row in get_combolist.Rows)
            {
                string? key = row[0] != DBNull.Value ? row[0].ToString() : string.Empty;
                string? value = row[1] != DBNull.Value ? row[1].ToString() : string.Empty;
                comboList.Add(key, string.Format("{0} {1}", key, value));
            }
            ComboDepartment = comboList;
            EditSupplier.Department = ComboDepartment.FirstOrDefault().Key;

            // ✅ 仕入区分 (Purchase Type)
            ComboPurchaseType = new Dictionary<int, string>
            {
                { 0, "0 無" },
                { 1, "1 買取" },
                { 2, "2 委託" },
                { 3, "3 消化" }
            };
            EditSupplier.PurchaseType = ComboPurchaseType.FirstOrDefault().Key;

            // ✅ 支払印刷 (Payment Print Flag)
            ComboPaymentPrintFlag = new Dictionary<int, string>
            {
                { 0, "0 しない" },
                { 1, "1 する" }
            };
            EditSupplier.PaymentPrintFlag = ComboPaymentPrintFlag.FirstOrDefault().Key;

            // ✅ 支払方法 (Payment Method)
            ComboPaymentMethod = new Dictionary<string, string>
            {
                { "0", "80 現金" },
                { "1", "81 小切手" },
                { "2", "82 振込" },
                { "3", "85 手形" }
            };
            EditSupplier.PaymentMethod = ComboPaymentMethod.FirstOrDefault().Key;

            // ✅ 締日 (Closing Day) options
            ComboClosingDay = GenerateScheduledPaymentDays();
            EditSupplier.ClosingDay = 99;

            ComboClosingDay2 = GenerateScheduledPaymentDays();
            EditSupplier.ClosingDay2 = 99;

            ComboClosingDay3 = GenerateScheduledPaymentDays();
            EditSupplier.ClosingDay3 = 99;

            // ✅ 支払予定月 (Scheduled Payment Month)
            ComboScheduledPaymentMonth = GenerateScheduledPaymentMonths();
            EditSupplier.ScheduledPaymentMonth = ComboScheduledPaymentMonth.FirstOrDefault().Key;

            ComboScheduledPaymentMonth2 = GenerateScheduledPaymentMonths();
            EditSupplier.ScheduledPaymentMonth2 = ComboScheduledPaymentMonth2.FirstOrDefault().Key;

            ComboScheduledPaymentMonth3 = GenerateScheduledPaymentMonths();
            EditSupplier.ScheduledPaymentMonth3 = ComboScheduledPaymentMonth3.FirstOrDefault().Key;

            // ✅ 支払予定日 (Scheduled Payment Day)
            ComboScheduledPaymentDay = GenerateScheduledPaymentDays();
            EditSupplier.ScheduledPaymentDay = 99;

            ComboScheduledPaymentDay2 = GenerateScheduledPaymentDays();
            EditSupplier.ScheduledPaymentDay2 = 99;

            ComboScheduledPaymentDay3 = GenerateScheduledPaymentDays();
            EditSupplier.ScheduledPaymentDay3 = 99;

            // ✅ 部門 (Department)
            var comboList2 = new Dictionary<string, string>();
            string sql_query2 = "select A.名称CD,A.名称 from HC$Master_MEISHO a  where  a.名称区分='BMN' order by a.名称CD";
            var get_combolist2 = AppData.Http?.AspxSqlQuery(sql_query, null);
            foreach (DataRow row in get_combolist2.Rows)
            {
                string? key = row[0] != DBNull.Value ? row[0].ToString() : string.Empty;
                string? value = row[1] != DBNull.Value ? row[1].ToString() : string.Empty;
                comboList.Add(key, string.Format("{0} {1}", key, value));
            }
            ComboPaymentDestinationCd = comboList2;
            EditSupplier.PaymentDestinationCd = ComboPaymentDestinationCd.FirstOrDefault().Key;

            // ✅ 消費税CD (Tax Code)
            ComboTaxCd = new Dictionary<decimal, string>
            {
                { 0, "0 非課税" }, // 0: Tax exempt
                { 1, "1 課税" }   // 1: Taxable
            };
            EditSupplier.TaxCd = ComboTaxCd.FirstOrDefault().Key;

            // ✅ 消費税端数 (Tax Fraction / Rounding)
            ComboTaxRounding = new Dictionary<int, string>
            {
                { 0, "0 四捨五入" }, // Round half up
                { 1, "1 切り上げ" }, // Round up
                { 2, "2 切り捨て" }  // Round down
            };
            EditSupplier.TaxRounding = ComboTaxRounding.FirstOrDefault().Key;

            // ✅ 為替端数区分 (Tax Calculation Method)
            ComboTaxCalculationMethod = new Dictionary<int, string>
            {
                { 0, "0 支払単位" }, // 0: payment unit
                { 1, "1 伝票単位" }   // 1: Voucher unit
            };
            EditSupplier.TaxCalculationMethod = ComboTaxCalculationMethod.FirstOrDefault().Key;

            // ✅ 為替区分 (Currency Type)
            ComboCurrencyType = new Dictionary<string, string>
            {
                { "0", "00 円" },      // Japanese Yen
                { "1", "01 ドル" },    // US Dollar
                { "2", "02 ユーロ" },   // Euro
                { "3", "03 人民元" }    // Chinese Yuan
            };
            EditSupplier.CurrencyType = ComboCurrencyType.FirstOrDefault().Key;

            // ✅ 為替端数区分 (Exchange Fraction Division)
            ComboCurrencyRoundingDigit = new Dictionary<int, string>
            {
                { 0, "0 支払単位" }, // Payment unit
                { 1, "1 切り上げ" }, // Round up
                { 2, "2 切り捨て" }  // Round down
            };
            EditSupplier.CurrencyRoundingDigit = ComboCurrencyRoundingDigit.FirstOrDefault().Key;

            ComboCurrencyFractionType = new Dictionary<int, string>
            {
                { 0, "0 支払単位" }, // Payment unit
                { 1, "1 切り上げ" }, // Round up
                { 2, "2 切り捨て" }  // Round down
            };
            EditSupplier.CurrencyFractionType = ComboCurrencyFractionType.FirstOrDefault().Key;
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

                var item = Common.CloneObject(EditSupplier);
                Common.ConvertDotStringAdd(item);
                if (item == null) return;

                var ret = AppData.Http!.AspxSqlExe(
                    DBDef.DB_DML.INSERT,
                    "MASTER_SIIRE", // ← ✅ changed to supplier master table
                    0,
                    "0",
                    new string[]
                    {
                        "仕入先CD","旧コード","略称","仕入先名","カナ","郵便番号","住所1","住所2","住所3","TEL","FAX",
                        "掛率","掛率2","支払先CD","支払印刷","締日","支払予定月","支払予定日","支払方法","消費税CD",
                        "消費税計算方法","消費税端数","支払率","宛名FLG1","宛名FLG2","宛名FLG3","宛名名称1","宛名名称2",
                        "伝票発行区分","伝票印字1","伝票印字2","伝票印字3","伝票印字4","部門","備考","名称CD01","名称CD02",
                        "名称CD03","名称CD04","名称CD05","名称CD06","名称CD07","名称CD08","名称CD09","名称CD10","振込銀行",
                        "振込支店","振込種別","振込口座","備考2","発注FLG","生産FLG","為替区分","為替桁切指定","為替端数区分",
                        "POS区分","入力社員CD","発注停止FLG","仕入区分","法人CD","連携CD","連携先法人CD","仕入先MAIL",
                        "期日","下限額","倉庫郵便番号","倉庫住所1","倉庫住所2","倉庫住所3","倉庫TEL","倉庫FAX",
                        "締日2","支払予定月2","支払予定日2","締日3","支払予定月3","支払予定日3","登録番号"
                    },
                    new string[]
                    {
                        item.SupplierCD!, item.OldCD!, item.Abbreviation!, item.SupplierName!, item.Kana!, item.PostalCode!,
                        item.Address1!, item.Address2!, item.Address3!, item.Tel!, item.Fax!,
                        item.Rate1.ToString()!, item.Rate2.ToString()!, item.PaymentDestinationCd!, item.PaymentPrintFlag.ToString()!,
                        item.ClosingDay.ToString()!, item.ScheduledPaymentMonth.ToString()!, item.ScheduledPaymentDay.ToString()!,
                        item.PaymentMethod!, item.TaxCd.ToString()!, item.TaxCalculationMethod.ToString()!, item.TaxRounding.ToString()!,
                        item.PaymentRate.ToString()!, item.RecipientFlag1!, item.RecipientFlag2!, item.RecipientFlag3!,
                        item.RecipientName1!, item.RecipientName2!, item.SlipIssueType.ToString()!, item.SlipPrint1!, item.SlipPrint2!,
                        item.SlipPrint3!, item.SlipPrint4!, item.Department!, item.Remarks!, item.NameCd01!, item.NameCd02!,
                        item.NameCd03!, item.NameCd04!, item.NameCd05!, item.NameCd06!, item.NameCd07!, item.NameCd08!,
                        item.NameCd09!, item.NameCd10!, item.BankName!, item.BranchName!, item.TransferType!, item.AccountNumber!,
                        item.Remarks2!, item.OrderFlag.ToString()!, item.ProductionFlag.ToString()!, item.CurrencyType!,
                        item.CurrencyRoundingDigit.ToString()!, item.CurrencyFractionType.ToString()!, item.PosType.ToString()!,
                        item.InputEmployeeCd!, item.OrderStopFlag.ToString()!, item.PurchaseType.ToString()!, item.CorporationCd!,
                        item.LinkCd!, item.LinkedCorporationCd!, item.SupplierMail!, item.DueDate.ToString()!, item.MinimumAmount.ToString()!,
                        item.WarehousePostalCode!, item.WarehouseAddress1!, item.WarehouseAddress2!, item.WarehouseAddress3!,
                        item.WarehouseTel!, item.WarehouseFax!, item.ClosingDay2.ToString()!, item.ScheduledPaymentMonth2.ToString()!,
                        item.ScheduledPaymentDay2.ToString()!, item.ClosingDay3.ToString()!, item.ScheduledPaymentMonth3.ToString()!,
                        item.ScheduledPaymentDay3.ToString()!, item.RegistrationNumber!
                    }
                );

                if (ret.Code == 0)
                {
                    item.SeqNo = ret.NewSeq;
                    item.VdateUpdate = decimal.Parse(ret.VDate);
                    item.VdateCreate = item.VdateUpdate;

                    Common.ConvertDotStringDel(item);
                    ListSupplier!.Add(item);
                    SelectedSupplier = item;
                    EditSupplier = Common.CloneObject(item);
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
                                SeqNo = long.TryParse(dr["SEQ_NO"]?.ToString(), out var _seq) ? _seq : 0,
                                VdateCreate = decimal.TryParse(dr["VDATE_CREATE"]?.ToString(), out var _vcreate) ? _vcreate : 0,
                                VdateUpdate = decimal.TryParse(dr["VDATE_UPDATE"]?.ToString(), out var _vupdate) ? _vupdate : 0,

                                SupplierCD = dr["仕入先CD"]?.ToString() ?? string.Empty,
                                OldCD = dr["旧コード"]?.ToString() ?? string.Empty,
                                Abbreviation = dr["略称"]?.ToString() ?? string.Empty,
                                SupplierName = dr["仕入先名"]?.ToString() ?? string.Empty,
                                Kana = dr["カナ"]?.ToString() ?? string.Empty,
                                PostalCode = dr["郵便番号"]?.ToString() ?? string.Empty,
                                Address1 = dr["住所1"]?.ToString() ?? string.Empty,
                                Address2 = dr["住所2"]?.ToString() ?? string.Empty,
                                Address3 = dr["住所3"]?.ToString() ?? string.Empty,
                                Tel = dr["TEL"]?.ToString() ?? string.Empty,
                                Fax = dr["FAX"]?.ToString() ?? string.Empty,

                                Rate1 = decimal.TryParse(dr["掛率"]?.ToString(), out var _rate1) ? _rate1 : 0,
                                Rate2 = decimal.TryParse(dr["掛率2"]?.ToString(), out var _rate2) ? _rate2 : 0,
                                PaymentDestinationCd = dr["支払先CD"]?.ToString() ?? string.Empty,
                                PaymentPrintFlag = int.TryParse(dr["支払印刷"]?.ToString(), out var _payPrint) ? _payPrint : 0,
                                ClosingDay = int.TryParse(dr["締日"]?.ToString(), out var _closeDay) ? _closeDay : 0,
                                ScheduledPaymentMonth = int.TryParse(dr["支払予定月"]?.ToString(), out var _schedMonth) ? _schedMonth : 0,
                                ScheduledPaymentDay = int.TryParse(dr["支払予定日"]?.ToString(), out var _schedDay) ? _schedDay : 0,
                                PaymentMethod = dr["支払方法"]?.ToString() ?? string.Empty,

                                TaxCd = decimal.TryParse(dr["消費税CD"]?.ToString(), out var _taxCd) ? _taxCd : 0,
                                TaxCalculationMethod = int.TryParse(dr["消費税計算方法"]?.ToString(), out var _taxCalc) ? _taxCalc : 0,
                                TaxRounding = int.TryParse(dr["消費税端数"]?.ToString(), out var _taxRound) ? _taxRound : 0,
                                PaymentRate = decimal.TryParse(dr["支払率"]?.ToString(), out var _payRate) ? _payRate : 0,

                                RecipientFlag1 = dr["宛名FLG1"]?.ToString() ?? string.Empty,
                                RecipientFlag2 = dr["宛名FLG2"]?.ToString() ?? string.Empty,
                                RecipientFlag3 = dr["宛名FLG3"]?.ToString() ?? string.Empty,
                                RecipientName1 = dr["宛名名称1"]?.ToString() ?? string.Empty,
                                RecipientName2 = dr["宛名名称2"]?.ToString() ?? string.Empty,

                                SlipIssueType = int.TryParse(dr["伝票発行区分"]?.ToString(), out var _slipType) ? _slipType : 0,
                                SlipPrint1 = dr["伝票印字1"]?.ToString() ?? string.Empty,
                                SlipPrint2 = dr["伝票印字2"]?.ToString() ?? string.Empty,
                                SlipPrint3 = dr["伝票印字3"]?.ToString() ?? string.Empty,
                                SlipPrint4 = dr["伝票印字4"]?.ToString() ?? string.Empty,

                                Department = dr["部門"]?.ToString() ?? string.Empty,
                                Remarks = dr["備考"]?.ToString() ?? string.Empty,

                                NameCd01 = dr["名称CD01"]?.ToString() ?? string.Empty,
                                NameCd02 = dr["名称CD02"]?.ToString() ?? string.Empty,
                                NameCd03 = dr["名称CD03"]?.ToString() ?? string.Empty,
                                NameCd04 = dr["名称CD04"]?.ToString() ?? string.Empty,
                                NameCd05 = dr["名称CD05"]?.ToString() ?? string.Empty,
                                NameCd06 = dr["名称CD06"]?.ToString() ?? string.Empty,
                                NameCd07 = dr["名称CD07"]?.ToString() ?? string.Empty,
                                NameCd08 = dr["名称CD08"]?.ToString() ?? string.Empty,
                                NameCd09 = dr["名称CD09"]?.ToString() ?? string.Empty,
                                NameCd10 = dr["名称CD10"]?.ToString() ?? string.Empty,

                                BankName = dr["振込銀行"]?.ToString() ?? string.Empty,
                                BranchName = dr["振込支店"]?.ToString() ?? string.Empty,
                                TransferType = dr["振込種別"]?.ToString() ?? string.Empty,
                                AccountNumber = dr["振込口座"]?.ToString() ?? string.Empty,
                                Remarks2 = dr["備考2"]?.ToString() ?? string.Empty,

                                OrderFlag = int.TryParse(dr["発注FLG"]?.ToString(), out var _orderFlag) ? _orderFlag : 0,
                                ProductionFlag = int.TryParse(dr["生産FLG"]?.ToString(), out var _prodFlag) ? _prodFlag : 0,
                                CurrencyType = dr["為替区分"]?.ToString() ?? string.Empty,
                                CurrencyRoundingDigit = int.TryParse(dr["為替桁切指定"]?.ToString(), out var _currRoundDigit) ? _currRoundDigit : 0,
                                CurrencyFractionType = int.TryParse(dr["為替端数区分"]?.ToString(), out var _currFrac) ? _currFrac : 0,
                                PosType = int.TryParse(dr["POS区分"]?.ToString(), out var _posType) ? _posType : 0,
                                InputEmployeeCd = dr["入力社員CD"]?.ToString() ?? string.Empty,
                                OrderStopFlag = int.TryParse(dr["発注停止FLG"]?.ToString(), out var _orderStop) ? _orderStop : 0,
                                PurchaseType = int.TryParse(dr["仕入区分"]?.ToString(), out var _purchaseType) ? _purchaseType : 0,
                                CorporationCd = dr["法人CD"]?.ToString() ?? string.Empty,
                                LinkCd = dr["連携CD"]?.ToString() ?? string.Empty,
                                LinkedCorporationCd = dr["連携先法人CD"]?.ToString() ?? string.Empty,
                                SupplierMail = dr["仕入先MAIL"]?.ToString() ?? string.Empty,

                                DueDate = int.TryParse(dr["期日"]?.ToString(), out var _dueDate) ? _dueDate : 0,
                                MinimumAmount = decimal.TryParse(dr["下限額"]?.ToString(), out var _minAmt) ? _minAmt : 0,

                                ClosingDay2 = int.TryParse(dr["締日2"]?.ToString(), out var _closeDay2) ? _closeDay2 : 0,
                                ScheduledPaymentMonth2 = int.TryParse(dr["支払予定月2"]?.ToString(), out var _schedMonth2) ? _schedMonth2 : 0,
                                ScheduledPaymentDay2 = int.TryParse(dr["支払予定日2"]?.ToString(), out var _schedDay2) ? _schedDay2 : 0,
                                ClosingDay3 = int.TryParse(dr["締日3"]?.ToString(), out var _closeDay3) ? _closeDay3 : 0,
                                ScheduledPaymentMonth3 = int.TryParse(dr["支払予定月3"]?.ToString(), out var _schedMonth3) ? _schedMonth3 : 0,
                                ScheduledPaymentDay3 = int.TryParse(dr["支払予定日3"]?.ToString(), out var _schedDay3) ? _schedDay3 : 0,

                                RegistrationNumber = dr["登録番号"]?.ToString() ?? string.Empty
                            }).OrderBy(c => c.SupplierCD).ToList();

                Common.ConvertDotStringDel(list);
                ListSupplier = new ObservableCollection<MasterSupplier>(list);

                if (ListSupplier.Count > 0)
                {
                    SelectedSupplier = ListSupplier[0];
                }
            }

            [RelayCommand]
            void DoUpdate()
            {
                // Confirm update
                if (!ClientLib.MessageBox(this, "修正しますか？")) return;

                // Clone and prepare the editing object
                var item = Common.CloneObject(EditSupplier);
                Common.ConvertDotStringAdd(item);
                if (item == null) return;

                // Execute UPDATE SQL
                var ret = AppData.Http!.AspxSqlExe(
                    DBDef.DB_DML.UPDATE,
                    "HC$MASTER_SIIRE", // ← table name for supplier master
                    item.SeqNo,
                    item.VdateUpdate.ToString(),
                    new string[]
                    {
                        "仕入先CD","旧コード","略称","仕入先名","カナ","郵便番号","住所1","住所2","住所3","TEL","FAX",
                        "掛率","掛率2","支払先CD","支払印刷","締日","支払予定月","支払予定日","支払方法","消費税CD",
                        "消費税計算方法","消費税端数","支払率","宛名FLG1","宛名FLG2","宛名FLG3","宛名名称1","宛名名称2",
                        "伝票発行区分","伝票印字1","伝票印字2","伝票印字3","伝票印字4","部門","備考","名称CD01","名称CD02",
                        "名称CD03","名称CD04","名称CD05","名称CD06","名称CD07","名称CD08","名称CD09","名称CD10","振込銀行",
                        "振込支店","振込種別","振込口座","備考2","発注FLG","生産FLG","為替区分","為替桁切指定","為替端数区分",
                        "POS区分","入力社員CD","発注停止FLG","仕入区分","法人CD","連携CD","連携先法人CD","仕入先MAIL",
                        "期日","下限額","倉庫郵便番号","倉庫住所1","倉庫住所2","倉庫住所3","倉庫TEL","倉庫FAX",
                        "締日2","支払予定月2","支払予定日2","締日3","支払予定月3","支払予定日3","登録番号"
                    },
                    new string[]
                    {
                        item.SupplierCD!, item.OldCD!, item.Abbreviation!, item.SupplierName!, item.Kana!, item.PostalCode!,
                        item.Address1!, item.Address2!, item.Address3!, item.Tel!, item.Fax!,
                        item.Rate1.ToString()!, item.Rate2.ToString()!, item.PaymentDestinationCd!, item.PaymentPrintFlag.ToString()!,
                        item.ClosingDay.ToString()!, item.ScheduledPaymentMonth.ToString()!, item.ScheduledPaymentDay.ToString()!,
                        item.PaymentMethod!, item.TaxCd.ToString()!, item.TaxCalculationMethod.ToString()!, item.TaxRounding.ToString()!,
                        item.PaymentRate.ToString()!, item.RecipientFlag1!, item.RecipientFlag2!, item.RecipientFlag3!,
                        item.RecipientName1!, item.RecipientName2!, item.SlipIssueType.ToString()!, item.SlipPrint1!, item.SlipPrint2!,
                        item.SlipPrint3!, item.SlipPrint4!, item.Department!, item.Remarks!, item.NameCd01!, item.NameCd02!,
                        item.NameCd03!, item.NameCd04!, item.NameCd05!, item.NameCd06!, item.NameCd07!, item.NameCd08!,
                        item.NameCd09!, item.NameCd10!, item.BankName!, item.BranchName!, item.TransferType!, item.AccountNumber!,
                        item.Remarks2!, item.OrderFlag.ToString()!, item.ProductionFlag.ToString()!, item.CurrencyType!,
                        item.CurrencyRoundingDigit.ToString()!, item.CurrencyFractionType.ToString()!, item.PosType.ToString()!,
                        item.InputEmployeeCd!, item.OrderStopFlag.ToString()!, item.PurchaseType.ToString()!, item.CorporationCd!,
                        item.LinkCd!, item.LinkedCorporationCd!, item.SupplierMail!, item.DueDate.ToString()!, item.MinimumAmount.ToString()!,
                        item.WarehousePostalCode!, item.WarehouseAddress1!, item.WarehouseAddress2!, item.WarehouseAddress3!,
                        item.WarehouseTel!, item.WarehouseFax!, item.ClosingDay2.ToString()!, item.ScheduledPaymentMonth2.ToString()!,
                        item.ScheduledPaymentDay2.ToString()!, item.ClosingDay3.ToString()!, item.ScheduledPaymentMonth3.ToString()!,
                        item.ScheduledPaymentDay3.ToString()!, item.RegistrationNumber!
                    }
                );

                // ✅ Update result handling
                if (ret.Code == 0)
                {
                    Common.ConvertDotStringDel(item);
                    if (SelectedSupplier != null)
                    {
                        SelectedSupplier.VdateUpdate = decimal.Parse(ret.VDate);
                        // Update all properties on SelectedSupplier from item
                        SelectedSupplier = Common.CloneObject(item);
                        EditSupplier = Common.CloneObject(SelectedSupplier);
                    }
                }
                else
                {
                    ClientLib.MessageBoxError(this, ret.Code.ToString());
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
