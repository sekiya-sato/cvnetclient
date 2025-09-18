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
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        [ObservableProperty]
        string? startCode;

        string sql_list = "SELECT * FROM (SELECT * FROM HC$MASTER_SHAIN ORDER BY 社員CD {1}) WHERE ROWNUM <= {2}";

        partial void OnSelectedWorkerChanged(MasterWorker? value)
        {
            if (value != null)
                EditWorker = Common.CloneObject(value);
            else
                EditWorker = null;
        }

        [RelayCommand]
        void DoList() {
            SubList(string.IsNullOrEmpty(StartCode) ? "." : StartCode, ">=","asc", AppData.maxQueryCnt);
            if(ListWorker == null || ListWorker.Count == 0)
                ClientLib.MessageBoxOk(this, "データがありません");
        }

        void SubList(string startCd, string sql_P1, string sql_P2, int sql_P3) {
            var sql = string.Format(sql_list, sql_P1, sql_P2, sql_P3);
            var retData = AppData.Http?.AspxSqlQuery(sql, new string[] {startCd});
            if (retData == null || retData.Rows.Count == 0) return;
            var list = (from DataRow dr in retData.Rows
                        select new MasterWorker
                        {
                            SeqNo = Convert.ToInt64(dr["SEQ_NO"]),
                            VdateCreate = Convert.ToDecimal(dr["VDATE_CREATE"]),
                            VdateUpdate = Convert.ToDecimal(dr["VDATE_UPDATE"]),
                            WorkerCD = dr["社員CD"].ToString() ?? string.Empty,
                            Name = dr["名前"].ToString() ?? string.Empty,
                            Department = dr["部門"].ToString() ?? string.Empty,
                            ShopCD = dr["店舗CD"].ToString() ?? string.Empty,
                            SalesFlg = Convert.ToInt32(dr["営業FLG"]),
                            Mail = dr["メール"].ToString() ?? string.Empty,
                            TelNo = dr["携帯TEL"].ToString() ?? string.Empty,
                            SpecialFlg = dr["特権FLG"].ToString() ?? string.Empty,
                            Furigana = dr["フリガナ"].ToString() ?? string.Empty,
                            PositionCD = dr["役職CD"].ToString() ?? string.Empty,
                            EmploymentFLG = dr["就業FLG"].ToString() ?? string.Empty,
                            OutputFLG = Convert.ToInt32(dr["出力FLG"]),
                            Notes = dr["備考"].ToString() ?? string.Empty,
                            JoiningDate = dr["入社日"].ToString() ?? string.Empty,
                            VacationRemaining = dr["有給残"].ToString() ?? string.Empty,
                            SalaryCate = dr["給与区分"].ToString() ?? string.Empty,
                            SalaryAmount = Convert.ToInt32(dr["給与支給額"]),
                            TransExpCate = dr["交通費区分"].ToString() ?? string.Empty,
                            TransExpAmount = Convert.ToInt32(dr["交通費支給額"]),
                            SectionCD = dr["部課CD"].ToString() ?? string.Empty,
                            NameCD01 = dr["名称CD01"].ToString() ?? string.Empty,
                            NameCD02 = dr["名称CD02"].ToString() ?? string.Empty,
                            NameCD03 = dr["名称CD03"].ToString() ?? string.Empty,
                            NameCD04 = dr["名称CD04"].ToString() ?? string.Empty,
                            NameCD05 = dr["名称CD05"].ToString() ?? string.Empty,
                            PosCate = Convert.ToInt32(dr["POS区分"]),
                            EmailFLG = Convert.ToInt32(dr["メールFLG"]),
                            EmployeeInpCD = dr["入力社員CD"].ToString() ?? string.Empty,
                            SpecHolidayRemain = Convert.ToInt32(dr["特休残"]),
                            EndDate = dr["退勤日"].ToString() ?? string.Empty,
                            RetireDate = dr["退職日"].ToString() ?? string.Empty,
                            Profile = dr["プロフィール"].ToString() ?? string.Empty
                        }).OrderBy(c => c.WorkerCD).ToList();
            Common.ConvertDotStringDel(list);
            ListWorker = new ObservableCollection<MasterWorker>(list);
            if (ListWorker.Count > 0) { 
                SelectedWorker = ListWorker[0];
            }
        }

        [RelayCommand]
        void BackList()
        {
            var startcd = string.IsNullOrEmpty(StartCode) ? "." : StartCode;
            if (ListWorker != null && ListWorker.Count > 0) 
            {
                startcd = ListWorker.Min(c => c.WorkerCD);
            }
            SubList(startcd!, "<=", "desc", AppData.maxQueryCnt);
            if (ListWorker == null || ListWorker.Count == 0)
                ClientLib.MessageBoxOk(this, "データがありません");
        }

        [RelayCommand]
        void NextList()
        {
            var startcd = string.IsNullOrEmpty(StartCode) ? "." : StartCode;
            if (ListWorker != null && ListWorker.Count > 0)
            {
                startcd = ListWorker.Max(c => c.WorkerCD);
            }
            SubList(startcd!, "<=", "desc", AppData.maxQueryCnt);
            if (ListWorker == null || ListWorker.Count == 0)
                ClientLib.MessageBoxOk(this, string.Empty);
        }

        [RelayCommand]
        void DoInsert()
        {
            if (!ClientLib.MessageBox(this, "新規登録しますか？")) return;
            var item = Common.CloneObject(EditWorker);
            Common.ConvertDotStringAdd(item);
            if (item == null) return;
            var ret = AppData.Http!.AspxSqlExe(DBDef.DB_DML.INSERT, "MASTER_SHAIN", 0, "0",
                new string[] { "社員CD", "名前", "部門", "店舗CD", "営業FLG", "メール", "携帯TEL", "特権FLG", "フリガナ", "役職CD", "就業FLG", "出力FLG", "備考", "入社日", "有給残", "給与区分", "給与支給額", "交通費区分", "交通費支給額", "部課CD", "名称CD01", "名称CD02", "名称CD03", "名称CD04", "名称CD05", "POS区分", "メールFLG", "入力社員CD", "特休残", "退勤日", "退職日", "プロフィール" },
                new string[] { item.WorkerCD!, item.Name!, item.Department!, item.ShopCD!, item.SalesFlg.ToString()!, item.Mail!, item.TelNo!, item.SpecialFlg!, item.Furigana!, item.PositionCD!, item.EmploymentFLG!, item.OutputFLG.ToString()!, item.Notes!, item.JoiningDate!, item.VacationRemaining!, item.SalaryCate!, item.SalaryAmount.ToString()!, item.TransExpCate!, item.TransExpAmount.ToString()!, item.SectionCD!, item.NameCD01!, item.NameCD02!, item.NameCD03!, item.NameCD04!, item.NameCD05!, item.PosCate.ToString()!, item.EmailFLG.ToString()!, item.EmployeeInpCD!, item.SpecHolidayRemain.ToString()!, item.EndDate!, item.RetireDate!, item.Profile! });
            if (ret.Code == 0)
            {
                item.SeqNo = ret.NewSeq;
                item.VdateUpdate = decimal.Parse(ret.VDate);
                item.VdateCreate = item.VdateUpdate;
                Common.ConvertDotStringDel(item);
                ListWorker!.Add(item);
                SelectedWorker = item;
            }
            else {
                ClientLib.MessageBoxError(this, ret.Code.ToString());
            }
        }

        [RelayCommand]
        void DoUpdate()
        {
            if (!ClientLib.MessageBox(this, "修正しますか？")) return;
            var item = Common.CloneObject(EditWorker);
            Common.ConvertDotStringAdd(item);
            if (item == null) return;
            var ret = AppData.Http!.AspxSqlExe(DBDef.DB_DML.UPDATE,"HASTER_SHAIN", item.SeqNo, item.VdateUpdate.ToString(),
                new string[] { "社員CD", "名前", "部門", "店舗CD", "営業FLG", "メール", "携帯TEL", "特権FLG", "フリガナ", "役職CD", "就業FLG", "出力FLG", "備考", "入社日", "有給残", "給与区分", "給与支給額", "交通費区分", "交通費支給額", "部課CD", "名称CD01", "名称CD02", "名称CD03", "名称CD04", "名称CD05", "POS区分", "メールFLG", "入力社員CD", "特休残", "退勤日", "退職日", "プロフィール" },
                new string[] { item.WorkerCD!, item.Name!, item.Department!, item.ShopCD!, item.SalesFlg.ToString()!, item.Mail!, item.TelNo!, item.SpecialFlg!, item.Furigana!, item.PositionCD!, item.EmploymentFLG!, item.OutputFLG.ToString()!, item.Notes!, item.JoiningDate!, item.VacationRemaining!, item.SalaryCate!, item.SalaryAmount.ToString()!, item.TransExpCate!, item.TransExpAmount.ToString()!, item.SectionCD!, item.NameCD01!, item.NameCD02!, item.NameCD03!, item.NameCD04!, item.NameCD05!, item.PosCate.ToString()!, item.EmailFLG.ToString()!, item.EmployeeInpCD!, item.SpecHolidayRemain.ToString()!, item.EndDate!, item.RetireDate!, item.Profile! });
            if (ret.Code == 0)
            {
                Common.ConvertDotStringDel(item);
                if (SelectedWorker != null) 
                {
                    SelectedWorker.VdateUpdate = decimal.Parse(ret.VDate);
                    SelectedWorker.WorkerCD = item.WorkerCD;
                    SelectedWorker.Name = item.Name;
                    SelectedWorker.Department = item.Department;
                    SelectedWorker.ShopCD = item.ShopCD;
                    SelectedWorker.SalesFlg = item.SalesFlg;
                    SelectedWorker.Mail = item.Mail;
                    SelectedWorker.TelNo = item.TelNo;
                    SelectedWorker.SpecialFlg = item.SpecialFlg;
                    SelectedWorker.Furigana = item.Furigana;
                    SelectedWorker.PositionCD = item.PositionCD;
                    SelectedWorker.EmploymentFLG = item.EmploymentFLG;
                    SelectedWorker.OutputFLG = item.OutputFLG;
                    SelectedWorker.Notes = item.Notes;
                    SelectedWorker.JoiningDate = item.JoiningDate;
                    SelectedWorker.VacationRemaining = item.VacationRemaining;
                    SelectedWorker.SalaryCate = item.SalaryCate;
                    SelectedWorker.SalaryAmount = item.SalaryAmount;
                    SelectedWorker.TransExpCate = item.TransExpCate;
                    SelectedWorker.TransExpAmount = item.TransExpAmount;
                    SelectedWorker.SectionCD = item.SectionCD;
                    SelectedWorker.NameCD01 = item.NameCD01;
                    SelectedWorker.NameCD02 = item.NameCD02;
                    SelectedWorker.NameCD03 = item.NameCD03;
                    SelectedWorker.NameCD04 = item.NameCD04;
                    SelectedWorker.NameCD05 = item.NameCD05;
                    SelectedWorker.PosCate = item.PosCate;
                    SelectedWorker.EmailFLG = item.EmailFLG;
                    SelectedWorker.EmployeeInpCD = item.EmployeeInpCD;
                    SelectedWorker.SpecHolidayRemain = item.SpecHolidayRemain;
                    SelectedWorker.EndDate = item.EndDate;
                    SelectedWorker.RetireDate = item.RetireDate;
                    SelectedWorker.Profile = item.Profile;
                    EditWorker = Common.CloneObject(SelectedWorker);
                }
                
            }
            else {
                ClientLib.MessageBoxError(this, ret.Code.ToString());
            }
        }

        [RelayCommand]
        void DoDelete()
        {
            if (!ClientLib.MessageBox(this, "削除しますか？")) return;
            if (EditWorker == null) return;
            var ret = AppData.Http!.AspxSqlExe(DBDef.DB_DML.DELETE, "Master_MEISHO", EditWorker.SeqNo, EditWorker.VdateUpdate.ToString(),
                new string[0], new string[0]);
            if (ret.Code == 0)
            {
                if (SelectedWorker != null)
                {
                    ListWorker!.Remove(SelectedWorker);
                    var item = ListWorker.Where(c => c.WorkerCD == ListWorker.Min(c => c.WorkerCD)).FirstOrDefault();
                    SelectedWorker = item;
                }
            }
            else
            {
                ClientLib.MessageBoxError(this, ret.Code.ToString());
            }
        }

        string printsql = """
				 select A.SEQ_NO
				,SUBSTR(GET_VDATE(a.VDATE_CREATE),0,8)||SUBSTR(GET_VDATE(a.VDATE_CREATE),10,6) 作成日時
				,SUBSTR(GET_VDATE(a.VDATE_UPDATE),0,8)||SUBSTR(GET_VDATE(a.VDATE_UPDATE),10,6) 更新日時

				,NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='BMN' and H.名称CD=A.部門),'.') 部門名
				,NVL((select H.得意先名 from HC$MASTER_TOKUI H where  H.得意先CD=A.店舗CD),'.') 店舗名
				,NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='YAK' and H.名称CD=A.役職CD),'.') 役職名
				,NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='BKA' and H.名称CD=A.部課CD),'.') 部課名
				,NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='E01' and H.名称CD=A.名称CD01),'.') 分類01名		
				,NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='E02' and H.名称CD=A.名称CD02),'.') 分類02名
				,NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='E03' and H.名称CD=A.名称CD03),'.') 分類03名
				,NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='E04' and H.名称CD=A.名称CD04),'.') 分類04名
				,NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='E05' and H.名称CD=A.名称CD05),'.') 分類05名		
				,(A.入力社員CD ||' '|| (select B.名前 from HC$MASTER_SHAIN B where B.社員CD=A.入力社員CD)) 最終修正者						
				,NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='E01'),'.') title1
				,NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='E02'),'.') title2
				,NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='E03'),'.') title3
				,NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='E04'),'.') title4
				,NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='E05'),'.') title5                
				""";
        /// <summary>
        /// PDF印刷
        /// </summary>
        [RelayCommand]
        async Task DoPrintAsync()
        {
            if (!ClientLib.MessageBox(this, "印刷しますか？")) return;
            ClientLib.CursorToWait();
            if (ListWorker == null || ListWorker.Count == 0) return;
            var cdlist = string.Join(",", ListWorker.Select(c => $"'{c.WorkerCD}'"));
            
            var ret = AppData.Http!.AspxSqlQueryCsv(string.Format(printsql + " where A.社員CD in (:0) order by A.社員CD", cdlist), new string[] { }, "cvnet_shain.qfm");
            if (ret.Split('\n').Length < 2)
            {
                ClientLib.MessageBoxError(this, "PDFデータがありません");
                return;
            }
            var ret1 = ret.Split('\n');
            var url = AppData.Http.URLroot + ret1[0] + "/data.pdf";
            await Task.Delay(1500); // PDF生成待ち
            var win = new WebpdfView();
            var vm = win.DataContext as WebpdfViewModel;
            if (vm == null) return;
            vm.Pdfdata = url;
            ClientLib.CursorToNormal();
            ClientLib.ShowDialogView(win, this);
        }

        
    }
}
