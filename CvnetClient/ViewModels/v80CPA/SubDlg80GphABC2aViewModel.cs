using CommunityToolkit.Mvvm.ComponentModel;
using CvnetClient.Models;
using Microsoft.VisualBasic;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Input;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg80GphABC2aViewModel : BaseViewModel
    {
        public PlotModel AbcModel { get; }
        public PlotController Controller { get; }

        public ObservableCollection<AbcRow> AbcTable { get; } = new();

        [ObservableProperty]
        private string selectedInfo;

        string sql = "select b.*"
			+ " ,decode(総計,0,0,round(合計/総計*100,2)) 構成比"
			+ " ,decode(総計,0,0,round(累計/総計*100,2)) 累計比"
			+ ",'20221001' 期間01,'20221020' 期間02"
			+ ",'ブランド名' 集計01,'アイテム名' 集計02"
			+ " ,'全店' 店舗"
			+ " from (select a.*,sum(合計) over () 総計,sum(合計) over (order by rownum) 累計"
			+ " from ("			
			+ "select a.ブランドCD, nvl((select 名称 from hc$master_meisho where 名称区分='BRD' and 名称CD=ブランドCD),'') ブランド名" +
            ",a.アイテムCD,nvl((select 名称 from hc$master_meisho where 名称区分='ITM' and 名称CD=アイテムCD),'') アイテム名,a.数量,a.合計 from " +
            "(select b.ブランドCD,b.アイテムCD " +
            ",sum(DECODE(TRUNC(取引区分/10),1,1,2,-1,3,-1,0)*a.数量) 数量" +
            ",sum(DECODE(TRUNC(取引区分/10),1,1,2,-1,3,-1,0)*a.金額) 合計,max(商品名) 商品名 from " +
            " hc$tran_tori1 a,hc$master_shohin b,hc$tran_tori0 c where a.商品cd = b.商品cd" +
            " and c.伝票処理区分 = 1 " +
            " and c.在庫計上日 between '20221001' and '20221020' and " +
            " a.ヘッダNO = c.SEQ_NO and " +
            " A.在庫計上日 = c.在庫計上日" +
            " group by (b.ブランドCD,b.アイテムCD)) a " +
            " order by a.合計 desc"			
			+ " ) a) b ";

        public SubDlg80GphABC2aViewModel()
        {
            AbcModel = new PlotModel { Title = "ABC Analysis" };

            var dt = AppData.Http?.AspxSqlQuery(sql); // DataTable dari DB

            if (dt == null || dt.Rows.Count == 0)
                return;

            // Ambil kolum 金額 dari DB
            var values = dt.AsEnumerable()
                           .Select(r => Convert.ToDouble(r["合計"]))
                           .ToList();

            double total = values.Sum();
            double running = 0;
            var cumulative = values.Select(v => { running += v; return running / total * 100.0; }).ToList();

            double aThreshold = 70;
            double bThreshold = 90;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var row = dt.Rows[i];
                string category;
                if (cumulative[i] <= aThreshold) category = "A";
                else if (cumulative[i] <= bThreshold) category = "B";
                else category = "C";

                AbcTable.Add(new AbcRow
                {
                    ブランドCD = row["ブランドCD"].ToString(),
                    アイテムCD = row["アイテムCD"].ToString(),
                    ブランド名 = row["ブランド名"].ToString(),
                    アイテム名 = row["アイテム名"].ToString(),
                    数量 = Convert.ToInt32(row["数量"]),
                    金額 = Convert.ToDouble(row["合計"]),
                    構成比 = Convert.ToDouble(row["構成比"]),
                    累計比 = Convert.ToDouble(row["累計比"]),
                    Category = category
                });
            }

            // ---- GRAPH (guna data DB juga) ----
            var rectBars = new RectangleBarSeries { Title = "Sales Value" };
            for (int i = 0; i < values.Count; i++)
            {
                double left = i - 0.4;
                double right = i + 0.4;
                double top = values[i];

                OxyColor color = OxyColors.Orange;
                if (cumulative[i] > aThreshold && cumulative[i] <= bThreshold) color = OxyColors.Purple;
                else if (cumulative[i] > bThreshold) color = OxyColors.Green;

                rectBars.Items.Add(new RectangleBarItem(left, 0, right, top) { Color = color });
            }

            var lineSeries = new LineSeries
            {
                Title = "Cumulative %",
                Color = OxyColors.Red,
                MarkerType = MarkerType.Circle,
                YAxisKey = "CumulativeAxis"
            };
            for (int i = 0; i < cumulative.Count; i++)
                lineSeries.Points.Add(new DataPoint(i, cumulative[i]));

            var categoryAxis = new CategoryAxis
            {
                Position = AxisPosition.Bottom,
                ItemsSource = AbcTable.Select(r => $"{r.ブランド名}-{r.アイテム名}").ToList()
            };

            AbcModel.Axes.Add(categoryAxis);
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

            Controller = new PlotController();
            Controller.UnbindAll();

            Controller.BindMouseDown(
                OxyMouseButton.Left,
                new DelegatePlotCommand<OxyMouseDownEventArgs>((view, controller, args) =>
                {
                    var hitResults = AbcModel.HitTest(new HitTestArguments(args.Position, 10));
                    var first = hitResults.FirstOrDefault();

                    if (first?.Item is RectangleBarItem bar)
                    {
                        SelectedInfo = $"Bar clicked, Value={bar.Y1}";
                    }
                    else if (first?.Item is DataPoint dp)
                    {
                        SelectedInfo = $"Point clicked, X={dp.X}, Y={dp.Y}";
                    }
                    else
                    {
                        SelectedInfo = "Nothing clicked";
                    }

                    AbcModel.InvalidatePlot(false);
                    args.Handled = true;
                }));
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

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
