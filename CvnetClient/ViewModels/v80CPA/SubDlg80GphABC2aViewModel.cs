using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Models;
using CvnetClient.Utils;
using Microsoft.VisualBasic;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Windows.Input;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg80GphABC2aViewModel : BaseViewModel
    {
        public PlotModel AbcModel { get; set; }
        public PlotController Controller { get; }
        [ObservableProperty]
        private AbcRow selectedRow;
        public ObservableCollection<AbcRow> AbcTable { get; } = new();
        public DataTable MainTbl { get; set; }
        public DataTable ClickedTbl { get; set; }
        private DataTable OriginalTbl { get; set; }
        [ObservableProperty]
        private string selectedInfo;
        [ObservableProperty]
        private int selectedIndex = 0;
        [ObservableProperty]
        public SearchCond searchCondList;
        [ObservableProperty]
        public string[] total = new string[2];
        [ObservableProperty]
        public string[] totalMei = new string[2];
        [ObservableProperty]
        public string[] smei = new string[2];
        [ObservableProperty]
        public string dateFrom;
        [ObservableProperty]
        public string dateTo;
        private bool IsDrilled { get; set; } = false;

        public SubDlg80GphABC2aViewModel(SearchCond cond,BizArray v_para,int cnt)
        {

            AbcModel = new PlotModel { Title = "ABC分析" };

            string v_sokbn = "";
            if (AppData.ClassCvnet.config.shokbnflg == 1)
            {
                v_sokbn = " AND b.商品区分flg=0 ";
            }

            SearchCondList = cond;
            DateFrom = SearchCondList.date1;
            DateTo = SearchCondList.date2;
            SearchCondList.date1 = SearchCondList.date1.Substring(0, 4) + "/" + SearchCondList.date1.Substring(4, 2) + "/" + SearchCondList.date1.Substring(6, 2);
            SearchCondList.date2 = SearchCondList.date2.Substring(0, 4) + "/" + SearchCondList.date2.Substring(4, 2) + "/" + SearchCondList.date2.Substring(6, 2);
            GetShukei(SearchCondList.shukei1.Split()[0],0);
            GetShukei(SearchCondList.shukei2.Split()[0], 1);

            string sql = "select a." + Total[0] + "," + TotalMei[0] + ",a." + Total[1] + "," + TotalMei[1] + " ,a.数量,a.合計 from " +
            "(select b." + Total[0] + ",b." + Total[1] +
            ",sum(DECODE(TRUNC(取引区分/10),1,1,2,-1,3,-1,0)*a.数量) 数量" +
            ",sum(DECODE(TRUNC(取引区分/10),1,1,2,-1,3,-1,0)*a.金額) 合計,max(商品名) 商品名 from " +
            " hc$tran_tori1 a,hc$master_shohin b,hc$tran_tori0 c where a.商品cd = b.商品cd " +
            " and c.伝票処理区分 = 1 " +
            SearchCondList.condstr +
            " and c.在庫計上日 between '" + DateFrom + "' and '" + DateTo + "' and " +
             ((SearchCondList.tenpo　== "全店")?" ":"c.取引先cd1='" + SearchCondList.tenpo + "' and ") +
            " a.ヘッダNO = c.SEQ_NO and" +
            " A.在庫計上日 = c.在庫計上日 " +
             v_sokbn +
            " group by (b." + Total[0] + ",b." + Total[1] + ")) a " +
            " order by a.合計 desc";

            sql = "select b.*"
            + " ,decode(総計,0,0,round(合計/総計*100,2)) 構成比"
            + " ,decode(総計,0,0,round(累計/総計*100,2)) 累計比"
            + ",' ' d101,' ' d102"
            + ",' ' d201,' ' d202"
            + ",' ' d301,' ' d302"
            + ",' ' 期間01,' ' 期間02"
            + ",'" + Smei[0] + "' 集計01,'" + Smei[1] + "' 集計02"
            + " ,'" + SearchCondList.tenpo.ToString() + "' 店舗"
            + " from (select a.*,sum(合計) over () 総計,sum(合計) over (order by rownum) 累計"
            + " from ("
            + sql
            + " ) a) b ";

            var dt = AppData.Http?.AspxSqlQuery(sql, v_para.ToArray());
            

            if (dt == null || dt.Rows.Count == 0)
                return;


            Controller = new PlotController();
            Controller.UnbindAll();

            TblGphGeneration(dt);
            MainTbl = dt;
        }

        partial void OnSelectedRowChanged(AbcRow? value)
        {
            if (value != null)
            {
                var idx = AbcTable.IndexOf(
                    AbcTable.FirstOrDefault(r => r.ブランド名 == value.ブランド名 &&
                                                 r.アイテム名 == value.アイテム名));
                if (idx >= 0)
                    SelectedIndex = idx;
            }
        }

        partial void OnSelectedIndexChanged(int value)
        {
            if (value >= 0 && value < AbcTable.Count)
                SelectedRow = AbcTable[value];
        }

        void GetShukei(string total_para, int i)
        {
            switch (total_para)
            {
                case "BRD":
                    Total[i] = "ブランドCD";
                    break;
                case "ITM":
                    Total[i] = "アイテムCD";
                    break;
                case "MKR":
                    Total[i] = "メーカーCD";
                    break;
                case "DZN":
                    Total[i] = "デザイナーCD";
                    break;
                case "SZI":
                    Total[i] = "素材CD";
                    break;
                case "SZN":
                    Total[i] = "シーズンCD";
                    break;
                case "GEN":
                    Total[i] = "原産国CD";
                    break;
                case "TNJ":
                    Total[i] = "展示会CD";
                    break;
                case "BN0":
                    Total[i] = "大分類CD";
                    break;
                case "BN1":
                    Total[i] = "中分類CD";
                    break;
                case "BN2":
                    Total[i] = "小分類CD";
                    break;
                case "SIK":
                    Total[i] = "仕入区分";
                    break;
                case "-1":
                    Total[i] = "''";
                    break;
                case "B00":
                    Total[i] = "商品CD";
                    break;
                default:
                    Total[i] = "名称CD" + total_para.Substring(1);
                    break;
            }
            if (total_para == "B00")
            {
                TotalMei[i] = "商品名";
                Smei[i] = "商品";
            }
            else if (total_para == "-1")
            {
                TotalMei[i] = "";
            }
            else 
            {
                var tm = "";
                if (i == 0)
                {
                    tm = AppData.ClassCvnet.GetStringValue(SearchCondList.shukei1);
                }
                else {
                    tm = AppData.ClassCvnet.GetStringValue(SearchCondList.shukei2);
                }
                    Smei[i] = tm;
                tm = tm + "名";
                TotalMei[i] = " nvl((select 名称 from hc$master_meisho where 名称区分='" + total_para + "' and 名称CD=" + Total[i] + "),'') " + tm ;
            }
        }

        public void TblGphGeneration(DataTable dt) {
            var totals = dt.AsEnumerable()
                   .Select(r => Convert.ToDouble(r["合計"]))
                   .ToList();

            double grandTotal = totals.Sum();

            // 1) Compute 構成比 for each row:
            //    jika dt sudah ada kolom 構成比, gunakan nilainya (asumsi dalam persen, contoh 12.34)
            //    kalau tak ada, compute = 合計 / grandTotal * 100
            var compositions = new List<double>();
            foreach (DataRow row in dt.Rows)
            {
                double comp = 0;
                if (dt.Columns.Contains("構成比"))
                {
                    // safe parse (kolom mungkin string/number)
                    double.TryParse(row["構成比"]?.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out comp);
                }

                if (grandTotal == 0 || comp == 0)
                {
                    // hitung dari 合計
                    var val = Convert.ToDouble(row["合計"]);
                    comp = (grandTotal == 0) ? 0 : Math.Round(val / grandTotal * 100.0, 2);
                }

                compositions.Add(comp);
            }

            // 2) Compute 累計比 by summing 構成比
            var cumulativePercent = new List<double>();
            double runComp = 0;
            foreach (var c in compositions)
            {
                runComp += c;
                cumulativePercent.Add(Math.Round(runComp, 2)); // keep 2 decimal
            }

            // 3) Thresholds (baca dari SearchCondList yang user ada)
            double.TryParse(SearchCondList?.Agroup ?? "0", NumberStyles.Any, CultureInfo.InvariantCulture, out var aThreshold);
            double.TryParse(SearchCondList?.Bgroup ?? "0", NumberStyles.Any, CultureInfo.InvariantCulture, out var bThreshold);

            // Reset any ruikei accumulator if you use it elsewhere
            if (SearchCondList != null)
                SearchCondList.ruikei = 0;

            // 4) Build AbcTable rows and prepare series
            var rectBars = new RectangleBarSeries { Title = "Sales Value" };
            var lineSeries = new LineSeries
            {
                Title = "Cumulative %",
                Color = OxyColors.Black,
                MarkerType = MarkerType.Circle,
                YAxisKey = "CumulativeAxis"
            };

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var row = dt.Rows[i];

                double amount = Convert.ToDouble(row["合計"]);
                double comp = compositions[i];
                double cumPct = cumulativePercent[i];

                // Tentukan category berdasarkan 累計比 (bukan berdasarkan nilai mentah)
                string category;
                if (cumPct <= aThreshold) category = "A";
                else if (cumPct <= bThreshold) category = "B";
                else category = "C";

                // Update ruikei (jika masih mahu menyimpan sum 合計)
                if (SearchCondList != null)
                    SearchCondList.ruikei += amount;

                // Tambah ke data model (dipakai untuk table binding / drill mapping)
                AbcTable.Add(new AbcRow
                {
                    ブランドCD = row.Table.Columns.Contains("ブランドCD") ? row["ブランドCD"].ToString() : "",
                    アイテムCD = row.Table.Columns.Contains("アイテムCD") ? row["アイテムCD"].ToString() : "",
                    ブランド名 = row.Table.Columns.Contains("ブランド名") ? row["ブランド名"].ToString() : "",
                    アイテム名 = row.Table.Columns.Contains("アイテム名") ? row["アイテム名"].ToString() : "",
                    数量 = row.Table.Columns.Contains("数量") ? Convert.ToInt32(row["数量"]) : 0,
                    金額 = amount,
                    構成比 = comp,
                    累計比 = cumPct,
                    Category = category
                });

                // RectangleBarItem expects (left, y0, right, y1)
                double left = i - 0.4;
                double right = i + 0.4;
                double top = amount;

                // Pick color by category
                OxyColor color = OxyColors.Brown;
                if (cumPct > aThreshold && cumPct <= bThreshold) color = OxyColors.CadetBlue;
                else if (cumPct > bThreshold) color = OxyColors.BurlyWood;

                rectBars.Items.Add(new RectangleBarItem(left, 0, right, top) { Color = color });

                // Line series use 累計比 on right axis
                lineSeries.Points.Add(new DataPoint(i, cumPct));
            }

            // 5) Axes & add series
            AbcModel.Axes.Add(new CategoryAxis
            {
                Position = AxisPosition.Bottom,
                ItemsSource = AbcTable.Select(r => $"{r.ブランド名}-{r.アイテム名}").ToList()
            });

            AbcModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "金額" });

            AbcModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Right,
                Title = "累計比率",
                Minimum = 0,
                Maximum = 100,
                Key = "CumulativeAxis"
            });

            AbcModel.Series.Add(rectBars);
            AbcModel.Series.Add(lineSeries);

            Controller.BindMouseDown(
                OxyMouseButton.Left,
                new DelegatePlotCommand<OxyMouseDownEventArgs>((view, controller, args) =>
                {
                    var hitResults = AbcModel.HitTest(new HitTestArguments(args.Position, 10));
                    var first = hitResults.FirstOrDefault();

                    if (first?.Item is RectangleBarItem bar)
                    {
                        int index = rectBars.Items.IndexOf(bar);
                        if (index >= 0 && index < AbcTable.Count)
                        {
                            SelectedIndex = index + 1; // +1 sebab manusia biasa kira dari 1
                            SelectedRow = AbcTable[index];
                            SelectedInfo = $"Bar clicked: {SelectedRow.ブランド名}-{SelectedRow.アイテム名}";
                        }
                    }
                    else if (first?.Item is DataPoint dp)
                    {
                        int index = (int)dp.X;
                        if (index >= 0 && index < AbcTable.Count)
                        {
                            SelectedIndex = index + 1;
                            SelectedRow = AbcTable[index];
                            SelectedInfo = $"Point clicked: {SelectedRow.ブランド名}-{SelectedRow.アイテム名}, 累計比={dp.Y}";
                        }
                    }
                    else
                    {
                        SelectedRow = null;
                        SelectedInfo = "Nothing clicked";
                    }

                    AbcModel.InvalidatePlot(false);
                    args.Handled = true;
                }));
        }

        public void DrillDownByCategory(string category)
        {
            if (IsDrilled)
            {
                ShowDetailFromView("これ以上分解できません");
                return;
            }

            // Filter ikut category
            var filteredRows = MainTbl.AsEnumerable()
                .Where(r => AbcTable.Any(a =>
                    a.ブランドCD == r["ブランドCD"].ToString() &&
                    a.アイテムCD == r["アイテムCD"].ToString() &&
                    a.Category == category));

            if (!filteredRows.Any())
            {
                ShowDetailFromView("No data found for this category");
                return;
            }

            // Buat DataTable baru dari filter
            ClickedTbl = filteredRows.CopyToDataTable();

            // Reset cumulative ruikei supaya kira fresh
            SearchCondList.ruikei = 0;

            // Generate balik table + graph
            AbcTable.Clear();
            AbcModel.Series.Clear();
            AbcModel.Axes.Clear();
            TblGphGeneration(ClickedTbl);

            OnPropertyChanged(nameof(AbcModel));

            // Full refresh
            AbcModel.InvalidatePlot(true);
            IsDrilled = true;
        }

        [RelayCommand]
        public void ResetToOriginal()
        {
            if (IsDrilled == false) return;


            AbcTable.Clear();
            AbcModel.Series.Clear();
            AbcModel.Axes.Clear();
            TblGphGeneration(MainTbl);
            
            ClickedTbl = null;
            OnPropertyChanged(nameof(ClickedTbl));
            OnPropertyChanged(nameof(AbcModel));
            AbcModel.InvalidatePlot(true);
            IsDrilled = false;   // balik ke asal
        }

        public void ShowDetailFromView(string message)
        {
            System.Windows.MessageBox.Show(message, "Detail",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Information);
        }

        public class AbcRow
        {
            public string ブランドCD {  get; set; }
            public string アイテムCD { get; set; }
            public string ブランド名 { get; set; }
            public string アイテム名 { get; set; }
            public int 数量 { get; set; }
            public double 金額 { get; set; }
            public double 構成比 { get; set; }
            public double 累計比 { get; set; }
            public string Category { get; set; }  // "A" / "B" / "C"
        }

        public class SearchCond
        {
            public string condstr { get; set; }
            public string date1 { get; set; }
            public string date2 { get; set; }
            public string tenpo { get; set; }
            public string shukei1 { get; set; }
            public string shukei2 { get; set; }
            public string Agroup { get; set; }
            public string Bgroup { get; set; }
            public string Cgroup { get; set; }
            public double ruikei { get; set; }
            public double ruikei2 { get; set; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
