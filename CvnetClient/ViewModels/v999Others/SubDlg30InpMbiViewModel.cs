using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using static CvnetClient.ViewModels.SubDlg01MeiConvViewModel;
using static CvnetClient.ViewModels.SubDlg80GphABC2aViewModel;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg30InpMbiViewModel : BaseViewModel
    {
        [ObservableProperty]
        ObservableCollection<Budget>? budgetList;
        [ObservableProperty]
        public int? sum;
        [ObservableProperty]
        Search? searchCond;

        public void OnInit() {
            SearchCond = new Search();
            SearchCond.ItemTo = "zzzzzzzzzzzzzz";
            //SearchCond.Date = DateOnly.TryParse(DateTime.Now.ToString(), out null);
        }

        [RelayCommand]
        void DoSearch() {
            int strFLG = 0;
            string strSql = "";
            //var ret_csv = null;
            string[] wrk_para = new string[5];
            wrk_para[0] = SearchCond?.Date?.ToString("yyyyMMdd");
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

            strSql = "SELECT A.名称CD,A.名称,nvl(B.予算数量,0) AS su ,nvl(B.予算金額,0) AS kin ";
            strSql += "FROM HC$MASTER_MEISHO A LEFT OUTER JOIN HC$MASTER_YO_ITEM B ";
            strSql += " ON A.名称CD = B.アイテムCD ";
            strSql += "WHERE A.名称区分 = 'ITM'";
            strSql += " AND A.ランク != '.'";
            strSql += " AND 店舗CD = '" + wrk_para[1] + "'";
            strSql += " AND 日付 = '" + wrk_para[0] + "'";
            strSql += " AND ブランドCD = '" + wrk_para[2] + "'";
            strSql += " AND アイテムCD between '" + wrk_para[3] + "' and '" + wrk_para[4] + "'";
            strSql += " union";
            strSql += " SELECT 名称CD,名称,0 AS su ,0 AS kin ";
            strSql += "FROM HC$MASTER_MEISHO ";
            strSql += "WHERE 名称区分 = 'ITM'";
            strSql += " AND ランク != '.'";
            strSql += " AND 名称CD between '" + wrk_para[3] + "' and '" + wrk_para[4] + "'";

            string strSql2 = "";
            strSql2 = " select A.名称CD, A.名称, sum(A.su) su, sum(A.kin) kin ";
            strSql2 += " from (" + strSql + ") a";
            strSql2 += " group by a.名称CD, a.名称";
            strSql2 += " order by 名称CD ";

            strSql = strSql2;

            var ret_csv = AppData.Http!.AspxSqlQuery(strSql);

            var list = (from DataRow dr in ret_csv.Rows
                        select new Budget
                        {
                            ItemCD = dr["名称CD"].ToString() ?? string.Empty,
                            ItemName = dr["名称"].ToString() ?? string.Empty,
                            BudgetPrice = int.TryParse(dr["kin"].ToString(), out var _budgetPrice) ? _budgetPrice : 0,
                            Quantity = int.TryParse(dr["su"].ToString(), out var _quantity) ? _quantity : 0
                        }).ToList();
            Common.ConvertDotStringDel(list);
            BudgetList = new ObservableCollection<Budget>(list);
            foreach (var item in BudgetList)

                item.PropertyChanged += (_, e) =>
                {
                    if (e.PropertyName == nameof(Budget.BudgetPrice))
                        UpdateSum();
                };

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
            var v_para = new string[3];
            v_para[0] = "Master_YO_Item";
            v_para[1] = "";
            v_para[2] = "4";
            var ret_csv = AppData.Http!.AspxSqlQuery2("mi_csv", v_para);
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
    }

    public partial class Budget : ObservableObject 
    {
        [ObservableProperty]
        public string? itemCD;
        [ObservableProperty]
        public string? itemName;
        [ObservableProperty]
        public int? budgetPrice;
        [ObservableProperty]
        public int? quantity;
    }

    public partial class Search : ObservableObject 
    {
        [ObservableProperty]
        public DateOnly? date;
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
