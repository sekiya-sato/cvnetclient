using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Class;
using CvnetClient.Interface;
using CvnetClient.Models;
using CvnetClient.Service;
using CvnetClient.Utils;
using CvnetClient.Views;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg01bcdViewModel : BaseViewModel
    {
        // ------------------------------------------------------
        // BizD-compatible internal vars
        // ------------------------------------------------------
        private BizArray para;
        private BizArray sv_cat;
        private BizArray wrk_jodai;

        // ------------------------------------------------------
        // Observable properties (for data binding)
        // ------------------------------------------------------
        [ObservableProperty] private ProductSelector selectProduct = new();
        [ObservableProperty] private int selProductSelect = 0; // 0=範囲指定, 1=個別指定
        [ObservableProperty] private BcdSearchOpt searchOpt = new();
        [ObservableProperty] private SelectProductType? rangeFrom = new();
        [ObservableProperty] private SelectProductType? rangeTo = new();

        public bool IsRangeSelected => SelProductSelect == 0;
        public bool IsIndividualSelected => SelProductSelect == 1;

        // ------------------------------------------------------
        // 初期化 (BizD互換)
        // ------------------------------------------------------
        public void OnInit(string[] init_para = null)
        {
            para = new BizArray();
            sv_cat = new BizArray();
            wrk_jodai = new BizArray();

            if (init_para != null)
                para = new BizArray(init_para);

            // reset product selection and print options
            SelectProduct = new ProductSelector();
            SearchOpt = new BcdSearchOpt();

        }

        [RelayCommand]
        public void SelRangeFrom(object value)
        {
            if (value is SelValueModel get_sel00)
            {
                RangeFrom ??= new();
                RangeFrom.Code = get_sel00.Code;
                RangeFrom.Name = get_sel00.Name;

                // auto-copy logic (BizD style)
                if (string.IsNullOrEmpty(RangeTo?.Code) || RangeTo.Code == "99999999")
                {
                    RangeTo ??= new();
                    RangeTo.Code = get_sel00.Code;
                    RangeTo.Name = get_sel00.Name;
                }
            }
        }

        [RelayCommand]
        public void SelRangeTo(object value)
        {
            if (value is SelValueModel get_sel00)
            {
                RangeTo ??= new();
                RangeTo.Code = get_sel00.Code;
                RangeTo.Name = get_sel00.Name;
            }
        }

        // ------------------------------------------------------
        // 商品選択処理
        // ------------------------------------------------------
        [RelayCommand]
        public void SelSelectProduct(object value)
        {
            if (value is not SelValueModel get_sel00)
                return;

            string selectedCode = get_sel00.Code ?? string.Empty;
            string selectedName = get_sel00.Name ?? string.Empty;

            // Loop through Code01–Code20 dynamically (fill first empty)
            for (int i = 1; i <= 20; i++)
            {
                var codeProp = typeof(ProductSelector).GetProperty($"Code{i:D2}");
                var nameProp = typeof(ProductSelector).GetProperty($"Name{i:D2}");
                if (codeProp == null || nameProp == null)
                    continue;

                string? currentCode = codeProp.GetValue(SelectProduct) as string;
                if (string.IsNullOrEmpty(currentCode))
                {
                    codeProp.SetValue(SelectProduct, selectedCode);
                    nameProp.SetValue(SelectProduct, selectedName);
                    break;
                }
            }
        }

        // ------------------------------------------------------
        // 印刷処理
        // ------------------------------------------------------
        [RelayCommand]
        public async Task DoPrintAsync()
        {
            if (!ClientLib.MessageBox(this, "印刷しますか？")) return;
            ClientLib.CursorToWait();

            try
            {
                string sql_query = @"
                    select A.商品CD, A.商品名,
                           NVL(B.サイズCD ||' '||get_sizename(A.商品CD,B.サイズCD),'.') サイズ,
                           NVL(B.色CD ||' '||(select H.名称 from HC$MASTER_MEISHO H where H.名称区分='COL' and H.名称CD=B.色CD),'.') 色,
                           B.JANコード1, B.JANコード2, B.JANコード3
                    from HC$MASTER_SHOHIN A, HC$MASTER_SHOHIN_JAN B
                    where A.商品CD = B.商品CD(+)
                ";

                var v_para = new BizArray();

                // 範囲指定 or 個別指定
                if (SelProductSelect == 0)
                {
                    // ✅ Range mode (use RangeFrom & RangeTo)
                    v_para[0] = RangeFrom?.Code ?? "0";
                    v_para[1] = RangeTo?.Code ?? "ZZZZZZZZ";
                    sql_query += " and A.商品CD between :1 and :2";
                }
                else
                {
                    // ✅ Individual mode
                    var selectedCodes = new System.Collections.Generic.List<string>();
                    for (int i = 1; i <= 20; i++)
                    {
                        var codeProp = typeof(ProductSelector).GetProperty($"Code{i:D2}");
                        string? code = codeProp?.GetValue(SelectProduct) as string;
                        if (!string.IsNullOrWhiteSpace(code))
                            selectedCodes.Add($"'{code}'");
                    }

                    if (selectedCodes.Count == 0)
                    {
                        ClientLib.MessageBoxError(this, "商品が選択されていません。");
                        ClientLib.CursorToNormal();
                        return;
                    }

                    sql_query += $" and A.商品CD in ({string.Join(",", selectedCodes)})";
                }

                // 出力区分 (正規商品 / 中止商品 / 全て)
                switch (SearchOpt.AddressCustomer)
                {
                    case 0: sql_query += " and B.使用FLG = 0"; break;
                    case 1: sql_query += " and B.使用FLG = 1"; break;
                    default: break;
                }

                sql_query += " order by A.商品CD, B.色CD, B.サイズCD";
                sql_query = $"select * from ({sql_query})";

                // バーコードテンプレート (BizD互換)
                string qfm = "cvnet_bcdJAN.qfm";
                if (SearchOpt.BarcodeLayout == 1)
                {
                    qfm = SearchOpt.BarcodeType switch
                    {
                        0 => "cvnet_bcd_exJAN.qfm",
                        1 => "cvnet_bcd_exCODE39.qfm",
                        2 => "cvnet_bcd_exNW7.qfm",
                        _ => qfm
                    };
                }
                else
                {
                    qfm = SearchOpt.BarcodeType switch
                    {
                        0 => "cvnet_bcdJAN.qfm",
                        1 => "cvnet_bcdCODE39.qfm",
                        2 => "cvnet_bcdNW7.qfm",
                        _ => qfm
                    };
                }

                // SQL + QFM 実行
                var resp = AppData.Http!.AspxSqlQueryCsv(sql_query, v_para.ToArray(), qfm);
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
            catch (Exception ex)
            {
                ClientLib.CursorToNormal();
                ClientLib.MessageBoxError(this, $"印刷中にエラーが発生しました:\n{ex.Message}");
            }
        }

        // ------------------------------------------------------
        // 終了
        // ------------------------------------------------------
        [RelayCommand]
        private void DoExit()
        {
            ClientLib.ExitDialogResult(this, true);
        }
    }

    // --------------------------------------------------------
    // Helper: 商品CD + 名称 (20 ペア)
    // --------------------------------------------------------
    public partial class ProductSelector : ObservableObject
    {

        [ObservableProperty] private string? code01;
        [ObservableProperty] private string? name01;
        [ObservableProperty] private string? code02;
        [ObservableProperty] private string? name02;
        [ObservableProperty] private string? code03;
        [ObservableProperty] private string? name03;
        [ObservableProperty] private string? code04;
        [ObservableProperty] private string? name04;
        [ObservableProperty] private string? code05;
        [ObservableProperty] private string? name05;
        [ObservableProperty] private string? code06;
        [ObservableProperty] private string? name06;
        [ObservableProperty] private string? code07;
        [ObservableProperty] private string? name07;
        [ObservableProperty] private string? code08;
        [ObservableProperty] private string? name08;
        [ObservableProperty] private string? code09;
        [ObservableProperty] private string? name09;
        [ObservableProperty] private string? code10;
        [ObservableProperty] private string? name10;
        [ObservableProperty] private string? code11;
        [ObservableProperty] private string? name11;
        [ObservableProperty] private string? code12;
        [ObservableProperty] private string? name12;
        [ObservableProperty] private string? code13;
        [ObservableProperty] private string? name13;
        [ObservableProperty] private string? code14;
        [ObservableProperty] private string? name14;
        [ObservableProperty] private string? code15;
        [ObservableProperty] private string? name15;
        [ObservableProperty] private string? code16;
        [ObservableProperty] private string? name16;
        [ObservableProperty] private string? code17;
        [ObservableProperty] private string? name17;
        [ObservableProperty] private string? code18;
        [ObservableProperty] private string? name18;
        [ObservableProperty] private string? code19;
        [ObservableProperty] private string? name19;
        [ObservableProperty] private string? code20;
        [ObservableProperty] private string? name20;
    }

    // --------------------------------------------------------
    // 印刷設定オプション
    // --------------------------------------------------------
    public partial class BcdSearchOpt : ObservableObject
    {
        // 出力区分: 0=正規商品, 1=中止品番, 2=全て
        [ObservableProperty] private int addressCustomer = 0;

        // バーコード種: 0=JAN, 1=CODE39, 2=NW7
        [ObservableProperty] private int barcodeType = 0;

        // バーコード並び: 0=通常, 1=拡張
        [ObservableProperty] private int barcodeLayout = 0;

        public BcdSearchOpt()
        {
            AddressCustomer = 0; // 正規商品
            BarcodeType = 0;     // JAN
            BarcodeLayout = 0;   // 通常
        }
    }
    public partial class SelectProductType : ObservableObject
    {
        [ObservableProperty] private string? code;
        [ObservableProperty] private string? name;
    }

}