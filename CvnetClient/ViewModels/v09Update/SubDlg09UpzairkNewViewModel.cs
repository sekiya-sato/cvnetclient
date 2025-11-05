using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Models;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg09UpzairkNewViewModel : BaseViewModel
    {
        public enum CalcType { 再計算,指定月数以前在庫0除外 }
        [ObservableProperty]
        SearchCondition? condition;
        public void OnInit() 
        { 
            Condition = new SearchCondition();

            Condition.Date1 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            Condition.Date2 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-1);
            Condition.Day = "12";
            Condition.Date3 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(int.TryParse(Condition.Day, out var _day) ? _day : 12 * -1);
        }

        [RelayCommand]
        public void DoExecute() 
        {
            var v_start = DateTime.Now;
            var wrk_para = new string[4];
            wrk_para[0] = Condition.Date1?.ToString("yyyyMMdd");
            wrk_para[1] = Condition.Date2?.ToString("yyyyMMdd");
            wrk_para[2] = Condition.Date3?.ToString("yyyyMMdd");
            if (Condition.SelectedCalc == CalcType.再計算)
            {
                wrk_para[3] = "0";
            }
            else 
            {
                wrk_para[3] = "1";
            }

            var wrk_csv = AppData.Http!.AspxSqlQuery2("tran_zaikork_new", wrk_para);

            var wrk_mess = "時刻：" + v_start.ToString("HH:mm:ss") + "-" + DateTime.Now.ToString("HH:mm:ss");
            var elapsed = DateTime.Now - v_start;
            wrk_mess += "\n経過時間：" + elapsed.ToString(@"hh\:mm\:ss");
            ClientLib.MessageBoxOk(this, "更新終了しました\n" + wrk_mess, "メッセージ");
        }
        public partial class SearchCondition : ObservableObject 
        {
            [ObservableProperty]
            private DateTime? date1;
            [ObservableProperty]
            private DateTime? date2;
            [ObservableProperty]
            private DateTime? date3;
            [ObservableProperty]
            private string? day;
            [ObservableProperty]
            private CalcType selectedCalc = CalcType.指定月数以前在庫0除外;

            partial void OnDate1Changed(DateTime? value)
            {
                if (value is DateTime dt)
                {
                    Date2 = new DateTime(dt.Year, dt.Month, 1).AddMonths(-1);
                    Date3 = new DateTime(dt.Year, dt.Month, 1).AddMonths(int.TryParse(Day, out var _day) ? _day : 0 * -1);
                }
                else 
                {
                    Date2 = null;
                    Date3 = null;
                }
            }

            partial void OnDayChanged(string? value) 
            {
                if (Date1 is DateTime dt)
                {
                    int day = int.TryParse(value, out var _day) ? _day : 0;
                    Date3 = new DateTime(dt.Year, dt.Month, 1).AddMonths(day * -1);
                }
                else 
                {
                    Date3 = null;
                }
                
            }
        }
    }
}
