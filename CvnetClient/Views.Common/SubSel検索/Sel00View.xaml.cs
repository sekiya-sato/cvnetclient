using CvnetClient.ViewModels;
using System.Windows; 

namespace CvnetClient.Views
{
    /// <summary>
    /// Interaction logic for Sel00View.xaml
    /// </summary>
    public partial class Sel00View : Window
    {
        public Sel00View()
        {
            InitializeComponent();
        }

        public static object ShowPopup()
        { 
            var view = new Sel00View();
            var vm = view.DataContext as Sel00ViewModel;

            //Ensure vm can close the window 
            vm.RequestClose += (s, e) => view.DialogResult = e;

            bool? result = view.ShowDialog();
            return result == true ? vm.SelectSel00 : null;
        }
    }
}
