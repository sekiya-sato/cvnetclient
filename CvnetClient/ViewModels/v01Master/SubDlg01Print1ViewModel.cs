using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg01Print1ViewModel : BaseViewModel
    {
        [ObservableProperty]
        public Dictionary<string, string>? comboList;
        [ObservableProperty]
        private string? selectedCombo;
        [ObservableProperty]
        private string? selectedChange;
        public SubDlg01Print1ViewModel() 
        {
            var sql_str = "SELECT 項目名 FROM (SELECT 項目01 項目名 FROM HC$MASTER_PRT_KANRI P GROUP BY 項目01 ";
            for (var i = 2; i < 90; i++)
            {
                sql_str += " UNION SELECT 項目" + i.ToString("00") + " 項目名 FROM HC$MASTER_PRT_KANRI P GROUP BY 項目" + i.ToString("00");
            }
            sql_str += " )  WHERE 項目名!='.' ORDER BY 項目名 ";
            var ret_csv = AppData.Http!.AspxSqlQuery(sql_str);
            if (ret_csv.Rows.Count != 0)
            {
                foreach(DataRow row in ret_csv.Rows)
                {
                    ComboList.Add(row[0].ToString(), row[0].ToString());
                }
                SelectedCombo = ComboList.FirstOrDefault().Key;
            }           
        }

        [RelayCommand]
        void DoUpdate()
        {
            var wrk_para = new string[2];
            wrk_para[0] = SelectedCombo ?? string.Empty;
            wrk_para[1] = SelectedChange ?? string.Empty;
            var ret = AppData.Http!.AspxSqlQuery2("prt_ksn", wrk_para, "", 00);
            if (int.Parse(ret.Split(",")[0].ToString()) != 0)
            {
                return;
            }
            Close();

        }
    }
}
