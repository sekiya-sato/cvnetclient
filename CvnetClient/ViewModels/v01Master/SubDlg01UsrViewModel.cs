using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Models;
using CvnetClient.Utils;
using CvnetClient.Views;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Windows.Media.Imaging;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg01UsrViewModel : BaseViewModel
    {
        #region Declare
        [ObservableProperty]
        ObservableCollection<MasterWorker>? listWorker;
        [ObservableProperty]
        MasterWorker? selectedWorker;
        [ObservableProperty]
        MasterWorker? editWorker;
        [ObservableProperty]
        string? startCode;
        [ObservableProperty]
        public Dictionary<string, string>? comboListBumon;
        [ObservableProperty]
        public Dictionary<string, string>? comboListShop;
        [ObservableProperty]
        public Dictionary<int, string>? comboListSalesFLG;
        [ObservableProperty]
        public Dictionary<int, string>? comboListMailFLG;
        [ObservableProperty]
        public Dictionary<int, string>? comboListOutPutFLG;
        [ObservableProperty]
        public Dictionary<int, string>? comboListPosCate;
        [ObservableProperty]
        public Dictionary<string, string>? comboListBuka;
        [ObservableProperty]
        public Dictionary<string, string>? comoListYakuShoku;
        [ObservableProperty]
        public Dictionary<string, string>? comboListEmplyomentFLG;
        [ObservableProperty]
        public Dictionary<string, string>? comboListSalaryCate;
        [ObservableProperty]
        public Dictionary<string, string>? comboListTransFeeCate;
        [ObservableProperty]
        public Dictionary<string, string>? comboListName1;
        [ObservableProperty]
        public Dictionary<string, string>? comboListName2;
        [ObservableProperty]
        public Dictionary<string, string>? comboListName3;
        [ObservableProperty]
        public Dictionary<string, string>? comboListName4;
        [ObservableProperty]
        public Dictionary<string, string>? comboListName5;
        [ObservableProperty]
        public string? shopName;
        [ObservableProperty]
        public string? tenpoFlg;
        [ObservableProperty]
        private BitmapImage? sourceImage;

        private string? _localImagePath;

        private readonly string _baseUrl = AppData.Url;
        private readonly string _dataPath = AppData.DataAddPath;
        #endregion

        public void OnInit(object? init_para = null, string? init_flg = null) {

            OnInitBase(init_para, init_flg);

            EditWorker = new MasterWorker();
            #region ComboBox
            var comboList = new Dictionary<string, string>();
            string sql_query = "select A.名称CD,A.名称 from HC$Master_MEISHO a  where  a.名称区分='BMN' order by a.名称CD";
            var get_combolist = AppData.Http?.AspxSqlQuery(sql_query, null);
            foreach (DataRow row in get_combolist.Rows)
            {
                string? key = row[0] != DBNull.Value ? row[0].ToString() : string.Empty;
                string? value = row[1] != DBNull.Value ? row[1].ToString() : string.Empty;
                comboList.Add(key, string.Format("{0} {1}", key, value));
            }
            ComboListBumon = comboList;
            EditWorker.Department = ComboListBumon.FirstOrDefault().Key;

            ComboListSalesFLG = new Dictionary<int, string>
            {
                {  0, "0 ---" },
                {  1, "1 営業担当" }
            };
            EditWorker.SalesFlg = ComboListSalesFLG.FirstOrDefault().Key;

            ComboListMailFLG = new Dictionary<int, string>
            {
                {  0, "0 送信しない" },
                {  1, "1 送信する" }
            };
            EditWorker.EmailFLG = ComboListMailFLG.FirstOrDefault().Key;

            ComboListOutPutFLG = new Dictionary<int, string> 
            {
                {  0, "0 通常" },
                {  1, "1 出力しない" }
            };
            EditWorker.OutputFLG = ComboListOutPutFLG.FirstOrDefault().Key;

            ComboListPosCate = new Dictionary<int, string> 
            {
                {0, "0 通常"},
                {9, "9 POSマスタ削除指示" },
                {10, "10 出力しない" }
            };
            EditWorker.PosCate = ComboListPosCate.FirstOrDefault().Key;            

            ComboListBuka = new Dictionary<string, string> 
            {
                {"01010101", "01010101 レディ" },
                {"02020202", "02020202 マカロン" }
            };
            EditWorker.SectionCD = ComboListBuka.FirstOrDefault().Key;

            ComoListYakuShoku = new Dictionary<string, string>
            {
                {"1", "1000001" },
                {"ABC1", "1000001" }
            };
            EditWorker.PositionCD = ComoListYakuShoku.FirstOrDefault().Key;

            ComboListEmplyomentFLG = new Dictionary<string, string>
            {
                {"0", "0 在職" },
                {"1", "1 休職" },
                {"9","9 退職" }
            };
            EditWorker.EmploymentFLG = ComboListEmplyomentFLG.FirstOrDefault().Key;

            ComboListSalaryCate = new Dictionary<string, string> 
            {
                {"0", "0 月給" },
                {"4", "4 時給" }
            };
            EditWorker.SalaryCate = ComboListSalaryCate.FirstOrDefault().Key;

            ComboListTransFeeCate = new Dictionary<string, string> 
            {
                {"0", "0 定額" },
                {"1", "1 月払" }
            };
            EditWorker.TransExpCate = ComboListTransFeeCate.FirstOrDefault().Key;

            comboList = new Dictionary<string, string>();
            sql_query = "select A.名称CD,A.名称 from HC$Master_MEISHO a  where  a.名称区分='E01' order by a.名称CD";
            var get_combolist1 = AppData.Http?.AspxSqlQuery(sql_query, null);
            foreach (DataRow row in get_combolist1.Rows)
            {
                string? key = row[0] != DBNull.Value ? row[0].ToString() : string.Empty;
                string? value = row[1] != DBNull.Value ? row[1].ToString() : string.Empty;
                comboList.Add(key, string.Format("{0} {1}", key, value));
            }
            ComboListName1 = comboList;
            EditWorker.NameCD01 = ComboListName1.FirstOrDefault().Key;

            comboList = new Dictionary<string, string>();
            sql_query = "select A.名称CD,A.名称 from HC$Master_MEISHO a  where  a.名称区分='E02' order by a.名称CD";
            var get_combolist2 = AppData.Http?.AspxSqlQuery(sql_query, null);
            foreach (DataRow row in get_combolist2.Rows)
            {
                string? key = row[0] != DBNull.Value ? row[0].ToString() : string.Empty;
                string? value = row[1] != DBNull.Value ? row[1].ToString() : string.Empty;
                comboList.Add(key, string.Format("{0} {1}", key, value));
            }
            ComboListName2 = comboList;
            EditWorker.NameCD02 = ComboListName2.FirstOrDefault().Key;

            comboList = new Dictionary<string, string>();
            sql_query = "select A.名称CD,A.名称 from HC$Master_MEISHO a  where  a.名称区分='E03' order by a.名称CD";
            var get_combolist3 = AppData.Http?.AspxSqlQuery(sql_query, null);
            foreach (DataRow row in get_combolist3.Rows)
            {
                string? key = row[0] != DBNull.Value ? row[0].ToString() : string.Empty;
                string? value = row[1] != DBNull.Value ? row[1].ToString() : string.Empty;
                comboList.Add(key, string.Format("{0} {1}", key, value));
            }
            ComboListName3 = comboList;
            EditWorker.NameCD03 = ComboListName3.FirstOrDefault().Key;

            comboList = new Dictionary<string, string>();
            sql_query = "select A.名称CD,A.名称 from HC$Master_MEISHO a  where  a.名称区分='E04' order by a.名称CD";
            var get_combolist4 = AppData.Http?.AspxSqlQuery(sql_query, null);
            foreach (DataRow row in get_combolist4.Rows)
            {
                string? key = row[0] != DBNull.Value ? row[0].ToString() : string.Empty;
                string? value = row[1] != DBNull.Value ? row[1].ToString() : string.Empty;
                comboList.Add(key, string.Format("{0} {1}", key, value));
            }
            ComboListName4 = comboList;
            EditWorker.NameCD04 = ComboListName4.FirstOrDefault().Key;

            comboList = new Dictionary<string, string>();
            sql_query = "select A.名称CD,A.名称 from HC$Master_MEISHO a  where  a.名称区分='E05' order by a.名称CD";
            var get_combolist5 = AppData.Http?.AspxSqlQuery(sql_query, null);
            foreach (DataRow row in get_combolist5.Rows)
            {
                string? key = row[0] != DBNull.Value ? row[0].ToString() : string.Empty;
                string? value = row[1] != DBNull.Value ? row[1].ToString() : string.Empty;
                comboList.Add(key, string.Format("{0} {1}", key, value));
            }
            ComboListName5 = comboList;
            EditWorker.NameCD05 = ComboListName5.FirstOrDefault().Key;
            #endregion
            if (AppData.ClassCvnet.config.MultiCoop > 0)
            {
                TenpoFlg = "移動倉庫S";
            }
            else
            {
                TenpoFlg = "移動倉庫";
            }
        }

        partial void OnSelectedWorkerChanged(MasterWorker? value)
        {
            if (value != null)
            {
                EditWorker = CvnetBaseCore.Common.CloneObject(value);
                LoadWorkerImage(value.TelNo);
            }
                
            else
                EditWorker = null;
        }

        [RelayCommand]
        public void SelShop(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && EditWorker != null)
            {
                EditWorker.ShopCD = get_sel00.Code;
                EditWorker.ShopName = get_sel00.Name;
            }
        }

        [RelayCommand]
        void DoList() {

            if (AppData.ClassCvnet.MstDialog.ContainsKey("社員") && AppData.ClassCvnet.ComboListFLg == 1)
            {
                var ar = new string[] { "A" };
                var vm = AppData.DlgService.GetSelUsr(AppData.ClassCvnet.MstDialog["社員"].v_mstname, null, ar);
                if (vm != null && vm.SelUsrResult1 != null)
                {
                    OnQuery(vm.SelUsrResult1.Item2.ToArray(),vm.SelUsrResult1.Item1,null);                    
                }
                return;
            }
            var v_para = new string[] { StartCode };
        }

        void OnQuery(string[]? v_para = null,string? qs = null,string? p_sort = null) 
        {
            if (qs != null) {
                qs += " order by 社員CD";
                var ret_Data = AppData.Http!.AspxSqlQuery(qs, v_para);

                qs = string.Join(",",
                    ret_Data.AsEnumerable()
                           .Take(40)
                           .Select(r => $"'{r[0]}'"));
            }
            
            var v_sort = " asc ";
            if (p_sort != null) v_sort = " desc";
            var v_hugo = ">=";
            if (p_sort != null) v_hugo = "<=";

            var sql_query = "select A.SEQ_NO,A.VDATE_CREATE,A.VDATE_UPDATE,(SELECT 得意先名 FROM HC$MASTER_TOKUI WHERE 得意先CD=A.店舗CD) 店舗,";            
            sql_query += "A.社員CD,A.名前,A.部門,A.店舗CD,A.営業FLG,A.メール,A.携帯TEL,A.備考,A.特権FLG,A.プロフィール";
            sql_query += ",A.役職CD,A.就業FLG,A.入社日,A.有給残,A.給与区分,A.給与支給額,A.交通費区分,A.交通費支給額,A.出力FLG,A.部課CD,A.POS区分";
            sql_query += ",A.名称CD01,A.名称CD02,A.名称CD03,A.名称CD04,A.名称CD05,A.フリガナ,A.メールFLG,A.退勤日,A.退職日,A.特休残";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='BMN' and H.名称CD=A.部門),'.') 部門名";
            sql_query += ",NVL((select H.得意先名 from HC$MASTER_TOKUI H where  H.得意先CD=A.店舗CD),'.') 店舗名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='YAK' and H.名称CD=A.役職CD),'.') 役職名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='BKA' and H.名称CD=A.部課CD),'.') 部課名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='E01' and H.名称CD=A.名称CD01),'.') 分類01名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='E02' and H.名称CD=A.名称CD02),'.') 分類02名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='E03' and H.名称CD=A.名称CD03),'.') 分類03名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='E04' and H.名称CD=A.名称CD04),'.') 分類04名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='E05' and H.名称CD=A.名称CD05),'.') 分類05名";
            
            sql_query += ",(A.入力社員CD ||' '|| (select B.名前 from HC$MASTER_SHAIN B where B.社員CD=A.入力社員CD)) 最終修正者";
            sql_query += " from HC$Master_SHAIN A";
            
            if (qs == null || qs == "")
            {
                sql_query += " where A.社員CD" + v_hugo.ToString() + " '"+ StartCode + "'  order by A.社員CD " + v_sort.ToString();
            }
            else {
                sql_query += " where A.社員CD in (" + qs + ")  order by A.社員CD " + v_sort.ToString();
            }

            sql_query = "select * from (" + sql_query + ") where rownum <= " + AppData.maxQueryCnt;

            var retData = AppData.Http!.AspxSqlQuery(sql_query, new string[] { });
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
                            ShopName = dr["店舗"].ToString() ?? string.Empty,
                            SalesFlg = Convert.ToInt32(dr["営業FLG"]),
                            Mail = dr["メール"].ToString() ?? string.Empty,
                            TelNo = dr["携帯TEL"].ToString() ?? string.Empty,
                            SpecialFlg = dr["特権FLG"].ToString() ?? string.Empty,
                            Furigana = dr["フリガナ"].ToString() ?? string.Empty,
                            PositionCD = dr["役職CD"].ToString() ?? string.Empty,
                            EmploymentFLG = dr["就業FLG"].ToString() ?? string.Empty,
                            OutputFLG = Convert.ToInt32(dr["出力FLG"]),
                            Notes = dr["備考"].ToString() ?? string.Empty,
                            JoiningDate = DateTime.ParseExact(dr["入社日"]?.ToString(), "yyyyMMdd", null),
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
                            EmployeeInpCD = dr["最終修正者"].ToString() ?? string.Empty,
                            SpecHolidayRemain = Convert.ToInt32(dr["特休残"]),
                            EndDate = dr["退勤日"].ToString() ?? string.Empty,
                            RetireDate = DateTime.ParseExact(dr["退職日"]?.ToString(), "yyyyMMdd", null),
                            Profile = dr["プロフィール"].ToString() ?? string.Empty
                        }).OrderBy(c => c.WorkerCD).ToList();
            Common.ConvertDotStringDel(list);
            ListWorker = new ObservableCollection<MasterWorker>(list);
            if (ListWorker.Count > 0)
            {
                SelectedWorker = ListWorker[0];
            }
        }        

        [RelayCommand]
        void BackList()
        {
            if (ListWorker != null && ListWorker.Count > 0)
            {
                StartCode = ListWorker.Min(c => c.WorkerCD);
            }
            else 
            { 
                DoList();
            }
            OnQuery(null, null, "<=");
            if (ListWorker == null || ListWorker.Count == 0)
                ClientLib.MessageBoxOk(this, "データがありません");
        }

        [RelayCommand]
        void NextList()
        {            
            if (ListWorker != null && ListWorker.Count > 0)
            {
                StartCode = ListWorker.Max(c => c.WorkerCD);
            }
            else
            {
                DoList();
            }
            OnQuery(null, null, null);
            if (ListWorker == null || ListWorker.Count == 0)
                ClientLib.MessageBoxOk(this, "データがありません");
        }

        [RelayCommand]
        void DoInsert()
        {
            if (!ClientLib.MessageBox(this, "新規登録しますか？")) return;
            var item = Common.CloneObject(EditWorker);
            Common.ConvertDotStringAdd(item);
            if (item == null) return;
            var ret = AppData.Http!.AspxSqlExe(DBDef.DB_DML.INSERT, "Master_SHAIN", 0, "0",
                new string[] { "社員CD", "名前", "部門", "店舗CD", "営業FLG", "メール", "携帯TEL", "特権FLG", "フリガナ", "役職CD", "就業FLG", "出力FLG", "備考", "入社日", "有給残", "給与区分", "給与支給額", "交通費区分", "交通費支給額", "部課CD", "名称CD01", "名称CD02", "名称CD03", "名称CD04", "名称CD05", "POS区分", "メールFLG", "入力社員CD", "特休残", "退勤日", "退職日", "プロフィール" },
                new string[] { item.WorkerCD!, item.Name!, item.Department!, item.ShopCD!, item.SalesFlg.ToString()!, item.Mail!, item.TelNo!, item.SpecialFlg!, item.Furigana!, item.PositionCD!, item.EmploymentFLG!, item.OutputFLG.ToString()!, item.Notes!, item.JoiningDate?.ToString("yyyyMMdd"), item.VacationRemaining!, item.SalaryCate!, item.SalaryAmount.ToString()!, item.TransExpCate!, item.TransExpAmount.ToString()!, item.SectionCD!, item.NameCD01!, item.NameCD02!, item.NameCD03!, item.NameCD04!, item.NameCD05!, item.PosCate.ToString()!, item.EmailFLG.ToString()!, AppData.ClassSatoo.SHAIN_CD ?? ".", item.SpecHolidayRemain.ToString() ?? ".", item.EndDate!, item.RetireDate?.ToString("yyyyMMdd"), item.Profile! });
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
                ClientLib.MessageBoxError(this, "データが追加出来ませんでした");
            }
        }

        [RelayCommand]
        void DoUpdate()
        {
            if (!ClientLib.MessageBox(this, "修正しますか？")) return;
            var item = Common.CloneObject(EditWorker);
            Common.ConvertDotStringAdd(item);
            if (item == null) return;
            var ret = AppData.Http!.AspxSqlExe(DBDef.DB_DML.UPDATE, "Master_SHAIN", item.SeqNo, item.VdateUpdate.ToString(),
                new string[] { "社員CD", "名前", "部門", "店舗CD", "営業FLG", "メール", "携帯TEL", "特権FLG", "フリガナ", "役職CD", "就業FLG", "出力FLG", "備考", "入社日", "有給残", "給与区分", "給与支給額", "交通費区分", "交通費支給額", "部課CD", "名称CD01", "名称CD02", "名称CD03", "名称CD04", "名称CD05", "POS区分", "メールFLG", "入力社員CD", "特休残", "退勤日", "退職日", "プロフィール" },
                new string[] { item.WorkerCD!, item.Name!, item.Department!, item.ShopCD!, item.SalesFlg.ToString()!, item.Mail!, item.TelNo!, item.SpecialFlg!, item.Furigana!, item.PositionCD!, item.EmploymentFLG!, item.OutputFLG.ToString()!, item.Notes!, item.JoiningDate?.ToString("yyyyMMdd"), item.VacationRemaining!, item.SalaryCate!, item.SalaryAmount.ToString()!, item.TransExpCate!, item.TransExpAmount.ToString()!, item.SectionCD!, item.NameCD01!, item.NameCD02!, item.NameCD03!, item.NameCD04!, item.NameCD05!, item.PosCate.ToString()!, item.EmailFLG.ToString()!, AppData.ClassSatoo.SHAIN_CD ?? ".", item.SpecHolidayRemain.ToString()!, item.EndDate!, item.RetireDate?.ToString("yyyyMMdd"), item.Profile! });
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
                ClientLib.MessageBoxError(this, "データが修正出来ませんでした");
            }
        }

        [RelayCommand]
        void DoDelete()
        {
            if (!ClientLib.MessageBox(this, "削除しますか？")) return;
            if (EditWorker == null) return;
            var ret = AppData.Http!.AspxSqlExe(DBDef.DB_DML.DELETE, "Master_SHAIN", EditWorker.SeqNo, EditWorker.VdateUpdate.ToString(),
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

        string printsql = "select * from (select A.SEQ_NO,SUBSTR(GET_VDATE(a.VDATE_CREATE),0,8)||SUBSTR(GET_VDATE(a.VDATE_CREATE),10,6) 作成日時" +
            ",SUBSTR(GET_VDATE(a.VDATE_UPDATE),0,8)||SUBSTR(GET_VDATE(a.VDATE_UPDATE),10,6) 更新日時" +
            ",A.社員CD,A.名前,A.部門,A.店舗CD,A.営業FLG,A.メール,A.携帯TEL,A.備考" +
            ",A.役職CD,A.就業FLG,A.入社日,A.有給残,A.給与区分,A.給与支給額,A.交通費区分,A.交通費支給額,A.出力FLG,A.部課CD,A.POS区分" +
            ",A.名称CD01,A.名称CD02,A.名称CD03,A.名称CD04,A.名称CD05,A.フリガナ,A.メールFLG,A.退職日,特休残" +
            ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='BMN' and H.名称CD=A.部門),'.') 部門名" +
            ",NVL((select H.得意先名 from HC$MASTER_TOKUI H where  H.得意先CD=A.店舗CD),'.') 店舗名" +
            ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='YAK' and H.名称CD=A.役職CD),'.') 役職名" +
            ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='BKA' and H.名称CD=A.部課CD),'.') 部課名" +
            ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='E01' and H.名称CD=A.名称CD01),'.') 分類01名" +		
            ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='E02' and H.名称CD=A.名称CD02),'.') 分類02名" +
            ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='E03' and H.名称CD=A.名称CD03),'.') 分類03名" +
            ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='E04' and H.名称CD=A.名称CD04),'.') 分類04名" +
            ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='E05' and H.名称CD=A.名称CD05),'.') 分類05名" +		
            ",(A.入力社員CD ||' '|| (select B.名前 from HC$MASTER_SHAIN B where B.社員CD=A.入力社員CD)) 最終修正者" +						
            ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='E01'),'.') title1" +
            ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='E02'),'.') title2" +
            ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='E03'),'.') title3" +
            ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='E04'),'.') title4" +
            ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='IDX' and H.名称CD='E05'),'.') title5" +
            ",CASE WHEN (A.営業FLG = 0) THEN '0 ---' WHEN (A.営業FLG = 1) THEN '1 営業担当' ELSE '.' END 営業FLG名" +
            ",CASE WHEN (A.出力FLG = 0) THEN '0 通常' WHEN (A.出力FLG = 99) THEN '99 出力しない' ELSE '.' END 出力FLG名" +
            ",CASE WHEN (A.POS区分 = 0) THEN '0 通常' WHEN (A.POS区分 = 9) THEN '9 POSマスタ削除指示' WHEN (A.POS区分 = 10) THEN '10 出力しない' ELSE '.' END POS区分名" +
            ",CASE WHEN (A.就業FLG = 0) THEN '0 在職' WHEN (A.就業FLG = 1) THEN '1 休職' WHEN (A.就業FLG = 9) THEN '9 退職' ELSE '.' END 就業FLG名" +
            ",CASE WHEN (A.給与区分 = 1) THEN '1 月給' WHEN (A.給与区分 = 4) THEN '4 時給' ELSE '.' END 給与区分名" +
            ",CASE WHEN (A.交通費区分 = 0) THEN '0 定額' WHEN (A.交通費区分 = 1) THEN '1 月払' ELSE '.' END 交通費区分名" +
            " from HC$Master_SHAIN A";
        /// <summary>
        /// PDF印刷
        /// </summary>
        [RelayCommand]
        async Task DoPrintAsync()
        {
            if (!ClientLib.MessageBox(this, "印刷しますか？")) return;
            ClientLib.CursorToWait();
            if (ListWorker == null || ListWorker.Count == 0) { ClientLib.CursorToNormal(); return; } 
            var paramNames = ListWorker.Select((c, i) => $":p{i}||''").ToList();

            var sql = printsql +
                      $" where TO_CHAR(A.社員CD) in ({string.Join(",", paramNames)}) order by A.社員CD)";

            var parameters = ListWorker.Select(c => c.WorkerCD).ToArray();

            var ret = AppData.Http!.AspxSqlQueryCsv(sql, parameters, "cvnet_shain.qfm");
            var lines = ret.Split('\n');

            if (lines.Length < 2 || lines[1] == "0")
            {
                ClientLib.MessageBoxError(this, "PDFデータがありません");
                return;
            }

            string pdfPath = lines[0];
            string url = AppData.Http.URLroot + pdfPath + "/data.pdf";

            bool ready = await Utils.GlobalFunc.WaitForPdfAsync(url, TimeSpan.FromSeconds(30));
            if (!ready)
            {
                ClientLib.MessageBoxError(this, "PDF生成に時間がかかりすぎています。\n 条件を絞ってください。");
                return;
            }

            var win = new WebpdfView();
            if (win.DataContext is WebpdfViewModel vm)
            {
                vm.Pdfdata = url;
            }
            ClientLib.CursorToNormal();
            ClientLib.ShowDialogView(win, this);
        }

        public async void OnImageDropped(string filePath)
        {
            if (!System.IO.File.Exists(filePath)) return;
            
            _localImagePath = filePath;
            if (EditWorker != null) { 
                EditWorker.TelNo = Path.GetFileName(_localImagePath);
            }
            
            SourceImage = CreateSafeBitmapImage(filePath);

            if (EditWorker != null)
                await UploadToAspxServerAsync();
        }

        [RelayCommand]
        private async Task OpenFileDialogAsync()
        {
            var dialog = new OpenFileDialog
            {
                Title = "画像を選択してください",
                //Filter = "画像ファイル|*.jpg;*.jpeg;*.png;*.bmp;*.gif"
            };

            if (dialog.ShowDialog() == true)
            {
                _localImagePath = dialog.FileName;
                if (EditWorker != null)
                {
                    EditWorker.TelNo = Path.GetFileName(_localImagePath);
                }
                SourceImage = CreateSafeBitmapImage(_localImagePath);

                if (EditWorker != null)
                    await UploadToAspxServerAsync();
            }
        }
       
        private void LoadWorkerImage(string imageFileName)
        {
            if (string.IsNullOrEmpty(imageFileName))
            {
                SourceImage = null;
                return;
            }

            string fullUrl = $"{_baseUrl}Data/{_dataPath}{imageFileName}?t={DateTime.Now.Ticks}";
            SourceImage = CreateSafeBitmapImage(fullUrl);
        }

        private BitmapImage CreateSafeBitmapImage(string path)
        {
            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            bmp.CreateOptions = BitmapCreateOptions.IgnoreImageCache;

            if (path.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                bmp.UriSource = new Uri(path, UriKind.Absolute);
            else
                bmp.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);

            bmp.EndInit();
            return bmp;
        }

        private async Task UploadToAspxServerAsync()
        {
            if (string.IsNullOrEmpty(_localImagePath))
                return;

            await Task.Run(() =>
            {
                string mess;
                bool ok = AppData.Http!.UploadAllFile(_localImagePath, Path.GetFileName(_localImagePath), _dataPath, out mess);
                if (!ok)
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        ClientLib.MessageBoxError(this,$"アップロード失敗: {mess}", "エラー");
                    });
                }
                else
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        ClientLib.MessageBoxOk(this, "アップロード成功！", "完了");
                    });
                }
            });
        }
    }
}
