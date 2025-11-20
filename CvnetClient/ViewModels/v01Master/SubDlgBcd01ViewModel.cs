using CommunityToolkit.Mvvm.Input;
using CvnetClient.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CvnetClient.ViewModels
{
    public partial class SubDlgBcd01ViewModel : BaseViewModel
    {
        public SubDlgBcd01ViewModel(string[] para) 
        { 

        }
        [RelayCommand]
        public void SearchJan() 
        {
            var vm = new SubDlg01Jan00NewViewModel();
            var window = new SubDlg01Jan00NewView { DataContext = vm };
            window.ShowDialog();
        }
    }
}
