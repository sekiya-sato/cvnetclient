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
    /// Interaction logic for SubDlg01UsrView.xaml
    /// </summary>
    public partial class SubDlg01UsrView : Window
    {
        public SubDlg01UsrView()
        {
            InitializeComponent();
        }

        private void Border_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Copy;
            else
                e.Effects = DragDropEffects.None;
        }

        private void Border_Drop(object sender, DragEventArgs e)
        {
            if (DataContext is SubDlg01UsrViewModel vm)
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                    if (files.Length > 0)
                        vm.OnImageDropped(files[0]);
                }
            }
        }
    }
}
