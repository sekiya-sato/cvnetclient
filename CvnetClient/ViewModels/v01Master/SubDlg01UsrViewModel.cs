using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Models;
using CvnetClient.Utils;
using CvnetClient.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg01UsrViewModel : BaseViewModel
    {
        [ObservableProperty]
        ObservableCollection<MasterWorker> listWorker;
        [ObservableProperty]
        MasterWorker? selectedWorker;
        [ObservableProperty]
        MasterWorker? editWorker;

        string sql_list = "SELECT * FROM (SELECT SEQ_NO,VDATE_CREATE,VDATE_UPDATE,社員CD,名前 FROM HC$MASTER_SHAIN ORDER BY 社員CD {1}) WHERE ROWNUM <= {2}";

        partial void OnSelectedWorkerChanged(MasterWorker? value)
        {
            if (value != null)
                EditWorker = Common.CloneObject(value);
            else
                EditWorker = null;
        }

        [RelayCommand]
        void DoList() {
            SubList(">=","asc", AppData.maxQueryCnt);
            if(ListWorker == null || ListWorker.Count == 0)
                ClientLib.MessageBoxOk(this, "データがありません");
        }

        void SubList(string sql_P1, string sql_P2, int sql_P3) {
            var sql = string.Format(sql_list, sql_P1, sql_P2, sql_P3);
            var retData = AppData.Http?.AspxSqlQuery(sql);
            if (retData == null || retData.Rows.Count == 0) return;
            var list = (from DataRow dr in retData.Rows
                        select new MasterWorker
                        {
                            SeqNo = Convert.ToInt64(dr["SEQ_NO"]),
                            VdateCreate = Convert.ToDecimal(dr["VDATE_CREATE"]),
                            VdateUpdate = Convert.ToDecimal(dr["VDATE_UPDATE"]),
                            WorkerCD = dr["社員CD"].ToString() ?? string.Empty,
                            Name = dr["名前"].ToString() ?? string.Empty,
                        }).OrderBy(c => c.WorkerCD).ToList();
            Common.ConvertDotStringDel(list);
            ListWorker = new ObservableCollection<MasterWorker>(list);
            if (ListWorker.Count > 0) { 
                SelectedWorker = ListWorker[0];
            }
        }
    }
}
