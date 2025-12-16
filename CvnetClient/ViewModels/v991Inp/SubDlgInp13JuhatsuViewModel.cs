using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Class;
using CvnetClient.Models;
using CvnetClient.Utils;
using CvnetClient.Views;
using System.Windows;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CvnetClient.ViewModels
{
    public partial class SubDlgInp13JuhatsuViewModel : BaseViewModel
    {
        public enum SettingType {Maker,Input}
        [ObservableProperty]
        ListFlexData listFlexData = new ListFlexData();
        [ObservableProperty]
        SearchCondition? condition;
        [ObservableProperty]
        Visibility showOrNot;
        [ObservableProperty]
        bool onOrNot;
        public void OnInit() 
        {
            Condition = new SearchCondition();
            Condition.CodeNen1 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            Condition.CodeNen2 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddDays(-1);
            Condition.CodeNen3 = DateTime.Now;
            Condition.CodeNen4 = DateTime.Now;
            Condition.CodeTokui1 = new BtListHelper("", "");
            Condition.CodeTokui2 = new BtListHelper("99999999","");
            Condition.CodeDenNo1 = 0;
            Condition.CodeDenNo2 = 99999999;
            Condition.CodeTan1 = new BtListHelper("", "");
            Condition.CodeTan2 = new BtListHelper("99999999","");
            ShowOrNot = Visibility.Visible;
            OnOrNot = true;
            var ret_csv = AppData.ClassCvnet.AspxSqlQueryImp();
            if (ret_csv.Rows.Count > 0) Condition.CodeSoko = new BtListHelper(ret_csv.Rows[0][1].ToString(),ret_csv.Rows[0][2].ToString());

            var v_sqlstr = "select S.仕入先CD,S.仕入先名 from hc$master_siire S,(SELECT 名称CD FROM HC$MASTER_MEISHO WHERE 名称区分='HJ2') M where S.仕入先CD=M.名称CD";
            ret_csv = AppData.Http!.AspxSqlQuery(v_sqlstr, null);
            if (ret_csv.Rows.Count > 0) Condition.CodeSiire = new BtListHelper(ret_csv.Rows[0][0].ToString(),ret_csv.Rows[0][1].ToString());

            List<CsvItem> def = null;
            ListFlexData.ListConfig = new ListFlexConfig()
            {
                init_csv = def ?? new List<CsvItem>(),
                flag = 1
            };

            if (AppData.ClassCvnet.config.UserFlg == 28 || AppData.ClassCvnet.config.UserFlg == 29 || AppData.ClassCvnet.config.UserFlg == 35 || AppData.ClassCvnet.config.UserFlg == 36 || AppData.ClassCvnet.config.UserFlg == 55)
            {
                Condition.SelectedSetting = SettingType.Input;
                ShowOrNot = Visibility.Hidden;
                OnOrNot = false;
            }
        }
        [RelayCommand]
        public void SelSiire(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Condition != null)
            {
                Condition.CodeSiire = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        [RelayCommand]
        public void SelSoko(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Condition != null)
            {
                Condition.CodeSoko = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }

        [RelayCommand]
        public void SelTokui1(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Condition != null)
            {
                Condition.CodeTokui1 = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }

        [RelayCommand]
        public void SelTokui2(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Condition != null)
            {
                Condition.CodeTokui2 = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }


        [RelayCommand]
        public void SelUser1(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Condition != null)
            {
                Condition.CodeTan1 = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }

        [RelayCommand]
        public void SelUser2(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Condition != null)
            {
                Condition.CodeTan2 = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }

        [RelayCommand]
        void DoExecute() 
        {
            /* エラーチェック */
            if (OnCheckError() < 0) return;
            if (OnCheckMst() < 0) return;

            /* 伝票作成処理 */
            if (!ClientLib.MessageBox(this,"指定内容に該当する受注伝票から発注伝票を作成します、よろしいですか？")) return;

            var v_start = DateTime.Now;
            
            var wrk_para = new string[14];
            wrk_para[0] = Condition.CodeNen1.ToString("yyyyMMdd");
            wrk_para[1] = Condition.CodeNen2.ToString("yyyyMMdd");
            wrk_para[2] = Condition.CodeSoko.Code;
            wrk_para[3] = Condition.CodeNen3.ToString("yyyyMMdd");
            wrk_para[4] = Condition.CodeNen4.ToString("yyyyMMdd");
            wrk_para[5] = Condition.CodeSiire?.Code ?? string.Empty;

            /* 追加 10.11.12 */
            var wrk_para2 = new BizArray();
            var bunrui_sql = ListFlexData.GetQueryStr2(wrk_para2, 1, 0, "S.");

            wrk_para[6] = Condition.CodeTokui1?.Code ?? string.Empty;
            wrk_para[7] = Condition.CodeTokui2?.Code ?? string.Empty;
            wrk_para[8] = bunrui_sql.ToString();
            wrk_para[9] = Condition.CodeDenNo1.ToString();
            wrk_para[10] = Condition.CodeDenNo2.ToString();
            wrk_para[11] = Condition.CodeTan1?.Code ?? string.Empty;
            wrk_para[12] = Condition.CodeTan2?.Code ?? string.Empty;

            if (Condition.SelectedSetting == SettingType.Maker) { 
                wrk_para[13] = "0";
            }
            else
            {
                wrk_para[13] = "1";
            }

            var wrk_csv = AppData.Http!.AspxSqlQuery2("juchu_hachu", wrk_para, "", 55);
            if (int.Parse(wrk_csv.Split(",")[0].ToString()) == -99)
            {
                ClientLib.MessageBoxError(this,"データが存在しません");
                return;
            }
            else if (int.Parse(wrk_csv.Split(",")[0].ToString()) < 0)
            {
                ClientLib.MessageBoxError(this,"受注更新明細件数を更新できませんでした");
                return;
            }
            ClientLib.MessageBoxOk(this,"受注更新明細件数を更新しました ( " + wrk_csv.Split(",")[1].ToString() + " 件 )");
        }
        int OnCheckError() 
        {
            if (Condition.CodeSoko.Code == "" || Condition.CodeSoko == null)
            {
                ClientLib.MessageBoxError(this, "入庫倉庫を入力して下さい！");
                return -1;
            }
            if (Condition.SelectedSetting == SettingType.Input)
            {
                if (Condition.CodeSiire.Code == "" || Condition.CodeSiire == null)
                {
                    ClientLib.MessageBoxError(this,"仕入先を入力して下さい！");
                    return -1;
                }
            }
            return 0;
        }
        int OnCheckMst()
        {
            var v_sqlstr = "select 得意先CD,得意先名 from hc$master_tokui where 得意先CD =:1 and (店種区分=0 OR 倉庫区分=9)" + AppData.ClassCvnet.GetQueryStrHoujin().ToString();
            var v_array = new BizArray();
            v_array[0] = Condition.CodeSoko.Code;
            var wrk_csv = AppData.Http!.AspxSqlQuery(v_sqlstr, v_array.ToArray());
            if (wrk_csv.Rows.Count > 0)
            {
                Condition.CodeSoko = new BtListHelper(wrk_csv.Rows[0][0].ToString(), wrk_csv.Rows[0][1].ToString());
            }
            else
            {
                Condition.CodeSoko = new BtListHelper("", "");
                ClientLib.MessageBoxError(this,"倉庫CDがマスタに存在しません｡");
                return -1;
            }

            if (Condition.SelectedSetting == SettingType.Input)
            {
                v_sqlstr = "select 仕入先CD,仕入先名 from hc$master_siire where 仕入先CD=:1" + AppData.ClassCvnet.GetQueryStrHoujin().ToString();
                v_array = new BizArray();
                v_array[0] = Condition.CodeSiire.Code;
                wrk_csv = AppData.Http!.AspxSqlQuery(v_sqlstr, v_array.ToArray());
                if (wrk_csv.Rows.Count > 0)
                {
                    Condition.CodeSiire = new BtListHelper(wrk_csv.Rows[0][0].ToString(), wrk_csv.Rows[0][1].ToString());
                }
                else
                {
                    Condition.CodeSiire = new BtListHelper("", "");
                    ClientLib.MessageBoxError(this, "仕入先CDがマスタに存在しません｡");
                    return -1;
                }
            }
            return 0;
        }

        public partial class SearchCondition : ObservableObject
        {
            [ObservableProperty]
            private BtListHelper? codeSoko;
            [ObservableProperty]
            private BtListHelper? codeSiire;
            [ObservableProperty]
            private DateTime codeNen1;
            [ObservableProperty]
            private DateTime codeNen2;
            [ObservableProperty]
            private BtListHelper? codeTokui1;
            [ObservableProperty]
            private BtListHelper? codeTokui2;
            [ObservableProperty]
            private long codeDenNo1;
            [ObservableProperty]
            private long codeDenNo2;
            [ObservableProperty]
            private BtListHelper? codeTan1;
            [ObservableProperty]
            private BtListHelper? codeTan2;
            [ObservableProperty]
            private DateTime codeNen3;
            [ObservableProperty]
            private DateTime codeNen4;
            [ObservableProperty]
            private SettingType selectedSetting = SettingType.Maker;
        }
    }
}
