using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Class;
using CvnetClient.Models;
using CvnetClient.Utils;
using CvnetClient.Views;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg_prn_kakumstViewModel : BaseViewModel
    {
        private BizArray para;
        public enum PrintType { スプール, CSV }

        // ---------------- Tabs & Header ----------------
        public ObservableCollection<string> ChooseTab { get; } = new()
        {
            "商品マスタ", "得意先マスタ", "仕入先マスタ",
            "名称マスタ", "生地付属マスタ", "社員マスタ"
        };

        [ObservableProperty] private string selectedTab = "";
        [ObservableProperty] private int selectedTabIndex;
        [ObservableProperty] private int dateFlag = 1; // 1=修正日, 0=作成日
        [ObservableProperty] private BtListHelper employee = new();
        [ObservableProperty] SearchCondition? condition;
        [ObservableProperty] private string? selectedOrder;

        // ---------------- Tab4 (名称マスタ) ----------------
        [ObservableProperty] private ObservableCollection<KeyValuePair<string, string>> kubunOptions = new();
        [ObservableProperty] private string selectedKubun = "";   // stores code only (e.g. "B01")
        [ObservableProperty] private BtListHelper rangeFrom3 = new();
        [ObservableProperty] private BtListHelper rangeTo3 = new();

        public partial class SearchCondition : ObservableObject
        {
            [ObservableProperty] private DateTime? dateFrom;
            [ObservableProperty] private DateTime? dateTo;
            [ObservableProperty] private PrintType selectedPrint = PrintType.スプール;
        }

        // --------------- Range Row (used by Tab1/2/3/5/6) ---------------
        public partial class RangeRow : ObservableObject
        {
            [ObservableProperty] private string label1 = "";
            [ObservableProperty] private string label2 = "";
            [ObservableProperty] private string mstname1 = "";
            [ObservableProperty] private string mstname2 = "";
            [ObservableProperty] private string parameter1 = "";
            [ObservableProperty] private string parameter2 = "";
            [ObservableProperty] private BtListHelper rangeFrom1 = new();
            [ObservableProperty] private BtListHelper rangeTo1 = new();
            [ObservableProperty] private BtListHelper rangeFrom2 = new();
            [ObservableProperty] private BtListHelper rangeTo2 = new();

            // Helpers to accept either SelValueModel OR (SelValueModel, object)
            private static SelValueModel? ExtractSel(object value, out string param2Str)
            {
                param2Str = "";
                // tuple?
                if (value is ValueTuple<object, object> t)
                {
                    var s = t.Item1 as SelValueModel;
                    if (t.Item2 is string s2) param2Str = s2;
                    return s;
                }
                // plain
                return value as SelValueModel;
            }

            // ----- Commands used by both Tab1A/Tab2/Tab3/Tab5 and Tab1B/Tab6B (tuple) -----
            [RelayCommand]
            private void PickFrom1(object value)
            {
                var sel = ExtractSel(value, out var p2);
                if (sel == null) return;
                RangeFrom1.Code = sel.Code;
                RangeFrom1.Name = sel.Name;
            }

            [RelayCommand]
            private void PickTo1(object value)
            {
                var sel = ExtractSel(value, out var p2);
                if (sel == null) return;
                RangeTo1.Code = sel.Code;
                RangeTo1.Name = sel.Name;
            }

            [RelayCommand]
            private void PickFrom2(object value)
            {
                var sel = ExtractSel(value, out var p2);
                if (sel == null) return;
                RangeFrom2.Code = sel.Code;
                RangeFrom2.Name = sel.Name;
            }

            [RelayCommand]
            private void PickTo2(object value)
            {
                var sel = ExtractSel(value, out var p2);
                if (sel == null) return;
                RangeTo2.Code = sel.Code;
                RangeTo2.Name = sel.Name;
            }
        }

        // ---------------- Tab Collections ----------------
        public ObservableCollection<RangeRow> Tab1RowsA { get; } = new();
        public ObservableCollection<RangeRow> Tab1RowsB { get; } = new();
        public ObservableCollection<RangeRow> Tab2Rows { get; } = new();
        public ObservableCollection<RangeRow> Tab3Rows { get; } = new();
        public ObservableCollection<RangeRow> Tab5Rows { get; } = new();
        public ObservableCollection<RangeRow> Tab6RowsA { get; } = new();
        public ObservableCollection<RangeRow> Tab6RowsB { get; } = new();

        // ---------------- Templates ----------------
        private readonly (string, string)[] _labelTab1A =
        {
            ("ﾌﾞﾗﾝﾄ", "ｱｲﾃﾑ"),
            ("ｼｰｽﾞﾝ", "ﾃﾞｻﾞｲﾅｰ"),
            ("展示会", "素材"),
            ("原産国", "ﾒｰｶｰ"),
        };

        private readonly (string, string)[] _labelTab1B =
        {
            ("補足 1", "補足 6"),
            ("補足 2", "補足 7"),
            ("補足 3", "補足 8"),
            ("補足 4", "補足 9"),
            ("補足 5", "補足 10"),
        };

        private readonly string[] _labelTab2 = { "得意先CD", "営業担当CD", "請求先CD" };
        private readonly string[] _labelTab3 = { "仕入先CD", "支払先CD" };
        private readonly string[] _labelTab5 = { "生地付属CD", "仕入先CD" };
        private readonly string[] _labelTab6A = { "社員CD", "所属店舗CD" };

        private readonly (string, string)[] _MstNameTab1A =
        {
            ("ブランド", "アイテム"),
            ("シーズン", "デザイナー"),
            ("展示会", "素材"),
            ("原産国", "メーカー"),
        };

        private readonly (string, string)[] _ParameterTab1B =
        {
            ("B01", "B06"),
            ("B02", "B07"),
            ("B03", "B08"),
            ("B04", "B09"),
            ("B05", "B10"),
        };

        private readonly string[] _MstNameTab2 = { "得意先MST", "営業担当", "請求" };
        private readonly string[] _MstNameTab3 = { "仕入先", "支払" };
        private readonly string[] _MstNameTab5 = { "生地", "仕入先" };
        private readonly string[] _MstNameTab6 = { "担当", "移動倉庫" };
        private readonly string[] _ParameterTab6B = { "E01", "E02", "E03", "E04", "E05" };

        public SubDlg_prn_kakumstViewModel()
        {
            Condition = new SearchCondition();
            OnInit();
        }
        // ---------------- Init ----------------
        private void OnInit(string[] init_para = null)
        {
            para = init_para != null ? new BizArray(init_para) : new BizArray();

            Employee = new BtListHelper();
            SelectedTab = ChooseTab[0];
            UpdateSelectedTabIndex(SelectedTab);

            InitTab1();
            InitTab2();
            InitTab3();
            InitTab4();
            InitTab5();
            InitTab6();

            UpdateLabelsFromDb(); // rename Tab1RowsB labels by DB IDX (B01..B10)
        }

        private void InitTab1()
        {
            Tab1RowsA.Clear();
            Tab1RowsB.Clear();

            for (int i = 0; i < _labelTab1A.Length; i++)
            {
                var (l1, l2) = _labelTab1A[i];
                var (m1, m2) = _MstNameTab1A[i];
                Tab1RowsA.Add(new RangeRow{Label1 = l1,Label2 = l2,Mstname1 = m1,Mstname2 = m2});
            }

            for (int i = 0; i < _labelTab1B.Length; i++)
            {
                var (l1, l2) = _labelTab1B[i];
                var (p1, p2) = _ParameterTab1B[i];
                Tab1RowsB.Add(new RangeRow{Label1 = l1,Label2 = l2,Mstname1 = "名称",Mstname2 = "名称",Parameter1 = p1,Parameter2 = p2});
            }
        }

        private void InitTab2()
        {
            Tab2Rows.Clear();
            for (int i = 0; i < _labelTab2.Length; i++)
            {
                Tab2Rows.Add(new RangeRow{Label1 = _labelTab2[i],Mstname1 = _MstNameTab2[i]});
            }
        }

        private void InitTab3()
        {
            Tab3Rows.Clear();
            for (int i = 0; i < _labelTab3.Length; i++)
            {
                Tab3Rows.Add(new RangeRow{Label1 = _labelTab3[i],Mstname1 = _MstNameTab3[i]});
            }
        }

        private void InitTab4()
        {
            KubunOptions.Clear();

            try
            {
                const string sql = @"
                    SELECT A.名称CD, A.名称
                    FROM HC$Master_MEISHO A
                    WHERE A.名称区分='IDX'
                    ORDER BY A.名称CD";

                var dt = AppData.Http?.AspxSqlQuery(sql, null);
                if (dt == null || dt.Rows.Count == 0) return;

                foreach (DataRow row in dt.Rows)
                {
                    string code = row["名称CD"]?.ToString()?.Trim() ?? "";
                    string name = row["名称"]?.ToString()?.Trim() ?? "";
                    if (!string.IsNullOrEmpty(code))
                        KubunOptions.Add(new KeyValuePair<string, string>(code, $"{code} {name}"));
                }

                SelectedKubun = KubunOptions.FirstOrDefault().Key ?? "";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[InitTab4 Error] {ex.Message}");
            }
        }

        private void InitTab5()
        {
            Tab5Rows.Clear();
            for (int i = 0; i < _labelTab5.Length; i++)
            {
                Tab5Rows.Add(new RangeRow{Label1 = _labelTab5[i],Mstname1 = _MstNameTab5[i]});
            }
        }

        private void InitTab6()
        {
            Tab6RowsA.Clear();
            Tab6RowsB.Clear();

            // A-section
            for (int i = 0; i < _labelTab6A.Length; i++)
            {
                Tab6RowsA.Add(new RangeRow{Label1 = _labelTab6A[i],Mstname1 = _MstNameTab6[i]});
            }

            // B-section (社員分類 E01..E05)
            for (int i = 0; i < _ParameterTab6B.Length; i++)
            {
                Tab6RowsB.Add(new RangeRow{Label1 = $"社員分類{i + 1}",Mstname1 = "名称",Parameter1 = _ParameterTab6B[i]});
            }
        }

        // --------------- DB Label Update (Tab1RowsB labels) ---------------
        private void UpdateLabelsFromDb()
        {
            try
            {
                const string sql = @"
                    SELECT 名称CD, 名称
                    FROM HC$master_meisho
                    WHERE 名称区分='IDX' AND 名称CD BETWEEN 'B01' AND 'B10'
                    ORDER BY 名称CD";

                var dt = AppData.Http?.AspxSqlQuery(sql, null);
                if (dt == null || dt.Rows.Count == 0) return;

                var labelDict = dt.AsEnumerable()
                    .Where(r => !string.IsNullOrEmpty(r.Field<string>("名称CD")))
                    .ToDictionary(r => r.Field<string>("名称CD")!.Trim(),
                                  r => r.Field<string>("名称")?.Trim() ?? "");

                foreach (var row in Tab1RowsB)
                {
                    if (!string.IsNullOrEmpty(row.Parameter1) && labelDict.TryGetValue(row.Parameter1, out var v1))
                        row.Label1 = v1;
                    if (!string.IsNullOrEmpty(row.Parameter2) && labelDict.TryGetValue(row.Parameter2, out var v2))
                        row.Label2 = v2;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[UpdateLabelsFromDb] Error: {ex.Message}");
            }
        }

        // ---------------- Commands (header + Tab4) ----------------
        [RelayCommand] private void ToggleDateFlag() => DateFlag = 1 - DateFlag;

        [RelayCommand]
        private void ChooseEmployee(object value)
        {
            if (value is SelValueModel sel)
            {
                Employee.Code = sel.Code;
                Employee.Name = sel.Name;
            }
        }

        // Tab4 名称CD From
        [RelayCommand]
        private void PickFrom3((object result1, object result2) value)
        {
            var (result1, result2) = value;
            var sel = result1 as SelValueModel;
            // result2 is SelectedKubun (string) if needed:
            // var kubun = result2 as string ?? "";
            if (sel == null) return;
            RangeFrom3.Code = sel.Code;
            RangeFrom3.Name = sel.Name;
        }

        // Tab4 名称CD To
        [RelayCommand]
        private void PickTo3((object result1, object result2) value)
        {
            var (result1, result2) = value;
            var sel = result1 as SelValueModel;
            if (sel == null) return;
            RangeTo3.Code = sel.Code;
            RangeTo3.Name = sel.Name;
        }

        // ---------------- Tab switching ----------------
        partial void OnSelectedTabChanged(string value) => UpdateSelectedTabIndex(value);
        private void UpdateSelectedTabIndex(string tabName) => SelectedTabIndex = ChooseTab.IndexOf(tabName);

        private BizArray BuildWrkPara_Tab1()
        {
            var p = new BizArray();
            int i = 0;

            // 1. Date Range
            p[i++] = Condition.DateFrom?.ToString("yyyyMMdd") ?? string.Empty;
            p[i++] = Condition.DateTo?.ToString("yyyyMMdd") ?? string.Empty;

            // 2. Tab1RowsA (4 rows × 4 values)
            foreach (var row in Tab1RowsA)
            {
                p[i++] = row.RangeFrom1.Code ?? ".";
                p[i++] = row.RangeTo1.Code ?? ".";
                p[i++] = row.RangeFrom2.Code ?? ".";
                p[i++] = row.RangeTo2.Code ?? ".";
            }

            // 3. Tab1RowsB (5 rows × 4 values)
            foreach (var row in Tab1RowsB)
            {
                p[i++] = row.RangeFrom1.Code ?? ".";
                p[i++] = row.RangeTo1.Code ?? ".";
                p[i++] = row.RangeFrom2.Code ?? ".";
                p[i++] = row.RangeTo2.Code ?? ".";
            }

            // 4. Employee (optional)
            p[i++] = Employee?.Code ?? ".";

            return p;
        }
        private BizArray BuildWrkPara2_Tab1(BizArray p)
        {
            var p2 = new BizArray();

            // Blank → "."
            for (int i = 0; i < p.Count; i++)
                if (string.IsNullOrEmpty(p[i]))
                    p[i] = ".";



            // WHERE clause
            string dateField = (DateFlag == 0)
                ? "A.VDATE_CREATE"
                : "A.VDATE_UPDATE";

            string sql =
                $"{dateField} between '{p[0]}' and '{p[1]}'" +
                $" and A.ブランドCD between '{p[2]}' and '{p[3]}'" +
                $" and A.アイテムCD between '{p[4]}' and '{p[5]}'" +
                $" and A.シーズンCD between '{p[6]}' and '{p[7]}'" +
                $" and A.デザイナーCD between '{p[8]}' and '{p[9]}'" +
                $" and A.展示会CD between '{p[10]}' and '{p[11]}'" +
                $" and A.素材CD between '{p[12]}' and '{p[13]}'" +
                $" and A.原産国CD between '{p[14]}' and '{p[15]}'" +
                $" and A.メーカーCD between '{p[16]}' and '{p[17]}'" +
                $" and A.名称CD01 between '{p[18]}' and '{p[19]}'" +
                $" and A.名称CD02 between '{p[20]}' and '{p[21]}'" +
                $" and A.名称CD03 between '{p[22]}' and '{p[23]}'" +
                $" and A.名称CD04 between '{p[24]}' and '{p[25]}'" +
                $" and A.名称CD05 between '{p[26]}' and '{p[27]}'" +
                $" and A.名称CD06 between '{p[28]}' and '{p[29]}'" +
                $" and A.名称CD07 between '{p[30]}' and '{p[31]}'" +
                $" and A.名称CD08 between '{p[32]}' and '{p[33]}'" +
                $" and A.名称CD09 between '{p[34]}' and '{p[35]}'" +
                $" and A.名称CD10 between '{p[36]}' and '{p[37]}'";

            // Employee filter
            if (!string.IsNullOrEmpty(p[38]) && p[38] != ".")
                sql += $" and A.入力社員CD = '{p[38]}'";

            p2[0] = sql;

            // ORDER BY
            p2[1] = BuildOrderByClause();

            return p2;
        }
        private string BuildOrderByClause()
        {
            return SelectedOrder switch
            {
                "ブランドCD" => " order by A.ブランドCD,A.商品CD",
                "アイテムCD" => " order by A.アイテムCD,A.商品CD",
                "年度" => " order by A.JANコード1,A.商品CD",
                "シーズンCD" => " order by A.シーズンCD,A.商品CD",
                "デザイナーCD" => " order by A.デザイナーCD,A.商品CD",
                "展示会CD" => " order by A.展示会CD,A.商品CD",
                "素材CD" => " order by A.素材CD,A.商品CD",
                "メーカーCD" => " order by A.メーカーCD,A.商品CD",
                "原産国CD" => " order by A.原産国CD,A.商品CD",
                "作成日" => " order by A.VDATE_CREATE DESC,A.商品CD",
                "更新日" => " order by A.VDATE_UPDATE DESC,A.商品CD",
                _ => " order by A.商品CD" // default
            };
        }


        [RelayCommand]
        private async Task DoPrintAsync()
        {
            try
            {
                ClientLib.CursorToWait();

                // -----------------------------
                // 1. Build Search Params
                // -----------------------------
                var wrk_para = BuildWrkPara_Tab1();
                var wrk_para2 = BuildWrkPara2_Tab1(wrk_para);

                // -----------------------------
                // 2. Execute CVNET Print API
                // -----------------------------
                var dt = await Task.Run(() =>
                    AppData.ClassCvnet.OnQueryPrintShohin(wrk_para2.ToArray(), 1)
                );
                string pdfPath = dt.Rows[0][0].ToString(); 
                // -----------------------------
                // 3. Handle Spool (PDF)
                // -----------------------------
                if (Condition.SelectedPrint.ToString() == "スプール")
                {
                    string url = AppData.Http.URLroot + pdfPath + "/data.pdf";

                    bool ready = await GlobalFunc.WaitForPdfAsync(url, TimeSpan.FromSeconds(30));
                    if (!ready)
                    {
                        ClientLib.MessageBoxError(this, "PDF生成に時間がかかりすぎています。\n条件を絞ってください。");
                        return;
                    }

                    ClientLib.CursorToNormal();

                    var win = new WebpdfView();
                    if (win.DataContext is WebpdfViewModel vm)
                        vm.Pdfdata = url;

                    ClientLib.ShowDialogView(win, this);
                    return;
                }

                // -----------------------------
                // 4. Handle CSV Output
                // -----------------------------
                if (Condition.SelectedPrint.ToString() == "CSV")
                {
                    string datapath = AppData.Http.URLroot + pdfPath + "/data.txt";
                    string headpath = AppData.Http.URLroot + pdfPath + "/d_sql.txt";

                    bool ready = await GlobalFunc.WaitForPdfAsync(datapath, TimeSpan.FromSeconds(30));
                    if (!ready)
                    {
                        ClientLib.MessageBoxError(this, "Data生成に時間がかかりすぎています。\n条件を絞ってください。");
                        return;
                    }

                    ready = await GlobalFunc.WaitForPdfAsync(headpath, TimeSpan.FromSeconds(30));
                    if (!ready)
                    {
                        ClientLib.MessageBoxError(this, "Header生成に時間がかかりすぎています。\n条件を絞ってください。");
                        return;
                    }

                    var csv_para = new BizCsvDocument();
                    await csv_para.LoadFromUrlAsync(datapath, headpath);

                    csv_para.SaveCsv($"{DateTime.Now:yyyyMMdd}_得意先別売上月報");

                    ClientLib.CursorToNormal();
                    return;
                }
            }
            catch (Exception ex)
            {
                ClientLib.CursorToNormal();
                ClientLib.MessageBoxError(this, $"印刷中にエラー:\n{ex.Message}");
            }
        }


    }

}
