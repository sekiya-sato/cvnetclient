using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CvnetClient.Models
{
    // Class to represent ClassSasaki
    public class ClassEtc
    {
        /* =========================================
        汎用検索フォーム用追加条件 配列
        ==========================================*/
        Array SearchShohinCols;
        Array SearchTokuiCols;
        Array SearchSiireCols;
        Array SearchShainCols;
        DataTable ListFlexPara;

        /* 2010.07.14 追加　LiseFlexView用CSV */
        List<bunruiItem> Bunrui_List0;   /* 商品マスタ */
        List<bunruiItem> Bunrui_List1;   /* 得意先マスタ */
        List<bunruiItem> Bunrui_List2;   /* 仕入先マスタ */
        List<bunruiItem> Bunrui_List3;	/* 社員マスタ */
    }

    public class bunruiItem
    {
        public string col01 { get; set; } = string.Empty;
        public string col02 { get; set; } = string.Empty;
        public string col03 { get; set; } = string.Empty;
    }
}
