using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Class;
using CvnetClient.Models;
using CvnetClient.Utils;
using CvnetClient.Views;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg05PrnSwatchViewModel : BaseViewModel
    {
        #region Declare
        public enum DayType { DateDisp, SeasonDisp }
        public enum OutPutType { All, Product }
        public enum OrderType { Not, Do }
        public enum ShopType { All, ByCust }
        public enum SendiType { AND, OR }
        [ObservableProperty]
        ListFlexData listFlexData = new ListFlexData();
        [ObservableProperty]
        SearchCondition? condition;
        [ObservableProperty]
        BizArray? vpara;
        [ObservableProperty]
        public int? menu_para;
        [ObservableProperty]
        public bool flagCond;
        [ObservableProperty]
        public bool flagCond1;
        private BizArray para;
        private BizArray v_flg;
        [ObservableProperty]
        private OrderType selectedOrder = OrderType.Not;
        [ObservableProperty]
        private ShopType selectedShop = ShopType.All;
        #endregion
        #region Initialize
        public void OnInit(object? init_para = null, object? init_flg = null) 
        {
            if (init_para != null)
            {
                if (init_para is string s)
                {
                    var v_para = new string[] { s };
                    para = new BizArray(v_para);
                }
                else if (init_para is string[] arr)
                    para = new BizArray(arr);
                else para = new BizArray();
            }
            else para = new BizArray();

            if (init_flg != null)
            {
                if (init_flg is string s)
                {
                    var v_para = new string[] { s };
                    v_flg = new BizArray(v_para);
                }
                else if (init_flg is string[] arr)
                    v_flg = new BizArray(arr);
                else v_flg = new BizArray();
            }
            else v_flg = new BizArray();

            Menu_para = 0;
            List<CsvItem> def = null;
            Condition = new SearchCondition();
            Condition.BrandTo = new BtListHelper("ZZZZZZZZ", "");
            Condition.ExhbTo = new BtListHelper("ZZZZZZZZ", "");
            Condition.MaterialTo = new BtListHelper("ZZZZZZZZ", "");
            //AppData.ClassCvnet.AspxSqlQueryImp();
            ListFlexData.ListConfig = new ListFlexConfig()
            {
                init_csv = def ?? new List<CsvItem>(),
                flag = 1
            };
        }
        #endregion
        #region Function
        [RelayCommand]
        public void SelBrd1(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Condition != null)
            {
                Condition.BrandFrom = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        [RelayCommand]
        public void SelBrd2(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Condition != null)
            {
                Condition.BrandTo = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        [RelayCommand]
        public void SelExhb1(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Condition != null)
            {
                Condition.ExhbFrom = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        [RelayCommand]
        public void SelExhb2(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Condition != null)
            {
                Condition.ExhbTo = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        [RelayCommand]
        public void SelSoz1(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Condition != null)
            {
                Condition.MaterialFrom = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        [RelayCommand]
        public void SelSoz2(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Condition != null)
            {
                Condition.MaterialTo = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        [RelayCommand]
        public void SelShop(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Condition != null)
            {
                Condition.SelectedShop1 = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        [RelayCommand]
        async Task DoPrintAsync()
        {
            if (Condition == null) {
                return;
            }
            if (Condition.BrandFrom?.Code == "" && Condition.BrandTo?.Code == "")
            {
                ClientLib.MessageBoxError(this,"ブランドコードが未入力です");
                return;
            }
            else
            {
                if (Condition.BrandTo?.Code == "") Condition.BrandTo.Code = Condition.BrandFrom?.Code;
            }
            if (Condition.ExhbFrom?.Code == "" && Condition.ExhbTo?.Code == "")
            {
                ClientLib.MessageBoxError(this, "展示会コードが未入力です");
                return;
            }
            else
            {
                if (Condition.ExhbTo?.Code == "") Condition.ExhbTo.Code = Condition.ExhbFrom?.Code;
            }
            if (Condition.MaterialFrom?.Code != "")
            {
                if (Condition.MaterialTo?.Code == "") Condition.MaterialTo.Code = Condition.MaterialFrom?.Code;
            }

            /* 範囲指定対応 2010.01.29 */
            /* 展開数をチェックし、帳票ファイルを決定 */
            var wrk_para = new string[4];
            wrk_para[0] = Condition.ExhbFrom?.Code ?? string.Empty;
            wrk_para[1] = Condition.ExhbTo?.Code ?? string.Empty;
            wrk_para[2] = Condition.BrandFrom?.Code ?? string.Empty;
            wrk_para[3] = Condition.BrandTo?.Code ?? string.Empty;

            var sql_query = ""
                + " SELECT t.展開数,t.レイアウトNO"
                + " FROM HC$TRAN_TENSWT t"
                + " WHERE t.展示会CD BETWEEN :1 AND :2 AND "
                    + "t.名称CD1 BETWEEN :3 AND :4 AND "
                    + "t.名称CD2 BETWEEN '" + ((Condition.MaterialFrom?.Code != null) ? Condition.MaterialTo?.Code : ".") + "' AND '" + ((Condition.MaterialFrom?.Code != null) ? Condition.MaterialFrom?.Code : ".") + "'"
                    + ((Condition.SelectedOutPut == OutPutType.Product) ? " and exists (select 'X' from HC$MASTER_SHOHIN_JAN j where j.商品CD=t.商品CD and j.使用FLG=0)" : "")
                + " GROUP BY t.展開数,t.レイアウトNO";

            var sql_query2 = "SELECT count(*) FROM (" + sql_query + ")";

            var ret_csv = AppData.Http!.AspxSqlQuery(sql_query2, wrk_para);
            if (int.Parse(ret_csv.Rows[0][0].ToString()) > 1)
            {
                ClientLib.MessageBoxError(this, "展開数・レイアウト体系が異なるスワッチが存在する為、印刷できません");
                return;
            }

            var v_para = new string[7];
            v_para[0] = Condition.ExhbFrom?.Code ?? string.Empty;
            v_para[1] = Condition.ExhbTo?.Code ?? string.Empty;
            v_para[2] = Condition.BrandFrom?.Code ?? string.Empty;
            v_para[3] = Condition.BrandTo?.Code ?? string.Empty;
            v_para[4] = Condition.MaterialFrom?.Code ?? string.Empty;
            if (Condition.MaterialFrom?.Code == "") v_para[5] = "zzzzzzzz";
            else v_para[5] = Condition.MaterialTo.Code;
            if (Condition.SelectedDay == DayType.DateDisp)
            {
                v_para[6] = "0";
            }
            else {
                v_para[6] = "1";
            }
            var wrk_para2 = new string[4];
            for (var i = 0; i < 4; i++) wrk_para2[i] = v_para[i];
            ret_csv = AppData.Http!.AspxSqlQuery(sql_query, wrk_para2);
            var tenkai_su = 0;
            var layout_no = 0;
            if (ret_csv.Rows.Count > 0)
            {
                tenkai_su = int.Parse(ret_csv.Rows[0][0].ToString());
                layout_no = int.Parse(ret_csv.Rows[0][1].ToString());
            }
            wrk_para = new string[13];
            for (var i = 0; i < v_para.Length; i++) wrk_para[i] = v_para[i];

            wrk_para[7] = tenkai_su.ToString();
            wrk_para[8] = layout_no.ToString();

            if (SelectedOrder == OrderType.Not)
            {
                wrk_para[9] = "0";
            }
            else
            {
                wrk_para[9] = "1";
            }
            if (SelectedShop == ShopType.All)
            {
                wrk_para[10] = "0";
                wrk_para[11] = "";
            }
            else
            {
                wrk_para[10] = "1";
                wrk_para[11] = Condition.SelectedShop1?.Code;
            }
            

            wrk_para[12] = "";
            if (SelectedOrder == OrderType.Do)
            {
                if (Condition.SelectedSendi == SendiType.AND)
                {
                    wrk_para[12] = ListFlexData.GetQueryStr2(Vpara, 1, 0, "t.");
                }
                else
                {
                    wrk_para[12] = ListFlexData.GetQueryStr2(Vpara, 1, 1, "t.");
                }
            }

            if (Condition.SelectedOutPut == OutPutType.All) 
            {
                wrk_para[13] = "0";
            }
            else
            {
                wrk_para[13] = "1";
            }
            var ten = string.Empty;
            if (tenkai_su != 6 && tenkai_su != 0) ten = tenkai_su.ToString();
            var lo = string.Empty;
            if (layout_no == 1) lo = "_b";
            if (layout_no == 2) lo = "_c";
            if ((tenkai_su == 2) && (layout_no == 1 || layout_no == 2)) lo = "_b";

            var bcd = string.Empty;
            if (Menu_para == 2) lo = "_bcd";
            var qfm_file = "cvnet05prnswt" + ten + lo + bcd + ".qfm";
            var retcsv = AppData.Http!.AspxSqlQueryCsv("_tenswth", wrk_para, qfm_file);

            var lines = retcsv.Split('\n');

            if (lines.Length < 2 || lines[1] == "0")
            {
                ClientLib.MessageBoxError(this, "PDFデータがありません");
                return;
            }

            string pdfPath = lines[0];
            string url = AppData.Http.URLroot + pdfPath + "/data.pdf";

            bool ready = await Utils.GlobalFunc.WaitForPdfAsync(url, TimeSpan.FromSeconds(30));
            if (!ready)
            {
                ClientLib.MessageBoxError(this, "PDF生成に時間がかかりすぎています。\n 条件を絞ってください。");
                return;
            }

            var win = new WebpdfView();
            if (win.DataContext is WebpdfViewModel vm)
            {
                vm.Pdfdata = url;
            }
            ClientLib.CursorToNormal();
            ClientLib.ShowDialogView(win, this);
        }
        partial void OnSelectedShopChanged(ShopType value)
        {
            if (value == ShopType.All)
            {
                FlagCond = false;
            }
            else
            {
                FlagCond = true;
            }
        }
        partial void OnSelectedOrderChanged(OrderType value)
        {
            if (value == OrderType.Not)
            {
                FlagCond1 = false;
            }
            else
            {
                FlagCond1 = true;
            }
        }
        #endregion
        public partial class SearchCondition : ObservableObject
        {
            [ObservableProperty]
            private DayType selectedDay = DayType.DateDisp;
            [ObservableProperty]
            private OutPutType selectedOutPut = OutPutType.All;            
            [ObservableProperty]
            private SendiType selectedSendi = SendiType.AND;
            [ObservableProperty]
            private BtListHelper? brandFrom;
            [ObservableProperty]
            private BtListHelper? brandTo;
            [ObservableProperty]
            private BtListHelper? exhbFrom;
            [ObservableProperty]
            private BtListHelper? exhbTo;
            [ObservableProperty]
            private BtListHelper? materialFrom;
            [ObservableProperty]
            private BtListHelper? materialTo;
            [ObservableProperty]
            private BtListHelper? selectedShop1;            
        }
    }
}
