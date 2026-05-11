using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Class;
using CvnetClient.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg00CsvBaihenViewModel : BaseViewModel
    {
        [ObservableProperty]
        ListFlexData listFlexData = new ListFlexData();
        [ObservableProperty]
        public ObservableCollection<InputCSVList>? m_InputCSVList1;
        public void OnInit(object? init_para = null, string? init_flg = null)
        {
            List<CsvItem> def = null;
            OnInitBase(init_para, init_flg);
            ListFlexData.ListConfig = new ListFlexConfig()
            {
                init_csv = def ?? new List<CsvItem>(),
                flag = 1
            };
        }

        [RelayCommand]
        void SearchShop()
        {

            /* パラメータ生成 */
            var wrk_para1 = new BizArray();
            var tokui_para = ListFlexData.GetQueryStr2(wrk_para1, 1, 2, "t.");
            var wrk_para = new BizArray();
            wrk_para[0] = new String(tokui_para);

            TblInit(wrk_para);
            if (InputCSVList1 == null || InputCSVList1.Count == 0)
            {
                //^.OnMess2("該当店舗がありません");
                return;
            }
        }
        void TblInit(BizArray init_para) { 
        
        }
        [RelayCommand]
        void DoExecute()
        {
            if (InputCSVList1 == null || InputCSVList1.Count == 0) return;
            /* 入力エラーチェック */
            if (OnCheckError() < 0) return;
            //if (MessageBox("取込データを追加しますか？", "確認", $OkCancel + $IconExclamation) == CancelSelected) return;

            /* CSV取込 */
            BizArray sv_csv_data = OnGetCsv();
            if (sv_csv_data.Count == 0)
            {
                //^.OnMess2("取込データがありません");
                return;
            }

            /* 取込CSVエラーチェック */           
            var ret_code = OnChkCsv();
            if (ret_code < 0)
            {
                //MessageBox("【エラーデータが存在します】" + ^.csv_err_mess);
                try
                {
                    //var fs = new FileSystem;
                    //var f = fs.SaveDialog("ﾃｷｽﾄ保存", "*.txt", "txt", "SELLERR.txt");
                    //f.Write(^.csv_err_list);
                    //f.close();
                    //Form1.OnMess2("データを保存しました");
                }
                catch (Exception e)
                {
                    //Form1.OnMess2("保存を中止しました");
                }
            }
            else
            {
                /* 更新処理 */
                OnKosin();
            }
        }
        void OnKosin() { 
        
        }
        private static int OnCheckError() { 
            int ret = 0;
            return ret;
        }
        private static int OnChkCsv() { 
            int ret = 0;
            return ret;
        }
        private static BizArray OnGetCsv() 
        { 
            BizArray csvArray = new BizArray();
            return csvArray;
        }
        public partial class InputCSVList : ObservableObject
        {
            [ObservableProperty]
            private bool isChecked;
            [ObservableProperty]
            private string line1;
            [ObservableProperty]
            private string line2;
            [ObservableProperty]
            private DateTime line3;
            [ObservableProperty]
            private DateTime line4;
        }
    }
}
