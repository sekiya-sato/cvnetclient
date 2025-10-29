using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CvnetClient.ViewModels.SubDlg09Upkeihi2ViewModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg09Upkeihi2ViewModel : BaseViewModel
    {
        public enum PriceType { しない, する }
        [ObservableProperty]
        SearchCondition? condition;
        [ObservableProperty]
        public Dictionary<string, string>? comboListHow;
        public void OnInit() 
        { 
            Condition = new SearchCondition();

            ComboListHow = new Dictionary<string, string>
            {
                { "1",   "1 数量平均" },
                { "2",   "2 数量比率" },
                { "3",   "3 金額比率" }
            };
            Condition.ComboBox = ComboListHow.FirstOrDefault().Key;

            Condition.Date = DateTime.Now;

            Condition.DateFrom = new DateTime(Condition.Date.Value.Year, Condition.Date.Value.Month, 1);
            Condition.DateTo = Condition.DateFrom?.AddMonths(1).AddDays(-1);
        }
        [RelayCommand]
        public void DoExecute()
        {
            var v_chk = AppData.ClassCvnet.GetSime();
            if (DateTime.Parse(v_chk.Substring(0,4) + "/" + v_chk.Substring(4, 2) + "/" + v_chk.Substring(6, 2)) >= Condition.DateFrom)
            {
                ClientLib.MessageBoxError(this,"締日(" + v_chk.Substring(0, 4) + "/" + v_chk.Substring(4, 2) + "/" + v_chk.Substring(6, 2) + ")以前は更新できません。","エラー");
                return;
            }
            if (!ClientLib.MessageBox(this, "実行しますか？", "確認")) return;

            var v_start = DateTime.Now;

            var wrk_para = new string[6];
            wrk_para[0] = Condition.ComboBox.ToString() ?? string.Empty;
            wrk_para[1] = Condition.Date?.ToString("yyyyMMdd") ?? string.Empty;
            if (Condition.SelectedPrice == PriceType.しない) {
                wrk_para[2] = "0";
            }
            else {
                wrk_para[2] = "1";
            }
            wrk_para[3] = AppData.ClassSatoo.SHAIN_CD ?? string.Empty;
            wrk_para[4] = Condition.DateFrom?.ToString("yyyyMMdd") ?? string.Empty;
            wrk_para[5] = Condition.DateTo?.ToString("yyyyMMdd") ?? string.Empty;

            var wrk_csv = AppData.Http!.AspxSqlQuery2("keihiksn", wrk_para, "", 0);

            if (int.Parse(wrk_csv.Split(',')[0].ToString()) < 0)
            {
                ClientLib.MessageBoxError(this, "エラーが起きました(" + wrk_csv.Split(',')[0].ToString() + "," + wrk_csv.Split(',')[1].ToString() + ")");
            }
            else
            {
                var wrk_mess = "時刻：" + v_start.ToString("HH:mm:ss") + "-" + DateTime.Now.ToString("HH:mm:ss");
                var elapsed = DateTime.Now - v_start;
                wrk_mess += "\n経過時間：" + elapsed.ToString(@"hh\:mm\:ss");
                ClientLib.MessageBoxOk(this, "更新終了しました\n" + wrk_mess, "メッセージ");
            }

        }

        public partial class SearchCondition : ObservableObject 
        {
            [ObservableProperty]
            private PriceType selectedPrice = PriceType.しない;
            [ObservableProperty]
            private string? comboBox;
            [ObservableProperty]
            private string? textBox;
            [ObservableProperty]
            private DateTime? dateFrom;
            [ObservableProperty]
            private DateTime? dateTo;
            [ObservableProperty]
            private DateTime? date;
        }
    }
}
