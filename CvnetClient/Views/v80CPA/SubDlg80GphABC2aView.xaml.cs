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
            if (DataContext is SubDlg80GphABC2aViewModel vm)
            {
                // Ambil posisi mouse dalam plot
                var plotView = (OxyPlot.Wpf.PlotView)sender;
                var screenPoint = new OxyPlot.ScreenPoint(e.GetPosition(plotView).X, e.GetPosition(plotView).Y);

                // Buat hit test
                var hitResults = vm.AbcModel.HitTest(new HitTestArguments(screenPoint, 10));
                var first = hitResults.FirstOrDefault();

                if (first?.Item is RectangleBarItem bar)
                    vm.ShowDetailFromView($"DoubleClick Bar Value={bar.Y1}");
                else if (first?.Item is DataPoint dp)
                    vm.ShowDetailFromView($"DoubleClick Point X={dp.X}, Y={dp.Y}");
                else
                    vm.ShowDetailFromView("Nothing selected");
            }
        }
    }
}
