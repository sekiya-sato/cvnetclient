using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Models;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg80gphSelViewModel : BaseViewModel
    {
        [ObservableProperty]
        string[] wrk_para = null;

        [ObservableProperty]
        string dlg_title = string.Empty;

        bool is1stInit = true;
        List<SubDlg80gphSelModel>? listSelOri;

        [ObservableProperty]
        List<SubDlg80gphSelModel>? listSel00;

        [ObservableProperty]
        SubDlg80gphSelModel? selectSel00;
         
        void Init(string[] init_para = null, string v_hugo = "", string v_sort = "")
        {
            if (init_para == null) init_para = wrk_para; 
            if (v_hugo == "") v_hugo = ">=";
            string sql_query = " select 名称CD,名称,略称 from  hc$master_meisho ";
            sql_query += " where 名称区分=:1 and 名称CD" + v_hugo + ":2 order by 名称CD " + v_sort;
            string[] v_para = new string[2];
            if (init_para.Length > 0)
            {
                if (init_para[0] == "HC$MASTER_MEISHO")
                {
                    v_para = new string[2];
                    dlg_title = "名称を選択して下さい";
                    v_para[0] = init_para[1];
                    v_para[1] = init_para[2];
                }
                else if (init_para[0] == "HC$MASTER_TOKUI")
                {
                    v_para = new string[1];
                    sql_query = " select 得意先CD,得意先名,カナ from  hc$master_tokui ";
                    sql_query += " where 得意先CD" + v_hugo + ":1 ";
                    if (init_para.Length > 2 && init_para[3] != null) sql_query += init_para[3];
                    sql_query += " order by 得意先CD " + v_sort;
                    v_para[0] = init_para[2];
                }
                else if (init_para[0] == "HC$MASTER_SIIRE")
                {
                    v_para = new string[1];
                    sql_query = "select 仕入先CD,仕入先名,カナ from  hc$master_siire ";
                    sql_query += " where 仕入先CD" + v_hugo + ":1 ";
                    if (init_para.Length > 2 && init_para[3] != null) sql_query += init_para[3];
                    sql_query += " order by 仕入先CD " + v_sort;
                    v_para[0] = init_para[2];
                }
                else if (init_para[0] == "HC$MASTER_SHOHIN")
                {
                    v_para = new string[1];
                    sql_query = "select 商品CD,商品名,略称 from  hc$master_shohin ";
                    sql_query += " where 商品CD" + v_hugo + ":1 order by 商品CD " + v_sort;
                    v_para[0] = init_para[2];
                }
                else if (init_para[0] == "HC$MASTER_SHAIN")
                {
                    v_para = new string[1];
                    sql_query = "select 社員CD,名前 from  hc$master_shain ";
                    sql_query += " where 社員CD" + v_hugo + ":1 and 営業FLG = 1 order by 社員CD " + v_sort;
                    v_para[0] = init_para[2];
                }
                else if (init_para[0] == "HC$MASTER_KOKYAKU")
                {
                    v_para = new string[1];
                    sql_query = "select 顧客CD,顧客名 from  hc$master_kokyaku ";
                    sql_query += " where 顧客CD" + v_hugo + ":1 order by 顧客CD " + v_sort;
                    v_para[0] = init_para[2];
                }
                sql_query = cvnet.GetSqlDisp(sql_query);
                AppData.Http?.AspxSqlQuery(sql_query, v_para);
            }
        }

        [RelayCommand]
        void PrevList()
        {

        }

        [RelayCommand]
        void NextList()
        {

        }

        [RelayCommand]
        void TopList()
        {
            
        }

        [RelayCommand]
        void NameSearch()
        {
            
        }

        [RelayCommand]
        void DoSearch()
        { 
        
        }

        [RelayCommand]
        void DoExit()
        { 
        
        }
    }
}
