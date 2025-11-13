using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Models;
using CvnetClient.Utils;
using System.Collections.ObjectModel;
using System.Data;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg30InpMbiViewModel : BaseViewModel
    {
        #region Declare
        [ObservableProperty]
        ObservableCollection<Budget>? budgetList;
        [ObservableProperty]
        public int? sum;
        [ObservableProperty]
        Search? searchCond;
        #endregion
        #region Initialize
        public void OnInit(object? init_para = null, string? init_flg = null) {
            if (init_para != null)
            {
                OnInitBase(init_para, init_flg);
                SearchCond = new Search();
            SearchCond.Date = DateTime.Now;
            SearchCond.ItemTo = "zzzzzzzzzzzzzz";
            //SearchCond.Date = DateOnly.TryParse(DateTime.Now.ToString(), out null);
        }
        #endregion
        #region Function
        [RelayCommand]
        void DoSearch() {
            int strFLG = 0;
            string strSql = "";
            //var ret_csv = null;
            string[] wrk_para = new string[5];
            wrk_para[0] = SearchCond?.Date?.ToString("yyyyMM") + "01";
            wrk_para[1] = ".";
            wrk_para[2] = SearchCond.Brd;
            wrk_para[3] = SearchCond.ItemFrom;
            wrk_para[4] = SearchCond.ItemTo;

            if (wrk_para[3] == null)
            {
                wrk_para[3] = ".";
            }

            if (wrk_para[4] == null)
            {
                wrk_para[4] = "zzzzzzzzzzzzzz";
            }
            string worker = AppData.ClassSatoo.SHAIN_CD;
            if (worker == null || worker == "")
            {
                worker = ".";
            } 
            strSql = "SELECT A.名称CD,A.名称,nvl(B.予算数量,0) AS su ,nvl(B.予算金額,0) AS kin,";
            strSql += "'"+ wrk_para[1] + "' 店舗CD,'"+ wrk_para[0] + "' 日付,'"+ wrk_para[2] + "' ブランドCD,";

            strSql += "'"+AppData.ClassSatoo.SHAIN_CD+ "' 入力社員CD ";
            strSql += "FROM HC$MASTER_MEISHO A LEFT OUTER JOIN HC$MASTER_YO_ITEM B ";
            strSql += " ON A.名称CD = B.アイテムCD ";
            strSql += "WHERE A.名称区分 = 'ITM'";
            strSql += " AND A.ランク != '.'";
            strSql += " AND 店舗CD = '" + wrk_para[1] + "'";
            strSql += " AND 日付 = '" + wrk_para[0] + "'";
            strSql += " AND ブランドCD = '" + wrk_para[2] + "'";
            strSql += " AND アイテムCD between '" + wrk_para[3] + "' and '" + wrk_para[4] + "'";
            strSql += " union";
            strSql += " SELECT 名称CD,名称,0 AS su ,0 AS kin, ";
            strSql += "'" + wrk_para[1] + "' 店舗CD,'" + wrk_para[0] + "' 日付,'" + wrk_para[2] + "' ブランドCD,";
            strSql += "'" + AppData.ClassSatoo.SHAIN_CD + "' 入力社員CD ";
            strSql += "FROM HC$MASTER_MEISHO ";
            strSql += "WHERE 名称区分 = 'ITM'";
            strSql += " AND ランク != '.'";
            strSql += " AND 名称CD between '" + wrk_para[3] + "' and '" + wrk_para[4] + "'";

            string strSql2 = "";
            strSql2 = " select A.名称CD, A.名称, sum(A.su) su, sum(A.kin) kin,A.店舗CD,A.日付,A.ブランドCD,A.入力社員CD ";
            strSql2 += " from (" + strSql + ") a";
            strSql2 += " group by a.名称CD, a.名称,A.店舗CD,A.日付,A.ブランドCD,A.入力社員CD";
            strSql2 += " order by 名称CD ";

            strSql = strSql2;

            var ret_csv = AppData.Http!.AspxSqlQuery(strSql);

            var list = (from DataRow dr in ret_csv.Rows
                        select new Budget
                        {
                            ShopCD = dr["店舗CD"].ToString() ?? string.Empty,
                            BrdCD = dr["ブランドCD"].ToString() ?? string.Empty,
                            ItemCD = dr["名称CD"].ToString() ?? string.Empty,
                            ItemName = dr["名称"].ToString() ?? string.Empty,
                            Date = dr["日付"].ToString() ?? string.Empty,
                            Quantity = int.TryParse(dr["su"].ToString(), out var _quantity) ? _quantity : 0,
                            BudgetPrice = int.TryParse(dr["kin"].ToString(), out var _budgetPrice) ? _budgetPrice : 0,
                            InpName = dr["入力社員CD"].ToString() ?? string.Empty
                            
                        }).ToList();
            Common.ConvertDotStringDel(list);
            BudgetList = new ObservableCollection<Budget>(list);
            foreach (var item in BudgetList)
            {
                item.InpName = ".";
                item.PropertyChanged += (_, e) =>
                {
                    if (e.PropertyName == nameof(Budget.BudgetPrice))
                        UpdateSum();
                };
            }
            BudgetList.CollectionChanged += (_, __) => UpdateSum();

            UpdateSum();
        }
        private void UpdateSum()
        {
            Sum = BudgetList?.Sum(x => x.BudgetPrice ?? 0) ?? 0;
        }
        [RelayCommand]
        void DoSum() 
        {
            if (BudgetList == null || BudgetList.Count == 0) return;
            var excludeColumns = new[] { nameof(Budget.ItemName) };
            var props = typeof(Budget).GetProperties()
                              .Where(p => !excludeColumns.Contains(p.Name))
                              .ToList();
            DataTable dt = new DataTable("Budget");
            foreach (var prop in props)
                dt.Columns.Add(prop.Name);

            foreach (var b in BudgetList)
            {
                var row = dt.NewRow();
                foreach (var prop in props)
                {
                    row[prop.Name] = prop.GetValue(b) ?? DBNull.Value;
                }
                dt.Rows.Add(row);
            }
            string colHeader = "店舗CD, ブランドCD, アイテムCD, 日付, 予算数量, 予算金額, 入力社員CD";
            var csv_para = new BizCsvDocument(dt);
            csv_para!.SetColHeader(colHeader);
            
            var v_para = new string[3];
            v_para[0] = "Master_YO_Item";
            v_para[1] = csv_para!.SaveStr(0);
            v_para[2] = "4";
            var ret_csv = AppData.Http!.AspxSqlQuery2("mi_csv", v_para,"",-1);

            if (ret_csv.Split('\n')[0].Trim() != "0")
            {
                ClientLib.MessageBoxError(this, "登録に失敗しました。");
            }
            else
            {
                ClientLib.MessageBoxOk(this, "登録しました。");
            }
        }
        [RelayCommand]
        public void SelItem1(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && SearchCond != null)
            {
                SearchCond.ItemFrom = get_sel00.Code;
                SearchCond.ItemFromName = get_sel00.Name;
            }
        }
        [RelayCommand]
        public void SelItem2(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && SearchCond != null)
            {
                SearchCond.ItemTo = get_sel00.Code;
                SearchCond.ItemToName = get_sel00.Name;
            }
        }
        [RelayCommand]
        public void SelBrand(object value) 
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && SearchCond != null)
            {
                SearchCond.Brd = get_sel00.Code;
                SearchCond.BrdName = get_sel00.Name;
            }
        }
        #endregion
        public partial class Budget : ObservableObject
        {
            [ObservableProperty]
            public string? shopCD;
            [ObservableProperty]
            public string? brdCD;
            [ObservableProperty]
            public string? itemCD;
            [ObservableProperty]
            public string? itemName;
            [ObservableProperty]
            public string? date;
            [ObservableProperty]
            public int? quantity;
            [ObservableProperty]
            public int? budgetPrice;
            [ObservableProperty]
            public string? inpName;
        }
        public partial class Search : ObservableObject
        {
            [ObservableProperty]
            public DateTime? date;
            [ObservableProperty]
            public string? brd;
            [ObservableProperty]
            public string? brdName;
            [ObservableProperty]
            public string? itemFrom;
            [ObservableProperty]
            public string? itemTo;
            [ObservableProperty]
            public string? itemFromName;
            [ObservableProperty]
            public string? itemToName;
        }
    }   
}
