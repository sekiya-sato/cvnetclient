using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Models;
using CvnetClient.Utils;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg09UpallViewModel : BaseViewModel
    {
        #region Declare
        public enum ProcessType { Stock, Account, Payable, All }
        [ObservableProperty]
        SearchCondition? condition;
        #endregion
        #region Initialize
        public void OnInit(object? init_para = null, string? init_flg = null) 
        {
            OnInitBase(init_para, init_flg);
            Condition = new SearchCondition();
            Condition.DateMonthFrom = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            Condition.DateMonthTo = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

        }
        #endregion
        #region Function
        [RelayCommand]
        public void DoExecute()
        {
            var v_chk = AppData.ClassCvnet.GetSime();
            if (DateTime.Parse(v_chk.Substring(0, 4) + "/" + v_chk.Substring(4, 2) + "/" + v_chk.Substring(6, 2)) >= Condition.DateMonthTo)
            {
                ClientLib.MessageBoxError(this, "締日(" + v_chk.Substring(0, 4) + "/" + v_chk.Substring(4, 2) + "/" + v_chk.Substring(6, 2) + ")以前は更新できません。", "エラー");
                return;
            }

            if (!ClientLib.MessageBox(this, "実行しますか？")) return;

            var v_start = DateTime.Now;
            var wrk_para = new string[2];
            wrk_para[0] = Condition.DateFrom?.ToString("yyyyMMdd");
            wrk_para[1] = Condition.DateTo?.ToString("yyyyMMdd");

            var wrk_csv = string.Empty;
            switch (Condition.SelectedProcess)
            {
                case ProcessType.Stock:   /* 在庫 */
                    wrk_csv = AppData.Http!.AspxSqlQuery2("tran_zaiko00", wrk_para);
                    break;
                case ProcessType.Account:  /* 売掛 */
                    wrk_csv = AppData.Http!.AspxSqlQuery2("tran_kakeuri00", wrk_para);
                    break;
                case ProcessType.Payable:  /* 買掛 */
                    wrk_csv = AppData.Http!.AspxSqlQuery2("tran_kakekai00", wrk_para);
                    break;
                case ProcessType.All:  /* 全て */
                    wrk_csv = AppData.Http!.AspxSqlQuery2("tran_zaiall00", wrk_para);
                    break;
            }

            var wrk_mess = "時刻：" + v_start.ToString("HH:mm:ss") + "-" + DateTime.Now.ToString("HH:mm:ss");
            var elapsed = DateTime.Now - v_start;
            wrk_mess += "\n経過時間：" + elapsed.ToString(@"hh\:mm\:ss");
            ClientLib.MessageBoxOk(this, "更新終了しました\n" + wrk_mess, "メッセージ");
        }
        #endregion
        public partial class SearchCondition : ObservableObject 
        {
            [ObservableProperty]
            private ProcessType selectedProcess = ProcessType.Stock;
            [ObservableProperty]
            private DateTime? dateFrom;
            [ObservableProperty]
            private DateTime? dateTo;
            [ObservableProperty]
            private DateTime? dateMonthFrom;
            [ObservableProperty]
            private DateTime? dateMonthTo;

            partial void OnDateMonthFromChanged(DateTime? value)
            {
                if (value is DateTime dt)
                {
                    // Paksa reset masa + ambil hari pertama bulan
                    DateMonthFrom = new DateTime(dt.Year, dt.Month, 1);
                    DateFrom = new DateTime(dt.Year, dt.Month, 1);
                }
                else
                {
                    DateFrom = null;
                }
            }

            partial void OnDateMonthToChanged(DateTime? value)
            {
                if (value is DateTime dt)
                {
                    // Paksa reset masa + ambil hari terakhir bulan
                    var lastDay = DateTime.DaysInMonth(dt.Year, dt.Month);
                    DateMonthTo = new DateTime(dt.Year, dt.Month, 1);
                    DateTo = new DateTime(dt.Year, dt.Month, lastDay);
                }
                else
                {
                    DateTo = null;
                }
            }
        }
    }
}
