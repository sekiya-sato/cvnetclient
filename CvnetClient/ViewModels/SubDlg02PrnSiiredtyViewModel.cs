using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Class;
using CvnetClient.Models;
using CvnetClient.Utils;
using CvnetClient.Views;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Windows;
using System.Windows.Media;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg02PrnSiiredtyViewModel : BaseViewModel
    {
        public enum Output
        { PDF = 0, CSV = 1 }

        [ObservableProperty]
        private Output selectedOutput = Output.PDF;

        [ObservableProperty]
        private DateTime codeNen1 = DateTime.Now.Date;

        [ObservableProperty]
        private DateTime codeNen2 = DateTime.Now.Date;

        [ObservableProperty]
        BtListHelper? findWareHouse1 = new();
        [ObservableProperty]
        private string? selectedWareHouse1;

        [ObservableProperty]
        BtListHelper? findWareHouse2 = new();
        [ObservableProperty]
        private string? selectedWareHouse2;

        [ObservableProperty]
        private string? slipNo1;

        [ObservableProperty]
        private string? slipNo2;



        public void OnInit(object? init_para = null, string? init_flg = null)
        {
            OnInitBase(init_para, init_flg);

            para = new BizArray();
            codeNen1 = DateTime.Now.Date;
            codeNen2 = DateTime.Now.Date;
            slipNo1 = ".";
            slipNo2 = "zzzzzzzzzzzzzzzzzzzz";
            selectedOutput = Output.PDF;
        }

        [RelayCommand]
        async Task DoPrint()
        {
            if (!ClientLib.MessageBox(this, "印刷しますか？")) return;
            SelectedWareHouse1 = findWareHouse1.Code;
            SelectedWareHouse2 = findWareHouse2.Code;
            if (string.IsNullOrEmpty(SlipNo1)){ SlipNo1 = "."; }
            if (string.IsNullOrEmpty(SlipNo2)) { SlipNo2 = "zzzzzzzzzzzzzzzzzzzz"; }
            if (string.IsNullOrEmpty(SelectedWareHouse1)) { SelectedWareHouse1 = "."; }
            if (string.IsNullOrEmpty(SelectedWareHouse2)) { SelectedWareHouse2 = "99999999"; }
            string[] wrk_para = new string[12];
            wrk_para[0] = CodeNen1.ToString("yyyyMMdd");
            wrk_para[1] = CodeNen2.ToString("yyyyMMdd");
            wrk_para[2] = selectedWareHouse1;
            wrk_para[3] = SelectedWareHouse2;
            wrk_para[4] = SlipNo1;
            wrk_para[5] = SlipNo2;
            wrk_para[6] = CodeNen1.ToString("yyyyMMdd");
            wrk_para[7] = CodeNen2.ToString("yyyyMMdd");
            wrk_para[8] = selectedWareHouse1;
            wrk_para[9] = SelectedWareHouse2;
            wrk_para[10] = SlipNo1;
            wrk_para[11] = SlipNo2;

            var sql_str = "SELECT A.* FROM ";

            sql_str += " (SELECT '2' 区分,A.手入力伝票NO,A.在庫計上日, ";
            sql_str += " A.取引先CD1 仕入先CD, ";
            sql_str += " MAX(NVL((SELECT B.仕入先名 FROM HC$MASTER_SIIRE B WHERE A.取引先CD1=B.仕入先CD),'')) 仕入先名, ";
            sql_str += " B.商品CD,B.明細名称,0 数量,SUM(DECODE(trunc(A.取引区分/10),1,1,2,-1,3,-1,4,-1,9,1,0)*B.金額) 金額,MAX(B.明細メモ) 摘要,A.SEQ_NO 伝票NO, ";
            sql_str += " '＜＜　諸掛情報 ＞＞' 見出1 ";
            sql_str += " FROM HC$TRAN_TORI0 A,HC$TRAN_TORI1 B ";
            sql_str += " WHERE A.手入力伝票NO!='.' AND A.SEQ_NO=B.ヘッダNO AND A.伝票処理区分=2";
            sql_str += " AND A.在庫計上日 BETWEEN :1 AND :2 AND A.取引先CD1 BETWEEN :3 AND :4 AND A.手入力伝票NO BETWEEN :5 AND :6";
            sql_str += " GROUP BY A.SEQ_NO,A.手入力伝票NO,A.取引先CD1,A.在庫計上日,B.商品CD,B.明細名称 ";

            sql_str += "  UNION ALL";

            sql_str += " SELECT '1' 区分,A.手入力伝票NO,A.在庫計上日,";
            sql_str += " A.取引先CD1 仕入先CD,";
            sql_str += " MAX(NVL((SELECT B.仕入先名 FROM HC$MASTER_SIIRE B WHERE A.取引先CD1=B.仕入先CD),'')) 仕入先名, ";
            sql_str += " '' 商品CD,substr(" + AppData.ClassCvnet.comboItem00.GetCaseStr("商品仕入区分", "A.取引区分") + ",4) 明細名称, ";
            sql_str += " SUM(DECODE(trunc(A.取引区分/10),1,1,2,-1,3,-1,4,-1,9,1,0)*A.数量合計) 数量,SUM(DECODE(trunc(A.取引区分/10),1,1,2,-1,3,-1,4,-1,9,1,0)*A.明細金額合計) 金額, ";
            sql_str += " MAX(A.メモ) 摘要,A.SEQ_NO 伝票NO, ";
            sql_str += " '＜＜　仕入情報 ＞＞' 見出1 ";
            sql_str += " FROM HC$TRAN_TORI0 A";
            sql_str += " WHERE A.手入力伝票NO!='.' AND A.伝票処理区分=3 AND A.在庫計上日 BETWEEN :5 AND :6";
            sql_str += " AND A.取引先CD1 BETWEEN :7 AND :8 AND A.手入力伝票NO BETWEEN :9 AND :10";
            sql_str += " GROUP BY A.SEQ_NO,A.手入力伝票NO,A.取引先CD1,A.在庫計上日,A.取引区分) A";

            sql_str += " ORDER BY A.手入力伝票NO,A.区分,A.在庫計上日,A.仕入先CD,A.伝票NO";

            var qfm_file = "cvnet02prnsiiredty.qfm";

            var ret_csv = AppData.Http!.AspxSqlQueryCsv(sql_str, wrk_para, qfm_file);
            var lines = ret_csv.Split('\n');

            if (lines.Length < 2 || lines[1] == "0")
            {
                ClientLib.MessageBoxError(this, "PDFデータがありません");
                return;
            }

            string pdfPath = lines[0];

            if (SelectedOutput == Output.PDF)
            {

                await Task.Delay(1500); // PDF生成待ち

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
            else
            {
                var csv_para = new BizCsvDocument();
                string datapath = AppData.Http.URLroot + pdfPath + "/data.txt";
                string headpath = AppData.Http.URLroot + pdfPath + "/d_sql.txt";
                bool ready = await Utils.GlobalFunc.WaitForPdfAsync(datapath, TimeSpan.FromSeconds(30));
                if (!ready)
                {
                    ClientLib.MessageBoxError(this, "Data生成に時間がかかりすぎています。\n 条件を絞ってください。");
                    return;
                }

                ready = await Utils.GlobalFunc.WaitForPdfAsync(headpath, TimeSpan.FromSeconds(30));
                if (!ready)
                {
                    ClientLib.MessageBoxError(this, "Header生成に時間がかかりすぎています。\n 条件を絞ってください。");
                    return;
                }
                await csv_para.LoadFromUrlAsync(datapath, headpath);
                try
                {
                    csv_para.SaveCsv(DateTime.Now.ToString("yyyyMMdd") + "_仕入台帳.csv");
                }
                catch (Exception ex) { }
            }
        }

        [RelayCommand]
        public void SelWareHouse1(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null)
            {
                FindWareHouse1.Code = get_sel00.Code;
                FindWareHouse1.Name = get_sel00.Name;
            }
        }

        [RelayCommand]
        public void SelWareHouse2(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null)
            {
                FindWareHouse2.Code = get_sel00.Code;
                FindWareHouse2.Name = get_sel00.Name;
            }
        }

    }
}
