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

namespace CvnetClient.Views
{
    /// <summary>
    /// Interaction logic for SubDlg00AutoskView.xaml
    /// </summary>
    public partial class SubDlg00AutoskView : Window
    {
        public static readonly DependencyProperty DataSourceProperty =
        DependencyProperty.Register(nameof(DataSource),
        typeof(ListFlexData),
        typeof(ListFlexView),
        new PropertyMetadata(null));
        public ListFlexData DataSource
        {
            get => (ListFlexData)GetValue(DataSourceProperty);
            set => SetValue(DataSourceProperty, value);
        }
        private void DeleteRow_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is ListFlexItem row)
            {
                DataSource.ListFlexSource.Remove(row);

                // re-number No after delete
                for (int i = 0; i < DataSource.ListFlexSource.Count; i++)
                {
                    DataSource.ListFlexSource[i].No = i + 1;
                }
            }
        }
    }
}
