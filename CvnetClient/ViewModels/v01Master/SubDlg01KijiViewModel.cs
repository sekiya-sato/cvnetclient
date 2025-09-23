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

        [RelayCommand]
        void Init()
        {
            editKiji = new MasterSHKiji();

            var kubunCdList = new Dictionary<string, string>();
            string sql_query = "select A.名称CD,A.名称 from HC$Master_MEISHO a  where  a.名称区分='KFK' order by a.名称CD";
            var get_kubun = AppData.Http?.AspxSqlQuery(sql_query, null);
            foreach (DataRow row in get_kubun.Rows)
            {
                string? key = row[0] != DBNull.Value ? row[0].ToString() : string.Empty;
                string? value = row[1] != DBNull.Value ? row[1].ToString() : string.Empty;
                kubunCdList.Add(key, string.Format("{0} {1}", key, value));
            }
            SelectKubun = kubunCdList;
            editKiji.CateCd = SelectKubun.FirstOrDefault().Key;
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
            if (!string.IsNullOrEmpty(findFabricCd)) {
                conditions.Add("A.商品CD{0}:1");
                parameters.Add(findFabricCd);
            }
            if (!string.IsNullOrEmpty(findSupplierCd)) {
                conditions.Add("A.仕入先CD=:2");
                parameters.Add(findSupplierCd);
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
    WHERE ROWNUM <= {AppData.maxQueryCnt}";
            
            subList(sql,">=","asc",parameters);
            if (listKiji == null || listKiji.Count == 0)
                ClientLib.MessageBoxOk(this, "データがありません");
        }
        void subList(string sql_onExec, string fugo,string sort, List<object> parameters)
        {

            var supplierCd = string.IsNullOrWhiteSpace(findSupplierCd)? ".": findSupplierCd.Split(' ')[0];
            var sql = string.Format(sql_onExec, fugo, sort);
            var retData = AppData.Http?.AspxSqlQuery(sql, parameters.Select(p => p.ToString()).ToArray());
            if (retData == null || retData.Rows.Count == 0) return;
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
            Common.ConvertDotStringDel(list);
            listKiji = new ObservableCollection<MasterSHKiji>(list);
            if (listKiji.Count > 0)
            {
                selectedKiji = listKiji[0];
            }
        }


    }
}

