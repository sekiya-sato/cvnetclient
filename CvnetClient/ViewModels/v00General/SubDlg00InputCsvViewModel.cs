using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Models;
using CvnetClient.Utils;
using System.Collections.ObjectModel;
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

        /// <summary>
        /// Enable Date Status (0: Not Enable, 1: Enable)
        /// </summary>
        [ObservableProperty]
        public int m_IsDateEnable;

        [ObservableProperty]
        public ObservableCollection<InputCsvItem>? m_InputCsvList;

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

            IsDateEnable = 0;
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

        #region OnChecked Events
        [RelayCommand]
        void DoChkAll()
        { 
            
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

            wrk_csv33.SetColHeader("LineX,Line1,Line0,Line2,Line3,Line4,Line5");
            var wrk_csv = GlobalFunc.ConvertDataTableToListV2<InputCsvItem>(wrk_csv33.GetTable());
             
            var sql_str = "select  c.index_name , c.column_name,constraint_type"
                            + " from user_ind_columns c"
                            + " left join user_indexes i "
                            + " on (c.index_name = i.index_name)"
                            + " left join user_constraints cns "
                            + " on (c.index_name = cns.constraint_name)"
                            + " where  c.table_name=:1"
                            + " AND constraint_type='U'"
                            + " order by  c.index_name , column_position";
            var v_para = new BizArray(); 
            v_para[0] = (wrk_csv.Count != 0) ? wrk_csv.FirstOrDefault().Line1 : string.Empty;
            var csv_tb = AppData.Http?.AspxSqlQuery(sql_str, v_para.ToArray());
            var csv_doc = new BizCsvDocument(csv_tb);
            csv_doc.SetColHeader("Line0,Line1,Line2");
            var ret_csv = GlobalFunc.ConvertDataTableToListV2<InputCsvItem>(csv_doc.GetTable());

            foreach (var row in wrk_csv)
            {
                if (row.Line2 == "NUMBER")
                {
                    row.Line3 = row.Line4;
                    row.Line4 = row.Line5;
                }
                int chkLine1 = ret_csv.Where(x => x.Line1 == row.Line1).Count();
                if (chkLine1 > 0)
                {
                    row.Line6 = "1";
                    row.IsChecked = true; //Set Checkbox True
                    row.IsRowActive = 0; //Set Row Enable False
                    row.SetRowColor = 96;
                } 
            }
            // Remove Row in SEQ_NO, VDATE_CREATE & VDATE_UPDATE
            if (wrk_csv.Count != 0) 
                wrk_csv.RemoveAll(e => e.Line1.ToUpper() == "SEQ_NO" || e.Line1.ToUpper() == "VDATE_CREATE" || e.Line1.ToUpper() == "VDATE_UPDATE");

            InputCsvList = new ObservableCollection<InputCsvItem>(wrk_csv);
        }

        [RelayCommand]
        void DoSelAllCol()
        {
            if (InputCsvList?.Count() == 0) return;

            string chklist = " 商品CD, 略称, "
                    + " 商品名, 展示会CD, ブランドCD, アイテムCD, シーズンCD, 素材CD, "
                    + " デザイナーCD, メーカーCD, 元上代, 上代, 売変日, 原価, 営業原価, "
                    + " 原産国CD, 加工工賃, デリバリー日, 納品日, JANコード1, "
                    + " JANコード2, JANコード3, 洗濯表示, 絵型名, メモ, 消費税計算方法, "
                    + " 在庫管理FLG, 消費税CD, "
                    + "  名称CD01, 名称CD02, 名称CD03, "
                    + "  商品サイズ区分, JAN先頭桁, POS区分, 仕入区分, 消化計算区分, 消化桁切指定, "
                    + " 消化端数区分, 消化掛率, 入力社員CD,  "
                    + " 仕入価格";

            var chkNames = chklist
                            .Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => s.Trim())
                            .ToList();

            var inputList = InputCsvList?.ToList();
            foreach (var item in inputList)
            {
                if (chkNames.Contains(item.Line1?.Trim()))
                    item.IsChecked = true;
                else
                    item.IsChecked = false;
            }
            InputCsvList = new ObservableCollection<InputCsvItem>(inputList);
        }

        [RelayCommand]
        void DoSearchDate()
        {
            if (IsDateEnable == 1)
                IsDateEnable = 0;
            else IsDateEnable = 1;
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

    public partial class InputCsvItem : ObservableObject
    {
        /// <summary>
        /// Data Row Select Status (0: Not Selected, 1: Selected)
        /// </summary>
        [ObservableProperty]
        public bool m_IsChecked;

        /// <summary>
        /// Data Row Enable Status (0: Not Enable, 1: Enable)  
        /// </summary>
        [ObservableProperty]
        public int m_IsRowActive;

        /// <summary>
        /// Data Row Color (0: Default White, 96: Pink)
        /// </summary>
        [ObservableProperty]
        public int m_SetRowColor;
          
        [ObservableProperty]
        public string? m_Line0;

        [ObservableProperty]
        public string? m_Line1;

        [ObservableProperty]
        public string? m_Line2;

        [ObservableProperty]
        public int m_Line3;

        [ObservableProperty]
        public int m_Line4;

        [ObservableProperty]
        public int m_Line5;

        [ObservableProperty]
        public string? m_Line6;

        [ObservableProperty]
        public string? m_Line7;

        public InputCsvItem()
        {
            IsChecked = false;
            IsRowActive = 1;
            SetRowColor = 0;
        }
    }
}
