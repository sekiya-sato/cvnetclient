using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Models;
using CvnetClient.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg03Prn04fViewModel : BaseViewModel
    {
        [ObservableProperty]
        private MasterShain? chooseMenu;
        [ObservableProperty]
        private MasterWorkerShopMenu? selectShop = new();
        [ObservableProperty]
        private SelectConditionItem? selectedSelectDeadline;

        [ObservableProperty]
        public Dictionary<int, string>? shimebi;

        [ObservableProperty]
        BtListHelper findFromWorkerShopCd = new();
        [ObservableProperty]
        BtListHelper findToWorkerShopCd = new();

        [ObservableProperty]
        //private string mstName = "移動倉庫";   // default
        private string mstName = "請求";   // default
        [ObservableProperty]
        public string? parameter1 ="";   // default

        public bool IsOutputUnitVoucher { get; set; }      // 伝票単位
        public bool IsOutputUnitProduct { get; set; }       // 商品CD単位
        public bool IsOutputTypeParent { get; set; }      // 親のみ
        public bool IsOutputTypeSubdetail { get; set; }       // 子明細
        public bool IsOutputWayPrint { get; set; }      // 印刷
        public bool IsOutputWayMail { get; set; }       // メール

        public void OnInit(object? init_para = null, string? init_flg = null)
        {
            OnInitBase(init_para, init_flg);
            ChooseMenu = new MasterShain();
            SelectShop = new MasterWorkerShopMenu();
            FindFromWorkerShopCd = new BtListHelper();
            FindToWorkerShopCd = new BtListHelper();

            Shimebi = new Dictionary<int, string>
            {
                { 1, "1" },
                { 5, "5" },
                { 10, "10" },
                { 15, "15" },
                { 20, "20" },
                { 25, "25" },
                { 30, "30" },
                { 99, "99" },
            };
            ChooseMenu.DeadLine = Shimebi.FirstOrDefault().Key;
        }

        [RelayCommand]
        private void FindFromWorkerShop(object value)
        {
            var sel = ExtractSel(value, out var p2);
            if (sel == null) return;
            {
                SelectShop.FromWorkerShopCd = sel.Code;
                SelectShop.FromWorkerShopName = sel.Name;
                SelectShop.ToWorkerShopCd = sel.Code;
                SelectShop.ToWorkerShopName = sel.Name;
            }

        }
        //public void FindFromWorkerShop(SelValueModel value)
        //{
            //Parameter1 = ChooseMenu.DeadLine?.ToString() ?? "";
            //if (value == null) return;

            //FindFromWorkerShopCd = new BtListHelper(value.Code, value.Name);

            //SelectShop.FromWorkerShopCd = value.Code;
            //SelectShop.FromWorkerShopName = value.Name;
            //SelectShop.ToWorkerShopCd = value.Code;
            //SelectShop.ToWorkerShopName = value.Name;
            //public void FindFromWorkerShop((object Result1, object Result2) value)
            //{
            //    //    parameter1 = ChooseMenu.DeadLine.ToString();
            //    Parameter1 = ChooseMenu.DeadLine?.ToString() ?? "";
            //    var (result1, result2) = value;

            //    var get_sell00 = (SelValueModel)result1;

            //    string kubun = string.Empty;
            //    if (result2 is string res2) kubun = res2;

            //    //if (get_sell00 != null && FindFromWorkerShopCd != null)
            //    //if (get_sell00 != null)
            //    //{
            //    //    if (kubun == "5")
            //    //    {
            //            SelectShop.FromWorkerShopCd = get_sell00.Code;
            //            SelectShop.FromWorkerShopName = get_sell00.Name;
            //            SelectShop.ToWorkerShopCd = get_sell00.Code;
            //            SelectShop.ToWorkerShopName = get_sell00.Name;
            //    //    }
            //    //}
            //    //    //}
        //}

        [RelayCommand]
        public void FindToWorkerShop(SelValueModel value)
        {
            if (value == null) return;

            FindToWorkerShopCd = new BtListHelper(value.Code, value.Name);

            SelectShop.ToWorkerShopCd = value.Code;
            SelectShop.ToWorkerShopName = value.Name;
        }

        //      string wrk_para = new BizArray;
        //      wrk_para[0] = new string ();
        //wrk_para[1] = new string ();
        //wrk_para[2] = new string ();
        //wrk_para[3] = new string ();
        //wrk_para[4] = new string ();

        public partial class MasterShain : ObservableObject
        {
            [ObservableProperty]
            private int? deadLine;
            [ObservableProperty]
            private string? deadLineString;
        }

        public partial class MasterWorkerShopMenu : ObservableObject
        {
            [ObservableProperty]
            private string? fromWorkerShopCd;
            [ObservableProperty]
            private string? fromWorkerShopName;
            [ObservableProperty]
            private string? toWorkerShopCd;
            [ObservableProperty]
            private string? toWorkerShopName;
        }

        partial void OnSelectedSelectDeadlineChanged(SelectConditionItem? value)
        {
            if (value == null) return;
        }

        private static SelValueModel? ExtractSel(object value, out string param2Str)
        {
            param2Str = "";
            // tuple?
            if (value is ValueTuple<object, object> t)
            {
                var s = t.Item1 as SelValueModel;
                if (t.Item2 is string s2) param2Str = s2;
                return s;
            }
            // plain
            return value as SelValueModel;
        }

        public class SelectConditionItem
        {
            public string Code { get; set; } = "";
            public string Name { get; set; } = "";

            public SelectConditionItem(string code, string name)
            {
                Code = code;
                Name = name;
            }
        }

    }
}
