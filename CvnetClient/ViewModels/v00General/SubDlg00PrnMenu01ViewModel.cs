using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Models;
using CvnetClient.Utils;
using CvnetClient.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg00PrnMenu01ViewModel : BaseViewModel
    {
        [ObservableProperty]
        private MasterWorkerShopMenu? selectWorkerShop = new();
        [ObservableProperty]
        public Dictionary<string, string>? selectedCondition;
        //    public ObservableCollection<SelectConditionItem> SelectConditionList { get; }
        //= new ObservableCollection<SelectConditionItem>
        //    {
        //        new SelectConditionItem { Code = "0", Name = "社員CD" },
        //        new SelectConditionItem { Code = "1", Name = "店舗CD" }
        //    };
        public ObservableCollection<SelectConditionItem> SelectConditionList { get; }
        = new ObservableCollection<SelectConditionItem>();
        public ObservableCollection<string> SortConditionList { get; }
    = new ObservableCollection<string>();

        [ObservableProperty]
        private SelectConditionItem? selectedSelectCondition;
        [ObservableProperty]
        private string? selectedSortCondition;
        [ObservableProperty]
        BtListHelper findFromWorkerShopCd = new();
        [ObservableProperty]
        BtListHelper findToWorkerShopCd = new();
        [ObservableProperty]
        private string mstName = "担当";   // default

        // ラベル文字列（社員CD範囲 or 店舗CD範囲）
        [ObservableProperty]
        private string rangeCdLabel = "社員CD範囲";   // default

        public bool IsAllEmployees { get; set; }      // 全社員
        public bool IsWorkingOnly { get; set; }       // 就業者のみ

        public void OnInit(object? init_para = null, string? init_flg = null)
        {
            OnInitBase(init_para, init_flg);

            SelectConditionList.Clear();

            SelectConditionList.Add(new SelectConditionItem("0", "社員CD"));
            SelectConditionList.Add(new SelectConditionItem("1", "店舗CD"));
            // default selection:
            SelectedSelectCondition = SelectConditionList[0];
            // Optional initial default sort options
            SortConditionList.Add("社員CD順");
            SortConditionList.Add("氏名順");
        }

        private void ClearWorkerShopBindings()
        {
            if (SelectWorkerShop == null)
            {
                // ensure it’s never null, just in case OnMstNameChanged fires earlier
                SelectWorkerShop = new MasterWorkerShopMenu();
            }

            SelectWorkerShop.FromWorkerShopCd = string.Empty;
            SelectWorkerShop.FromWorkerShopName = string.Empty;
            SelectWorkerShop.ToWorkerShopCd = string.Empty;
            SelectWorkerShop.ToWorkerShopName = string.Empty;

            // clear the selected values for the list buttons:
            FindFromWorkerShopCd = null;
            FindToWorkerShopCd = null;
        }

        [RelayCommand]
        public void FindFromWorkerShop(SelValueModel value)
        {
            if (value == null) return;

            FindFromWorkerShopCd = new BtListHelper(value.Code, value.Name);

            SelectWorkerShop.FromWorkerShopCd = value.Code;
            SelectWorkerShop.FromWorkerShopName = value.Name;
            SelectWorkerShop.ToWorkerShopCd = value.Code;
            SelectWorkerShop.ToWorkerShopName = value.Name;
        }

        [RelayCommand]
        public void FindToWorkerShop(SelValueModel value)
        {
            if (value == null) return;

            FindToWorkerShopCd = new BtListHelper(value.Code, value.Name);

            SelectWorkerShop.ToWorkerShopCd = value.Code;
            SelectWorkerShop.ToWorkerShopName = value.Name;
        }

        // This method is auto-called whenever SelectedSelectCondition changes
        partial void OnSelectedSelectConditionChanged(SelectConditionItem? value)
        {
            if (value?.Code == "1")          // 店舗CD
                RangeCdLabel = "店舗CD範囲";
            else
                RangeCdLabel = "社員CD範囲";

            ClearWorkerShopBindings();
            // >>> Update sort conditions <<<
            UpdateSortConditions(value?.Code);
            // >>> Update search item <<<
            UpdateMstName(value?.Code);
        }


        // for 印刷 button
        [RelayCommand]
        async Task DoPrintAsync()
        {
            var wrk_para = BuildWrkPara();   // string[]
            string sqlQuery = "_mshain01";
            string printsql = "";
            string qfmFile = "cvnet00prn_menu01.qfm";
            // Confirmation
            if (!ClientLib.MessageBox(this, "印刷しますか？")) return;
            ClientLib.CursorToWait();

            // Execute with parameters
            var ret = AppData.Http!.AspxSqlQueryCsv("_mshain01", wrk_para.ToArray(), "cvnet00prn_menu01.qfm", 0);
            if (ret.Split('\n').Length < 2)
            {
                ClientLib.MessageBoxError(this, "PDFデータがありません");
                return;
            }
            var ret1 = ret.Split('\n');
            var url = AppData.Http.URLroot + ret1[0] + "/data.pdf";
            await Task.Delay(1500); // PDF生成待ち
            var win = new WebpdfView();
            var vm = win.DataContext as WebpdfViewModel;
            if (vm == null) return;
            vm.Pdfdata = url;
            ClientLib.CursorToNormal();
            ClientLib.ShowDialogView(win, this);
        }

        //private string[] BuildWrkPara()
        //{
        //    var v_para = new string[8];

        //    // [0] 範囲FROM
        //    v_para[0] = SelectWorkerShop?.FromWorkerShopCd?.Trim() ?? "";
        //    // [1] 範囲TO
        //    v_para[1] = SelectWorkerShop?.ToWorkerShopCd?.Trim() ?? "";
        //    // [2] ソート条件
        //    //v_para[2] = SelectedSortCondition?.Trim() ?? "";
        //    v_para[2] = "0";
        //    // [3] 選択条件 (0 or 1)
        //    v_para[3] = SelectedSelectCondition?.Code?.Trim() ?? "";
        //    // [4] 就業者のみ (1) / 全社員 (0)
        //    v_para[4] = IsWorkingOnly ? "1" : "0";

        //    // [5], [6], [7] MUST BE "0"
        //    v_para[5] = "10";
        //    v_para[6] = "";
        //    v_para[7] = "";

        //    return v_para;
        //}

        private BizArray BuildWrkPara()
        {
            var v_para = new BizArray();

            // [0] 範囲FROM
            v_para[0] = SelectWorkerShop?.FromWorkerShopCd?.Trim() ?? "";
            // [1] 範囲TO
            v_para[1] = SelectWorkerShop?.ToWorkerShopCd?.Trim() ?? "";
            // [2] ソート条件
            //v_para[2] = SelectedSortCondition?.Trim() ?? "";
            v_para[2] = "0";
            // [3] 選択条件 (0 or 1)
            v_para[3] = SelectedSelectCondition?.Code?.Trim() ?? "";
            // [4] 就業者のみ (1) / 全社員 (0)
            v_para[4] = IsWorkingOnly ? "1" : "0";

            // [5], [6], [7] MUST BE "0"
            v_para[5] = "400";
            v_para[6] = "";
            v_para[7] = "";

            return v_para;
        }

        private void UpdateSortConditions(string? code)
        {
            SortConditionList.Clear();

            if (code == "1")   // 店舗CD
            {
                SortConditionList.Add("店舗CD");
                SortConditionList.Add("略称");
                SortConditionList.Add("カナ");
            }
            else               // 社員CD
            {
                SortConditionList.Add("社員CD");
                SortConditionList.Add("名前");
                SortConditionList.Add("店舗CD");
            }

            // Reset selected item
            SelectedSortCondition = SortConditionList.FirstOrDefault();
        }

        private void UpdateMstName(string? code)
        {
            if (code == "1")        // 店舗CD
                MstName = "移動倉庫";
            else                    // 社員CD
                MstName = "担当";   // 
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
