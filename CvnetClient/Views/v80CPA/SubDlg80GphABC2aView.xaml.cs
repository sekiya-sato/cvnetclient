using CvnetClient.ViewModels;
using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CvnetClient.Views
{
    /// <summary>
    /// Interaction logic for SubDlg80GphABC2aView.xaml
    /// </summary>
    public partial class SubDlg80GphABC2aView : Window
    {
        public SubDlg80GphABC2aView()
        {
            InitializeComponent();
        }

        private void PlotView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!(DataContext is SubDlg80GphABC2aViewModel vm)) return;

            var plotView = (OxyPlot.Wpf.PlotView)sender;
            var pos = e.GetPosition(plotView);

            if (vm.AbcModel == null) return;

            // Buat hit test
            var args = new HitTestArguments(new ScreenPoint(pos.X, pos.Y), 10);
            var hitResults = vm.AbcModel.HitTest(args);
            var first = hitResults?.FirstOrDefault();

            if (first == null)
            {
                vm.ShowDetailFromView("Nothing selected");
                return;
            }

            // Kalau klik RectangleBarSeries (bar chart)
            if (first.Element is RectangleBarSeries rectSeries && first.Item is RectangleBarItem rectItem)
            {
                int index = rectSeries.Items.IndexOf(rectItem);
                if (index >= 0 && index < vm.AbcTable.Count)
                {
                    // Drill sekali sahaja
                    vm.DrillDownByCategory(vm.AbcTable[index].Category);
                    return;
                }
            }

            // Kalau klik line / scatter (DataPoint)
            if (first.Item is DataPoint dp)
            {
                vm.ShowDetailFromView($"DoubleClick Point X={dp.X}, Y={dp.Y}");
                return;
            }

            vm.ShowDetailFromView("Hit test found element but item type not handled");
        }

    }
}
