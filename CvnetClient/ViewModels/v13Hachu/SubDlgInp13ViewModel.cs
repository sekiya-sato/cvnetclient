using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using static MaterialDesignThemes.Wpf.Theme;

namespace CvnetClient.ViewModels
{
    public partial class SubDlgInp13ViewModel : BaseViewModel
    {
        [ObservableProperty]
        SearchModel? editSearch;
        [ObservableProperty]
        public Dictionary<string, string>? comboListToriHikiFrom;
        [ObservableProperty]
        public Dictionary<string, string>? comboListToriHikiTo;
        [ObservableProperty]
        ObservableCollection<OrderList>? listOrder;
        [ObservableProperty]
        OrderHaeder? selectedOrderHeader;
        [ObservableProperty]
        ObservableCollection<OrderDetail>? selectedOrderDetail;

        [RelayCommand]
        void Init() 
        {
            EditSearch = new SearchModel
            {
                DenpyoNo1 = "0",
                DenpyoNo2 = "9999999999",
                NohinbiFrom = "2025/09/14",
                NohinbiTo = "2099/12/31",
                ToriKubunFrom = "00",
                ToriKubunTo = "99",
                Kanren1From = "0",
                Kanren1To = "9999999999999",
                Kanren2From = "0",
                Kanren2To = "9999999999999",
                SupplierFrom = "0",
                SupplierTo = "99999999",
                WareFrom = "0",
                WareTo = "99999999",
                ProductFrom = ".",
                ProductTo = "zzzzzzzzzzzzzzzzzzzz",
                UserFrom = "0",
                UserTo = "99999999"
            };


            ComboListToriHikiFrom = new Dictionary<string, string>
            {
                {  "00", "00" },
                {  "10", "10 発注" },
                {  "11", "11 追加発注" },
                {  "15", "15 自動発注" }
            };

            ComboListToriHikiTo = new Dictionary<string, string>
            {
                {  "99", "99" },
                {  "10", "10 発注" },
                {  "11", "11 追加発注" },
                {  "15", "15 自動発注" }
            };

            var rand = new Random();
            ListOrder = new ObservableCollection<OrderList>();

            string[] torihikiList = { "10 発注", "11 追加発注", "15 自動発注" };
            string[] supplierNames = { "ABC商事", "XYZ物産", "田中株式会社", "山田商店", "Global Trading" };
            string[] wareNames = { "東京倉庫", "大阪倉庫", "名古屋倉庫", "福岡倉庫" };

            for (int i = 1; i <= 40; i++)
            {
                var supplierIndex = rand.Next(supplierNames.Length);
                var wareIndex = rand.Next(wareNames.Length);

                ListOrder.Add(new OrderList
                {
                    DenpyoNo = $"D{i:0000}",
                    Nohinbi = DateTime.Today.AddDays(-rand.Next(1, 100)).ToString("yyyy/MM/dd"),
                    Supplier = $"S{rand.Next(100, 999)}",
                    SupplierName = supplierNames[supplierIndex],
                    Ware = $"W{wareIndex + 1:00}",
                    WareName = wareNames[wareIndex],
                    Torihiki = torihikiList[rand.Next(torihikiList.Length)],
                    WeightSum = $"{rand.Next(10, 200)} kg",
                    PriceSum = $"{rand.Next(1000, 50000):N0} 円",
                    Kanren = $"K{rand.Next(1, 9999):0000}"
                });
            }
        }

        [RelayCommand]
        void DoList() { 
        
        }

        public class SearchModel 
        { 
            public string? DenpyoNo1 { get; set; }
            public string? DenpyoNo2 { get; set; }
            public string? NohinbiFrom { get; set; }
            public string? NohinbiTo { get; set; }
            public string? ToriKubunFrom { get; set; }
            public string? ToriKubunTo { get; set; }
            public string? Kanren1From { get; set; }
            public string? Kanren1To { get; set; }
            public string? Kanren2From { get;set; }
            public string? Kanren2To { get; set; }
            public string? Tenyuryoku1 { get; set; }
            public string? Tenyuryoku2 { get; set; }
            public string? SupplierFrom {  get; set; }
            public string? SupplierTo { get; set; }
            public string? WareFrom { get; set; }
            public string? WareTo { get;set; }
            public string? ProductFrom {  get; set; }
            public string? ProductTo { get; set; }
            public string? UserFrom { get; set; }
            public string? UserTo { get; set; }
        }

        public class OrderList { 
            public string? DenpyoNo { get; set; }
            public string? Nohinbi { get; set; }
            public string? Supplier { get;set; }
            public string? SupplierName { get; set; }
            public string? Ware { get; set; }
            public string? WareName { get; set; }
            public string? Torihiki { get; set; }
            public string? WeightSum { get; set; }
            public string? PriceSum { get; set; }
            public string? Kanren { get; set; }
        }

        public class OrderHaeder
        {
            public string? DenpyoNo { get; set; }
            public string? Hachubi { get; set; }
            public string? Nohinbi { get; set; }
            public string? ToriKubun { get; set; }
            public string? Kanren1 { get; set; }
            public string? Kanren2 { get; set; }
            public string? Tenyuryoku { get; set; }
            public string? Supplier { get; set; }
            public string? Ware { get; set; }
            public string? User { get; set; }
            public string? Biko { get; set; }
        }

        public class OrderDetail 
        { 
        
        }
    }
}