using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Models;
using CvnetClient.Utils;
using CvnetClient.Views;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg05PrnSwatchsdViewModel : BaseViewModel
    {
        #region Declare
        [ObservableProperty]
        SearchCondition? condition;
        #endregion
        #region Initialize
        public void OnInit(object? init_para = null, string? init_flg = null) 
        {
            OnInitBase(init_para, init_flg);
            Condition = new SearchCondition();
            var sql_str = "select m.名称||nvl((select (' '||t.名称) from HC$MASTER_MEISHO t where t.名称区分='TNJ' and t.名称CD=m.名称),'') 展示会"
                + " from HC$MASTER_MEISHO m where m.名称区分='CDS' and m.名称CD='01'";
            var ret_csv = AppData.Http!.AspxSqlQuery(sql_str);
            if (ret_csv.Rows.Count > 0) Condition.Exhibition = ret_csv.Rows[0][0].ToString();
        }
        #endregion
        #region Function
        [RelayCommand]        
        public void SelCust(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Condition != null)
            {
                Condition.CustCD = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        [RelayCommand]
        async Task DoPrintAsync()
        {
            if (Condition.Exhibition == "")
            {
                ClientLib.MessageBoxError(this, "展示会を登録してください");
                return;
            }
            if (Condition.CustCD.Code == "")
            {
                ClientLib.MessageBoxError(this, "倉庫を入力してください");
                return;
            }

            if (Condition == null) {
                return;
            }
            var wrk_para = new string[2];
            wrk_para[0] = Condition.Exhibition.Split(" ")[0].ToString();
            wrk_para[1] = Condition.CustCD.Code;

            var sql_str = ""
                + " select "
                    + "t0.展示会CD,"
                    + "nvl((select m.名称 from HC$MASTER_MEISHO m where m.名称区分='TNJ' and m.名称CD=t0.展示会CD),'') 展示会名,"
                    + "t0.取引先CD1 お客様CD,"
                    + "nvl((select t.得意先名 from HC$MASTER_TOKUI t where t.得意先CD=t0.取引先CD1),'') お客様名,"
                    + "S.ブランドCD,"
                    + "nvl((select m.名称 from HC$MASTER_MEISHO m where m.名称区分='BRD' and m.名称CD=S.ブランドCD),'') ブランド名,"
                    + "S.素材CD,"
                    + "nvl((select m.名称 from HC$MASTER_MEISHO m where m.名称区分='SZI' and m.名称CD=S.素材CD),'') 素材名,"
                    + "t1.商品CD,"
                    + "S.商品名,"
                    + "S.上代,"
                    + "t0.納品日 納期,"
                    + "t1.色CD,"
                    + "GET_COLORNAME(t1.色CD) 色名,"
                    + "t1.サイズCD,"
                    + "GET_SIZENAME(t1.商品CD,t1.サイズCD) サイズ名,"
                    + "sum(t1.数量) 数量,"
                    + "sum(t1.上代金額) 上代計,"
                    + "(S.ブランドCD||S.素材CD) Pflg"
                + " from "
                    + "HC$TRAN_TORI0 t0,"
                    + "HC$TRAN_TORI1 t1,"
                    + "HC$MASTER_SHOHIN S"
                + " where "
                    + " t0.SEQ_NO=t1.ヘッダNO"
                    + " and t1.商品CD=S.商品CD"
                    + " and t0.伝票処理区分=12"
                    + " and t1.伝票処理区分=12"
                    + " and t1.数量!=0"
                    + " and t0.展示会CD=:1"
                    + " and t0.取引先CD1=:2"
                + " group by "
                    + "t0.展示会CD,t0.取引先CD1,S.ブランドCD,S.素材CD,t1.商品CD,S.商品名,S.上代,t0.納品日,t1.色CD,t1.サイズCD"
                + " order by "
                    + "t0.展示会CD,t0.取引先CD1,S.ブランドCD,S.素材CD,t1.商品CD,t1.色CD,t1.サイズCD"
                + "";
            var ret_csv = AppData.Http!.AspxSqlQueryCsv(sql_str, wrk_para, "cvnet05prn_swatch_sd.qfm");
            var lines = ret_csv.Split('\n');

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
        #endregion
        public partial class SearchCondition : ObservableObject {
            [ObservableProperty]
            private string? exhibition;
            [ObservableProperty]
            private BtListHelper? custCD;
        }
    }
}
