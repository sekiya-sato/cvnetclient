using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace CvnetClient.ViewModels
{
    public class SubDlg80GphABC2aViewModel : INotifyPropertyChanged
    {
        public PlotModel AbcModel { get; }
        public PlotController Controller { get; }

        private string _selectedInfo = "Click a bar/point...";
        public string SelectedInfo
        {
            get => _selectedInfo;
            set { _selectedInfo = value; OnPropertyChanged(nameof(SelectedInfo)); }
        }

        public SubDlg80GphABC2aViewModel()
        {
            AbcModel = new PlotModel { Title = "ABC Analysis" };

            // Dummy data
            var items = new[] { "Item1", "Item2", "Item3", "Item4", "Item5" };
            var values = new[] { 500.0, 300.0, 150.0, 100.0, 50.0 };

            double total = values.Sum();
            double running = 0;
            var cumulative = values.Select(v =>
            {
                running += v;
                return running / total * 100.0;
            }).ToArray();

            double aThreshold = 70;
            double bThreshold = 90;

            // Bars
            var rectBars = new RectangleBarSeries { Title = "Sales Value" };
            for (int i = 0; i < values.Length; i++)
            {
                double left = i - 0.4;
                double right = i + 0.4;
                double top = values[i];

                OxyColor color = OxyColors.Orange;
                if (cumulative[i] > aThreshold && cumulative[i] <= bThreshold) color = OxyColors.Purple;
                else if (cumulative[i] > bThreshold) color = OxyColors.Green;

                rectBars.Items.Add(new RectangleBarItem(left, 0, right, top) { Color = color });
            }

            // Cumulative line
            var lineSeries = new LineSeries
            {
                Title = "Cumulative %",
                Color = OxyColors.Red,
                MarkerType = MarkerType.Circle,
                YAxisKey = "CumulativeAxis"
            };
            for (int i = 0; i < cumulative.Length; i++)
                lineSeries.Points.Add(new DataPoint(i, cumulative[i]));

            // Axes
            AbcModel.Axes.Add(new CategoryAxis { Position = AxisPosition.Bottom, ItemsSource = items });
            AbcModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Sales Value" });
            AbcModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Right,
                Title = "Cumulative %",
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
                    // Debug → tengok clickcount
                    SelectedInfo = $"ClickCount = {args.ClickCount}";

                    var hitResults = AbcModel.HitTest(new HitTestArguments(args.Position, 10));
                    var first = hitResults.FirstOrDefault();

                    //if (args.ClickCount == 2)
                    //{
                    //    if (first?.Item is RectangleBarItem bar)
                    //        ShowDetailWindow($"Detail for Bar Value={bar.Y1}");
                    //    else if (first?.Item is DataPoint dp)
                    //        ShowDetailWindow($"Detail for Point X={dp.X}, Y={dp.Y}");
                    //    else
                    //        ShowDetailWindow("Nothing selected");
                    //}
                    //else
                    if (args.ClickCount == 1) // single click
                    {
                        if (first?.Item is RectangleBarItem bar)
                            SelectedInfo = $"Bar clicked, Value={bar.Y1}";
                        else if (first?.Item is DataPoint dp)
                            SelectedInfo = $"Point clicked, X={dp.X}, Y={dp.Y}";
                        else
                            SelectedInfo = "Nothing clicked";
                    }

                    AbcModel.InvalidatePlot(false);
                    args.Handled = true;
                }));
        }

        //private void ShowDetailWindow(string message)
        //{
        //    System.Windows.MessageBox.Show(message, "Detail",
        //        System.Windows.MessageBoxButton.OK,
        //        System.Windows.MessageBoxImage.Information);
        //}

        public void ShowDetailFromView(string message)
        {
            System.Windows.MessageBox.Show(message, "Detail",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Information);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
