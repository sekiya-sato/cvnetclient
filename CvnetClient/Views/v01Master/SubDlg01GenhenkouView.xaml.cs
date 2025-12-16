using CvnetClient.ViewModels;
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
using static MaterialDesignThemes.Wpf.Theme;

namespace CvnetClient.Views
{
    /// <summary>
    /// Interaction logic for SubDlg01GenhenkouView.xaml
    /// </summary>
    public partial class SubDlg01GenhenkouView : Window
    {
        public SubDlg01GenhenkouView()
        {
            InitializeComponent();
        }

        //private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        //{
        //    // Get the index
        //    int index = e.Row.GetIndex() + 1;

        //    // Find the TextBlock in the first column
        //    if (e.Row.Item is object dataItem)
        //    {
        //        //var textBlock =
        //        //    ListDetail.Columns[0].GetCellContent(e.Row) as TextBlock;

        //        //if (textBlock != null)
        //        //{
        //        //    textBlock.Text = index.ToString();
        //        //}
        //        var grid = ListDetail.Columns[0].GetCellContent(e.Row) as Grid;
        //        if (grid != null)
        //        {
        //            var tb = grid.Children.OfType<TextBlock>().FirstOrDefault();
        //            if (tb != null)
        //                tb.Text = (e.Row.GetIndex() + 1).ToString();
        //        }

        //    }
        //}

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is SubDlg01GenhenkouViewModel vm &&
                vm.DoDetailListCommand.CanExecute(null))
            {
                vm.DoDetailListCommand.Execute(null);
            }
        }

    }
}
