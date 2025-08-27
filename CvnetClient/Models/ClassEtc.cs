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
        public string[] SearchShohinCols;
        public string[] SearchTokuiCols;
        public string[] SearchSiireCols;
        public string[] SearchShainCols;
        public List<CsvItem> ListFlexPara;

        /* 2010.07.14 追加　LiseFlexView用CSV */
        public List<CsvItem> Bunrui_List0;   /* 商品マスタ */
        public List<CsvItem> Bunrui_List1;   /* 得意先マスタ */
        public List<CsvItem> Bunrui_List2;   /* 仕入先マスタ */
        public List<CsvItem> Bunrui_List3;  /* 社員マスタ */

        /* 商品検索内の文字列検索のヒット対象 */
        public string[] SearchShohinStr;

        /* 得意先検索内の文字列検索のヒット対象 */
        public string[] SearchTokuiStr;

        /* 仕入先検索内の文字列検索のヒット対象 */
        public string[] SearchSiireStr;

        /* 件名検索内の文字列検索のヒット対象 */
        public string[] SearchMsgStr;

        /* EC商品検索内の文字列検索のヒット対象 ECCUBE用コソーリ */
        public string[] SearchECShohinStr;

        /* 社員検索内の文字列検索のヒット対象 */
        public string[] SearchShainStr;

        /* 汎用ワーク */
        public Array WksHan01;
        public Array WksHan02;
        public Array WksHan03;

        public ClassEtc() 
        {
            SearchShohinStr = new string[2];
            SearchShohinStr[0] = "商品名";
            SearchShohinStr[1] = "略称";

            SearchTokuiStr = new string[2];
            SearchTokuiStr[0] = "得意先名";
            SearchTokuiStr[1] = "カナ";

            SearchSiireStr = new string[2];
            SearchSiireStr[0] = "仕入先名";
            SearchSiireStr[1] = "カナ";

            SearchMsgStr = new string[2];
            SearchMsgStr[0] = "件名";
            SearchMsgStr[1] = "MSG01";

            SearchECShohinStr = new string[2];
            SearchECShohinStr[0] = "商品CD";
            SearchECShohinStr[1] = "EC商品名";

            SearchShainStr = new string[2];
            SearchShainStr[0] = "名前";
            SearchShainStr[1] = "フリガナ";
        }
    } 

    public class CsvItem
    {
        public string col01 { get; set; } = string.Empty;
        public string col02 { get; set; } = string.Empty;
        public string col03 { get; set; } = string.Empty;
    }
}
