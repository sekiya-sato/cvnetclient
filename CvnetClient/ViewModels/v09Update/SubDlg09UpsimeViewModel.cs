using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Models;
using CvnetClient.Utils;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg09UpsimeViewModel : BaseViewModel
    {
        #region Declare
        [ObservableProperty]
        SearchCondition? condition;
        private BizArray para;
        private BizArray v_flg;
        #endregion
        #region Initialize
        public void OnInit(object? init_para = null, object? init_flg = null) 
        {
            if (init_para != null)
            {
                if (init_para is string s)
                {
                    var v_para = new string[] { s };
                    para = new BizArray(v_para);
                }
                else if (init_para is string[] arr)
                    para = new BizArray(arr);
                else para = new BizArray();
            }
            else para = new BizArray();

            if (init_flg != null)
            {
                if (init_flg is string s)
                {
                    var v_para = new string[] { s };
                    v_flg = new BizArray(v_para);
                }
                else if (init_flg is string[] arr)
                    v_flg = new BizArray(arr);
                else v_flg = new BizArray();
            }
            else v_flg = new BizArray();
            Condition = new SearchCondition();

            var ret_para = AppData.ClassCvnet.GetSime();
            Condition.FirstDate = "前回締日:" + ret_para.Substring(0,4) + "/" + ret_para.Substring(4, 2) + "/" + ret_para.Substring(6, 2);

            var v_sqlstr = "select GET_VDATE(A.VDATE_UPDATE) 更新日,A.入力社員CD||' '||NVL(B.名前,'') 社員 from HC$MASTER_MEISHO A,HC$MASTER_SHAIN B where A.名称区分='SIM' AND A.入力社員CD=B.社員CD(+) ";
            var wrk_csv = AppData.Http!.AspxSqlQuery(v_sqlstr, null);
            if (wrk_csv.Rows.Count > 0)
            {
                Condition.LastDate = "最終更新日:"
                    + wrk_csv.Rows[0][0].ToString().Substring( 0, 4) + "/" + wrk_csv.Rows[0][0].ToString().Substring( 4, 2) + "/" + wrk_csv.Rows[0][0].ToString().Substring(6, 2)
                    + " " + wrk_csv.Rows[0][0].ToString().Substring(9, 2) + ":" + wrk_csv.Rows[0][0].ToString().Substring(11, 2) + ":" + wrk_csv.Rows[0][0].ToString().Substring(13, 2);
                Condition.Person = "最終更新者:" + wrk_csv.Rows[0][1].ToString();
            }
        }
        #endregion
        #region Function
        [RelayCommand]
        public void DoExecute() 
        {
            if (!ClientLib.MessageBox(this, "実行しますか？")) return;

            var v_start = DateTime.Now;

            var wrk_para = new string[3];
            wrk_para[0] = Condition.Date?.ToString("yyyyMMdd") ?? string.Empty;
            wrk_para[1] = new string("1");
            wrk_para[2] = AppData.ClassSatoo.SHAIN_CD ?? ".";
            if (wrk_para[2] == "") {
                wrk_para[2] = ".";
            }
            var wrk_csv = AppData.Http!.AspxSqlQuery2("simeksn", wrk_para, "", 0);

            if (AppData.ClassCvnet.config.pricesvflg == 1)
            {
                var firstDay = new DateTime(Condition.Date.Value.Year, Condition.Date.Value.Month, 1);
                var lastDay = firstDay.AddMonths(1).AddDays(-1);
                var wrk_para1 = new string[2];
                wrk_para1[0] = firstDay.ToString("yyyyMMdd");
                wrk_para1[1] = lastDay.ToString("yyyyMMdd");
                var wrk_csv1 = AppData.Http!.AspxSqlQuery2("pricesave_ksn", wrk_para1, "", 0);
                if (int.Parse(wrk_csv1.Split(',')[0].ToString()) < 0)
                {
                    ClientLib.MessageBoxError(this, "エラーが起きました(" + wrk_csv1.Split(',')[0].ToString());                    
                    return;
                }
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
            private string? person;
            [ObservableProperty]
            private string? lastDate;
            [ObservableProperty]
            private string? firstDate;
            [ObservableProperty]
            private DateTime? date;
        }
    }
}
