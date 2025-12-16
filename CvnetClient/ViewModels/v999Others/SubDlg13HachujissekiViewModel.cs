using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Class;
using CvnetClient.Models;
using CvnetClient.Utils;
using CvnetClient.Views;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg13HachujissekiViewModel : BaseViewModel
    {
        public enum MethodType { Supplier, WareHouse, SW, WS }
        public enum ProductType { No, Product, SKU }
        public enum OutPutType { Spool, CSV }
        public enum List1Type { And, Or }
        public enum List2Type { And, Or }
        public enum List3Type { And, Or }
        [ObservableProperty]
        private List1Type selectedList1;
        [ObservableProperty]
        private List2Type selectedList2;
        [ObservableProperty]
        private List3Type selectedList3;
        [ObservableProperty]
        private OutPutType selectedOutput;
        [ObservableProperty]
        private ProductType selectedProduct;
        [ObservableProperty]
        private MethodType selectedMethod;
        [ObservableProperty]
        ListFlexData listFlexData1 = new ListFlexData();
        [ObservableProperty]
        ListFlexData listFlexData2 = new ListFlexData();
        [ObservableProperty]
        ListFlexData listFlexData3 = new ListFlexData();
        [ObservableProperty]
        SearchCondition condition;
        [ObservableProperty]
        private string menukbn = "0";
        public void OnInit(object? init_para = null) 
        {
            OnInitBase(init_para);
            if (para[0] != "") {
                Menukbn = para[0];
            }
            Condition = new SearchCondition();
            Condition.CodeNen1 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            Condition.CodeNen2 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddDays(-1);
            Condition.CodeNen3 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            Condition.CodeNen4 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(4).AddDays(-1);
            Condition.CodeSiire2 = new BtListHelper("99999999","");
            Condition.CodeTenpo2 = new BtListHelper("99999999","");
            Condition.CodeSyohin2 = new BtListHelper("ZZZZZZZZZZZZZZZZ","");
            SelectedOutput = OutPutType.Spool;
            SelectedProduct = ProductType.No;
            SelectedMethod = MethodType.Supplier;
            SelectedList1 = List1Type.And;
            SelectedList2 = List2Type.And;
            SelectedList3 = List3Type.And;
            List<CsvItem> def = null;
            ListFlexData1.ListConfig = new ListFlexConfig()
            {
                init_csv = def ?? new List<CsvItem>(),
                flag = 0
            };
            ListFlexData2.ListConfig = new ListFlexConfig()
            {
                init_csv = def ?? new List<CsvItem>(),
                flag = 1
            };
            ListFlexData3.ListConfig = new ListFlexConfig()
            {
                init_csv = def ?? new List<CsvItem>(),
                flag = 2
            };
        }
        [RelayCommand]
        async Task DoPrintAsync() 
        {
            var wrk_para = new string[17];
            wrk_para[0] = Condition.CodeNen1.ToString("yyyyMMdd");
            wrk_para[1] = Condition.CodeNen2.ToString("yyyyMMdd");
            wrk_para[2] = Condition.CodeNen3.ToString("yyyyMMdd");
            wrk_para[3] = Condition.CodeNen4.ToString("yyyyMMdd");
            wrk_para[4] = Condition.CodeSiire1?.Code ?? string.Empty;
            wrk_para[5] = Condition.CodeSiire2?.Code ?? string.Empty;
            /* 仕入先分類条件追加 */
            var wrk_para2 = new BizArray();
            int v_flg3 = 0;
            if (SelectedList3 == List3Type.Or) {
                v_flg3 = 1;
            }
            var v_sql = ListFlexData3.GetQueryStr2(wrk_para2, 1, v_flg3, "si.");
            wrk_para[6] = new String(v_sql + " ");
            wrk_para[7] = Condition.CodeTenpo1?.Code ?? string.Empty;
            wrk_para[8] = Condition.CodeTenpo2?.Code ?? string.Empty;
            /* 入庫先分類条件追加 */
            var wrk_para3 = new BizArray();
            int v_flg2 = 0;
            if (SelectedList2 == List2Type.Or)
            {
                v_flg2 = 1;
            }
            var v_sql2 = ListFlexData2.GetQueryStr2(wrk_para3, 1, v_flg2, "tk.");
            wrk_para[9] = new String(v_sql2 + " ");
            wrk_para[10] = Condition.CodeSyohin1?.Code ?? string.Empty;
            wrk_para[11] = Condition.CodeSyohin2?.Code ?? string.Empty;
            /* 商品分類条件追加 */
            var wrk_para4 = new BizArray();
            int v_flg1 = 0;
            if (SelectedList1 == List1Type.Or)
            {
                v_flg1 = 1;
            }
            var v_sql3 = ListFlexData1.GetQueryStr2(wrk_para4, 1, v_flg1, "sh.");
            wrk_para[12] = new String(v_sql3 + " ");
            if (SelectedMethod == MethodType.Supplier)
            {
                wrk_para[13] = "0";
            } else if (SelectedMethod == MethodType.WareHouse) {
                wrk_para[13] = "1";
            }
            else if (SelectedMethod == MethodType.SW)
            {
                wrk_para[13] = "2";
            }
            else if (SelectedMethod == MethodType.WS)
            {
                wrk_para[13] = "3";
            }
            if (SelectedProduct == ProductType.No) {
                wrk_para[14] = "0";
            }else if (SelectedProduct == ProductType.Product)
            {
                wrk_para[14] = "1";
            }
            else if (SelectedProduct == ProductType.SKU)
            {
                wrk_para[14] = "2";
            }
            if (SelectedOutput == OutPutType.Spool) {
                wrk_para[15] = "0";
            }
            else
            {
                wrk_para[15] = "1";
            }
            wrk_para[16] = Menukbn;

            var ret_csv = OnQuery(wrk_para);
            var lines = ret_csv.Split('\n');

            if (lines.Length < 2 || lines[1] == "0")
            {
                ClientLib.MessageBoxError(this, "PDFデータがありません");
                return;
            }

            string pdfPath = lines[0];

            if (SelectedOutput == OutPutType.Spool)
            {
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
            else
            {
                var csv_para = new BizCsvDocument();
                string datapath = AppData.Http.URLroot + pdfPath + "/data.txt";
                string headpath = AppData.Http.URLroot + pdfPath + "/d_sql.txt";
                bool ready = await Utils.GlobalFunc.WaitForPdfAsync(datapath, TimeSpan.FromSeconds(30));
                if (!ready)
                {
                    ClientLib.MessageBoxError(this, "Data生成に時間がかかりすぎています。\n 条件を絞ってください。");
                    return;
                }

                ready = await Utils.GlobalFunc.WaitForPdfAsync(headpath, TimeSpan.FromSeconds(30));
                if (!ready)
                {
                    ClientLib.MessageBoxError(this, "Header生成に時間がかかりすぎています。\n 条件を絞ってください。");
                    return;
                }
                await csv_para.LoadFromUrlAsync(datapath, headpath);
                try
                {
                    csv_para.SaveCsv(DateTime.Now.ToString("yyyyMMdd") + "_得意先別売上日報");
                }
                catch (Exception ex) { }
            }
        }
        string OnQuery(string[] wrk_para)
        {
            string qfm_file = "";
            int value1 = 0;
            int value2 = 0;
            if (SelectedMethod == MethodType.Supplier) {
                value1 = 0;
            } else if (SelectedMethod == MethodType.WareHouse) {
                value1 = 1;
            } else if (SelectedMethod == MethodType.SW) {
                value1 = 2;
            } else if (SelectedMethod == MethodType.WS) {
                value1 = 3;
            }

            if (SelectedProduct == ProductType.No) {
                value2 = 0;
            }
            else if(SelectedProduct == ProductType.Product)
            {
                value2 = 1;
            }else if(SelectedProduct == ProductType.SKU)
            {
                value2 = 2;
            }
            switch (value1)
            {
                case 0:
                case 1:
                    switch (value2)
                    {
                        case 0:
                            qfm_file = "cvnet13_hachujisseki_1_1.qfm";
                            break;
                        case 1:
                        case 2:
                            qfm_file = "cvnet13_hachujisseki_1_2.qfm";
                            break;
                        default:
                            break;
                    }
                    break;
                case 2:
                case 3:
                    switch (value2)
                    {
                        case 0:
                            qfm_file = "cvnet13_hachujisseki_2_1.qfm";
                            break;
                        case 1:
                        case 2:
                            qfm_file = "cvnet13_hachujisseki_2_2.qfm";
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
            return AppData.Http!.AspxSqlQueryCsv("_hachujisseki", wrk_para, qfm_file, 05);
        }
        [RelayCommand]
        public void SelSiire1(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Condition != null)
            {
                Condition.CodeSiire1 = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        [RelayCommand]
        public void SelSiire2(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Condition != null)
            {
                Condition.CodeSiire2 = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }

        [RelayCommand]
        public void SelTenpo1(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Condition != null)
            {
                Condition.CodeTenpo1 = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        [RelayCommand]
        public void SelTenpo2(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Condition != null)
            {
                Condition.CodeTenpo2 = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }

        [RelayCommand]
        public void SelSyohin1(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Condition != null)
            {
                Condition.CodeSyohin1 = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        [RelayCommand]
        public void SelSyohin2(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Condition != null)
            {
                Condition.CodeSyohin2 = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }

        public partial class SearchCondition : ObservableObject
        {
            [ObservableProperty]
            private DateTime codeNen1;
            [ObservableProperty]
            private DateTime codeNen2;
            [ObservableProperty]
            private DateTime codeNen3;
            [ObservableProperty]
            private DateTime codeNen4;
            [ObservableProperty] 
            private BtListHelper? codeSiire1;
            [ObservableProperty]
            private BtListHelper? codeSiire2;
            [ObservableProperty]
            private BtListHelper? codeTenpo1;
            [ObservableProperty]
            private BtListHelper? codeTenpo2;
            [ObservableProperty]
            private BtListHelper? codeSyohin1;
            [ObservableProperty]
            private BtListHelper? codeSyohin2;           
        }
    }
}
