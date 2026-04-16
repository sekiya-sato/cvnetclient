using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CvnetClient.Models
{
    /// <summary>
    /// View Class - Global Selected Value for CvnetBtListView 
    /// </summary>
    public partial class SelValueModel : ObservableObject
    {
        [ObservableProperty]
        string? code;
        [ObservableProperty]
        string? name;
    }

    /// <summary>
    /// View Class - SubDlgSelShoView
    /// </summary>
    public partial class Sel00Model : ObservableObject
    {
        [ObservableProperty]
        string? code;
        [ObservableProperty]
        string? name;
    }

    /// <summary>
    /// View Class - SubDlg80gphSelView
    /// </summary>
    public partial class SubDlg80gphSelModel : ObservableObject
    {
        [ObservableProperty]
        string? code;
        [ObservableProperty]
        string? name;
        [ObservableProperty]
        string? abbreaviate;
    }

    /// <summary>
    /// View Class - SubDlgSel2View
    /// </summary>
    public partial class SubDlgSel2Model : ObservableObject
    {
        [ObservableProperty]
        string? productCD; //商品CD
        [ObservableProperty]
        string? makerNo; //メーカー品番
        [ObservableProperty]
        string? productName; //商品名
        [ObservableProperty]
        long price; //上代
        [ObservableProperty]
        string? imgName; //絵型名
        [ObservableProperty]
        string? exhibitCD; // 展示会CD
        [ObservableProperty]
        string? brandCD; //ブランドCD
        [ObservableProperty]
        string? itemCD; //アイテムCD
        [ObservableProperty]
        string? custDeliDate; //デリバリー日
        [ObservableProperty]
        string? exhibitName; //展示会名
        [ObservableProperty]
        string? brandName; //ブランド名
        [ObservableProperty]
        string? itemName; //アイテム名
        [ObservableProperty]
        long cost; //原価
        [ObservableProperty]
        int taxCalcMethod; //消費税計算方法
        [ObservableProperty]
        long masterPrice; //マスタ上代
        [ObservableProperty]
        string? initLaunchDate; //店頭投入日
        [ObservableProperty]
        long opCostPrice; //営業原価
        [ObservableProperty]
        long purchasePrice; //仕入価格
        [ObservableProperty]
        string? deliveryDate; //納品日
        [ObservableProperty]
        int purchaseCate;  //仕入区分
        [ObservableProperty]
        int prdMngmentFLG; //商品管理FLG
        [ObservableProperty]
        string? unit; //単位
        [ObservableProperty]
        int salesUnitPrice;//売単価
        [ObservableProperty]
        string? subnameCD1; //副名CD1
        [ObservableProperty]
        string? subname1; //副名1
        [ObservableProperty]
        string? subnameCD2; //副名CD2
        [ObservableProperty]
        string? subname2; //副名2
        [ObservableProperty]
        string? subnameCD3; //副名CD3
        [ObservableProperty]
        string? subname3; //副名3
        [ObservableProperty]
        string? prodSizeCate; //商品サイズ区分
        [ObservableProperty]
        int invMngmentFLG; //在庫管理FLG
        [ObservableProperty]
        long invQuantity; //在庫数
    }

    /// <summary>
    /// View Class - SubDlgSel002View
    /// </summary>
    public partial class SubDlgSel002Model : ObservableObject
    {
        [ObservableProperty]
        string? code;
        [ObservableProperty]
        string? name;
    }

    /// <summary>
    /// View Class - SubDlgSel10pView
    /// </summary>
    public partial class Sel10pItem : ObservableObject
    {
        [ObservableProperty]
        string? code;
        [ObservableProperty]
        string? name;
    }

    /// <summary>
    /// View Class - SubDlgSelybnView
    /// </summary>
    public partial class SelybnModel : ObservableObject
    {
        [ObservableProperty]
        string? postalCode;
        [ObservableProperty]
        string? addres1;
        [ObservableProperty]
        string? addres2;
    }
}
