using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Models;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg80gphSelViewModel : BaseViewModel
    {
        [ObservableProperty]
        string[] wrk_para = null;

        bool is1stInit = true;
        List<Sel00Model>? listSelOri;

        [ObservableProperty]
        List<Sel00Model>? listSel00;

        [ObservableProperty]
        Sel00Model? selectSel00;
         
        void Init()
        {

        }

        [RelayCommand]
        void PrevList()
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
        
        }

        [RelayCommand]
        void DoExit()
        { 
        
        }
    }
}
