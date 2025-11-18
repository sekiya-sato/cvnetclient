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
using System.DirectoryServices;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg_prn_kakumstViewModel : BaseViewModel
    {
        public ComboItem00 comboItem00 = new ComboItem00();
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
        [ObservableProperty] private string selectedOrder = "";

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

        [ObservableProperty] private ObservableCollection<string> sortOptions = new();

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
        private void OnInit(object? init_para = null, string? init_flg = null)
        {
            OnInitBase(init_para, init_flg);

            Employee = new BtListHelper();
            SelectedTab = ChooseTab[0];
            Condition.DateFrom = DateTime.Now;
            Condition.DateTo = DateTime.Now;

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

        [RelayCommand]
        private void PickFrom3((object result1, object result2) value)
        {
            var (result1, result2) = value;
            var sel = result1 as SelValueModel;
            if (sel == null) return;
            RangeFrom3.Code = sel.Code;
            RangeFrom3.Name = sel.Name;
        }

        [RelayCommand]
        private void PickTo3((object result1, object result2) value)
        {
            var (result1, result2) = value;
            var sel = result1 as SelValueModel;
            if (sel == null) return;
            RangeTo3.Code = sel.Code;
            RangeTo3.Name = sel.Name;
        }

        partial void OnSelectedTabChanged(string value)
        {

            SelectedTabIndex = ChooseTab.IndexOf(value);

            SortOptions.Clear();

            var newSortItems = value switch
            {
                "商品マスタ" => new[]
                {
                    "", "ブランドCD", "アイテムCD", "年度", "シーズンCD",
                    "デザイナーCD", "展示会CD", "素材CD", "メーカーCD",
                    "原産国CD", "作成日", "更新日"
                },

                "得意先マスタ" => new[]
                {
                    "", "得意先CD", "営業担当CD", "請求先CD", "作成日", "更新日"
                },

                "仕入先マスタ" => new[]
                {
                    "", "仕入先CD", "支払先CD", "作成日", "更新日"
                },

                "名称マスタ" => new[]
                {
                    "", "名称CD", "作成日", "更新日"
                },

                "生地付属マスタ" => new[]
                {
                    "", "生地付属CD", "仕入先CD", "作成日", "更新日"
                },

                "社員マスタ" => new[]
                {
                    "", "社員CD", "所属店舗CD", "作成日", "更新日"
                },

                _ => new[] { "" }
            };

            foreach (var item in newSortItems)
            {
                SortOptions.Add(item);
            }

            SelectedOrder = "";

            OnPropertyChanged(nameof(SelectedOrder));
        }
        private string GetDateField()
        {
            return DateFlag == 0 ? "A.VDATE_CREATE" : "A.VDATE_UPDATE";
        }

        private static string Between(string field, string from, string to)
        {
            return $" and {field} between '{from}' and '{to}'";
        }
        private BizArray BuildWrkPara_Tab1()
        {
            var p = new BizArray();
            int i = 0;

            double datefrom = ClassSatoo.DateToValue(Condition.DateFrom ?? DateTime.MinValue);
            double dateto = ClassSatoo.DateToValue(Condition.DateTo ?? DateTime.MaxValue);
            p[i++] = datefrom.ToString();   // VDATE numeric
            p[i++] = dateto.ToString();     // VDATE numeric

            foreach (var row in Tab1RowsA)
            {
                p[i++] = row.RangeFrom1.Code ?? ".";
                p[i++] = row.RangeTo1.Code ?? "ZZZZZZZZ";
                p[i++] = row.RangeFrom2.Code ?? ".";
                p[i++] = row.RangeTo2.Code ?? "ZZZZZZZZ";
            }

            foreach (var row in Tab1RowsB)
            {
                p[i++] = row.RangeFrom1.Code ?? ".";
                p[i++] = row.RangeTo1.Code ?? "ZZZZZZZZ";
                p[i++] = row.RangeFrom2.Code ?? ".";
                p[i++] = row.RangeTo2.Code ?? "ZZZZZZZZ";
            }

            p[i++] = Employee?.Code ?? ".";

            return p;
        }
        private BizArray BuildWrkPara2_Tab1(BizArray p)
        {
            var p2 = new BizArray();

            string dateField = GetDateField();
            int idx = 0;

            // Date range
            string dateFrom = p[idx++];
            string dateTo = p[idx++];

            string sql = $"{dateField} between '{dateFrom}' and '{dateTo}'";

            var fieldPairsTab1A = new (string Field1, string Field2)[]
            {
                ("A.ブランドCD",  "A.アイテムCD"),
                ("A.シーズンCD",  "A.デザイナーCD"),
                ("A.展示会CD",    "A.素材CD"),
                ("A.原産国CD",    "A.メーカーCD"),
            };

            foreach (var (field1, field2) in fieldPairsTab1A)
            {
                // field1 uses RangeFrom1/To1
                string from1 = p[idx++];
                string to1 = p[idx++];
                sql += Between(field1, from1, to1);

                // field2 uses RangeFrom2/To2
                string from2 = p[idx++];
                string to2 = p[idx++];
                sql += Between(field2, from2, to2);
            }

            // Tab1RowsB: 補足1〜10 → 名称CD01〜10 (10 fields, 5 rows × 2 ranges)
            string[] nameFields =
            {
                "A.名称CD01", "A.名称CD02",
                "A.名称CD03", "A.名称CD04",
                "A.名称CD05", "A.名称CD06",
                "A.名称CD07", "A.名称CD08",
                "A.名称CD09", "A.名称CD10",
            };

            foreach (var field in nameFields)
            {
                string from = p[idx++];
                string to = p[idx++];
                sql += Between(field, from, to);
            }

            // Employee filter (最後の 1 つ)
            string emp = p[idx++];
            if (!string.IsNullOrEmpty(emp) && emp != ".")
                sql += $" and A.入力社員CD = '{emp}'";

            p2[0] = sql;
            p2[1] = BuildOrderByClause();   // your existing method

            return p2;
        }

        private string BuildOrderByClause()
        {
            return SelectedOrder switch
            {
                null or "" => " order by A.商品CD",

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

                _ => " order by A.商品CD"
            };
        }
        private BizArray BuildWrkPara_Tab2()
        {
            var p = new BizArray();
            int i = 0;

            // 1. Date Range
            double datefrom = ClassSatoo.DateToValue(Condition.DateFrom ?? DateTime.MinValue);
            double dateto = ClassSatoo.DateToValue(Condition.DateTo ?? DateTime.MaxValue);
            p[i++] = datefrom.ToString();   // VDATE numeric
            p[i++] = dateto.ToString();     // VDATE numeric
            // 2. Tab1RowsA (4 rows × 4 values)
            foreach (var row in Tab2Rows)
            {
                p[i++] = row.RangeFrom1.Code ?? ".";
                p[i++] = row.RangeTo1.Code ?? "ZZZZZZZZ";
            }

            // 4. Employee (optional)
            p[i++] = Employee?.Code ?? ".";

            return p;
        }

        private BizArray BuildWrkPara2_Tab2(BizArray p)
        {
            var p2 = new BizArray();
            string dateField = GetDateField();
            int idx = 0;

            string dateFrom = p[idx++];
            string dateTo = p[idx++];

            string sql = $"{dateField} between '{dateFrom}' and '{dateTo}'";

            string[] fields =
            {
                "A.得意先CD",
                "A.営業担当CD",
                "A.請求先CD"
            };

            foreach (var field in fields)
            {
                string from = p[idx++];
                string to = p[idx++];
                sql += Between(field, from, to);
            }

            // Employee
            string emp = p[idx++];
            if (!string.IsNullOrEmpty(emp) && emp != ".")
                sql += $" and A.入力社員CD = '{emp}'";

            p2[0] = sql;
            p2[1] = BuildOrderByClause2();
            return p2;
        }

        private string BuildOrderByClause2()
        {
            return SelectedOrder switch
            {
                null or "" => " order by A.得意先CD",

                "得意先CD" => " order by A.得意先CD",
                "営業担当CD" => " order by A.営業担当CD,A.得意先CD",
                "請求先CD" => " order by A.請求先CD,A.得意先CD",
                "作成日" => " order by A.VDATE_CREATE DESC,A.得意先CD",
                "更新日" => " order by A.VDATE_UPDATE DESC,A.得意先CD",

                _ => " order by A.得意先CD"
            };
        }
        private BizArray BuildWrkPara_Tab3()
        {
            var p = new BizArray();
            int i = 0;

            double datefrom = ClassSatoo.DateToValue(Condition.DateFrom ?? DateTime.MinValue);
            double dateto = ClassSatoo.DateToValue(Condition.DateTo ?? DateTime.MaxValue);
            p[i++] = datefrom.ToString();   // VDATE numeric
            p[i++] = dateto.ToString();     // VDATE numeric

            foreach (var row in Tab3Rows)
            {
                p[i++] = row.RangeFrom1.Code ?? ".";
                p[i++] = row.RangeTo1.Code ?? "ZZZZZZZZ";
            }

            p[i++] = Employee?.Code ?? ".";

            return p;
        }
        private BizArray BuildWrkPara2_Tab3(BizArray p)
        {
            var p2 = new BizArray();
            string dateField = GetDateField();
            int idx = 0;

            string dateFrom = p[idx++];
            string dateTo = p[idx++];

            string sql = $"{dateField} between '{dateFrom}' and '{dateTo}'";

            string[] fields =
            {
                "A.仕入先CD",
                "A.支払先CD"
            };

            foreach (var field in fields)
            {
                string from = p[idx++];
                string to = p[idx++];
                sql += Between(field, from, to);
            }

            // Employee
            string emp = p[idx++];
            if (!string.IsNullOrEmpty(emp) && emp != ".")
                sql += $" and A.入力社員CD = '{emp}'";

            p2[0] = sql;
            p2[1] = BuildOrderByClause3();
            return p2;
        }

        private string BuildOrderByClause3()
        {
            return SelectedOrder switch
            {
                null or "" => "order by A.仕入先CD",

                "仕入先CD" => " order by A.仕入先CD",
                "支払先CD" => " order by A.支払先CD,A.仕入先CD",
                "作成日" => " order by A.VDATE_CREATE DESC,A.仕入先CD",
                "更新日" => " order by A.VDATE_UPDATE DESC,A.仕入先CD",

                _ => " order by A.仕入先CD"
            };
        }
        private BizArray BuildWrkPara_Tab4()
        {
            var p = new BizArray();
            int i = 0;

            // 1. Date Range
            double datefrom = ClassSatoo.DateToValue(Condition.DateFrom ?? DateTime.MinValue);
            double dateto = ClassSatoo.DateToValue(Condition.DateTo ?? DateTime.MaxValue);
            p[i++] = datefrom.ToString();   // VDATE numeric
            p[i++] = dateto.ToString();     // VDATE numeric

            p[i++] = SelectedKubun ?? ".";
            p[i++] = RangeFrom3.Code ?? ".";
            p[i++] = RangeTo3.Code ?? "ZZZZZZZZ";
            // 4. Employee (optional)
            p[i++] = Employee?.Code ?? ".";

            return p;
        }
        private BizArray BuildWrkPara2_Tab4(BizArray p)
        {
            var p2 = new BizArray();

            string dateField = GetDateField();
            int idx = 0;

            string dateFrom = p[idx++];
            string dateTo = p[idx++];
            string kubun = p[idx++]; // 名称区分
            string cdFrom = p[idx++];
            string cdTo = p[idx++];
            string emp = p[idx++];

            string sql =
                $"{dateField} between '{dateFrom}' and '{dateTo}'" +
                $" and A.名称区分 = '{kubun}'" +
                Between("A.名称CD", cdFrom, cdTo).Replace("and A.名称CD between", " and A.名称CD between");

            if (!string.IsNullOrEmpty(emp) && emp != ".")
                sql += $" and A.入力社員CD = '{emp}'";

            p2[0] = sql;
            p2[1] = BuildOrderByClause4();
            return p2;
        }

        private string BuildOrderByClause4()
        {
            return SelectedOrder switch
            {
                null or "" => " order by A.名称CD",

                "名称CD" => " order by A.名称CD",
                "作成日" => " order by A.VDATE_CREATE DESC,A.名称CD",
                "更新日" => " order by A.VDATE_UPDATE DESC,A.名称CD",

                _ => " order by A.名称CD"
            };
        }
        private BizArray BuildWrkPara_Tab5()
        {
            var p = new BizArray();
            int i = 0;

            // 1. Date Range
            double datefrom = ClassSatoo.DateToValue(Condition.DateFrom ?? DateTime.MinValue);
            double dateto = ClassSatoo.DateToValue(Condition.DateTo ?? DateTime.MaxValue);
            p[i++] = datefrom.ToString();   // VDATE numeric
            p[i++] = dateto.ToString();     // VDATE numeric
            // 2. Tab1RowsA (4 rows × 4 values)
            foreach (var row in Tab5Rows)
            {
                p[i++] = row.RangeFrom1.Code ?? ".";
                p[i++] = row.RangeTo1.Code ?? "ZZZZZZZZ";
            }

            // 4. Employee (optional)
            p[i++] = Employee?.Code ?? ".";

            return p;
        }
        private BizArray BuildWrkPara2_Tab5(BizArray p)
        {
            var p2 = new BizArray();
            string dateField = GetDateField();
            int idx = 0;

            string dateFrom = p[idx++];
            string dateTo = p[idx++];

            string sql = $"{dateField} between '{dateFrom}' and '{dateTo}'";

            string[] fields =
            {
                "A.商品CD",
                "A.仕入先CD"
            };

            foreach (var field in fields)
            {
                string from = p[idx++];
                string to = p[idx++];
                sql += Between(field, from, to);
            }

            // Employee
            string emp = p[idx++];
            if (!string.IsNullOrEmpty(emp) && emp != ".")
                sql += $" and A.入力社員CD = '{emp}'";

            p2[0] = sql;
            p2[1] = BuildOrderByClause5();
            return p2;
        }

        private string BuildOrderByClause5()
        {
            return SelectedOrder switch
            {
                null or "" => " order by A.商品CD,A.仕入先CD",

                "生地付属CD" => " order by A.商品CD",
                "仕入先CD" => " order by A.仕入先CD,A.商品CD",
                "作成日" => " order by A.VDATE_CREATE DESC,A.商品CD",
                "更新日" => " order by A.VDATE_UPDATE DESC,A.商品CD",

                _ => " order by A.商品CD,A.仕入先CD"
            };
        }
        private BizArray BuildWrkPara_Tab6()
        {
            var p = new BizArray();
            int i = 0;

            double datefrom = ClassSatoo.DateToValue(Condition.DateFrom ?? DateTime.MinValue);
            double dateto = ClassSatoo.DateToValue(Condition.DateTo ?? DateTime.MaxValue);
            p[i++] = datefrom.ToString();   // VDATE numeric
            p[i++] = dateto.ToString();     // VDATE numeric

            foreach (var row in Tab6RowsA)
            {
                p[i++] = row.RangeFrom1.Code ?? ".";
                p[i++] = row.RangeTo1.Code ?? "ZZZZZZZZ";
            }

            foreach (var row in Tab6RowsB)
            {
                p[i++] = row.RangeFrom1.Code ?? ".";
                p[i++] = row.RangeTo1.Code ?? "ZZZZZZZZ";
            }

            p[i++] = Employee?.Code ?? ".";

            return p;
        }
        private BizArray BuildWrkPara2_Tab6(BizArray p)
        {
            var p2 = new BizArray();
            string dateField = GetDateField();
            int idx = 0;

            string dateFrom = p[idx++];
            string dateTo = p[idx++];

            string sql = $"{dateField} between '{dateFrom}' and '{dateTo}'";

            // Tab6RowsA (社員CD, 店舗CD) + Tab6RowsB (名称CD01〜05)
            string[] fields =
            {
                "A.社員CD",
                "A.店舗CD",
                "A.名称CD01",
                "A.名称CD02",
                "A.名称CD03",
                "A.名称CD04",
                "A.名称CD05"
            };

            foreach (var field in fields)
            {
                string from = p[idx++];
                string to = p[idx++];
                sql += Between(field, from, to);
            }

            // Employee
            string emp = p[idx++];
            if (!string.IsNullOrEmpty(emp) && emp != ".")
                sql += $" and A.入力社員CD = '{emp}'";

            p2[0] = sql;
            p2[1] = BuildOrderByClause6();
            return p2;
        }

        private string BuildOrderByClause6()
        {
            return SelectedOrder switch
            {
                null or "" => " order by A.社員CD",

                "社員CD" => " order by A.社員CD",
                "所属店舗CD" => " order by A.店舗CD,A.社員CD",
                "作成日" => " order by A.VDATE_CREATE DESC,A.社員CD",
                "更新日" => " order by A.VDATE_UPDATE DESC,A.社員CD",

                _ => " order by A.社員CD"
            };
        }

        [RelayCommand]
        private async Task DoPrintAsync()
        {
            try
            {
                ClientLib.CursorToWait();

                var dt = await ExecPrintQueryAsync();
                if (dt == null || dt.Rows.Count == 0)
                    throw new Exception("印刷データが取得できませんでした。条件を見直してください。");

                // The server returns temp folder path in first cell, possibly with trailing spaces or \r\n
                string folderPath = (dt.Rows[0][0]?.ToString() ?? "").Split('\n')[0].Trim();
                if (string.IsNullOrEmpty(folderPath))
                    throw new Exception("レポート生成に失敗しました。サーバーに接続できない可能性があります。");

                string baseUrl = $"{AppData.Http.URLroot}{folderPath}";

                switch (Condition.SelectedPrint)
                {
                    case PrintType.スプール:
                        ClientLib.CursorToNormal();
                        await ShowPdfAsync($"{baseUrl}/data.pdf");
                        break;

                    case PrintType.CSV:
                        await ProcessCsvAsync(baseUrl);
                        ClientLib.CursorToNormal();
                        break;

                    default:
                        throw new Exception($"不明な印刷形式: {Condition.SelectedPrint}");
                }
            }
            catch (Exception ex)
            {
                ClientLib.CursorToNormal();
                ClientLib.MessageBoxError(this, $"印刷エラー:\n{ex.Message}");
            }
        }
        private Task<DataTable> ExecPrintQueryAsync()
        {
            return SelectedTab switch
            {
                "商品マスタ" => ExecShohinAsync(),
                "得意先マスタ" => ExecTokuiAsync(),
                "仕入先マスタ" => ExecSiireAsync(),
                "名称マスタ" => ExecMeishoAsync(),
                "生地付属マスタ" => ExecKijiAsync(),
                "社員マスタ" => ExecShainAsync(),
                _ => throw new InvalidOperationException("Unknown Tab.")
            };
        }
        private async Task<bool> ShowPdfAsync(string url)
        {
            if (!await GlobalFunc.WaitForPdfAsync(url, TimeSpan.FromSeconds(30)))
            {
                ClientLib.MessageBoxError(this,
                    "PDF generation is taking too long.\nPlease narrow your conditions.");
                return false;
            }

            var win = new WebpdfView();
            if (win.DataContext is WebpdfViewModel vm)
                vm.Pdfdata = url;

            ClientLib.ShowDialogView(win, this);
            return true;
        }
        private async Task ProcessCsvAsync(string baseUrl)
        {
            string dataUrl = $"{baseUrl}/data.txt";
            string headUrl = $"{baseUrl}/d_sql.txt";

            if (!await GlobalFunc.WaitForPdfAsync(dataUrl, TimeSpan.FromSeconds(30)))
                throw new Exception("CSV data generation is taking too long.");

            if (!await GlobalFunc.WaitForPdfAsync(headUrl, TimeSpan.FromSeconds(30)))
                throw new Exception("CSV header generation is taking too long.");

            var csvDoc = new BizCsvDocument();
            await csvDoc.LoadFromUrlAsync(dataUrl, headUrl);

            csvDoc.SaveCsv($"{DateTime.Now:yyyyMMdd}_得意先別売上月報");
        }
        private async Task<DataTable> ExecShohinAsync()
        {
            // 1. Build parameters
            var wrkPara = BuildWrkPara_Tab1();
            var wrkPara2 = BuildWrkPara2_Tab1(wrkPara);

            // 2. Execute CVNET query on a background thread
            var dt = await Task.Run(() =>
                AppData.ClassCvnet.OnQueryPrintShohin(wrkPara2.ToArray(), 1)
            );

            return dt;
        }
        private async Task<DataTable> ExecTokuiAsync()
        {
            var wrkPara = BuildWrkPara_Tab2();
            var wrkPara2 = BuildWrkPara2_Tab2(wrkPara);

            var result = await Task.Run(() =>
                AppData.ClassCvnet.OnQueryPrintTokui(wrkPara2.ToArray(), 1)
            );

            // Since it returns string (folder path), wrap it into a DataTable manually
            var dt = new DataTable();
            dt.Columns.Add("FolderPath");
            dt.Rows.Add(result);  // result is the folder path string

            return dt;
        }
        private async Task<DataTable> ExecSiireAsync()
        {
            var wrkPara = BuildWrkPara_Tab3();
            var wrkPara2 = BuildWrkPara2_Tab3(wrkPara);

            var dt = await Task.Run(() =>
                AppData.ClassCvnet.OnQueryPrintSiire(wrkPara2.ToArray(), 1)
            );

            return dt;
        }
        private async Task<DataTable> ExecMeishoAsync()
        {
            const string qfmFile = "cvnet_meisho.qfm";

            var wrkPara = BuildWrkPara_Tab4();
            var wrkPara2 = BuildWrkPara2_Tab4(wrkPara);

            string whereClause = (string)wrkPara2[0];
            string orderBy = (string)wrkPara2[1];

            string sql = $@"
        SELECT A.SEQ_NO
              ,SUBSTR(GET_VDATE(A.VDATE_CREATE),0,8)||SUBSTR(GET_VDATE(A.VDATE_CREATE),10,6) 作成日時
              ,SUBSTR(GET_VDATE(A.VDATE_UPDATE),0,8)||SUBSTR(GET_VDATE(A.VDATE_UPDATE),10,6) 更新日時
              ,A.名称区分
              ,A.名称CD
              ,A.名称
              ,A.略称
              ,A.ランク
              ,A.連番
              ,A.カナ
              ,A.POS区分
              ,(A.入力社員CD ||' '|| (SELECT B.名前 FROM HC$MASTER_SHAIN B WHERE B.社員CD=A.入力社員CD)) 最終修正者
              ,'【通常印刷】'
        FROM HC$MASTER_MEISHO A
        WHERE {whereClause}
        {orderBy}";

            var dt = await Task.Run(() =>
                AppData.Http?.AspxSqlQuery(sql, wrkPara2.ToArray(), qfmFile)
            );

            return dt; // This now returns 1 row with folder path!
        }
        private async Task<DataTable> ExecKijiAsync()
        {
            const string qfmFile = "cvnet_kiji.qfm";

            var wrkPara = BuildWrkPara_Tab5();
            var wrkPara2 = BuildWrkPara2_Tab5(wrkPara);

            string whereClause = (string)wrkPara2[0];
            string orderBy = (string)wrkPara2[1];

            string sql = $@"
        SELECT A.SEQ_NO
              ,SUBSTR(GET_VDATE(A.VDATE_CREATE),0,8)||SUBSTR(GET_VDATE(A.VDATE_CREATE),10,6) 作成日時
              ,SUBSTR(GET_VDATE(A.VDATE_UPDATE),0,8)||SUBSTR(GET_VDATE(A.VDATE_UPDATE),10,6) 更新日時
              ,A.商品CD,A.旧コード,A.略称,A.商品名,A.区分CD,A.仕入先CD,A.仕入先商品CD,A.単価,A.メモ
              ,B.仕入先名
              ,(A.入力社員CD ||' '|| (SELECT S.名前 FROM HC$MASTER_SHAIN S WHERE S.社員CD=A.入力社員CD)) 最終修正者
              ,{comboItem00.GetCaseStr("生地付属", "A.区分CD")} 区分CD名
        FROM HC$MASTER_SHKIJI A
        LEFT JOIN HC$MASTER_SIIRE B ON B.仕入先CD = A.仕入先CD
        WHERE {whereClause}
        {orderBy}";

            var dt = await Task.Run(() =>
                AppData.Http?.AspxSqlQuery(sql, wrkPara2.ToArray(), qfmFile)
            );

            return dt;
        }
        private async Task<DataTable> ExecShainAsync()
        {
            const string qfmFile = "cvnet_shain.qfm";

            var wrkPara = BuildWrkPara_Tab6();
            var wrkPara2 = BuildWrkPara2_Tab6(wrkPara);

            string whereClause = (string)wrkPara2[0];
            string orderBy = (string)wrkPara2[1];

            string sql = $@"
        SELECT A.SEQ_NO
              ,SUBSTR(GET_VDATE(A.VDATE_CREATE),0,8)||SUBSTR(GET_VDATE(A.VDATE_CREATE),10,6) 作成日時
              ,SUBSTR(GET_VDATE(A.VDATE_UPDATE),0,8)||SUBSTR(GET_VDATE(A.VDATE_UPDATE),10,6) 更新日時
              ,A.社員CD,A.名前,A.部門,A.店舗CD,A.営業FLG,A.メール,A.携帯TEL,A.備考
              ,A.役職CD,A.就業FLG,A.入社日,A.有給残,A.給与区分,A.給与支給額,A.交通費区分,A.交通費支給額,A.出力FLG,A.部課CD,A.POS区分
              ,A.名称CD01,A.名称CD02,A.名称CD03,A.名称CD04,A.名称CD05
              ,NVL((SELECT H.名称 FROM HC$MASTER_MEISHO H WHERE H.名称区分='BMN' AND H.名称CD=A.部門),'.') 部門名
              ,NVL((SELECT H.得意先名 FROM HC$MASTER_TOKUI H WHERE H.得意先CD=A.店舗CD),'.') 店舗名
              ,NVL((SELECT H.名称 FROM HC$MASTER_MEISHO H WHERE H.名称区分='YAK' AND H.名称CD=A.役職CD),'.') 役職名
              ,NVL((SELECT H.名称 FROM HC$MASTER_MEISHO H WHERE H.名称区分='BKA' AND H.名称CD=A.部課CD),'.') 部課名
              ,NVL((SELECT H.名称 FROM HC$MASTER_MEISHO H WHERE H.名称区分='E01' AND H.名称CD=A.名称CD01),'.') 分類01名
              ,NVL((SELECT H.名称 FROM HC$MASTER_MEISHO H WHERE H.名称区分='E02' AND H.名称CD=A.名称CD02),'.') 分類02名
              ,NVL((SELECT H.名称 FROM HC$MASTER_MEISHO H WHERE H.名称区分='E03' AND H.名称CD=A.名称CD03),'.') 分類03名
              ,NVL((SELECT H.名称 FROM HC$MASTER_MEISHO H WHERE H.名称区分='E04' AND H.名称CD=A.名称CD04),'.') 分類04名
              ,NVL((SELECT H.名称 FROM HC$MASTER_MEISHO H WHERE H.名称区分='E05' AND H.名称CD=A.名称CD05),'.') 分類05名
              ,(A.入力社員CD ||' '|| (SELECT B.名前 FROM HC$MASTER_SHAIN B WHERE B.社員CD=A.入力社員CD)) 最終修正者
              ,NVL((SELECT H.名称 FROM HC$MASTER_MEISHO H WHERE H.名称区分='IDX' AND H.名称CD='E01'),'.') title1
              ,NVL((SELECT H.名称 FROM HC$MASTER_MEISHO H WHERE H.名称区分='IDX' AND H.名称CD='E02'),'.') title2
              ,NVL((SELECT H.名称 FROM HC$MASTER_MEISHO H WHERE H.名称区分='IDX' AND H.名称CD='E03'),'.') title3
              ,NVL((SELECT H.名称 FROM HC$MASTER_MEISHO H WHERE H.名称区分='IDX' AND H.名称CD='E04'),'.') title4
              ,NVL((SELECT H.名称 FROM HC$MASTER_MEISHO H WHERE H.名称区分='IDX' AND H.名称CD='E05'),'.') title5
              ,CASE WHEN A.営業FLG='0' THEN '0 --' WHEN A.営業FLG='1' THEN '1 営業担当' ELSE '.' END 営業FLG名
              ,CASE WHEN A.出力FLG='0' THEN '0 通常' WHEN A.出力FLG='1' THEN '99 出力しない' ELSE '.' END 出力FLG名
              ,CASE WHEN A.POS区分='0' THEN '0 通常' WHEN A.POS区分='1' THEN '9 POSﾏｽﾀ削除指示' WHEN A.POS区分='2' THEN '10 出力しない' ELSE '.' END POS区分名
              ,CASE WHEN A.就業FLG='0' THEN '0 在職' WHEN A.就業FLG='1' THEN '1 休職' WHEN A.就業FLG='2' THEN '9 退職' ELSE '.' END 就業FLG名
              ,CASE WHEN A.給与区分='0' THEN '1 月次' WHEN A.給与区分='1' THEN '4 時給' ELSE '.' END 給与区分名
              ,CASE WHEN A.交通費区分='0' THEN '0 定額' WHEN A.交通費区分='1' THEN '1 月払' ELSE '.' END 交通費区分名
        FROM HC$MASTER_SHAIN A
        WHERE {whereClause}
        {orderBy}";

            var dt = await Task.Run(() =>
                AppData.Http?.AspxSqlQuery(sql, wrkPara2.ToArray(), qfmFile)
            );

            return dt;
        }

        partial void OnSelectedTabIndexChanged(int value)
        {
            if (value >= 0 && value < ChooseTab.Count)
                SelectedTab = ChooseTab[value];   // This line makes everything 100% safe
        }
    }

}
