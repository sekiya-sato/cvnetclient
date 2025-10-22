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
            SearchCond.ItemFrom = "zzzzzzzzzzzzzz";
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

            if (wrk_para[3] == "")
            {
                wrk_para[3] = ".";
            }

            if (wrk_para[4] == "")
            {
                wrk_para[3] = "zzzzzzzzzzzzzz";
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
                            ItemName = dr["名称"].ToString() ?? string.Empty
                        }).ToList();
            Common.ConvertDotStringDel(list);
            BudgetList = new ObservableCollection<Budget>(list);

        }

        [RelayCommand]
        void DoSum() 
        { 
        
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
    }

    public partial class Search : ObservableObject 
    {
        [ObservableProperty]
        public DateOnly? date;
        [ObservableProperty]
        public string? brd;
        [ObservableProperty]
        public string? itemFrom;
        [ObservableProperty]
        public string? itemTo;
    }
}
