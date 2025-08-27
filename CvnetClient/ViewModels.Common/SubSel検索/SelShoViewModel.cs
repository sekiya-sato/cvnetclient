using CommunityToolkit.Mvvm.ComponentModel;
using CvnetClient.ViewModels.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CvnetClient.ViewModels
{
    public partial class SelShoViewModel : BaseViewModel
    {
        [ObservableProperty]
        List<ListFlexItem>? listFlex = new List<ListFlexItem>();

        [ObservableProperty]
        ListFlexConfig? listConfig = new ListFlexConfig();

        [ObservableProperty]
        string? whereClaus;
    }
}
