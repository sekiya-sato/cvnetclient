using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Class;
using CvnetClient.Models;
using CvnetClient.Utils;
using CvnetClient.Views;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CvnetClient.ViewModels
{
    public enum TenpoType { 全店, 店別 }
    public partial class SubDlg80GphABC2ViewModel : BaseViewModel
    {
        [ObservableProperty]
        Search? listSearch;
        [ObservableProperty]
        public List<KeyValuePair<string, string>>? comboListShukei1;
        [ObservableProperty]
        public List<KeyValuePair<string, string>>? comboListShukei2;
        [ObservableProperty]
        ListFlexData listFlexData = new ListFlexData();
        [ObservableProperty]
        private TenpoType selectedTenpo = TenpoType.全店;
        public bool IsTenpoEnabled => SelectedTenpo == TenpoType.店別;

        [RelayCommand]
        void OnInit()
        {
            List<CsvItem> def = null;

            DataTable dt = AppData.ClassCvnet.GetMeiList_Shohin();
            var list = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("B00", "B00 商品CD") 
            };
            list.AddRange(
                dt.AsEnumerable()
                  .Select(row => new KeyValuePair<string, string>(
                      row[0].ToString(),
                      row[0].ToString()
                  ))
            );

            ComboListShukei1 = list;
            ComboListShukei2 = list;
            ListSearch = new Search
            {
                KikanFrom = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1),
                KikanTo = DateOnly.FromDateTime(DateTime.Now),
                Tenpo = "",
                Agroup = 70.0,
                Shukei1 = ComboListShukei1.FirstOrDefault(x => x.Key == "BRD ﾌﾞﾗﾝﾄﾞ").Key,
                Shukei2 = ComboListShukei2.FirstOrDefault(x => x.Key == "ITM ｱｲﾃﾑ").Key,
                Bgroup = 90.0,
                Cgroup = "90.0%以上",
                Joken = ""
            };

            ListFlexData.ListConfig = new ListFlexConfig()
            {
                init_csv = def ?? new List<CsvItem>(),
                flag = 1
            };
        }

        [RelayCommand]
        private void RadioChanged(string? tenpo)
        {
            if (Enum.TryParse(tenpo, out TenpoType parsed))
            {
                SelectedTenpo = parsed;
            }
        }
        partial void OnSelectedTenpoChanged(TenpoType value)
        {
            OnPropertyChanged(nameof(IsTenpoEnabled));

            if (value == TenpoType.全店 && ListSearch != null)
            {
                ListSearch.Tenpo = string.Empty; // auto clear
            }
        }

        [RelayCommand]
        void DoExec() 
        {
            try
            {
                var type = Type.GetType(typeof(Views.SubDlg80GphABC2aView).FullName );

                var existing = Application.Current.Windows
            .OfType<Window>()
            .FirstOrDefault(w => w.GetType() == type);

                if (existing != null)
                {
                    if (existing.WindowState == WindowState.Minimized)
                        existing.WindowState = WindowState.Normal;

                    existing.Activate();
                    return;
                }
                else
                {
                    var v_para = new BizArray();
                    int cnt = 1;
                    ListSearch.Joken = ListFlexData.GetQueryStr(v_para, cnt,null,"b.");
                    var cond = new SubDlg80GphABC2aViewModel.SearchCond
                    {
                        date1 = ListSearch?.KikanFrom?.ToString("yyyyMMdd"),
                        date2 = ListSearch?.KikanTo?.ToString("yyyyMMdd"),
                        tenpo = SelectedTenpo.ToString() == "全店" ? "全店" : ListSearch?.Tenpo,
                        shukei1 = ListSearch.Shukei1!,
                        shukei2 = ListSearch?.Shukei2!,
                        Agroup = ListSearch?.Agroup?.ToString("00.0"),
                        Bgroup = ListSearch?.Bgroup?.ToString("00.0"),
                        Cgroup = ListSearch?.Cgroup,
                        condstr = ListSearch?.Joken
                    };

                    var vm = new SubDlg80GphABC2aViewModel(cond,v_para,cnt);
                    var window = new SubDlg80GphABC2aView { DataContext = vm };
                    window.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading window: {ex.Message}");
            }
        }
        public partial class Search : ObservableObject
        {
            [ObservableProperty]
            private DateOnly? kikanFrom;

            [ObservableProperty]
            private DateOnly? kikanTo;

            [ObservableProperty]
            private string? tenpoFlg;

            [ObservableProperty]
            private string? tenpo;

            [ObservableProperty]
            private string? shukei1;

            [ObservableProperty]
            private string? shukei2;

            [ObservableProperty]
            private double? agroup;

            [ObservableProperty]
            private double? bgroup;

            [ObservableProperty]
            private string? cgroup;

            [ObservableProperty]
            private string? joken;
        }
    }
}
