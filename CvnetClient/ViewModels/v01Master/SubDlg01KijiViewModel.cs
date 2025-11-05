using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CvnetBaseCore;
using System.Data;
using CvnetClient.Models;
using CvnetClient.Views;
using System.Collections.ObjectModel;
using CvnetClient.Utils;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg01KijiViewModel : BaseViewModel
    {

        [ObservableProperty]
        public Dictionary<string, string>? m_selectKubun;

        [ObservableProperty]
        MasterSHKiji? selectedKiji;

        //[ObservableProperty]
        //string? supplierName;

        [ObservableProperty]
        MasterSHKiji? editKiji;

        [ObservableProperty]
        string? findFabricCd;

        [ObservableProperty]
        BtListHelper? findSupplierCd = new();

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
        public void OnInit()
        {
            EditKiji = new MasterSHKiji();
            FindSupplierCd = new BtListHelper();

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
            if (!string.IsNullOrEmpty(FindSupplierCd?.Code))
            {
                conditions.Add("A.仕入先CD=:2");
                parameters.Add(FindSupplierCd?.Code);
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

            var supplierCd = string.IsNullOrWhiteSpace(FindSupplierCd?.Code) ? "." : FindSupplierCd?.Code;
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
                            //SupplierCd = dr["仕入先CD"].ToString() ?? string.Empty,
                            SupplierName = dr["仕入先名"].ToString() ?? string.Empty,
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


        [RelayCommand]
        void DoInsert()
        {
            if (!ClientLib.MessageBox(this, "新規登録しますか？")) return;
            var item = Common.CloneObject(EditKiji);
            Common.ConvertDotStringAdd(item);
            if (item == null) return;
            var ret = AppData.Http!.AspxSqlExe(DBDef.DB_DML.INSERT, "Master_SHKIJI", 0, "0",
                new string[] { "商品CD", "旧コード", "略称", "商品名", "区分CD", "仕入先CD", "仕入先商品CD", "単価", "メモ", "入力社員CD" },
                new string[] { item.Product!, item.OldCD!, item.Abbreviation!, item.ProductName!, item.CateCd!, item.SupplierCd!, item.SupplierProdCd!, item.UnitPrice.ToString()!, item.Memo!, AppData.ClassSatoo.SHAIN_CD });

            if (ret.Code == 0)
            {
                item.SeqNo = ret.NewSeq;
                item.VdateUpdate = decimal.Parse(ret.VDate);
                item.VdateCreate = item.VdateUpdate;
                Common.ConvertDotStringDel(item);
                ListKiji!.Add(item);
                SelectedKiji = item;
                ClientLib.MessageBoxOk(this, "登録しました");
            }
            else
            {
                ClientLib.MessageBoxError(this, ret.Code.ToString());
            }
        }

        [RelayCommand]
        void DoUpdate()
        {
            if (!ClientLib.MessageBox(this, "修正しますか？")) return;
            var item = Common.CloneObject(EditKiji);
            Common.ConvertDotStringAdd(item);
            if (item == null) return;
            var ret = AppData.Http!.AspxSqlExe(DBDef.DB_DML.UPDATE, "Master_SHKIJI", item.SeqNo, item.VdateUpdate.ToString(),
                new string[] { "商品CD", "旧コード", "略称", "商品名", "区分CD", "仕入先CD", "仕入先商品CD", "単価", "メモ", "入力社員CD"},
                new string[] { item.Product!, item.OldCD!, item.Abbreviation!, item.ProductName!,item.CateCd! ,item.SupplierCd!, item.SupplierProdCd!, item.UnitPrice.ToString()!, item.Memo!, AppData.ClassSatoo.SHAIN_CD});
            if (ret.Code == 0)
            {
                Common.ConvertDotStringDel(item);
                if (SelectedKiji != null)
                {
                    SelectedKiji.VdateUpdate = decimal.Parse(ret.VDate);
                    SelectedKiji.Product = item.Product;
                    SelectedKiji.OldCD = item.OldCD;
                    SelectedKiji.Abbreviation = item.Abbreviation;
                    SelectedKiji.ProductName = item.ProductName;
                    SelectedKiji.CateCd = item.CateCd;
                    SelectedKiji.SupplierCd = item.SupplierCd;
                    SelectedKiji.SupplierProdCd = item.SupplierProdCd;
                    SelectedKiji.UnitPrice = item.UnitPrice;
                    SelectedKiji.Memo = item.Memo;
                    SelectedKiji.InpStaffCD = item.InpStaffCD;
                    EditKiji = Common.CloneObject(SelectedKiji);
                    ClientLib.MessageBoxOk(this,"修正しました");
                }

            }
            else
            {
                ClientLib.MessageBoxError(this, ret.Code.ToString());
            }
        }

        [RelayCommand]
        void DoDelete()
        {
            if (!ClientLib.MessageBox(this, "削除しますか？")) return;
            if (EditKiji == null) return;
            var ret = AppData.Http!.AspxSqlExe(DBDef.DB_DML.DELETE, "Master_SHKIJI", EditKiji.SeqNo, EditKiji.VdateUpdate.ToString(),
                new string[0], new string[0]);
            if (ret.Code == 0)
            {
                if (SelectedKiji != null)
                {
                    ListKiji!.Remove(SelectedKiji);
                    var item = ListKiji.Where(c => c.Product == ListKiji.Min(c => c.Product)).FirstOrDefault();
                    SelectedKiji = item;
                    ClientLib.MessageBoxOk(this, "削除しました");
                }
            }
            else
            {
                ClientLib.MessageBoxError(this, ret.Code.ToString());
            }
        }

        string printsql = """
				select A.SEQ_NO,SUBSTR(GET_VDATE(a.VDATE_CREATE),0,8)||SUBSTR(GET_VDATE(a.VDATE_CREATE),10,6) 作成日時
				,SUBSTR(GET_VDATE(a.VDATE_UPDATE),0,8)||SUBSTR(GET_VDATE(a.VDATE_UPDATE),10,6) 更新日時
				,A.商品CD,A.旧コード,A.略称,A.商品名,A.区分CD,A.仕入先CD,A.仕入先商品CD,A.単価
				,B.仕入先名 ,(A.入力社員CD ||' '|| (select S.名前 from HC$MASTER_SHAIN S where S.社員CD=A.入力社員CD)) 最終修正者,{0} 区分CD名
				from HC$Master_SHKIJI A, HC$MASTER_SIIRE B where A.商品CD in ({1}) and A.仕入先CD=B.仕入先CD(+)  order by A.商品CD,A.仕入先CD
				""";
        /// <summary>
        /// PDF印刷
        /// </summary>

        [RelayCommand]
        async Task DoPrintAsync()
        {
            if (!ClientLib.MessageBox(this, "印刷しますか？")) return;
            ClientLib.CursorToWait();
            if (ListKiji == null || ListKiji.Count == 0) return;
            //var paramNames = ListKiji.Select((c, i) => $":p{i}||''").ToList();
            var getCateKiji = AppData.ClassCvnet.comboItem00.GetCaseStr("生地付属", "A.区分CD");
            var getProductCD = string.Join(",", ListKiji.Select(c => $"'{c.Product}'"));


            // Build SQL dengan placeholder
            var sql = string.Format(printsql, getCateKiji, getProductCD);

            // Build parameter values
            var parameters = ListKiji.Select(c => c.Product).ToArray();

            // Execute with parameters
            var ret = AppData.Http!.AspxSqlQueryCsv(sql, null, "cvnet_kiji.qfm");
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

        [RelayCommand]
        public void SelectSupplier(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && FindSupplierCd != null)
            { 
                FindSupplierCd = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }

        [RelayCommand]
        public void SelectSupplier1(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && EditKiji != null)
            {
                EditKiji.SupplierCd = get_sel00.Code;
                //SupplierName = get_sel00.Name;
                EditKiji.SupplierName = get_sel00.Name;
            }
        }


    }
}

