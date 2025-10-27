using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg23PrnDayreportViewModel : BaseViewModel
    {
        public enum PrintType { スプール, CSV}
        [ObservableProperty]
        SearchCondition? condition;
        public void OnInit() 
        {
            Condition = new SearchCondition();
            
        }
        public partial class SearchCondition : ObservableObject 
        {
            [ObservableProperty]
            private string? shopFrom;
            [ObservableProperty]
            private string? shopTo;
            [ObservableProperty]
            private string? shopFromName;
            [ObservableProperty]
            private string? shopToName;
            [ObservableProperty]
            private PrintType selectedPrint = PrintType.スプール;
            [ObservableProperty]
            private DateTime? dateFrom;
            [ObservableProperty]
            private DateTime? dateTo;
        }
    }


}
