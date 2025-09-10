using CommunityToolkit.Mvvm.ComponentModel;
using CvnetClient.ViewModels.Component;
using System.Collections.ObjectModel;

namespace CvnetClient.ViewModels
{
    public partial class SubDlgSelShoViewModel : BaseViewModel
    {
        [ObservableProperty]
        ObservableCollection<ListFlexItem>? listFlex = new ObservableCollection<ListFlexItem>();

        [ObservableProperty]
        ListFlexConfig? listConfig = new ListFlexConfig();

        [ObservableProperty]
        string? whereClaus;
    }
}
