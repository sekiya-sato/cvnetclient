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
        private string mstName = "請求";   // default

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
            string sql_str = "SELECT TO_CHAR(TRUNC(ADD_MONTHS(TO_DATE(D.日付, 'YYYYMMDD'), -1), 'MM') + D.締日, 'YYYYMMDD') AS 開始日,"
                        + "TO_CHAR(TRUNC(TO_DATE(D.日付, 'YYYYMMDD'), 'MM') + D.締日 - 1, 'YYYYMMDD') AS 終了日 "
                        + "FROM(SELECT '20251201' AS 日付, 自社締日 AS 締日 "
                        + "FROM HC$MASTER_SYSKANRI WHERE 自社締日 != 99) D "
                        + "UNION ALL "
                        + "SELECT TO_CHAR(TRUNC(TO_DATE(D.日付, 'YYYYMMDD'), 'MM'), 'YYYYMMDD') AS 開始日,"
                        + "TO_CHAR(LAST_DAY(TO_DATE(D.日付, 'YYYYMMDD')), 'YYYYMMDD') AS 終了日 FROM (SELECT '20251201' AS 日付 FROM HC$MASTER_SYSKANRI WHERE 自社締日 = 99) D";

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
