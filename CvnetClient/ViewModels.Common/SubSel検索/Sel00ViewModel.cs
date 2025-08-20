using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Models;
using CvnetClient.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CvnetClient.ViewModels
{
    public partial class Sel00ViewModel : BaseViewModel
    {
        public event EventHandler<bool> RequestClose;

        [ObservableProperty]
        List<Sel00Model>? listSel00;

        [ObservableProperty]
        Sel00Model? selectSel00;

        [ObservableProperty]
        string _textmeisho = "";

        [RelayCommand]
        void Init()
        { 
            
        }

        [RelayCommand]
        void NextList()
        { 
        }

        [RelayCommand]
        void TopList()
        { 
        
        }

        [RelayCommand]
        void NameSearch()
        {
            
        }

        [RelayCommand]
        void DoSearch()
        {
            //confirm and close window
            RequestClose?.Invoke(this, true);
        }
    }
}
