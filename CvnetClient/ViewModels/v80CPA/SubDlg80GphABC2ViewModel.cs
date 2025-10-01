using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg80GphABC2ViewModel : BaseViewModel
    {
        [ObservableProperty]
        Search? listSearch;

        [RelayCommand]
        void Init()
        {
            ListSearch = new Search
            {
                KikanFrom = DateTime.Now,
                KikanTo = DateTime.Now.AddYears(99),
                TenpoFlg = "0",
                Tenpo = "",
                Shukei1 = "0",
                Shukei2 = "0",
                Agroup = "70.0%",
                Bgroup = "90.0%",
                Cgroup = "90.0%",
                Joken = ""
            };
        }

        public class Search
        {
            public DateTime? KikanFrom { get; set; }
            public DateTime? KikanTo { get; set; }
            public string? TenpoFlg { get; set; }
            public string? Tenpo { get; set; }
            public string? Shukei1 { get; set; }
            public string? Shukei2 { get; set; }
            public string? Agroup { get; set; }
            public string? Bgroup { get; set; }
            public string? Cgroup { get; set; }
            public string? Joken { get; set; }
        }
    }
}
