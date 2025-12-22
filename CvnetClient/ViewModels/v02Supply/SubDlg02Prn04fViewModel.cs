using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Models;
using CvnetClient.Utils;
using CvnetClient.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg02Prn04fViewModel : BaseViewModel
    {
        #region Declare
        [ObservableProperty]
        private MasterShain? chooseMenu;
        [ObservableProperty]
        private MasterWorkerShopMenu? selectShop = new();

        [ObservableProperty]
        public Dictionary<int, string>? shimebi;

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

        [ObservableProperty]
        private string mstName = "請求";   // default

        [ObservableProperty]
        BtListHelper findFromWorkerShopCd = new();
        [ObservableProperty]
        BtListHelper findToWorkerShopCd = new();

        //private DateTime dateFrom = DateTime.Today;

        //private string f_date;
        //private string t_date;
        #endregion

        #region Initialization
        public void OnInit(object? init_para = null, string? init_flg = null)
        {
            OnInitBase(init_para, init_flg);
            ChooseMenu = new MasterShain();
            FindFromWorkerShopCd = new BtListHelper();
            FindToWorkerShopCd = new BtListHelper();

            SelectShop = new MasterWorkerShopMenu();
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

            // Initialize the value for ToWorkerShopCd
            SelectShop.ToWorkerShopCd = "99999999";
            SelectShop.DateFrom = dateFrom;
            SelectShop.OutputBaseline = 0;

            // Output unit: "伝票単位" checked by default
            IsOutputUnitVoucher = true;
            //IsOutputUnitProduct = false;

            // Output type: "親のみ" checked by default
            IsOutputTypeParent = true;

        }
        #endregion

        [RelayCommand]
        private void FindFromWorkerShop(SelValueModel value)
        {
            if (value == null) return;

            FindFromWorkerShopCd = new BtListHelper(value.Code, value.Name);

            SelectShop.FromWorkerShopCd = value.Code;
            SelectShop.FromWorkerShopName = value.Name;
            SelectShop.ToWorkerShopCd = value.Code;
            SelectShop.ToWorkerShopName = value.Name;
            
        }

        [RelayCommand]
        public void FindToWorkerShop(SelValueModel value)
        {
            if (value == null) return;

            FindToWorkerShopCd = new BtListHelper(value.Code, value.Name);

            SelectShop.ToWorkerShopCd = value.Code;
            SelectShop.ToWorkerShopName = value.Name;
        }

        // Display result
        [RelayCommand]
        async Task DoPrint()
        {
            dateSime = ChooseMenu.DeadLine.ToString();
            var qfm_file = "cvpsk001.qfm";

            if (!ClientLib.MessageBox(this, "実施しますか？")) return;

            //OnQuery();
            string[] wrkPara = new string[5];

            wrkPara[0] = dateSime ?? "."; // DateSime.OnGet()
            wrkPara[1] = AppData.ClassSatoo.GetDateVal(SelectShop.DateFrom, 0, "0");
            wrkPara[2] = AppData.ClassSatoo.GetDateVal(SelectShop.DateFrom, 1, "0");
            wrkPara[3] = SelectShop.FromWorkerShopCd ?? ".";
            wrkPara[4] = SelectShop.ToWorkerShopCd;

            //wrkPara[0] = "99";
            //wrkPara[1] = "20211201";
            //wrkPara[2] = "20211231";
            //wrkPara[3] = ".";
            //wrkPara[4] = "99999999";

            string ret = OnQuery(wrkPara);

            string ret_csv4 = AppData.Http!.AspxSqlQueryCsv(ret, wrkPara, qfm_file, 0);

            // Check wether data is available or not
            var lines = ret_csv4.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

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

        private string OnQuery(string[] wrkPara)
        {
            var horyu = 0; 

            var v_kanren1 = "decode(C.関連伝票NO2,0,'',C.関連伝票NO2)";
            var kijun = "";
            if (IsOutputUnitVoucher)
            {
                kijun = "支払日";
                SelectShop.OutputBaseline = 0;
            }
            else if (IsOutputUnitProduct)
            {
                kijun = "支払予定日";
                SelectShop.OutputBaseline = 1;
            }

            /* 倉庫CDを追加 07.01.21 */
            var sql_str2 = "select A.SEQ_NO SIH_SEQ,A.支払予定日,A.当月支払額";
            sql_str2 += ",A.現金支払+A.振込手数料+A.手形支払+A.相殺支払+A.その他支払 当月支払額合計,A.仕入金額,A.返品金額+A.値引金額 返品値引";
            sql_str2 += ",A.その他仕入,A.消費税";
            sql_str2 += ",NVL((select sum(NVL(q.当月支払額,0)) from HC$manage_kakesih Q,(select 仕入先CD,decode(支払先CD,'.',仕入先CD,支払先CD) 支払先CD from HC$master_siire) s";
            sql_str2 += " where q.仕入先CD=s.仕入先CD and s.支払先CD=B.支払先CD and Q.支払日 < A.支払日),0) 前月残";
            sql_str2 += ",B.仕入先CD,B.支払先CD,B.締日";
            sql_str2 += ",C.SEQ_NO 伝票NO";
            sql_str2 += ",C.伝票処理区分,C.掛計上日,DECODE(C.伝票処理区分,7,D.明細取引区分,C.取引区分) 取引区分";
            sql_str2 += ",DECODE(C.伝票処理区分,7," + AppData.ClassCvnet.comboItem00.GetCaseStr("入金区分", "D.明細取引区分");
            sql_str2 += ",2," + AppData.ClassCvnet.comboItem00.GetCaseStr("生地仕入区分", "D.明細取引区分") + "," + AppData.ClassCvnet.comboItem00.GetCaseStr("商品仕入区分", "D.明細取引区分") + ") 取引区分名";
            sql_str2 += ",C.数量合計";
            sql_str2 += ",decode(trunc(C.取引区分/10),1,1,2,-1,3,-1,4,1)*C.明細金額合計 明細金額合計,";
            /* 消費税にも±つける 20061011 */
            sql_str2 += "decode(sign(C.取引区分-79),1,1,DECODE(trunc(C.取引区分/10),1,1,4,1,-1))*(C.内税消費税+C.外税消費税) 消費税合計";
            /* sql_str2+= ",(select t0.手入力伝票NO from HC$tran_tori0 t0 where t0.seq_no=C.関連伝票NO) 手入力伝票NO,C.来勘FLG"; */
            /* 手入力Noを関連2表示に 20070219 */
            /* sql_str2 += ",decode(C.関連伝票NO2,0,'',C.関連伝票NO2) 手入力伝票NO,C.来勘FLG"; */
            sql_str2 += "," + v_kanren1 + " 手入力伝票NO,C.来勘FLG";
            sql_str2 += ",D.行NO,decode(D.伝票処理区分,7,D.金額,0) 金額";
            /* 消費税にも±つける 20061011 */
            sql_str2 += ",decode(sign(D.明細取引区分-79),1,1,DECODE(trunc(D.明細取引区分/10),1,1,4,1,-1))*(D.内税消費税+D.外税消費税) 明細消費税";
            sql_str2 += ",decode(D.明細取引区分,99,'('||C.取引先CD1||')',C.メモ) 備考";
            /* 2014.07.02 #14017対応（自社情報はシステム管理マスタから） */
            sql_str2 += ",E.自社名,E.郵便番号 自社郵便番号,E.住所1 自社住所1,E.住所2 自社住所2,E.住所3 自社住所3,E.TEL 自社TEL,E.FAX 自社FAX,E.振込先1,E.振込先2,E.振込先3";


            sql_str2 += ",(select sum(case when t0.来勘FLG=0 then decode(trunc(t0.取引区分/10),1,1,2,-1,3,-1,4,1,0)*t0.明細金額合計 else 0 end) from HC$tran_tori0 t0";
            sql_str2 += ",(select 仕入先CD,decode(支払先CD,'.',仕入先CD,支払先CD) 支払先CD from HC$master_siire) s";
            sql_str2 += " where ((t0.伝票処理区分=2 and t0.掛計上FLG=1) or (t0.伝票処理区分=3 and t0.掛計上FLG=1)) and t0.掛計上日 between A.支払開始日 and A.支払終了日 and B.支払先CD=S.支払先CD and S.仕入先CD=t0.取引先CD1) 当月勘定";
            sql_str2 += ",(select sum(case when t0.来勘FLG=1 then decode(trunc(t0.取引区分/10),1,1,2,-1,3,-1,4,1,0)*t0.明細金額合計 else 0 end) from HC$tran_tori0 t0";
            sql_str2 += ",(select 仕入先CD,decode(支払先CD,'.',仕入先CD,支払先CD) 支払先CD from HC$master_siire) s";
            sql_str2 += " where ((t0.伝票処理区分=2 and t0.掛計上FLG=1) or (t0.伝票処理区分=3 and t0.掛計上FLG=1)) and t0.掛計上日 between A.支払開始日 and A.支払終了日 and B.支払先CD=S.支払先CD and S.仕入先CD=t0.取引先CD1) 来月勘定";
            sql_str2 += ",C.関連伝票NO 発注NO";
            sql_str2 += " from (select max(q.seq_no) seq_no,s.支払先CD 仕入先CD,q.支払日,q.支払開始日,q.支払終了日,q.支払予定日,sum(q.当月支払額) 当月支払額,sum(q.仕入金額) 仕入金額,sum(q.返品金額) 返品金額";
            sql_str2 += ",sum(q.値引金額) 値引金額,sum(q.その他仕入) その他仕入,sum(q.SMP仕入金額) SMP仕入金額,sum(q.SMP返品金額) SMP返品金額,sum(q.消費税) 消費税,sum(q.現金支払) 現金支払";
            sql_str2 += ",sum(q.振込手数料) 振込手数料,sum(q.手形支払) 手形支払,sum(q.相殺支払) 相殺支払,sum(q.その他支払) その他支払 from HC$manage_kakesih q";
            sql_str2 += ",(select 仕入先CD,decode(支払先CD,'.',仕入先CD,支払先CD) 支払先CD from HC$master_siire) s";
            sql_str2 += " where q.仕入先CD=s.仕入先CD group by s.支払先CD,q.支払日,q.支払開始日,q.支払終了日,q.支払予定日) A";
            /* #5981 支払印刷=1：するのみとする 14.05.22 */
            sql_str2 += ",(select distinct 仕入先CD,decode(支払先CD,'.',仕入先CD,支払先CD) 支払先CD,締日 from HC$master_siire where 支払印刷=1) B,HC$tran_tori0 C,HC$tran_tori1 D,HC$master_syskanri E";
            sql_str2 += " where A.仕入先CD=B.支払先CD and B.仕入先CD=C.取引先CD1";
            /* sql_str2+= " and ((C.伝票処理区分=2 and C.掛計上FLG=1 and D.行NO=1) OR (C.伝票処理区分=3 and C.掛計上FLG=1 and D.行NO=1) OR C.伝票処理区分=7)"; */
            sql_str2 += " and ((C.伝票処理区分=2 and C.掛計上FLG=1 and D.行NO=1) OR (C.伝票処理区分=3 and C.掛計上FLG=1 and D.行NO=1) OR (C.伝票処理区分=7 AND D.明細取引区分<>99))";
            sql_str2 += " and C.掛計上日 between A.支払開始日 and A.支払終了日 AND D.ヘッダNO=C.SEQ_NO";
            sql_str2 += " and B.締日=:1 and A." + kijun + " between :2 and :3 and B.支払先CD between :4 and :5";

            /* 当月取引無し、残あり対応 07.02.13 */
            sql_str2 += " union";
            sql_str2 += " select A.SEQ_NO SIH_SEQ,A.支払予定日";
            sql_str2 += " ,A.当月支払額,A.現金支払+A.振込手数料+A.手形支払+A.相殺支払+A.その他支払 当月支払額合計";
            sql_str2 += " ,A.仕入金額,A.返品金額+A.値引金額 返品値引";
            sql_str2 += " ,A.その他仕入,A.消費税";
            sql_str2 += " ,NVL((select sum(NVL(q.当月支払額,0)) from HC$manage_kakesih Q,(select 仕入先CD,decode(支払先CD,'.',仕入先CD,支払先CD) 支払先CD from HC$master_siire) s";
            sql_str2 += " where q.仕入先CD=s.仕入先CD and s.支払先CD=B.支払先CD and Q.支払日 < A.支払日),0) 前月残";
            sql_str2 += " ,'' 仕入先CD,B.支払先CD,B.締日";
            sql_str2 += (SelectShop.OutputBaseline == 0) ? " ,0 伝票NO" : " ,-1 伝票NO";
            sql_str2 += " ,0 伝票処理区分,'' 掛計上日,0 取引区分";
            sql_str2 += " ,'' 取引区分名,0 数量合計,0 明細金額合計,0 消費税合計,'' 手入力伝票NO,0 来勘FLG";
            sql_str2 += " ,0 行NO,0 金額,0 明細消費税,'' 備考";
            /* 2014.07.02 #14017対応（自社情報はシステム管理マスタから） */
            sql_str2 += ",E.自社名,E.郵便番号 自社郵便番号,E.住所1 自社住所1,E.住所2 自社住所2,E.住所3 自社住所3,E.TEL 自社TEL,E.FAX 自社FAX,E.振込先1,E.振込先2,E.振込先3";

            sql_str2 += " ,0 当月勘定,0 来月勘定,0 発注NO";
            sql_str2 += " from (select max(q.seq_no) seq_no, s.支払先CD 仕入先CD,q.支払日,q.支払開始日,q.支払終了日,q.支払予定日,sum(q.当月支払額) 当月支払額,sum(q.仕入金額) 仕入金額,sum(q.返品金額) 返品金額";
            sql_str2 += " ,sum(q.値引金額) 値引金額,sum(q.その他仕入) その他仕入,sum(q.SMP仕入金額) SMP仕入金額,sum(q.SMP返品金額) SMP返品金額,sum(q.消費税) 消費税,sum(q.現金支払) 現金支払";
            sql_str2 += " ,sum(q.振込手数料) 振込手数料,sum(q.手形支払) 手形支払,sum(q.相殺支払) 相殺支払,sum(q.その他支払) その他支払 from HC$manage_kakesih q";
            sql_str2 += " ,(select 仕入先CD,decode(支払先CD,'.',仕入先CD,支払先CD) 支払先CD from HC$master_siire) s";
            sql_str2 += " where q.仕入先CD=s.仕入先CD group by s.支払先CD,q.支払日,q.支払開始日,q.支払終了日,q.支払予定日) A";
            /* #5981 支払印刷=1：するのみとする 14.05.22 */
            sql_str2 += " ,(select 支払先CD,max(締日) 締日 from (select decode(支払先CD,'.',仕入先CD,支払先CD) 支払先CD,締日 from HC$master_siire where 支払印刷=1) group by 支払先CD) B,HC$master_syskanri E";
            sql_str2 += " where A.仕入先CD=B.支払先CD";
            sql_str2 += " and B.締日=:1 and A." + kijun + " between :2 and :3 and B.支払先CD between :4 and :5";
            /* sql_str2+= " and (A.当月支払額+A.現金支払+A.振込手数料+A.手形支払+A.相殺支払+A.その他支払+A.仕入金額+A.返品金額+A.値引金額+A.その他仕入+A.消費税=0)"; */
            sql_str2 += " and (A.当月支払額=0 AND A.現金支払+A.振込手数料+A.手形支払+A.相殺支払+A.その他支払=0 AND A.仕入金額+A.返品金額+A.値引金額+A.その他仕入+A.消費税=0)";

            string sql_str = "select A.*,A.前月残-A.当月支払額合計 繰越金額,A.前月残-A.当月支払額合計+A.仕入金額-A.返品値引+A.消費税+A.その他仕入 当月残高,B.仕入先名 支払先名,B.郵便番号 支払先郵便番号,B.住所1 支払先住所1,B.住所2 支払先住所2,B.住所3 支払先住所3,'" + horyu.ToString() + "' 支払保留額";

            sql_str += ",nvl(C.倉庫CD,'') 倉庫CD,nvl(r.得意先名,'') 店舗名";
            sql_str += " from (" + sql_str2 + ") A,HC$master_siire B,hc$tran_tori0 C,hc$master_tokui r where A.支払先CD=B.仕入先CD and A.伝票NO=C.SEQ_NO(+) and c.倉庫cd=r.得意先cd(+)"
                    + " and (A.前月残<>0 or (A.前月残+A.当月支払額)<>0 or A.当月支払額合計<>0 or A.返品値引<>0 or (A.仕入金額+A.その他仕入+A.消費税)<>0)";
            /* + " and (A.前月残<>0 or (A.前月残+A.当月請求額)<>0 or A.当月入金額合計<>0 or A.返品値引<>0 or (A.売上金額+A.その他売上+A.消費税)<>0)" */
            sql_str += " order by A.支払先CD,A.支払予定日,A.掛計上日,A.手入力伝票NO,A.発注NO,A.伝票NO,行NO";

            return sql_str;
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
            [ObservableProperty]
            private int? outputBaseline;
            [ObservableProperty]
            private int? outputUnit;
            [ObservableProperty]
            private int? reservePayment;
        }

        public partial class MasterShain : ObservableObject
        {
            [ObservableProperty]
            private int? deadLine;
            [ObservableProperty]
            private string? deadLineString;
        }
    }
}
