using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Models;
using CvnetClient.Utils;
using System.Data;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg00InputCsvViewModel : BaseViewModel
    {
        #region Variables 

        [ObservableProperty]
        public string? m_SelKubunName;

        [ObservableProperty]
        public string? m_DspKubunName;

        [ObservableProperty]
        public int m_DspRowCnt;

        private BizArray para;
        #endregion

        #region Combobox List  
        [ObservableProperty]
        public Dictionary<string, InputTbItem>? m_ImpSelKubun;
        #endregion

        public void OnInit(object? init_para = null)
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

            #region Set Combobox Value
            BizCsvDocument wrk_csv;
            if (AppData.ClassCvnet.SysCnt == null ||
                AppData.ClassCvnet.SysCnt?.Columns.Count == 0 ||
                AppData.ClassCvnet.SysCnt?.Rows.Count == 0)
            {
                /* 概算でよい場合はcount2,正確に集計する場合はcount */
                string get_csv = AppData.Http?.AspxSqlQuery2("count2", null);

                wrk_csv = new BizCsvDocument(get_csv);
                if (AppData.ClassCvnet.SysCnt == null)
                    AppData.ClassCvnet.SysCnt = new DataTable();
                else
                {
                    AppData.ClassCvnet.SysCnt.Clear();
                    AppData.ClassCvnet.SysCnt = wrk_csv.GetTable();
                }
            }
            else wrk_csv = new BizCsvDocument(AppData.ClassCvnet.SysCnt);

            var _kubunList = new Dictionary<string, InputTbItem>(); 
            foreach(DataRow row in wrk_csv.GetTable().Rows)
            {
                InputTbItem item = new InputTbItem();
                item.tb_cnt = int.TryParse(row[2]?.ToString(), out var _cnt) ? _cnt : 0;
                item.tb_file = row[3]?.ToString() ?? string.Empty;

                string key = row[1]?.ToString() ?? string.Empty;
                if (!string.IsNullOrEmpty(key))
                {
                    if (!_kubunList.ContainsKey(key))
                        _kubunList[key] = item;
                }
            }
            ImpSelKubun = _kubunList;
            SelKubunName = ImpSelKubun.FirstOrDefault().Key;             
            #endregion
        }

        #region OnChange Events
        partial void OnSelKubunNameChanged(string? value)
        {
            if (string.IsNullOrEmpty(value) || ImpSelKubun?.Count == 0) return;
            InputTbItem selItem = ImpSelKubun[value];
            DspKubunName = selItem.tb_file;
            DspRowCnt = selItem.tb_cnt;
        }
        #endregion

        #region Button Events
        [RelayCommand]
        void DoSearch()
        {
            if (string.IsNullOrEmpty(SelKubunName) || ImpSelKubun?.Count == 0) return;
            /* スキーマ情報取得&SET */
            var wrk_para = new BizArray();
            wrk_para[0] = "1"; 
            wrk_para[1] = $"HC${ImpSelKubun[SelKubunName].tb_file.Trim().ToUpper()}";
            var get_csv = AppData.Http?.AspxSqlQuery2("db_schema", wrk_para.ToArray());
            var wrk_csv33 = new BizCsvDocument(get_csv);
            foreach (DataRow row in wrk_csv33.GetTable().Rows)
            { 
                
            }
        }

        [RelayCommand]
        void DoSelAllCol()
        { 
            
        }

        [RelayCommand]
        void DoSearchData()
        { 
            
        }

        [RelayCommand]
        void DoExecute()
        { 
        
        }

        [RelayCommand]
        void DoExit()
        {
            ClientLib.ExitDialogResult(this, true);
        }
        #endregion
    }

    public class InputTbItem
    {
        public int tb_cnt { get; set; } = 0;
        public string tb_file { get; set; } = string.Empty;
    }

    public partial class TableCol : ObservableObject
    {
        [ObservableProperty]
        public string? m_Line0;

        [ObservableProperty]
        public string? m_Line1;

        [ObservableProperty]
        public string? m_Line2;

        [ObservableProperty]
        public string? m_Line3;

        [ObservableProperty]
        public string? m_Line4;

        [ObservableProperty]
        public string? m_Line5;
    }
}
