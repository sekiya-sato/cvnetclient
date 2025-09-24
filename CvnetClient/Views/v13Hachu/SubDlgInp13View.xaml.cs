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
    /// Interaction logic for SubDlg13HbnhtykNewView.xaml
    /// </summary>
    public partial class SubDlg13HbnhtykNewView : Window
    {
        public SubDlg13HbnhtykNewView()
        {
            InitializeComponent();
            MyDataGrid.Columns[0].Header = "行";
        }

        private void MyDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            // contoh: kita nak letak row number ke dalam column pertama (index 0)
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}
