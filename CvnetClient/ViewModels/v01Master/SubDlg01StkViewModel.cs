using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Class;
using CvnetClient.Models;
using CvnetClient.Utils;
using CvnetClient.Views;
using System;
using System.Text;
using System.Threading.Tasks;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg01StkViewModel : BaseViewModel
    {
        private BizArray para;
        private BizArray sv_cat;
        private BizArray wrk_jodai;

        [ObservableProperty] private ListFlexData listFlexData = new();
        [ObservableProperty] private StkSearchOpt searchOpt = new();

        // -------------------- 得意先／仕入先 --------------------
        [ObservableProperty] private int selCustomerSupplier = 0; // 0=得意先, 1=仕入先
        [ObservableProperty] private string labelRange = "得意先 範囲";

        // -------------------- 範囲 (FROM / TO) --------------------
        [ObservableProperty] private MasterCodeName? rangeFrom = new();
        [ObservableProperty] private MasterCodeName? rangeTo = new();

        // ✅ Visibility binding helper properties
        public bool IsTokuiSelected => SelCustomerSupplier == 0;
        public bool IsSiiresakiSelected => SelCustomerSupplier == 1;

        // ----------------------------------------------------------
        // Radio selection handler (switch 得意先/仕入先)
        // ----------------------------------------------------------
        partial void OnSelCustomerSupplierChanged(int value)
        {
            LabelRange = (value == 0 ? "得意先" : "仕入先") + " 範囲";

            // ✅ Reset both ranges when switching master
            RangeFrom = new MasterCodeName { Code = "0", Name = string.Empty };
            RangeTo = new MasterCodeName { Code = "99999999", Name = string.Empty };

            OnPropertyChanged(nameof(IsTokuiSelected));
            OnPropertyChanged(nameof(IsSiiresakiSelected));
        }


        // ----------------------------------------------------------
        // 初期化（ListFlex を BizD 互換で動かすための設定を含む）
        // ----------------------------------------------------------
        public void OnInit(string[] init_para = null)
        {
            para = new BizArray();
            sv_cat = new BizArray();
            wrk_jodai = new BizArray();

            if (init_para != null)
                para = new BizArray(init_para);

            SearchOpt = new StkSearchOpt();

            // ✅ Default range setup (BizD style)
            RangeFrom = new MasterCodeName { Code = "0", Name = string.Empty };
            RangeTo = new MasterCodeName { Code = "99999999", Name = string.Empty };

            // ✅ BizD的な初期化：フレックスの開始バインド位置や初期CSVなど
            //   - range の :1, :2 を使うので、フレックスは :3 から開始
            ListFlexData.ListConfig = new ListFlexConfig()
            {
                init_csv = new System.Collections.Generic.List<CsvItem>(),
                cnt_start = 3,   // ここ重要：range で2個使った後、フレックスは3から
                flag = 0         // 必要に応じて（BizDのフラグに準ずる）
            };
        }

        // ----------------------------------------------------------
        // 🔍 Range-Select Buttons
        // ----------------------------------------------------------
        [RelayCommand]
        public void SelRangeFrom(object value)
        {
            if (value is SelValueModel get_sel00)
            {
                RangeFrom ??= new();
                RangeFrom.Code = get_sel00.Code;
                RangeFrom.Name = get_sel00.Name;

                // ✅ Auto-copy logic (BizD behavior)
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

        // ----------------------------------------------------------
        // 🖨️ Execute (印刷)  — ListFlex を含む動的クエリで印刷
        // ----------------------------------------------------------
        [RelayCommand]
        public async Task DoPrintAsync()
        {
            // 1️⃣ Confirm print
            if (!ClientLib.MessageBox(this, "印刷しますか？")) return;
            ClientLib.CursorToWait();

            // 2️⃣ クエリ生成（SQL + バインドパラメータ）
            var ret = OnQueryList();  // (sql, v_para)
            string sql_query = ret.Item1;
            var bindParams = ret.Item2?.ToArray() ?? Array.Empty<string>();

            // 3️⃣ QFM は BizD と同じ単一ファイル
            string fileName = "cvnet_takku.qfm";

            // 4️⃣ 実行 & PDF 生成要求
            var resp = AppData.Http!.AspxSqlQueryCsv(sql_query, bindParams, fileName);
            var lines = resp.Split('\n');

            // 5️⃣ データ無し
            if (lines.Length < 2 || lines[1] == "0")
            {
                ClientLib.MessageBoxError(this, "PDFデータがありません");
                ClientLib.CursorToNormal();
                return;
            }

            // 6️⃣ PDF URL 作成
            string pdfPath = lines[0];
            string url = AppData.Http.URLroot + pdfPath + "/data.pdf";

            // 7️⃣ 生成待ち
            bool ready = await Utils.GlobalFunc.WaitForPdfAsync(url, TimeSpan.FromSeconds(30));
            if (!ready)
            {
                ClientLib.MessageBoxError(this, "PDF生成に時間がかかりすぎています。\n条件を絞ってください。");
                ClientLib.CursorToNormal();
                return;
            }

            // 8️⃣ 表示
            var win = new WebpdfView();
            if (win.DataContext is WebpdfViewModel vm)
                vm.Pdfdata = url;

            ClientLib.CursorToNormal();
            ClientLib.ShowDialogView(win, this);
        }

        // ----------------------------------------------------------
        // 戻る
        // ----------------------------------------------------------
        [RelayCommand]
        private void DoExit()
        {
            ClientLib.ExitDialogResult(this, true);
        }

        // ----------------------------------------------------------
        // マスター切替用の設定（柔軟なSQL生成）
        // ----------------------------------------------------------
        private sealed class MasterConfig
        {
            public string Table { get; init; } = "";
            public string CodeCol { get; init; } = "";
            public string NameCol { get; init; } = "";
            public string OrderByCol { get; init; } = "";
        }

        private MasterConfig GetMasterConfig(int mode)
        {
            return (mode == 0)
                ? new MasterConfig
                {
                    Table = "HC$MASTER_TOKUI",
                    CodeCol = "得意先CD",
                    NameCol = "得意先名",
                    OrderByCol = "得意先CD"
                }
                : new MasterConfig
                {
                    Table = "HC$MASTER_SIIRE",
                    CodeCol = "仕入先CD",
                    NameCol = "仕入先名",
                    OrderByCol = "仕入先CD"
                };
        }

        // ----------------------------------------------------------
        // BizD の ListFlexView.GetQueryStr2 相当（C#版）
        //  - ここでは ListFlexData.GetQueryStr を使って v_sql を生成
        //  - v_para を組み立ててバインドも拡張
        // ----------------------------------------------------------
        private Tuple<string, BizArray> OnQueryList()
        {
            var cfg = GetMasterConfig(SelCustomerSupplier);
            var sb = new StringBuilder();
            var v_para = new BizArray();

            // --- 範囲 (bind :1, :2)
            v_para[0] = RangeFrom?.Code ?? "0";
            v_para[1] = RangeTo?.Code ?? "99999999";

            // --- 追加列（文字列リテラル／印刷日付・担当者名など）
            string lit1 = "印刷日付".Replace("'", "''");
            string lit2 = "担当者名".Replace("'", "''");

            // --- SELECT 本体
            sb.Append("select ")
              .Append(cfg.CodeCol).Append(',')
              .Append(cfg.NameCol).Append(',')
              .Append("郵便番号,住所1,住所2,住所3,宛名名称1,宛名名称2")
              .Append(", '").Append(lit1).Append("'")
              .Append(", '").Append(lit2).Append("' ")
              .Append("from ").Append(cfg.Table).Append(' ')
              .Append("where ").Append(cfg.CodeCol).Append(" between :1 and :2");

            // --- フレックスの条件生成（:3 以降のパラメータが追加される）
            //     第3引数は AND/OR の結合方法（0=AND, 1=OR）
            int cntStart = 3; // range 2つの後ろから
            sb.Append(ListFlexData.GetQueryStr(v_para, cntStart, SearchOpt.SelJoinCond));

            // --- 法人CD対応（BizD 相当）
            string houjin_str = AppData.ClassCvnet.GetQueryStrHoujin();
            if (!string.IsNullOrWhiteSpace(houjin_str))
                sb.Append(' ').Append(houjin_str);

            // --- ORDER
            sb.Append(' ').Append("order by ").Append(cfg.OrderByCol);

            string sql = sb.ToString();
            System.Diagnostics.Debug.WriteLine($"sql_query={sql}");
            return Tuple.Create(sql, v_para);
        }
    }

    // ----------------------------------------------------------
    // 検索オプション
    // ----------------------------------------------------------
    public partial class StkSearchOpt : ObservableObject
    {
        [ObservableProperty] private int selJoinCond = 0;     // 0=AND, 1=OR
        [ObservableProperty] private int addressCustomer = 0; // 宛名修飾文字（今はクエリ非連動、将来拡張用）
    }

    // ----------------------------------------------------------
    // 範囲 (コード＋名称)
    // ----------------------------------------------------------
    public partial class MasterCodeName : ObservableObject
    {
        [ObservableProperty] private string? code;
        [ObservableProperty] private string? name;
    }


}
