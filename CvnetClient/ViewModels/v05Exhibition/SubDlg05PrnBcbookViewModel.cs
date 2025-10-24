using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Models;
using CvnetClient.Utils;
using CvnetClient.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static CvnetClient.ViewModels.SubDlg05PrnBcbookViewModel;

namespace CvnetClient.ViewModels
{
    
    public partial class SubDlg05PrnBcbookViewModel : BaseViewModel
    {
        public enum OutPutType { 正規商品, 中止商品CD, 全て }
        public enum BarcodeType { JAN, CODE39, NW7 }

        [ObservableProperty]
        Condition? condition;
        

        public void OnInit() 
        {
            Condition = new Condition();
            Condition.ExhibitionFrom = ".";
            Condition.ExhibitionTo = "ZZZZZZZZ";
            Condition.BrdFrom = ".";
            Condition.BrdTo = "ZZZZZZZZ";
            Condition.ProductFrom = ".";
            Condition.ProductTo = "ZZZZZZZZ"; 
        }
        [RelayCommand]
        public void SelExhibition1(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Condition != null)
            {
                Condition.ExhibitionFrom = get_sel00.Code;
                Condition.ExhibitionFromName = get_sel00.Name;
            }
        }
        [RelayCommand]
        public void SelExhibition2(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Condition != null)
            {
                Condition.ExhibitionTo = get_sel00.Code;
                Condition.ExhibitionToName = get_sel00.Name;
            }
        }
        [RelayCommand]
        public void SelBrd1(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Condition != null)
            {
                Condition.BrdFrom = get_sel00.Code;
                Condition.BrdFromName = get_sel00.Name;
            }
        }
        [RelayCommand]
        public void SelBrd2(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Condition != null)
            {
                Condition.BrdTo = get_sel00.Code;
                Condition.BrdToName = get_sel00.Name;
            }
        }

        [RelayCommand]
        public void SelProd1(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Condition != null)
            {
                Condition.ProductFrom = get_sel00.Code;
                Condition.ProductFromName = get_sel00.Name;
            }
        }
        [RelayCommand]
        public void SelProd2(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Condition != null)
            {
                Condition.ProductTo = get_sel00.Code;
                Condition.ProductToName = get_sel00.Name;
            }
        }
        
        [RelayCommand]
        async Task DoPrintAsync()
        {
            if (!ClientLib.MessageBox(this, "印刷しますか？")) return;
            ClientLib.CursorToWait();

            string[] wrk_para2 = new string[1];
            wrk_para2[0] = "Data/img";
            var ret_csv = AppData.Http!.AspxSqlQuery2("get_img_path", wrk_para2);
            var ret_csv1 = ret_csv.Split('\n');
            string image_path = ret_csv1[0].ToString() + "\\";
            string[] wrk_para = new string[6];
            wrk_para[0] = Condition.ExhibitionFrom.ToString();
            wrk_para[1] = Condition.ExhibitionTo.ToString();
            wrk_para[2] = Condition.BrdFrom.ToString();
            wrk_para[3] = Condition.BrdTo.ToString();
            wrk_para[4] = Condition.ProductFrom.ToString();
            wrk_para[5] = Condition.ProductTo.ToString();
            var sql_str = string.Empty;
            int OutMethod = 1;//0:商品,1:SKU
            if (OutMethod == 0)
            {
                sql_str = "select '" + wrk_para[0] + "' 範囲0,'" + wrk_para[1] + "' 範囲1,'" + wrk_para[2] + "' 範囲2,'" + wrk_para[3] + "' 範囲3,'" + wrk_para[4] + "' 範囲4,'" + wrk_para[5] + "' 範囲5";
                sql_str += ",a.商品CD,a.商品名,a.元上代,a.展示会CD,c.展示会名,a.ブランドCD,d.ブランド名,a.アイテムCD,e.アイテム名,a.デリバリー日";
                sql_str += ",'" + image_path + "'||nvl(a.絵型名,'.') 絵型名,NVL((SELECT min(J.JANコード1) FROM HC$MASTER_SHOHIN_JAN J WHERE J.商品CD=a.商品CD),'') JANコード";
                sql_str += " from HC$Master_SHOHIN a";
                sql_str += " ,(select 名称CD,名称 展示会名 from HC$Master_meisho where 名称区分='TNJ') c";
                sql_str += " ,(select 名称CD,名称 ブランド名 from HC$Master_meisho where 名称区分='BRD') d";
                sql_str += " ,(select 名称CD,名称 アイテム名 from HC$Master_meisho where 名称区分='ITM') e";
                sql_str += " where a.展示会CD between :1 and :2 and a.ブランドCD between :3 and :4 and a.商品CD between :5 and :6";
                sql_str += " and (a.展示会CD=c.名称CD(+)) and (a.ブランドCD=d.名称CD(+)) and (a.アイテムCD=e.名称CD(+))";
                sql_str += " order by a.展示会CD,a.商品CD";
            }
            else 
            {
                sql_str = "select '" + wrk_para[0] + "' 範囲0,'" + wrk_para[1] + "' 範囲1,'" + wrk_para[2] + "' 範囲2,'" + wrk_para[3] + "' 範囲3,'" + wrk_para[4] + "' 範囲4,'" + wrk_para[5] + "' 範囲5";
                sql_str += ",a.商品CD,a.商品名,a.元上代,a.展示会CD,c.展示会名,a.ブランドCD,d.ブランド名,a.アイテムCD,e.アイテム名,a.デリバリー日";
                sql_str += ",'" + image_path + "'||nvl(a.絵型名,'.') 絵型名,f.色名,get_sizename(a.商品CD,b.サイズCD) サイズ名,b.JANコード1,b.JANコード2";
                sql_str += " from HC$Master_SHOHIN a,HC$Master_SHOHIN_JAN b";
                sql_str += " ,(select 名称CD,名称 展示会名 from HC$Master_meisho where 名称区分='TNJ') c";
                sql_str += " ,(select 名称CD,名称 ブランド名 from HC$Master_meisho where 名称区分='BRD') d";
                sql_str += " ,(select 名称CD,名称 アイテム名 from HC$Master_meisho where 名称区分='ITM') e";
                sql_str += " ,(select 名称CD,名称 色名 from HC$Master_meisho where 名称区分='COL') f";
                sql_str += " ,(select 名称CD,名称 サイズ名 from HC$Master_meisho where 名称区分='SIZ') g";
                sql_str += " where a.展示会CD between :1 and :2 and a.ブランドCD between :3 and :4 and a.商品CD between :5 and :6";
                sql_str += " and (a.商品CD=b.商品CD(+)) and (a.展示会CD=c.名称CD(+)) and (a.ブランドCD=d.名称CD(+))";
                sql_str += " and (a.アイテムCD=e.名称CD(+)) and (b.色CD=f.名称CD(+)) and (b.サイズCD=g.名称CD(+))";
                if (Condition.SelectedOutPut.ToString() == "正規商品") sql_str += " and b.使用FLG=0";
                else if (Condition.SelectedOutPut.ToString() == "中止商品CD") sql_str += " and b.使用FLG=1";
                sql_str += " order by a.展示会CD,a.商品CD,b.色CD,b.サイズCD";
            }

            var qfm = "cvnet05prn002.qfm";
            if (OutMethod == 0)
            {
                qfm = "cvnet05prn_sho.qfm";
                if (Condition.SelectedBarcode == BarcodeType.CODE39) qfm = "cvnet05prn_sho_code39.qfm";
                else if (Condition.SelectedBarcode == BarcodeType.NW7) qfm = "cvnet05prn_sho_nw7.qfm";
            }
            else
            {
                if (Condition.SelectedBarcode == BarcodeType.CODE39) qfm = "cvnet05prn0021.qfm";
                else if (Condition.SelectedBarcode == BarcodeType.NW7) qfm = "cvnet05prn0022.qfm";
            }
            
            var ret = AppData.Http!.AspxSqlQueryCsv(string.Format(sql_str), wrk_para, qfm);
            var lines = ret.Split('\n');

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

        

    }


    public partial class Condition : ObservableObject 
    {
        [ObservableProperty]
        private string? exhibitionFrom;
        [ObservableProperty]
        private string? exhibitionFromName;
        [ObservableProperty]
        private string? exhibitionTo;
        [ObservableProperty]
        private string? exhibitionToName;
        [ObservableProperty]
        private string? brdFrom;
        [ObservableProperty]
        private string? brdFromName;
        [ObservableProperty]
        private string? brdTo;
        [ObservableProperty]
        private string? brdToName;
        [ObservableProperty]
        private string? productFrom;
        [ObservableProperty]
        private string? productFromName;
        [ObservableProperty]
        private string? productTo;
        [ObservableProperty]
        private string? productToName;
        [ObservableProperty]
        private OutPutType selectedOutPut = OutPutType.正規商品;
        [ObservableProperty]
        private BarcodeType selectedBarcode = BarcodeType.JAN;
    }
}
