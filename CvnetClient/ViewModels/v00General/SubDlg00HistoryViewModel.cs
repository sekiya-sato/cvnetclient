using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Class;
using CvnetClient.Interface;
using CvnetClient.Models;
using CvnetClient.Service;
using CvnetClient.Utils;
using CvnetClient.Views;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg00HistoryViewModel : BaseViewModel
    {
        [ObservableProperty] string? title = "メンテナンス : LOGIN履歴情報(管理者用)";

        [ObservableProperty] ObservableCollection<TranRireki>? rirekiList;
        [ObservableProperty] RirekiSearchOpt? m_SearchOpt;
        [ObservableProperty] private TranRireki? selectedRireki;
        private BizArray para;
        [ObservableProperty] private long? firstSeqNo;   
        [ObservableProperty] private long? lastSeqNo;    

        public void OnInit(object? init_para = null, string? init_flg = null)
        {
            OnInitBase(init_para, init_flg);
            para = new BizArray();
            RirekiList = new ObservableCollection<TranRireki>();
            SelectedRireki = new TranRireki();
            SearchOpt = new RirekiSearchOpt();
        }

        [RelayCommand]
        public void DoSearch()
        {
            ExecuteSearch(0, null);
        }

        [RelayCommand]
        public void NextPage()
        {
            ExecuteSearch(1, LastSeqNo);
        }

        [RelayCommand]
        public void BackPage()
        {
            ExecuteSearch(-1, FirstSeqNo);
        }

        [RelayCommand]
        void DoExit()
        {
            ClientLib.ExitDialogResult(this, true);
        }

        [RelayCommand]
        public void ToggleChkDate()
        {
            SearchOpt.ChkDate = !SearchOpt.ChkDate;
        }

        //[RelayCommand]
        //public void SelPrint()
        //{
        //    /* ダイアログ検索条件を追加 */
        //    if (AppData.ClassCvnet.MstDialog.ContainsKey("商品"))
        //    {
        //        var ar = new string[] { "1" };
        //        var vm = AppData.DlgService.GetSelSho(AppData.ClassCvnet.MstDialog["商品"].v_mstname, null, ar);
        //        if (vm != null)
        //        {
        //            //vm.SelShoResult0 
        //            PageView(vm.SelShoResult1.Item2, 0, vm.SelShoResult1.Item1);
        //        }

        //        return;
        //    }

        //    var v_para = new string[] { SelProductCd };
        //}
        private string GetSql(long? v_no, int v_flg)
        {
            string sql_str = SubGetSql(v_no, v_flg);
            return AppData.ClassCvnet.GetSqlDisp(sql_str, 24);
        }
        private string SubGetSql(long? v_no, int v_flg)
        {
            string sort = (v_flg == 1) ? "asc" : "desc";
            string sql = "select A.LOGIN時間,A.社員CD";
            sql += ",DECODE(NVL(GET_KUBUN(A.USER_ID),-1),0,"
                + "NVL((SELECT B.名前 from HC$master_shain B where A.社員CD=B.社員CD),' '),"
                + "1,NVL(D.仕入先名,' ')) 名前";
            sql += ",A.REMOTE_ADDR,最終ACCESS時間,A.MESS,A.HTTP_USER_AGENT";
            sql += ",A.USER_ID,A.RAND_ID,A.SEQ_NO,NVL(GET_KUBUN(A.USER_ID),-1) 区分";
            sql += ",NVL(C.得意先CD,'.') 店舗CD,NVL(C.得意先名,'.') 店舗名";
            sql += ",DECODE(NVL(GET_KUBUN(A.USER_ID),-1),1,NVL(D.TEL,'.'),NVL(C.TEL,'.')) 店舗TEL";
            sql += " from HC$tran_rireki A";
            sql += " left outer join HC$master_shain B on (A.社員CD=B.社員CD)";
            sql += " left outer join HC$master_tokui C on (C.得意先CD=B.店舗CD)";
            sql += " left outer join HC$master_siire D on (D.仕入先CD=A.社員CD)";
            sql += " where NOT(A.MESS LIKE '%DEVELOP0%') and NOT(A.MESS LIKE '%DTP150%')";

            // Apply filters from SearchOpt
            if (SearchOpt.ChkUserOnly)
                sql += " and A.USER_ID>=0 and A.社員CD!='.'";

            if (SearchOpt.ChkNotTN)
                sql += " and NVL(substr(A.MESS,instr(A.MESS,'___',1,5)+3,2),'.') NOT IN ('TN')";

            if (SearchOpt.ChkNotDaTe)
                sql += " and NVL(substr(A.MESS,instr(A.MESS,'___',1,5)+3,2),'.') NOT IN ('Da','.','te')";

            if (SearchOpt.ChkBizBrowser)
                sql += " and A.HTTP_USER_AGENT='Biz/Browser'";

            if (SearchOpt.ChkDate)
                sql += $" and get_vdate(login時間) like '{SearchOpt.SearchDate}%'";

            if (v_no != null)
            {
                if (v_flg == 1)
                    sql += $" and A.SEQ_NO>={v_no}";
                else
                    sql += $" and A.SEQ_NO<={v_no}";
            }

            sql += $" order by A.SEQ_NO {sort}";
            return sql;
        }
        private void ExecuteSearch(int direction, long? v_no)
        {
            int v_flg = 0;

            if (direction == 1) 
                v_flg = 1;

            if (direction == -1)   
                v_flg = 0;

            string sql = GetSql(v_no, v_flg);

            var ret = AppData.Http?.AspxSqlQuery(sql, new string[] { });
            var dt = ret ?? new DataTable();

            RirekiList.Clear();

            foreach (DataRow dr in dt.Rows)
            {
                var item = new TranRireki
                {
                    LoginTime = VDateHelper.FromVDate(Convert.ToDecimal(dr["LOGIN時間"])),
                    EmployeeCD = dr["社員CD"].ToString() ?? ".",
                    EmployeeName = dr["名前"].ToString() ?? ".",
                    RemoteADDR = dr["REMOTE_ADDR"].ToString() ?? "",
                    LastAccessTime = VDateHelper.FromVDate(Convert.ToDecimal(dr["最終ACCESS時間"])),
                    StoreCD = dr["店舗CD"].ToString() ?? ".",
                    StoreName = dr["店舗名"].ToString() ?? ".",
                    Tel = dr["店舗TEL"].ToString() ?? ".",
                    GetKubun = Convert.ToInt64(dr["区分"]),
                    Mess = dr["MESS"].ToString() ?? ".",
                    Httpuseragent = dr["HTTP_USER_AGENT"].ToString() ?? "",
                    UserID = Convert.ToInt64(dr["USER_ID"]),
                    RandID = dr["RAND_ID"].ToString() ?? "",
                    SeqNo = Convert.ToInt64(dr["SEQ_NO"]),
                };

                item.Shop = $"{item.StoreCD} {item.StoreName}";

                string txt = item.Mess;
                string[] arr = txt.Split(new[] { "___" }, StringSplitOptions.None);

                if (arr.Length > 4)
                {
                    item.Winusername = arr[0];
                    item.Winmachinename = arr[1];
                    item.Cpu = arr[2];
                    item.Os = arr[3];
                    item.Bizver = arr[4];

                    if (arr.Length > 5 && !arr[5].Contains("Date="))
                        item.Crs = arr[5];
                }

                RirekiList.Add(item);
            }

            if (RirekiList.Count > 0)
            {
                FirstSeqNo = RirekiList.First().SeqNo;
                LastSeqNo = RirekiList.Last().SeqNo;
            }
        }

    }
    public partial class RirekiSearchOpt : ObservableObject
    {
        [ObservableProperty]
        private string searchDate;

        [ObservableProperty]
        private bool chkUserOnly;

        [ObservableProperty]
        private bool chkNotTN;

        [ObservableProperty]
        private bool chkNotDaTe;

        [ObservableProperty]
        private bool chkDate;

        [ObservableProperty]
        private bool chkBizBrowser;
    }

}
