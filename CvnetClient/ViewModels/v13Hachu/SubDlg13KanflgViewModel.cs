using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg13KanflgViewModel : BaseViewModel
    {
        public ObservableCollection<PurchaseRow> Rows { get; }
    = new ObservableCollection<PurchaseRow>
{
    //new PurchaseRow
    //{
    //    No = 1,
    //    完了FLG = 0,
    //    仕入先 = "ABC商事",
    //    商品コード = "P-001",
    //    商品名 = "テスト商品A",
    //    倉庫 = "東京",
    //    数量 = 10,
    //    単価 = 1000,
    //    金額 = 10000,
    //    税区分 = "外税",
    //    税率 = 10,
    //    納品日 = DateTime.Today,
    //    伝票No = "D0001",
    //    備考 = "ダミーデータ"
    //},
    //new PurchaseRow
    //{
    //    No = 2,
    //    完了FLG = 1,
    //    仕入先 = "XYZ株式会社",
    //    商品コード = "P-002",
    //    商品名 = "テスト商品B",
    //    倉庫 = "大阪",
    //    数量 = 5,
    //    単価 = 2000,
    //    金額 = 10000,
    //    税区分 = "内税",
    //    税率 = 10,
    //    納品日 = DateTime.Today,
    //    伝票No = "D0002",
    //    備考 = "スクロール確認用"
    //},
    //new PurchaseRow
    //{
    //    No = 1,
    //    完了FLG = 1,
    //    仕入先 = "XYZ株式会社",
    //    商品コード = "P-002",
    //    商品名 = "テスト商品B",
    //    倉庫 = "大阪",
    //    数量 = 5,
    //    単価 = 2000,
    //    金額 = 10000,
    //    税区分 = "内税",
    //    税率 = 10,
    //    納品日 = DateTime.Today,
    //    伝票No = "D0002",
    //    備考 = "スクロール確認用"
    //}
};

        public void OnInit(object? init_para = null, string? init_flg = null)
        {
            OnInitBase(init_para, init_flg);
        }

        public void OnQuery()
        {
            /* 初期化処理 */

            /* 検索SQL生成・実行処理 */
            var sql_str = "select"
                + " t1.SEQ_NO,t1.ヘッダNO,t1.行NO,t1.完了FLG"
                + ",t1.商品CD,t1.色CD,(select m.名称 from HC$master_meisho m where m.名称区分='COL' and m.名称CD=t1.色CD) 色名"
                + ",t1.サイズCD,GET_SIZENAME(t1.商品CD,t1.サイズCD) SZNAME"
                + ",t1.数量,t1.単価,t1.金額"
                + ",NVL((select sum(decode(trunc(tt1.明細取引区分/10),1,1,2,-1,0)*tt1.数量) from HC$tran_tori1 tt1 where tt1.伝票処理区分=3 and tt1.関連伝票NO=t1.ヘッダNO and tt1.商品CD=t1.商品CD and tt1.色CD=t1.色CD and tt1.サイズCD=t1.サイズCD),0) 入荷数"
                + ",t1.明細メモ 摘要"
                + ",t0.取引先CD1 ||' '|| si.仕入先名 仕入先"
                + " from"
                + " HC$tran_tori0 t0"
                + " join HC$tran_tori1 t1 on (t0.SEQ_NO=t1.ヘッダNO)"
                + " join HC$master_shohin s on (t1.商品CD=s.商品CD)"
                + " join HC$MASTER_SIIRE si on (t0.取引先CD1=si.仕入先CD)"
                + " where"
                + " t0.伝票処理区分=13 and trunc(t0.取引区分/10)=1 and s.在庫管理FLG!=0"
                + " and t0.取引先CD1 between :1 and :2 and t0.倉庫CD between :3 and :4 ";
            //if (flg == 1)
            //{
            //    sql_str += " and t0.在庫計上日 between :5 and :6 ";
            //}
            //else
            //{
            //    sql_str += " and t0.納品日 between :5 and :6 ";
            //}
            sql_str += " and t1.商品CD between :7 and :8 and t0.seq_no between :9 and :10 "
            + " order by"
            + " t1.vdate_create,t1.行NO";

            sql_str = "SELECT * FROM (" + sql_str + ") where 数量>入荷数";   /* 2008.02.04 残無非表示対応 追加 */

            //var ret_csv = Classsatoo.AspxSqlQuery(sql_str, wrk_para);
            ///* 検索データ画面展開処理 */
            //ret_csv.SetCol2("line");
            //Form1.CvnetFlexView1 << ret_csv;
        }
    }
}

//public class PurchaseRow
//{
//    public int No { get; set; }
//    public int KanFLG { get; set; }
//    public string Shiire { get; set; }
//    public string ProdCode { get; set; }
//    public string ProdName { get; set; }
//    public string Stock { get; set; }
//    public int Quantity { get; set; }
//    public decimal Price { get; set; }
//    public decimal Genka { get; set; }
//    public string Tax { get; set; }
//    public int TaxRate { get; set; }
//    public DateTime Lod { get; set; }
//    public string Denpyo { get; set; }
//    public string Note { get; set; }
//}

public class PurchaseRow
{
    public int No { get; set; }
    public int 完了FLG { get; set; }
    public string 仕入先 { get; set; }
    public string 商品コード { get; set; }
    public string 商品名 { get; set; }
    public string 倉庫 { get; set; }
    public int 数量 { get; set; }
    public decimal 金額 { get; set; }
    public decimal 単価 { get; set; }
    public string 税区分 { get; set; }
    public int 税率 { get; set; }
    public DateTime 納品日 { get; set; }
    public string 伝票No { get; set; }
    public string 備考 { get; set; }
}