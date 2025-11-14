using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Models;
using CvnetClient.Utils;
using CvnetClient.Views;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg00UsrViewModel : BaseViewModel
    {
        [ObservableProperty]
        ObservableCollection<MasterWorker>? searchWorker;

        [ObservableProperty]
        public Dictionary<int, string>? felicaInitialization;

        public void OnInit()
        {

        }
    }
}
