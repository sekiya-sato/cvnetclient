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
        MasterSHKiji? kijiMaster;

        [ObservableProperty]
        string? findFabricCd;

        [ObservableProperty]
        string? findSupplierCd;

        [RelayCommand]
        void Init()
        {
            KijiMaster = new MasterSHKiji();

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
            KijiMaster.CateCd = SelectKubun.FirstOrDefault().Key;
        }
    }
}

