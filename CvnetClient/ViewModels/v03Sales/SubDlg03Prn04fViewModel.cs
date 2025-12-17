using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Models;
using CvnetClient.Utils;
using CvnetClient.Views;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid;

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
        private string mstName = "請求";   // default
        [ObservableProperty]
        public string? parameter1 = "";   // default

        private DateTime dateFrom = DateTime.Today;

        [ObservableProperty]
        private bool isOutputUnitVoucher;

        [ObservableProperty]
        private bool isOutputUnitProduct;

        [ObservableProperty]
        private bool isOutputTypeParent;

        [ObservableProperty]
        private bool isOutputTypeSubdetail;

        [ObservableProperty]
        private bool isOutputWayPrint;

        [ObservableProperty]
        private bool isOutputWayMail;

        [ObservableProperty]
        private string dateSime;

        private int x;

        string qfm_file = "";

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
                { 99, "99" },
            };
            ChooseMenu.DeadLine = Shimebi.FirstOrDefault().Key;
            ChooseMenu.DeadLineString = Shimebi.FirstOrDefault().Value;

            SelectShop.DateFrom = dateFrom;

            // Output unit: "伝票単位" checked by default
            IsOutputUnitVoucher = true;
            //IsOutputUnitProduct = false;

            // Output type: "親のみ" checked by default
            IsOutputTypeParent = true;
            //IsOutputTypeSubdetail = false;

            // Output method (hidden): "印刷" checked by default
            IsOutputWayPrint = true;
            //IsOutputWayMail = false;
            SelectShop.ToWorkerShopCd = "99999999";
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

        [RelayCommand]
        public void FindToWorkerShop(SelValueModel value)
        {
            if (value == null) return;

            FindToWorkerShopCd = new BtListHelper(value.Code, value.Name);

            SelectShop.ToWorkerShopCd = value.Code;
            SelectShop.ToWorkerShopName = value.Name;
        }
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
            [ObservableProperty]
            private DateTime dateFrom;
        }

        partial void OnSelectedSelectDeadlineChanged(SelectConditionItem? value)
        {
            //if (value?.Value == null) return;
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


        private string OnQuery(string[] wrk_para)
        {
            int zeiritsu1 = 0;
            int zeiritsu2 = 0;
            string zeiritsu_sql = "select "
                + "nvl((select st.新消費税率 from HC$MASTER_SYSTAX st where st.消費税CD = 1),0)  標準税率,"
                + "nvl((select st.消費税率 from HC$MASTER_SYSTAX st where st.消費税CD = 2),0)  軽減税率"
            + " from dual";
            var ret_csv = AppData.Http?.AspxSqlQuery(zeiritsu_sql);

            string table_name = "HC$MANAGE_KAKESKY";

            //string dateSime = ChooseMenu.DeadLine.ToString();

            /* --------------------------------------------------------------------------------------------------------- */
            /* 出力タイプ：「親のみ」選択時 */
            /* --------------------------------------------------------------------------------------------------------- */
            var sql_str2 = ""
            + " SELECT "
                + "A.SEQ_NO SKY_SEQ,"
                + "A.請求日,"
                + "A.当月請求額,"
                /* 当月入金額合計から、相殺入金を抜く */
                + "A.現金入金+A.振込手数料+A.手形入金+A.その他入金 当月入金額合計,"
                + "A.売上金額,"
                + "A.返品金額+A.値引金額 返品値引,"
                + "A.その他売上,"
                + "A.消費税,"
                + "NVL((SELECT "
                        + "SUM(H.当月請求額)"
                    + " FROM "
                        + table_name + " H,"
                        + "(SELECT 得意先CD, DECODE(請求先CD, '.', 得意先CD, 請求先CD) 請求先CD FROM HC$MASTER_TOKUI) I"
                    + " WHERE "
                        + "H.得意先CD=I.得意先CD AND I.請求先CD=B.請求先CD AND H.請求日 < A.請求日),0) 前月残,"
                + "B.得意先CD,"
                + "B.請求先CD,"
                /* + "B.締日," */
                + "DECODE('" + dateSime + "', '99', '末', '" + dateSime + "') 締日,"
                + "C.SEQ_NO 伝票NO,"
                + "C.伝票処理区分,"
                + "C.掛計上日,"
                + "DECODE(C.伝票処理区分, 6, D.明細取引区分, C.取引区分) 取引区分, "
                + "DECODE(C.伝票処理区分, 6, " + AppData.ClassCvnet.comboItem00.GetCaseStr("入金区分", "D.明細取引区分") + "," + AppData.ClassCvnet.comboItem00.GetCaseStr("共通売上区分", "C.取引区分") + ") 取引区分名, "
                + "DECODE(TRUNC(C.取引区分 / 10), 1, 1, 2, -1, -1) * C.数量合計 数量合計,"
                + "DECODE(D.伝票処理区分, 6, 0, DECODE(TRUNC(C.取引区分 / 10), 1, 1, 2, -1, 9, 1, -1) * C.下代合計) 明細金額合計,"
                + "DECODE(TRUNC(C.取引区分 / 10), 1, 1, 2, -1, 9, 1, -1) * DECODE(B.店種区分, 3, (C.売仕内税消費税+C.売仕外税消費税), C.内税消費税+C.外税消費税) 消費税合計,"
                + "C.手入力伝票NO,"
                + "DECODE(D.伝票処理区分, 6, D.明細メモ, C.メモ) メモ,"
                + "D.行NO,"
                + "DECODE(D.伝票処理区分, 6, D.金額, 0) 金額,"
                + "DECODE(TRUNC(C.取引区分 / 10), 1, 1, 2, -1, -1) * DECODE(B.店種区分, 3, (D.売仕内税消費税+D.売仕外税消費税), D.内税消費税+D.外税消費税) 明細消費税,"
                + "D.明細メモ,"
                + "E.自社名,"
                + "E.郵便番号 自社郵便番号,"
                + "E.住所1 自社住所1,"
                + "E.住所2 自社住所2,"
                + "E.住所3 自社住所3,"
                + "E.TEL 自社TEL,"
                + "E.FAX 自社FAX,"
                + "CASE WHEN (B.振込先1 != '.') THEN B.振込先1 ELSE E.振込先1 END 振込先1,"
                + "CASE WHEN (B.振込先1 != '.') THEN B.振込先2 ELSE E.振込先2 END 振込先2,"
                + "CASE WHEN (B.振込先1 != '.') THEN B.振込先3 ELSE E.振込先3 END 振込先3,"
                + "A.相殺入金,"
                + "C.請求書NO,"

                /* 2022.05.13 #66849対応追加（消費税関連項目追加） */
                + "CASE WHEN (DECODE(C.伝票処理区分, 6, D.明細取引区分, C.取引区分) = 99) THEN TO_CHAR(decode(C.消費税率,0,GET_TAXRATE(C.在庫計上日),C.消費税率))"
                        + " WHEN (DECODE(C.伝票処理区分, 6, D.明細取引区分, C.取引区分) >= 80) THEN ''"
                        + " ELSE  TO_CHAR(C.消費税率) END 消費税率,"
                + zeiritsu1.ToString() + " 標準税率,"
                + zeiritsu2.ToString() + " 軽減税率,"
                + "decode(C.消費税率," + zeiritsu1.ToString() + ",DECODE(TRUNC(C.取引区分 / 10), 1, 1, 2, -1, 9, 1, -1) * DECODE(B.店種区分, 3, (C.売仕内税消費税+C.売仕外税消費税), C.内税消費税+C.外税消費税),0) 標準税額,"
                + "decode(C.消費税率," + zeiritsu2.ToString() + ",DECODE(TRUNC(C.取引区分 / 10), 1, 1, 2, -1, 9, 1, -1) * DECODE(B.店種区分, 3, (C.売仕内税消費税+C.売仕外税消費税), C.内税消費税+C.外税消費税),0) 軽減税額,"

                + "A.入金予定日"
            + " FROM "
                + "(SELECT "
                    + "MAX(q.SEQ_NO) seq_no,"
                    + "t.請求先CD,"
                    + "q.請求日,"
                    + "q.請求開始日,"
                    + "q.請求終了日,"
                    + "q.入金予定日,"
                    + "MAX(q.請求書NO) 請求書NO,"
                    + "SUM(q.当月請求額) 当月請求額,"
                    + "SUM(q.売上金額) 売上金額,"
                    + "SUM(q.返品金額) 返品金額,"
                    + "SUM(q.値引金額) 値引金額,"
                    + "SUM(q.その他売上) その他売上,"
                    + "SUM(q.消費税) 消費税,"
                    + "SUM(q.現金入金) 現金入金,"
                    + "SUM(q.振込手数料) 振込手数料,"
                    + "SUM(q.手形入金) 手形入金,"
                    + "SUM(q.相殺入金) 相殺入金,"
                    + "SUM(q.その他入金) その他入金"
                + " FROM "
                    + table_name + " q,"
                    + "(SELECT "
                        + "得意先CD, DECODE(請求先CD, '.', 得意先CD, 請求先CD) 請求先CD, 締日, 締日2, 締日3"
                    + " FROM "
                        + "HC$MASTER_TOKUI"
                    + " WHERE "
                        + "請求印刷=1) t"
                + " WHERE "
                    + "t.得意先CD=q.得意先CD AND "
                    + "q.締日=:1 AND "
                    + "(t.締日=" + dateSime + " OR t.締日2=" + dateSime + " OR t.締日3=" + dateSime + ") AND "
                    + "q.請求日 BETWEEN :2 AND :3 AND "
                    + "t.請求先CD BETWEEN :4 AND :5"
                + " GROUP BY "
                    + "t.請求先CD, q.請求日, q.請求開始日, q.請求終了日, q.入金予定日"
                + ") A, "
                + "(SELECT "
                    + "得意先CD, "
                    + "DECODE(請求先CD, '.', 得意先CD, 請求先CD) 請求先CD,"
                    /* + "締日," */
                    + "'" + dateSime + "' 締日,"
                    + "店種区分,"
                    + "名称CD10,"
                    + "振込先1,"
                    + "振込先2,"
                    + "振込先3"
                + " FROM "
                    + "HC$MASTER_TOKUI"
                + ") B,"
                + "HC$TRAN_TORI0 C,"
                + "HC$TRAN_TORI1 D,"
                + "HC$MASTER_SYSKANRI E"
            + " WHERE "
                + "A.請求先CD=B.請求先CD AND "
                + "B.得意先CD=C.取引先CD1 AND "
                /* + "((C.伝票処理区分 in (0, 20) AND B.店種区分 in (1, 7) AND D.行NO=1) OR " */
                /* 2023.11.20 #71591対応修正（出荷売上の店種区分条件を削除） */
                + "((C.伝票処理区分 in (0, 20) AND D.行NO=1) OR "
                    + "(C.伝票処理区分 in (1, 20) AND B.店種区分=3 AND D.行NO=1) OR "
                    + "(C.伝票処理区分=6 AND D.明細取引区分<>99)) AND "
                + "C.掛計上日 BETWEEN A.請求開始日 AND A.請求終了日 AND "
                + "D.ヘッダNO=C.SEQ_NO"

            /* 当月取引無し、残あり */
            + " UNION "
            + " SELECT "
                + "A.SEQ_NO SKY_SEQ, "
                + "A.請求日, "
                + "A.当月請求額,"
                + "A.現金入金+A.振込手数料+A.手形入金+A.その他入金 当月入金額合計,"
                + "A.売上金額,"
                + "A.返品金額+A.値引金額 返品値引,"
                + "A.その他売上,"
                + "A.消費税,"
                + "NVL((SELECT "
                        + "SUM(H.当月請求額)"
                    + " FROM "
                        + table_name + " H,"
                        + "(SELECT 得意先CD,DECODE(請求先CD, '.', 得意先CD, 請求先CD) 請求先CD FROM HC$MASTER_TOKUI) I"
                    + " WHERE "
                        + "H.得意先CD=I.得意先CD AND I.請求先CD=B.請求先CD AND H.請求日 < A.請求日), 0) 前月残,"
                + "B.請求先CD 得意先CD,"
                + "B.請求先CD,"
                /* + "B.締日," */
                + "DECODE('" + dateSime + "', '99', '末', '" + dateSime + "') 締日,"
                + "0 伝票NO,"
                + "0 伝票処理区分,"
                + "'' 掛計上日,"
                + "0 取引区分,"
                + "'' 取引区分名,"
                + "0 数量合計,"
                + "0 明細金額合計,"
                + "0 消費税合計,"
                + "'' 手入力伝票NO,"
                + "'' メモ,"
                + "0 行NO,"
                + "0 金額,"
                + "0 明細消費税,"
                + "'' 明細メモ,"
                + "E.自社名,"
                + "E.郵便番号 自社郵便番号,"
                + "E.住所1 自社住所1,"
                + "E.住所2 自社住所2,"
                + "E.住所3 自社住所3,"
                + "E.TEL 自社TEL,"
                + "E.FAX 自社FAX,"
                /* 20161201 出力タイプ：親のみであっても得意先マスタの振込先を優先するように変更 */
                /* + "E.振込先1,E.振込先2,E.振込先3," */
                + "CASE WHEN (B.振込先1 != '.') THEN B.振込先1 ELSE E.振込先1 END 振込先1,"
                + "CASE WHEN (B.振込先1 != '.') THEN B.振込先2 ELSE E.振込先2 END 振込先2,"
                + "CASE WHEN (B.振込先1 != '.') THEN B.振込先3 ELSE E.振込先3 END 振込先3,"
                + "A.相殺入金,"
                + "A.請求書NO,"
                /*#63441 インボイス制度導入*/
                /* + "'' 消費税率," */
                /* + "nvl((select st.新消費税率 from HC$MASTER_SYSTAX st where st.消費税CD=1),'') 消費税率1," */
                /* + "nvl((select st.消費税率 from HC$MASTER_SYSTAX st where st.消費税CD=2),'') 消費税率2," */

                /* 2022.05.13 #66849対応追加（消費税関連項目追加） */
                + "'' 消費税率,"
                + zeiritsu1.ToString() + " 標準税率,"
                + zeiritsu2.ToString() + " 軽減税率,"
                + "0 標準税額,"
                + "0 軽減税額,"

                + "A.入金予定日"
             + " FROM "
                + "(SELECT "
                    + "MAX(q.SEQ_NO) seq_no,"
                    + "t.請求先CD 得意先CD,"
                    + "q.請求日,"
                    + "q.請求開始日,"
                    + "q.請求終了日,"
                    + "q.入金予定日,"
                    + "SUM(q.当月請求額) 当月請求額,"
                    + "SUM(q.売上金額) 売上金額,"
                    + "SUM(q.返品金額) 返品金額,"
                    + "SUM(q.値引金額) 値引金額,"
                    + "SUM(q.その他売上) その他売上,"
                    + "SUM(q.消費税) 消費税,"
                    + "SUM(q.現金入金) 現金入金,"
                    + "SUM(q.振込手数料) 振込手数料,"
                    + "SUM(q.手形入金) 手形入金,"
                    + "SUM(q.相殺入金) 相殺入金,"
                    + "SUM(q.その他入金) その他入金,"
                    + "MAX(q.請求書NO) 請求書NO"
                + " FROM "
                    + table_name + " q,"
                    /* 請求書印刷FLG追加 */
                    + "(SELECT "
                        + "得意先CD, DECODE(請求先CD, '.', 得意先CD, 請求先CD) 請求先CD, 締日, 締日2, 締日3"
                    + " FROM "
                        + "HC$MASTER_TOKUI"
                    + " WHERE "
                        + "請求印刷=1"
                    + ") t"
                + " WHERE "
                    + "t.得意先CD=q.得意先CD AND "
                    + "q.締日=:1 AND "
                    /* + "(t.締日=:1 OR t.締日2=" + datesime + " OR t.締日3=" + datesime + ") AND " */
                    + "(t.締日=" + dateSime + " OR t.締日2=" + dateSime + " OR t.締日3=" + dateSime + ") AND "
                    + "q.請求日 BETWEEN :2 AND :3 AND "
                    + "t.請求先CD BETWEEN :4 AND :5"
                + " GROUP BY "
                    + "t.請求先CD, q.請求日, q.請求開始日, q.請求終了日, q.入金予定日"
                + ") A,"
                + "(SELECT "
                    + "請求先CD,"
                    /* + "MAX(締日) 締日" */
                    + "'" + dateSime + "' 締日"
                    + ",振込先1"
                    + ",振込先2"
                    + ",振込先3"
                + " FROM "
                    + "(SELECT "
                        + "DECODE(請求先CD, '.', 得意先CD, 請求先CD) 請求先CD, 締日,振込先1,振込先2,振込先3"
                    + " FROM "
                        + "HC$MASTER_TOKUI"
                    + ")"
                + " GROUP BY "
                    + "請求先CD"
                    + ",振込先1"
                    + ",振込先2"
                    + ",振込先3"
                + ") B,"
                + "HC$MASTER_SYSKANRI E"
             + " WHERE "
                + "A.得意先CD=B.請求先CD AND "
                + "(A.当月請求額=0 AND "
                    + "A.現金入金+A.振込手数料+A.手形入金+A.相殺入金+A.その他入金=0 AND "
                    + "A.売上金額+A.返品金額+A.値引金額+A.その他売上+A.消費税=0)";

            /* 純売上金額追加 20061107  当月買上計追加 20070214 */
            var sql_str = ""
            + " SELECT "
                + "A.*,"
                + "A.前月残-A.当月入金額合計-A.相殺入金 繰越金額,"
                + "A.前月残+A.当月請求額 当月残高,"
                + "B.得意先名 請求先名,"
                + "B.郵便番号 請求先郵便番号,"
                + "B.住所1 請求先住所1,"
                + "B.住所2 請求先住所2,"
                + "B.住所3 請求先住所3,"
                + "B.営業担当CD,"
                + "NVL((SELECT Q.名前 FROM HC$MASTER_SHAIN Q WHERE Q.社員CD=B.営業担当CD), '.') 担当名,"
                + "(A.売上金額-A.返品値引+A.その他売上) 純売上金額, "
                + "(A.売上金額-A.返品値引+A.その他売上+A.消費税) 当月買上計,"
                + "NVL((SELECT t.得意先名 FROM hC$MASTER_TOKUI t WHERE t.得意先CD=A.得意先CD), '') 子名,"
                + "請求書NO 請求書NO2";

            sql_str += ""
                + ",sum(decode(A.消費税率, " + zeiritsu1.ToString() + ", A.明細金額合計, 0)) over (partition by A.請求先CD) 標準税率対象金額合計"
                + ",sum(decode(A.消費税率, " + zeiritsu2.ToString() + ", A.明細金額合計, 0)) over (partition by A.請求先CD) 軽減税率対象金額合計"
                + ",sum(decode(A.消費税率, 0, A.明細金額合計, 0)) over (partition by A.請求先CD) 非課税対象金額合計"
                + ",sum(decode(A.消費税率, " + zeiritsu1.ToString() + ", A.標準税額, 0)) over (partition by A.請求先CD) 標準税額合計"
                + ",sum(decode(A.消費税率, " + zeiritsu2.ToString() + ", A.軽減税額, 0)) over (partition by A.請求先CD) 軽減税額合計"
                /* + ",nvl((select m.名称 from HC$MASTER_MEISHO m where m.名称区分='IBS' and m.名称CD='01'),'') 管理番号"; */
                + ",nvl((select max(m.登録番号) from HC$MASTER_SYSKANRI m),'') 自社登録番号"
                + ",decode(B.登録番号, '.', '', '登録番号：'||B.登録番号) 請求先登録番号";


            sql_str += " FROM "
                + "(" + sql_str2 + ") A,"
                + "HC$MASTER_TOKUI B"
            + " WHERE "
                + "A.請求先CD=B.得意先CD AND "
                + "(A.前月残<>0 OR "
                    + "(A.前月残+A.当月請求額)<>0 OR "
                    + "(A.当月入金額合計+A.相殺入金)<>0 OR "
                    + "A.返品値引<>0 OR "
                    + "(A.売上金額+A.その他売上+A.消費税)<>0"
                + ")"
            + " ORDER BY "
                + "A.請求先CD, A.請求日, A.掛計上日, A.伝票NO, 行NO";

            qfm_file = "CVPSK003f_v2.qfm";/* (08.08.27) A4サイズ */

            /* 出力単位：「商品CD単位」選択時 */
            if (IsOutputUnitProduct == true)
            {
                sql_str = ""
                + " SELECT DISTINCT "
                    + "a.*,"
                    + "t1.商品CD,"
                    + "(SELECT decode(s.消費税CD,2,'*','') || s.商品名 FROM HC$MASTER_SHOHIN s WHERE s.商品CD=t1.商品CD) 商品名,"
                    + "DECODE(TRUNC(t1.明細取引区分 / 10), 1, 1, 2, -1, 3, -1) * t1.金額 明細金額,"
                    + "DECODE(a.伝票NO, -1, 0, a.伝票NO) 伝票番号,"
                    + "DECODE(TRUNC(t1.明細取引区分 / 10), 1, 1, 2, -1, 3, -1) * t1.数量 明細数量"
                + " FROM "
                    + "(" + sql_str + ") a,"
                    + "(SELECT "
                        + "t1.ヘッダNO,"
                        + "t1.明細取引区分,"
                        + "t1.商品CD,"
                        + "SUM(t1.下代金額) 金額,"
                        + "SUM(t1.数量) 数量"
                    + " FROM "
                        + "HC$TRAN_TORI1 t1"
                    + " GROUP BY "
                        + "t1.ヘッダNO,"
                        + "t1.明細取引区分,"
                        + "t1.商品CD) t1"
                        + ",HC$MASTER_TOKUI t"
                + " WHERE "
                    + "a.伝票NO=t1.ヘッダNO(+) and a.請求先CD=t.得意先CD"
                + " ORDER BY "
                    + "A.請求先CD, A.請求書NO, A.請求日, A.掛計上日, A.伝票NO, A.行NO, t1.商品CD";

                /* qfm_file = "CVPSK003_hinf.qfm"; */
                /* 2022.05.13 #66849対応追加（新レイアウト） */
                qfm_file = "CVPSK003_hinf_v2.qfm";
            }

            /* --------------------------------------------------------------------------------------------------------- */
            /* 出力タイプ：「子明細」選択時 */
            /* --------------------------------------------------------------------------------------------------------- */
            if (IsOutputTypeSubdetail == true)
            {
                sql_str = "";
                sql_str += "SELECT A.SEQ_NO AS SKY_SEQ";
                sql_str += "     , A.請求日";
                sql_str += "     , A.当月請求額";
                sql_str += "     , CASE WHEN A.親子判別用 = 1 THEN A.当月入金額合計 ELSE NULL END AS 当月入金額合計";
                sql_str += "     , A.売上金額";
                sql_str += "     , A.返品値引";
                sql_str += "     , A.その他売上";
                sql_str += "     , A.消費税";
                sql_str += "     , CASE WHEN A.親子判別用 = 1 THEN A.前月残 ELSE NULL END AS 前月残";
                sql_str += "     , A.得意先CD";
                sql_str += "     , A.請求先CD";
                /* sql_str += "     , B.締日"; */
                sql_str += "     , DECODE('" + dateSime + "', '99', '末', '" + dateSime + "') 締日";
                sql_str += "     , B.伝票NO";
                sql_str += "     , B.伝票処理区分";
                sql_str += "     , B.掛計上日";
                sql_str += "     , B.取引区分";
                sql_str += "     , B.取引区分名";
                sql_str += "     , B.数量合計";
                sql_str += "     , B.明細金額合計";
                sql_str += "     , B.消費税合計";
                sql_str += "     , B.手入力伝票NO";
                sql_str += "     , B.メモ";
                sql_str += "     , B.行NO";
                sql_str += "     , B.金額";
                sql_str += "     , B.明細消費税";
                sql_str += "     , B.明細メモ";
                sql_str += "     , D.自社名";
                sql_str += "     , D.郵便番号 AS 自社郵便番号";
                sql_str += "     , D.住所1 AS 自社住所1";
                sql_str += "     , D.住所2 AS 自社住所2";
                sql_str += "     , D.住所3 AS 自社住所3";
                sql_str += "     , D.TEL AS 自社TEL";
                sql_str += "     , D.FAX AS 自社FAX";
                sql_str += "     , CASE WHEN (C.振込先1!='.') THEN C.振込先1 ELSE D.振込先1 END 振込先1";
                sql_str += "     , CASE WHEN (C.振込先1!='.') THEN C.振込先2 ELSE D.振込先2 END 振込先2";
                sql_str += "     , CASE WHEN (C.振込先1!='.') THEN C.振込先3 ELSE D.振込先3 END 振込先3";
                sql_str += "     , CASE WHEN A.親子判別用 = 1 THEN A.相殺入金 ELSE NULL END AS 相殺入金";
                sql_str += "     , 0 AS 請求書NO";
                sql_str += "     , CASE WHEN A.親子判別用 = 1 THEN A.前月残 - A.当月入金額合計 - A.相殺入金 ELSE NULL END AS 繰越金額";
                sql_str += "     , CASE WHEN A.親子判別用 = 1 THEN A.前月残 + A.当月請求額 ELSE NULL END AS 当月残高";
                sql_str += "     , C.得意先名 AS 請求先名";
                sql_str += "     , C.郵便番号 AS 請求先郵便番号";
                sql_str += "     , C.住所1 AS 請求先住所1";
                sql_str += "     , C.住所2 AS 請求先住所2";
                sql_str += "     , C.住所3 AS 請求先住所3";
                sql_str += "     , C.営業担当CD";
                sql_str += "     , NVL( E.名前, '.' ) AS 担当名";
                sql_str += "     , (A.売上金額 - A.返品値引 + A.その他売上) AS 純売上金額";
                sql_str += "     , (A.売上金額 - A.返品値引 + A.その他売上 + A.消費税) AS 当月買上計";
                sql_str += "     , CASE WHEN A.親子判別用 = 2 AND F.得意先名 IS NOT NULL THEN '(' || A.得意先CD || ') ' || F.得意先名 || ' 様分' ELSE '' END AS 子名";
                sql_str += "     , 0 AS 請求書NO2";

                if (IsOutputUnitProduct == true)
                {
                    /* 商品CD単位 */
                    sql_str += "     , G.商品CD";
                    sql_str += "     , decode(H.消費税CD,2,'*','') || H.商品名 商品名";
                    sql_str += "     , DECODE( TRUNC( G.明細取引区分 / 10), 1, 1, 2, -1, 3, -1 ) * ( G.金額 + DECODE( C.名称CD10, '02', DECODE( C.店種区分, 3, G.売仕消費税, G.消費税 ), 0 ) ) AS 明細金額";
                    sql_str += "     , B.伝票NO AS 伝票番号";
                    sql_str += "     , DECODE( TRUNC( G.明細取引区分 / 10), 1, 1, 2, -1, 3, -1) * G.数量 AS 明細数量";
                }
                else
                {
                    /* 伝票単位 */
                    sql_str += "     , NULL AS 商品CD";
                    sql_str += "     , NULL AS 商品名";
                    sql_str += "     , NULL AS 明細金額";
                    sql_str += "     , NULL AS 伝票番号";
                    sql_str += "     , NULL AS 明細数量";
                }

                sql_str += "     , B.関連伝票NO2";
                sql_str += "     , A.親子判別用";
                sql_str += "     , NVL(B.請求書NO,A.請求書NO)";
                sql_str += "     , B.消費税率";

                /* 2022.05.13 #66849対応追加（消費税関連項目追加） */
                sql_str += "     , B.標準税率";
                sql_str += "     , B.軽減税率";

                if (IsOutputUnitProduct == true)
                {
                    /* 商品CD単位 */
                    sql_str += ""
                    /* + ",sum(decode(B.消費税率, " + str( zeiritsu1 ) + ", DECODE( TRUNC( G.明細取引区分 / 10), 1, 1, 2, -1, 3, -1 ) * ( G.金額 ), 0)) over (partition by A.得意先CD) 標準税率対象金額合計" */
                    /* + ",sum(decode(B.消費税率, " + str( zeiritsu2 ) + ", DECODE( TRUNC( G.明細取引区分 / 10), 1, 1, 2, -1, 3, -1 ) * ( G.金額 ), 0)) over (partition by A.得意先CD) 軽減税率対象金額合計" */
                    + ",decode(A.請求先CD,A.得意先CD,sum(decode(B.消費税率, " + zeiritsu1.ToString() + ", DECODE( TRUNC( G.明細取引区分 / 10), 1, 1, 2, -1, 3, -1 ) * ( G.金額 ), 0)) over (partition by A.請求先CD),sum(decode(B.消費税率, " + zeiritsu1.ToString() + ", DECODE( TRUNC( G.明細取引区分 / 10), 1, 1, 2, -1, 3, -1 ) * ( G.金額 ), 0)) over (partition by A.得意先CD)) 標準税率対象金額合計"
                    + ",decode(A.請求先CD,A.得意先CD,sum(decode(B.消費税率, " + zeiritsu2.ToString() + ", DECODE( TRUNC( G.明細取引区分 / 10), 1, 1, 2, -1, 3, -1 ) * ( G.金額 ), 0)) over (partition by A.請求先CD),sum(decode(B.消費税率, " + zeiritsu2.ToString() + ", DECODE( TRUNC( G.明細取引区分 / 10), 1, 1, 2, -1, 3, -1 ) * ( G.金額 ), 0)) over (partition by A.得意先CD)) 軽減税率対象金額合計"
                    + ",sum(decode(B.消費税率, 0, DECODE( TRUNC( G.明細取引区分 / 10), 1, 1, 2, -1, 3, -1 ) * ( G.金額 + DECODE( C.名称CD10, '02', DECODE( C.店種区分, 3, G.売仕消費税, G.消費税 ), 0 ) ), 0)) over (partition by A.得意先CD) 非課税対象金額合計"
                    /* 2023.11.08 #71519対応修正 */
                    + ",nvl((select sum(DECODE( TRUNC( t0.取引区分 / 10 ), 1, 1, 2, -1, 9, 1, -1) * DECODE( C.店種区分, 3, ( t0.売仕内税消費税 + t0.売仕外税消費税 ), t0.内税消費税 + t0.外税消費税)) from HC$TRAN_TORI0 t0 where t0.伝票処理区分 in (0,1) and t0.取引先CD1=B.得意先CD and t0.掛計上日 between A.請求開始日 and A.請求終了日 and t0.消費税率=" + zeiritsu1.ToString() + "),0) 標準税額合計"
                    + ",nvl((select sum(DECODE( TRUNC( t0.取引区分 / 10 ), 1, 1, 2, -1, 9, 1, -1) * DECODE( C.店種区分, 3, ( t0.売仕内税消費税 + t0.売仕外税消費税 ), t0.内税消費税 + t0.外税消費税)) from HC$TRAN_TORI0 t0 where t0.伝票処理区分 in (0,1) and t0.取引先CD1=B.得意先CD and t0.掛計上日 between A.請求開始日 and A.請求終了日 and t0.消費税率=" + zeiritsu2.ToString() + "),0) 軽減税額合計"
                    + "";

                }
                else
                {

                    /* 伝票単位 */
                    sql_str += ""
                    /* + ",sum(decode(B.消費税率, " + str( zeiritsu1 ) + ", B.明細金額合計, 0)) over (partition by A.得意先CD) 標準税率対象金額合計" */
                    /* + ",sum(decode(B.消費税率, " + str( zeiritsu2 ) + ", B.明細金額合計, 0)) over (partition by A.得意先CD) 軽減税率対象金額合計" */
                    + ",decode(A.請求先CD,A.得意先CD,sum(decode(B.消費税率, " + zeiritsu1.ToString() + ", B.明細金額合計, 0)) over (partition by A.請求先CD),sum(decode(B.消費税率, " + zeiritsu1.ToString() + ", B.明細金額合計, 0)) over (partition by A.得意先CD)) 標準税率対象金額合計"
                    + ",decode(A.請求先CD,A.得意先CD,sum(decode(B.消費税率, " + zeiritsu2.ToString() + ", B.明細金額合計, 0)) over (partition by A.請求先CD),sum(decode(B.消費税率, " + zeiritsu2.ToString() + ", B.明細金額合計, 0)) over (partition by A.得意先CD)) 軽減税率対象金額合計"
                    + ",sum(decode(B.消費税率, 0, B.明細金額合計, 0)) over (partition by A.得意先CD) 非課税対象金額合計"
                    + ",sum(decode(B.消費税率, " + zeiritsu1.ToString() + ", B.標準税額, 0)) over (partition by A.得意先CD) 標準税額合計"
                    + ",sum(decode(B.消費税率, " + zeiritsu2.ToString() + ", B.軽減税額, 0)) over (partition by A.得意先CD) 軽減税額合計";
                }

                sql_str += ""
                + ",nvl((select max(m.登録番号) from HC$MASTER_SYSKANRI m),'') 自社登録番号"
                + ",decode(C.登録番号, '.', '', '登録番号：'||C.登録番号) 請求先登録番号";

                sql_str += "     , A.入金予定日";
                sql_str += "  FROM (";
                sql_str += "        SELECT A.*";
                sql_str += "             , NVL(( SELECT SUM( H.当月請求額 ) FROM " + table_name + " H,(SELECT 得意先CD, DECODE(請求先CD,'.',得意先CD,請求先CD) 請求先CD FROM HC$master_tokui) I WHERE H.得意先CD = I.得意先CD AND I.請求先CD = A.請求先CD AND H.請求日 < A.請求日 ), 0) AS 前月残";
                sql_str += "          FROM (";
                sql_str += "                SELECT B.請求先CD";
                sql_str += "                     , NVL( B.得意先CD, B.請求先CD ) 得意先CD";
                sql_str += "                     , NVL2( B.得意先CD, 2, 1 ) AS 親子判別用";
                sql_str += "                     , MAX( A.SEQ_NO ) AS SEQ_NO";
                /* sql_str += "                     , MAX( A.請求日 ) AS 請求日"; */
                /* sql_str += "                     , MAX( A.請求開始日 ) AS 請求開始日"; */
                /* sql_str += "                     , MAX( A.請求終了日 ) AS 請求終了日"; */
                /* sql_str += "                     , MAX( A.入金予定日 ) AS 入金予定日"; */
                sql_str += "                     , A.請求日";
                sql_str += "                     , A.請求開始日";
                sql_str += "                     , A.請求終了日";
                sql_str += "                     , A.入金予定日";
                sql_str += "                     , SUM( A.当月請求額 ) AS 当月請求額";
                sql_str += "                     , SUM( A.売上金額 ) AS 売上金額";
                sql_str += "                     , SUM( A.返品金額 + A.値引金額 ) AS 返品値引";
                sql_str += "                     , SUM( A.その他売上 ) AS その他売上";
                sql_str += "                     , SUM( A.消費税 ) AS 消費税";
                sql_str += "                     , SUM( A.現金入金 + A.振込手数料 + A.手形入金 + A.その他入金 ) AS 当月入金額合計";
                sql_str += "                     , SUM( A.相殺入金 ) AS 相殺入金";
                /* sql_str += "                     , MAX( A.請求書NO ) AS 請求書NO"; */
                sql_str += "                     , A.請求書NO";
                sql_str += "                  FROM " + table_name + " A";
                sql_str += "                       INNER JOIN (";
                sql_str += "                        SELECT 得意先CD";
                sql_str += "                             , CASE WHEN 請求先CD = '.' THEN 得意先CD ELSE 請求先CD END AS 請求先CD";
                sql_str += "                             , 締日";
                sql_str += "                          FROM HC$master_tokui";
                sql_str += "                         WHERE 請求印刷 = 1";
                sql_str += "                       ) B ON ( A.得意先CD = B.得意先CD )";
                /* sql_str += "                 WHERE B.締日 = :1"; */
                sql_str += "                 WHERE A.締日 = :1";
                sql_str += "                   AND A.請求日 between :2 AND :3";
                sql_str += "                   AND B.請求先CD between :4 AND :5";
                sql_str += "                 GROUP BY ROLLUP( B.請求先CD, B.得意先CD )";
                sql_str += "                     , A.請求日";
                sql_str += "                     , A.請求開始日";
                sql_str += "                     , A.請求終了日";
                sql_str += "                     , A.入金予定日";
                sql_str += "                     , A.請求書NO";
                sql_str += "                HAVING B.請求先CD != B.得意先CD OR ( B.請求先CD IS NOT NULL AND B.得意先CD IS NULL )";
                sql_str += "               ) A";
                sql_str += "       ) A";
                sql_str += "       LEFT JOIN (";
                /* sql_str += "        SELECT A.得意先CD"; */
                sql_str += "        SELECT DISTINCT A.得意先CD";
                sql_str += "             , B.掛計上日";
                /* sql_str += "             , A.締日"; */
                sql_str += "             , D.締日";
                sql_str += "             , B.SEQ_NO AS 伝票NO";
                sql_str += "             , B.伝票処理区分";
                sql_str += "             , CASE WHEN B.伝票処理区分 = 6 THEN C.明細取引区分 ELSE B.取引区分 END AS 取引区分";
                sql_str += "             , DECODE( B.伝票処理区分, 6, " + AppData.ClassCvnet.comboItem00.GetCaseStr("入金区分", "C.明細取引区分") + " , " + AppData.ClassCvnet.comboItem00.GetCaseStr("共通売上区分", "B.取引区分") + " ) 取引区分名";
                sql_str += "             , DECODE( TRUNC( B.取引区分 / 10 ), 1, 1, 2, -1, -1) * B.数量合計 AS 数量合計";
                sql_str += "             , DECODE( C.伝票処理区分, 6, 0, DECODE( TRUNC( B.取引区分 / 10 ), 1, 1, 2, -1, 9, 1, -1 ) * B.下代合計) AS 明細金額合計";
                sql_str += "             , DECODE( TRUNC( B.取引区分 / 10 ), 1, 1, 2, -1, 9, 1, -1) * DECODE( A.店種区分, 3, ( B.売仕内税消費税 + B.売仕外税消費税 ), B.内税消費税 + B.外税消費税) AS 消費税合計";
                sql_str += "             , B.手入力伝票NO";
                sql_str += "             , B.関連伝票NO2";
                /* sql_str += "             , B.メモ"; */
                sql_str += "             , DECODE( B.伝票処理区分, 6, C.明細メモ, B.メモ ) メモ"; /* 13.03.14 izumichi 入金の場合には「明細メモ」を表示 */
                sql_str += "             , C.行NO";
                sql_str += "             , DECODE( C.伝票処理区分, 6, C.金額, 0 ) AS 金額";
                sql_str += "             , DECODE( TRUNC( B.取引区分 / 10 ), 1, 1, 2, -1, -1 ) * DECODE( A.店種区分, 3, ( C.売仕内税消費税 + C.売仕外税消費税 ), C.内税消費税 + C.外税消費税) AS 明細消費税";
                sql_str += "             , C.明細メモ";
                sql_str += "             , B.請求書NO";
                /* sql_str += "             , TO_CHAR(B.消費税率) || '%' 消費税率"; */
                /* sql_str += "             , CASE WHEN (DECODE(B.伝票処理区分, 6, C.明細取引区分, B.取引区分)>=80) THEN '' ELSE TO_CHAR(B.消費税率) || '%' END 消費税率"; */
                /* sql_str += "             , B.消費税率"; */
                sql_str += "             , CASE WHEN (DECODE(B.伝票処理区分, 6, C.明細取引区分, B.取引区分) = 99) THEN TO_CHAR(B.消費税率) WHEN (DECODE(B.伝票処理区分, 6, C.明細取引区分, B.取引区分) >= 80) THEN '' ELSE TO_CHAR(B.消費税率) END 消費税率";
                /* 2022.05.13 #66849対応追加（消費税関連項目追加） */
                sql_str += "             , " + zeiritsu1.ToString() + " 標準税率";
                sql_str += "             , " + zeiritsu2.ToString() + " 軽減税率";
                sql_str += "             , decode(B.消費税率," + zeiritsu1.ToString() + ",DECODE( TRUNC( B.取引区分 / 10 ), 1, 1, 2, -1, 9, 1, -1) * DECODE( A.店種区分, 3, ( B.売仕内税消費税 + B.売仕外税消費税 ), B.内税消費税 + B.外税消費税),0) 標準税額";
                sql_str += "             , decode(B.消費税率," + zeiritsu2.ToString() + ",DECODE( TRUNC( B.取引区分 / 10 ), 1, 1, 2, -1, 9, 1, -1) * DECODE( A.店種区分, 3, ( B.売仕内税消費税 + B.売仕外税消費税 ), B.内税消費税 + B.外税消費税),0) 軽減税額";

                sql_str += "          FROM HC$master_tokui A";
                sql_str += "               INNER JOIN " + table_name + " D ON ( A.得意先CD = D.得意先CD )";
                sql_str += "               LEFT JOIN HC$tran_tori0 B ON ( A.得意先CD = B.取引先CD1 )";
                sql_str += "               LEFT JOIN HC$tran_tori1 C ON ( B.SEQ_NO = C.ヘッダNO )";
                /* sql_str += "         WHERE A.締日 = :1"; */
                sql_str += "         WHERE D.締日 = :1";
                sql_str += "           AND D.請求日 between :2 AND :3";
                /* sql_str += "           AND A.請求先CD between :4 AND :5"; */
                /* 2019.03.13 #47394対応修正（請求先CDが「.」の場合、出力されない現象） */
                sql_str += "           AND DECODE(A.請求先CD, '.', A.得意先CD, A.請求先CD) between :4 AND :5";
                /* sql_str += "           AND (( B.伝票処理区分 = 0 AND A.店種区分 IN ( 1, 7 ) AND C.行NO = 1 )"; */
                /* 2023.11.20 #71591対応修正（出荷売上の店種区分条件を削除） */
                sql_str += "           AND (( B.伝票処理区分 = 0 AND C.行NO = 1 )";
                sql_str += "                OR ( B.伝票処理区分 = 1 AND A.店種区分 = 3 AND C.行NO = 1 )";
                sql_str += "                OR ( B.伝票処理区分 = 6 AND C.明細取引区分 <> 99)";
                sql_str += "               )";
                sql_str += "       ) B ON ( A.得意先CD = B.得意先CD AND B.掛計上日 BETWEEN A.請求開始日 AND A.請求終了日 )";
                sql_str += "       INNER JOIN HC$master_tokui C ON ( A.請求先CD = C.得意先CD )";
                sql_str += "       CROSS JOIN HC$master_syskanri D";
                sql_str += "       LEFT JOIN HC$master_shain E ON ( C.営業担当CD = E.社員CD ) ";
                sql_str += "       LEFT JOIN HC$master_tokui F ON ( A.得意先CD = F.得意先CD )";

                if (IsOutputUnitProduct == true)
                {
                    /* 商品CD単位 */

                    sql_str += "       LEFT JOIN (";
                    sql_str += "        SELECT ヘッダNO";
                    sql_str += "             , 明細取引区分";
                    sql_str += "             , 商品CD";
                    sql_str += "             , SUM( 下代金額 ) AS 金額";
                    sql_str += "             , SUM( 数量 ) AS 数量";
                    sql_str += "             , SUM( 売仕内税消費税 + 売仕外税消費税 ) AS 売仕消費税";
                    sql_str += "             , SUM( 内税消費税 + 外税消費税 ) AS 消費税";
                    sql_str += "          FROM HC$tran_tori1";
                    sql_str += "         GROUP BY ヘッダNO, 明細取引区分, 商品CD";
                    sql_str += "       ) G ON ( B.伝票NO = G.ヘッダNO AND ( B.伝票処理区分 <> 6 OR ( B.伝票処理区分 = 6 AND B.取引区分 = G.明細取引区分 ) ) )";

                    sql_str += "       LEFT JOIN HC$MASTER_SHOHIN H ON ( G.商品CD = H.商品CD )";
                }
                sql_str += ",HC$MASTER_TOKUI t";
                sql_str += " WHERE NOT ( A.親子判別用 = 2 AND 伝票NO IS NULL )";
                sql_str += "   AND (     A.前月残 <> 0 ";
                sql_str += "         OR (A.前月残 + A.当月請求額) <> 0 ";
                sql_str += "         OR (A.当月入金額合計 + A.相殺入金) <> 0 ";
                sql_str += "         OR  A.返品値引 <> 0 ";
                sql_str += "         OR (A.売上金額 + A.その他売上 + A.消費税) <> 0";
                sql_str += "       )";
                sql_str += " and A.請求先CD=t.得意先CD";

                if (IsOutputUnitProduct == true)
                {
                    /* 商品CD単位 */
                    /* sql_str += " ORDER BY A.請求先CD, A.親子判別用, A.得意先CD, A.請求日, B.掛計上日, B.伝票NO, B.行NO, G.商品CD"; */
                    sql_str += " ORDER BY A.請求先CD, A.請求日, A.親子判別用, A.得意先CD, B.掛計上日, B.伝票NO, B.行NO, G.商品CD";

                    /* qfm_file = "CVPSK003_hin_31f.qfm"; */
                    /* 2022.05.13 #66849対応追加（新レイアウト） */
                    qfm_file = "CVPSK003_hin_31f_v2.qfm";
                }
                else
                {
                    /* 伝票単位 */
                    /* sql_str += " ORDER BY A.請求先CD, A.親子判別用, A.得意先CD, A.請求日, B.掛計上日, B.伝票NO, B.行NO"; */
                    sql_str += " ORDER BY A.請求先CD, A.請求日, A.親子判別用, A.得意先CD, B.掛計上日, B.伝票NO, B.行NO";

                    /* qfm_file = "CVPSK003_31f.qfm"; */
                    /* 2022.05.13 #66849対応追加（新レイアウト） */
                    qfm_file = "CVPSK003_31f_v2.qfm";
                }
            }

            /* var sql_str3 = sql_str; */

            string imagePath = string.Empty;

            string[] wrkPara2 = new string[1];
            wrkPara2[0] = "Data/img";

            // Call SQL
            string retCsv = AppData.Http?.AspxSqlQuery2("get_img_path", wrkPara2, "");

            if (!string.IsNullOrEmpty(retCsv))
            {
                imagePath = retCsv + "\\";
            }

            var sql_str3 = "select A.*, '" + imagePath + "'|| nvl((select decode(m.カナ, '.', '白地.JPG', m.カナ) from HC$MASTER_MEISHO m where m.名称区分 = 'IDX' and m.名称CD = 'IMG'), '') 社判子 from ( " + sql_str + " ) A";

            return sql_str3;

        }

        [RelayCommand]
        async Task DoPrintAsync()
        {
            dateSime = ChooseMenu.DeadLine.ToString();
            qfm_file = "CVPSK003f_v2.qfm";/* (08.08.27) A4サイズ */

            string[] wrkPara = new string[5];

            wrkPara[0] = dateSime ?? "."; // DateSime.OnGet()
            wrkPara[1] = AppData.ClassSatoo.GetDateVal(SelectShop.DateFrom, 0, "0");
            wrkPara[2] = AppData.ClassSatoo.GetDateVal(SelectShop.DateFrom, 1, "0");
            wrkPara[3] = SelectShop.FromWorkerShopCd ?? ".";
            wrkPara[4] = SelectShop.ToWorkerShopCd;

            string ret = OnQuery(wrkPara);

            string ret_csv4 = AppData.Http!.AspxSqlQueryCsv(ret, wrkPara, qfm_file, 0);

            // Check wether data is available or not
            var lines = ret_csv4.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            // Debug
            //ClientLib.MessageBox(this, $"Let see {wrkPara[0]}, {wrkPara[1]}, {wrkPara[2]}, {wrkPara[3]}, {wrkPara[4]}");

            if (ret_csv4.Split('\n').Length < 2 || lines[^1] == "0")
            {
                ClientLib.MessageBoxError(this, "PDFデータがありません");
                return;
            }
            var ret1 = ret_csv4.Split('\n');
            var url = AppData.Http.URLroot + ret1[0] + "/data.pdf";
            await Task.Delay(1500); // PDF生成待ち
            var win = new WebpdfView();
            var vm = win.DataContext as WebpdfViewModel;
            if (vm == null) return;
            vm.Pdfdata = url;
            ClientLib.CursorToNormal();
            ClientLib.ShowDialogView(win, this);
        }

    }
}

