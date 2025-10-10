using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CvnetBaseCore;
using System.Data;
using CvnetClient.Models;
using CvnetClient.Views;
using System.Collections.ObjectModel;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg01KijiViewModel : BaseViewModel
    {

        [ObservableProperty]
        public Dictionary<string, string>? m_selectKubun;

        [ObservableProperty]
        MasterSHKiji? selectedKiji;

        [ObservableProperty]
        MasterSHKiji? editKiji;

        [ObservableProperty]
        string? findFabricCd;

        [ObservableProperty]
        string? findSupplierCd;

        [ObservableProperty]
        ObservableCollection<MasterSHKiji>? listKiji;

        private bool _canNext;
        public bool CanNext
        {
            get => _canNext;
            set => SetProperty(ref _canNext, value);
        }

        private bool _canBack;
        public bool CanBack
        {
            get => _canBack;
            set => SetProperty(ref _canBack, value);
        }


        [RelayCommand]
        void Init()
        {
            EditKiji = new MasterSHKiji();

            var kubunCdList = new Dictionary<string, string>();
            string sql_query = "select A.名称CD,A.名称 from HC$Master_MEISHO a  where  a.名称区分='KFK' order by a.名称CD";
            var get_kubun = AppData.Http?.AspxSqlQuery(sql_query, null);
            foreach (DataRow row in get_kubun!.Rows)
            {
                string? key = row[0] != DBNull.Value ? row[0].ToString() : string.Empty;
                string? value = row[1] != DBNull.Value ? row[1].ToString() : string.Empty;
                kubunCdList.Add(key!, string.Format("{0} {1}", key, value));
            }
            SelectKubun = kubunCdList;
            EditKiji.CateCd = SelectKubun.FirstOrDefault().Key;
        }

        //Excute Button//

        //     string sql_onExec = """"
        //	select * from(select A.SEQ_NO, A.VDATE_CREATE, A.VDATE_UPDATE, A.商品CD, A.旧コード, A.略称, A.商品名, A.区分CD, A.仕入先CD, A.仕入先商品CD,A.単価,A.メモ,B.仕入先名,
        //	(A.入力社員CD ||' '|| (select S.名前 from HC$MASTER_SHAIN S where S.社員CD=A.入力社員CD)) 最終修正者
        //	from HC$Master_SHKIJI A,HC$MASTER_SIIRE B where  A.仕入先CD=B.仕入先CD(+) AND A.商品CD{0}:1 {1} order by A.商品CD {2},A.仕入先CD) where rownum<={3}
        //"""";

        [RelayCommand]
        void DoList()
        {
            //if (string.IsNullOrEmpty(findSupplierCd)) findSupplierCd = ".";
            var conditions = new List<string>();
            var parameters = new List<object>();
            if (!string.IsNullOrEmpty(FindFabricCd))
            {
                conditions.Add("A.商品CD{0}:1");
                parameters.Add(FindFabricCd);
            }
            if (!string.IsNullOrEmpty(FindSupplierCd))
            {
                conditions.Add("A.仕入先CD=:2");
                parameters.Add(FindSupplierCd);
            }

            string whereClause = string.Join(" AND ", conditions);
            string condition = "";
            if (!string.IsNullOrEmpty(whereClause)) { condition = " where " + whereClause; }

            string sql = $@"
    SELECT *
    FROM (
        SELECT A.SEQ_NO, A.VDATE_CREATE, A.VDATE_UPDATE,
               A.商品CD, A.旧コード, A.略称, A.商品名,
               A.区分CD, A.仕入先CD, A.仕入先商品CD,
               A.単価, A.メモ, B.仕入先名,
               (A.入力社員CD || ' ' ||
                 (SELECT S.名前 FROM HC$MASTER_SHAIN S
                  WHERE S.社員CD = A.入力社員CD)) 最終修正者
        FROM HC$Master_SHKIJI A
        LEFT JOIN HC$MASTER_SIIRE B ON A.仕入先CD = B.仕入先CD
        {condition}
        ORDER BY A.商品CD ASC, A.仕入先CD
    )
    WHERE ROWNUM <= {AppData.maxQueryCnt + 1}";

            subList(sql, ">=", "asc", parameters);
            if (ListKiji == null || ListKiji.Count == 0)
                ClientLib.MessageBoxOk(this, "データがありません");
        }
        void subList(string sql_onExec, string fugo, string sort, List<object> parameters)
        {

            var supplierCd = string.IsNullOrWhiteSpace(FindSupplierCd) ? "." : FindSupplierCd.Split(' ')[0];
            var sql = string.Format(sql_onExec, fugo, sort);
            var retData = AppData.Http?.AspxSqlQuery(sql, parameters.Select(p => p.ToString()).ToArray());
            if (retData == null || retData.Rows.Count == 0)
            {
                ListKiji = new ObservableCollection<MasterSHKiji>();
                CanNext = false;
                CanBack = false;
                return;
            }
            var list = (from DataRow dr in retData.Rows
                        select new MasterSHKiji
                        {
                            SeqNo = Convert.ToInt64(dr["SEQ_NO"]),
                            VdateCreate = Convert.ToDecimal(dr["VDATE_CREATE"]),
                            VdateUpdate = Convert.ToDecimal(dr["VDATE_UPDATE"]),
                            Product = dr["商品CD"].ToString() ?? string.Empty,
                            OldCD = dr["旧コード"].ToString() ?? string.Empty,
                            Abbreviation = dr["略称"].ToString() ?? string.Empty,
                            ProductName = dr["商品名"].ToString() ?? string.Empty,
                            CateCd = dr["区分CD"].ToString() ?? string.Empty,
                            SupplierCd = dr["仕入先CD"].ToString() ?? string.Empty + ' ' + dr["仕入先名"].ToString() ?? string.Empty,
                            SupplierProdCd = dr["仕入先商品CD"].ToString() ?? string.Empty,
                            UnitPrice = Convert.ToDecimal(dr["単価"]),
                            Memo = dr["メモ"].ToString() ?? string.Empty,
                            InpStaffCD = dr["最終修正者"].ToString() ?? string.Empty
                        }).OrderBy(c => c.Product).ThenBy(c => c.SupplierCd).ToList();

            // 🔹 Lookahead logic
            if (list.Count > AppData.maxQueryCnt)
            {
                // There is more data → enable Next
                list = list.Take(AppData.maxQueryCnt).ToList();
                CanNext = true;
            }
            else
            {
                // End reached
                CanNext = false;
            }

            Common.ConvertDotStringDel(list);
            ListKiji = new ObservableCollection<MasterSHKiji>(list);
            if (ListKiji.Count > 0)
            {
                SelectedKiji = ListKiji[0];
            }

        //    if (IsFirstPage)
        //        CanBack = false;
        //    else
        //        CanBack = true;
        //}
        }

        partial void OnSelectedKijiChanged(MasterSHKiji? value)
        {
            if (value != null)
                EditKiji = Common.CloneObject(value);
            else
                EditKiji = null;
        }

        void PageList(string fabricCd, string supplierCd, string fugo, string sort)
        {
            var conditions = new List<string>
    {
        $"A.商品CD {fugo} :1",
        $"A.仕入先CD {fugo} :2"
    };
            var parameters = new List<object> { fabricCd, supplierCd };

            string whereClause = string.Join(" AND ", conditions);

            string sql = $@"
SELECT *
FROM (
    SELECT A.SEQ_NO, A.VDATE_CREATE, A.VDATE_UPDATE,
           A.商品CD, A.旧コード, A.略称, A.商品名,
           A.区分CD, A.仕入先CD, A.仕入先商品CD,
           A.単価, A.メモ, B.仕入先名,
           (A.入力社員CD || ' ' ||
             (SELECT S.名前 FROM HC$MASTER_SHAIN S
              WHERE S.社員CD = A.入力社員CD)) 最終修正者
    FROM HC$Master_SHKIJI A
    LEFT JOIN HC$MASTER_SIIRE B ON A.仕入先CD = B.仕入先CD
    WHERE {whereClause}
    ORDER BY A.商品CD {sort}, A.仕入先CD {sort}
)
WHERE ROWNUM <= {AppData.maxQueryCnt + 1}";

            subList(sql, fugo, sort, parameters);

            if (ListKiji == null || ListKiji.Count == 0)
                ClientLib.MessageBoxOk(this, "データがありません");
        }


        [RelayCommand]
        void NextList()
        {
            if (ListKiji == null || ListKiji.Count == 0) return;

            // Use the last row as starting point
            var last = ListKiji.Last();
            var startFabric = last.Product;
            var startSupplier = last.SupplierCd.Split(' ')[0]; // because you join "cd name"

            PageList(startFabric, startSupplier, ">=", "asc");
        }

        [RelayCommand]
        void BackList()
        {
            if (ListKiji == null || ListKiji.Count == 0) return;

            // Use the first row as starting point
            var first = ListKiji.First();
            var startFabric = first.Product;
            var startSupplier = first.SupplierCd.Split(' ')[0];

            PageList(startFabric, startSupplier, "<=", "desc");
        }


        //[RelayCommand]
        //void DoInsert()
        //{
        //    if (!ClientLib.MessageBox(this, "新規登録しますか？")) return;
        //    var item = Common.CloneObject(EditKiji);
        //    Common.ConvertDotStringAdd(item);
        //    if (item == null) return;
        //    var ret = AppData.Http!.AspxSqlExe(DBDef.DB_DML.INSERT, "MASTER_SHAIN", 0, "0",
        //        new string[] { "社員CD", "名前", "部門", "店舗CD", "営業FLG", "メール", "携帯TEL", "特権FLG", "フリガナ", "役職CD", "就業FLG", "出力FLG", "備考", "入社日", "有給残", "給与区分", "給与支給額", "交通費区分", "交通費支給額", "部課CD", "名称CD01", "名称CD02", "名称CD03", "名称CD04", "名称CD05", "POS区分", "メールFLG", "入力社員CD", "特休残", "退勤日", "退職日", "プロフィール" },
        //        new string[] { item.WorkerCD!, item.Name!, item.Department!, item.ShopCD!, item.SalesFlg.ToString()!, item.Mail!, item.TelNo!, item.SpecialFlg!, item.Furigana!, item.PositionCD!, item.EmploymentFLG!, item.OutputFLG.ToString()!, item.Notes!, item.JoiningDate!, item.VacationRemaining!, item.SalaryCate!, item.SalaryAmount.ToString()!, item.TransExpCate!, item.TransExpAmount.ToString()!, item.SectionCD!, item.NameCD01!, item.NameCD02!, item.NameCD03!, item.NameCD04!, item.NameCD05!, item.PosCate.ToString()!, item.EmailFLG.ToString()!, item.EmployeeInpCD!, item.SpecHolidayRemain.ToString()!, item.EndDate!, item.RetireDate!, item.Profile! });
        //    if (ret.Code == 0)
        //    {
        //        item.SeqNo = ret.NewSeq;
        //        item.VdateUpdate = decimal.Parse(ret.VDate);
        //        item.VdateCreate = item.VdateUpdate;
        //        Common.ConvertDotStringDel(item);
        //        ListWorker!.Add(item);
        //        SelectedWorker = item;
        //    }
        //    else
        //    {
        //        ClientLib.MessageBoxError(this, ret.Code.ToString());
        //    }
        //}

        //[RelayCommand]
        //void DoUpdate()
        //{
        //    if (!ClientLib.MessageBox(this, "修正しますか？")) return;
        //    var item = Common.CloneObject(EditWorker);
        //    Common.ConvertDotStringAdd(item);
        //    if (item == null) return;
        //    var ret = AppData.Http!.AspxSqlExe(DBDef.DB_DML.UPDATE, "HASTER_SHAIN", item.SeqNo, item.VdateUpdate.ToString(),
        //        new string[] { "社員CD", "名前", "部門", "店舗CD", "営業FLG", "メール", "携帯TEL", "特権FLG", "フリガナ", "役職CD", "就業FLG", "出力FLG", "備考", "入社日", "有給残", "給与区分", "給与支給額", "交通費区分", "交通費支給額", "部課CD", "名称CD01", "名称CD02", "名称CD03", "名称CD04", "名称CD05", "POS区分", "メールFLG", "入力社員CD", "特休残", "退勤日", "退職日", "プロフィール" },
        //        new string[] { item.WorkerCD!, item.Name!, item.Department!, item.ShopCD!, item.SalesFlg.ToString()!, item.Mail!, item.TelNo!, item.SpecialFlg!, item.Furigana!, item.PositionCD!, item.EmploymentFLG!, item.OutputFLG.ToString()!, item.Notes!, item.JoiningDate!, item.VacationRemaining!, item.SalaryCate!, item.SalaryAmount.ToString()!, item.TransExpCate!, item.TransExpAmount.ToString()!, item.SectionCD!, item.NameCD01!, item.NameCD02!, item.NameCD03!, item.NameCD04!, item.NameCD05!, item.PosCate.ToString()!, item.EmailFLG.ToString()!, item.EmployeeInpCD!, item.SpecHolidayRemain.ToString()!, item.EndDate!, item.RetireDate!, item.Profile! });
        //    if (ret.Code == 0)
        //    {
        //        Common.ConvertDotStringDel(item);
        //        if (SelectedWorker != null)
        //        {
        //            SelectedWorker.VdateUpdate = decimal.Parse(ret.VDate);
        //            SelectedWorker.WorkerCD = item.WorkerCD;
        //            SelectedWorker.Name = item.Name;
        //            SelectedWorker.Department = item.Department;
        //            SelectedWorker.ShopCD = item.ShopCD;
        //            SelectedWorker.SalesFlg = item.SalesFlg;
        //            SelectedWorker.Mail = item.Mail;
        //            SelectedWorker.TelNo = item.TelNo;
        //            SelectedWorker.SpecialFlg = item.SpecialFlg;
        //            SelectedWorker.Furigana = item.Furigana;
        //            SelectedWorker.PositionCD = item.PositionCD;
        //            SelectedWorker.EmploymentFLG = item.EmploymentFLG;
        //            SelectedWorker.OutputFLG = item.OutputFLG;
        //            SelectedWorker.Notes = item.Notes;
        //            SelectedWorker.JoiningDate = item.JoiningDate;
        //            SelectedWorker.VacationRemaining = item.VacationRemaining;
        //            SelectedWorker.SalaryCate = item.SalaryCate;
        //            SelectedWorker.SalaryAmount = item.SalaryAmount;
        //            SelectedWorker.TransExpCate = item.TransExpCate;
        //            SelectedWorker.TransExpAmount = item.TransExpAmount;
        //            SelectedWorker.SectionCD = item.SectionCD;
        //            SelectedWorker.NameCD01 = item.NameCD01;
        //            SelectedWorker.NameCD02 = item.NameCD02;
        //            SelectedWorker.NameCD03 = item.NameCD03;
        //            SelectedWorker.NameCD04 = item.NameCD04;
        //            SelectedWorker.NameCD05 = item.NameCD05;
        //            SelectedWorker.PosCate = item.PosCate;
        //            SelectedWorker.EmailFLG = item.EmailFLG;
        //            SelectedWorker.EmployeeInpCD = item.EmployeeInpCD;
        //            SelectedWorker.SpecHolidayRemain = item.SpecHolidayRemain;
        //            SelectedWorker.EndDate = item.EndDate;
        //            SelectedWorker.RetireDate = item.RetireDate;
        //            SelectedWorker.Profile = item.Profile;
        //            EditWorker = Common.CloneObject(SelectedWorker);
        //        }

        //    }
        //    else
        //    {
        //        ClientLib.MessageBoxError(this, ret.Code.ToString());
        //    }
        //}

        //[RelayCommand]
        //void DoDelete()
        //{
        //    if (!ClientLib.MessageBox(this, "削除しますか？")) return;
        //    if (EditWorker == null) return;
        //    var ret = AppData.Http!.AspxSqlExe(DBDef.DB_DML.DELETE, "Master_MEISHO", EditWorker.SeqNo, EditWorker.VdateUpdate.ToString(),
        //        new string[0], new string[0]);
        //    if (ret.Code == 0)
        //    {
        //        if (SelectedWorker != null)
        //        {
        //            ListWorker!.Remove(SelectedWorker);
        //            var item = ListWorker.Where(c => c.WorkerCD == ListWorker.Min(c => c.WorkerCD)).FirstOrDefault();
        //            SelectedWorker = item;
        //        }
        //    }
        //    else
        //    {
        //        ClientLib.MessageBoxError(this, ret.Code.ToString());
        //    }
        //}

        //[RelayCommand]
        //async Task DoPrintAsync()
        //{
        //    if (!ClientLib.MessageBox(this, "印刷しますか？")) return;
        //    ClientLib.CursorToWait();
        //    if (ListWorker == null || ListWorker.Count == 0) return;
        //    var paramNames = ListWorker.Select((c, i) => $":p{i}||''").ToList();


        //    // Build SQL dengan placeholder
        //    var sql = printsql +
        //              $" where TO_CHAR(A.社員CD) in ({string.Join(",", paramNames)}) order by A.社員CD)";

        //    // Build parameter values
        //    var parameters = ListWorker.Select(c => c.WorkerCD).ToArray();

        //    // Execute with parameters
        //    var ret = AppData.Http!.AspxSqlQueryCsv(sql, parameters, "cvnet_shain.qfm");
        //    if (ret.Split('\n').Length < 2)
        //    {
        //        ClientLib.MessageBoxError(this, "PDFデータがありません");
        //        return;
        //    }
        //    var ret1 = ret.Split('\n');
        //    var url = AppData.Http.URLroot + ret1[0] + "/data.pdf";
        //    await Task.Delay(1500); // PDF生成待ち
        //    var win = new WebpdfView();
        //    var vm = win.DataContext as WebpdfViewModel;
        //    if (vm == null) return;
        //    vm.Pdfdata = url;
        //    ClientLib.CursorToNormal();
        //    ClientLib.ShowDialogView(win, this);
        //}




    }
}

