using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Models;
using CvnetClient.Utils;
using CvnetClient.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Windows;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg01GenhenkouViewModel : BaseViewModel, INotifyPropertyChanged
    {
        [ObservableProperty]
        SearchCondition? condition;

        [ObservableProperty]
        ObservableCollection<MasterGenkaHenkou>? listGenka;

        [ObservableProperty]
        ObservableCollection<MasterGenkaHenkou>? listDetailGenka;

        [ObservableProperty]
        MasterGenkaHenkou? selectedProduct;

        [ObservableProperty]
        MasterGenkaHenkou? editGenka;

        [ObservableProperty]
        private int selectedTabIndex;

        public bool IsChecked { get; set; }

        [ObservableProperty]
        private int receiptNo;

        [ObservableProperty]
        private string inputStaff;

        [ObservableProperty]
        public DateTime changeDate;

        [ObservableProperty]
        public decimal markupRateChange;

        private bool _canNext;
        public bool CanNext
        {
            get => _canNext;
            set => SetProperty(ref _canNext, value);
        }

        private bool _canBack;
        public bool CanBack
        {
            get => _canBack;
            set => SetProperty(ref _canBack, value);
        }
        public void OnInit(object? init_para = null, string? init_flg = null)
        {
            OnInitBase(init_para, init_flg);
            Condition = new SearchCondition();

            Condition.StaffFrom = new BtListHelper(".", "");
            Condition.StaffTo = new BtListHelper("99999999", "");
            Condition.ProductFrom = new BtListHelper(".", "");
            Condition.ProductTo = new BtListHelper("ZZZZZZZZZZZZZZ", "");

            Condition.RecptFrom = "0";
            Condition.RecptTo = "99999999";

            Condition.DateFrom = new DateTime(1901, 1, 1);
            Condition.DateTo = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            EditGenka = new MasterGenkaHenkou();
            changeDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            receiptNo = 0;
            inputStaff = ".";


            DoList();

        }

        //private int _selectedTabIndex;
        //public int SelectedTabIndex
        //{
        //    get {  return _selectedTabIndex; } 
        //    set
        //    {
        //        if (_selectedTabIndex != value)
        //        {
        //            _selectedTabIndex = value;
        //            OnPropertyChanged(nameof(SelectedTabIndex));
        //        }
        //    }
        //}

        [RelayCommand]
        public void SelProduct1(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Condition != null)
            {
                Condition.ProductFrom = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }

        [RelayCommand]
        public void SelProduct2(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Condition != null)
            {
                Condition.ProductTo = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }

        [RelayCommand]
        public void SelStaff1(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Condition != null)
            {
                Condition.StaffFrom = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }

        [RelayCommand]
        public void SelStaff(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Condition != null)
            {
                Condition.StaffTo = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }

        string onQuerySql = """
       select * from (
        select A.伝票NO,MAX(A.VDATE_UPDATE) 修正日
        ,MAX(変更日) 変更日,max(A.担当者) 担当者
        ,MAX(C.名前) 社員名,MIN(B.仕入区分) 仕入区分 
        from HC$MASTER_SHOHIN_GENKA_HENKOU A
        ,HC$MASTER_SIIRE B
        ,HC$MASTER_SHAIN C
        ,HC$MASTER_SHOHIN D  
        where A.商品CD=D.商品CD 
        and D.メーカーCD=B.仕入先CD(+) 
        and A.担当者=C.社員CD(+) 
        and A.伝票NO between :1 and :2 
        {0}
        and A.商品CD between :5 and :6 
        and A.担当者 between :7 and :8 
        and A.伝票NO>=:9
        {1}
        GROUP BY A.伝票NO 
        order by A.伝票NO {2}
      ) where rownum<={3}
""";

        [RelayCommand]
        void DoList()
        {
            subList(0, "<=", "desc");
            if (ListGenka == null || ListGenka.Count == 0)
                ClientLib.MessageBoxOk(this, "データがありません");
        }

        void subList(int slipNo, string fugo, string sort)
        {
            var parameters = new List<object>();

            string conditionChgDate = "";
            if (para[0] == "1") { conditionChgDate = "  and A.変更日 between :3 and :4 "; }
            else { conditionChgDate = " and A.変更日 between (substr(:3,0,6)||'01') and (substr(:4,0,6)||'31') "; }

            string para9str = " and A.伝票NO" + fugo + ":10";
            if (slipNo==0 || slipNo == null) para9str = "";

            parameters.Add(condition.RecptFrom);
            parameters.Add(condition.RecptTo);
            parameters.Add(condition.DateFrom.HasValue
        ? condition.DateFrom.Value.ToString("yyyyMM")
        : "190101");
            parameters.Add(condition.DateTo.HasValue
        ? condition.DateTo.Value.ToString("yyyyMM")
        : DateTime.Now.ToString("yyyyMM"));
            parameters.Add(condition.ProductFrom);
            parameters.Add(condition.ProductTo);
            parameters.Add(condition.StaffFrom);
            parameters.Add(condition.StaffTo);
            parameters.Add(condition.RecptFrom);
            if (slipNo != 0 && slipNo != null) parameters.Add(condition.RecptFrom);

            var sql = string.Format(onQuerySql, conditionChgDate, para9str, sort, AppData.maxQueryCnt+1);
            var retData = AppData.Http?.AspxSqlQuery(sql, parameters.Select(p => p.ToString()).ToArray());
            if (retData == null || retData.Rows.Count == 0)
            {
                ListGenka = new ObservableCollection<MasterGenkaHenkou>();
                CanNext = false;
                CanBack = false;
                return;
            }

            var list = (from DataRow dr in retData.Rows
                        select new MasterGenkaHenkou
                        {
                            receiptNo = Convert.ToInt32(dr["伝票NO"]),
                            editDate = dr["修正日"].ToString() ?? string.Empty,
                            changeDate = dr["変更日"].ToString() ?? string.Empty,
                            staffIncharge = dr["担当者"].ToString() ?? string.Empty,
                            staffName = dr["社員名"].ToString() ?? string.Empty,
                            wereCate = dr["仕入区分"].ToString() ?? string.Empty
                        }).OrderBy(c => c.receiptNo).ToList();

            // 🔹 Lookahead logic
            if (list.Count > AppData.maxQueryCnt)
            {
                // There is more data → enable Next
                list = list.Take(AppData.maxQueryCnt).ToList();
                CanNext = true;
            }
            else
            {
                // End reached
                CanNext = false;
            }

            Common.ConvertDotStringDel(list);
            ListGenka = new ObservableCollection<MasterGenkaHenkou>(list);
            if (ListGenka.Count > 0)
            {
                selectedProduct = ListGenka[0];
            }
        }

        [RelayCommand]
        void DoDetailList() 
        {
            if (SelectedProduct == null)
                return;
            subListDetail();
            SelectedTabIndex = 1;
        }

        void subListDetail() {

            string sql = @"select * from (
                select rownum as rn ,A.SEQ_NO,A.VDATE_CREATE,A.VDATE_UPDATE 修正日
                ,A.伝票NO
                ,A.変更日
                ,A.商品CD,B.商品名,A.行NO,B.上代
                ,A.元原価
                ,decode(B.上代,0,0,TRUNC(A.新原価/B.上代*100,2)) 掛率
                ,A.新原価
                ,B.メーカー品番 
                from HC$MASTER_SHOHIN_GENKA_HENKOU A,HC$MASTER_SHOHIN B 
                where A.商品CD=B.商品CD 
                and A.伝票NO=:1 
                order by A.商品CD,A.行NO
            )";

            string[] paraDetail = new string[1];
            paraDetail[0] = selectedProduct.receiptNo.ToString();

            var retData = AppData.Http?.AspxSqlQuery(sql, paraDetail);
            if (retData == null || retData.Rows.Count == 0)
            {
                listDetailGenka = new ObservableCollection<MasterGenkaHenkou>();
                return;
            }

            var list = (from DataRow dr in retData.Rows
                        select new MasterGenkaHenkou
                        {
                            rownum = Convert.ToInt32(dr["rn"]),
                            receiptNo = Convert.ToInt32(dr["伝票NO"]),
                            editDate = dr["修正日"].ToString() ?? string.Empty,
                            changeDate = dr["変更日"].ToString() ?? string.Empty,
                            productCD = dr["商品CD"].ToString() ?? string.Empty,
                            productName = dr["商品名"].ToString() ?? string.Empty,
                            oriCostPrice = Convert.ToInt32(dr["元原価"]),
                            newCostPrice = Convert.ToInt32(dr["新原価"]),
                            rowNumIsGenkaFLG = Convert.ToInt32(dr["行NO"]),
                            retailPrice = Convert.ToInt32(dr["上代"]),
                        }).OrderBy(c => c.receiptNo).ToList();

            receiptNo = selectedProduct.receiptNo;
            inputStaff = selectedProduct.staffIncharge;
            if (DateTime.TryParse(selectedProduct.ChangeDate, out var dt))
            {
                changeDate = dt;
            }
            else changeDate = new DateTime(1901, 01, 01);
            ListDetailGenka = new ObservableCollection<MasterGenkaHenkou>(list);

        }

        [RelayCommand]
        void NextList()
        {
            if (ListGenka == null || ListGenka.Count == 0) return;

            // Use the last row as starting point
            var last = ListGenka.Last();
            var receiptNo = last.receiptNo;

            subList(receiptNo, "<=", "desc");
            if (ListGenka == null || ListGenka.Count == 0)
                ClientLib.MessageBoxOk(this, "データがありません");
        }

        [RelayCommand]
        void BackList()
        {
            if (ListGenka == null || ListGenka.Count == 0) return;

            // Use the first row as starting point
            var first = ListGenka.First();
            var receiptNo = first.receiptNo;

            subList(receiptNo, ">=", "asc");
            if (ListGenka == null || ListGenka.Count == 0)
                ClientLib.MessageBoxOk(this, "データがありません");

        }

        [RelayCommand]
        void DoDelete()
        {
            
            if (EditGenka == null) ClientLib.MessageBox(this, "原価変更伝票を選択してください");
            if (ListGenka.Count() == 0) return;

            var v_chk = AppData.ClassCvnet.GetSime();
            DateTime Shimebi = DateTime.Now;
            DateTime ChangeDate = DateTime.Now;
            DateTime.TryParse(EditGenka.changeDate, out ChangeDate);
            if (!string.IsNullOrEmpty(v_chk)) {
                if (v_chk.Length==8) { DateTime.TryParse(v_chk, out Shimebi); }
                if (Shimebi >= ChangeDate)
                {
                    ClientLib.MessageBoxError(this, "締日(" + v_chk.Substring(0, 4) + "/" + v_chk.Substring( 4, 2) + "/" + v_chk.Substring(6, 2) + ")以前は更新できません。");
                    return;
                }
            }else
            {
                ClientLib.MessageBoxError(this, "締日見つかりません");
                return;
            }

            string[] v_para = new string[1];
            v_para[0] = EditGenka.receiptNo.ToString();
            var sql_query = ""
            + " select "
                + "max(decode(g.変更日,nvl((select max(gg.変更日) from HC$MASTER_SHOHIN_GENKA_HENKOU gg where gg.商品CD=g.商品CD),'19010101'),0,1)) 判定"
            + " from "
                + "HC$MASTER_SHOHIN_GENKA_HENKOU g"
            + " where "
                + "g.伝票NO=:1";
            var ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para);
            if (ret_csv?.Rows.Count > 0)
            {
                if (ret_csv.Rows[0][0] == "1")
                {
                    ClientLib.MessageBox(this, "最新の変更履歴ではない商品が含まれている為、削除できません。");
                    return;
                }
            }else ClientLib.MessageBox(this, "最新の変更履歴チェックエラー");

            if (!ClientLib.MessageBoxOk(this, "伝票NO=" + EditGenka.receiptNo.ToString() + "を削除してもよろしいですか？")) return;

            string[] wrk_para = new string[1];
            wrk_para[0] = EditGenka.receiptNo.ToString();
            var retcsv = AppData.Http!.AspxSqlQuery2("gen_del_sho", wrk_para,null,0);
            if (int.Parse(retcsv.Split(",")[0].ToString()) < 0)
            {
                ClientLib.MessageBoxError(this, retcsv.Split(",")[0].ToString() + "削除エラー");
                return;
            }
            else {
                if (SelectedProduct != null)
                {
                    ListGenka!.Remove(SelectedProduct);
                    var item = ListGenka.Where(c => c.receiptNo == ListGenka.Min(c => c.receiptNo)).FirstOrDefault();
                    SelectedProduct = item;
                    ClientLib.MessageBox(this, "削除しました");
                }
            }
        }

        [RelayCommand]
        void DoInsert()
        {
            if (EditGenka.receiptNo >= 0) { ClientLib.MessageBoxError(this, "既に登録されている伝票は追加できません"); }
            int errorCheck = OnCheckError();

            if (errorCheck == 0)
            {
                if (!ClientLib.MessageBoxOk(this, "追加しますか？")) return;
                OnKousin(0);
            }

            return;

        }

        [RelayCommand]
        void DoUpdate()
        {
            if (EditGenka.receiptNo == 0) { ClientLib.MessageBoxError(this, "修正伝票ではありません"); }
            int errorCheck = OnCheckError();

            if (errorCheck == 0)
            {
                if (!ClientLib.MessageBoxOk(this, "修正しますか？")) return;
                OnKousin(EditGenka.receiptNo);
            }

            return;
        }

        void OnKousin(int receiptNo) {

            var item = Common.CloneObject(EditGenka);

            //Common.ConvertDotStringAdd(item);
            if (item == null) { 
                ClientLib.MessageBoxOk(this, "登録しました");
                return;
            }
            string henkoubi = AppData.ClassSatoo.GetDateVal(changeDate, 0, "0");
            if (para[0] == "1") { henkoubi = changeDate.ToString("yyyyMMdd"); }

            string[] v_para = new string[4];
            v_para[0] = "";
            if (receiptNo != 0) { v_para[0] = receiptNo.ToString(); }
            v_para[1] = henkoubi;
            v_para[2] = AppData.ClassSatoo.SHAIN_CD;
            v_para[3] = EditGenka.productCD + "," + EditGenka.rowNumIsGenkaFLG + "," + EditGenka.oriCostPrice + "," + EditGenka.newCostPrice + "\n";

            var ret = AppData.Http!.AspxSqlQuery2("gen_ksn", v_para, null, 0);
            var details_ret = ret.Split(",");
            if (details_ret[0].ToString() != "0")
            {
                ClientLib.MessageBoxError(this, "[ " + details_ret[0].ToString() + " ] 更新エラー");

            }
            else
            {
                ClientLib.MessageBoxOk(this, "登録しました");
                DoList();
            }

            SelectedTabIndex = 0;
        }

        int OnCheckError() {
            int v_chek = 0;
            DateTime defaultDate = DateTime.MinValue;
            if (changeDate == defaultDate)
            {
                ClientLib.MessageBox(this, "変更日を入力して下さい！");
                return -1;
            }

            if (ListDetailGenka.Count==0) {
                ClientLib.MessageBox(this, "明細レコードがありません！");
                return -1;
            }

            var v_chk = AppData.ClassCvnet.GetSime();
            DateTime Shimebi = DateTime.Now;
            if (!string.IsNullOrEmpty(v_chk))
            {
                if (v_chk.Length == 8) { DateTime.TryParse(v_chk, out Shimebi); }
                else { 
                    ClientLib.MessageBoxError(this, "締日の形式が正しくありません"); 
                    return -3; 
                }
                if (Shimebi >= changeDate)
                {
                    ClientLib.MessageBoxError(this, "締日(" + v_chk.Substring(0, 4) + "/" + v_chk.Substring(4, 2) + "/" + v_chk.Substring(6, 2) + ")以前は更新できません");
                    return -4;
                }
            }
            else
            {
                ClientLib.MessageBoxError(this, "締日見つかりません");
                return -2;
            }

            bool anyChecked = ListDetailGenka.Any(x => x.isChecked);

            if (!anyChecked)
            {
                ClientLib.MessageBox(this,"選択レコードがありません！");
                return - 5;
            }

            return 0;
        }

        [RelayCommand]
        async Task DoPrintAsync()
        {
            if (!ClientLib.MessageBox(this, "印刷しますか？")) return;
            ClientLib.CursorToWait();
            if (selectedProduct == null) return;

            string sql = @"select * from (
                select A.伝票NO
                ,A.変更日
                ,A.担当者
                ,C.名前
                ,B.メーカーCD
                ,D.仕入先名
                ,'' d1,'' d2
                ,A.商品CD
                ,B.商品名
                ,A.行NO
                ,B.上代
                ,A.元原価
                ,A.新原価 
                ,DECODE(B.上代,0,0,TRUNC(A.新原価/B.上代*100,2)) 掛率
                ,B.メーカー品番 MK品番  
                from HC$MASTER_SHOHIN_GENKA_HENKOU A
                ,HC$MASTER_SHOHIN B
                ,HC$MASTER_SHAIN C
                ,HC$MASTER_SIIRE D  
                where A.商品CD=B.商品CD(+) 
                and A.担当者=C.社員CD(+) 
                and B.メーカーCD=D.仕入先CD(+) 
                and A.伝票NO=:1 
                order by A.商品CD,A.行NO
            )";
            string qfm_file = "cvnet01prn01genhen.qfm";
            if (para[0] == "1") qfm_file = "cvnet01prn01genhen_new.qfm";

            string[] paraPrint = new string[1];
            paraPrint[0] = selectedProduct.receiptNo.ToString();
            var ret = AppData.Http!.AspxSqlQueryCsv(sql, paraPrint, qfm_file);
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

        [RelayCommand]
        void AllRateChange() 
        {
            foreach(var item in listDetailGenka) {
                item.markupRate = markupRateChange;
                item.newCostPrice = (int)Math.Floor((100 - markupRateChange) * (100 * item.retailPrice));
            }
        }

        [RelayCommand]
        void AllClear() {
            receiptNo = 0;
            changeDate = DateTime.Now;
            ListDetailGenka.Clear();
        }

        [RelayCommand]
        void SelectAll()
        {
            if (ListDetailGenka == null) return;

            foreach (var item in ListDetailGenka)
            {
                item.IsChecked = true;
            }
        }

        [RelayCommand]
        void UnselectAll()
        {
            if (ListDetailGenka == null) return;

            foreach (var item in ListDetailGenka)
            {
                item.IsChecked = false;
            }
        }



        public partial class SearchCondition : ObservableObject
        {
            [ObservableProperty]
            private string? recptFrom;
            [ObservableProperty]
            private string? recptTo;
            [ObservableProperty]
            private DateTime? dateFrom;
            [ObservableProperty]
            private DateTime? dateTo;
            [ObservableProperty]
            BtListHelper? staffFrom;
            [ObservableProperty]
            BtListHelper? staffTo;
            [ObservableProperty]
            BtListHelper? productFrom;
            [ObservableProperty]
            BtListHelper? productTo;
        }

        public partial class MasterGenkaHenkou : ObservableObject
        {
            [ObservableProperty]
            public int receiptNo;
            [ObservableProperty]
            public bool isChecked;
            [ObservableProperty]
            public string? editDate;
            [ObservableProperty]
            public string? changeDate;
            [ObservableProperty]
            public string? staffIncharge;
            [ObservableProperty]
            public string? staffName;
            [ObservableProperty]
            public string? wereCate;
            [ObservableProperty]
            public string? productCD;
            [ObservableProperty]
            public string? productName;
            [ObservableProperty]
            public int? rowNumIsGenkaFLG;
            [ObservableProperty]
            public int retailPrice;
            [ObservableProperty]
            public int oriCostPrice;
            [ObservableProperty]
            public decimal markupRate;
            [ObservableProperty]
            public int newCostPrice;
            [ObservableProperty]
            public string? makerNo;
            [ObservableProperty]
            public int rownum;

        }
    }
}
