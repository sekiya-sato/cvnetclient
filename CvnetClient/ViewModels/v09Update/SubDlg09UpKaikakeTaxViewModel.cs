using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Models;
using CvnetClient.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg09UpKaikakeTaxViewModel : BaseViewModel
    {
        #region Declare
        [ObservableProperty]
        private MasterWorkerShopMenu? selectShop = new();
        [ObservableProperty]
        private string mstName = "支払先登録";   // default not 支払？
        [ObservableProperty]
        BtListHelper findFromWorkerShopCd = new();
        [ObservableProperty]
        BtListHelper findToWorkerShopCd = new();

        private DateTime dateFrom = DateTime.Today;

        private string f_date;
        private string t_date;
        #endregion

        public void OnInit(object? init_para = null, string? init_flg = null)
        {
            OnInitBase(init_para, init_flg);

            FindFromWorkerShopCd = new BtListHelper();
            FindToWorkerShopCd = new BtListHelper();

            SelectShop = new MasterWorkerShopMenu();
            SelectShop.DateFrom = dateFrom;
            // Initialize the value for ToWorkerShopCd
            SelectShop.ToWorkerShopCd = "99999999";
        }

        // Display result
        [RelayCommand]
        async Task DoPrintAsync()
        {
            var v_start = DateTime.Now;
            var elapsed = DateTime.Now - v_start;

            if (!ClientLib.MessageBox(this, "実施しますか？")) return;

            OnQuery();
            string[] wrkPara = new string[4];

            wrkPara[0] = SelectShop.FromWorkerShopCd ?? ".";
            wrkPara[1] = SelectShop.ToWorkerShopCd ?? ".";
            wrkPara[2] = f_date;
            wrkPara[3] = t_date;

            // Debug
            // if (!ClientLib.MessageBox(this, $"削除しますか？ {f_date}, {t_date} ")) return;

            var wrk_csv = string.Empty;

            wrk_csv = AppData.Http!.AspxSqlQuery2("urikake_tax", wrkPara);

            if (string.IsNullOrWhiteSpace(wrk_csv))
            {
                ClientLib.MessageBox(this, "サーバーからの応答がありません。");
                return;
            }

            var lines = wrk_csv.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            var wrk_mess = "時刻：" + v_start.ToString("HH:mm:ss") + "-" + DateTime.Now.ToString("HH:mm:ss");
            elapsed = DateTime.Now - v_start;
            wrk_mess += "\n経過時間：" + elapsed.ToString(@"hh\:mm\:ss");
            ClientLib.MessageBoxOk(this, "更新終了しました\n" + wrk_mess, "メッセージ");
        }

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

        private void OnQuery()
        {
            string sql_str = "select to_char(to_date((substr(to_char(last_day(to_date(D.日付)-to_number(D.締日)),'YYYYMMDD'),0,6)||D.締日))+1,'YYYYMMDD') 開始日,"
                    + "(to_char(last_day(to_date((substr(to_char(last_day(to_date(D.日付)-to_number(D.締日)),'YYYYMMDD'),0,6)||D.締日)))+1,'YYYYMM')||D.締日) 終了日"
                    + " from (select '" + SelectShop.DateFrom + "' 日付,trim(TO_CHAR(自社締日,'09')) 締日 from HC$MASTER_SYSKANRI where 自社締日!=99) D"
                    + " union all select (substr(D.日付,0,6)||'01') 開始日,to_char(last_day(to_date(D.日付)),'YYYYMMDD') 終了日"
                    + " from (select '" + SelectShop.DateFrom + "' 日付,trim(TO_CHAR(自社締日,'09')) 締日 from HC$MASTER_SYSKANRI where 自社締日=99) D";

            var retCsv = AppData.Http!.AspxSqlQueryCsv(sql_str, null);

            if (!string.IsNullOrWhiteSpace(retCsv))
            {
                var lines = retCsv.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

                if (lines.Length > 0)
                {
                    var cells = lines[0].Split(',');

                    if (cells.Length >= 2)
                    {
                        f_date = cells[0].Trim();
                        t_date = cells[1].Trim();
                    }
                }
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
    }
}
